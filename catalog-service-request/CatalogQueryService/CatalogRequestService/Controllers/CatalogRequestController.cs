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
            [FromQuery] int hotelId,
            [FromQuery] int roomTypeId,
            [FromQuery] List<DateTime>? checkIn,
            [FromQuery] List<DateTime>? checkOut,
            [FromQuery] int reservationId
            )
        {
            if (hotelId == null || roomTypeId == null || checkIn == null || checkOut == null || reservationId == null)
            {
                return BadRequest("Invalid request parameters");
            }

            RoomReserveRequest request = new RoomReserveRequest();
            request.HotelId = hotelId;
            request.RoomTypeId = roomTypeId;
            request.CheckInDate = checkIn.FirstOrDefault();
            request.CheckOutDate = checkOut.FirstOrDefault();
            request.ReservationId = reservationId;

            var result = await _catalogQueryPublisher.ReserveRoom(request);
            return Ok(result);
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
