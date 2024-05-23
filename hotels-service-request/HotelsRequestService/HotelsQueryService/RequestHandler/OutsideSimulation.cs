using HotelsRequestService.Data;
using HotelsRequestService.QueryHandler;
using Microsoft.EntityFrameworkCore;

namespace HotelsRequestService.RequestHandler
{
    public class OutsideSimulation : IHostedService
    {
        private readonly HotelsRequestHandler _hotelsRequestHandler;
        private readonly IDbContextFactory<ApiDbContext> _contextFactory;
        private readonly Serilog.ILogger _logger;

        public OutsideSimulation(HotelsRequestHandler hotelsRequestHandler, IDbContextFactory<ApiDbContext> contextFactory, Serilog.ILogger logger)
        {
            _hotelsRequestHandler = hotelsRequestHandler;
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async void Run()
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

                    var result = _hotelsRequestHandler.ReserveMock(randomHotel.Id, randomRoom.RoomNumber, start, end);
                }
                catch (Exception e)
                {
                    _logger.Information($"Failed to reserve room.");
                    Task.Delay(5000).Wait();
                }
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Run();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
