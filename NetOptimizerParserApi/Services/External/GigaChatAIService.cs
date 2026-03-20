using NetOptimizerParserApi.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NetOptimizerParserApi.Services.External
{
    public class GigaChatAIService : IGigaChatAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _authKey;
        private string? _accessToken;
        private DateTime _tokenExpiry = DateTime.MinValue;

        public GigaChatAIService(IConfiguration configuration)
        {
            _authKey = configuration["GigaChat:AuthorizationKey"]
                       ?? throw new ArgumentNullException("GigaChat AuthorizationKey is missing in config");

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            _httpClient = new HttpClient(handler);
        }

        private async Task EnsureTokenAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry.AddMinutes(-1))
                return;

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://ngw.devices.sberbank.ru:9443/api/v2/oauth");
            request.Headers.Add("Authorization", $"Basic {_authKey}");
            request.Headers.Add("RqUID", Guid.NewGuid().ToString());

            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("scope", "GIGACHAT_API_PERS")
            });

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            _accessToken = doc.RootElement.GetProperty("access_token").GetString();

            long expiresAt = doc.RootElement.GetProperty("expires_at").GetInt64();
            _tokenExpiry = DateTimeOffset.FromUnixTimeMilliseconds(expiresAt).UtcDateTime;
        }

        public async Task<string> AskQuestionAndGetAnswer(string question)
        {
            int maxRetries = 10; 
            int delayMs = 1500; 

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    await EnsureTokenAsync();

                    using var request = new HttpRequestMessage(HttpMethod.Post, "https://gigachat.devices.sberbank.ru/api/v1/chat/completions");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

                    var requestBody = new
                    {
                        model = "GigaChat-2",
                        messages = new[]
                        {
                            new { role = "user", content = question }
                        },
                        stream = false,
                        update_interval = 0
                    };

                    request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                    var response = await _httpClient.SendAsync(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        Console.WriteLine($"[GigaChat] Лимит запросов исчерпан. Ожидание {delayMs}мс... (Попытка {i + 1})");
                        await Task.Delay(delayMs);
                        continue; 
                    }
                    response.EnsureSuccessStatusCode();

                    var resultJson = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(resultJson);

                    var textAnswer = doc.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString()?.Trim();
                    Console.WriteLine($"Ответ от GigaChat: {textAnswer}");
                    return textAnswer;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GigaChat Error] Попытка {i + 1}: {ex.Message}");
                    if (i == maxRetries - 1) return null; 
                    await Task.Delay(delayMs);
                }

            }
            return null;
        }
    }


}
