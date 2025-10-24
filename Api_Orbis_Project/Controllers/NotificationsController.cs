using Api_Orbis_Project.Data;
using Api_Orbis_Project.Dtos;
using Api_Orbis_Project.Models;
using Api_Orbis_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Api_Orbis_Project.Controllers
{
    [ApiController]
    [Route("notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IExpoPushService _expo;

        public NotificationsController(AppDbContext db, IExpoPushService expo)
        {
            _db = db;
            _expo = expo;
        }

        // ✅ Registrar token de dispositivo
        [HttpPost("register")]
        [Authorize]
        public async Task<IActionResult> RegisterToken([FromBody] RegisterPushTokenDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Token))
                return BadRequest("Token requerido");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("Usuario no autenticado");

            var existing = await _db.DeviceRegistrations
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ExpoPushToken == dto.Token);

            if (existing == null)
            {
                var reg = new DeviceRegistration
                {
                    UserId = userId,
                    ExpoPushToken = dto.Token,
                    Platform = dto.Platform,
                    IsActive = true
                };
                _db.DeviceRegistrations.Add(reg);
            }
            else
            {
                existing.UpdatedAt = DateTime.UtcNow;
                existing.IsActive = true;
                _db.DeviceRegistrations.Update(existing);
            }

            await _db.SaveChangesAsync();
            return Ok(new { success = true });
        }

        // ✅ Enviar notificación de prueba
        [HttpPost("send-test")]
        [Authorize]
        public async Task<IActionResult> SendTest([FromBody] Dictionary<string, object>? data)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var tokens = await _db.DeviceRegistrations
                .Where(x => x.UserId == userId && x.IsActive)
                .Select(x => x.ExpoPushToken)
                .ToListAsync();

            if (tokens.Count == 0)
                return BadRequest("No hay tokens registrados");

            await _expo.SendAsync(tokens, "Prueba", "Hola desde Orbis 👋", data);
            return Ok(new { success = true, sent = tokens.Count });
        }

        // ✅ Enviar a todos los usuarios (admin)
        [HttpPost("send")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendToAll([FromBody] SendNotificationDto dto)
        {
            var tokens = await _db.DeviceRegistrations
                .Where(x => x.IsActive)
                .Select(x => x.ExpoPushToken)
                .Distinct()
                .ToListAsync();

            if (tokens.Count == 0)
                return BadRequest("No hay tokens registrados");

            await _expo.SendAsync(tokens, dto.Title, dto.Message, dto.Data);
            return Ok(new { success = true, sent = tokens.Count });
        }
    }
}

