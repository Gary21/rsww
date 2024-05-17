using MessagePack;

namespace CatalogQueryService.Queries
{
    [MessagePackObject]
    public class RoomReserveRequest
    {
        [Key(0)]
        public int hotelId { get; set; }
        [Key(1)]
        public int roomNumber { get; set; }
        [Key(2)]
        public DateTime checkIn { get; set; }
        [Key(3)]
        public DateTime checkOut { get; set; }
    }
}
