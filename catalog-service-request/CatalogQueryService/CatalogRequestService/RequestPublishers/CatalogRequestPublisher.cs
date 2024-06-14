using CatalogRequestService.Queries;
using CatalogRequestService.DTOs;
using MessagePack;
using RabbitMQ.Client;
using RabbitUtilities;
using RabbitUtilities.Configuration;
using ILogger = Serilog.ILogger;
using CatalogRequestService.RequestPublishers;



namespace CatalogRequestService.QueryPublishers
{
    public class CatalogRequestPublisher : PublisherServiceBase
    {
        private readonly string _routingKey = "request";

        public CatalogRequestPublisher(ILogger logger, IConfiguration config, IConnectionFactory connectionFactory, IHostApplicationLifetime appLifeTime)
            : base(logger, connectionFactory, config.GetSection("CatalogQueryPublisher").Get<ServiceConfig>()!, appLifeTime)
        {
            //_exchangeName = config.GetSection("CatalogQueryPublisher").GetValue<string>("exchange");
            //_routingKey = config.GetSection("CatalogQueryPublisher").GetValue<string>("routing");
        }

        public async Task<int> ReserveRoom(RoomReserveRequest query)
        {
            Guid messageCorellationId = PublishRequestWithReply("resources/hotels", _routingKey, MessageType.RESERVE, query);

            CancellationToken cancellationToken = new CancellationToken(false);

            var result = await GetReply(messageCorellationId, cancellationToken);

            //var success = MessagePackSerializer.Deserialize<int>(result);

            return 1;
        }

        public async Task<int> ReserveTransport(TransportReserveRequest query)
        {
            Guid messageCorellationId = PublishRequestWithReply("resources/transport", _routingKey, MessageType.RESERVE, query);

            CancellationToken cancellationToken = new CancellationToken(false);

            var result = await GetReply(messageCorellationId, cancellationToken);

            //var success = MessagePackSerializer.Deserialize<int>(result);

            return 1;
        }

        public async Task<int> CreateReservation(int clientId)
        {
            Guid messageCorellationId = PublishRequestWithReply("resources/order", _routingKey, MessageType.ADD, clientId);

            CancellationToken cancellationToken = new CancellationToken(false);

            var result = await GetReply(messageCorellationId, cancellationToken);

            var success = MessagePackSerializer.Deserialize<int>(result);

            return success;
        }




        public async Task<bool> SecurePayment(int reservationId)
        {
            Guid messageCorellationId = PublishRequestWithReply("resources/payment", _routingKey, MessageType.UPDATE, reservationId);

            CancellationToken cancellationToken = new CancellationToken(false);

            var result = await GetReply(messageCorellationId, cancellationToken);

            var success = MessagePackSerializer.Deserialize<bool>(result);

            return success;
        }

        public async Task<int> CancelReservation(int reservationId)
        {
            Guid messageCorellationId = PublishRequestWithReply("resources/order", _routingKey, MessageType.DELETE, reservationId);

            CancellationToken cancellationToken = new CancellationToken(false);

            var result = await GetReply(messageCorellationId, cancellationToken);

            var success = MessagePackSerializer.Deserialize<int>(result);

            return success;
        }

        public async Task<int> CancelHotel(int reservationId)
        {
            Guid messageCorellationId = PublishRequestWithReply("resources/hotels", _routingKey, MessageType.DELETE, reservationId);

            CancellationToken cancellationToken = new CancellationToken(false);

            var result = await GetReply(messageCorellationId, cancellationToken);

            var success = MessagePackSerializer.Deserialize<int>(result);

            return success;
        }

        public async Task<int> CancelTransport(int reservationId)
        {
            Guid messageCorellationId = PublishRequestWithReply("resources/transport", _routingKey, MessageType.DELETE, reservationId);

            CancellationToken cancellationToken = new CancellationToken(false);

            var result = await GetReply(messageCorellationId, cancellationToken);

            var success = MessagePackSerializer.Deserialize<int>(result);

            return success;
        }

        public async Task<bool> SecureReservation(TripDTO tripDTO)
        {
            var order = new Order
            {
                HotelId = tripDTO.HotelId,
                OccupationId = new int[] { tripDTO.RoomTypeId },
                TransportId = tripDTO.TransportThereId,
                UserId = 3,
                BabyCount = 0,
                ChildCount = 0,
                TeenCount = 0,
                AdultCount = tripDTO.PeopleNumber,
                WithFood = false,
                Discount = 0,
                Price = decimal.Parse(tripDTO.Price)
            };

            // getting hotel name
            var data = MessagePackSerializer.Serialize(tripDTO.HotelId);
            var payload = new KeyValuePair<string, byte[]>("GetHotel", data);
            var messageId = PublishRequestWithReply("catalog", "query", MessageType.GET, payload);
            CancellationToken cancellationToken = new CancellationToken(false);
            var bytes = await GetReply(messageId, cancellationToken);
            var hotelDTO = MessagePackSerializer.Deserialize<HotelDTO>(bytes);
            //

            // getting roomType name
            var dataR = MessagePackSerializer.Serialize(tripDTO.RoomTypeId);
            var payloadR = new KeyValuePair<string, byte[]>("GetRoomType", dataR);
            var messageIdR = PublishRequestWithReply("catalog", "query", MessageType.GET, payloadR);
            CancellationToken cancellationTokenR = new CancellationToken(false);
            var bytesR = await GetReply(messageIdR, cancellationTokenR);
            var roomTypeDTO = MessagePackSerializer.Deserialize<RoomTypeDTO>(bytesR);
            //

            // getting country name
            var countryName = "Malta";
            //

            // getting transport type name
            var dataT = MessagePackSerializer.Serialize(tripDTO.TransportThereId);
            var payloadT = new KeyValuePair<string, byte[]>("GetTransport", dataT);
            var messageIdT = PublishRequestWithReply("catalog", "query", MessageType.GET, payloadT);
            CancellationToken cancellationTokenT = new CancellationToken(false);
            var bytesT = await GetReply(messageIdT, cancellationTokenT);
            var transportDTO = MessagePackSerializer.Deserialize<TransportDTO>(bytesT);
            //

            var addRequest = new AddRequest
            {
                Order = order,
                HotelName = hotelDTO.Name,
                RoomType = roomTypeDTO.Name,
                City = tripDTO.DestinationCity,
                Country = countryName,
                TransportType = transportDTO.Type
            };

            var dataAddRequest = MessagePackSerializer.Serialize(addRequest);

            PublishRequestNoReply("order", "all", MessageType.ADD, addRequest);

            CancellationToken cancellationTokenTwo = new CancellationToken(false);

            //var reservationId = MessagePackSerializer.Deserialize<bool>(true);

            return true;
        }
    }
}
