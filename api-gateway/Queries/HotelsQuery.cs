namespace api_gateway.Queries;

public class HotelsQuery
{
    public string? HotelId { get; set; }
    public string? Departure { get; set; }
    public string? DepartureDate { get; set; }
    public int? Adults { get; set; }
    public int? Children18 { get; set; }
    public int? Children10 { get; set; }
    public int? Children3 { get; set; }
}