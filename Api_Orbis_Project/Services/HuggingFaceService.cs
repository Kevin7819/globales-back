using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

public class HuggingFaceService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public HuggingFaceService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["HuggingFace:ApiKey"] ?? throw new Exception("HuggingFace ApiKey not found");
        _model = config["HuggingFace:Model"] ?? "Kwaipilot/KAT-Dev";
    }

    public async Task<string> AskAsync(string prompt, string language = "es")
    {
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://router.huggingface.co/v1/chat/completions"
        );

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        var payload = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "system", content = $"Eres un asistente experto en viajes internacionales. Responde siempre en {language}. Brinda consejos culturales, sanitarios y logísticos para el país de destino." },
                new { role = "user", content = prompt }
            },
            max_tokens = 300
        };

        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

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
            return content;
        }
    }
}
