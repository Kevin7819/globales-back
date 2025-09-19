using System.Security.Claims;

namespace Api_Orbis_Project.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetRole(this ClaimsPrincipal user)
        {
            return user?.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}
