using MessagePack;

namespace CatalogQueryService.Queries
{
    [MessagePackObject]
    public class RoomReserveRequest
    {
        [Key(0)]
        public int HotelId { get; set; }
        [Key(1)]
        public int RoomTypeId { get; set; }
        [Key(2)]
        public DateTime CheckInDate { get; set; }
        [Key(3)]
        public DateTime CheckOutDate { get; set; }
        [Key(4)]
        public int ReservationId { get; set; }
    }
}
