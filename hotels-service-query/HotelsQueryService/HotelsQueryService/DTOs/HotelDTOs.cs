using MP = MessagePack;

namespace HotelsQueryService.DTOs
{
    [MP.MessagePackObject]
    public class HotelDTO
    {
        [MP.Key(0)]
        public int Id { get; set; }
        [MP.Key(1)]
        public string Name { get; set; }
        [MP.Key(2)]
        public string Address { get; set; }
        [MP.Key(3)]
        public string Description { get; set; }
        [MP.Key(4)]
        public decimal Rating { get; set; }
        [MP.Key(5)]
        public int Stars { get; set; }
        [MP.Key(6)]
        public bool HasFood { get; set; }
        [MP.Key(7)]
        public int? CityId { get; set; }
        [MP.Key(8)]
        public string? CityName { get; set; }
        [MP.Key(9)]
        public int? CountryId { get; set; }
        [MP.Key(10)]
        public string? CountryName { get; set; }
        [MP.Key(11)]
        public string ImgPaths { get; set; }
    }

    public class HotelCreateDTO
    {
        public required string Name { get; set; }
        public required int CityId { get; set; }
        public required string Address { get; set; }
        public required string Description { get; set; }
        public required decimal Rating { get; set; }
    }

    public class HotelResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public decimal Rating { get; set; }
        public int? CityId { get; set; }
        public string? CityName { get; set; }
    }

    public class HotelResponseRecDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public decimal Rating { get; set; }
        public ICollection<RoomResponseRecDTO> HasRoom { get; set; } = new List<RoomResponseRecDTO>();
    }

    public class HotelDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public decimal Rating { get; set; }
        public int? CityId { get; set; }
        public string? CityName { get; set; }
    }

    public class HotelDetailsWithRoomsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public decimal Rating { get; set; }
        public int? CityId { get; set; }
        public string? CityName { get; set; }
        public ICollection<RoomResponseDTO> HasRooms { get; set; } = new List<RoomResponseDTO>();
    }
}
