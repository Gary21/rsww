using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;

namespace HotelsQueryService.Entities
{
    [PrimaryKey("HotelId", "RoomNumber")]
    [MessagePackObject]
    public class Room
    {
        [System.ComponentModel.DataAnnotations.Key, Column(Order = 0)]
        [MessagePack.Key(0)] 
        public int HotelId { get; set; }

        [System.ComponentModel.DataAnnotations.Key, Column(Order = 1)]
        [MessagePack.Key(1)] 
        public int RoomNumber { get; set; }

        [MessagePack.Key(2)] 
        public string? Description { get; set; }

        [MessagePack.Key(3)] 
        public int BasePrice { get; set; }

        [Required]
        [MessagePack.Key(4)] 
        public Hotel Hotel { get; set; }

        [Required]
        [MessagePack.Key(5)] 
        public RoomType RoomType { get; set; }

        [MessagePack.IgnoreMember] 
        public ICollection<Occupancy> Occupancies { get; set; }
    }
}
