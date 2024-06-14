using CatalogRequestService.DTOs;
using CatalogRequestService.Queries;
using CatalogRequestService.QueryPublishers;
using CatalogRequestService.RequestPublishers;
using MessagePack;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitUtilities;
using System.Text;
using ConsumerConfig = RabbitUtilities.Configuration.ConsumerConfig;
using JS = System.Text.Json.JsonSerializer;

namespace CatalogRequestService.RequestHandlers
{
    public class CatalogRequestHandler : ConsumerServiceBase
    {
        private readonly CatalogRequestPublisher _catalogRequestPublisher;
        private readonly Publisher2Service _transactionRequestPublisher;
        private readonly CancellationToken _cancellationToken;

        public CatalogRequestHandler(Serilog.ILogger logger, IConfiguration config, IConnectionFactory connectionFactory, PublisherServiceBase catalogQueryPublisher, Publisher2Service transactionRequestPublisher, IHostApplicationLifetime appLifeTime)
            : base(logger, connectionFactory, config.GetSection("CatalogQueryPublisher").Get<ConsumerConfig>()!, appLifeTime)
        {
            _catalogRequestPublisher = (CatalogRequestPublisher)catalogQueryPublisher;
            _transactionRequestPublisher = transactionRequestPublisher;
            _cancellationToken = appLifeTime.ApplicationStopping;
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

            _logger.Information($"=>| Consuming Message");

            switch (type)
            {
                case MessageType.RESERVE:
                    MakeReservation(ea);
                    break;
                case MessageType.ADD:
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
            var message = MessagePackSerializer.Deserialize<KeyValuePair<string, byte[]>>(ea.Body.ToArray());
            var tripDTO = MessagePackSerializer.Deserialize<TripDTO>(message.Value);
            _logger.Information($"=>| RESERVE :: MakeReservation - tripDTO: {JS.Serialize(tripDTO)}");

            var hotelReserveRequest = new RoomReserveRequest
            {
                HotelId = tripDTO.HotelId,
                RoomTypeId = tripDTO.RoomTypeId
            };
            if (tripDTO.DateStart == null || tripDTO.DateEnd == null)
            {
                hotelReserveRequest.CheckInDate = DateTime.Today;
                hotelReserveRequest.CheckOutDate = DateTime.Today.AddDays(7);
            }
            else
            {
                hotelReserveRequest.CheckInDate = DateTime.Parse(tripDTO.DateStart);
                hotelReserveRequest.CheckOutDate = DateTime.Parse(tripDTO.DateEnd);
            }

            var transportReserveRequest = new TransportReserveRequest
            {
                TransportId = tripDTO.TransportThereId,
                NumberOfPassengers = tripDTO.PeopleNumber
            };

            _logger.Information($">|< MakeReservation() :: reserving transport - {JS.Serialize(transportReserveRequest)}");
            var transportResult = await _catalogRequestPublisher.ReserveTransport(transportReserveRequest);
            //if (transportResult < 0)
            //{
            //    //_catalogRequestPublisher.CancelTransport(reservationId);
            //}

            transportReserveRequest = new TransportReserveRequest
            {
                TransportId = tripDTO.TransportBackId,
                NumberOfPassengers = tripDTO.PeopleNumber
            };
            _logger.Information($">|< MakeReservation() :: reserving transport - {JS.Serialize(transportReserveRequest)}");
            transportResult = await _catalogRequestPublisher.ReserveTransport(transportReserveRequest);
            //if (transportResult < 0)
            //{
            //    //_catalogRequestPublisher.CancelTransport(reservationId);
            //}

            _logger.Information($">|< MakeReservation() :: reserving hotel - {JS.Serialize(hotelReserveRequest)}");
            var hotelResult = await _catalogRequestPublisher.ReserveRoom(hotelReserveRequest);
            if (hotelResult < 0)
            {
                //_catalogRequestPublisher.CancelHotel(reservationId);
            }

            var secureResult = await _catalogRequestPublisher.SecureReservation(tripDTO);

            //Reply(ea, MessagePackSerializer.Serialize(secureResult));
            _logger.Information($"<=| RESERVE :: MakeReservation");
            Reply(ea, MessagePackSerializer.Serialize(11));
        }

        private async void SecureReservation(BasicDeliverEventArgs ea)
        {
            var mesId = _transactionRequestPublisher.PublishRequestWithReply("transactions-exchange", "incoming.all", MessageType.GET, 1);
            //var randomTrue = new Random().Next(0, 2) == 1;
            var response = await _transactionRequestPublisher.GetReply(mesId, _cancellationToken);
            var result = MessagePackSerializer.Deserialize<bool>(response);
            Reply(ea, MessagePackSerializer.Serialize(result));
        }

    }
}
