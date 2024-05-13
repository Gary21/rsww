using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;

namespace HotelsRequestService.Entities
{
    [MessagePackObject]
    public class City
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
        public Country Country { get; set; }

        [IgnoreMember]
        public ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
    }
}
