using System.ComponentModel.DataAnnotations;

namespace Api_Orbis_Project.Dtos
{
    public class UpdateUserDto
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        public string? CountryOfOrigin { get; set; }
        public string? PreferredLanguage { get; set; }

        public DateOnly? BirthDate { get; set; }

        public string Role { get; set; } = "Passenger";
    }
}