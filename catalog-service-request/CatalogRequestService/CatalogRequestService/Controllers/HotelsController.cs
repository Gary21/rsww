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


        // POST api/<HotelsController>
        [HttpPost]
        public void Post([FromBody] HotelRequestDTO hotelRequestDTO)
        {
            _hotelQueryPublisher.PublishHotelRequest(hotelRequestDTO);
        }
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
