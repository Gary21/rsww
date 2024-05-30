using AutoMapper;
using CatalogQueryService.DTOs;
using CatalogQueryService.Filters;
using CatalogQueryService.Queries;
using CatalogQueryService.QueryPublishers;
using CatalogRequestService.DTOs;
using MessagePack;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitUtilities;
using System.Text;
using ConsumerConfig = RabbitUtilities.Configuration.ConsumerConfig;


namespace CatalogQueryService.QueryHandler
{
    public class CatalogQueryHandler : ConsumerServiceBase
    {
        private readonly CatalogQueryPublisher _catalogQueryPublisher;
        private readonly IMapper _mapper;

        public CatalogQueryHandler(Serilog.ILogger logger, IConfiguration config, IConnectionFactory connectionFactory, PublisherServiceBase catalogQueryPublisher, IMapper mapper, IHostApplicationLifetime appLifeTime)
            : base(logger, connectionFactory, config.GetSection("CatalogQueryPublisher").Get<ConsumerConfig>()!, appLifeTime)
        {
            _catalogQueryPublisher = (CatalogQueryPublisher)catalogQueryPublisher;
            _mapper = mapper;
        }

        protected override void ConsumeMessage(object model, BasicDeliverEventArgs ea)
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
                case MessageType.GET:
                    GetCall(ea);
                    break;
                case MessageType.UPDATE:
                    _logger.Information($"Received getlistof");
                    GetListOf(ea);
                    break;
                default:
                    _logger.Information($"Received message with unknown type.");
                    GetCall(ea);
                    break;
            }
        }

        public async void GetListOf(BasicDeliverEventArgs ea)
        {
            var message = MessagePackSerializer.Deserialize<string>(ea.Body.ToArray());
            if (message is null) { _logger.Information($"Received message null."); }

            if (message == "countries")
            {
                var countries = await _catalogQueryPublisher.GetCountries();
                var countriesDTO = _mapper.Map<List<CountryDTO>>(countries);
                var serialized = MessagePackSerializer.Serialize(countriesDTO);
                Reply(ea, serialized);
            }
            else if (message == "cities")
            {
                var cities = await _catalogQueryPublisher.GetCities();
                var citiesDTO = _mapper.Map<List<CityDTO>>(cities);
                var serialized = MessagePackSerializer.Serialize(citiesDTO);
                Reply(ea, serialized);
            }
            else if (message == "roomtypes")
            {
                var roomTypes = await _catalogQueryPublisher.GetRoomTypes();
                var roomTypeNames = roomTypes.Select(rt => rt.Name).ToList();
                var serialized = MessagePackSerializer.Serialize(roomTypeNames);
                Reply(ea, serialized);
            }
            else
            {
                _logger.Information($"Received message with unknown type.");
            }
        }

        public async void GetCall(BasicDeliverEventArgs ea)
        {
            var message = MessagePackSerializer.Deserialize<KeyValuePair<string, byte[]>>(ea.Body.ToArray());
            var callCode = message.Key;
            if (callCode is null) { _logger.Information($"Received message null."); return; }

            switch (callCode)
            {
                case "GetDestinations":
                    var cities = await _catalogQueryPublisher.GetCities();
                    var cityNames = cities.Select(c => c.Name).ToList();
                    var serialized = MessagePackSerializer.Serialize(cityNames);
                    Reply(ea, serialized);
                    break;


                case "GetHotels":
                    var HotelsQueyFiltersGateway = MessagePackSerializer.Deserialize<HotelsQueryFiltersGatway>(message.Value);

                    var hotelGetQuery = new HotelsGetQuery { filters = HotelsFiltersAdapter.GatewayHotelToHotel(HotelsQueyFiltersGateway) };

                    var tripGetQuery = new TripGetQuery();
                    tripGetQuery.filters = TripQueryFiltersAdapter.AdaptHotelQueryToTripQuery(hotelGetQuery.filters);

                    var citiesTwo = await _catalogQueryPublisher.GetCities();
                    var cityId = citiesTwo.FirstOrDefault(c => c.Name == HotelsQueyFiltersGateway.Destination)?.Id;
                    tripGetQuery.filters.CityIds = new List<int> { cityId.Value };

                    var hotels = await Get(tripGetQuery);
                    var serializedHotels = MessagePackSerializer.Serialize(hotels);
                    _logger.Information($"Hotels count: {hotels.Count}");
                    Reply(ea, serializedHotels);
                    break;


                case "GetHotel":
                    var hotelId = MessagePackSerializer.Deserialize<int>(message.Value);

                    var tripGetQueryTwo = new TripGetQuery();
                    tripGetQueryTwo.filters = new TripQueryFilters { HotelIds = new List<int> { hotelId } };

                    var hotel = await Get(tripGetQueryTwo);
                    var serializedHotel = MessagePackSerializer.Serialize(hotel);
                    Reply(ea, serializedHotel);
                    break;


                case "GetAvailability":
                    //var availabilityQuery = MessagePackSerializer.Deserialize<AvailabilityQuery>(message.Value);
                    //var availability = await _catalogQueryPublisher.GetAvailability(availabilityQuery);
                    var serializedAvailability = MessagePackSerializer.Serialize(true);
                    Reply(ea, serializedAvailability);
                    break;


                case "ValidateReservation":
                    var reservationQuery = MessagePackSerializer.Deserialize<ReservationQuery>(message.Value);
                    var tripGetQueryThree = new TripGetQuery();
                    tripGetQueryThree.filters = TripQueryFiltersAdapter.AdaptReservationQueryToTripQuery(reservationQuery);

                    var trips = await Get(tripGetQueryThree);

                    if (trips.Count == 0) { Reply(ea, MessagePackSerializer.Serialize(false)); }
                    else { Reply(ea, MessagePackSerializer.Serialize(true)); }

                    break;


                case "GetHotelRooms":
                    var hotelIdForRooms = MessagePackSerializer.Deserialize<int>(message.Value);
                    var rooms = await _catalogQueryPublisher.GetRoomTypesForHotelId(hotelIdForRooms);
                    var serializedRooms = MessagePackSerializer.Serialize(rooms);
                    Reply(ea, serializedRooms);
                    break;


                default:
                    _logger.Information($"Received message with unknown type.");
                    break;
            }
        }

        public async void Get(BasicDeliverEventArgs ea)
        {
            var message = MessagePackSerializer.Deserialize<TripGetQuery>(ea.Body.ToArray());
            var filt_ser = MessagePackSerializer.ConvertToJson(MessagePackSerializer.Serialize(message.filters));
            _logger.Information($"GET Trips {filt_ser}");

            var hotelTransportMatches = new List<TripDTO>();
            var allCities = await _catalogQueryPublisher.GetCities();

            var mf = message.filters ?? new TripQueryFilters();
            if (mf.CheckInDate != null) { mf.CheckInDate = mf.CheckInDate.Value.Date; }
            if (mf.CheckOutDate != null) { mf.CheckOutDate = mf.CheckOutDate.Value.Date; }
            if (mf.CheckInDate == null || mf.CheckOutDate == null)
            {
                mf.CheckInDate = null;
                mf.CheckOutDate = null;
            }

            var listOfCapacities = new List<int> { mf.PeopleNumber };

            var hotelGetQuery = new HotelsGetQuery
            {
                filters = new HotelQueryFilters
                {
                    HotelIds = mf.HotelIds,
                    CityIds = mf.CityIds,
                    CountryIds = mf.CountryIds,
                    RoomTypes = mf.RoomTypes,
                    RoomCapacities = listOfCapacities,
                    CheckInDate = mf.CheckInDate,
                    CheckOutDate = mf.CheckOutDate,
                    MinPrice = mf.MinPrice,
                    MaxPrice = mf.MaxPrice
                }
            };

            var hotels = await _catalogQueryPublisher.GetHotels(hotelGetQuery);

            if (hotels.Count == 0)
            {
                Reply(ea, MessagePackSerializer.Serialize(hotelTransportMatches));
                return;
            }

            foreach (var hotel in hotels)
            {
                _logger.Information($"Hotel: {hotel.Name}");

                var hotelCityName = allCities.FirstOrDefault(c => c.Id == hotel.CityId)?.Name;

                var match = new TripDTO(hotel.Id);
                var departureCityNames = mf.DepartureCityIds.Select(dc => allCities.FirstOrDefault(c => c.Id == dc)?.Name).ToList();


                var transportThereGetQuery = new TransportGetQuery
                {
                    filters = new TransportQueryFilters
                    {
                        CityOrigins = departureCityNames,
                        Types = mf.TransportTypes,
                        CityDestinations = new List<string> { hotelCityName },
                        AvailableSeats = mf.PeopleNumber
                    }
                };

                var transports = await _catalogQueryPublisher.GetTransports(transportThereGetQuery);

                if (transports.Count == 0)
                {
                    continue;
                }

                foreach (var transport in transports)
                {
                    match.TransportThereIds.Add(transport.Id);
                }

                var transportBackGetQuery = new TransportGetQuery
                {
                    filters = new TransportQueryFilters
                    {
                        CityOrigins = new List<string> { hotelCityName },
                        Types = mf.TransportTypes,
                        CityDestinations = new List<string> { departureCityNames.FirstOrDefault() },
                        AvailableSeats = mf.PeopleNumber
                    }
                };

                transports = await _catalogQueryPublisher.GetTransports(transportBackGetQuery);

                if (transports.Count == 0)
                {
                    continue;
                }

                foreach (var transport in transports)
                {
                    match.TransportBackIds.Add(transport.Id);
                }

                hotelTransportMatches.Add(match);


            }

            Reply(ea, MessagePackSerializer.Serialize(hotelTransportMatches));
        }

        public async Task<ICollection<GatewayHotelDTO>> Get(TripGetQuery tripGetQuery)
        {
            var message = tripGetQuery;

            var filt_ser = MessagePackSerializer.ConvertToJson(MessagePackSerializer.Serialize(message.filters));
            _logger.Information($"GET Trips {filt_ser}");

            var hotelTransportMatches = new List<TripDTO>();
            var allCities = await _catalogQueryPublisher.GetCities();

            var mf = message.filters ?? new TripQueryFilters();
            if (mf.CheckInDate != null) { mf.CheckInDate = mf.CheckInDate.Value.Date; }
            if (mf.CheckOutDate != null) { mf.CheckOutDate = mf.CheckOutDate.Value.Date; }
            if (mf.CheckInDate == null || mf.CheckOutDate == null)
            {
                mf.CheckInDate = null;
                mf.CheckOutDate = null;
            }

            var listOfCapacities = new List<int> { mf.PeopleNumber };

            var hotelGetQuery = new HotelsGetQuery
            {
                filters = new HotelQueryFilters
                {
                    HotelIds = mf.HotelIds,
                    CityIds = mf.CityIds,
                    CountryIds = mf.CountryIds,
                    RoomTypes = mf.RoomTypes,
                    RoomCapacities = listOfCapacities,
                    CheckInDate = mf.CheckInDate,
                    CheckOutDate = mf.CheckOutDate,
                    MinPrice = mf.MinPrice,
                    MaxPrice = mf.MaxPrice
                }
            };

            var hotels = await _catalogQueryPublisher.GetHotels(hotelGetQuery);

            if (hotels.Count == 0)
            {
                return hotels.Select(h => DTOAdapterHotelToGatewayHotel.Adapt(h)).ToList();
            }

            foreach (var hotel in hotels)
            {
                _logger.Information($"Hotel: {hotel.Name}");

                var hotelCityName = allCities.FirstOrDefault(c => c.Id == hotel.CityId)?.Name;

                var match = new TripDTO(hotel.Id);
                var departureCityNames = mf.DepartureCityIds.Select(dc => allCities.FirstOrDefault(c => c.Id == dc)?.Name).ToList();


                var transportThereGetQuery = new TransportGetQuery
                {
                    filters = new TransportQueryFilters
                    {
                        CityOrigins = departureCityNames,
                        Types = mf.TransportTypes,
                        CityDestinations = new List<string> { hotelCityName },
                        AvailableSeats = mf.PeopleNumber
                    }
                };

                var transports = await _catalogQueryPublisher.GetTransports(transportThereGetQuery);

                if (transports.Count == 0)
                {
                    continue;
                }

                foreach (var transport in transports)
                {
                    match.TransportThereIds.Add(transport.Id);
                }

                var transportBackGetQuery = new TransportGetQuery
                {
                    filters = new TransportQueryFilters
                    {
                        CityOrigins = new List<string> { hotelCityName },
                        Types = mf.TransportTypes,
                        CityDestinations = new List<string> { departureCityNames.FirstOrDefault() },
                        AvailableSeats = mf.PeopleNumber
                    }
                };

                transports = await _catalogQueryPublisher.GetTransports(transportBackGetQuery);

                if (transports.Count == 0)
                {
                    continue;
                }

                foreach (var transport in transports)
                {
                    match.TransportBackIds.Add(transport.Id);
                }

                hotelTransportMatches.Add(match);


            }

            return hotels.Select(h => DTOAdapterHotelToGatewayHotel.Adapt(h)).ToList();
        }


        public async Task<List<TripDTO>> GetTripsTest(TripGetQuery tripGetQuery)
        {
            var mf = tripGetQuery.filters ?? new TripQueryFilters();

            var hotelTransportMatches = new List<TripDTO>();
            var allCities = await _catalogQueryPublisher.GetCities();


            if (mf.CheckInDate != null) { mf.CheckInDate = mf.CheckInDate.Value.Date; }
            if (mf.CheckOutDate != null) { mf.CheckOutDate = mf.CheckOutDate.Value.Date; }
            if (mf.CheckInDate == null || mf.CheckOutDate == null)
            {
                mf.CheckInDate = null;
                mf.CheckOutDate = null;
            }

            var listOfCapacities = new List<int> { mf.PeopleNumber };

            var hotelGetQuery = new HotelsGetQuery
            {
                filters = new HotelQueryFilters
                {
                    HotelIds = mf.HotelIds,
                    CityIds = mf.CityIds,
                    CountryIds = mf.CountryIds,
                    RoomTypes = mf.RoomTypes,
                    RoomCapacities = listOfCapacities,
                    CheckInDate = mf.CheckInDate,
                    CheckOutDate = mf.CheckOutDate,
                    MinPrice = mf.MinPrice,
                    MaxPrice = mf.MaxPrice
                }
            };

            var hotels = await _catalogQueryPublisher.GetHotels(hotelGetQuery);

            if (hotels.Count == 0)
            {
                return null;
            }

            foreach (var hotel in hotels)
            {
                _logger.Information($"Hotel: {hotel.Name}");

                var hotelCityName = allCities.FirstOrDefault(c => c.Id == hotel.CityId)?.Name;

                var match = new TripDTO(hotel.Id);
                var departureCityNames = mf.DepartureCityIds.Select(dc => allCities.FirstOrDefault(c => c.Id == dc)?.Name).ToList();


                var transportThereGetQuery = new TransportGetQuery
                {
                    filters = new TransportQueryFilters
                    {
                        CityOrigins = departureCityNames,
                        Types = mf.TransportTypes,
                        CityDestinations = new List<string> { hotelCityName },
                        AvailableSeats = mf.PeopleNumber
                    }
                };

                var transports = await _catalogQueryPublisher.GetTransports(transportThereGetQuery);

                if (transports.Count == 0)
                {
                    continue;
                }

                foreach (var transport in transports)
                {
                    match.TransportThereIds.Add(transport.Id);
                }

                var transportBackGetQuery = new TransportGetQuery
                {
                    filters = new TransportQueryFilters
                    {
                        CityOrigins = new List<string> { hotelCityName },
                        Types = mf.TransportTypes,
                        CityDestinations = new List<string> { departureCityNames.FirstOrDefault() },
                        AvailableSeats = mf.PeopleNumber
                    }
                };

                transports = await _catalogQueryPublisher.GetTransports(transportBackGetQuery);

                if (transports.Count == 0)
                {
                    continue;
                }

                foreach (var transport in transports)
                {
                    match.TransportBackIds.Add(transport.Id);
                }

                hotelTransportMatches.Add(match);

                match.Price = mf.CheckOutDate.Value.Subtract(mf.CheckInDate.Value).Days * 1000 + mf.PeopleNumber * 100 +
                    transports.FirstOrDefault().PricePerTicket * mf.PeopleNumber + hotel.Stars * 100;
                match.Price = match.Price - DateTime.Now.Subtract(mf.CheckInDate.Value).Days * 5;

            }

            return hotelTransportMatches;
        }



        private async Task<List<TripDTO>> MatchTrips(ICollection<HotelDTO> hotels, ICollection<TransportDTO> transportsThere, ICollection<TransportDTO> transportsBack, TripQueryFilters tripQueryFilters)
        {
            var cities = await _catalogQueryPublisher.GetCities();

            List<TripDTO> hotelTransportMatches = new List<TripDTO>();

            foreach (var hotel in hotels)
            {
                var hotelCityName = cities.FirstOrDefault(c => c.Id == hotel.CityId)?.Name;
                var match = new TripDTO(hotel.Id);

                foreach (var transport in transportsThere)
                {



                }
            }

            return hotelTransportMatches;
        }
    }
}
