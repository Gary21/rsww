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

                // var transport = new Transport() { Id = i.ToString(), Origin = "Polska", Destination = "Niemcy", PricePerTicket = 3.21M, SeatsNumber = 10, SeatsTaken = 1, Type = "Plane", DepartureDate = DateTime.Now, ArrivalDate = DateTime.Now.AddDays(2) };
                var query = new TransportGetQuery
                {
                    filters = new TransportQueryService.Filters.Filter()
                    {
                        Ids = new List<int>() { 1 },
                        //Origins = new List<string>() { "Gdańsk", "Poznań" },
                        //Destinations = new List<string>() { "Egipt", "Malta" }
                    }
                    // ,
                    // sorting = new TransportQueryService.Filters.Sort()
                    // {
                    //     Column = "Ids",
                    //     Order = "ASC"
                    // }
                };
                //List<Guid> tasks = new List<Guid>();
                //for (int x = 0; x < 2; x++)
                //{
                //    tasks.Add(transportPublisherTest.PublishRequestWithReply<TransportGetQuery>("resources/transport", "query", MessageType.GET, query));

                //}
                //Task.WaitAll(tasks.Select(i => transportPublisherTest.GetReply(i, stoppingToken) ).ToArray());

                //transportPublisherTest.PublishToFanoutNoReply<Transport>("event", MessageType.GET, transport);
        

                var query2 = new ReserveRequest(){Id = 1, Seats = 3};

                var msgId = transportPublisherTest.PublishRequestWithReply("resources/transport","request",MessageType.RESERVE,query2);
                var msgId2 = transportPublisherTest.PublishRequestWithReply<TransportGetQuery>("resources/transport", "query", MessageType.GET, query);
                await Task.Delay(1000);
                bool result = MessagePackSerializer.Deserialize<bool>( await transportPublisherTest.GetReply(msgId, stoppingToken));
                
                var result2 = MessagePackSerializer.Deserialize<IEnumerable<Transport>>(await transportPublisherTest.GetReply(msgId2, stoppingToken));
                //await transportPublisherTest.GetReply(msgId3, stoppingToken);

                var query3 = new TransportGetQuery
                {
                    filters = new TransportQueryService.Filters.Filter()
                    {
                        AvailableSeats = 25
                    }
                };
            
                var msgId3 = transportPublisherTest.PublishRequestWithReply<TransportGetQuery>("resources/transport", "query", MessageType.GET, query3);

                var result3 = MessagePackSerializer.Deserialize<IEnumerable<Transport>>(await transportPublisherTest.GetReply(msgId3, stoppingToken));
                //var body = MessagePackSerializer.Deserialize<IEnumerable<Transport>>(await transportPublisherTest.GetReply(msgId,stoppingToken));

                //Do some stuff

                Console.WriteLine(result2.First()?.SeatsTaken);
                Console.WriteLine(MessagePackSerializer.SerializeToJson(result3));
                i++;
            }
            return;  
        }
    }
[MessagePackObject]
public class ReserveRequest
{
    [Key(0)]
    public int Id;
    [Key(1)]
    public int Seats;
}

}

