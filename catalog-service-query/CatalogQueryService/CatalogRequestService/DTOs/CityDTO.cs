using MP = MessagePack;

namespace CatalogQueryService.DTOs
{
    [MP.MessagePackObject]
    public class CityDTO
    {
        [MP.Key(0)]
        public int Id { get; set; }
        [MP.Key(1)]
        public string Name { get; set; }
        [MP.Key(2)]
        public string CountryName { get; set; }
    }
}
