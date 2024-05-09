using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;

namespace HotelsRequestService.Entities
{
    [MessagePackObject]
    public class Hotel
    {
        [MessagePack.Key(0)]
        [System.ComponentModel.DataAnnotations.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MessagePack.Key(1)]
        [Required]
        public string Name { get; set; }

        [MessagePack.Key(2)]
        [Required]
        public string Address { get; set; }

        [MessagePack.Key(3)]
        public string Description { get; set; }

        [MessagePack.Key(4)]
        public int Rating { get; set; }

        [MessagePack.Key(5)]
        public int Stars { get; set; }

        [MessagePack.Key(6)]
        public bool HasFood { get; set; }


        [MessagePack.Key(7)]
        [Required]
        public City City { get; set; }

        [MessagePack.IgnoreMember]
        public ICollection<HasRoom> HasRooms { get; set; } = new List<HasRoom>();
    }
}
