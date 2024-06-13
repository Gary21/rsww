using MessagePack;

namespace CatalogQueryService.DTOs
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


    //[MessagePackObject]
    //public class HotelDTO
    //{
    //    [Key(0)]
    //    public int Id { get; set; }
    //    [Key(1)]
    //    public string Name { get; set; }
    //    [Key(2)]
    //    public string Address { get; set; }
    //    [Key(3)]
    //    public string Description { get; set; }
    //    [Key(4)]
    //    public decimal Rating { get; set; }
    //    [Key(5)]
    //    public int Stars { get; set; }
    //    [Key(6)]
    //    public bool HasFood { get; set; }
    //    [Key(7)]
    //    public int? CityId { get; set; }
    //    [Key(8)]
    //    public string? CityName { get; set; }
    //    [Key(9)]
    //    public int? CountryId { get; set; }
    //    [Key(10)]
    //    public string? CountryName { get; set; }
    //    [Key(11)]
    //    public string ImgPaths { get; set; }
    //}

    //[MessagePackObject]
    //public class GatewayHotelDTO
    //{
    //    [Key(0)]
    //    public int Id { get; set; }
    //    [Key(1)]
    //    public string Name { get; set; }
    //    [Key(2)]
    //    public string[] ImgUrls { get; set; }
    //    [Key(3)]
    //    public string Address { get; set; }
    //    [Key(4)]
    //    public string Description { get; set; }
    //    [Key(5)]
    //    public int Rating { get; set; }
    //    [Key(6)]
    //    public string? CityName { get; set; }
    //    [Key(7)]
    //    public string? CountryName { get; set; }
    //    [Key(8)]
    //    public int Stars { get; set; }
    //    [Key(9)]
    //    public bool HasFood { get; set; }
    //}

    public class DTOAdapterHotelToGatewayHotel
    {
        public static GatewayHotelDTO Adapt(HotelDTO hotelDTO)
        {
            return new GatewayHotelDTO
            {
                Id = hotelDTO.Id,
                Name = hotelDTO.Name,
                ImgUrls = hotelDTO.ImgPaths.Split(","),
                Address = hotelDTO.Address,
                Description = hotelDTO.Description,
                Rating = (int)(hotelDTO.Rating + 1),
                CityName = hotelDTO.CityName,
                CountryName = hotelDTO.CountryName,
                Stars = hotelDTO.Stars,
                HasFood = hotelDTO.HasFood
            };
        }
    }
}