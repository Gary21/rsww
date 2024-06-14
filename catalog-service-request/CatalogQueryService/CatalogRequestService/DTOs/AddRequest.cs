using MessagePack;

namespace CatalogRequestService.DTOs
{
    [MessagePackObject]
    public class AddRequest
    {
        [Key(0)] public Order Order { get; set; }
        [Key(1)] public string HotelName { get; set; }
        [Key(2)] public string RoomType { get; set; }
        [Key(3)] public string City { get; set; }
        [Key(4)] public string Country { get; set; }
        [Key(5)] public string TransportType { get; set; }
    }
}
