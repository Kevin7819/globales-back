using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Api_Orbis_Project.Services
{
    public class ExpoPushService : IExpoPushService
    {
        private readonly HttpClient _http;

        public ExpoPushService(HttpClient http)
        {
            _http = http;
        }

        public async Task SendAsync(IEnumerable<string> tokens, string title, string message, object? data = null)
        {
            var url = "https://exp.host/--/api/v2/push/send";

            var notifications = new List<object>();
            foreach (var token in tokens)
            {
                notifications.Add(new
                {
                    to = token,
                    sound = "default",
                    title,
                    body = message,
                    data
                });
            }

            var json = JsonSerializer.Serialize(notifications);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.PostAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"📤 Notificación enviada a {notifications.Count} tokens.");
            Console.WriteLine($"📦 Payload: {json}");
            Console.WriteLine($"📥 Respuesta: {result}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"⚠️ Error enviando notificación: {response.StatusCode}");
            }
        }
    }
}
    