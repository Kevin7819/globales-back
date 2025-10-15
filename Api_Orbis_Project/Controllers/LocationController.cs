using Api_Orbis_Project.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace Api_Orbis_Project.Controllers
{   
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public LocationController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Get a list of countries from RestCountries API
        /// </summary>
        [HttpGet("countries")]
        public async Task<IActionResult> GetCountries()
        {
            var response = await _httpClient.GetAsync("https://restcountries.com/v3.1/all?fields=name");
            response.EnsureSuccessStatusCode();

            var countries = await response.Content.ReadFromJsonAsync<List<CountryResponse>>();

            if (countries == null)
                return StatusCode(500, "Error fetching countries");

            var countryNames = countries
                .Where(c => c.Name != null && c.Name.Common != null)
                .Select(c => c.Name.Common)
                .OrderBy(n => n)
                .ToList();

            return Ok(countryNames);
        }

        /// <summary>
        /// Get a list of languages from RestCountries API
        /// </summary>
        [HttpGet("languages")]
        public async Task<IActionResult> GetLanguages()
        {
            var response = await _httpClient.GetAsync("https://restcountries.com/v3.1/all?fields=languages");
            response.EnsureSuccessStatusCode();

            var countries = await response.Content.ReadFromJsonAsync<List<CountryResponse>>();

            if (countries == null)
                return StatusCode(500, "Error fetching countries");

            var languages = new HashSet<string>();
            foreach (var c in countries)
            {
                if (c.Languages != null)
                {
                    foreach (var lang in c.Languages.Values)
                        languages.Add(lang);
                }
            }

            return Ok(languages.OrderBy(l => l));
        }

        /// <summary>
        /// Get health data for a country using World Bank API
        /// Example indicator: SH.XPD.CHEX.PC.CD (Current health expenditure per capita)
        /// </summary>
        [HttpGet("health/{countryCode}")]
        public async Task<IActionResult> GetHealthData(string countryCode)
        {
            // Example World Bank API endpoint
            var url = $"https://api.worldbank.org/v2/country/{countryCode}/indicator/SH.XPD.CHEX.PC.CD?format=json";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Error fetching health data");

            var content = await response.Content.ReadAsStringAsync();
            return Ok(JsonDocument.Parse(content));
        }

        /// <summary>
        /// Get GeoJSON borders for a country
        /// Uses a free GitHub dataset of world borders
        /// </summary>
        [HttpGet("geojson/{countryCode}")]
        public async Task<IActionResult> GetCountryGeoJson(string countryCode)
        {
            try
            {
                Console.WriteLine($"Solicitando GeoJSON para: {countryCode}");
                
                var url = $"https://raw.githubusercontent.com/johan/world.geo.json/master/countries/{countryCode}.geo.json";
                Console.WriteLine($"URL: {url}");
                
                var response = await _httpClient.GetAsync(url);
                Console.WriteLine($"Response status: {response.StatusCode}");
                
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                    return StatusCode((int)response.StatusCode, new { error = $"HTTP {response.StatusCode}" });
                }

                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Content length: {content.Length}");
                
                return Content(content, "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
