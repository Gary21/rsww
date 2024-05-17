using AutoMapper;
using HotelsQueryService.Data;
using HotelsQueryService.DTOs;
using HotelsQueryService.Queries;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitUtilities;
using System.Text;
using ConsumerConfig = RabbitUtilities.Configuration.ConsumerConfig;


namespace HotelsQueryService.QueryHandler
{
    public class HotelsQueryHandler : ConsumerServiceBase
    {
        private readonly IDbContextFactory<ApiDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public HotelsQueryHandler(Serilog.ILogger logger, IConfiguration config, IConnectionFactory connectionFactory, IDbContextFactory<ApiDbContext> repositoryFactory, IMapper mapper) 
            : base(logger, connectionFactory, config.GetSection("hotelsQueryConsumer").Get<ConsumerConfig>()!)
        {
            _contextFactory = repositoryFactory;
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
                    Get(ea);
                    break;
                case MessageType.UPDATE:
                    _logger.Information($"Received getlistof");
                    GetListOf(ea);
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
                var countries = await repository.Countries.ToListAsync();
                var countriesDTO = _mapper.Map<List<CountryDTO>>(countries);
                var serialized = MessagePackSerializer.Serialize(countriesDTO);
                Reply(ea, serialized);
            }
            else if (message == "cities")
            {
                var cities = await repository.Cities.ToListAsync();
                var citiesDTO = _mapper.Map<List<CityDTO>>(cities);
                var serialized = MessagePackSerializer.Serialize(citiesDTO);
                Reply(ea, serialized);
            }
            else if (message == "roomtypes")
            {
                var roomTypes = await repository.RoomTypes.ToListAsync();
                var roomTypesDTO = _mapper.Map<List<RoomTypeDTO>>(roomTypes);
                var serialized = MessagePackSerializer.Serialize(roomTypesDTO);
                Reply(ea, serialized);
            }
            else
            {
                _logger.Information($"Received message with unknown type.");
            }
        }

        private async void Get(BasicDeliverEventArgs ea)
        {
            using var repository = _contextFactory.CreateDbContext();
            var message = MessagePackSerializer.Deserialize<HotelsGetQuery>(ea.Body.ToArray());
            var filt_ser = MessagePackSerializer.ConvertToJson(MessagePackSerializer.Serialize(message.filters));
            _logger.Information($"GET Hotels {filt_ser}");
            //message.filters
            //message.sort

            var query = from hotel in repository.Hotels
                               join city in repository.Cities on hotel.City equals city
                               join country in repository.Countries on city.Country equals country
                        where message.filters.HotelIds == null || message.filters.HotelIds.Contains(hotel.Id) || message.filters.HotelIds.Count() == 0
                        where message.filters.CountryIds == null || message.filters.CountryIds.Contains(country.Id) || message.filters.CountryIds.Count() == 0
                        where message.filters.CityIds == null || message.filters.CityIds.Contains(city.Id) || message.filters.CityIds.Count() == 0
                        where message.filters.RoomTypeIds == null || hotel.Rooms.Any(r => message.filters.RoomTypeIds.Contains(r.RoomType.Id)) || message.filters.RoomTypeIds.Count() == 0
                        where message.filters.RoomCapacities == null || hotel.Rooms.Any(r => message.filters.RoomCapacities.Contains(r.RoomType.Capacity)) || message.filters.RoomCapacities.Count() == 0
                        where message.filters.MinPrice == null || hotel.Rooms.Any(hotel => hotel.BasePrice >= message.filters.MinPrice) || message.filters.MinPrice == 0
                        where message.filters.MaxPrice == null || hotel.Rooms.Any(hotel => hotel.BasePrice <= message.filters.MaxPrice) || message.filters.MaxPrice == 0
                        select new { hotel, city, country };

            var result = await query.ToListAsync();
            var hotelsDTO = result.Select(r => new HotelDTO
            {
                Id = r.hotel.Id,
                Name = r.hotel.Name,
                Description = r.hotel.Description,
                Address = r.hotel.Address,
                CityId = r.city.Id,
                CityName = r.city.Name,
                CountryId = r.country.Id,
                CountryName = r.country.Name,
                ImgPaths = r.hotel.ImgPaths
            }).ToList();


                //.Where(h => message.filters.HotelIds.Contains(h.Id) || message.filters.HotelIds.Count() == 0)
                //.Where(h => message.filters.CityIds.Contains(h.City.Id) || message.filters.CityIds.Count() == 0)
                //.Where(h => message.filters.CountryIds.Contains(h.City.Country.Id) || message.filters.CountryIds.Count() == 0)

                //.Include(h => h.Rooms)
                //    .ThenInclude(r => r.RoomType)
                //.Where(h => h.Rooms.Any(r => message.filters.RoomTypeIds.Contains(r.RoomType.Id)) || message.filters.RoomTypeIds.Count() == 0)
                //.Where(h => h.Rooms.Any(r => message.filters.RoomCapacities.Contains(r.RoomType.Capacity)) || message.filters.RoomCapacities.Count() == 0)

                //.Include(h => h.Rooms)
                //    .ThenInclude(r => r.BasePrice)
                //.ToListAsync();

            //var hotels = await repository.Hotels.ToListAsync();

            var serialized = MessagePackSerializer.Serialize(hotelsDTO);
            Reply(ea, serialized);
        }

        //private async void Reserve(BasicDeliverEventArgs ea)
        //{
        //    var message = MessagePackSerializer.Deserialize<HotelsReserveQuery>(ea.Body.ToArray());
        //    _logger.Information($"POST Hotels {message}");

        //    var hasRoom = _context.Rooms
        //        .Include(r => r.RoomType)
        //        .Where(r => r.HotelId == message.HotelId)
        //        .Where(r => r.RoomNumber == message.RoomNumber)
        //        .FirstOrDefault();

        //    if (hasRoom == null)
        //    {
        //        _logger.Information($"Room with number {message.RoomNumber} not found.");
        //        return;
        //    }

        //}

    }
}
