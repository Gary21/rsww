using MessagePack;
using TransportQueryService.Filters;

namespace TransportQueryService.Queries
{
    [MessagePackObject]
    public class TransportGetQuery
    {
        [Key(0)]
        public Filter? filters { get; set; } = null;
        [Key(1)]
        public Sort? sorting { get; set; } = null;
    }
}
