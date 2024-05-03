using MessagePack;
using Microsoft.Extensions.Hosting;
using RabbitUtilities;
using TransportQueryService.Queries;
using TransportRequestService.Entities;

namespace TransportRequestService.TransportServiceTests
{
    public class TestPublish : BackgroundService
    {
        private readonly PublisherServiceBase transportPublisherTest;
        public TestPublish(PublisherServiceBase test) { 
            transportPublisherTest = test;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int i = 0;
            while (!stoppingToken.IsCancellationRequested) {
                //await Task.Delay(1000);

                //var transport = new Transport() { Id = i.ToString(), Origin = "Polska", Destination = "Niemcy", PricePerTicket = 3.21M, SeatsNumber = 10, SeatsTaken = 1, Type = "Plane", DepartureDate = DateTime.Now, ArrivalDate = DateTime.Now.AddDays(2) };
                var query = new TransportGetQuery
                {
                    filters = new TransportQueryService.Filters.Filter()
                    {
                        Ids = new List<string>() { "322", "432" },
                        Origins = new List<string>() { "Gdańsk", "Poznań" },
                        Destinations = new List<string>() { "Egipt", "Malta" }
                    },
                    sorting = new TransportQueryService.Filters.Sort()
                    {
                        Column = "Ids",
                        Order = "ASC"
                    }
                };
                List<Guid> tasks = new List<Guid>();
                for (int x = 0; x < 100; x++)
                {
                    tasks.Add(transportPublisherTest.PublishRequestWithReply<TransportGetQuery>("resources/transport", "query", MessageType.GET, query));
                }

                Task.WaitAll(tasks.Select(i => transportPublisherTest.GetReply(i, stoppingToken) ).ToArray());
                

                //await transportPublisherTest.GetReply(msgId, stoppingToken);
                //await transportPublisherTest.GetReply(msgId2, stoppingToken);
                //await transportPublisherTest.GetReply(msgId3, stoppingToken);



                //var body = MessagePackSerializer.Deserialize<IEnumerable<Transport>>(await transportPublisherTest.GetReply(msgId,stoppingToken));

                //Do some stuff

                i++;
            }
            return;  
        }
    }
}
