using MessagePack;

namespace TransportRequestService.Requests
{
    [MessagePackObject]
    public class ReserveRequest
    {
        [Key(0)]
        public int Id;
        [Key(1)]
        public int Seats;
    }
}
