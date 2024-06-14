using MessagePack;

namespace HotelsQueryService.DTOs
{
    [MessagePackObject]
    public class HotelDTO
    {
        [Key(0)] public int Id { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public string Location { get; set; }
        [Key(3)] public string Rating { get; set; }
        [Key(4)] public string Stars { get; set; }
        [Key(5)] public string ImgPaths { get; set; }
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
