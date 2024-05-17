using CatalogRequestService.DTOs;
using NuGet.Protocol.Core.Types;
using RabbitMQ.Client;
using RabbitUtilities;
using RabbitUtilities.Configuration;
using ILogger = Serilog.ILogger;
using MessagePack;
using CatalogQueryService.DTOs;
using CatalogQueryService.Queries;
using CatalogQueryService.Filters;



namespace CatalogRequestService.QueryPublishers
{
    public class CatalogQueryPublisher : PublisherServiceBase
    {
        private readonly string _exchangeName = "resources/hotels";
        private readonly string _routingKey = "query";

        public CatalogQueryPublisher(ILogger logger, IConfiguration config, IConnectionFactory connectionFactory)
            : base(logger, connectionFactory, config.GetSection("CatalogQueryPublisher").Get<ServiceConfig>()!)
        {
            //_exchangeName = config.GetSection("CatalogQueryPublisher").GetValue<string>("exchange");
            //_routingKey = config.GetSection("CatalogQueryPublisher").GetValue<string>("routing");
        }

        public async Task<ICollection<HotelDTO>> GetHotels(HotelsGetQuery query)
        {
            Guid messageCorellationId = PublishRequestWithReply("resources/hotels", _routingKey, MessageType.GET, query);

            CancellationToken cancellationToken = new CancellationToken(false);

            var hotelsBytes = await GetReply(messageCorellationId, cancellationToken);

            var hotels = MessagePackSerializer.Deserialize<ICollection<HotelDTO>>(hotelsBytes);

            return hotels;
        }

        public async Task<ICollection<TransportDTO>> GetTransports(TransportGetQuery query)
        {
            Guid messageCorellationId = PublishRequestWithReply("resources/transport", _routingKey, MessageType.GET, query);

            CancellationToken cancellationToken = new CancellationToken(false);

            var transportsBytes = await GetReply(messageCorellationId, cancellationToken);

            var transports = MessagePackSerializer.Deserialize<ICollection<TransportDTO>>(transportsBytes);

            return transports;
        }

    }
}
