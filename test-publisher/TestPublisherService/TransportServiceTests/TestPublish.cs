using MessagePack;
using Microsoft.Extensions.Hosting;
using RabbitUtilities;
using TestPublisherService.Entities;
using TestPublisherService.Requests;
using TransportQueryService.Filters;
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

                await OrderTestsAsync(stoppingToken);



                i++;
                await Task.Delay(1000);
            }
            return;  
        }

        async Task TransportTestsAsync(CancellationToken token)
        {
            //await Task.Delay(1000);

            // var transport = new Transport() { Id = i.ToString(), Origin = "Polska", Destination = "Niemcy", PricePerTicket = 3.21M, SeatsNumber = 10, SeatsTaken = 1, Type = "Plane", DepartureDate = DateTime.Now, ArrivalDate = DateTime.Now.AddDays(2) };
            var query = new TransportGetQuery
            {
                filters = new TransportQueryService.Filters.Filter()
                {
                    Ids = new List<int>() { Random.Shared.Next(10000), Random.Shared.Next(10000) },
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


            //     var query2 = new ReserveRequest() { Id = Random.Shared.Next(100000), Seats = 1 };

            //     var msgId = transportPublisherTest.PublishRequestWithReply("resources/transport", "request", MessageType.RESERVE, query2);
            //     bool result = MessagePackSerializer.Deserialize<bool>(await transportPublisherTest.GetReply(msgId, stoppingToken));

            //     //var msgId2 = transportPublisherTest.PublishRequestWithReply<TransportGetQuery>("resources/transport", "query", MessageType.GET, query);
            //     //var result2 = MessagePackSerializer.Deserialize<IEnumerable<Transport>>(await transportPublisherTest.GetReply(msgId2, stoppingToken));
            //     //foreach (var result4 in result2)
            //     //{
            //    //     Console.WriteLine(result4.Id);
            //     //}

            //     //await transportPublisherTest.GetReply(msgId3, stoppingToken);

            //     var query3 = new TransportGetQuery
            //     {
            //         filters = new TransportQueryService.Filters.Filter()
            //         {
            //             AvailableSeats = 25
            //         }
            //     };

            //     //var msgId3 = transportPublisherTest.PublishRequestWithReply<TransportGetQuery>("resources/transport", "query", MessageType.GET, query3);

            ////     var result3 = MessagePackSerializer.Deserialize<IEnumerable<Transport>>(await transportPublisherTest.GetReply(msgId3, stoppingToken));
            //     //var body = MessagePackSerializer.Deserialize<IEnumerable<Transport>>(await transportPublisherTest.GetReply(msgId,stoppingToken));

            //     Console.WriteLine(MessagePackSerializer.SerializeToJson(result));

            //Console.WriteLine(result2.First()?.SeatsTaken);
            //    Console.WriteLine(MessagePackSerializer.SerializeToJson(result3));


        }

        async Task OrderTestsAsync(CancellationToken token)
        {
            var getnone = transportPublisherTest.PublishRequestWithReply("order", "all", MessageType.GET, new GetRequest { Filters = new TransportQueryService.Filters.OrderFilter { Ids = new List<int>() { Random.Shared.Next(1000) } } } );
            var body5 = MessagePackSerializer.Deserialize<IEnumerable<Order>>(await transportPublisherTest.GetReply(getnone, token));
            Console.WriteLine(MessagePackSerializer.SerializeToJson(body5));

            transportPublisherTest.PublishRequestNoReply("order", "all", MessageType.ADD, new AddRequest { Order = new Order { HotelId = Random.Shared.Next(5), TransportId = Random.Shared.Next(10), OccupationId = Random.Shared.Next(20), UserId = Random.Shared.Next(3), Price = 10 } });
            transportPublisherTest.PublishRequestNoReply("order", "all", MessageType.ADD, new AddRequest { Order = new Order { HotelId = Random.Shared.Next(5), TransportId = Random.Shared.Next(10), OccupationId = Random.Shared.Next(20), UserId = Random.Shared.Next(3), Price = 10 } });


            var getmsg = transportPublisherTest.PublishRequestWithReply("order", "all", MessageType.GET, new GetRequest { Filters = new TransportQueryService.Filters.OrderFilter { UserIds =  new List<int>() { Random.Shared.Next(3) } } });
            var body = MessagePackSerializer.Deserialize<IEnumerable<Order>>(await transportPublisherTest.GetReply(getmsg, token));
            Console.WriteLine(MessagePackSerializer.SerializeToJson(body));


            getmsg = transportPublisherTest.PublishRequestWithReply("order", "all", MessageType.GET, new GetRequest { });
            body = MessagePackSerializer.Deserialize<IEnumerable<Order>>(await transportPublisherTest.GetReply(getmsg, token));
            Console.WriteLine(MessagePackSerializer.SerializeToJson(body));



            transportPublisherTest.PublishRequestNoReply("order", "all", MessageType.DELETE, new DeleteRequest { Id = body.OrderBy(order => order.Id).Last().Id });
            
            var getmsg2 = transportPublisherTest.PublishRequestWithReply("order", "all", MessageType.GET, new GetRequest { Filters = new TransportQueryService.Filters.OrderFilter { Ids = new List<int>() { body.OrderBy(order => order.Id).Last().Id } } });
            var body2 = MessagePackSerializer.Deserialize<IEnumerable<Order>>(await transportPublisherTest.GetReply(getmsg2, token));
            Console.WriteLine(MessagePackSerializer.SerializeToJson(body2));


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

