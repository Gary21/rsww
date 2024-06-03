using MessagePack;

namespace CatalogQueryService.DTOs
{
    [MessagePackObject]
    public class RoomTypeDTO
    {
        [Key(0)]
        public int Id { get; set; }
        [Key(1)]
        public string Name { get; set; }
        [Key(2)]
        public int Capacity { get; set; }
    }
}
