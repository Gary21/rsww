using HotelsRequestService.Queries;
using HotelsRequestService.Requests;
using MessagePack;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using RabbitUtilities;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SimulationService
{
    public class SimulationBackgroundService : BackgroundService
    {

        private readonly PublisherServiceBase _publisherService;
        private readonly ILogger _logger;
        private IEnumerable<string> destinations;

        public SimulationBackgroundService(PublisherServiceBase publisherService, ILogger logger)
        {
            _publisherService = publisherService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var data = MessagePackSerializer.Serialize("");
            var payload = new KeyValuePair<string, byte[]>("GetDestinations", data);
            var messageId = _publisherService.PublishRequestWithReply("catalog", "query", MessageType.GET, payload);
            var bytes = await _publisherService.GetReply(messageId, stoppingToken);
            destinations = MessagePackSerializer.Deserialize<IEnumerable<string>>(bytes);


            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(2000);

                if (Random.Shared.NextDouble() < 0.05)
                    await UpdateHotelAvailability(stoppingToken);
                else
                    await UpdateHotelPrices(stoppingToken);

                await UpdateTransportPrices(stoppingToken);

                await ReserveTours(stoppingToken);
            }
        }

        private async Task UpdateHotelPrices(CancellationToken stoppingToken)
        {
            var newPrice = new decimal(Random.Shared.NextDouble() * 200 - 100);
            var updateRequest = new HotelUpdateRequest { HotelId = 1, AvailabilityChange = false, PriceChange = newPrice, RoomTypeId = 1 };
            var result = MessagePackSerializer.Deserialize<bool>(await _publisherService.GetReply(
                _publisherService.PublishRequestWithReply("resources/hotels", "request", MessageType.UPDATE, updateRequest
                ), stoppingToken));
            _logger.Information($"Updated price of hotel ID: {updateRequest.HotelId}");
        }

        private async Task UpdateHotelAvailability(CancellationToken stoppingToken)
        {
            var updateRequest = new HotelUpdateRequest { HotelId = 1, AvailabilityChange = false, PriceChange = 0, RoomTypeId = 0 };
            var result = MessagePackSerializer.Deserialize<bool>(await _publisherService.GetReply(
                _publisherService.PublishRequestWithReply("resources/hotels", "request", MessageType.UPDATE, updateRequest
                ), stoppingToken));
            _logger.Information($"Updated availability of hotel ID: {updateRequest.HotelId}");
        }

        private async Task UpdateTransportPrices(CancellationToken stoppingToken)
        {
            var newPrice = new decimal(Random.Shared.NextDouble() * 200 - 100);
            var updateRequest = new TransportUpdateRequest { Id = 1, SeatsChange = 1, PriceChange = newPrice };
            var result = MessagePackSerializer.Deserialize<bool>(await _publisherService.GetReply(
                _publisherService.PublishRequestWithReply("resources/transport", "request", MessageType.UPDATE, updateRequest
                ), stoppingToken));
            _logger.Information($"Updated price of transport ID: {updateRequest.Id}");
        }

        // INFO HOTELE
        // INFO TRANSPORTY
        // REZERWACJA HOTELU + TRANSPORT DLA KONKRETNEGO HOTELU
        // EW. KUPNO
        private async Task ReserveTours(CancellationToken stoppingToken)
        {
            TripDTO query = new TripDTO() { DestinationCity = destinations.ElementAt(Random.Shared.Next(0,destinations.Count())), PeopleNumber = Random.Shared.Next(1,4)};
            var data = MessagePackSerializer.Serialize(query);
            var payload = new KeyValuePair<string, byte[]>("GetTrips", data);
            //var hotelQueryPayload = MessagePackSerializer.Serialize(query);
            var messageId = _publisherService.PublishRequestWithReply("catalog", "query", MessageType.GET, payload);
            var bytes = await _publisherService.GetReply(messageId, stoppingToken);
            var hotels = MessagePackSerializer.Deserialize<IEnumerable<TripDTO>>(bytes);

            var rand = new Random().Next(0,hotels.Count());
            var chosentrip = hotels.ElementAt(rand);

            var payload2 = new KeyValuePair<string, byte[]>("MakeReservation", MessagePackSerializer.Serialize(chosentrip));
            var messageId2 = _publisherService.PublishRequestWithReply("catalog", "request", MessageType.RESERVE, payload2);
            var bytes2 = await _publisherService.GetReply(messageId2, stoppingToken);
            var reservation = MessagePackSerializer.Deserialize<int>(bytes2);


            var data3 = MessagePackSerializer.Serialize(reservation);
            var payload3 = new KeyValuePair<string, byte[]>("BuyReservation", data3);
            var messageId3 = _publisherService.PublishRequestWithReply("catalog", "request", MessageType.ADD, payload3);
            var bytes3 = await _publisherService.GetReply(messageId3, stoppingToken);
            var purchase = MessagePackSerializer.Deserialize<bool>(bytes3);

        }

    }



    [MessagePackObject]
    public class TripDTO
    {
        [Key(0)] public int HotelId { get; set; }
        [Key(1)] public int RoomTypeId { get; set; }
        [Key(2)] public int TransportThereId { get; set; }
        [Key(3)] public int TransportBackId { get; set; }
        [Key(4)] public string DestinationCity { get; set; }
        [Key(5)] public string OriginCity { get; set; }
        [Key(6)] public string DateStart { get; set; }
        [Key(7)] public string DateEnd { get; set; }
        [Key(8)] public string Price { get; set; }
        [Key(9)] public int PeopleNumber { get; set; }
    }
}
