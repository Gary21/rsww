using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;

namespace HotelsQueryService.Entities
{
    [MessagePackObject]
    public class HasRoom
    {
        [System.ComponentModel.DataAnnotations.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [MessagePack.Key(0)] // Added MessagePack reference
        public int Id { get; set; }

        [Required]
        [MessagePack.Key(1)] // Added MessagePack reference
        public int RoomNumber { get; set; }

        [MessagePack.Key(2)] // Added MessagePack reference
        public string? Description { get; set; }

        [MessagePack.Key(3)] // Added MessagePack reference
        public int BasePrice { get; set; }

        [Required]
        [MessagePack.Key(4)] // Added MessagePack reference
        public Hotel Hotel { get; set; }

        [Required]
        [MessagePack.Key(5)] // Added MessagePack reference
        public RoomType RoomType { get; set; }
    }
}
