using MessagePack;
using CatalogRequestService.Filters;

namespace CatalogRequestService.Queries
{
    [MessagePackObject]
    public class HotelsGetQuery
    {
        [Key(0)]
        public HotelQueryFilters? filters { get; set; } = null;
        [Key(1)]
        public Sort? sorting { get; set; } = null;
    }
}
