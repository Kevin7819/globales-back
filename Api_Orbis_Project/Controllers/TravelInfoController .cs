using Api_Orbis_Project.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace Api_Orbis_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TravelInfoController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public TravelInfoController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Get consolidated travel info for a country
        /// Includes: basic info, health indicators, and GeoJSON borders
        /// </summary>
        [HttpGet("{countryCode}")]
        public async Task<IActionResult> GetTravelInfo(string countryCode)
        {
            try
            {
                // 1️⃣ Get basic country info from RestCountries
                var restCountriesUrl = $"https://restcountries.com/v3.1/alpha/{countryCode}";
                var restCountriesResponse = await _httpClient.GetAsync(restCountriesUrl);

                if (!restCountriesResponse.IsSuccessStatusCode)
                    return StatusCode((int)restCountriesResponse.StatusCode, "Error fetching country data");

                var restCountries = await restCountriesResponse.Content.ReadFromJsonAsync<List<CountryResponse>>();
                var countryData = restCountries?.FirstOrDefault();

                // 2️⃣ Get health data from World Bank (gasto en salud per cápita)
                var worldBankUrl = $"https://api.worldbank.org/v2/country/{countryCode}/indicator/SH.XPD.CHEX.PC.CD?format=json";
                var worldBankResponse = await _httpClient.GetAsync(worldBankUrl);

                JsonElement? healthData = null;
                if (worldBankResponse.IsSuccessStatusCode)
                {
                    var worldBankJson = await worldBankResponse.Content.ReadAsStringAsync();
                    healthData = JsonDocument.Parse(worldBankJson).RootElement;
                }

                // 3️⃣ Get GeoJSON borders (ISO Alpha-3 required, so we fallback if not present)
                var isoAlpha3 = countryData?.Cca3 ?? countryCode;
                var geoJsonUrl = $"https://raw.githubusercontent.com/johan/world.geo.json/master/countries/{isoAlpha3}.geo.json";
                var geoJsonResponse = await _httpClient.GetAsync(geoJsonUrl);

                JsonElement? geoJson = null;
                if (geoJsonResponse.IsSuccessStatusCode)
                {
                    var geoJsonContent = await geoJsonResponse.Content.ReadAsStringAsync();
                    geoJson = JsonDocument.Parse(geoJsonContent).RootElement;
                }

                // ✅ Build unified response
                var result = new
                {
                    basicInfo = new
                    {
                        name = countryData?.Name?.Common,
                        region = countryData?.Region,
                        subregion = countryData?.Subregion,
                        population = countryData?.Population,
                        languages = countryData?.Languages?.Values
                    },
                    health = healthData,
                    geoJson = geoJson
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
