using MessagePack;

namespace OrderService.Requests
{
    [MessagePackObject]
    public class GetRequest
    {
        [Key(0)]
        public Filters.Filter Filters { get; set; }

        [Key(1)]
        public Filters.Sort Sort { get; set; }
    }
}
