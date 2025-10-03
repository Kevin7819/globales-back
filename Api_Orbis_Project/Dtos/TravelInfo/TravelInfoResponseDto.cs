using System.Text.Json;
using Api_Orbis_Project.Models;

namespace Api_Orbis_Project.Dtos
{
    public class TravelInfoResponse
    {
        public BasicInfoDto BasicInfo { get; set; }
        public JsonElement? Health { get; set; }
        public JsonElement? GeoJson { get; set; }
    }
}
