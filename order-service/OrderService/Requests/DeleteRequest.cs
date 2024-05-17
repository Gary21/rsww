using MessagePack;

namespace OrderService.Requests
{
    [MessagePackObject]
    public class DeleteRequest
    {
        [Key(0)]
        public int Id { get; set; }
    }
}
