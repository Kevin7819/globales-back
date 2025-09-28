using Api_Orbis_Project.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;

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
            // Call RestCountries API
            var response = await _httpClient.GetAsync("https://restcountries.com/v3.1/all?fields=name");
            response.EnsureSuccessStatusCode();

            var countries = await response.Content.ReadFromJsonAsync<List<CountryResponse>>();

            if (countries == null)
                return StatusCode(500, "Error fetching countries");
                
            // Extract "Common" names, sort alphabetically
            var countryNames = countries
                .Where(c => c.Name != null && c.Name.Common != null)
                .Select(c => c.Name.Common)
                .OrderBy(n => n)
                .ToList();

            return Ok(countryNames);
        }

        [HttpGet("languages")]
        public async Task<IActionResult> GetLanguages()
        {   
            // Call RestCountries API
            var response = await _httpClient.GetAsync("https://restcountries.com/v3.1/all?fields=languages");
            response.EnsureSuccessStatusCode();

            var countries = await response.Content.ReadFromJsonAsync<List<CountryResponse>>();

            if (countries == null)
                return StatusCode(500, "Error fetching countries");

            // Use HashSet to avoid duplicate languages
            var languages = new HashSet<string>();
            foreach (var c in countries)
            {
                if (c.Languages != null)
                    foreach (var lang in c.Languages.Values)
                        languages.Add(lang);
            }

            return Ok(languages.OrderBy(l => l));
        }
    }
}
