using Api_Orbis_Project.Dtos;
using Api_Orbis_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;
using System.Text.Json;

namespace Api_Orbis_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TravelInfoController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TravelInfoController> _logger;
        private readonly IMemoryCache _cache;

        public TravelInfoController(HttpClient httpClient, ILogger<TravelInfoController> logger, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _cache = cache;
        }

        /// <summary>
        /// Get consolidated travel info for a country.
        /// Includes: basic info (RestCountries), health data (WorldBank), and GeoJSON borders.
        /// This endpoint is ready to be consumed by the chatbot and Mapbox frontend.
        /// </summary>
        /// <param name="countryCode">ISO Alpha-2 or Alpha-3 country code</param>
        /// <returns>Unified travel info object</returns>
        [HttpGet("{countryCode}")]
        public async Task<IActionResult> GetTravelInfo(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                return BadRequest(new { error = "Country code is required" });

            // Revisar cache
            if (_cache.TryGetValue(countryCode.ToUpper(), out TravelInfoResponse cached))
            {
                _logger.LogInformation("Cache hit for {countryCode}", countryCode);
                return Ok(cached);
            }

            try
            {
                // =====================
                // 1. Basic country info
                // =====================
                var restCountriesUrl = $"https://restcountries.com/v3.1/alpha/{countryCode}";
                var restCountriesResponse = await _httpClient.GetAsync(restCountriesUrl);

                if (!restCountriesResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Error fetching RestCountries data for {countryCode}", countryCode);
                    return StatusCode((int)restCountriesResponse.StatusCode, new { error = "Error fetching country data" });
                }

                var restCountries = await restCountriesResponse.Content.ReadFromJsonAsync<List<CountryResponse>>();
                var countryData = restCountries?.FirstOrDefault();

                if (countryData == null)
                    return NotFound(new { error = $"Country data not found for code {countryCode}" });

                // =====================
                // 2. Health data (World Bank)
                // =====================
                var worldBankUrl = $"https://api.worldbank.org/v2/country/{countryCode}/indicator/SH.XPD.CHEX.PC.CD?format=json";
                var worldBankResponse = await _httpClient.GetAsync(worldBankUrl);

                JsonElement? healthData = null;
                if (worldBankResponse.IsSuccessStatusCode)
                {
                    var worldBankJson = await worldBankResponse.Content.ReadAsStringAsync();
                    try
                    {
                        healthData = JsonDocument.Parse(worldBankJson).RootElement;
                    }
                    catch (JsonException jsonEx)
                    {
                        _logger.LogError(jsonEx, "Error parsing WorldBank JSON for {countryCode}", countryCode);
                    }
                }

                // =====================
                // 3. GeoJSON Borders
                // =====================
                var isoAlpha3 = countryData?.Cca3 ?? countryCode;
                var geoJsonUrl = $"https://raw.githubusercontent.com/johan/world.geo.json/master/countries/{isoAlpha3}.geo.json";
                var geoJsonResponse = await _httpClient.GetAsync(geoJsonUrl);

                JsonElement? geoJson = null;
                if (geoJsonResponse.IsSuccessStatusCode)
                {
                    var geoJsonContent = await geoJsonResponse.Content.ReadAsStringAsync();
                    try
                    {
                        geoJson = JsonDocument.Parse(geoJsonContent).RootElement;
                    }
                    catch (JsonException jsonEx)
                    {
                        _logger.LogError(jsonEx, "Error parsing GeoJSON for {countryCode}", countryCode);
                    }
                }

                // =====================
                // Build DTO Response
                // =====================
                var response = new TravelInfoResponse
                {
                    BasicInfo = new BasicInfoDto
                    {
                        Code = countryCode.ToUpper(),
                        Name = countryData?.Name?.Common,
                        OfficialName = countryData?.Name?.Official,
                        Region = countryData?.Region,
                        Subregion = countryData?.Subregion,
                        Population = countryData?.Population,
                        Languages = countryData?.Languages?.Values,
                        Capital = countryData?.Capital
                    },
                    Health = healthData,
                    GeoJson = geoJson
                };

                // Guardar en cache 24h
                _cache.Set(countryCode.ToUpper(), response, TimeSpan.FromHours(24));

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching travel info for {countryCode}", countryCode);
                return StatusCode(500, new
                {
                    error = "Internal Server Error",
                    details = ex.Message
                });
            }
        }
    }
}
