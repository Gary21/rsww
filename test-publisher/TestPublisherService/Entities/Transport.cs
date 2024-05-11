using MessagePack;

namespace TransportRequestService.Entities
{
    [MessagePackObject]
    public class Transport
    {
        [Key(0)]
        public int Id { get; set; } //long?
        [Key(1)]
        public string Type { get; set; } = null;
        [Key(2)]
        public DateTime DepartureDate { get; set; } = DateTime.MinValue;
        [Key(3)]
        public DateTime ArrivalDate { get; set; } = DateTime.MinValue;
        [Key(4)]
        public string DestinationCity { get; set; }
        [Key(5)]
        public string DestinationCountry { get; set; }
        [Key(6)]
        public string OriginCity { get; set; }
        [Key(7)]
        public string OriginCountry { get; set; }
        [Key(8)]
        public int SeatsNumber { get; set; }
        [Key(9)]
        public int SeatsTaken { get; set; }
        [Key(10)]
        public decimal PricePerTicket { get; set; }

    }

    public enum TransportType
    {
        Plane,Bus,Ship
    }

    public class TransportDTO { 
    
    }
}
