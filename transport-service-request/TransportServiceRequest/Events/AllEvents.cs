using MessagePack;

namespace TransportRequestService.Events
{
    [MessagePackObject]
    public class HotelReservationEvent
    {
        [Key(0)]
        public string HotelName { get; set; }
        [Key(1)]
        public string roomType { get; set; }
        [Key(2)]
        public string destinationCity { get; set; }
        [Key(3)]
        public string destinationCountry { get; set; }
        [Key(4)]
        public int people { get; set; }
        [Key(5)]
        public decimal price { get; set; }
    }

    [MessagePackObject]
    public class HotelChangeEvent
    {
        [Key(0)]
        public string HotelName { get; set; }
        [Key(1)]
        public string roomType { get; set; }
        [Key(2)]
        public string destinationCity { get; set; }
        [Key(3)]
        public string destinationCountry { get; set; }
        [Key(4)]
        public int people { get; set; }
        [Key(5)]
        public decimal price { get; set; }
    }




    [MessagePackObject]
    public class TransportReservationEvent
    {
        [Key(0)]
        public string transportType { get; set; }
        [Key(1)]
        public int seats { get; set; }
        [Key(2)]
        public string destinationCity { get; set; }

        //[Key(3)]
        //string destinationCountry;
        //[Key(4)]
        //decimal price;
    }
    [MessagePackObject]
    public class TransportChangeEvent
    {
        [Key(0)]
        public int id { get; set; }
        [Key(1)]
        public string transportType { get; set; }
        [Key(2)]
        public string destinationCity { get; set; }

        [Key(3)]
        public string destinationCountry { get; set; }

        [Key(4)]
        public int seatsChange { get; set; }

        [Key(5)]
        public decimal priceChange { get; set; }
    }


}
