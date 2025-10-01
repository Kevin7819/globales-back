using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Api_Orbis_Project.Models
{
    public class CountryResponse
    {
        [JsonPropertyName("name")]
        public NameResponse? Name { get; set; }

        [JsonPropertyName("cca3")]
        public string? Cca3 { get; set; }           

        [JsonPropertyName("region")]
        public string? Region { get; set; }

        [JsonPropertyName("subregion")]
        public string? Subregion { get; set; }

        [JsonPropertyName("population")]
        public long? Population { get; set; }

        [JsonPropertyName("languages")]
        public Dictionary<string, string>? Languages { get; set; }
    }
}
