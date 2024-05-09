using MessagePack;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitUtilities;
using Serilog;
using System.Text;
using HotelsQueryService.Entities;
using HotelsQueryService.Filters;
using HotelsQueryService.Queries;
using HotelsQueryService.Data;
using Microsoft.EntityFrameworkCore;


namespace HotelsQueryService.QueryHandler
{
    public class HotelsQueryHandler : ConsumerServiceBase
    {
        private readonly ApiDbContext _context;

        public HotelsQueryHandler(Serilog.ILogger logger, IConfiguration config, IConnectionFactory connectionFactory, ApiDbContext context) 
            : base(logger, connectionFactory, config.GetSection("hotelsQueryConsumer").Get<RabbitUtilities.Configuration.ConsumerConfig>()!)
        {
            _context = context;
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
                    Reserve(ea);
                    break;
                default:
                    _logger.Information($"Received message with unknown type.");
                    break;
            }
        }

        private async void Get(BasicDeliverEventArgs ea)
        {
            var message = MessagePackSerializer.Deserialize<HotelsGetQuery>(ea.Body.ToArray());
            var filt_ser = MessagePackSerializer.ConvertToJson(MessagePackSerializer.Serialize(message.filters));
            _logger.Information($"GET Hotels {filt_ser}");
            //message.filters
            //message.sort

            var hotels = _context.Hotels
                .Include(h => h.City)
                    .ThenInclude(c => c.Country)
                .Where(h => message.filters.HotelIds.Contains(h.Id) || message.filters.HotelIds.Count() == 0)
                .Where(h => message.filters.CityIds.Contains(h.City.Id) || message.filters.CityIds.Count() == 0)
                .Where(h => message.filters.CountryIds.Contains(h.City.Country.Id) || message.filters.CountryIds.Count() == 0)

                .Include(h => h.HasRooms)
                    .ThenInclude(r => r.RoomType)
                .Where(h => h.HasRooms.Any(r => message.filters.RoomTypeIds.Contains(r.RoomType.Id)) || message.filters.RoomTypeIds.Count() == 0)
                .Where(h => h.HasRooms.Any(r => message.filters.RoomCapacities.Contains(r.RoomType.Capacity)) || message.filters.RoomCapacities.Count() == 0)

                .Include(h => h.HasRooms)
                    .ThenInclude(r => r.BasePrice);


            var serialized = MessagePackSerializer.Serialize(hotels);
            Reply(ea, serialized);
        }

        private async void Reserve(BasicDeliverEventArgs ea)
        {
            var message = MessagePackSerializer.Deserialize<HotelsReserveQuery>(ea.Body.ToArray());
            _logger.Information($"POST Hotels {message}");

            var hasRoom = _context.HasRooms
                .Include(r => r.RoomType)
                .Where(r => r.Id == message.RoomId)
                .FirstOrDefault();

            if (hasRoom == null)
            {
                _logger.Information($"Room with id {message.RoomId} not found.");
                return;
            }

            var reservation = new Reservation
            {
                HasRoom = hasRoom,
                CheckIn = message.CheckIn,
                CheckOut = message.CheckOut,
                GuestName = message.GuestName,
                GuestEmail = message.GuestEmail
            };

        }

    }
}
