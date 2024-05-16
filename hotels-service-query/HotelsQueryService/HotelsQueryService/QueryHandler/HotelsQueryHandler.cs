using HotelsQueryService.Data;
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

        public HotelsQueryHandler(Serilog.ILogger logger, IConfiguration config, IConnectionFactory connectionFactory, IDbContextFactory<ApiDbContext> repositoryFactory) 
            : base(logger, connectionFactory, config.GetSection("hotelsQueryConsumer").Get<ConsumerConfig>()!)
        {
            _contextFactory = repositoryFactory;
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
                case MessageType.ADD:
                    _logger.Information($"Received message with type POST.");
                    //Reserve(ea);
                    break;
                default:
                    _logger.Information($"Received message with unknown type.");
                    break;
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

            var hotels = await repository.Hotels
                .Include(h => h.City)
                    .ThenInclude(c => c.Country)
                .Where(h => message.filters.HotelIds.Contains(h.Id) || message.filters.HotelIds.Count() == 0)
                .Where(h => message.filters.CityIds.Contains(h.City.Id) || message.filters.CityIds.Count() == 0)
                .Where(h => message.filters.CountryIds.Contains(h.City.Country.Id) || message.filters.CountryIds.Count() == 0)

                .Include(h => h.Rooms)
                    .ThenInclude(r => r.RoomType)
                .Where(h => h.Rooms.Any(r => message.filters.RoomTypeIds.Contains(r.RoomType.Id)) || message.filters.RoomTypeIds.Count() == 0)
                .Where(h => h.Rooms.Any(r => message.filters.RoomCapacities.Contains(r.RoomType.Capacity)) || message.filters.RoomCapacities.Count() == 0)

                .Include(h => h.Rooms)
                    .ThenInclude(r => r.BasePrice)
                .ToListAsync();

            var serialized = MessagePackSerializer.Serialize(hotels);
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
