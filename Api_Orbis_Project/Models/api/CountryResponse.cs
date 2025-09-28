using System;
using System.ComponentModel.DataAnnotations;

namespace Api_Orbis_Project.Models
{
        public class CountryResponse
    {
        public NameResponse? Name { get; set; }
        public Dictionary<string, string>? Languages { get; set; }
    }
}
