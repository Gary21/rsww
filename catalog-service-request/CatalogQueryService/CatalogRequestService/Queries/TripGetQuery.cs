using CatalogRequestService.Filters;
using MessagePack;

namespace CatalogRequestService.Queries
{
    [MessagePackObject]
    public class TripGetQuery
    {
        [Key(0)]
        public TripQueryFilters? filters { get; set; } = null;
        [Key(1)]
        public Sort? sorting { get; set; } = null;
        
    }
}
