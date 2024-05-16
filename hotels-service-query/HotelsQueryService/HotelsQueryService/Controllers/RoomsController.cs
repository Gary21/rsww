using AutoMapper;
using HotelsQueryService.Data;
using HotelsQueryService.DTOs;
using HotelsQueryService.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelsQueryService.Controllers
{
    [Route("api/Hotels/{id}/rooms")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;

        public RoomsController(IMapper mapper, ApiDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: api/Hotels/5/rooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomResponseDTO>>> GetRooms(int id)
        {
            var hotel = await _context.Hotels
                .Include(h => h.Rooms)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hotel == null) { return NotFound(); }

            var roomsDTO = _mapper.Map<List<RoomResponseDTO>>(hotel.Rooms);

            return Ok(roomsDTO);
        }

        // GET: api/Hotels/5/rooms/5
        [HttpGet("{roomId}")]
        public async Task<ActionResult<RoomResponseDTO>> GetRoom(int id, int roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null) { return NotFound(); }

            var roomDTO = _mapper.Map<RoomResponseDTO>(room);

            return Ok(roomDTO);
        }

        // PUT: api/Hotels/5/rooms/5
        [HttpPut("{roomId}")]
        public async Task<IActionResult> PutRoom(int id, int roomId, RoomCreateDTO roomDTO)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null) { return NotFound(); }

            _mapper.Map(roomDTO, room);

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(room.HotelId, room.RoomNumber)) { return NotFound(); }
                else { throw; }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<RoomResponseDTO>> PostRoom(int id, RoomCreateDTO roomDTO)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) { return NotFound(); }

            var room = _mapper.Map<Room>(roomDTO);
            room.Hotel = hotel;

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            var roomDetailsDTO = _mapper.Map<RoomResponseDTO>(room);

            return CreatedAtAction("GetRooms", new { id = roomDetailsDTO.Id }, roomDetailsDTO);
        }

        // DELETE: api/Hotels/5/rooms/5
        [HttpDelete("{roomId}")]
        public async Task<IActionResult> DeleteRoom(int id, int roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null) { return NotFound(); }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomExists(int hotelId, int roomNumber)
        {
            return _context.Rooms.Any(e => e.Hotel.Id == hotelId && e.RoomNumber == roomNumber);
        }
        
    }
}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using AutoMapper;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using HotelsQueryService.Data;
//using HotelsQueryService.DTOs;
//using HotelsQueryService.Entities;
//using static HotelsQueryService.DTOs.HasRoomDTOs;

//namespace HotelsQueryService.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class HotelsController : ControllerBase
//    {
//        private readonly ApiDbContext _context;
//        private readonly IMapper _mapper;

//        public HotelsController(IMapper mapper, ApiDbContext context)
//        {
//            _mapper = mapper;
//            _context = context;
//        }

//        // GET: api/Hotels
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<HotelResponseDTO>>> GetHotels()
//        {
//            var hotels = await _context.Hotels
//                .Include(h => h.City)
//                .ToListAsync();

//            var hotelsDTO = _mapper.Map<List<HotelResponseDTO>>(hotels);

//            foreach (var hotelDTO in hotelsDTO)
//            {
//                hotelDTO.CityId = (hotels.FirstOrDefault(h => h.Id == hotelDTO.Id)?.City.Id);
//                hotelDTO.CityName = hotels.FirstOrDefault(h => h.Id == hotelDTO.Id)?.City.Name;
//            }

//            return Ok(hotelsDTO);
//        }

//        // GET: api/Hotels/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<HotelDetailsDTO>> GetHotel(int id)
//        {
//            var hotel = await _context.Hotels
//                .Include(h => h.City)
//                .FirstOrDefaultAsync(h => h.Id == id);

//            if (hotel == null) { return NotFound(); }

//            var hotelDTO = _mapper.Map<HotelDetailsDTO>(hotel);

//            hotelDTO.CityId = hotel?.City.Id;
//            hotelDTO.CityName = hotel?.City.Name;

//            return Ok(hotelDTO);

//        }

//        // GET: api/Hotels/5
//        [HttpGet("{id}/rooms")]
//        public async Task<ActionResult<HotelDetailsWithRoomsDTO>> GetHotelWithRooms(int id)
//        {
//            var hotel = await _context.Hotels
//                .Include(h => h.City)
//                .Include(h => h.Rooms)
//                .FirstOrDefaultAsync(h => h.Id == id);

//            if (hotel == null) { return NotFound(); }

//            var hotelDTO = _mapper.Map<HotelDetailsWithRoomsDTO>(hotel);

//            hotelDTO.CityId = hotel?.City.Id;
//            hotelDTO.CityName = hotel?.City.Name;

//            var rooms = _mapper.Map<List<RoomResponseDTO>>(hotel.Rooms);
//            hotelDTO.Rooms = rooms;

//            return Ok(hotelDTO);

//        }

//        // PUT: api/Hotels/5
//        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//        [HttpPut("{id}")]
//        public async Task<IActionResult> PutHotel(int id, HotelCreateDTO hotelDTO)
//        {
//            var city = await _context.Cities.FindAsync(hotelDTO.CityId);
//            if (city == null) { return BadRequest(); }

//            var hotel = _mapper.Map<Hotel>(hotelDTO);
//            hotel.Id = id;

//            if (id != hotel.Id) { return BadRequest(); }

//            _context.Entry(hotel).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!HotelExists(id)) { return NotFound(); }
//                else { throw; }
//            }

//            return NoContent();
//        }

//        // POST: api/Hotels
//        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//        [HttpPost]
//        public async Task<ActionResult<Hotel>> PostHotel(HotelCreateDTO hotelDTO)
//        {
//            var city = await _context.Cities.FindAsync(hotelDTO.CityId);
//            if (city == null) { return BadRequest(); }

//            var hotel = _mapper.Map<Hotel>(hotelDTO);
//            hotel.City = city;

//            _context.Hotels.Add(hotel);
//            await _context.SaveChangesAsync();

//            var hotelDetailsDTO = _mapper.Map<HotelDetailsDTO>(hotel);

//            return CreatedAtAction("GetHotel", new { id = hotelDetailsDTO.Id }, hotelDetailsDTO);
//        }

//        // DELETE: api/Hotels/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteHotel(int id)
//        {
//            var hotel = await _context.Hotels.FindAsync(id);
//            if (hotel == null) { return NotFound(); }

//            _context.Hotels.Remove(hotel);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        private bool HotelExists(int id)
//        {
//            return _context.Hotels.Any(e => e.Id == id);
//        }
//    }
//}
