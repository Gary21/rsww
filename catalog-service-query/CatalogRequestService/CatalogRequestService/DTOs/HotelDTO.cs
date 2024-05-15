using MessagePack;

namespace CatalogRequestService.DTOs
{
    [MessagePackObject]
    public class HotelDTO
    {
        [Key(0)]
        public int Id;
        [Key(1)]
        public string Name;
        [Key(2)]
        public string Address;
        [Key(3)]
        public string City;
        [Key(4)]
        public string Country;
        [Key(5)]
        public string Description;
        [Key(6)]
        public int Rating;
        [Key(7)]
        public int Stars;
        [Key(8)]
        public bool HasFood;
    }
}