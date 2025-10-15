using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;

namespace Api_Orbis_Project.Services
{
    // Interface for the AI client
    public interface IHuggingFaceService
    {
        // Conversational / chat-like responses (text)
        Task<string> AskAsync(string prompt, string language = "es");

        // Returns a pure GeoJSON string (FeatureCollection) for the given country/category
        Task<string> GetGeoJsonAsync(string countryCode, string category = "general");
    }

    public class HuggingFaceService : IHuggingFaceService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;

        public HuggingFaceService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["HuggingFace:ApiKey"] 
                ?? throw new Exception("❌ HuggingFace ApiKey not found in configuration.");
            _model = config["HuggingFace:Model"] ?? "Kwaipilot/KAT-Dev";
        }

        /// <summary>
        /// Sends a chat-like request to the Hugging Face API using the chosen model.
        /// </summary>
        public async Task<string> AskAsync(string prompt, string language = "es")
        {
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://router.huggingface.co/v1/chat/completions"
            );

            // Attach API Key
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            // JSON body for the AI request
            var payload = new
            {
                model = _model,
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = $"Eres un asistente especializado en análisis de datos geográficos. Responde siempre en {language}."
                    },
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                },
                max_tokens = 2048 
            };

            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            // Send request
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error calling Hugging Face API: {response.StatusCode} - {content}");

            try
            {
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                var text = root.GetProperty("choices")[0]
                               .GetProperty("message")
                               .GetProperty("content")
                               .GetString();

                return text ?? "No response generated.";
            }
            catch
            {
                return content; // fallback in case parsing fails
            }
        }

        

        // 🧭 Nuevo método especializado para obtener GeoJSON puro
        public async Task<string> GetGeoJsonAsync(string countryCode, string category = "general")
        {
            var prompt = $@"
            Devuélveme únicamente un objeto JSON válido en formato GeoJSON de tipo 'FeatureCollection'
            sobre la categoría '{category}' para el país '{countryCode}'.
            No escribas explicaciones, ni ```json, ni ``` — solo devuelve el JSON puro y válido.
            ";

                    return await AskAsync(prompt, "es");
                }
        }
}
