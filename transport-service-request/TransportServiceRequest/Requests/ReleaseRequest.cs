using MessagePack;

namespace TransportRequestService.Requests
{
    [MessagePackObject]
    public class ReleaseRequest
    {
        [Key(0)]
        public int Id;
        [Key(1)]
        public int Seats;
    }
}
