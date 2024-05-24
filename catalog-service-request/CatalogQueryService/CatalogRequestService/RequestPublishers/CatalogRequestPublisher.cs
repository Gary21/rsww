using CatalogRequestService.Queries;
using CatalogRequestService.DTOs;
using MessagePack;
using RabbitMQ.Client;
using RabbitUtilities;
using RabbitUtilities.Configuration;
using ILogger = Serilog.ILogger;



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

            var success = MessagePackSerializer.Deserialize<int>(result);

            return success;
        }

        public async Task<int> ReserveTransport(TransportReserveRequest query)
        {
            Guid messageCorellationId = PublishRequestWithReply("resources/transport", _routingKey, MessageType.RESERVE, query);

            CancellationToken cancellationToken = new CancellationToken(false);

            var result = await GetReply(messageCorellationId, cancellationToken);

            var success = MessagePackSerializer.Deserialize<int>(result);

            return success;
        }

        public async Task<int> CreateReservation(int clientId)
        {
            Guid messageCorellationId = PublishRequestWithReply("resources/order", _routingKey, MessageType.ADD, clientId);

            CancellationToken cancellationToken = new CancellationToken(false);

            var result = await GetReply(messageCorellationId, cancellationToken);

            var success = MessagePackSerializer.Deserialize<int>(result);

            return success;
        }




        public async Task<int> SecureReservation(int reservationId)
        {
            Guid messageCorellationId = PublishRequestWithReply("resources/order", _routingKey, MessageType.UPDATE, reservationId);

            CancellationToken cancellationToken = new CancellationToken(false);

            var result = await GetReply(messageCorellationId, cancellationToken);

            var success = MessagePackSerializer.Deserialize<int>(result);

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

        public async Task<bool> SecurePayment(int reservationId)
        {
            Guid messageCorellationId = PublishRequestWithReply("resources/payment", _routingKey, MessageType.UPDATE, reservationId);

            CancellationToken cancellationToken = new CancellationToken(false);

            var result = await GetReply(messageCorellationId, cancellationToken);

            var success = MessagePackSerializer.Deserialize<bool>(result);

            return success;
        }

    }
}
