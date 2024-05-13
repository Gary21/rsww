using HotelsQueryService.Entities;
using static HotelsQueryService.DTOs.HasRoomDTOs;

namespace HotelsQueryService.DTOs
{
    public class HotelCreateDTO
    {
        public required string Name { get; set; }
        public required int CityId { get; set; }
        public required string Address { get; set; }
        public required string Description { get; set; }
        public required int Rating { get; set; }
    }

    public class HotelResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        public int? CityId { get; set; }
        public string? CityName { get; set; }
    }

    public class HotelResponseRecDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        public ICollection<HasRoomResponseRecDTO> HasRoom { get; set; } = new List<HasRoomResponseRecDTO>();
    }

    public class HotelDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        public int? CityId { get; set; }
        public string? CityName { get; set; }
    }

    public class HotelDetailsWithRoomsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        public int? CityId { get; set; }
        public string? CityName { get; set; }
        public ICollection<HasRoomResponseDTO> HasRooms { get; set; } = new List<HasRoomResponseDTO>();
    }
}
