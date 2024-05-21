using MessagePack;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TransportRequestService.Entities
{
    [MessagePackObject]
    [Table("Transports")]
    public class Transport
    {
        [MessagePack.Key(0)]
        public int Id { get; set; }

        [MessagePack.Key(1)]
        public string Type { get; set; }

        [MessagePack.Key(2)]
        public DateTime DepartureDate { get; set; }

        [MessagePack.Key(3)]
        public TimeSpan DepartureTime { get; set; }

        [MessagePack.Key(4)]
        public DateTime ArrivalDate { get; set; }

        [MessagePack.Key(5)]
        public TimeSpan ArrivalTime { get; set; }

        [MessagePack.Key(6)]
        public string DestinationCity { get; set; }

        [MessagePack.Key(7)]
        public string DestinationCountry { get; set; }

        [MessagePack.Key(8)]
        public string OriginCity { get; set; }

        [MessagePack.Key(9)]
        public string OriginCountry { get; set; }

        [MessagePack.Key(10)]
        public int SeatsNumber { get; set; }

        [MessagePack.Key(11)]
        public int SeatsTaken { get; set; }

        [MessagePack.Key(12)]
        public decimal PricePerTicket { get; set; }
    }

    public enum TransportType
    {
        Plane, Bus, Ship
    }

    [Table("TransportsEvents")]
    public class TransportEvent
    {
        [MessagePack.Key(0)]
        public int TransportId { get; set; }

        [MessagePack.Key(1)]
        public int SequenceNumber { get; set; }

        [MessagePack.Key(2)]
        public int SeatsChange { get; set; }

        [MessagePack.Key(3)]
        public decimal PriceChange { get; set; }
    }
}
