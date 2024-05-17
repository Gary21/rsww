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
    public class CatalogRequestController : ControllerBase
    {
        private readonly CatalogRequestPublisher _catalogQueryPublisher;

        public CatalogRequestController(PublisherServiceBase catalogQueryPublisher)
        {
            _catalogQueryPublisher = (CatalogRequestPublisher)catalogQueryPublisher;
        }


        // GET: api/<HotelsController>
        [HttpPost("hotels")]
        public async Task<ActionResult> Reserve(
            [FromQuery] int? hotelId = null,
            [FromQuery] int? roomNumber = null,
            [FromQuery] DateTime? checkIn = null,
            [FromQuery] DateTime? checkOut = null
            )
        {
            if (hotelId == null || roomNumber == null || checkIn == null || checkOut == null)
            {
                return BadRequest("Invalid request parameters");
            }

            RoomReserveRequest request = new RoomReserveRequest();
            request.hotelId = hotelId.Value;
            request.roomNumber = roomNumber.Value;
            request.checkIn = checkIn.Value;
            request.checkOut = checkOut.Value;

            var success = await _catalogQueryPublisher.ReserveRoom(request);
            if (success)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }


        [HttpPost("transports")]
        public async Task<ActionResult> Reserve(
            [FromQuery] int transportId,
            [FromQuery] int numberOfPassengers
            )
        {
            if (transportId == null || numberOfPassengers == null)
            {
                return BadRequest("Invalid request parameters");
            }

            TransportReserveRequest request = new TransportReserveRequest();
            request.transportId = transportId;
            request.numberOfPassengers = numberOfPassengers;

            var success = await _catalogQueryPublisher.ReserveTransport(request);
            if (success)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }


    }
}
