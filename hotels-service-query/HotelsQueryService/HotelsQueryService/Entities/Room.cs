using System.ComponentModel.DataAnnotations.Schema;

using EF = Microsoft.EntityFrameworkCore;
using DA = System.ComponentModel.DataAnnotations;
using MP = MessagePack;

namespace HotelsQueryService.Entities
{
    [EF.PrimaryKey("HotelId", "RoomNumber")]
    [MP.MessagePackObject]
    public class Room
    {
        [DA.Key, Column(Order = 0)][MP.Key(0)] required public int HotelId { get; set; }

        [DA.Key, Column(Order = 1)][MP.Key(1)] required public int RoomNumber { get; set; }

        [MP.Key(2)] public string? Description { get; set; }

        [MP.Key(3)] public decimal BasePrice { get; set; }

        [DA.Required][MP.Key(4)] required public Hotel Hotel { get; set; }

        [DA.Required][MP.Key(5)] required public RoomType RoomType { get; set; }
        [MP.IgnoreMember] public ICollection<Occupancy> Occupancies { get; set; } = new List<Occupancy>();
    }
}
