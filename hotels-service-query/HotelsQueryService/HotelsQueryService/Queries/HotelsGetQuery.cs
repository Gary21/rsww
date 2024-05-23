using MessagePack;
using HotelsQueryService.Filters;

namespace HotelsQueryService.Queries
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
}
