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
                HealthDto? healthInfo = null;
                var worldBankUrl = $"https://api.worldbank.org/v2/country/{countryCode}/indicator/SH.XPD.CHEX.PC.CD?format=json";
                var worldBankResponse = await _httpClient.GetAsync(worldBankUrl);

                if (worldBankResponse.IsSuccessStatusCode)
                {
                    var worldBankJson = await worldBankResponse.Content.ReadFromJsonAsync<JsonElement[]>();

                    if (worldBankJson?.Length > 1 && worldBankJson[1].ValueKind == JsonValueKind.Array)
                    {
                        var points = new List<HealthDataPoint>();

                        foreach (var item in worldBankJson[1].EnumerateArray())
                        {
                            var year = item.GetProperty("date").GetString();
                            var value = item.GetProperty("value");

                            if (int.TryParse(year, out int parsedYear))
                            {
                                points.Add(new HealthDataPoint
                                {
                                    Year = parsedYear,
                                    Value = value.ValueKind == JsonValueKind.Null ? null : value.GetDecimal()
                                });
                            }
                        }

                        healthInfo = new HealthDto
                        {
                            CountryCode = countryCode.ToUpper(),
                            Data = points.OrderByDescending(p => p.Year).ToList()
                        };
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
                        Name = countryData?.Name?.Common ?? "Unknown",
                        OfficialName = countryData?.Name?.Official ?? "Unknown",
                        Region = countryData?.Region ?? "Unknown",
                        Subregion = countryData?.Subregion ?? "Unknown",
                        Population = countryData?.Population ?? 0,
                        Languages = countryData?.Languages?.Values.ToList() ?? new List<string>(),
                        Capital = countryData?.Capital?.ToList() ?? new List<string>()
                    },
                    Health = healthInfo,
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
