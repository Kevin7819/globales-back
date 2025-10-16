using System.Text.Json;
using Api_Orbis_Project.Models;

public interface IGeocodingService
{
    Task<string> GetCountryCodeFromDestination(string destination);
    Task<(double lat, double lng, string countryCode)> GetCoordinatesAndCountry(string destination);
    Task<CountryResponse> GetFullCountryInfo(string countryName);
}

public class GeocodingService : IGeocodingService
{
    private readonly HttpClient _httpClient;
    private const string NominatimUrl = "https://nominatim.openstreetmap.org/search";

    public GeocodingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "OrbisTravelApp/1.0");
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<string> GetCountryCodeFromDestination(string destination)
    {
        var result = await GetCoordinatesAndCountry(destination);
        return result.countryCode;
    }

    public async Task<(double lat, double lng, string countryCode)> GetCoordinatesAndCountry(string destination)
    {
        var restCountriesResult = await GetCountryInfoFromRestCountries(destination);
        if (restCountriesResult.countryCode != "UNK")
        {
            return restCountriesResult;
        }

        return await GetCoordinatesFromNominatim(destination);
    }

    private async Task<(double lat, double lng, string countryCode)> GetCountryInfoFromRestCountries(string countryName)
    {
        try
        {
            Console.WriteLine($"Buscando en RestCountries: {countryName}");
            
            var response = await _httpClient.GetAsync($"https://restcountries.com/v3.1/name/{Uri.EscapeDataString(countryName)}?fullText=true");
            
            if (!response.IsSuccessStatusCode)
            {
                response = await _httpClient.GetAsync($"https://restcountries.com/v3.1/name/{Uri.EscapeDataString(countryName)}");
            }

            if (response.IsSuccessStatusCode)
            {
                var countries = await response.Content.ReadFromJsonAsync<List<CountryResponse>>();
                if (countries?.Count > 0)
                {
                    var country = countries[0];
                    var latlng = country.Latlng ?? new List<double> { 0, 0 };
                    var cca2 = country.Cca2?.ToUpper() ?? "UNK";
                    
                    Console.WriteLine($"RestCountries encontrado: {country.Name?.Common} -> {cca2} ({latlng[0]}, {latlng[1]})");
                    
                    return (latlng.Count > 0 ? latlng[0] : 0, 
                            latlng.Count > 1 ? latlng[1] : 0, 
                            cca2);
                }
            }
            
            Console.WriteLine($"RestCountries no encontró: {countryName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"RestCountries error: {ex.Message}");
        }

        return (0, 0, "UNK");
    }

    private async Task<(double lat, double lng, string countryCode)> GetCoordinatesFromNominatim(string destination)
    {
        try
        {
            Console.WriteLine($"Fallback a Nominatim: {destination}");
            
            var url = $"{NominatimUrl}?q={Uri.EscapeDataString(destination)}&format=json&addressdetails=1&limit=1&accept-language=es";
            
            var response = await _httpClient.GetStringAsync(url);
            var results = JsonSerializer.Deserialize<JsonElement[]>(response);

            if (results?.Length > 0)
            {
                var result = results[0];
                var lat = result.GetProperty("lat").GetDouble();
                var lng = result.GetProperty("lon").GetDouble();
                
                string countryCode = "UNK";
                if (result.TryGetProperty("address", out JsonElement address))
                {
                    if (address.TryGetProperty("country_code", out JsonElement countryCodeElement))
                    {
                        countryCode = countryCodeElement.GetString()?.ToUpper() ?? "UNK";
                    }
                }

                Console.WriteLine($"Nominatim encontrado: {destination} -> {countryCode} ({lat}, {lng})");
                return (lat, lng, countryCode);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Nominatim error: {ex.Message}");
        }

        return (0, 0, "UNK");
    }

    // Método adicional para obtener información completa del país
    public async Task<CountryResponse> GetFullCountryInfo(string countryName)
    {
        try
        {
            var response = await _httpClient.GetAsync($"https://restcountries.com/v3.1/name/{Uri.EscapeDataString(countryName)}");
            if (response.IsSuccessStatusCode)
            {
                var countries = await response.Content.ReadFromJsonAsync<List<CountryResponse>>();
                return countries?.FirstOrDefault();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting full country info: {ex.Message}");
        }

        return null;
    }
}