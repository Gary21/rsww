using MessagePack;

namespace api_gateway.DTOs
{
    [MessagePackObject]
    public class HotelDTO
    {
        [Key(0)] public int Id { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public string Location { get; set; }
        [Key(3)] public string Rating { get; set; }
        [Key(4)] public string Stars { get; set; }
        [Key(5)] public string ImgPaths { get; set; }
    }
}