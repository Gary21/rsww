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
    public class CountriesController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;

        public CountriesController(IMapper mapper, ApiDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: api/Countries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CountryDetailsDTO>>> GetCountries()
        {
            var countries = await _context.Countries
                .Include(c => c.Cities)
                .ToListAsync();

            var countriesDTO = _mapper.Map<List<CountryDetailsDTO>>(countries);


            //var countriesDTO = countries.Select(c => new CountryDetailsDTO
            //{
            //    Id = c.Id,
            //    Name = c.Name,
            //    Cities = c.Cities.Select(city => new CityResponseDTO
            //    {
            //        Id = city.Id,
            //        Name = city.Name
            //    }).ToList()
            //});

            return Ok(countriesDTO);
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDetailsDTO>> GetCountry(int id)
        {
            var country = await _context.Countries
                .Include(c => c.Cities)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (country == null) { return NotFound(); }

            var countryDTO = _mapper.Map<CountryDetailsDTO>(country);

            countryDTO.Cities = _mapper.Map<List<CityResponseDTO>>(country.Cities);

            return countryDTO;
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(int id, CountryCreateDTO countryDTO)
        {
            var country = _mapper.Map<Country>(countryDTO);
            country.Id = id;

            if (id != country.Id) { return BadRequest(); }

            _context.Entry(country).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(id)) { return NotFound(); }
                else { throw; }
            }

            return NoContent();
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Country>> PostCountry(CountryCreateDTO countryDTO)
        {
            var country = _mapper.Map<Country>(countryDTO);

            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country == null) { return NotFound(); }

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CountryExists(int id)
        {
            return _context.Countries.Any(e => e.Id == id);
        }
    }
}
