namespace HotelsQueryService.DTOs
{
    public class RoomCreateDTO
    {
        public int RoomNumber { get; set; }
        public string Description { get; set; }
        public int BasePrice { get; set; }
        public int RoomTypeId { get; set; }
    }

    public class RoomResponseDTO
    {
        public int Id { get; set; }
        public int RoomsCount { get; set; }
        public string Description { get; set; }
        public int BasePrice { get; set; }
        public RoomTypeResponseDTO RoomType { get; set; }
    }

    public class RoomResponseRecDTO
    {
        public int Id { get; set; }
        public int RoomsCount { get; set; }
        public string Description { get; set; }
        public int BasePrice { get; set; }
        public RoomTypeResponseDTO RoomType { get; set; }
    }
}
