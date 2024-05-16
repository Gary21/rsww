using AutoMapper;
using HotelsQueryService.Entities;

namespace HotelsQueryService.DTOs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Country, CountryResponseDTO>();
            CreateMap<Country, CountryResponseRecDTO>();
            CreateMap<Country, CountryDetailsDTO>();
            CreateMap<CountryCreateDTO, Country>();

            CreateMap<City, CityResponseDTO>();
            CreateMap<City, CityResponseRecDTO>();
            CreateMap<City, CityDetailsDTO>();
            CreateMap<City, CityWithCountryResponseDTO>();
            CreateMap<CityCreateDTO, City>();

            CreateMap<Hotel, HotelResponseDTO>();
            CreateMap<Hotel, HotelResponseRecDTO>();
            CreateMap<Hotel, HotelDetailsDTO>();
            CreateMap<Hotel, HotelDetailsWithRoomsDTO>();
            CreateMap<HotelCreateDTO, Hotel>();

            CreateMap<RoomType, RoomTypeResponseDTO>();
            CreateMap<RoomTypeCreateDTO, RoomType>();


        }
    }
}
