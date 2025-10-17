using System.Threading.Tasks;
using Api_Orbis_Project.Models;

namespace Api_Orbis_Project.Services
{
    public interface ITravelGuideService
    {
        Task<QuickGuideResponse> GetQuickGuideAsync(string countryCode, string language = "es");
        Task<SafetyGuide> GetSafetyGuideAsync(string countryCode, string language = "es");
        Task<HealthGuide> GetHealthGuideAsync(string countryCode, string language = "es");
        Task<CultureGuide> GetCultureGuideAsync(string countryCode, string language = "es");
    }
}