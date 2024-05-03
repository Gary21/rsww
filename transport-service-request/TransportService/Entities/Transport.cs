using MessagePack;

namespace TransportRequestService.Entities
{
    [MessagePackObject]
    public class Transport
    {
        [Key(0)]
        public string Id { get; set; } //long?
        [Key(1)]
        public string Type { get; set; }
        [Key(2)]
        public DateTime DepartureDate { get; set; }
        [Key(3)]
        public DateTime ArrivalDate { get; set; }
        [Key(4)]
        public string Destination { get; set; }
        [Key(5)]
        public string Origin { get; set; }
        [Key(6)]
        public int SeatsNumber { get; set; }
        [Key(7)]
        public int SeatsTaken { get; set; }
        [Key(8)]
        public decimal PricePerTicket { get; set; }

    }

    public enum TransportType
    {
        Plane,Bus,Ship
    }

    public class TransportDTO { 
    
    }
}
