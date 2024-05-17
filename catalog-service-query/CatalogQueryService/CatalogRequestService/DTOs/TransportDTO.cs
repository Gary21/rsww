using MessagePack;

namespace CatalogRequestService.DTOs
{
    [MessagePackObject]
    public class TransportDTO
    {
        [Key(0)]
        public int Id { get; set; }
        [Key(1)]
        public string Type { get; set; }
        [Key(2)]
        public DateTime DepartureTime { get; set; }
        [Key(3)]
        public DateTime ArrivalTime { get; set; }
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
}
