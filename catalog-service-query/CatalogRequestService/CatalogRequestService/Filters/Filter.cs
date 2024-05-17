using MessagePack;

namespace CatalogQueryService.Filters
{
    [MessagePackObject]
    public class Filter
    {
        [Key(0)]
        public IEnumerable<int>? HotelIds { get; set; } = null;
        [Key(1)]
        public IEnumerable<int>? CityIds { get; set; } = null;
        [Key(2)]
        public IEnumerable<int>? CountryIds { get; set; } = null;

        [Key(3)]
        public IEnumerable<int>? RoomTypeIds { get; set; } = null;
        [Key(4)]
        public IEnumerable<int>? RoomCapacities { get; set; } = null;
        [Key(5)]
        public int? MinPrice { get; set; } = null;
        [Key(6)]
        public int? MaxPrice { get; set; } = null;
        public Filter() { }
        

    }
}
