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

namespace SimulationService
{
    public class SimulationBackgroundService : BackgroundService
    {

        private readonly PublisherServiceBase _publisherService;
        private readonly ILogger _logger;

        public SimulationBackgroundService(PublisherServiceBase publisherService, ILogger logger)
        {
            _publisherService = publisherService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(2000);

                if (Random.Shared.NextDouble() < 0.05)
                    await UpdateHotelAvailability(stoppingToken);
                else
                    await UpdateHotelPrices(stoppingToken);

                await UpdateTransportPrices(stoppingToken);
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
        private void ReserveTours()
        {

        }

    }
}
