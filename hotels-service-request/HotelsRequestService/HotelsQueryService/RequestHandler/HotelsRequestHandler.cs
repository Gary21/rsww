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
using HotelsRequestService.Requests;
using HotelsRequestService.Events;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Cryptography.Xml;


namespace HotelsRequestService.QueryHandler
{
    public class HotelsRequestHandler : ConsumerServiceBase
    {
        private readonly IDbContextFactory<ApiDbContext> _contextFactory;
        private readonly int CODE_FAILED_UPDATE = -1;
        private readonly int CODE_NOT_FOUND = -2;
        private readonly PublisherServiceBase _publisherService;

        public HotelsRequestHandler(Serilog.ILogger logger, IConfiguration config, IConnectionFactory connectionFactory, IDbContextFactory<ApiDbContext> repositoryFactory, IHostApplicationLifetime applicationLifetime, PublisherServiceBase publisherService) 
            : base(logger, connectionFactory, config.GetSection("hotelsQueryConsumer").Get<ConsumerConfig>()!, applicationLifetime)
        {
            _contextFactory = repositoryFactory;
            _publisherService = publisherService; 

            using var context = _contextFactory.CreateDbContext();
            context.Database.EnsureCreated();
            //SimulateOutside();
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
                case MessageType.RELEASE:
                    Release(ea); 
                    break;

                case MessageType.UPDATE:
                    Update(ea);
                    break;
                default:
                    _logger.Information($"Received message with unknown type.");
                    break;
            }
        }
        private async void Update(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = MessagePackSerializer.Deserialize<HotelUpdateRequest>(body);
            bool success = false;

            // with message
            _logger.Information($"Received message with type UPDATE: {message.HotelId}, {message.RoomTypeId}, {message.AvailabilityChange}, {message.PriceChange}");


            int result = 0;
            int tries = 0;
            while (!success)
            {
                var repository = _contextFactory.CreateDbContext();

                var hotel = await repository.Hotels.Where(h => h.Id == message.HotelId)
                    .Include(h => h.City)
                    .Include(h => h.City.Country)
                    .FirstOrDefaultAsync();
                var hotelEvents = await repository.HotelEvents
                    .Where(r => r.HotelId == message.HotelId)
                    .OrderBy(t => t.SequenceNumber)
                    .ToListAsync();

                //var roomPrice = (await repository.Rooms.Where(r => r.HotelId == message.HotelId).Where(r=>r.RoomType.Id == message.RoomTypeId).FirstOrDefaultAsync()).BasePrice;
                //bool availability = true;
                //foreach(var hotelEvent in hotelEvents.Where(r => r.RoomTypeId == message.RoomTypeId)) {
                //    roomPrice += hotelEvent.PriceChange;
                //    availability = hotelEvent.AvailabilityChange ? !availability : availability;
                //}

                int lastInSequence = hotelEvents.LastOrDefault()?.SequenceNumber ?? 0;

                if (hotel == null)
                {
                    _logger.Information($"Hotel not found.");
                    Reply(ea, MessagePackSerializer.Serialize<bool>(false));
                    return;
                }

                tries += 1;
                try
                {
                    if (repository.HotelEvents.Any(e => e.HotelId == hotel.Id && e.SequenceNumber == lastInSequence + 1))
                    {
                        success = false;
                        _logger.Warning($"Event for hotel {message.HotelId} with same sequence number {lastInSequence + 1} already exists, trying again [tryNum:{tries}]");
                        repository.Dispose();
                        await Task.Delay(1);
                        continue;
                    }
                    repository.HotelEvents.Add(
                        new HotelEvent()
                        {
                            HotelId = hotel.Id,
                            RoomTypeId = message.RoomTypeId,
                            SequenceNumber = lastInSequence+1,
                            AvailabilityChange = message.AvailabilityChange,
                            PriceChange = message.PriceChange
                        });
                    
                    result = await repository.SaveChangesAsync();
                    _logger.Information($"Successfuly Updated hotel: {hotel.Id} with Availability: {message.AvailabilityChange},RoomType:{message.RoomTypeId}, PriceChange: {message.PriceChange} ");
                    repository.Dispose();
                }
                catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
                {
                    success = false;
                    _logger.Warning($"Event for hotel {message.HotelId} with same sequence number {lastInSequence + 1} already exists, trying again [tryNum:{tries}]");
                    repository.Dispose();
                    await Task.Delay(1);
                    continue;
                }
                catch (DbUpdateException e)
                {
                    _logger.Information($"Failed to update database: {e.Message}");
                    if (tries < 3)
                    {
                        //tries++;
                        continue;
                    }
                    else
                    {
                        _logger.Information($"Failed to update Hotel.");
                        Reply(ea, MessagePackSerializer.Serialize<bool>(false));
                        return;
                    }
                }
                success = true;

                var hotelChangeEvent = new KeyValuePair<string, byte[]>("hotel",
                    MessagePackSerializer.Serialize(new HotelChangeEvent()
                {

                    Id = message.HotelId,
                    RoomType = message.RoomTypeId.ToString(),
                    HotelName = hotel.Name,
                    DestinationCity = hotel.City.Name,
                    DestinationCountry = hotel.City.Country.Name,
                    Availability = message.AvailabilityChange.ToString(),
                    Price = message.PriceChange
                    
                }));
                _publisherService.PublishToFanoutNoReply("event", MessageType.UPDATE, hotelChangeEvent);

                Reply(ea, MessagePackSerializer.Serialize<bool>(true));
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
                    .Include(r=>r.Hotel)
                    .Include(r => r.Hotel.City)
                    .Include(r => r.Hotel.City.Country)
                    .Include(r=>r.RoomType)
                    .FirstOrDefaultAsync();
                
                if (room == null)
                {
                    _logger.Information($"Room not found.");
                    Reply(ea, MessagePackSerializer.Serialize<int>(-1));
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
                    _logger.Information($"Successfuly Reserved Room:{room.RoomNumber} for {dateSeries.Count()} days.");
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
                        Reply(ea, MessagePackSerializer.Serialize<int>(-2));
                        return;
                    }
                }
                success = true;
                var roomReservationEvent = new KeyValuePair<string, byte[]>("hotel",
                    MessagePackSerializer.Serialize(new HotelReservationEvent()
                {
                    Id = room.HotelId,
                    RoomType = room.RoomType.Name,
                    HotelName = room.Hotel.Name,
                    DestinationCity = room.Hotel.City.Name,
                    DestinationCountry = room.Hotel.City.Country.Name,
                    RoomCount = 1
                }));
                _publisherService.PublishToFanoutNoReply("event",MessageType.RESERVE, roomReservationEvent);

                Reply(ea, MessagePackSerializer.Serialize<int>(room.RoomNumber));
            }
        }


        private async void Release(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = MessagePackSerializer.Deserialize<RoomReleaseRequest>(body);
            bool success = false;

            // with message
            _logger.Information($"Received message with type RELEASE: {message.HotelId}, {message.RoomNumber}, {message.CheckInDate}, {message.CheckOutDate}");

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
                //var room = await repository.Rooms
                //    .Where(r => r.HotelId == message.HotelId)
                //    .Where(r => r.RoomNumber == message.RoomNumber)
                //    .FirstOrDefaultAsync();
                

                var occupancies = await repository.Occupancies
                    .Where(o => o.HotelId == message.HotelId)
                    .Where(o => o.RoomNumber == message.RoomNumber)
                    .Where(o => dateSeries.Contains(o.Date)).ToListAsync();


                if (!occupancies?.Any() ?? true)
                {
                    _logger.Information($"Occupancies not found.");
                    Reply(ea, MessagePackSerializer.Serialize<int>(-1));
                    return;
                }
                try
                {
            
                    foreach(var occupancy in occupancies) /*int i = 0; i < occupancies.Count(); i++*/
                    {
                        repository.Occupancies.Remove(occupancy);
                    }
                    result = await repository.SaveChangesAsync();
                    _logger.Information($"Successfuly RolledBack Reservation for Room:{message.RoomNumber} for {dateSeries.Count()} days.");
                }
                catch (DbUpdateException e)
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
                        Reply(ea, MessagePackSerializer.Serialize<int>(-2));
                        return;
                    }
                }
                success = true;
                Reply(ea, MessagePackSerializer.Serialize<int>(result));
            }
        }

        public async Task<int> ReserveMock(int hotelId, int roomNumber, DateTime start, DateTime end)
        {
            // with message
            _logger.Information($"Received message with type RESERVE: {hotelId}, {roomNumber}, {start}, {end}");

            List<DateTime> dateSeries = new List<DateTime>();
            for (DateTime date = start; date <= end; date = date.AddDays(1))
            {
                dateSeries.Add(date.Date);
            }

            bool success = false;
            int result = 0;
            int tries = 0;
            while (!success)
            {
                var repository = _contextFactory.CreateDbContext();

                try
                {
                    var room = await repository.Rooms
                        .Where(r => r.HotelId == hotelId)
                        .Where(r => r.RoomNumber == roomNumber)
                        .Where(r => !r.Occupancies.Any(o => dateSeries.Contains(o.Date.Date)))
                        .FirstOrDefaultAsync();

                    if (room == null)
                    {
                        _logger.Information($"Room not found.");
                        return CODE_NOT_FOUND;
                    }

                    repository.Occupancies.AddRange(dateSeries.Select(date => new Occupancy
                    {
                        HotelId = hotelId,
                        RoomNumber = roomNumber,
                        Date = date,
                        ReservationId = 0,
                        Room = room
                    }));
                    result = await repository.SaveChangesAsync();
                }
                catch (Exception e)
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
                        return CODE_FAILED_UPDATE;
                    }
                }

                success = true;
                
            }
            return result;
        }


        private async void SimulateOutside()
        {
            while (true)
            {
                try
                {
                    using var repository = _contextFactory.CreateDbContext();
                    var hotels = await repository.Hotels.ToListAsync();

                    var random = new Random();
                    var randomHotel = hotels[random.Next(hotels.Count)];

                    var rooms = await repository.Rooms.Where(r => r.HotelId == randomHotel.Id).ToListAsync();
                    var randomRoom = rooms[random.Next(rooms.Count)];

                    var start = DateTime.Now.Date.AddDays(random.Next(1, 10));
                    var end = start.AddDays(random.Next(2, 7));

                    var result = ReserveMock(randomHotel.Id, randomRoom.RoomNumber, start, end);

                    Task.Delay(5000).Wait();
                }
                catch (Exception e)
                {
                    _logger.Information($"Failed to reserve room.");
                    Task.Delay(5000).Wait();
                }
            }
        }
    }
}
