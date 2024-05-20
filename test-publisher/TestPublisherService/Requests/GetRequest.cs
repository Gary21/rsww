using MessagePack;
using TransportQueryService.Filters;

namespace TestPublisherService.Requests
{
    [MessagePackObject]
    public class GetRequest
    {
        [Key(0)]
        public OrderFilter Filters { get; set; } = new OrderFilter();

        [Key(1)]
        public Sort Sort { get; set; } = null;
    }
}
