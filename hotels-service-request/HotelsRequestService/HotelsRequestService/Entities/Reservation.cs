using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;

namespace HotelsRequestService.Entities
{
    public class Reservation
    {
        [System.ComponentModel.DataAnnotations.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [MessagePack.Key(0)]
        public int Id { get; set; }

        [MessagePack.Key(1)]
        public int UserId { get; set; }

        [Required]
        [MessagePack.Key(2)]
        public int BabyCount { get; set; } // less than 3

        [Required]
        [MessagePack.Key(3)]
        public int ChildCount { get; set; } // 3-10

        [Required]
        [MessagePack.Key(4)]
        public int TeenCount { get; set; } // 11-17

        [Required]
        [MessagePack.Key(5)]
        public int AdultCount { get; set; } // adults

        [Required]
        [MessagePack.Key(6)]
        public bool IsPaid { get; set; }

        [Required]
        [MessagePack.Key(7)]
        public DateTime ReservationTime { get; set; }

        [Required]
        [MessagePack.Key(7)]
        public bool WithFood { get; set; }

        [Required]
        [MessagePack.Key(8)]
        public int Discount { get; set; }

        [Required]
        [MessagePack.Key(9)]
        public Occupancy Occupancy { get; set; }
    }
}
