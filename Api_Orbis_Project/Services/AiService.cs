using System;
using System.Threading.Tasks;

namespace Api_Orbis_Project.Services
{
    public interface IIAService
    {
        // 🔹 Generates a general AI answer
        Task<string> GetGeneralAnswerAsync(string question, string lang = "es");

        // 🔹 Generates raw GeoJSON data (for an entire country)
        Task<string> GetMapDataRawAsync(string countryCode);

        // 🔹 Generates GeoJSON data filtered by specific category
        Task<string> GetMapDataByCategoryAsync(string countryCode, string category);
    }

    public class IAService : IIAService
    {
        private readonly IHuggingFaceService _huggingFace;

        public IAService(IHuggingFaceService huggingFace)
        {
            _huggingFace = huggingFace;
        }

        /// <summary>
        /// General question-answering using the AI model.
        /// </summary>
        public async Task<string> GetGeneralAnswerAsync(string question, string lang = "es")
        {
            // Uses normal AskAsync for conversational responses
            return await _huggingFace.AskAsync(question, lang);
        }

        /// <summary>
        /// Requests the AI to generate a pure GeoJSON FeatureCollection with simulated data for a given country.
        /// </summary>
        public async Task<string> GetMapDataRawAsync(string countryCode)
        {
            // Calls the GeoJSON-only method to ensure valid JSON output
            return await _huggingFace.GetGeoJsonAsync(countryCode, "general");
        }

        /// <summary>
        /// Requests the AI to generate GeoJSON filtered by category (salud, seguridad, cultura).
        /// </summary>
        public async Task<string> GetMapDataByCategoryAsync(string countryCode, string category)
        {
            string normalizedCategory = category.ToLower();

            // Ensure category is valid
            if (normalizedCategory != "salud" && normalizedCategory != "seguridad" && normalizedCategory != "cultura")
                throw new ArgumentException("Categoría no válida. Debe ser 'salud', 'seguridad' o 'cultura'.");

            // Uses GetGeoJsonAsync to ensure the response is pure JSON
            return await _huggingFace.GetGeoJsonAsync(countryCode, normalizedCategory);
        }
    }
}
