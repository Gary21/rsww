using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportQueryService.Filters;
using MP = MessagePack;

namespace TestPublisherService.HotelQueries
{
    [MessagePackObject]
    public class HotelsGetQuery
    {
        [Key(0)]
        public HotelQueryFilters? filters { get; set; } = null;
        [Key(1)]
        public Sort? sorting { get; set; } = null;

        public HotelsGetQuery()
        {
            filters = new HotelQueryFilters();
            sorting = new Sort();
        }
    }

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



    [MP.MessagePackObject]
    public class HotelDTO
    {
        [MP.Key(0)]
        public int Id { get; set; }
        [MP.Key(1)]
        public string Name { get; set; }
        [MP.Key(2)]
        public string Address { get; set; }
        [MP.Key(3)]
        public string Description { get; set; }
        [MP.Key(4)]
        public decimal Rating { get; set; }
        [MP.Key(5)]
        public int Stars { get; set; }
        [MP.Key(6)]
        public bool HasFood { get; set; }
        [MP.Key(7)]
        public int? CityId { get; set; }
        [MP.Key(8)]
        public string? CityName { get; set; }
        [MP.Key(9)]
        public int? CountryId { get; set; }
        [MP.Key(10)]
        public string? CountryName { get; set; }
        [MP.Key(11)]
        public string ImgPaths { get; set; }
    }

}
