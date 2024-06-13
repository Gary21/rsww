using CatalogQueryService.DTOs;
using CatalogQueryService.Queries;
using MessagePack;
using RabbitMQ.Client;
using RabbitUtilities;
using RabbitUtilities.Configuration;
using System.Text.Json;
using ILogger = Serilog.ILogger;



namespace CatalogQueryService.QueryPublishers
{
    public class CatalogQueryPublisher : PublisherServiceBase
    {
        private readonly string _exchangeName = "resources/hotels";
        private readonly string _routingKey = "query";

        public CatalogQueryPublisher(ILogger logger, IConfiguration config, IConnectionFactory connectionFactory, IHostApplicationLifetime appLifeTime)
            : base(logger, connectionFactory, config.GetSection("CatalogQueryPublisher").Get<ServiceConfig>()!, appLifeTime)
        {
            //_exchangeName = config.GetSection("CatalogQueryPublisher").GetValue<string>("exchange");
            //_routingKey = config.GetSection("CatalogQueryPublisher").GetValue<string>("routing");
        }


        public async Task<ICollection<CountryDTO>> GetCountries()
        {
            Guid messageCorellationId = PublishRequestWithReply("resources/hotels", _routingKey, MessageType.UPDATE, "countries");

            CancellationToken cancellationToken = new CancellationToken(false);

            var countriesBytes = await GetReply(messageCorellationId, cancellationToken);

            var countries = MessagePackSerializer.Deserialize<ICollection<CountryDTO>>(countriesBytes);

            return countries;
        }

        public async Task<ICollection<CityDTO>> GetCities()
        {
            _logger.Information("|=> UPDATE :: GetCities");
            Guid messageCorellationId = PublishRequestWithReply("resources/hotels", _routingKey, MessageType.UPDATE, "cities");

            CancellationToken cancellationToken = new CancellationToken(false);

            var citiesBytes = await GetReply(messageCorellationId, cancellationToken);

            var cities = MessagePackSerializer.Deserialize<ICollection<CityDTO>>(citiesBytes);

            return cities;
        }

        public async Task<ICollection<RoomTypeDTO>> GetRoomTypes()
        {
            _logger.Information("|=> UPDATE :: GetRoomTypes");
            Guid messageCorellationId = PublishRequestWithReply("resources/hotels", _routingKey, MessageType.UPDATE, "roomtypes");

            CancellationToken cancellationToken = new CancellationToken(false);

            var roomTypesBytes = await GetReply(messageCorellationId, cancellationToken);

            var roomTypes = MessagePackSerializer.Deserialize<ICollection<RoomTypeDTO>>(roomTypesBytes);

            return roomTypes;
        }

        public async Task<ICollection<RoomTypeDTO>> GetRoomTypesForHotelId(int hotelId)
        {
            _logger.Information("|=> GET :: GetRoomTypesForHotelId");
            Guid messageCorellationId = PublishRequestWithReply("resources/hotels", _routingKey, MessageType.EVENT, hotelId);

            var roomTypesBytes = await GetReply(messageCorellationId, new CancellationToken(false));

            var roomTypes = MessagePackSerializer.Deserialize<ICollection<RoomTypeDTO>>(roomTypesBytes);

            var roomTypesJson = JsonSerializer.Serialize(roomTypes);
            _logger.Information($"|<= GET :: GetRoomTypesForHotelId - roomTypes count {roomTypes.Count},\n roomTypes: {roomTypesJson}");

            return roomTypes;
        }

        public async Task<ICollection<HotelDTO>> GetHotels(HotelsGetQuery query)
        {
            _logger.Information("|=> GET :: GetHotels");
            Guid messageCorellationId = PublishRequestWithReply("resources/hotels", _routingKey, MessageType.GET, query);

            CancellationToken cancellationToken = new CancellationToken(false);

            var hotelsBytes = await GetReply(messageCorellationId, cancellationToken);

            var hotels = MessagePackSerializer.Deserialize<ICollection<HotelDTO>>(hotelsBytes);
            var hotelNames = hotels.Select(h => h.Name).ToList();
            _logger.Information($"|<= GET :: GetHotels - hotels count {hotels.Count},\n hotels: {string.Join(", ", hotelNames)}");

            return hotels;
        }

        public async Task<ICollection<TransportDTO>> GetTransports(TransportGetQuery query)
        {
            //_logger.Information("|=> GET :: GetTransports");
            Guid messageCorellationId = PublishRequestWithReply("resources/transport", _routingKey, MessageType.GET, query);

            CancellationToken cancellationToken = new CancellationToken(false);

            var transportsBytes = await GetReply(messageCorellationId, cancellationToken);

            var transports = MessagePackSerializer.Deserialize<ICollection<TransportDTO>>(transportsBytes);

            return transports;
        }

    }
}
