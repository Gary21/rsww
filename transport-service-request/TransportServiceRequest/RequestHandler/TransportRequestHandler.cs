using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Npgsql;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitUtilities;
using Serilog;
using System.Text;
using TransportRequestService.Entities;
using TransportRequestService.Events;
using TransportRequestService.Repositories;
using TransportRequestService.Requests;

namespace TransportRequestService.RequestHandler
{
    public class TransportRequestHandler : ConsumerServiceBase
    {
        private readonly IDbContextFactory<PostgresRepository> _repositoryFactory;
        private readonly PublisherServiceBase _publisher;

        public TransportRequestHandler(
            ILogger logger, 
            IConfiguration config, 
            IConnectionFactory connectionFactory, 
            PublisherServiceBase publisher, 
            IDbContextFactory<PostgresRepository> repositoryFactory, 
            IHostApplicationLifetime appLifetime )
                : base(logger, connectionFactory, config.GetSection("transportConsumer").Get<RabbitUtilities.Configuration.ConsumerConfig>()!, appLifetime)
        {
            _repositoryFactory= repositoryFactory;
            _publisher = publisher;

            using var repository = _repositoryFactory.CreateDbContext();
            //repository.Database.EnsureDeleted();
            repository.Database.EnsureCreated();
            //repository.Transports.Add(new Transport()
            //{
            //    OriginCountry = "Polska",
            //    OriginCity = "Gdańsk",
            //    DestinationCountry = "Niemcy",
            //    DestinationCity = "Berlin",
            //    PricePerTicket = 3.21M,
            //    SeatsNumber = 1000,
            //    SeatsTaken = 0,
            //    Type = "Plane",
            //    DepartureTime = TimeSpan.FromHours(3),
            //    DepartureDate = DateTime.UtcNow,
            //    ArrivalTime = TimeSpan.Zero,
            //    ArrivalDate = DateTime.UtcNow.AddDays(2)
            //}) ;
            //repository.TransportEvents.Add(new TransportEvent() { TransportId = 1, SequenceNumber = 0, SeatsChange = 1 });
            //repository.SaveChanges();
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
                case MessageType.UPDATE:
                    Update(ea);
                    break;
                default:
                    _logger.Information($"Received message with unknown type.");
                    break;
            }
        }

        private async void Update(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = MessagePackSerializer.Deserialize<UpdateRequest>(body);
            bool accepted = false;
            bool success = false;

            int tries = 0;
            while (!success)
            {
                var repository = _repositoryFactory.CreateDbContext();
                var transport = (await repository.Transports.Where(t => t.Id == message.Id).ToListAsync()).FirstOrDefault();

                if (transport == null)
                {
                    _logger.Information($"Rejected reservation for transport {message.Id}");
                    break;
                }

                var transportEvents = await repository.TransportEvents.Where(t => t.TransportId == message.Id).OrderBy(t => t.SequenceNumber).ToListAsync();

                int seats = transport.SeatsTaken;
                decimal price = transport.PricePerTicket;
                //foreach (var transportEvent in transportEvents)
                //{
                //    seats += transportEvent.SeatsChange;
                //    price += transportEvent.PriceChange;
                //}

                int lastInSequence = transportEvents.LastOrDefault()?.SequenceNumber ?? 0; //transportEvents.Count();
                //accepted = true; //seats + message.Seats <= transport.SeatsNumber && message.Seats > 0 ;
                //if (accepted)
                //{
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

                    repository.TransportEvents.Add( 
                        new TransportEvent() { 
                            TransportId = transport.Id, 
                            SequenceNumber = lastInSequence + 1, 
                            SeatsChange = message.SeatsChange, 
                            PriceChange=message.PriceChange 
                        });
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
                _logger.Information($"Updated transport {message.Id} with {message.SeatsChange} seats and {message.PriceChange} price change [seq:{lastInSequence + 1}] [tryNum:{tries}]");
                var changeEvent = new KeyValuePair<string, byte[]>("transport", MessagePackSerializer.Serialize(new TransportChangeEvent()
                {
                    id = message.Id,
                    destinationCity = transport.DestinationCity,
                    destinationCountry = transport.DestinationCountry,
                    priceChange = message.PriceChange,
                    seatsChange = message.SeatsChange,
                    transportType = transport.Type
                } ));
                
                _publisher.PublishToFanoutNoReply("event", MessageType.UPDATE, changeEvent );
                //}
                //else
                //{
                //    _logger.Information($"Rejected reservation for transport {message.Id}, not enough seats");
                //}
                success = true;
            }
            Reply(ea, MessagePackSerializer.Serialize<bool>(accepted));

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
                var transport = (await repository.Transports.Where(t => t.Id == message.Id).ToListAsync()).FirstOrDefault();
                
                if (transport == null)
                {
                    _logger.Information($"Rejected reservation for transport {message.Id}");
                    break;
                }
                    
                var transportEvents = await repository.TransportEvents.Where(t => t.TransportId == message.Id).OrderBy(t => t.SequenceNumber).ToListAsync();


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
                    var reserveEvent = new KeyValuePair<string, byte[]>("transport",
                        MessagePackSerializer.Serialize(new TransportReservationEvent() { transportType = transport.Type, destinationCity = transport.DestinationCity, seats = message.Seats }));

                    _publisher.PublishToFanoutNoReply("event", MessageType.RESERVE, reserveEvent);
                }
                else
                {
                    _logger.Information($"Rejected reservation for transport {message.Id}, not enough seats");
                }
                success = true;
            }
            Reply(ea, MessagePackSerializer.Serialize<bool>(accepted));
        }

        private async void Release(BasicDeliverEventArgs ea) //As message body use id of event to "delete" => get event and create new event with negative values
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
