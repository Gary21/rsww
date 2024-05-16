using Microsoft.AspNetCore.Mvc;
using CatalogRequestService.DTOs;
using CatalogRequestService.QueryPublishers;
using CatalogQueryService.DTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CatalogRequestService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly HotelRequestPublisher _hotelQueryPublisher;

        public HotelsController(HotelRequestPublisher hotelQueryPublisher)
        {
            _hotelQueryPublisher = hotelQueryPublisher;
        }


        // GET: api/<HotelsController>
        [HttpGet("all")]
        public async Task<IEnumerable<HotelDTO>> Get()
        {
            var hotels = await _hotelQueryPublisher.GetHotelsAll();
            return hotels;
        }


        // GET: api/<HotelsController>/Countries
        [HttpGet("Countries")]
        public async Task<IEnumerable<CountryDTO>> GetCountries()
        {
            var countries = await _hotelQueryPublisher.GetCountriesAll();
            return countries;
        }

        // GET: api/<HotelsController>/Cities
        [HttpGet("Cities")]
        public async Task<IEnumerable<CityDTO>> GetCities()
        {
            var cities = await _hotelQueryPublisher.GetCitiesAll();
            return cities;
        }

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
