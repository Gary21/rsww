using MessagePack;

namespace OrderService.Filters
{
    [MessagePackObject]
    public class Filter
    {
        [Key(0)]
        public IEnumerable<int>? Ids { get; set; } = null;//long?
        [Key(1)]
        public IEnumerable<int>? UserIds{ get; set; } = null;        

    }
}
