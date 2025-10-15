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
        public async Task<string> GetMapDataRawAsync(string countryCode)
        {
            return await _huggingFace.GetGeoJsonAsync(countryCode, "general");
        }

        public async Task<string> GetMapDataByCategoryAsync(string countryCode, string category)
        {
            string normalizedCategory = category.ToLower();

            if (normalizedCategory != "salud" && normalizedCategory != "seguridad" && normalizedCategory != "cultura")
                throw new ArgumentException("Categoría no válida.");

            var raw = await _huggingFace.GetGeoJsonAsync(countryCode, normalizedCategory);
            
            // Limpieza robusta
            var clean = raw
                .Replace("```json", "")
                .Replace("```", "")
                .Replace("]>**Error**:", "")
                .Replace("**Error**:", "")
                .Replace("Intentaré corregirlo.", "")
                .Trim();

            // Extraer solo el JSON entre el primer { y el último }
            var start = clean.IndexOf('{');
            var end = clean.LastIndexOf('}');
            
            if (start >= 0 && end > start)
            {
                clean = clean.Substring(start, end - start + 1);
            }

            return clean;
        }
    }
}
