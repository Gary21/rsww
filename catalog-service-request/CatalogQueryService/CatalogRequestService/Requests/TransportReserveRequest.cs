using MessagePack;

namespace CatalogRequestService.Queries
{
    [MessagePackObject]
    public class TransportReserveRequest
    {
        [Key(0)]
        public int TransportId { get; set; }
        [Key(1)]
        public int NumberOfPassengers { get; set; }
    }
}
