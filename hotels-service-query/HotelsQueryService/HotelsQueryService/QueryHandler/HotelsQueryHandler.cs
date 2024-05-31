using AutoMapper;
using HotelsQueryService.Data;
using HotelsQueryService.DTOs;
using HotelsQueryService.Entities;
using HotelsQueryService.Filters;
using HotelsQueryService.Queries;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitUtilities;
using System.Text;
using System.Text.Json;
using ConsumerConfig = RabbitUtilities.Configuration.ConsumerConfig;


namespace HotelsQueryService.QueryHandler
{
    public class HotelsQueryHandler : ConsumerServiceBase
    {
        private readonly IDbContextFactory<ApiDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public HotelsQueryHandler(Serilog.ILogger logger, IConfiguration config, IConnectionFactory connectionFactory, IDbContextFactory<ApiDbContext> repositoryFactory, IMapper mapper, IHostApplicationLifetime applicationLifetime)
            : base(logger, connectionFactory, config.GetSection("hotelsQueryConsumer").Get<ConsumerConfig>()!, applicationLifetime)
        {
            _contextFactory = repositoryFactory;
            _mapper = mapper;
            using var repo = repositoryFactory.CreateDbContext();
            repo.Database.EnsureCreated();
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
                    Get(ea);
                    break;
                case MessageType.UPDATE:
                    _logger.Information($"Received getlistof");
                    GetListOf(ea);
                    break;
                case MessageType.EVENT:
                    GetRoomTypesForHotelId(ea);
                    break;
                default:
                    _logger.Information($"Received message with unknown type.");
                    break;
            }
        }

        private async void GetListOf(BasicDeliverEventArgs ea)
        {
            using var repository = _contextFactory.CreateDbContext();
            var message = MessagePackSerializer.Deserialize<string>(ea.Body.ToArray());
            if (message == "countries")
            {
                _logger.Information($"=> GET - Countries");
                var countries = await repository.Countries.ToListAsync();
                var countriesDTO = _mapper.Map<List<CountryDTO>>(countries);
                var serialized = MessagePackSerializer.Serialize(countriesDTO);
                _logger.Information($"<= GET - Countries :: countries count {countriesDTO.Count}");
                Reply(ea, serialized);
            }
            else if (message == "cities")
            {
                _logger.Information($"=> GET - Cities");
                var cities = await repository.Cities.ToListAsync();
                var citiesDTO = _mapper.Map<List<CityDTO>>(cities);
                var serialized = MessagePackSerializer.Serialize(citiesDTO);
                _logger.Information($"<= GET - Cities :: cities count {citiesDTO.Count}");
                Reply(ea, serialized);
            }
            else if (message == "roomtypes")
            {
                _logger.Information($"=> GET - RoomTypes");
                var roomTypes = await repository.RoomTypes.ToListAsync();
                var roomTypeNames = roomTypes.Select(rt => rt.Name).ToList();
                var serialized = MessagePackSerializer.Serialize(roomTypeNames);
                _logger.Information($"<= GET - RoomTypes :: room types count {roomTypeNames.Count}");
                Reply(ea, serialized);
            }
            else
            {
                _logger.Information($"=> GET - Received message with unknown type.");
                Reply(ea, MessagePackSerializer.Serialize(true));
            }
        }

        private async void GetRoomTypesForHotelId(BasicDeliverEventArgs ea)
        {
            var message = MessagePackSerializer.Deserialize<int>(ea.Body.ToArray());
            _logger.Information($"=>| GET - RoomTypes for HotelId {message}");

            using var repository = _contextFactory.CreateDbContext();
            var hotel = await repository.Hotels.Include(h => h.Rooms).ThenInclude(r => r.RoomType).FirstOrDefaultAsync(h => h.Id == message);
            if (hotel == null)
            {
                Reply(ea, MessagePackSerializer.Serialize(new List<string>()));
                return;
            }
            var roomTypes = hotel.Rooms.Select(r => r.RoomType.Name + " (" + r.RoomType.Capacity + ")").Distinct().ToList();
            //var roomTypesDTO = _mapper.Map<List<RoomTypeDTO>>(roomTypes);
            var roomTypesJson = JsonSerializer.Serialize(roomTypes);
            _logger.Information($"<=| GET - RoomTypes for HotelId {message} :: room types count {roomTypes.Count},\n room types: {roomTypesJson}");

            var serialized = MessagePackSerializer.Serialize(roomTypes);
            Reply(ea, serialized);
        }

        private async void Get(BasicDeliverEventArgs ea)
        {
            using var repository = _contextFactory.CreateDbContext();
            var message = MessagePackSerializer.Deserialize<HotelsGetQuery>(ea.Body.ToArray());
            var filt_ser = MessagePackSerializer.ConvertToJson(MessagePackSerializer.Serialize(message.filters));
            _logger.Information($"=>| GET - Hotels {filt_ser}");

            var mf = message.filters ?? new HotelQueryFilters();
            if (mf.RoomCapacities != null && mf.RoomCapacities.Count() == 1 && mf.RoomCapacities.FirstOrDefault() == 1) { mf.RoomCapacities = null; }
            //if (mf.CheckInDate != null) { mf.CheckInDate = mf.CheckInDate.Value.Date; }
            //if (mf.CheckOutDate != null) { mf.CheckOutDate = mf.CheckOutDate.Value.Date; }
            //if (mf.CheckInDate == null || mf.CheckOutDate == null)
            //{
            //    mf.CheckInDate = null;
            //    mf.CheckOutDate = null;
            //}


            var query = from h in repository.Hotels
                            //join room in repository.Rooms on h.Id equals room.HotelId
                            //join occupancy in repository.Occupancies on room equals occupancy.Room
                        join city in repository.Cities on h.City equals city
                        join country in repository.Countries on city.Country equals country

                        where mf.HotelIds == null || mf.HotelIds.Count() == 0 || mf.HotelIds.Contains(h.Id)
                        where mf.CountryIds == null || mf.CountryIds.Count() == 0 || mf.CountryIds.Contains(country.Id)
                        where mf.CityIds == null || mf.CityIds.Count() == 0 || mf.CityIds.Contains(city.Id)

                        where
                                h.Rooms.Any(r =>
                                    (
                                        mf.RoomTypes == null || mf.RoomTypes.Count() == 0 ||
                                        mf.RoomTypes.Contains(r.RoomType.Name)
                                        ) &&

                                    (
                                        mf.MinPrice == null || r.BasePrice >= mf.MinPrice
                                        ) &&

                                    (
                                        mf.MaxPrice == null || r.BasePrice <= mf.MaxPrice
                                        ) &&

                                    (
                                        mf.RoomCapacities == null || mf.RoomCapacities.Count() == 0 ||
                                        (r.RoomType.Capacity >= mf.RoomCapacities.Min() && r.RoomType.Capacity <= mf.RoomCapacities.Max())
                                        ) &&

                                    !r.Occupancies.Any(o => o.Date >= mf.CheckInDate && o.Date <= mf.CheckOutDate)
                                )

                        select new { h, city, country };


            try
            {
                var result = await query.ToListAsync();
                var hotelsDTO = result.Select(r => new HotelDTO
                {
                    Id = r.h.Id,
                    Name = r.h.Name,
                    Description = r.h.Description,
                    Rating = r.h.Rating,
                    Stars = r.h.Stars,
                    Address = r.h.Address,
                    CityId = r.city.Id,
                    CityName = r.city.Name,
                    CountryId = r.country.Id,
                    CountryName = r.country.Name,
                    ImgPaths = r.h.ImgPaths
                }).ToList();

                var hotelNames = hotelsDTO.Select(h => h.Name).ToList();
                _logger.Information($"<= GET - Hotels :: hotels count {hotelsDTO.Count},\n hotels: {string.Join(", ", hotelNames)}");

                Reply(ea, MessagePackSerializer.Serialize(hotelsDTO));
                return;

            }
            catch (Exception e)
            {
                _logger.Information($"<= GET - Hotels :: Exception 1 :: {e.Message}");
                Reply(ea, MessagePackSerializer.Serialize(new List<HotelDTO>()));
                return;
            }
        }
    }
}
