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
    public class HotelQueryPublisher : PublisherServiceBase
    {
        private readonly string _exchangeName = "resources/hotels";
        private readonly string _routingKey = "query";

        public HotelQueryPublisher(ILogger logger, IConfiguration config, IConnectionFactory connectionFactory)
            : base(logger, connectionFactory, config.GetSection("CatalogQueryPublisher").Get<ServiceConfig>()!)
        {
            //_exchangeName = config.GetSection("CatalogQueryPublisher").GetValue<string>("exchange");
            //_routingKey = config.GetSection("CatalogQueryPublisher").GetValue<string>("routing");
        }

        public async Task<ICollection<HotelDTO>> GetHotels(HotelsGetQuery query)
        {
            Guid messageCorellationId = PublishRequestWithReply(_exchangeName, _routingKey, MessageType.GET, query);

            CancellationToken cancellationToken = new CancellationToken(false);

            var hotelsBytes = await GetReply(messageCorellationId, cancellationToken);

            var hotels = MessagePackSerializer.Deserialize<ICollection<HotelDTO>>(hotelsBytes);

            return hotels;
        }

        public async Task<ICollection<CountryDTO>> GetCountriesAll()
        {
            Guid messageCorellationId = PublishRequestWithReply(_exchangeName, _routingKey, MessageType.GET, new HotelsGetQuery());

            CancellationToken cancellationToken = new CancellationToken(false);

            var countriesBytes = await GetReply(messageCorellationId, cancellationToken);

            var countries = MessagePackSerializer.Deserialize<ICollection<CountryDTO>>(countriesBytes);

            return countries;
        }

        public async Task<ICollection<CityDTO>> GetCitiesAll()
        {
            Guid messageCorellationId = PublishRequestWithReply(_exchangeName, _routingKey, MessageType.GET, new HotelsGetQuery());

            CancellationToken cancellationToken = new CancellationToken(false);

            var citiesBytes = await GetReply(messageCorellationId, cancellationToken);

            var cities = MessagePackSerializer.Deserialize<ICollection<CityDTO>>(citiesBytes);

            return cities;
        }

        


    }
}
