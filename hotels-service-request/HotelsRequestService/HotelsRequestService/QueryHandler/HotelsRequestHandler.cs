using MessagePack;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitUtilities;
using Serilog;
using System.Text;
using HotelsRequestService.Entities;
using HotelsRequestService.Filters;
using HotelsRequestService.Queries;
using HotelsRequestService.Data;
using Microsoft.EntityFrameworkCore;


namespace HotelsRequestService.QueryHandler
{
    public class HotelsRequestHandler : ConsumerServiceBase
    {
        private readonly ApiDbContext _context;

        public HotelsRequestHandler(Serilog.ILogger logger, IConfiguration config, IConnectionFactory connectionFactory, ApiDbContext context) 
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


            //GetFromDB

            var hotels = await _context.Hotels
                .Include(h => h.HasRooms)
                .Include(h => h.City)
                    .ThenInclude(c => c.Country)
                .ToListAsync();

            var serialized = MessagePackSerializer.Serialize(hotels);
            Reply(ea, serialized);
        }

    }
}
