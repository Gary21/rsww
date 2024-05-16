using CatalogRequestService.DTOs;
using NuGet.Protocol.Core.Types;
using RabbitMQ.Client;
using RabbitUtilities;
using RabbitUtilities.Configuration;
using ILogger = Serilog.ILogger;
using MessagePack;
using CatalogQueryService.DTOs;



namespace CatalogRequestService.QueryPublishers
{
    public class HotelQueryPublisher : PublisherServiceBase
    {
        private readonly string _exchangeName = "Hotels";
        private readonly string _routingKey = "Hotels";

        public HotelQueryPublisher(ILogger logger, IConfiguration config, IConnectionFactory connectionFactory)
            : base(logger, connectionFactory, config.GetSection("hotelQueryPublisher").Get<ServiceConfig>()!)
        {}

        public async Task<ICollection<HotelDTO>> GetHotelsAll()
        {
            Guid messageCorellationId = PublishRequestWithReply(_exchangeName, _routingKey, MessageType.GET, "Hotels/all");

            CancellationToken cancellationToken = new CancellationToken(false);

            var hotelsBytes = await GetReply(messageCorellationId, cancellationToken);

            var hotels = MessagePackSerializer.Deserialize<ICollection<HotelDTO>>(hotelsBytes);

            return hotels;
        }

        public async Task<HotelDTO> GetHotelById(int id)
        {
            Guid messageCorellationId = PublishRequestWithReply(_exchangeName, _routingKey, MessageType.GET, $"Hotels/{id}");

            CancellationToken cancellationToken = new CancellationToken(false);

            var hotelBytes = await GetReply(messageCorellationId, cancellationToken);

            var hotel = MessagePackSerializer.Deserialize<HotelDTO>(hotelBytes);

            return hotel;
        }

        public async Task<ICollection<CountryDTO>> GetCountriesAll()
        {
            Guid messageCorellationId = PublishRequestWithReply(_exchangeName, _routingKey, MessageType.GET, "Countries/all");

            CancellationToken cancellationToken = new CancellationToken(false);

            var countriesBytes = await GetReply(messageCorellationId, cancellationToken);

            var countries = MessagePackSerializer.Deserialize<ICollection<CountryDTO>>(countriesBytes);

            return countries;
        }

        public async Task<ICollection<CityDTO>> GetCitiesAll()
        {
            Guid guid = PublishRequestWithReply(_exchangeName, _routingKey, MessageType.GET, "Cities/all");

            CancellationToken cancellationToken = new CancellationToken(false);

            var citiesBytes = await GetReply(guid, cancellationToken);

            var cities = MessagePackSerializer.Deserialize<ICollection<CityDTO>>(citiesBytes);

            return cities;
        }


    }
}
