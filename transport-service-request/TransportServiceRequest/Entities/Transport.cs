using Microsoft.EntityFrameworkCore;
using MessagePack;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TransportRequestService.Entities
{
    [MessagePackObject]
    [Table("Transports")]
    public class Transport
    {
        [System.ComponentModel.DataAnnotations.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [MessagePack.Key(0)]
        public int Id { get; set; }

        [Required] 
        [MessagePack.Key(1)]
        public string Type { get; set; }

        [Required] 
        [MessagePack.Key(2)]
        [Column(TypeName = "Date")]
        public DateTime DepartureDate { get; set; }

        [Required]
        [MessagePack.Key(3)]
        [Column(TypeName = "Time")]
        public TimeSpan DepartureTime { get; set; }

        [Required]
        [MessagePack.Key(4)]
        [Column(TypeName = "Date")]
        public DateTime ArrivalDate { get; set; }

        [Required]
        [MessagePack.Key(5)]
        [Column(TypeName = "Time")]
        public TimeSpan ArrivalTime { get; set; }

        [Required]
        [MessagePack.Key(6)]
        public string DestinationCity { get; set; }

        [Required]
        [MessagePack.Key(7)]
        public string DestinationCountry { get; set; }

        [Required]
        [MessagePack.Key(8)]
        public string OriginCity { get; set; }

        [Required]
        [MessagePack.Key(9)]
        public string OriginCountry { get; set; }

        [Required]
        [MessagePack.Key(10)]
        public int SeatsNumber { get; set; }

        [Required]
        [MessagePack.Key(11)]
        public int SeatsTaken { get; set; }

        [Required]
        [MessagePack.Key(12)]
        public decimal PricePerTicket { get; set; }


    }

    public enum TransportType
    {
        Plane,Bus,Ship
    }

    [Table("TransportsEvents")]
    [PrimaryKey(nameof(TransportId), nameof(SequenceNumber))]
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