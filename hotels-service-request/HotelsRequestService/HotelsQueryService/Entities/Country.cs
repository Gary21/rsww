using System.ComponentModel.DataAnnotations.Schema;

using DA = System.ComponentModel.DataAnnotations;
using MP = MessagePack;

namespace HotelsRequestService.Entities
{
    [MP.MessagePackObject]
    public class Country
    {
        [DA.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [MP.Key(0)]
        required
        public int Id { get; set; }

        [DA.Required]
        [DA.MaxLength(100)]
        [MP.Key(1)]
        required
        public string Name { get; set; }

        [MP.IgnoreMember]
        public ICollection<City> Cities { get; set; } = new List<City>();
    }
}
