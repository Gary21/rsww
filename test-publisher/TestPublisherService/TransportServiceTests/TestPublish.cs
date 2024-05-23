using MessagePack;
using Microsoft.Extensions.Hosting;
using RabbitUtilities;
using System.Resources;
using System.Runtime.InteropServices;
using TestPublisherService.Entities;
using TestPublisherService.HotelQueries;
using TestPublisherService.Requests;
using TestPublisherService.SecondPublisher;
using TransportQueryService.Filters;
using TransportQueryService.Queries;
using TransportRequestService.Entities;

namespace TransportRequestService.TransportServiceTests
{
    public class TestPublish : BackgroundService
    {
        private readonly TransportPublisherService transportPublisherTest;
        private readonly Publisher2Service payment;
        public TestPublish(TransportPublisherService test,Publisher2Service publisher2) { 
            transportPublisherTest = test;
            payment = publisher2;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int i = 0;
            while (!stoppingToken.IsCancellationRequested) {

                //await OrderTestsAsync(stoppingToken);
                //await TransportTestsAsync(stoppingToken);
                //await PaymentTest(stoppingToken);
                await HotelTests(stoppingToken);

                i++;
                await Task.Delay(1000);
            }
            return;  
        }


        async Task HotelTests(CancellationToken token)
        {
            //resources/hotels
            //var msgId = transportPublisherTest.PublishRequestWithReply("resources/hotels", "query", MessageType.GET, new HotelsGetQuery { filters = new HotelQueryFilters { RoomCapacities = new List<int> { 4 } } });
            //var result = MessagePackSerializer.Deserialize<IEnumerable<HotelDTO>>(await transportPublisherTest.GetReply(msgId, token));
            //Console.WriteLine(MessagePackSerializer.SerializeToJson(result));

            var msgId2 = transportPublisherTest.PublishRequestWithReply("resources/hotels", "query", MessageType.GET, new HotelsGetQuery { filters = new HotelQueryFilters { HotelIds = new List<int>() { 1 } } });
            var result2 = MessagePackSerializer.Deserialize<IEnumerable<HotelDTO>>(await transportPublisherTest.GetReply(msgId2, token));
            Console.WriteLine(MessagePackSerializer.SerializeToJson(result2));
            

            var msgId3 = transportPublisherTest.PublishRequestWithReply("resources/hotels", "request", MessageType.RESERVE, new RoomReserveRequest{ HotelId = result2.First().Id, RoomTypeId = 2, ReservationId = 0 , CheckInDate = DateTime.Today.Date, CheckOutDate = DateTime.Today.Date.AddDays(2)});
            var result3 = MessagePackSerializer.Deserialize<int>(await transportPublisherTest.GetReply(msgId3, token));
            Console.WriteLine(MessagePackSerializer.SerializeToJson(result3));

        }

        async Task PaymentTest(CancellationToken token)
        {
            //var query = new TransportGetQuery { filters = null};
            var msgId = payment.PublishRequestWithReply<int>("transactions-exchange", "incoming.all", MessageType.GET, 23);
            var result = MessagePackSerializer.Deserialize<bool>(await payment.GetReply(msgId, token));
            Console.WriteLine(MessagePackSerializer.SerializeToJson(result));

            var query2 = new TransportGetQuery
            {
                filters = new TransportQueryService.Filters.Filter()
                {
                    AvailableSeats = 25
                }
            };

            var msgId2 = transportPublisherTest.PublishRequestWithReply<TransportGetQuery>("resources/transport", "query", MessageType.GET, query2);
            var result2 = MessagePackSerializer.Deserialize<IEnumerable<Transport>>(await transportPublisherTest.GetReply(msgId2, token));

            Console.WriteLine(MessagePackSerializer.SerializeToJson(result2));
        }


        async Task TransportTestsAsync(CancellationToken token)
        {
            //await Task.Delay(1000);
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

            var msgId = transportPublisherTest.PublishRequestWithReply<TransportGetQuery>("resources/transport", "query", MessageType.GET, query);
            var result = MessagePackSerializer.Deserialize<IEnumerable<Transport>>(await transportPublisherTest.GetReply(msgId, token));
            
            Console.WriteLine(MessagePackSerializer.SerializeToJson(result));

            var query2 = new TransportGetQuery
            {
                filters = new TransportQueryService.Filters.Filter()
                {
                    AvailableSeats = 25
                }
            };

            var msgId2 = transportPublisherTest.PublishRequestWithReply<TransportGetQuery>("resources/transport", "query", MessageType.GET, query2);
            var result2 = MessagePackSerializer.Deserialize<IEnumerable<Transport>>(await transportPublisherTest.GetReply(msgId2, token));
            foreach (var t in result2)
            {
                Console.WriteLine(t.Id);
            }
            Console.WriteLine(result2.Count());

            //     var query3 = new ReserveRequest() { Id = Random.Shared.Next(100000), Seats = 1 };
            //     var msgId3 = transportPublisherTest.PublishRequestWithReply("resources/transport", "request", MessageType.RESERVE, query3);
            //     bool result = MessagePackSerializer.Deserialize<bool>(await transportPublisherTest.GetReply(msgId3, token));

        }
        async Task EventTestPublishing()
        {
            //transportPublisherTest.PublishToFanoutNoReply<Transport>("event", MessageType.GET, transport);

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

