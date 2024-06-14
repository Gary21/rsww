using MessagePack;

namespace HotelsRequestService.Requests
{
    [MessagePackObject]
    public class HotelUpdateRequest
    {
        [Key(0)]
        public int HotelId;
        [Key(1)]
        public int RoomTypeId;

        [Key(2)]
        public bool AvailabilityChange;
        [Key(3)]
        public decimal PriceChange;
        
    }
}
