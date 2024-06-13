using MessagePack;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelsRequestService.Entities
{

    [Table("HotelEvents")]
    [PrimaryKey(nameof(HotelId), nameof(SequenceNumber))]
    public class HotelEvent
    {
        [Key(0)]
        public int HotelId { get; set; }
        [MessagePack.Key(1)]
        public int SequenceNumber { get; set; }

        [MessagePack.Key(2)]
        public bool AvailabilityChange { get; set; }


        [MessagePack.Key(3)]
        public int RoomTypeId { get; set; }
        [MessagePack.Key(4)]
        public decimal PriceChange { get; set; }
    }
}
