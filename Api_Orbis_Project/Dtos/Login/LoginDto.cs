// Api_Orbis_Project/Dtos/LoginDto.cs
using System.ComponentModel.DataAnnotations;

namespace Api_Orbis_Project.Dtos
{
    public class LoginDto
    {
        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
