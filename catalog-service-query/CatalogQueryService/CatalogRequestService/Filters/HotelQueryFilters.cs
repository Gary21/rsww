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
        public string? Destination { get; set; }
        [Key(2)]
        public string? Departure { get; set; }
        [Key(3)]
        public string? DepartureDate { get; set; }
        [Key(4)]
        public int? Adults { get; set; }
        [Key(5)]
        public int? Children18 { get; set; }
        [Key(6)]
        public int? Children10 { get; set; }
        [Key(7)]
        public int? Children3 { get; set; }
    }

    public class HotelsFiltersAdapter
    {
        public static HotelQueryFilters GatewayHotelToHotel(HotelsQueryFiltersGatway hqfg)
        {
            var hotelQueryFilters = new HotelQueryFilters();

            if (hqfg.HotelId != null)
            {
                hotelQueryFilters.HotelIds = new List<int> { int.Parse(hqfg.HotelId) };
            }

            //if (hqfg.Destination != null)
            //{
            //    hotelQueryFilters.CityIds = new List<int> { int.Parse(hqfg.Destination) };
            //}

            int peopleNumber = 0;
            if (hqfg.Children3 != null) { peopleNumber += hqfg.Children3.Value; }
            if (hqfg.Children10 != null) { peopleNumber += hqfg.Children10.Value; }
            if (hqfg.Children18 != null) { peopleNumber += hqfg.Children18.Value; }
            if (hqfg.Adults != null) { peopleNumber += hqfg.Adults.Value; }
            if (peopleNumber > 0) { hotelQueryFilters.RoomCapacities = new List<int> { peopleNumber }; }

            try
            {
                if (hqfg.DepartureDate == null) { throw new Exception(); }
                DateTime checkInDate = DateTime.Parse(hqfg.DepartureDate);
                DateTime checkOutDate = checkInDate.AddDays(7);

                hotelQueryFilters.CheckInDate = checkInDate;
                hotelQueryFilters.CheckOutDate = checkOutDate;
            }
            catch (Exception) { }

            return hotelQueryFilters;
        }
    }
}
