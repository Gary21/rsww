using MessagePack;

namespace api_gateway.Queries;
[MessagePackObject]
public class HotelsQuery
{
    [Key(0)]
    public string? HotelId { get; set; }
    [Key(1)]
    public string? Departure { get; set; }
    [Key(2)]
    public string? DepartureDate { get; set; }
    [Key(3)]
    public int? Adults { get; set; }
    [Key(4)]
    public int? Children18 { get; set; }
    [Key(5)]
    public int? Children10 { get; set; }
    [Key(6)]
    public int? Children3 { get; set; }
}