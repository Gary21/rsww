using System.ComponentModel.DataAnnotations.Schema;

using DA = System.ComponentModel.DataAnnotations;
using MP = MessagePack;

namespace HotelsRequestService.Entities
{
    [MP.MessagePackObject]
    public class Hotel
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
        public string Address { get; set; }

        [MP.Key(3)]
        public string? Description { get; set; }

        [MP.Key(4)]
        public decimal Rating { get; set; }

        [MP.Key(5)]
        public int Stars { get; set; }

        [MP.Key(6)]
        public bool HasFood { get; set; }

        [MP.IgnoreMember]
        [DA.Required]
        required
        public City City { get; set; }

        [MP.Key(7)]
        public string ImgPaths { get; set; }

        [MP.IgnoreMember]
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
