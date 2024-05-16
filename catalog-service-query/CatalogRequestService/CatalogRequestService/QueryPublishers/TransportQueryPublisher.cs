using CatalogRequestService.DTOs;
using MessagePack;
using RabbitMQ.Client;
using RabbitUtilities;
using RabbitUtilities.Configuration;
using ILogger = Serilog.ILogger;

namespace CatalogRequestService.QueryPublishers
{
    public class TransportQueryPublisher : PublisherServiceBase
    {
        private readonly string _exchangeName = "Transports";
        private readonly string _routingKey = "Transports";

        public TransportQueryPublisher(ILogger logger, IConfiguration config, IConnectionFactory connectionFactory)
            : base(logger, connectionFactory, config.GetSection("transportQueryPublisher").Get<ServiceConfig>()!)
        { }


        public async Task<ICollection<TransportDTO>> GetTransportsAll()
        {
            Guid messageCorellationId = PublishRequestWithReply(_exchangeName, _routingKey, MessageType.GET, "Transports/all");

            CancellationToken cancellationToken = new CancellationToken(false);

            var transportsBytes = await GetReply(messageCorellationId, cancellationToken);

            var transports = MessagePackSerializer.Deserialize<ICollection<TransportDTO>>(transportsBytes);

            return transports;
        }

    }
}
