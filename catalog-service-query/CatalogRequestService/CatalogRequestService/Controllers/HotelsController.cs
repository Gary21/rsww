using Microsoft.AspNetCore.Mvc;
using CatalogRequestService.DTOs;
using CatalogRequestService.QueryPublishers;
using CatalogQueryService.DTOs;
using RabbitUtilities;
using CatalogQueryService.Queries;
using CatalogQueryService.Filters;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CatalogRequestService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly HotelQueryPublisher _hotelQueryPublisher;

        public HotelsController(PublisherServiceBase hotelQueryPublisher)
        {
            _hotelQueryPublisher = (HotelQueryPublisher)hotelQueryPublisher;
        }


        // GET: api/<HotelsController>
        [HttpGet]
        public async Task<IEnumerable<HotelDTO>> Get(
            [FromQuery] List<int>? countryIds = null, 
            [FromQuery] List<int>? cityIds = null, 
            [FromQuery] List<int>? roomTypes = null,
            [FromQuery] List<int>? roomCapacities = null
            )
        {
            HotelsGetQuery query = new HotelsGetQuery();
            query.filters = new Filter();

            if (countryIds != null) { query.filters.CountryIds = countryIds; }
            if (cityIds != null) { query.filters.CityIds = cityIds;}
            if (roomTypes != null) { query.filters.RoomTypeIds = roomTypes; }
            if (roomCapacities != null) { query.filters.RoomCapacities = roomCapacities; }

            var hotels = await _hotelQueryPublisher.GetHotels(query);
            return hotels;
        }


        [HttpGet("countries")]
        public async Task<IEnumerable<CountryDTO>> GetCountries()
        {
            var countries = await _hotelQueryPublisher.GetCountriesAll();
            return countries;
        }

        [HttpGet("cities")]
        public async Task<IEnumerable<CityDTO>> GetCities()
        {
            var cities = await _hotelQueryPublisher.GetCitiesAll();
            return cities;
        }



        // GET: api/<HotelsController>/Countries
        //[HttpGet("Countries")]
        //public async Task<IEnumerable<CountryDTO>> GetCountries()
        //{
        //    var countries = await _hotelQueryPublisher.GetCountriesAll();
        //    return countries;
        //}

        // GET: api/<HotelsController>/Cities
        //[HttpGet("Cities")]
        //public async Task<IEnumerable<CityDTO>> GetCities()
        //{
        //    var cities = await _hotelQueryPublisher.GetCitiesAll();
        //    return cities;
        //}

        // GET api/<HotelsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<HotelsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<HotelsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<HotelsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
