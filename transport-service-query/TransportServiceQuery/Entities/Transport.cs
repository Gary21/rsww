using MessagePack;

namespace TransportQueryService.Entities
{
    [MessagePackObject]
    public class Transport
    {
        [Key(0)]
        public int Id { get; set; } = "";//long? 
        [Key(1)]
        public string Type { get; set; } = null;
        [Key(2)]
        public DateTime DepartureDate { get; set; } = DateTime.MinValue;
        [Key(3)]
        public DateTime ArrivalDate { get; set; } = DateTime.MinValue;
        [Key(4)]
        public string Destination { get; set; } = null;
        [Key(5)]
        public string Origin { get; set; } = null;
        [Key(6)]
        public int SeatsNumber { get; set; } = 0;
        [Key(7)]
        public int SeatsTaken { get; set; } = 0;
        [Key(8)]
        public decimal PricePerTicket { get; set; } = 0;

    }

    public enum TransportType
    {
        Plane,Bus,Ship
    }

    public class TransportDTO { 
    
    }
}
