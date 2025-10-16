using System.Text.Json.Serialization;

namespace Api_Orbis_Project.Models
{
    public class CountryResponse
    {
        [JsonPropertyName("name")]
        public NameResponse? Name { get; set; }

        [JsonPropertyName("cca3")]
        public string? Cca3 { get; set; }           

        [JsonPropertyName("cca2")]
        public string? Cca2 { get; set; }

        [JsonPropertyName("latlng")]
        public List<double>? Latlng { get; set; } 

        [JsonPropertyName("region")]
        public string? Region { get; set; }

        [JsonPropertyName("subregion")]
        public string? Subregion { get; set; }

        [JsonPropertyName("population")]
        public long? Population { get; set; }

        [JsonPropertyName("languages")]
        public Dictionary<string, string>? Languages { get; set; }
        
        [JsonPropertyName("capital")]
        public List<string>? Capital { get; set; }

        [JsonPropertyName("currencies")]
        public Dictionary<string, Currency>? Currencies { get; set; }

        [JsonPropertyName("translations")]
        public Dictionary<string, Translation>? Translations { get; set; }
    }

    public class NameResponse
    {
        [JsonPropertyName("common")]
        public string? Common { get; set; }

        [JsonPropertyName("official")]
        public string? Official { get; set; }

        [JsonPropertyName("nativeName")]
        public Dictionary<string, NativeName>? NativeName { get; set; }
    }

    public class NativeName
    {
        [JsonPropertyName("official")]
        public string? Official { get; set; }

        [JsonPropertyName("common")]
        public string? Common { get; set; }
    }

    public class Currency
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }
    }

    public class Translation
    {
        [JsonPropertyName("official")]
        public string? Official { get; set; }

        [JsonPropertyName("common")]
        public string? Common { get; set; }
    }
}