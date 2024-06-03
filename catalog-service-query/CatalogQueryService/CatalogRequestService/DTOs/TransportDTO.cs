using MessagePack;

namespace CatalogQueryService.DTOs
{
    [MessagePackObject]
    public class TransportDTO
    {
        [Key(0)]
        public int Id { get; set; }
        [Key(1)]
        public string Type { get; set; }
        [Key(2)]
        public DateTime DepartureDate { get; set; }
        [Key(3)]
        public TimeSpan DepartureTime { get; set; }
        [Key(4)]
        public DateTime ArrivalDate { get; set; }
        [Key(5)]
        public TimeSpan ArrivalTime { get; set; }
        [Key(6)]
        public string DestinationCity { get; set; }
        [Key(7)]
        public string DestinationCountry { get; set; }
        [Key(8)]
        public string OriginCity { get; set; }
        [Key(9)]
        public string OriginCountry { get; set; }
        [Key(10)]
        public int SeatsNumber { get; set; }
        [Key(11)]
        public int SeatsTaken { get; set; }
        [Key(12)]
        public decimal PricePerTicket { get; set; }
    }
}
