using System;
using System.Threading.Tasks;

namespace Api_Orbis_Project.Services
{
    public interface IIAService
    {
        Task<string> GetGeneralAnswerAsync(string question, string lang = "es");

        Task<string> GetMapDataRawAsync(string countryCode);

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
