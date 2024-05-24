﻿using MessagePack;

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
}