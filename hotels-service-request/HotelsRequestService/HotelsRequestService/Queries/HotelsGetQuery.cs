using MessagePack;
using HotelsRequestService.Filters;

namespace HotelsRequestService.Queries
{
    [MessagePackObject]
    public class HotelsGetQuery
    {
        [Key(0)]
        public Filter? filters { get; set; } = null;
        [Key(1)]
        public Sort? sorting { get; set; } = null;
    }
}
