using CatalogRequestService.DTOs;
using CatalogRequestService.Queries;
using CatalogRequestService.QueryPublishers;
using CatalogRequestService.RequestPublishers;
using CatalogRequestService.Requests;
using MessagePack;
using NuGet.Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitUtilities;
using System.Text;
using System.Text.Json;
using ConsumerConfig = RabbitUtilities.Configuration.ConsumerConfig;

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
                    GetCall(ea);
                    break;
            }
        }


        public async void GetCall(BasicDeliverEventArgs ea)
        {
            //    var message = MessagePackSerializer.Deserialize<KeyValuePair<string, byte[]>>(ea.Body.ToArray());
            //    var callCode = message.Key;
            //    switch (callCode)
            //    {

            //        case "MakeReservation":
            //            var reservationRequestGateway = MessagePackSerializer.Deserialize<ReservationQueryGateway>(message.Value);
            //            var reservationRequest = ReservationQueryGatewayToMeAdapter.Adapt(reservationRequestGateway);
            //            var tripReserveRequest = ReservationQueryGatewayToMeAdapter.Adapt(reservationRequestGateway);


            //            reservationRequest.ClientId = 1; // TODO: Get client id from token
            //            reservationRequest.TransportId = 1;

            //            var reservationId = await _catalogRequestPublisher.CreateReservation(reservationRequest.ClientId);

            //            var roomReservationRequest = new RoomReserveRequest
            //            {
            //                HotelId = reservationRequest.HotelId,
            //                RoomTypeId = reservationRequest.RoomTypeId,
            //                CheckInDate = reservationRequest.CheckInDate,
            //                CheckOutDate = reservationRequest.CheckOutDate,
            //                ReservationId = reservationId
            //            };

            //            var transportReservationRequest = new TransportReserveRequest
            //            {
            //                TransportId = reservationRequest.TransportId,
            //                NumberOfPassengers = reservationRequest.PeopleNumber
            //            };

            //            var result = MakeReservation(tripReserveRequest, reservationId);
            //            Reply(ea, MessagePackSerializer.Serialize(result));
            //            break;


            //        case "BuyReservation":
            //            var reservationToBuyId = MessagePackSerializer.Deserialize<int>(message.Value);
            //            var resultTwo = await _catalogRequestPublisher.SecurePayment(reservationToBuyId);
            //            Reply(ea, MessagePackSerializer.Serialize(resultTwo));
            //            break;


            //        default:
            //            _logger.Information($"Received message with unknown type.");
            //            break;
            //    }
        }


        //private async Task<int> MakeReservation(TripReserveRequest message, int reservationId)
        //{
        //    var hotelReserveRequest = new RoomReserveRequest
        //    {
        //        HotelId = message.HotelId,
        //        RoomTypeId = message.RoomTypeId,
        //        CheckInDate = message.CheckInDate,
        //        CheckOutDate = message.CheckOutDate,
        //        ReservationId = reservationId
        //    };

        //    var transportReserveRequest = new TransportReserveRequest
        //    {
        //        TransportId = message.TransportId,
        //        NumberOfPassengers = message.PeopleNumber
        //    };

        //    var transportResult = await _catalogRequestPublisher.ReserveTransport(transportReserveRequest);
        //    if (transportResult < 0)
        //    {
        //        _catalogRequestPublisher.CancelTransport(reservationId);
        //    }

        //    var hotelResult = await _catalogRequestPublisher.ReserveRoom(hotelReserveRequest);
        //    if (hotelResult < 0)
        //    {
        //        _catalogRequestPublisher.CancelHotel(reservationId);
        //    }

        //    var secureResult = await _catalogRequestPublisher.SecureReservation(reservationId);

        //    return secureResult;
        //}


        private async void MakeReservation(BasicDeliverEventArgs ea)
        {
            //var message = MessagePackSerializer.Deserialize<ReservationQueryGateway>(ea.Body.ToArray());

            //_logger.Information($"=>| MakeReservation :: reservationQueryGateway {JsonSerializer.Serialize(message)}");

            //var room

            //var success = false;
            //var reservationId = await _catalogRequestPublisher.CreateReservation(message.ClientId);

            //var hotelReserveRequest = new RoomReserveRequest
            //{
            //    HotelId = message.HotelId,
            //    RoomTypeId = message.RoomTypeId,
            //    CheckInDate = message.CheckInDate,
            //    CheckOutDate = message.CheckOutDate,
            //    ReservationId = reservationId
            //};

            //var transportReserveRequest = new TransportReserveRequest
            //{
            //    TransportId = message.TransportId,
            //    NumberOfPassengers = message.PeopleNumber
            //};


            //var transportResult = await _catalogRequestPublisher.ReserveTransport(transportReserveRequest);
            //if (transportResult < 0)
            //{
            //    _catalogRequestPublisher.CancelTransport(reservationId);
            //}

            //var hotelResult = await _catalogRequestPublisher.ReserveRoom(hotelReserveRequest);
            //if (hotelResult < 0)
            //{
            //    _catalogRequestPublisher.CancelHotel(reservationId);
            //}

            //var secureResult = await _catalogRequestPublisher.SecureReservation(reservationId);


            //Reply(ea, MessagePackSerializer.Serialize(secureResult));
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
