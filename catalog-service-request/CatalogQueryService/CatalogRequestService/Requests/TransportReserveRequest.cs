using MessagePack;
using CatalogQueryService.Filters;

namespace CatalogQueryService.Queries
{
    [MessagePackObject]
    public class TransportReserveRequest
    {
        [Key(0)]
        public int transportId { get; set; }
        [Key(1)]
        public int numberOfPassengers { get; set; }
    }
}
