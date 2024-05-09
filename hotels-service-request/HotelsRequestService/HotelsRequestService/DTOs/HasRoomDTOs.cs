using HotelsRequestService.Entities;

namespace HotelsRequestService.DTOs
{
    public class HasRoomDTOs
    {
        public class HasRoomCreateDTO
        {
            public int RoomNumber { get; set; }
            public string Description { get; set; }
            public int BasePrice { get; set; }
            public int RoomTypeId { get; set; }
        }

        public class HasRoomResponseDTO
        {
            public int Id { get; set; }
            public int RoomsCount { get; set; }
            public string Description { get; set; }
            public int BasePrice { get; set; }
            public RoomTypeResponseDTO RoomType { get; set; }
        }

        public class HasRoomResponseRecDTO
        {
            public int Id { get; set; }
            public int RoomsCount { get; set; }
            public string Description { get; set; }
            public int BasePrice { get; set; }
            public RoomTypeResponseDTO RoomType { get; set; }
        }
    }
}
