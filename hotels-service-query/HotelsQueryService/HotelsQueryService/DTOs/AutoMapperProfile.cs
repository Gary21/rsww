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
            CreateMap<Country, CountryDTO>();

            CreateMap<City, CityResponseDTO>();
            CreateMap<City, CityResponseRecDTO>();
            CreateMap<City, CityDetailsDTO>();
            CreateMap<City, CityWithCountryResponseDTO>();
            CreateMap<CityCreateDTO, City>();
            CreateMap<City, CityDTO>();

            CreateMap<Hotel, HotelResponseDTO>();
            CreateMap<Hotel, HotelResponseRecDTO>();
            CreateMap<Hotel, HotelDetailsDTO>();
            CreateMap<Hotel, HotelDetailsWithRoomsDTO>();
            CreateMap<HotelCreateDTO, Hotel>();
            CreateMap<Hotel, HotelDTO>();

            CreateMap<RoomType, RoomTypeResponseDTO>();
            CreateMap<RoomType, RoomTypeDTO>();
            CreateMap<RoomTypeCreateDTO, RoomType>();

            CreateMap<Room, RoomResponseDTO>();


        }
    }
}
