using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Api_Orbis_Project.Services;

namespace Api_Orbis_Project.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AiController : ControllerBase
    {
        private readonly IIAService _aiService;

        public AiController(IIAService aiService)
        {
            _aiService = aiService;
        }

        // 🔹 1. Endpoint general para preguntas (sin autenticación)
        [HttpGet("ask")]
        [AllowAnonymous]
        public async Task<IActionResult> Ask([FromQuery] string question, [FromQuery] string lang = "es")
        {
            if (string.IsNullOrWhiteSpace(question))
                return BadRequest(new { error = "Question cannot be empty." });

            try
            {
                var answer = await _aiService.GetGeneralAnswerAsync(question, lang);
                return Ok(new { answer });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new { error = "Error calling Hugging Face API", details = ex.Message });
            }
        }

        // 🔹 2. Datos crudos para el mapa (sin autenticación)
        [HttpGet("mapdata/raw/{countryCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMapDataRaw(string countryCode)
        {
            var raw = await _aiService.GetMapDataRawAsync(countryCode);
            return Ok(new { raw });
        }

        // 🔹 3. Mapa limpiado (sin autenticación)
        [HttpGet("mapdata/clean/{countryCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMapDataClean(string countryCode)
        {
            var raw = await _aiService.GetMapDataRawAsync(countryCode);
            var clean = raw.Replace("```json", "").Replace("```", "").Trim();
            return Ok(new { clean });
        }

        // 🔹 4. Mapa parseado a GeoJSON (sin autenticación)
        [HttpGet("mapdata/geojson/{countryCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMapDataGeoJson(string countryCode)
        {
            var raw = await _aiService.GetMapDataRawAsync(countryCode);
            var clean = raw.Replace("```json", "").Replace("```", "").Trim();

            try
            {
                var geojson = JsonSerializer.Deserialize<object>(clean);
                return Ok(geojson);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = "Invalid GeoJSON format",
                    details = ex.Message,
                    raw = clean
                });
            }
        }

        // 🔹 5. Categoría: Salud (sin autenticación)
        [HttpGet("mapdata/salud/{countryCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSalud(string countryCode)
        {
            var data = await _aiService.GetMapDataByCategoryAsync(countryCode, "salud");
            return Ok(data);
        }

        // 🔹 6. Categoría: Seguridad (sin autenticación)
        [HttpGet("mapdata/seguridad/{countryCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSeguridad(string countryCode)
        {
            var data = await _aiService.GetMapDataByCategoryAsync(countryCode, "seguridad");
            return Ok(data);
        }

        // 🔹 7. Categoría: Cultura (sin autenticación)
        [HttpGet("mapdata/cultura/{countryCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCultura(string countryCode)
        {
            var data = await _aiService.GetMapDataByCategoryAsync(countryCode, "cultura");
            return Ok(data);
        }
    }
}