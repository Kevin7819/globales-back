using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Api_Orbis_Project.Services;

namespace Api_Orbis_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly IIAService _aiService;

        public AiController(IIAService aiService)
        {
            _aiService = aiService;
            Console.WriteLine("✅ AiController inicializado");
        }

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

        [HttpGet("mapdata/raw/{countryCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMapDataRaw(string countryCode)
        {
            var raw = await _aiService.GetMapDataRawAsync(countryCode);
            return Ok(new { raw });
        }

        [HttpGet("mapdata/clean/{countryCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMapDataClean(string countryCode)
        {
            var raw = await _aiService.GetMapDataRawAsync(countryCode);
            var clean = raw.Replace("```json", "").Replace("```", "").Trim();
            return Ok(new { clean });
        }

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

        [HttpGet("mapdata/salud/{countryCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSalud(string countryCode)
        {
            try
            {
                var dataString = await _aiService.GetMapDataByCategoryAsync(countryCode, "salud");
                
                // Parsear el string a JSON object
                var jsonObject = JsonSerializer.Deserialize<JsonElement>(dataString);
                return Ok(jsonObject);
            }
            catch (Exception ex)
            {
                // Si falla el parseo, devolver el string limpio
                var dataString = await _aiService.GetMapDataByCategoryAsync(countryCode, "salud");
                var cleanData = dataString.Replace("```json", "").Replace("```", "").Trim();
                return Content(cleanData, "application/json");
            }
        }

        [HttpGet("mapdata/seguridad/{countryCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSeguridad(string countryCode)
        {
            try
            {
                var dataString = await _aiService.GetMapDataByCategoryAsync(countryCode, "seguridad");
                var jsonObject = JsonSerializer.Deserialize<JsonElement>(dataString);
                return Ok(jsonObject);
            }
            catch (Exception ex)
            {
                var dataString = await _aiService.GetMapDataByCategoryAsync(countryCode, "seguridad");
                var cleanData = dataString.Replace("```json", "").Replace("```", "").Trim();
                return Content(cleanData, "application/json");
            }
        }

        [HttpGet("mapdata/cultura/{countryCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCultura(string countryCode)
        {
            try
            {
                var dataString = await _aiService.GetMapDataByCategoryAsync(countryCode, "cultura");
                var jsonObject = JsonSerializer.Deserialize<JsonElement>(dataString);
                return Ok(jsonObject);
            }
            catch (Exception ex)
            {
                var dataString = await _aiService.GetMapDataByCategoryAsync(countryCode, "cultura");
                var cleanData = dataString.Replace("```json", "").Replace("```", "").Trim();
                return Content(cleanData, "application/json");
            }
        }

        [HttpGet("test")]
        [AllowAnonymous]
        public IActionResult Test()
        {
            Console.WriteLine("🔍 AiController/Test endpoint llamado");
            return Ok(new { 
                message = "AiController is working!", 
                timestamp = DateTime.UtcNow,
                controller = "AiController",
                route = "api/[controller]"
            });
        }
    }
}