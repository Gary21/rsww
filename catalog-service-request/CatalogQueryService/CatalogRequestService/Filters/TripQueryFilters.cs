using MessagePack;

namespace CatalogRequestService.Filters
{
    [MessagePackObject]
    public class TripQueryFilters
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
        public int PeopleNumber { get; set; } = 1;
        [Key(5)]
        public DateTime? CheckInDate { get; set; } = null;
        [Key(6)]
        public DateTime? CheckOutDate { get; set; } = null;
        [Key(7)]
        public int? MinPrice { get; set; } = null;
        [Key(8)]
        public int? MaxPrice { get; set; } = null;


        [Key(9)]
        public IEnumerable<int>? DepartureCityIds { get; set; } = null;
        [Key(10)]
        public IEnumerable<string>? TransportTypes { get; set; } = null;


        public TripQueryFilters() { }
    }

    [MessagePackObject]
    public class ReservationQuery
    {
        [Key(0)]
        public int HotelId { get; set; }
        [Key(1)]
        public string DepartureCity { get; set; }
        [Key(2)]
        public string RoomType { get; set; }
        [Key(3)]
        public string Date { get; set; }
        [Key(4)]
        public int Adults { get; set; }
        [Key(5)]
        public int Children18 { get; set; }
        [Key(6)]
        public int Children10 { get; set; }
        [Key(7)]
        public int Children3 { get; set; }
    }

    public class TripQueryFiltersAdapter
    {
        public static TripQueryFilters AdaptHotelQueryToTripQuery(HotelQueryFilters hotelQueryFilters)
        {
            return new TripQueryFilters
            {
                HotelIds = hotelQueryFilters.HotelIds,
                CityIds = hotelQueryFilters.CityIds,
                CountryIds = hotelQueryFilters.CountryIds,
                RoomTypes = hotelQueryFilters.RoomTypes,
                CheckInDate = hotelQueryFilters.CheckInDate,
                CheckOutDate = hotelQueryFilters.CheckOutDate,
                MinPrice = hotelQueryFilters.MinPrice,
                MaxPrice = hotelQueryFilters.MaxPrice,
                PeopleNumber = 1
            };
        }

        public static TripQueryFilters AdaptReservationQueryToTripQuery(ReservationQuery reservationQuery)
        {
            DateTime tmp1, tmp2;
            if (!DateTime.TryParseExact(reservationQuery.Date, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out tmp1))
            {
                tmp1 = DateTime.Now.Date;
            }

            if (!DateTime.TryParseExact(reservationQuery.Date, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out tmp2))
            {
                tmp2 = DateTime.Now.Date.AddDays(7);
            }

            // string : roomTypeName (Capacity)
            var roomType = reservationQuery.RoomType.Split(" ")[0] ?? null;

            return new TripQueryFilters
            {
                HotelIds = new List<int> { reservationQuery.HotelId },
                RoomTypes = new List<string> { roomType },
                CheckInDate = tmp1,
                CheckOutDate = tmp2,
                PeopleNumber = reservationQuery.Adults + reservationQuery.Children18 + reservationQuery.Children10 + reservationQuery.Children3,
                DepartureCityIds = null,
                TransportTypes = null
            };
        }
    }
}
