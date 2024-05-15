using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitUtilities;
using Serilog;
using System.Text;
using TransportRequestService.Entities;
using TransportRequestService.Repositories;
using TransportRequestService.Requests;

namespace TransportRequestService.RequestHandler
{
    public class TransportRequestHandler : ConsumerServiceBase
    {
        private readonly IDbContextFactory<PostgresRepository> _repositoryFactory;
        private readonly PublisherServiceBase _publisher;

        public TransportRequestHandler(ILogger logger, IConfiguration config, IConnectionFactory connectionFactory, PublisherServiceBase publisher, IDbContextFactory<PostgresRepository> repositoryFactory)
            : base(logger, connectionFactory, config.GetSection("transportConsumer").Get<RabbitUtilities.Configuration.ConsumerConfig>()!)
        {
            _repositoryFactory= repositoryFactory;
            _publisher = publisher;


            using var repository = _repositoryFactory.CreateDbContext();

            //repository.Database.EnsureDeleted();
            repository.Database.EnsureCreated();
            repository.Transports.Add(new Transport() { OriginCountry = "Polska", OriginCity = "Gdańsk",
                DestinationCountry = "Niemcy", DestinationCity = "Berlin",
                PricePerTicket = 3.21M, SeatsNumber = 1000, SeatsTaken = 0, Type = "Plane", DepartureDate = DateTime.UtcNow, ArrivalDate = DateTime.UtcNow.AddDays(2) });

            repository.SaveChanges();

        }

        protected override void ConsumeMessage(object model, BasicDeliverEventArgs ea)
        {
            var headers = ea.BasicProperties.Headers;

            if (!headers.TryGetValue("Type", out object? typeObj))
                return;
            var type = (MessageType)Enum.Parse(typeof(MessageType), ASCIIEncoding.ASCII.GetString((byte[])typeObj));

            if (!headers.TryGetValue("Date", out object? dateObj))
                return;
            DateTime.TryParse(ASCIIEncoding.ASCII.GetString((byte[])dateObj), out var date);


            switch (type)
            {
                case MessageType.RESERVE:
                    Reserve(ea);
                    break;
                case MessageType.RELEASE:
                    Release(ea);
                    break;
                default:
                    _logger.Information($"Received message with unknown type.");
                    break;
            }
        }



        private async void Reserve(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = MessagePackSerializer.Deserialize<ReserveRequest>(body);
            bool accepted = false;
            bool success = false;

            int tries = 0;
            while (!success) 
            {
                var repository = _repositoryFactory.CreateDbContext(); 
                var transportEvents = await repository.TransportEvents.Where(t => t.TransportId == message.Id).OrderBy(t => t.SequenceNumber).ToListAsync();
                var transport = await repository.Transports.Where(t => t.Id == message.Id).FirstAsync();

                int seats = transport.SeatsTaken;
                decimal price = transport.PricePerTicket;
                foreach (var transportEvent in transportEvents)
                {
                    seats += transportEvent.SeatsChange;
                    price += transportEvent.PriceChange;
                }

                int lastInSequence = transportEvents.LastOrDefault()?.SequenceNumber ?? 0; //transportEvents.Count();
                accepted = seats + message.Seats <= transport.SeatsNumber && message.Seats > 0;
                if (accepted)
                {
                    tries += 1;
                    try
                    {
                        if (repository.TransportEvents.Any(e => e.TransportId == transport.Id && e.SequenceNumber == lastInSequence + 1))
                        {
                            success = false;
                            _logger.Warning($"Event for transport {message.Id} with same sequence number {lastInSequence + 1} already exists, trying again [tryNum:{tries}]");
                            repository.Dispose();
                            await Task.Delay(1);
                            continue;
                        }
                        repository.TransportEvents.Add(new TransportEvent() { TransportId = transport.Id, SequenceNumber = lastInSequence + 1, SeatsChange = message.Seats });
                        await repository.SaveChangesAsync();
                        repository.Dispose();
                    }
                    catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
                    {
                        success = false;
                        _logger.Warning($"Event for transport {message.Id} with same sequence number {lastInSequence + 1} already exists, trying again [tryNum:{tries}]");
                        repository.Dispose();
                        await Task.Delay(1);
                        continue;
                    }
                    _logger.Information($"Accepted reservation for transport {message.Id} for {message.Seats} seats [seq:{lastInSequence + 1}] [tryNum:{tries}]");
                    _publisher.PublishToFanoutNoReply("event", MessageType.EVENT, $"Reservation for transport {message.Id} was made!");
                }
                else
                {
                    _logger.Information($"Rejected reservation for transport {message.Id}, not enough seats");
                }
                success = true;
            }
            Reply(ea, MessagePackSerializer.Serialize<bool>(accepted));
        }

        private async void Release(BasicDeliverEventArgs ea)//As message body use id of event to "delete" => get event and create new event with negative values
        {
            var body = ea.Body.ToArray();
            var message = MessagePackSerializer.Deserialize<ReleaseRequest>(body);
            bool accepted = false;
            bool success = false;

            int tries = 0;
            while (!success)
            {
                var repository = _repositoryFactory.CreateDbContext();
                var transportEvents = await repository.TransportEvents.Where(t => t.TransportId == message.Id).OrderBy(t => t.SequenceNumber).ToListAsync();
                var transport = await repository.Transports.Where(t => t.Id == message.Id).FirstAsync();

                int seats = transport.SeatsTaken;
                decimal price = transport.PricePerTicket;
                foreach (var transportEvent in transportEvents)
                {
                    seats += transportEvent.SeatsChange;
                    price += transportEvent.PriceChange;
                }

                int lastInSequence = transportEvents.LastOrDefault()?.SequenceNumber ?? 0;
                accepted = seats - message.Seats >= 0;
                if (accepted)
                {
                    tries += 1;
                    try
                    {
                        if (repository.TransportEvents.Any(e => e.TransportId == transport.Id && e.SequenceNumber == lastInSequence + 1))
                        {
                            success = false;
                            _logger.Warning($"Event for transport {message.Id} with same sequence number {lastInSequence + 1} already exists, trying again [tryNum:{tries}]");
                            repository.Dispose();
                            await Task.Delay(1);
                            continue;
                        }
                        repository.TransportEvents.Add(new TransportEvent() { TransportId = transport.Id, SequenceNumber = lastInSequence + 1, SeatsChange = -message.Seats });
                        await repository.SaveChangesAsync();
                        repository.Dispose();
                    }
                    catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
                    {
                        success = false;
                        _logger.Warning($"Event for transport {message.Id} with same sequence number {lastInSequence + 1} already exists, trying again [tryNum:{tries}]");
                        repository.Dispose();
                        await Task.Delay(1);
                        continue;
                    }
                    _logger.Information($"Successfully Rolledback reservation for transport {message.Id} for {message.Seats} seats");
                }
                else
                {
                    _logger.Information($"Rejected rollback for transport {message.Id}, seats number does not match");
                }
                success = true;
            }
            Reply(ea, MessagePackSerializer.Serialize<bool>(accepted));
        }
    }
}
