using MessagePack;

namespace CatalogRequestService.DTOs
{
    [MessagePackObject]
    public class TripDTO
    {
        [Key(0)]
        public int HotelId { get; set; }
        [Key(1)]
        public List<int> TransportThereIds { get; set; }
        [Key(2)]
        public List<int> TransportBackIds { get; set; }
        [Key(3)]
        public decimal Price { get; set; }

        public TripDTO(int hotelId) 
        { 
            HotelId = hotelId; 
            TransportThereIds = new List<int>();
            TransportBackIds = new List<int>();
        }
    }
}
