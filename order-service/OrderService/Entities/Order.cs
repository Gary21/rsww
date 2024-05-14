using Microsoft.EntityFrameworkCore;
using MessagePack;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Entities
{
    [MessagePackObject]
    [Table("Orders")]
    public class Order
    {
        [System.ComponentModel.DataAnnotations.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [MessagePack.Key(0)]
        public int Id { get; set; }

        [MessagePack.Key(1)]
        public int HotelId { get; set; }

        [MessagePack.Key(2)]
        public int OccupationId { get; set; }
        
        [MessagePack.Key(3)]
        public int TransportId { get; set; }

        [MessagePack.Key(4)]
        public int UserId { get; set; }

        
        [MessagePack.Key(5)]
        public int BabyCount { get; set; } = 0;
        [MessagePack.Key(6)]
        public int ChildCount { get; set; } = 0;
        [MessagePack.Key(7)]
        public int TeenCount { get; set; } = 0;
        [MessagePack.Key(8)]
        public int AdultCount { get; set; } = 1;

        [MessagePack.Key(9)]
        public bool WithFood { get; set; } = false;
        [MessagePack.Key(10)]
        public int Discount { get; set; } = 0;

        [MessagePack.Key(11)]
        public DateTime ReservationTime { get; set; } = DateTime.UtcNow;

        [MessagePack.Key(12)]
        public decimal Price { get; set; }
    }
}
