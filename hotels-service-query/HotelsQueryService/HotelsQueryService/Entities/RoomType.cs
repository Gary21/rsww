using System.ComponentModel.DataAnnotations.Schema;

using DA = System.ComponentModel.DataAnnotations;
using MP = MessagePack;

namespace HotelsQueryService.Entities
{
    [MP.MessagePackObject]
    [Table("RoomTypes")]
    public class RoomType
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
        public int Capacity { get; set; }

        [MP.IgnoreMember]
        public ICollection<Room> WhereExist { get; set; } = new List<Room>();
    }
}
