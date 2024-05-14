using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;

namespace HotelsQueryService.Entities
{
    [MessagePackObject]
    public class Hotel
    {
        [System.ComponentModel.DataAnnotations.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [MessagePack.Key(0)]
        public int Id { get; set; }

        [Required]
        [MessagePack.Key(1)]
        public string Name { get; set; }

        [Required]
        [MessagePack.Key(2)]
        public string Address { get; set; }

        [MessagePack.Key(3)]
        public string Description { get; set; }

        [MessagePack.Key(4)]
        public int Rating { get; set; }

        [MessagePack.Key(5)]
        public int Stars { get; set; }

        [MessagePack.Key(6)]
        public bool HasFood { get; set; }

        [Required]
        public City City { get; set; }

        public ICollection<HasRoom> HasRooms { get; set; } = new List<HasRoom>();
    }
}
