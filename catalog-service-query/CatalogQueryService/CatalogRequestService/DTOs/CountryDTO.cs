using MP = MessagePack;

namespace CatalogQueryService.DTOs
{
    [MP.MessagePackObject]
    public class CountryDTO
    {
        [MP.Key(0)]
        public int Id { get; set; }
        [MP.Key(1)]
        public string Name { get; set; }
    }
}
