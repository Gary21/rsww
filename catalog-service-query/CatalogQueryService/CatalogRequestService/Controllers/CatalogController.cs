using CatalogQueryService.Filters;
using CatalogQueryService.Queries;
using CatalogRequestService.DTOs;
using CatalogRequestService.QueryPublishers;
using Microsoft.AspNetCore.Mvc;
using RabbitUtilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CatalogRequestService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogQueryPublisher _catalogQueryPublisher;

        public CatalogController(PublisherServiceBase catalogQueryPublisher)
        {
            _catalogQueryPublisher = (CatalogQueryPublisher)catalogQueryPublisher;
        }


        // GET: api/<HotelsController>
        [HttpGet("hotels")]
        public async Task<IEnumerable<HotelDTO>> Get(
            [FromQuery] List<int>? countryIds = null, 
            [FromQuery] List<int>? cityIds = null, 
            [FromQuery] List<int>? roomTypes = null,
            [FromQuery] List<int>? roomCapacities = null,
            [FromQuery] int? minPrice = null,
            [FromQuery] int? maxPrice = null
            )
        {
            HotelsGetQuery query = new HotelsGetQuery();
            query.filters = new HotelQueryFilters();

            if (countryIds != null) { query.filters.CountryIds = countryIds; }
            if (cityIds != null) { query.filters.CityIds = cityIds;}
            if (roomTypes != null) { query.filters.RoomTypeIds = roomTypes; }
            if (roomCapacities != null) { query.filters.RoomCapacities = roomCapacities; }
            if (minPrice != null) { query.filters.MinPrice = minPrice; }
            if (maxPrice != null) { query.filters.MaxPrice = maxPrice; }

            var hotels = await _catalogQueryPublisher.GetHotels(query);
            return hotels;
        }


        [HttpGet("transports")]
        public async Task<IEnumerable<TransportDTO>> Get(
            [FromQuery] List<int>? departureCityIds = null,
            [FromQuery] List<int>? arrivalCityIds = null,
            [FromQuery] List<string>? transportTypes = null,
            [FromQuery] int? numberOfPassengers = null
            )
        {
            TransportGetQuery query = new TransportGetQuery();
            query.filters = new TransportQueryFilters();

            if (departureCityIds != null) { query.filters.DepartureCityIds = departureCityIds; }
            if (arrivalCityIds != null) { query.filters.ArrivalCityIds = arrivalCityIds; }
            if (transportTypes != null) { query.filters.TransportTypes = transportTypes; }
            if (numberOfPassengers != null) { query.filters.NumberOfPassengers = numberOfPassengers; }

            var transports = await _catalogQueryPublisher.GetTransports(query);
            return transports;
        }


    }
}
