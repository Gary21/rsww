using AutoMapper;
using HotelsRequestService.Data;
using HotelsRequestService.Queries;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitUtilities;
using System.Text;
using ConsumerConfig = RabbitUtilities.Configuration.ConsumerConfig;
using System.Collections.Generic;
using HotelsRequestService.Entities;


namespace HotelsRequestService.QueryHandler
{
    public class HotelsRequestHandler : ConsumerServiceBase
    {
        private readonly IDbContextFactory<ApiDbContext> _contextFactory;
        private readonly int CODE_FAILED_UPDATE = -1;
        private readonly int CODE_NOT_FOUND = -2;

        public HotelsRequestHandler(Serilog.ILogger logger, IConfiguration config, IConnectionFactory connectionFactory, IDbContextFactory<ApiDbContext> repositoryFactory) 
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
            bool success = false;

            // with message
            _logger.Information($"Received message with type RESERVE: {message.HotelId}, {message.RoomTypeId}, {message.CheckInDate}, {message.CheckOutDate}");

            List<DateTime> dateSeries = new List<DateTime>();
            for (DateTime date = message.CheckInDate; date <= message.CheckOutDate; date = date.AddDays(1))
            {
                dateSeries.Add(date.Date);
            }

            int result = 0;
            int tries = 0;
            while (!success)
            {
                var repository = _contextFactory.CreateDbContext();
                var room = await repository.Rooms
                    .Where(r => r.HotelId == message.HotelId)
                    .Where(r => r.RoomType.Id == message.RoomTypeId)
                    .Where(r => r.Occupancies.All(o => !dateSeries.Contains(o.Date.Date)))
                    .FirstOrDefaultAsync();

                if (room == null)
                {
                    _logger.Information($"Room not found.");
                    Reply(ea, MessagePackSerializer.Serialize<int>(CODE_NOT_FOUND));
                    return;
                }
                try
                {
                    repository.Occupancies.AddRange(dateSeries.Select(date => new Occupancy
                    {
                        HotelId = room.HotelId,
                        RoomNumber = room.RoomNumber,
                        Date = date,
                        ReservationId = message.ReservationId,
                        Room = room
                    }));
                    result = await repository.SaveChangesAsync();

                } catch (DbUpdateException e)
                {
                    _logger.Information($"Failed to update database: {e.Message}");
                    if (tries < 3)
                    {
                        tries++;
                        continue;
                    }
                    else
                    {
                        _logger.Information($"Failed to reserve room.");
                        Reply(ea, MessagePackSerializer.Serialize<int>(CODE_FAILED_UPDATE));
                        return;
                    }
                }
                success = true;
                Reply(ea, MessagePackSerializer.Serialize<int>(result));
            }
        }
    }
}
