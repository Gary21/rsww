﻿namespace HotelsRequestService.DTOs
{
    public class CityCreateDTO
    {
        public required string Name { get; set; }
        public required int CountryId { get; set; }
    }

    public class CityResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CityResponseRecDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<HotelResponseRecDTO> Hotels { get; set; } = new List<HotelResponseRecDTO>();
    }

    public class CityDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CountryId { get; set; }
        public string? CountryName { get; set; }
        public ICollection<HotelResponseDTO> Hotels { get; set; } = new List<HotelResponseDTO>();
    }

    public class CityWithCountryResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CountryId { get; set; }
        public string? CountryName { get; set; }
    }
}