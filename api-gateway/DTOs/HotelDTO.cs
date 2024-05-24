using MessagePack;

namespace api_gateway.DTOs;

public class HotelDTO
{
    [Key(0)]
    public int Id { get; set; }
    [Key(1)]
    public string Name { get; set; }
    [Key(2)]
    public string[] ImgUrls { get; set; }
    [Key(3)]
    public string Address { get; set; }
    [Key(4)]
    public string Description { get; set; }
    [Key(5)]
    public int Rating { get; set; }
    [Key(6)]
    public string? CityName { get; set; }
    [Key(7)]
    public string? CountryName { get; set; }
    [Key(8)]
    public int Stars { get; set; }
    [Key(9)]
    public bool HasFood { get; set; }
}