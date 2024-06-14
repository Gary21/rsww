using HotelsQueryService.Entities;
using MessagePack;

namespace HotelsQueryService.DTOs
{
    [MessagePackObject]
    public class RoomTypeDTO
    {
        [Key(0)] public int Id { get; set; }
        [Key(1)] public int Capacity { get; set; }
        [Key(2)] public string Name { get; set; }
        [Key(3)] public string PricePerNight { get; set; }

        public static RoomTypeDTO FromEntity(RoomType roomType)
        {
            return new RoomTypeDTO
            {
                Id = roomType.Id,
                Capacity = roomType.Capacity,
                Name = roomType.Name
            };
        }
    }

    public class RoomTypeCreateDTO
    {
        public required string Name { get; set; }
        public required int Capacity { get; set; }
    }

    public class RoomTypeResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
    }

}
