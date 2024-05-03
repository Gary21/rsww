using MessagePack;

namespace TransportQueryService.Filters
{
    [MessagePackObject]
    public class Sort
    {
        [Key(0)]
        public string Column { get; set; } = null;
        [Key(1)]
        public string Order { get; set; } = null; //= "ascending";
    }

    public enum SortOrder
    {
        Ascending,
        Descending
    }

}