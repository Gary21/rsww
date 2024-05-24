using CatalogQueryService.DTOs;
using CatalogQueryService.Filters;
using CatalogQueryService.Queries;
using CatalogQueryService.QueryPublishers;
using Microsoft.AspNetCore.Mvc;
using RabbitUtilities;
using CatalogRequestService.DTOs;
using CatalogQueryService.QueryHandler;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CatalogQueryService.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    //public class CatalogController : ControllerBase
    //{
    //    private readonly CatalogQueryPublisher _catalogQueryPublisher;
    //    private readonly CatalogQueryHandler _catalogQueryHandler;

    //    public CatalogController(PublisherServiceBase catalogQueryPublisher, PublisherServiceBase catalogQueryHandler)
    //    {
    //        _catalogQueryPublisher = (CatalogQueryPublisher)catalogQueryPublisher;
    //        _catalogQueryHandler = (CatalogQueryHandler)catalogQueryHandler;
    //    }

    //    [HttpGet("countries")]
    //    public async Task<IEnumerable<CountryDTO>> GetCountries()
    //    {
    //        var countries = await _catalogQueryPublisher.GetCountries();
    //        return countries;
    //    }

    //    [HttpGet("cities")]
    //    public async Task<IEnumerable<CityDTO>> GetCities()
    //    {
    //        var cities = await _catalogQueryPublisher.GetCities();
    //        return cities;
    //    }


    //    [HttpGet("testCatalogWhole")]
    //    public async Task<IEnumerable<TripDTO>> GetTripsTest(
    //        [FromQuery] List<int>? hotelIds = null,
    //        [FromQuery] List<int>? cityIds = null,
    //        [FromQuery] List<int>? countryIds = null,
    //        [FromQuery] List<string>? roomTypes = null,
    //        [FromQuery] int? peopleNumber = null,
    //        [FromQuery] DateTime? checkInDate = null,
    //        [FromQuery] DateTime? checkOutDate = null,
    //        [FromQuery] int? minPrice = null,
    //        [FromQuery] int? maxPrice = null,
    //        [FromQuery] List<int>? departureCityIds = null,
    //        [FromQuery] List<string>? transportTypes = null
    //        )
    //    {
    //        TripGetQuery query = new TripGetQuery();
    //        query.filters = new TripQueryFilters();

    //        if (hotelIds != null) { query.filters.HotelIds = hotelIds; }
    //        if (cityIds != null) { query.filters.CityIds = cityIds; }
    //        if (countryIds != null) { query.filters.CountryIds = countryIds; }
    //        if (roomTypes != null) { query.filters.RoomTypes = roomTypes; }
    //        if (peopleNumber != null) { query.filters.PeopleNumber = peopleNumber.Value; }
    //        if (checkInDate != null) { query.filters.CheckInDate = checkInDate; }
    //        if (checkOutDate != null) { query.filters.CheckOutDate = checkOutDate; }
    //        if (minPrice != null) { query.filters.MinPrice = minPrice; }
    //        if (maxPrice != null) { query.filters.MaxPrice = maxPrice; }
    //        if (departureCityIds != null) { query.filters.DepartureCityIds = departureCityIds; }
    //        if (transportTypes != null) { query.filters.TransportTypes = transportTypes; }
            

    //        var trips = await _catalogQueryHandler.GetTripsTest(query);

    //        return trips;
    //    }

    //    // GET: api/<HotelsController>
    //    [HttpGet("hotels")]
    //    public async Task<IEnumerable<HotelDTO>> Get(
    //        [FromQuery] List<int>? hotelId = null,
    //        [FromQuery] List<int>? countryIds = null, 
    //        [FromQuery] List<int>? cityIds = null, 
    //        [FromQuery] List<string>? roomTypes = null,
    //        [FromQuery] List<int>? roomCapacities = null,
    //        [FromQuery] List<DateTime>? checkInDate = null,
    //        [FromQuery] List<DateTime>? checkOutDate = null,
    //        [FromQuery] int? minPrice = null,
    //        [FromQuery] int? maxPrice = null
    //        )
    //    {
    //        HotelsGetQuery query = new HotelsGetQuery();
    //        query.filters = new HotelQueryFilters();

    //        if (hotelId != null) { query.filters.HotelIds = hotelId; }
    //        if (countryIds != null) { query.filters.CountryIds = countryIds; }
    //        if (cityIds != null) { query.filters.CityIds = cityIds;}
    //        if (roomTypes != null) { query.filters.RoomTypes = roomTypes; }
    //        if (roomCapacities != null) { query.filters.RoomCapacities = roomCapacities; }
    //        if (checkInDate != null) { query.filters.CheckInDate = checkInDate.FirstOrDefault(); }
    //        if (checkOutDate != null) { query.filters.CheckOutDate = checkOutDate.FirstOrDefault(); }
    //        if (minPrice != null) { query.filters.MinPrice = minPrice; }
    //        if (maxPrice != null) { query.filters.MaxPrice = maxPrice; }

    //        var hotels = await _catalogQueryPublisher.GetHotels(query);
    //        return hotels;
    //    }





    //    [HttpGet("transports")]
    //    public async Task<IEnumerable<TransportDTO>> Get(
    //        [FromQuery] List<int>? departureCityIds = null,
    //        [FromQuery] List<int>? arrivalCityIds = null,
    //        [FromQuery] List<string>? transportTypes = null,
    //        [FromQuery] int? numberOfPassengers = null
    //        )
    //    {
    //        TransportGetQuery query = new TransportGetQuery();
    //        query.filters = new TransportQueryFilters();

    //        if (departureCityIds != null) { query.filters.DepartureCityIds = departureCityIds; }
    //        if (arrivalCityIds != null) { query.filters.ArrivalCityIds = arrivalCityIds; }
    //        if (transportTypes != null) { query.filters.TransportTypes = transportTypes; }
    //        if (numberOfPassengers != null) { query.filters.NumberOfPassengers = numberOfPassengers; }

    //        var transports = await _catalogQueryPublisher.GetTransports(query);
    //        return transports;
    //    }


    //}
}

//public class TripQueryFilters
//{
//    [Key(0)]
//    public IEnumerable<int>? HotelIds { get; set; } = null;
//    [Key(1)]
//    public IEnumerable<int>? CityIds { get; set; } = null;
//    [Key(2)]
//    public IEnumerable<int>? CountryIds { get; set; } = null;
//    [Key(3)]
//    public IEnumerable<string>? RoomTypes { get; set; } = null;
//    [Key(4)]
//    public int PeopleNumber { get; set; } = 1;
//    [Key(5)]
//    public DateTime? CheckInDate { get; set; } = null;
//    [Key(6)]
//    public DateTime? CheckOutDate { get; set; } = null;
//    [Key(7)]
//    public int? MinPrice { get; set; } = null;
//    [Key(8)]
//    public int? MaxPrice { get; set; } = null;


//    [Key(9)]
//    public IEnumerable<int>? DepartureCityIds { get; set; } = null;
//    [Key(10)]
//    public IEnumerable<string>? TransportTypes { get; set; } = null;


//    public TripQueryFilters() { }
//}