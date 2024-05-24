using MessagePack;

namespace CatalogRequestService.Requests
{
    [MessagePackObject]
    public class TripReserveRequest
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
        public int PeopleNumber { get; set; }
        [Key(5)]
        public int TransportId { get; set; }
        [Key(6)]
        public int ClientId { get; set; }
        
    }
}
