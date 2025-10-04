using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api_Orbis_Project.Data;
using Api_Orbis_Project.Dtos.Trip;
using Api_Orbis_Project.Mappers;
using Api_Orbis_Project.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace Api_Orbis_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TripController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TripController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserIdFromToken()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrip([FromBody] CreateTripRequestDto dto)
        {
            var userId = GetUserIdFromToken();
            var trip = dto.ToTripFromCreateDto(userId);

            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();

            return Ok(trip.ToDto());
        }

        [HttpGet]
        public async Task<IActionResult> GetUserTrips()
        {
            var userId = GetUserIdFromToken();
            var trips = await _context.Trips
                .Where(t => t.UserId == userId)
                .ToListAsync();

            return Ok(trips.Select(t => t.ToDto()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTripById(int id)
        {
            var userId = GetUserIdFromToken();
            var trip = await _context.Trips.FirstOrDefaultAsync(t => t.TripId == id && t.UserId == userId);

            if (trip == null)
                return NotFound(new { message = "Trip not found or unauthorized" });

            return Ok(trip.ToDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrip(int id, [FromBody] UpdateTripRequestDto dto)
        {
            var userId = GetUserIdFromToken();
            var trip = await _context.Trips.FirstOrDefaultAsync(t => t.TripId == id && t.UserId == userId);

            if (trip == null)
                return NotFound(new { message = "Trip not found or unauthorized" });

            trip.UpdateTripFromDto(dto);
            await _context.SaveChangesAsync();

            return Ok(trip.ToDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrip(int id)
        {
            var userId = GetUserIdFromToken();
            var trip = await _context.Trips.FirstOrDefaultAsync(t => t.TripId == id && t.UserId == userId);

            if (trip == null)
                return NotFound(new { message = "Trip not found or unauthorized" });

            _context.Trips.Remove(trip);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
