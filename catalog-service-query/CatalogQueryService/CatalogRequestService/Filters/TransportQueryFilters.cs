using MessagePack;

namespace CatalogQueryService.Filters
{
    [MessagePackObject]
    public class TransportQueryFilters
    {
        [Key(0)]
        public IEnumerable<int>? DepartureCityIds { get; set; } = null;
        [Key(1)]
        public IEnumerable<int>? ArrivalCityIds { get; set; } = null;
        [Key(2)]
        public IEnumerable<string>? TransportTypes { get; set; } = null;
        [Key(3)]
        public int? NumberOfPassengers { get; set; } = null;

        public TransportQueryFilters() { }
        

    }
}
