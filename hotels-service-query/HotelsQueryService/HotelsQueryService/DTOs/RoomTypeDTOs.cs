namespace HotelsQueryService.DTOs
{
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
