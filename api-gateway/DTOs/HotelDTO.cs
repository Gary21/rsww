namespace api_gateway.DTOs;

public class HotelDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Description { get; set; }
    public int Rating { get; set; }
    public string? CityName { get; set; }
}