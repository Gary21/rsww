using Microsoft.EntityFrameworkCore;
using MessagePack;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransportQueryService.Entities
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
        public DateTime DepartureDate { get; set; }

        [Required]
        [MessagePack.Key(3)]
        public DateTime ArrivalDate { get; set; }

        [Required]
        [MessagePack.Key(4)]
        public string DestinationCity { get; set; }

        [Required]
        [MessagePack.Key(5)]
        public string DestinationCountry { get; set; }

        [Required]
        [MessagePack.Key(6)]
        public string OriginCity { get; set; }

        [Required]
        [MessagePack.Key(7)]
        public string OriginCountry { get; set; }

        [Required]
        [MessagePack.Key(8)]
        public int SeatsNumber { get; set; }

        [Required]
        [MessagePack.Key(9)]
        public int SeatsTaken { get; set; }

        [Required]
        [MessagePack.Key(10)]
        public decimal PricePerTicket { get; set; }


    }

    public enum TransportType
    {
        Plane, Bus, Ship
    }

    [Table("TransportsEvents")]
    [PrimaryKey(nameof(TransportId), nameof(SequenceNumber))]
    public class TransportEvent
    {
        //[System.ComponentModel.DataAnnotations.Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //[MessagePack.Key(0)]
        //public int Id { get; set; }

        //[MessagePack.Key(0)]
        //public Transport Transport { get; set; }

        [MessagePack.Key(0)]
        public int TransportId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [MessagePack.Key(1)]
        public int SequenceNumber { get; set; }

        [MessagePack.Key(2)]
        public int SeatsChange { get; set; }

        [MessagePack.Key(3)]
        public decimal PriceChange { get; set; }

    }

}
