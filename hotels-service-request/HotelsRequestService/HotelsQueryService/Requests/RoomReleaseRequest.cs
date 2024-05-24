using MessagePack;

namespace HotelsRequestService.Requests
{
    [MessagePackObject]
    public class RoomReleaseRequest
    {
        [Key(0)]
        public int HotelId { get; set; }
        [Key(1)]
        public int RoomNumber { get; set; }
        [Key(2)]
        public DateTime CheckInDate { get; set; }
        [Key(3)]
        public DateTime CheckOutDate { get; set; }
    
    }
}
