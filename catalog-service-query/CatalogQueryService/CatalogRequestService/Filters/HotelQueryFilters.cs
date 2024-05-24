using MessagePack;

namespace CatalogQueryService.Filters
{
    [MessagePackObject]
    public class HotelQueryFilters
    {
        [Key(0)]
        public IEnumerable<int>? HotelIds { get; set; } = null;
        [Key(1)]
        public IEnumerable<int>? CityIds { get; set; } = null;
        [Key(2)]
        public IEnumerable<int>? CountryIds { get; set; } = null;
        [Key(3)]
        public IEnumerable<string>? RoomTypes { get; set; } = null;
        [Key(4)]
        public IEnumerable<int>? RoomCapacities { get; set; } = null;
        [Key(5)]
        public DateTime? CheckInDate { get; set; } = null;
        [Key(6)]
        public DateTime? CheckOutDate { get; set; } = null;
        [Key(7)]
        public int? MinPrice { get; set; } = null;
        [Key(8)]
        public int? MaxPrice { get; set; } = null;
        public HotelQueryFilters() { }
    }

    [MessagePackObject]
    public class HotelsQueryFiltersGatway
    {
        [Key(0)]
        public string? HotelId { get; set; }
        [Key(1)]
        public string? Departure { get; set; }
        [Key(2)]
        public string? DepartureDate { get; set; }
        [Key(3)]
        public int? Adults { get; set; }
        [Key(4)]
        public int? Children18 { get; set; }
        [Key(5)]
        public int? Children10 { get; set; }
        [Key(6)]
        public int? Children3 { get; set; }
    }

    public class FilterAdapterHotelToHotelsGateway
    {
        public static HotelQueryFilters AdaptGatewayToMe(HotelsQueryFiltersGatway hotelsQueryFiltersGatway)
        {
            return new HotelQueryFilters
            {
                HotelIds = new List<int> { int.Parse(hotelsQueryFiltersGatway.HotelId) },
                CityIds = null,
                CountryIds = null,
                RoomTypes = null,
                RoomCapacities = new List<int> { hotelsQueryFiltersGatway.Adults.Value + hotelsQueryFiltersGatway.Children18.Value + hotelsQueryFiltersGatway.Children10.Value + hotelsQueryFiltersGatway.Children3.Value },
                CheckInDate = DateTime.Parse(hotelsQueryFiltersGatway.DepartureDate),
                CheckOutDate = DateTime.Parse(hotelsQueryFiltersGatway.DepartureDate).AddDays(7),
                MinPrice = null,
                MaxPrice = null
            };
        }
    }
}
