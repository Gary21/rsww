using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelsRequestService.Data;
using HotelsRequestService.DTOs;
using HotelsRequestService.Entities;

namespace HotelsRequestService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;

        public CitiesController(IMapper mapper, ApiDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: api/Cities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithCountryResponseDTO>>> GetCities()
        {
            var cities = await _context.Cities
                .Include(c => c.Country)
                .ToListAsync();

            var citiesDTO = _mapper.Map<List<CityWithCountryResponseDTO>>(cities);
            foreach (var cityDTO in citiesDTO)
            {
                cityDTO.CountryId = cities.FirstOrDefault(c => c.Id == cityDTO.Id)?.Country.Id;
                cityDTO.CountryName = cities.FirstOrDefault(c => c.Id == cityDTO.Id)?.Country.Name;
            }

            return Ok(citiesDTO);
        }

        // GET: api/Cities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CityDetailsDTO>> GetCity(int id)
        {
            var city = await _context.Cities
                .Include(c => c.Country)
                .Include(c => c.Hotels)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (city == null) { return NotFound(); }

            var cityDTO = _mapper.Map<CityDetailsDTO>(city);

            cityDTO.CountryId = city.Country.Id;
            cityDTO.CountryName = city.Country.Name;
            cityDTO.Hotels = city.Hotels.Select(hotel => new HotelResponseDTO
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Address = hotel.Address,
                Rating = hotel.Rating
            }).ToList();

            return cityDTO;
        }

        // PUT: api/Cities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(int id, CityCreateDTO cityDTO)
        {
            var country = await _context.Countries.FindAsync(cityDTO.CountryId);
            if (country == null) { return BadRequest(); }

            var city = _mapper.Map<City>(cityDTO);
            city.Id = id;

            if (id != city.Id) { return BadRequest(); }

            _context.Entry(city).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(id)) { return NotFound(); }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Cities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<City>> PostCity(CityCreateDTO cityDTO)
        {
            var country = await _context.Countries.FindAsync(cityDTO.CountryId);
            if (country == null) { return BadRequest(); }

            var city = _mapper.Map<City>(cityDTO);
            city.Country = country;

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            var cityResponseDTO = _mapper.Map<CityDetailsDTO>(city);

            return CreatedAtAction("GetCity", new { id = cityResponseDTO.Id }, cityResponseDTO);
        }

        // DELETE: api/Cities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null) { return NotFound(); }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CityExists(int id)
        {
            return _context.Cities.Any(e => e.Id == id);
        }
    }
}
