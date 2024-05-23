using MP = MessagePack;

namespace HotelsQueryService.DTOs
{
    [MP.MessagePackObject]
    public class CountryDTO
    {
        [MP.Key(0)]
        public int Id { get; set; }
        [MP.Key(1)]
        public string Name { get; set; }
    }

    public class CountryCreateDTO
    {
        public required string Name { get; set; }
    }

    public class CountryResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CountryResponseRecDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<CityResponseRecDTO> Cities { get; set; } = new List<CityResponseRecDTO>();
    }

    public class CountryDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<CityResponseDTO> Cities { get; set; } = new List<CityResponseDTO>();
    }
}
