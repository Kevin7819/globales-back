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

        private readonly IGeocodingService _geocodingService;

        public TripController(AppDbContext context, IGeocodingService geocodingService)
        {
            _context = context;
            _geocodingService = geocodingService;
        }

        private int GetUserIdFromToken()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateTrip([FromBody] CreateTripRequestDto dto)
        {
            var userId = GetUserIdFromToken();
            var trip = dto.ToTripFromCreateDto(userId);

            var (lat, lng, countryCode) = await _geocodingService.GetCoordinatesAndCountry(dto.Destination);
            
            trip.CountryCode = countryCode;
            trip.Latitude = lat;
            trip.Longitude = lng;

            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();

            return Ok(trip.ToDto());
        }

        [HttpGet("nearest")]
        public async Task<IActionResult> GetNearestTrip()
        {
            var userId = GetUserIdFromToken();
            var currentDate = DateTime.UtcNow;

            var nearestTrip = await _context.Trips
                .Where(t => t.UserId == userId && t.DepartureDate >= currentDate)
                .OrderBy(t => t.DepartureDate)
                .FirstOrDefaultAsync();

            if (nearestTrip == null)
            {
                nearestTrip = await _context.Trips
                    .Where(t => t.UserId == userId)
                    .OrderByDescending(t => t.DepartureDate)
                    .FirstOrDefaultAsync();
            }

            if (nearestTrip == null)
                return NotFound(new { message = "No trips found" });

            return Ok(nearestTrip.ToDto());
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
