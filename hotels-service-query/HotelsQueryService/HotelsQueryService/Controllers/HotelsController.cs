using AutoMapper;
using HotelsQueryService.Data;
using HotelsQueryService.DTOs;
using HotelsQueryService.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelsQueryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;

        public HotelsController(IMapper mapper, ApiDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: api/Hotels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelResponseDTO>>> GetHotels()
        {
            var hotels = await _context.Hotels
                .Include(h => h.City)
                .ToListAsync();

            var hotelsDTO = _mapper.Map<List<HotelResponseDTO>>(hotels);

            foreach (var hotelDTO in hotelsDTO)
            {
                hotelDTO.CityId = (hotels.FirstOrDefault(h => h.Id == hotelDTO.Id)?.City.Id);
                hotelDTO.CityName = hotels.FirstOrDefault(h => h.Id == hotelDTO.Id)?.City.Name;
            }

            return Ok(hotelsDTO);
        }

        // GET: api/Hotels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDetailsDTO>> GetHotel(int id)
        {
            var hotel = await _context.Hotels
                .Include(h => h.City)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hotel == null) { return NotFound(); }

            var hotelDTO = _mapper.Map<HotelDetailsDTO>(hotel);

            hotelDTO.CityId = hotel?.City.Id;
            hotelDTO.CityName = hotel?.City.Name;

            return Ok(hotelDTO);

        }

        // GET: api/Hotels/5
        [HttpGet("{id}/HotelRooms")]
        public async Task<ActionResult<HotelDetailsWithRoomsDTO>> GetHotelWithRooms(int id)
        {
            var hotel = await _context.Hotels
                .Include(h => h.City)
                .Include(h => h.Rooms)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hotel == null) { return NotFound(); }

            var hotelDTO = _mapper.Map<HotelDetailsWithRoomsDTO>(hotel);

            hotelDTO.CityId = hotel?.City.Id;
            hotelDTO.CityName = hotel?.City.Name;

            var rooms = _mapper.Map<List<RoomResponseDTO>>(hotel.Rooms);
            hotelDTO.HasRooms = rooms;

            return Ok(hotelDTO);

        }

        // PUT: api/Hotels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel(int id, HotelCreateDTO hotelDTO)
        {
            var city = await _context.Cities.FindAsync(hotelDTO.CityId);
            if (city == null) { return BadRequest(); }

            var hotel = _mapper.Map<Hotel>(hotelDTO);
            hotel.Id = id;

            if (id != hotel.Id) { return BadRequest(); }

            _context.Entry(hotel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HotelExists(id)) { return NotFound(); }
                else { throw; }
            }

            return NoContent();
        }

        // POST: api/Hotels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Hotel>> PostHotel(HotelCreateDTO hotelDTO)
        {
            var city = await _context.Cities.FindAsync(hotelDTO.CityId);
            if (city == null) { return BadRequest(); }

            var hotel = _mapper.Map<Hotel>(hotelDTO);
            hotel.City = city;

            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();

            var hotelDetailsDTO = _mapper.Map<HotelDetailsDTO>(hotel);

            return CreatedAtAction("GetHotel", new { id = hotelDetailsDTO.Id }, hotelDetailsDTO);
        }

        // DELETE: api/Hotels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) { return NotFound(); }

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HotelExists(int id)
        {
            return _context.Hotels.Any(e => e.Id == id);
        }
    }
}
