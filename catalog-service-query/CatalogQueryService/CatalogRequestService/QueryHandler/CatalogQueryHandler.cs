using AutoMapper;
using CatalogQueryService.DTOs;
using CatalogQueryService.Filters;
using CatalogQueryService.Queries;
using CatalogQueryService.QueryPublishers;
using CatalogRequestService.DTOs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitUtilities;
using System.Text;
using ConsumerConfig = RabbitUtilities.Configuration.ConsumerConfig;
using JS = System.Text.Json.JsonSerializer;
using MPS = MessagePack.MessagePackSerializer;

namespace CatalogQueryService.QueryHandler
{
    public class CatalogQueryHandler : ConsumerServiceBase
    {
        private readonly CatalogQueryPublisher _catalogQueryPublisher;
        private readonly IMapper _mapper;
        private ICollection<CityDTO> _cityList;
        private ICollection<RoomTypeDTO> _roomTypesList;

        public CatalogQueryHandler(Serilog.ILogger logger, IConfiguration config, IConnectionFactory connectionFactory, PublisherServiceBase catalogQueryPublisher, IMapper mapper, IHostApplicationLifetime appLifeTime)
            : base(logger, connectionFactory, config.GetSection("CatalogQueryPublisher").Get<ConsumerConfig>()!, appLifeTime)
        {
            _catalogQueryPublisher = (CatalogQueryPublisher)catalogQueryPublisher;
            _mapper = mapper;
        }

        protected override void ConsumeMessage(object model, BasicDeliverEventArgs ea)
        {
            _logger.Information($"=>| MESSAGE CONSUME");
            var headers = ea.BasicProperties.Headers;
            if (!headers.TryGetValue("Type", out object? typeObj)) { return; }
            var type = (MessageType)Enum.Parse(typeof(MessageType), ASCIIEncoding.ASCII.GetString((byte[])typeObj));
            if (!headers.TryGetValue("Date", out object? dateObj)) { return; }
            DateTime.TryParse(ASCIIEncoding.ASCII.GetString((byte[])dateObj), out var date);

            GetCall(ea);
        }

        public async void GetCall(BasicDeliverEventArgs ea)
        {
            var message = MPS.Deserialize<KeyValuePair<string, byte[]>>(ea.Body.ToArray());
            var callCode = message.Key;

            _logger.Information($"=>| GET :: Call - {callCode}");

            switch (callCode)
            {
                case "GetDestinations":

                    _logger.Information($"=>| GET :: Destinations");

                    var cities = await _catalogQueryPublisher.GetCities();
                    var cityNames = cities.Select(c => c.Name).ToList();
                    var serialized = MPS.Serialize(cityNames);

                    _logger.Information($"<=| GET :: Destinations - cities count {cityNames.Count}");

                    Reply(ea, serialized);
                    break;

                case "GetTrips":

                    var tripsQueryDTO = MPS.Deserialize<TripDTO>(message.Value);

                    _logger.Information($"=>| GET :: TripsQuery - {JS.Serialize(tripsQueryDTO)}");

                    var validTrips = await GetTrips(tripsQueryDTO);

                    _logger.Information($"<=| GET :: Trips - trips count {validTrips.Count}");

                    var tripsSerialized = MPS.Serialize(validTrips);
                    Reply(ea, tripsSerialized);

                    break;


                case "GetHotel":
                    var hotelId = MPS.Deserialize<int>(message.Value);

                    _logger.Information($"=>| GET :: Hotel - {hotelId}");

                    var hotelGetQueryTwo = new HotelsGetQuery
                    {
                        filters = new HotelQueryFilters
                        {
                            HotelIds = new List<int> { hotelId }
                        }
                    };

                    var hotels = await _catalogQueryPublisher.GetHotels(hotelGetQueryTwo);
                    var hotel = hotels.FirstOrDefault();

                    _logger.Information($"<=| GET :: Hotel - {JS.Serialize(hotel)}");

                    Reply(ea, MPS.Serialize(hotel));
                    break;


                case "GetAvailability":
                    _logger.Information($"=>| GET :: Availability");
                    _logger.Information($"<=| GET :: Availability - {true}");
                    Reply(ea, MPS.Serialize(true));
                    break;


                case "ValidateReservation":

                    var tripReservation = MPS.Deserialize<TripDTO>(message.Value);

                    _logger.Information($"=>| GET :: ValidateReservation - reservationQuery: {JS.Serialize(tripReservation)}");

                    var validReservations = await GetTrips(tripReservation);

                    _logger.Information($"<=| GET :: ValidateReservation - trips count: {validReservations.Count}");

                    if (validReservations.Count == 0) { Reply(ea, MPS.Serialize(false)); }
                    else { Reply(ea, MPS.Serialize(true)); }

                    break;


                case "GetHotelRooms":

                    var hotelIdForRooms = MPS.Deserialize<int>(message.Value);

                    _logger.Information($"=>| GET :: HotelRooms - {hotelIdForRooms}");

                    var rooms = await _catalogQueryPublisher.GetRoomTypesForHotelId(hotelIdForRooms);

                    _logger.Information($"<=| GET :: HotelRooms - rooms count: {rooms.Count},\n rooms: {JS.Serialize(rooms)}");

                    Reply(ea, MPS.Serialize(rooms));
                    break;

                case "GetRoomType":
                    var roomTypeId = MPS.Deserialize<int>(message.Value);

                    _logger.Information($"=>| GET :: RoomType - {roomTypeId}");

                    _roomTypesList = await _catalogQueryPublisher.GetRoomTypes();
                    var roomType = _roomTypesList.FirstOrDefault(rt => rt.Id == roomTypeId);

                    _logger.Information($"<=| GET :: RoomType - {JS.Serialize(roomType)}");

                    Reply(ea, MPS.Serialize(roomType));
                    break;

                case "FindTransports":
                    var transportQuery = MPS.Deserialize<TransportDTO>(message.Value);

                    _logger.Information($"=>| GET :: FindTransports - {JS.Serialize(transportQuery)}");

                    var transportQueryFilters = new TransportQueryFilters
                    {
                        CityOrigins = new List<string> { transportQuery.OriginCity },
                        CityDestinations = new List<string> { transportQuery.DestinationCity },
                        AvailableSeats = transportQuery.PeopleNumber,
                        Types = new List<string> { transportQuery.Type ?? "" }
                    };
                    if (transportQuery.Date == null) { transportQueryFilters.DepartureDates = new List<DateTime> { DateTime.Today }; }
                    else { transportQueryFilters.DepartureDates = new List<DateTime> { DateTime.Parse(transportQuery.Date) }; }

                    var transportQueryDTO = new TransportGetQuery { filters = transportQueryFilters };
                    var transports = await _catalogQueryPublisher.GetTransports(transportQueryDTO);

                    _logger.Information($"<=| GET :: FindTransports - transports count: {transports.Count},\n transports: {JS.Serialize(transports)}");

                    Reply(ea, MPS.Serialize(transports));
                    break;

                case "GetTransport":
                    var transportId = MPS.Deserialize<int>(message.Value);

                    _logger.Information($"=>| GET :: Transport - {transportId}");

                    var transportQueryFiltersTwo = new TransportQueryFilters
                    {
                        Ids = new List<int> { transportId }
                    };
                    var transportQueryDTOTwo = new TransportGetQuery { filters = transportQueryFiltersTwo };
                    var transportsTwo = await _catalogQueryPublisher.GetTransports(transportQueryDTOTwo);
                    var transport = transportsTwo.FirstOrDefault();

                    _logger.Information($"<=| GET :: Transport - {JS.Serialize(transport)}");

                    Reply(ea, MPS.Serialize(transport));
                    break;


                default:
                    _logger.Information($"Received message null or with unknown.");
                    break;
            }
        }



        private async Task<List<TripDTO>> GetTrips(TripDTO tripsQueryDTO)
        {
            // Getting cities and room types for conversion (name <-> id)
            _cityList = await _catalogQueryPublisher.GetCities();
            var destinationCityId = _cityList.FirstOrDefault(c => c.Name == tripsQueryDTO.DestinationCity)?.Id;
            var originCityId = _cityList.FirstOrDefault(c => c.Name == tripsQueryDTO.OriginCity)?.Id;
            _logger.Information($">|< Get() :: DestinationCityId {destinationCityId}, OriginCityId {originCityId}");

            _roomTypesList = await _catalogQueryPublisher.GetRoomTypes();
            var roomTypeName = _roomTypesList.FirstOrDefault(rt => rt.Id == tripsQueryDTO.RoomTypeId)?.Name;
            _logger.Information($">|< Get() :: RoomTypeName {roomTypeName}");
            //

            // Handling filters
            var hotelGetQuery = new HotelsGetQuery { filters = new HotelQueryFilters() };
            if (destinationCityId != null) { hotelGetQuery.filters.CityIds = new List<int> { destinationCityId.Value }; }
            if (roomTypeName != null) { hotelGetQuery.filters.RoomTypes = new List<string> { roomTypeName }; }
            if (tripsQueryDTO.DateStart != null) { hotelGetQuery.filters.CheckInDate = DateTime.Parse(tripsQueryDTO.DateStart); }
            if (tripsQueryDTO.DateEnd != null) { hotelGetQuery.filters.CheckOutDate = DateTime.Parse(tripsQueryDTO.DateEnd); }
            if (tripsQueryDTO.PeopleNumber > 0) { hotelGetQuery.filters.RoomCapacities = new List<int> { tripsQueryDTO.PeopleNumber }; }
            var serializedQuery = MPS.Serialize(hotelGetQuery);
            //

            var hotelTransportMatches = new List<TripDTO>();

            // Get hotels
            var hotels = await _catalogQueryPublisher.GetHotels(hotelGetQuery);
            if (hotels.Count == 0) { return hotelTransportMatches; }
            //

            // Parse valid hotels
            foreach (var hotelResponse in hotels)
            {
                // check for valid room type
                var roomTypes = await _catalogQueryPublisher.GetRoomTypesForHotelId(hotelResponse.Id);
                var validRoomType = roomTypes.FirstOrDefault(rt => rt.Capacity >= tripsQueryDTO.PeopleNumber);
                if (validRoomType == null) { continue; }
                //


                // check for valid transport there
                var transThereFilters = new TransportQueryFilters
                {
                    CityOrigins = new List<string> { tripsQueryDTO.OriginCity ?? "" },
                    CityDestinations = new List<string> { tripsQueryDTO.DestinationCity ?? "" },
                    AvailableSeats = tripsQueryDTO.PeopleNumber
                };
                if (tripsQueryDTO.DateStart != null) { transThereFilters.DepartureDates = new List<DateTime> { DateTime.Parse(tripsQueryDTO.DateStart) }; }
                if (tripsQueryDTO.DateEnd != null) { transThereFilters.ArrivalDates = new List<DateTime> { DateTime.Parse(tripsQueryDTO.DateEnd) }; }

                var transThereQuery = new TransportGetQuery { filters = transThereFilters };

                var transportsThere = await _catalogQueryPublisher.GetTransports(transThereQuery);
                if (transportsThere.Count == 0) { continue; }
                var transportThere = transportsThere.FirstOrDefault();
                //


                // check for valid transport back
                var transBackFilters = new TransportQueryFilters
                {
                    CityOrigins = new List<string> { tripsQueryDTO.OriginCity ?? "" },
                    CityDestinations = new List<string> { tripsQueryDTO.DestinationCity ?? "" },
                    AvailableSeats = tripsQueryDTO.PeopleNumber
                };
                if (tripsQueryDTO.DateStart != null) { transThereFilters.DepartureDates = new List<DateTime> { DateTime.Parse(tripsQueryDTO.DateStart) }; }
                if (tripsQueryDTO.DateEnd != null) { transThereFilters.ArrivalDates = new List<DateTime> { DateTime.Parse(tripsQueryDTO.DateEnd) }; }

                var transBackQuery = new TransportGetQuery { filters = transThereFilters };

                var transportsBack = await _catalogQueryPublisher.GetTransports(transBackQuery);
                if (transportsBack.Count == 0) { continue; }
                var transportBack = transportsBack.FirstOrDefault();


                // Calculate Price
                var tripLength = 7;
                decimal price = 0;
                if (tripsQueryDTO.DateEnd != null && tripsQueryDTO.DateStart != null)
                {
                    tripLength = DateTime.Parse(tripsQueryDTO.DateEnd).Subtract(DateTime.Parse(tripsQueryDTO.DateStart)).Days;
                }
                price += tripLength * int.Parse(validRoomType.PricePerNight);
                if (tripsQueryDTO.PeopleNumber > 1) { price += 100 * tripsQueryDTO.PeopleNumber; }
                if (transportThere != null) { price += decimal.Parse(transportThere.PricePerTicket) * tripsQueryDTO.PeopleNumber; }
                if (transportBack != null) { price += decimal.Parse(transportBack.PricePerTicket) * tripsQueryDTO.PeopleNumber; }

                var tripDTO = new TripDTO
                {
                    HotelId = hotelResponse.Id,
                    RoomTypeId = validRoomType.Id,
                    DestinationCity = tripsQueryDTO.DestinationCity,
                    OriginCity = tripsQueryDTO.OriginCity,
                    DateStart = tripsQueryDTO.DateStart,
                    DateEnd = tripsQueryDTO.DateEnd,
                    Price = price.ToString(),
                    PeopleNumber = tripsQueryDTO.PeopleNumber
                };
                if (transportThere != null) { tripDTO.TransportThereId = transportThere.Id; }
                if (transportBack != null) { tripDTO.TransportBackId = transportBack.Id; }

                hotelTransportMatches.Add(tripDTO);
                var tripDTOJson = JS.Serialize(tripDTO);
                _logger.Information($">|< Get() :: TripDTO {tripDTOJson}");
            }

            return hotelTransportMatches;
        }



    }
}
