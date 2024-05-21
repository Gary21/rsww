using MessagePack;
using TestPublisherService.Entities;

namespace TestPublisherService.Requests
{
    [MessagePackObject]
    public class AddRequest
    {
        [Key(0)]
        public Order Order { get; set; }
    }
}
