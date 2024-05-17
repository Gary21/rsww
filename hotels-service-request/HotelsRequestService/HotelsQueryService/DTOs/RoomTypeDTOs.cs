using MP = MessagePack;

namespace HotelsQueryService.DTOs
{
    public class RoomTypeDTO
    {
        [MP.Key(0)]
        public int Id { get; set; }
        [MP.Key(1)]
        public string Name { get; set; }
        [MP.Key(2)]
        public int Capacity { get; set; }
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
