namespace TransportRequestService.Filters
{
    public record Sort(string column, string order);
    
    public enum SortOrder
    {
        Ascending,
        Descending
    }

}
