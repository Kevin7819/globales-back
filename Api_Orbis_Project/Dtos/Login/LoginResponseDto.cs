// Api_Orbis_Project/Dtos/LoginResponseDto.cs
namespace Api_Orbis_Project.Dtos
{
    public class LoginResponseDto
    {
        public int id { get; set; }
        public string userName { get; set; } = null!;
        public string email { get; set; } = null!;
        public string token { get; set; } = null!;
        public string? role { get; set; }
    }
}
