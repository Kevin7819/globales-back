using System.Text.Json;
using Api_Orbis_Project.Models;

namespace Api_Orbis_Project.Dtos
{
    public class TravelInfoResponse
    {
        public BasicInfoDto BasicInfo { get; set; }
        public HealthDto? Health { get; set; }
        public JsonElement? GeoJson { get; set; }
    }

    public class HealthDto
    {
        public string Indicator { get; set; } = "Current Health Expenditure per Capita (USD)";
        public string CountryCode { get; set; }
        public List<HealthDataPoint> Data { get; set; } = new();
    }

    public class HealthDataPoint
    {
        public int Year { get; set; }
        public decimal? Value { get; set; }
    }
}
