using System.ComponentModel.DataAnnotations.Schema;

using DA = System.ComponentModel.DataAnnotations;
using MP = MessagePack;

namespace HotelsRequestService.Entities
{
    [MP.MessagePackObject]
    public class City
    {
        [DA.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [MP.Key(0)]
        required
        public int Id { get; set; }

        [DA.Required]
        [MP.Key(1)]
        required
        public string Name { get; set; }

        [DA.Required]
        [MP.Key(2)]
        required
        public Country Country { get; set; }

        [MP.IgnoreMember]
        public ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
    }
}
