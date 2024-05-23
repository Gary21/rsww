using api_gateway.DTOs;
using api_gateway.Queries;
using Microsoft.AspNetCore.Mvc;

namespace api_gateway.Controllers;

[ApiController]
[Route("[controller]")]
public class OffersController : ControllerBase
{

    private readonly ILogger<OffersController> _logger;

    public OffersController(ILogger<OffersController> logger)
    {
        _logger = logger;
    }

    [HttpGet("GetDestinations")]
    public IEnumerable<string> Get()
    {
        return new string[] { "Destination 1", "Destination 2", "Destination 3" };
    }
    
    [HttpGet("GetHotels")]
    public IEnumerable<HotelDTO> GetHotels([FromQuery] HotelsQuery query)
    {
        return new List<HotelDTO>
        {
            new HotelDTO
            {
                Id = 1,
                Name = "Hotel 1",
                Address = "Address 1",
                Description = "Description 1",
                Rating = 5,
                CityName = "City 1"
            },
            new HotelDTO
            {
                Id = 2,
                Name = "Hotel 2",
                Address = "Address 2",
                Description = "Description 2",
                Rating = 4,
                CityName = "City 2"
            },
            new HotelDTO
            {
                Id = 3,
                Name = "Hotel 3",
                Address = "Address 3",
                Description = "Description 3",
                Rating = 3,
                CityName = "City 3"
            }
        };
    }
    
    [HttpGet("GetHotel")]
    public HotelDTO GetHotel(int id)
    {
        return new HotelDTO
        {
            Id = id,
            Name = "Hotel 1",
            Address = "Address 1",
            Description = "Description 1",
            Rating = 5,
            CityName = "City 1"
        };
    }
    
    [HttpGet("GetAvailability")]
    public bool GetAvailability([FromQuery] string hotelId)
    {
        return new Random().Next(0, 2) == 0;
    }
    
    [HttpPost("MakeReservation")]
    public string MakeReservation([FromQuery] ReservationQuery query)
    {
        return "1111-2222-3333-4444";
    }
    
    [HttpPost("BuyReservation")]
    public bool BuyReservation([FromQuery] string hotelId)
    {
        return new Random().Next(0, 2) == 0;
    }
}