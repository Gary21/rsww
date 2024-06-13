using MessagePack;

namespace SimulationService
{
    [MessagePackObject]
    public class AddRequest
    {
        [Key(0)]
        public Order Order { get; set; }
    }
}
