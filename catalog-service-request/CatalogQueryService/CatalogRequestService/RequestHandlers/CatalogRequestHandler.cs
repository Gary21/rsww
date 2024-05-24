using CatalogRequestService.Queries;
using CatalogRequestService.QueryPublishers;
using CatalogRequestService.Requests;
using MessagePack;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitUtilities;
using System.Text;
using ConsumerConfig = RabbitUtilities.Configuration.ConsumerConfig;

namespace CatalogRequestService.RequestHandlers
{
    public class CatalogRequestHandler : ConsumerServiceBase
    {
        private readonly CatalogRequestPublisher _catalogQueryPublisher;

        public CatalogRequestHandler(Serilog.ILogger logger, IConfiguration config, IConnectionFactory connectionFactory, PublisherServiceBase catalogQueryPublisher)
            : base(logger, connectionFactory, config.GetSection("CatalogRequestHandler").Get<ConsumerConfig>()!)
        {
            _catalogQueryPublisher = (CatalogRequestPublisher)catalogQueryPublisher;
        }

        protected override void ConsumeMessage(object sender, BasicDeliverEventArgs ea)
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
                    MakeReservation(ea);
                    break;
                case MessageType.UPDATE:
                    _logger.Information($"Received getlistof");
                    SecureReservation(ea);
                    break;
                default:
                    _logger.Information($"Received message with unknown type.");
                    break;
            }
        }

        private async void MakeReservation(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = MessagePackSerializer.Deserialize<TripReserveRequest>(body);
            var success = false;

            _logger.Information($"Received reservation request for trip {message.HotelId}, {message.RoomTypeId}, {message.TransportId}.");


            var reservationId = 0; // TODO: Get reservation id from database

            var hotelReserveRequest = new RoomReserveRequest
            {
                HotelId = message.HotelId,
                RoomTypeId = message.RoomTypeId,
                CheckInDate = message.CheckInDate,
                CheckOutDate = message.CheckOutDate,
                ReservationId = reservationId
            };

            var transportReserveRequest = new TransportReserveRequest
            {
                TransportId = message.TransportId,
                NumberOfPassengers = message.PeopleNumber
            };

            var hotelResult = await _catalogQueryPublisher.ReserveRoom(hotelReserveRequest);
            var transportResult = await _catalogQueryPublisher.ReserveTransport(transportReserveRequest);

            if (hotelResult > 0 && transportResult > 0)
            {
                success = true;
            }
            else
            {
                // TODO: Rollback reservation
            }

            

        }
        
        private async void SecureReservation(BasicDeliverEventArgs ea)
        {
            var headers = ea.BasicProperties.Headers;

            if (!headers.TryGetValue("Type", out object? typeObj))
                return;
            var type = (MessageType)Enum.Parse(typeof(MessageType), ASCIIEncoding.ASCII.GetString((byte[])typeObj));

            if (!headers.TryGetValue("Date", out object? dateObj))
                return;
            DateTime.TryParse(ASCIIEncoding.ASCII.GetString((byte[])dateObj), out var date);

            if (!headers.TryGetValue("Resource", out object? resourceObj))
                return;
            var resource = ASCIIEncoding.ASCII.GetString((byte[])resourceObj);

            switch (resource)
            {
                case "hotels":
                    var request = MessagePackSerializer.Deserialize<RoomReserveRequest>(ea.Body.ToArray());
                    var result = await _catalogQueryPublisher.SecureRoom(request);
                    break;
                case "transport":
                    var request = MessagePackSerializer.Deserialize<TransportReserveRequest>(ea.Body.ToArray());
                    var result = await _catalogQueryPublisher.SecureTransport(request);
                    break;
                default:
                    _logger.Information($"Received message with unknown resource.");
                    break;
            }
        }

    }
}
