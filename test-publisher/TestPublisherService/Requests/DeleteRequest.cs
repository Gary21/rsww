using MessagePack;

namespace TestPublisherService.Requests
{
    [MessagePackObject]
    public class DeleteRequest
    {
        [Key(0)]
        public int Id { get; set; }
    }
}
