using MessagePack;
using Microsoft.Extensions.Hosting;
using RabbitUtilities;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;
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
                await HotelChangeTests(stoppingToken);


                //await PreferencesQueries(stoppingToken);

                //await PreferencesAddEvents(stoppingToken); 
                //await ChangesAddEvents(stoppingToken,i);
                i++;
                await Task.Delay(10000);
            }
            return;  
        }
        async Task ChangesAddEvents(CancellationToken token,int i)
        {
            //RESERVATION Transport
            transportPublisherTest.PublishToFanoutNoReply("event", MessageType.UPDATE,
                new KeyValuePair<string, byte[]>("transport", MessagePackSerializer.Serialize(
                    new TransportChangeEvent() { TransportType = "Bus", DestinationCity = "Gdańsk", PriceChange = 10, SeatsChange = 10, DestinationCountry = "Polska", Id = i })));

            transportPublisherTest.PublishToFanoutNoReply("event", MessageType.UPDATE,
                new KeyValuePair<string, byte[]>("transport", MessagePackSerializer.Serialize(
                    new TransportChangeEvent() { TransportType = "Bus", DestinationCity = "Gdańsk", PriceChange = -10, SeatsChange = -5, DestinationCountry = "Polska", Id = i })));

            transportPublisherTest.PublishToFanoutNoReply("event", MessageType.UPDATE,
                new KeyValuePair<string, byte[]>("hotel", MessagePackSerializer.Serialize(
                    new HotelChangeEvent() { HotelName = "Hilon", DestinationCity = "Gdańsk", Price = -210, Availability= "False" , DestinationCountry = "Polska", Id = i , RoomType = "Suite"})));
            transportPublisherTest.PublishToFanoutNoReply("event", MessageType.UPDATE,
                new KeyValuePair<string, byte[]>("hotel", MessagePackSerializer.Serialize(
                    new HotelChangeEvent() { HotelName = "Hilon", DestinationCity = "Gdańsk", Price = -210, DestinationCountry = "Polska", Id = i, RoomType = "Suite" })));




            Console.WriteLine(MessagePackSerializer.SerializeToJson(
                MessagePackSerializer.Deserialize<IEnumerable<Changes>>(
                    await transportPublisherTest.GetReply(
                        transportPublisherTest.PublishRequestWithReply("preferences", "query", MessageType.GET, "GetLastChanges"), token))));

        }
        async Task PreferencesAddEvents(CancellationToken token)
        {
            //RESERVATION Transport
            transportPublisherTest.PublishToFanoutNoReply("event", MessageType.RESERVE, 
                new KeyValuePair<string, byte[]>("transport", MessagePackSerializer.Serialize(
                    new TransportReservationEvent(){ TransportType="Bus4", DestinationCity = "Gdańsk2",Seats = 10 })));

            //RESERVATION Hotel
            transportPublisherTest.PublishToFanoutNoReply("event", MessageType.RESERVE,
                new KeyValuePair<string, byte[]>("hotel", MessagePackSerializer.Serialize(
                    new HotelReservationEvent() { Id=10 ,HotelName ="Hilton2",  DestinationCity = "Gdańsk2", DestinationCountry = "Polska", People = 10, RoomCount = 23, RoomType = "Suite"})));

            //BUY Transport
            transportPublisherTest.PublishToFanoutNoReply("event", MessageType.ADD,
                new KeyValuePair<string, byte[]>("transport", MessagePackSerializer.Serialize(
                    new TransportReservationEvent() { TransportType = "Bus4", DestinationCity = "Gdańsk2", Seats = 10 })));

            //BUY Hotel
            transportPublisherTest.PublishToFanoutNoReply("event", MessageType.ADD,
                new KeyValuePair<string, byte[]>("hotel", MessagePackSerializer.Serialize(
                    new HotelReservationEvent() { Id = 10, HotelName = "Hilton2", DestinationCity = "Gdańsk2", DestinationCountry = "Polska", People = 10, RoomCount = 23, RoomType = "Suite" })));



            Console.WriteLine(MessagePackSerializer.SerializeToJson(MessagePackSerializer.Deserialize<IDictionary<string, IDictionary<string, Preference>>>( await transportPublisherTest.GetReply(
                transportPublisherTest.PublishRequestWithReply("preferences", "query", MessageType.GET, "GetPreferences"), token))));
        }

        async Task PreferencesQueries(CancellationToken token)
        {
            var msgId = transportPublisherTest.PublishRequestWithReply("preferences", "query", MessageType.GET, "GetLastChanges");
            var result = MessagePackSerializer.Deserialize<IEnumerable<Changes>>(await transportPublisherTest.GetReply(msgId, token));
            Console.WriteLine(MessagePackSerializer.SerializeToJson(result));

            Console.WriteLine(MessagePackSerializer.SerializeToJson(MessagePackSerializer.Deserialize<IDictionary<string, IDictionary<string, Preference>>>(
                await transportPublisherTest.GetReply(
                    transportPublisherTest.PublishRequestWithReply("preferences", "query", MessageType.GET, "GetPreferences"), token))));
        }
        async Task HotelChangeTests(CancellationToken token)
        {
            var result = MessagePackSerializer.Deserialize<bool>(await transportPublisherTest.GetReply(
                transportPublisherTest.PublishRequestWithReply("resources/hotels", "request", MessageType.UPDATE, 
                new HotelUpdateRequest { HotelId = 2, AvailabilityChange = false, PriceChange = 100, RoomTypeId = 2 }), token));
            Console.WriteLine(MessagePackSerializer.SerializeToJson(result));

            var result2 = MessagePackSerializer.Deserialize<bool>(await transportPublisherTest.GetReply(
                transportPublisherTest.PublishRequestWithReply("resources/hotels", "request", MessageType.UPDATE,
                new HotelUpdateRequest { HotelId = 2, AvailabilityChange = true, PriceChange = 0, RoomTypeId = 0 }), token));
            Console.WriteLine(MessagePackSerializer.SerializeToJson(result2));

        }
        async Task HotelTests(CancellationToken token)
        {
            //resources/hotels
            //var msgId = transportPublisherTest.PublishRequestWithReply("resources/hotels", "query", MessageType.GET, new HotelsGetQuery { filters = new HotelQueryFilters { RoomCapacities = new List<int> { 4 } } });
            //var result = MessagePackSerializer.Deserialize<IEnumerable<HotelDTO>>(await transportPublisherTest.GetReply(msgId, token));
            //Console.WriteLine(MessagePackSerializer.SerializeToJson(result));

            //var msgId2 = transportPublisherTest.PublishRequestWithReply("resources/hotels", "query", MessageType.GET, new HotelsGetQuery { filters = new HotelQueryFilters { HotelIds = new List<int>() { 2 } } });
            //var result2 = MessagePackSerializer.Deserialize<IEnumerable<HotelDTO>>(await transportPublisherTest.GetReply(msgId2, token));
            //Console.WriteLine(MessagePackSerializer.SerializeToJson(result2));
            

            var msgId3 = transportPublisherTest.PublishRequestWithReply("resources/hotels", "request", MessageType.RESERVE, new RoomReserveRequest{ HotelId = 2 /*result2.First().Id*/, RoomTypeId = 2, ReservationId = 0 , CheckInDate = DateTime.Today.Date, CheckOutDate = DateTime.Today.Date.AddDays(2)});
            var result3 = MessagePackSerializer.Deserialize<int>(await transportPublisherTest.GetReply(msgId3, token));
            Console.WriteLine(MessagePackSerializer.SerializeToJson(result3));

            var msgId4 = transportPublisherTest.PublishRequestWithReply("resources/hotels", "request", MessageType.RELEASE, new RoomReleaseRequest { HotelId = 2/*result2.First().Id*/, RoomNumber = result3, CheckInDate = DateTime.Today.Date, CheckOutDate = DateTime.Today.Date.AddDays(2) });
            var result4 = MessagePackSerializer.Deserialize<int>(await transportPublisherTest.GetReply(msgId4, token));
            Console.WriteLine(MessagePackSerializer.SerializeToJson(result4));

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

            transportPublisherTest.PublishRequestNoReply("order", "all", MessageType.ADD, new AddRequest { Order = new Order { HotelId = Random.Shared.Next(5), TransportId = Random.Shared.Next(10), OccupationId = new int[] { Random.Shared.Next(20), Random.Shared.Next(20) , Random.Shared.Next(20) }, UserId = Random.Shared.Next(3), Price = 10 } });
            transportPublisherTest.PublishRequestNoReply("order", "all", MessageType.ADD, new AddRequest { Order = new Order { HotelId = Random.Shared.Next(5), TransportId = Random.Shared.Next(10), OccupationId = new int[] { Random.Shared.Next(20), Random.Shared.Next(20) , Random.Shared.Next(20) }, UserId = Random.Shared.Next(3), Price = 10 } });


            var getmsg = transportPublisherTest.PublishRequestWithReply("order", "all", MessageType.GET, new GetRequest { Filters = new TransportQueryService.Filters.OrderFilter { UserIds =  new List<int>() { Random.Shared.Next(3) } } });
            var body = MessagePackSerializer.Deserialize<IEnumerable<Order>>(await transportPublisherTest.GetReply(getmsg, token));
            Console.WriteLine(MessagePackSerializer.SerializeToJson(body));


            getmsg = transportPublisherTest.PublishRequestWithReply("order", "all", MessageType.GET, new GetRequest { });
            body = MessagePackSerializer.Deserialize<IEnumerable<Order>>(await transportPublisherTest.GetReply(getmsg, token));
            Console.WriteLine(MessagePackSerializer.SerializeToJson(body));


            if(body.Any())  
            transportPublisherTest.PublishRequestNoReply("order", "all", MessageType.DELETE, new DeleteRequest { Id = body.OrderBy(order => order.Id).Last().Id   } );
            
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


    [MessagePackObject]
    public class Changes
    {
        [Key(0)]
        public string ResourceType { get; set; }
        [Key(1)]
        public string Id { get; set; }
        [Key(2)]
        public string Name { get; set; }
        [Key(3)]
        public bool Availability { get; set; }
        [Key(4)]
        public decimal PriceChange { get; set; }

    }
    [MessagePackObject]
    public class Preference
    {
        [Key(0)]
        public int ReservationCount { get; set; }
        [Key(1)]
        public int PurchaseCount { get; set; }
        public void Add(Preference other)
        {
            this.ReservationCount += other.ReservationCount;
            this.PurchaseCount += other.PurchaseCount;
        }
    }

    [MessagePackObject]
    public class PreferenceUpdate
    {
        [Key(0)]
        public string PreferenceType { get; set; }
        [Key(1)]
        public string PreferenceName { get; set; }
        [Key(2)]
        public Preference Preference { get; set; }
    }

    [MessagePackObject]
    public class HotelReservationEvent
    {
        [Key(0)]
        public int Id { get; set; }

        [Key(1)]
        public string HotelName { get; set; }
        [Key(2)]
        public string RoomType { get; set; }
        [Key(3)]
        public string DestinationCity { get; set; }
        [Key(4)]
        public string DestinationCountry { get; set; }
        [Key(5)]
        public int People { get; set; }
        [Key(6)]
        public int RoomCount { get; set; }
        //[Key(6)]
        //public decimal Price { get; set; }
    }

    [MessagePackObject]
    public class HotelChangeEvent
    {
        [Key(0)]
        public int Id { get; set; }

        [Key(1)]
        public string HotelName { get; set; }
        [Key(2)]
        public string RoomType { get; set; }
        [Key(3)]
        public string DestinationCity { get; set; }
        [Key(4)]
        public string DestinationCountry { get; set; }
        [Key(5)]
        public string Availability { get; set; }
        [Key(6)]
        public decimal Price { get; set; }

    }




    [MessagePackObject]
    public class TransportReservationEvent
    {
        [Key(0)]
        public string TransportType { get; set; }
        [Key(1)]
        public int Seats { get; set; }
        [Key(2)]
        public string DestinationCity { get; set; }
    }

    [MessagePackObject]
    public class TransportChangeEvent
    {
        [Key(0)]
        public int Id { get; set; }
        [Key(1)]
        public string TransportType { get; set; }
        [Key(2)]
        public string DestinationCity { get; set; }

        [Key(3)]
        public string DestinationCountry { get; set; }

        [Key(4)]
        public int SeatsChange { get; set; }

        [Key(5)]
        public decimal PriceChange { get; set; }
    }

    [MessagePackObject]
    public class HotelUpdateRequest
    {
        [Key(0)]
        public int HotelId;
        [Key(1)]
        public int RoomTypeId;

        [Key(2)]
        public bool AvailabilityChange;
        [Key(3)]
        public decimal PriceChange;

    }
}


