using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Api_Orbis_Project.Services
{
    public interface IExpoPushService
    {
        Task SendAsync(IEnumerable<string> tokens, string title, string body, Dictionary<string, object>? data = null);
    }

    public class ExpoPushService : IExpoPushService
    {
        private readonly HttpClient _http;

        public ExpoPushService(HttpClient http)
        {
            _http = http;
            _http.BaseAddress = new Uri("https://exp.host/--/api/v2/push/");
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task SendAsync(IEnumerable<string> tokens, string title, string body, Dictionary<string, object>? data = null)
        {
            var messages = tokens.Select(t => new
            {
                to = t,
                sound = "default",
                title = title,
                body = body,
                data = data
            });

            var json = JsonSerializer.Serialize(messages);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await _http.PostAsync("send", content);
            var txt = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                throw new Exception($"Expo push error {res.StatusCode}: {txt}");
        }
    }
}