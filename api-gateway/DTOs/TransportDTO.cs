using MessagePack;

namespace api_gateway.DTOs
{
    [MessagePackObject]
    public class TransportDTO
    {
        [Key(0)] public int Id { get; set; }
        [Key(1)] public string OriginCity { get; set; }
        [Key(2)] public string DestinationCity { get; set; }
        [Key(3)] public string? Date { get; set; }
        [Key(4)] public string? Type { get; set; }
        [Key(5)] public string? PricePerTicket { get; set; }
        [Key(6)] public int PeopleNumber { get; set; }

    }
}
