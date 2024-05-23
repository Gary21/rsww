using MP = MessagePack;

namespace CatalogQueryService.DTOs
{
    [MP.MessagePackObject]
    public class HotelDTO
    {
        [MP.Key(0)]
        public int Id { get; set; }
        [MP.Key(1)]
        public string Name { get; set; }
        [MP.Key(2)]
        public string Address { get; set; }
        [MP.Key(3)]
        public string Description { get; set; }
        [MP.Key(4)]
        public decimal Rating { get; set; }
        [MP.Key(5)]
        public int Stars { get; set; }
        [MP.Key(6)]
        public bool HasFood { get; set; }
        [MP.Key(7)]
        public int? CityId { get; set; }
        [MP.Key(8)]
        public string? CityName { get; set; }
        [MP.Key(9)]
        public int? CountryId { get; set; }
        [MP.Key(10)]
        public string? CountryName { get; set; }
        [MP.Key(11)]
        public string ImgPaths { get; set; }
    }
}