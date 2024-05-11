using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly PostgresRepository _repository;
        private readonly PublisherServiceBase _publisher;

        public TransportRequestHandler(ILogger logger, IConfiguration config, IConnectionFactory connectionFactory, PublisherServiceBase publisher, PostgresRepository repository) 
            : base(logger, connectionFactory, config.GetSection("transportConsumer").Get<RabbitUtilities.Configuration.ConsumerConfig>()!)
        {
            _repository = repository;
            _publisher = publisher;
            _repository.Database.EnsureDeleted();
            _repository.Database.EnsureCreated();
            _repository.Transports.Add(new Transport() { OriginCountry = "Polska", OriginCity = "Gdańsk", 
                                                        DestinationCountry = "Niemcy", DestinationCity = "Berlin", 
                PricePerTicket = 3.21M, SeatsNumber = 50, SeatsTaken = 0, Type = "Plane", DepartureDate = DateTime.UtcNow, ArrivalDate = DateTime.UtcNow.AddDays(2) });

            _repository.SaveChanges();
            
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

                //case MessageType.ADD:
                //    Add(ea);
                //    break;
                //case MessageType.UPDATE:
                //    Update(ea);
                //    break;
                //case MessageType.DELETE:
                //    Delete(ea);
                //    break;
                default:
                    _logger.Information($"Received message with unknown type.");
                    break;
            }
        }

        //private void Add(BasicDeliverEventArgs ea)
        //{
        //    var body = ea.Body.ToArray();
        //    var message = MessagePackSerializer.Deserialize<Transport>(body);
        //    _logger.Information($"ADD {MessagePackSerializer.ConvertToJson(body)}");
        //    //Reply(ea,body);

        //}

        //private void Update(BasicDeliverEventArgs ea)
        //{
        //    var body = ea.Body.ToArray();
        //    var message = MessagePackSerializer.Deserialize<Transport>(body);
        //    _logger.Information($"UPDATE {MessagePackSerializer.ConvertToJson(body)}");
        //}

        //private void Delete(BasicDeliverEventArgs ea)
        //{
        //    var body = ea.Body.ToArray();
        //    var id = MessagePackSerializer.Deserialize<long>(body);
        //}

        private async void Reserve(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = MessagePackSerializer.Deserialize<ReserveRequest>(body);
            var transportEvents = await _repository.TransportEvents.Where(t => t.TransportId == message.Id).OrderBy(t=>t.SequenceNumber).ToListAsync();
            var transport = await  _repository.Transports.Where(t => t.Id == message.Id).FirstAsync();
            
            //var transport = await transportTask;
            //var transportEvents = await transportEventsTask;

            int seats = transport.SeatsTaken;
            decimal price = transport.PricePerTicket;
            foreach (var transportEvent in transportEvents){
                seats += transportEvent.SeatsChange;
                price += transportEvent.PriceChange;
            }
            var accepted = seats + message.Seats <= transport.SeatsNumber && message.Seats > 0;
            if (accepted)
            {
                _repository.TransportEvents.Add(new TransportEvent() { TransportId = transport.Id, SeatsChange = message.Seats });
                await _repository.SaveChangesAsync();
                _logger.Information($"Accepted reservation for transport {message.Id} for {message.Seats} seats");
                _publisher.PublishToFanoutNoReply("event", MessageType.EVENT, $"Reservation for transport {message.Id} was made!");
            }
            else
            {
                _logger.Information($"Rejected reservation for transport {message.Id}, not enough seats");
            }
                Reply(ea, MessagePackSerializer.Serialize<bool>(accepted));
        }

        private async void Release(BasicDeliverEventArgs ea)//As message body use id of event to "delete" => get event and create new event with negative values
        {
            var body = ea.Body.ToArray();
            var message = MessagePackSerializer.Deserialize<ReleaseRequest>(body);
            
            var transportEventsTask = _repository.TransportEvents.Where(t => t.TransportId == message.Id).OrderBy(t => t.SequenceNumber).ToListAsync();
            var transportTask = _repository.Transports.Where(t => t.Id == message.Id).FirstAsync();

            var transport = await transportTask;
            var transportEvents = await transportEventsTask;

            int seats = transport.SeatsTaken;
            decimal price = transport.PricePerTicket;
            foreach (var transportEvent in transportEvents)
            {
                seats += transportEvent.SeatsChange;
                price += transportEvent.PriceChange;
            }

            if (seats - message.Seats >= 0)
            {
                _logger.Information($"Rejected rollback for transport {message.Id}, seats number does not match");
                Reply(ea, MessagePackSerializer.Serialize<bool>(false));
            }

            _repository.TransportEvents.Add(new TransportEvent() { TransportId = transport.Id, SeatsChange = -message.Seats });
            await _repository.SaveChangesAsync();
            _logger.Information($"Rolledback reservation for transport {message.Id} for {message.Seats} seats");

            Reply(ea, MessagePackSerializer.Serialize<bool>(true));
        }
    }
}
