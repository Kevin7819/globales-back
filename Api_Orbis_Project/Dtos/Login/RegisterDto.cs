using System;
using System.ComponentModel.DataAnnotations;

namespace Api_Orbis_Project.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string UserName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;

        public string? CountryOfOrigin { get; set; }
        public string? PreferredLanguage { get; set; }

        [Required]
        public DateOnly BirthDate { get; set; }
    }
}
