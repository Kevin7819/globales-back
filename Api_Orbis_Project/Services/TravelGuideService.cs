using System;
using System.Threading.Tasks;
using Api_Orbis_Project.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Api_Orbis_Project.Services
{

    public class TravelGuideService : ITravelGuideService
    {
        private readonly IHuggingFaceService _aiService;
        private readonly IMemoryCache _cache;

        public TravelGuideService(IHuggingFaceService aiService, IMemoryCache cache)
        {
            _aiService = aiService;
            _cache = cache;
        }

        public async Task<QuickGuideResponse> GetQuickGuideAsync(string countryCode, string language = "es")
        {
            string cacheKey = $"QuickGuide_{countryCode}_{language}";
            
            if (_cache.TryGetValue(cacheKey, out QuickGuideResponse cached))
                return cached;

            var response = await _aiService.GenerateQuickGuideAsync(countryCode);
            
            if (response != null)
            {
                _cache.Set(cacheKey, response, TimeSpan.FromHours(12));
            }
            
            return response;
        }

        public async Task<SafetyGuide> GetSafetyGuideAsync(string countryCode, string language = "es")
        {
            string cacheKey = $"SafetyGuide_{countryCode}_{language}";
            
            if (_cache.TryGetValue(cacheKey, out SafetyGuide cached))
                return cached;

            var response = await _aiService.GenerateSafetyGuideAsync(countryCode);
            
            if (response != null)
            {
                _cache.Set(cacheKey, response, TimeSpan.FromHours(12));
            }
            
            return response;
        }

        public async Task<HealthGuide> GetHealthGuideAsync(string countryCode, string language = "es")
        {
            string cacheKey = $"HealthGuide_{countryCode}_{language}";
            
            if (_cache.TryGetValue(cacheKey, out HealthGuide cached))
                return cached;

            var response = await _aiService.GenerateHealthGuideAsync(countryCode);
            
            if (response != null)
            {
                _cache.Set(cacheKey, response, TimeSpan.FromHours(12));
            }
            
            return response;
        }

        public async Task<CultureGuide> GetCultureGuideAsync(string countryCode, string language = "es")
        {
            string cacheKey = $"CultureGuide_{countryCode}_{language}";
            
            if (_cache.TryGetValue(cacheKey, out CultureGuide cached))
                return cached;

            var response = await _aiService.GenerateCultureGuideAsync(countryCode);
            
            if (response != null)
            {
                _cache.Set(cacheKey, response, TimeSpan.FromHours(12));
            }
            
            return response;
        }
    }
}