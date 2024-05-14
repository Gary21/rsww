using MessagePack;
using OrderService.Entities;

namespace OrderService.Requests
{
    [MessagePackObject]
    public class AddRequest
    {
        [Key(0)]
        public Order Order { get; set; }
    }
}
