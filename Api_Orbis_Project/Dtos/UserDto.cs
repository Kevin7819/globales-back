using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api_Orbis_Project.Dtos
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string Password { get; set; } = null!;
        public string? Email { get; set; } // optionally remove for privacy
        public string? CountryOfOrigin { get; set; }
        public string? PreferredLanguage { get; set; }
        public string? Role { get; set; } // for response, easier as string
    }
}
