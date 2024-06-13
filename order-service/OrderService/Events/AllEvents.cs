using MessagePack;

namespace OrderService.Events
{
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
        public string Availability {  get; set; }
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

}
//[Key(3)]
//string destinationCountry;
