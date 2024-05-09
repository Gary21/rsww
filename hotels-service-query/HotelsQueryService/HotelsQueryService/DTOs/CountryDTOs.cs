namespace HotelsQueryService.DTOs
{
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
