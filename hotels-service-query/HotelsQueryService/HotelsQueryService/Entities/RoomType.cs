using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;

namespace HotelsQueryService.Entities
{
    [MessagePackObject]
    [Table("RoomTypes")]
    public class RoomType
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
        public int Capacity { get; set; }

        [IgnoreMember]
        public ICollection<HasRoom> WhereExist { get; set; } = new List<HasRoom>();
    }
}
