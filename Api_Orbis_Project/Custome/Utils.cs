using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Api_Orbis_Project.Models;

namespace api.Custome
{
    public class Utils
    {
        private readonly IConfiguration _configuration;

        public Utils(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string EncryptSHA256(string text)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(text));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        public string GenerateJWT(User model)
        {
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, model.UserId.ToString()),
                new Claim(ClaimTypes.Name, model.Name ?? string.Empty),
                new Claim(ClaimTypes.Email, model.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, model.UserRole.ToString())
            };

            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("JWT key is not configured.");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var expireMinutes = int.TryParse(_configuration["Jwt:ExpireMinutes"], out int minutes) ? minutes : 60;

            var jwtConfig = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtConfig);
        }
    }
}
