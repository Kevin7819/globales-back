using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Api_Orbis_Project.Models
{
    public class NameResponse
    {
        [JsonPropertyName("common")]
        public string? Common { get; set; }

        [JsonPropertyName("official")]
        public string? Official { get; set; }
    }
}
