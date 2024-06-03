using AutoMapper;
using HotelsQueryService.Data;
using HotelsQueryService.DTOs;
using HotelsQueryService.Entities;
using HotelsQueryService.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace HotelsQueryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly IDbContextFactory<ApiDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public HotelsController(IMapper mapper, IDbContextFactory<ApiDbContext> repositoryFactory, ApiDbContext context)
        {
            _mapper = mapper;
            _contextFactory = repositoryFactory;
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

        [HttpGet("Test")]
        public async Task<ActionResult<IEnumerable<HotelResponseDTO>>> Test()
        {
            using var repository = _contextFactory.CreateDbContext();
            var mf = new HotelQueryFilters
            {

            };

            var query = from h in repository.Hotels
                            //join room in repository.Rooms on h.Id equals room.HotelId
                            //join occupancy in repository.Occupancies on room equals occupancy.Room
                        join city in repository.Cities on h.City equals city
                        join country in repository.Countries on city.Country equals country

                        where mf.HotelIds == null || mf.HotelIds.Count() == 0 || mf.HotelIds.Contains(h.Id)
                        where mf.CountryIds == null || mf.CountryIds.Count() == 0 || mf.CountryIds.Contains(country.Id)
                        where mf.CityIds == null || mf.CityIds.Count() == 0 || mf.CityIds.Contains(city.Id)

                        where
                                h.Rooms.Any(r =>
                                    (
                                        mf.RoomTypes == null || mf.RoomTypes.Count() == 0 ||
                                        mf.RoomTypes.Contains(r.RoomType.Name)
                                        ) &&

                                    (
                                        mf.MinPrice == null || r.BasePrice >= mf.MinPrice
                                        ) &&

                                    (
                                        mf.MaxPrice == null || r.BasePrice <= mf.MaxPrice
                                        ) &&

                                    (
                                        mf.RoomCapacities == null || mf.RoomCapacities.Count() == 0 ||
                                        (r.RoomType.Capacity >= mf.RoomCapacities.Min() && r.RoomType.Capacity <= mf.RoomCapacities.Max())
                                        ) &&

                                    !r.Occupancies.Any(o => o.Date >= mf.CheckInDate && o.Date <= mf.CheckOutDate)
                                )

                        select new { h, city, country };

            try
            {
                var result = await query.ToListAsync();
                var hotelsDTO = result.Select(r => new HotelDTO
                {
                    Id = r.h.Id,
                    Name = r.h.Name,
                    Description = r.h.Description,
                    Address = r.h.Address,
                    CityId = r.city.Id,
                    CityName = r.city.Name,
                    CountryId = r.country.Id,
                    CountryName = r.country.Name,
                    ImgPaths = r.h.ImgPaths
                }).ToList();

                return Ok(hotelsDTO);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
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
