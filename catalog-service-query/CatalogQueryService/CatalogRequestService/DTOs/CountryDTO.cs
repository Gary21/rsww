using MessagePack;

namespace CatalogQueryService.DTOs
{
    [MessagePackObject]
    public class CountryDTO
    {
        [Key(0)] public int Id { get; set; }
        [Key(1)] public string Name { get; set; }
    }
}
