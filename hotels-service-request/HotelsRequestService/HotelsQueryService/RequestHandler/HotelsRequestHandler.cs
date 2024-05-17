using AutoMapper;
using HotelsQueryService.Data;
using HotelsQueryService.DTOs;
using HotelsQueryService.Queries;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitUtilities;
using System.Text;
using ConsumerConfig = RabbitUtilities.Configuration.ConsumerConfig;


namespace HotelsQueryService.QueryHandler
{
    public class HotelsRequestHandler : ConsumerServiceBase
    {
        private readonly IDbContextFactory<ApiDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public HotelsRequestHandler(Serilog.ILogger logger, IConfiguration config, IConnectionFactory connectionFactory, IDbContextFactory<ApiDbContext> repositoryFactory, IMapper mapper) 
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
                case MessageType.RESERVE:
                    Reserve(ea);
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

        private async void Reserve(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = MessagePackSerializer.Deserialize<RoomReserveRequest>(body);
            bool accepted = false;
            bool success = false;

            int tries = 0;
            while (!success)
            {
                var repository = _contextFactory.CreateDbContext();
                var hotelEvents = await repository.HotelEvents.Where(he => he.HotelId == message.HotelId)
                    .Where(he => he.Date == message.CheckIn)
                    .ToListAsync();

                
            }
            Reply(ea, MessagePackSerializer.Serialize<bool>(accepted));
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
