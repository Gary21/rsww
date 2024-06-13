using MessagePack;

namespace CatalogQueryService.DTOs
{
    [MessagePackObject]
    public class RoomTypeDTO
    {
        [Key(0)] public int Id { get; set; }
        [Key(1)] public int Capacity { get; set; }
        [Key(2)] public string Name { get; set; }
        [Key(3)] public decimal PricePerNight { get; set; }
    }
}
