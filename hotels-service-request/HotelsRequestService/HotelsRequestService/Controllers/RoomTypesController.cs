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
    public class RoomTypesController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;

        public RoomTypesController(IMapper mapper, ApiDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: api/RoomTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomTypeResponseDTO>>> GetRooms()
        {
            var roomTypes = await _context.Rooms.ToListAsync();
            var roomTypesDTO = _mapper.Map<List<RoomTypeResponseDTO>>(roomTypes);

            return Ok(roomTypesDTO);
        }

        // GET: api/RoomTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomTypeResponseDTO>> GetRoomType(int id)
        {
            var roomType = await _context.Rooms.FindAsync(id);
            if (roomType == null) { return NotFound(); }
            var roomTypeDTO = _mapper.Map<RoomTypeResponseDTO>(roomType);

            return Ok(roomTypeDTO);
        }

        // PUT: api/RoomTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoomType(int id, RoomTypeCreateDTO roomTypeDTO)
        {
            var roomType = _mapper.Map<RoomType>(roomTypeDTO);
            roomType.Id = id;

            _context.Entry(roomType).State = EntityState.Modified;

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomTypeExists(id)) { return NotFound(); }
                else { throw; }
            }

            return CreatedAtAction("GetRoomType", new { id = roomType.Id }, roomType);
        }

        // POST: api/RoomTypes
        [HttpPost]
        public async Task<ActionResult<RoomType>> PostRoomType(RoomTypeCreateDTO roomTypeDTO)
        {
            var roomType = _mapper.Map<RoomType>(roomTypeDTO);

            _context.Rooms.Add(roomType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoomType", new { id = roomType.Id }, roomType);
        }

        // DELETE: api/RoomTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomType(int id)
        {
            var roomType = await _context.Rooms.FindAsync(id);
            if (roomType == null) { return NotFound(); }

            _context.Rooms.Remove(roomType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomTypeExists(int id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }
    }
}
