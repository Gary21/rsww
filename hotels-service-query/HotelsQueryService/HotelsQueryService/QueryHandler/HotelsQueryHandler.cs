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
using ConsumerConfig = RabbitUtilities.Configuration.ConsumerConfig;
using JS = System.Text.Json.JsonSerializer;
using MPS = MessagePack.MessagePackSerializer;

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
            var message = MPS.Deserialize<string>(ea.Body.ToArray());
            if (message == "countries")
            {
                _logger.Information($"=> GET - Countries");
                var countries = await repository.Countries.ToListAsync();
                var countriesDTO = _mapper.Map<List<CountryDTO>>(countries);
                var serialized = MPS.Serialize(countriesDTO);
                _logger.Information($"<= GET - Countries :: countries count {countriesDTO.Count}");
                Reply(ea, serialized);
            }
            else if (message == "cities")
            {
                _logger.Information($"=> GET - Cities");
                var cities = await repository.Cities.ToListAsync();
                var citiesDTO = _mapper.Map<List<CityDTO>>(cities);
                var serialized = MPS.Serialize(citiesDTO);
                _logger.Information($"<= GET - Cities :: cities count {citiesDTO.Count}");
                Reply(ea, serialized);
            }
            else if (message == "roomtypes")
            {
                _logger.Information($"=> GET - RoomTypes");
                var roomTypes = await repository.RoomTypes.ToListAsync();
                var roomTypesDTOs = roomTypes.Select(RoomTypeDTO.FromEntity).ToList();

                _logger.Information($"<= GET - RoomTypes :: room types count {roomTypesDTOs.Count}");

                Reply(ea, MPS.Serialize(roomTypesDTOs));
            }
            else
            {
                _logger.Information($"=> GET - Received message with unknown type.");
                Reply(ea, MPS.Serialize(true));
            }
        }

        private async void GetRoomTypesForHotelId(BasicDeliverEventArgs ea)
        {
            var message = MPS.Deserialize<int>(ea.Body.ToArray());

            _logger.Information($"=>| GET - RoomTypes for HotelId {message}");

            using var repository = _contextFactory.CreateDbContext();
            var hotel = await repository.Hotels.Include(h => h.Rooms).ThenInclude(r => r.RoomType).FirstOrDefaultAsync(h => h.Id == message);
            if (hotel == null) { Reply(ea, MPS.Serialize(new List<string>())); return; }

            var roomTypes = hotel.Rooms.Select(r => r.RoomType.Id).Distinct().ToList();
            List<RoomTypeDTO> roomTypesDTO = new List<RoomTypeDTO>();
            foreach (var roomType in roomTypes)
            {
                var roomTypeDTO = new RoomTypeDTO
                {
                    Id = roomType,
                    Name = hotel.Rooms.FirstOrDefault(r => r.RoomType.Id == roomType).RoomType.Name,
                    Capacity = hotel.Rooms.FirstOrDefault(r => r.RoomType.Id == roomType).RoomType.Capacity,
                    PricePerNight = hotel.Rooms.FirstOrDefault(r => r.RoomType.Id == roomType).BasePrice.ToString()
                };
                roomTypesDTO.Add(roomTypeDTO);
            }

            _logger.Information($"<=| GET - RoomTypes for HotelId {message} :: room types count {roomTypes.Count},\n room types: {JS.Serialize(roomTypesDTO)}");

            Reply(ea, MPS.Serialize(roomTypesDTO));
        }

        private async void Get(BasicDeliverEventArgs ea)
        {
            using var repository = _contextFactory.CreateDbContext();
            var message = MPS.Deserialize<HotelsGetQuery>(ea.Body.ToArray());
            var filt_ser = MPS.ConvertToJson(MPS.Serialize(message.filters));
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
                    Location = r.h.City.Name,
                    Rating = r.h.Rating.ToString(),
                    Stars = r.h.Stars.ToString(),
                    ImgPaths = r.h.ImgPaths
                }).ToList();

                _logger.Information($"<=| GET - Hotels :: hotels count {hotelsDTO.Count}");

                Reply(ea, MPS.Serialize(hotelsDTO));
                return;

            }
            catch (Exception e)
            {
                _logger.Information($"<= GET - Hotels :: Exception 1 :: {e.Message}");
                Reply(ea, MPS.Serialize(new List<HotelDTO>()));
                return;
            }
        }
    }
}
