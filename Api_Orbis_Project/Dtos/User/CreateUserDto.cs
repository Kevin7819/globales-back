using Api_Orbis_Project.Models;

namespace Api_Orbis_Project.Dtos
{
    public class CreateUserDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? CountryOfOrigin { get; set; }
        public string? PreferredLanguage { get; set; }
        public DateOnly BirthDate { get; set; }
        public User.Role UserRole { get; set; } // uses enum nested in User model
    }
}
