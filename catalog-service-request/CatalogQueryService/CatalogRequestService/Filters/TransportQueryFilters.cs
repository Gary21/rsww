using MessagePack;

namespace CatalogRequestService.Filters
{
    [MessagePackObject]
    public class TransportQueryFilters
    {
        [Key(0)]
        public IEnumerable<string>? DepartureCityIds { get; set; } = null;
        [Key(1)]
        public string DestinationCity { get; set; }
        [Key(2)]
        public IEnumerable<string>? TransportTypes { get; set; } = null;
        [Key(3)]
        public int NumberOfPassengers { get; set; } = 0;

        public TransportQueryFilters() { }
        

    }
}
