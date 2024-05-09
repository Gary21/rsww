using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;

namespace HotelsQueryService.Entities
{
    [PrimaryKey("HotelId", "RoomNumber", "CheckIn")]
    [MessagePackObject]
    public class Occupancy
    {
        [System.ComponentModel.DataAnnotations.Key, Column(Order = 0)]
        [MessagePack.Key(0)]
        public int HotelId { get; set; }

        [System.ComponentModel.DataAnnotations.Key, Column(Order = 1)]
        [MessagePack.Key(1)]
        public int RoomNumber { get; set; }

        [System.ComponentModel.DataAnnotations.Key, Column(Order = 2)]
        [MessagePack.Key(2)]
        public DateTime CheckIn { get; set; }

        [Required]
        [MessagePack.Key(3)]
        public DateTime CheckOut { get; set; }

        [ForeignKey("Id")]
        [MessagePack.Key(4)]
        public Reservation Reservation { get; set; }

        [Required]
        [MessagePack.Key(5)]
        public HasRoom HasRoom { get; set; }
    }
}
