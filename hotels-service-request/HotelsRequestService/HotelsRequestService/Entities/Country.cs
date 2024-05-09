using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;

namespace HotelsRequestService.Entities
{
    [MessagePackObject]
    public class Country
    {
        [System.ComponentModel.DataAnnotations.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [MessagePack.Key(0)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [MessagePack.Key(1)]
        public string Name { get; set; }

        [MessagePack.IgnoreMember]
        public ICollection<City> Cities { get; set; } = new List<City>();
    }
}
