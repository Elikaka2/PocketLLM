using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PocketLLM.Services
{
    public class LLMService
    {
        private readonly Config.ConfigClass config;
        private readonly HttpClient _httpClient;

        public LLMService(Config.ConfigClass config)
        {
            this.config = config;
            _httpClient = new HttpClient();
        }

        public async Task<string> SendMessage(string message)
        {

            if (string.IsNullOrEmpty(config.ApiKey) || string.IsNullOrEmpty(config.Model) || string.IsNullOrEmpty(config.LLMType))
            {
                throw new InvalidOperationException("API Key, Model, and LLM Type must be set in the configuration.");
            }

            try
            {
                var requestBody = new
                {
                    model = config.Model ?? "gpt-4o-mini",
                    messages = new[]
                    {
                        new { role = "user", content = message }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {config.ApiKey}");

                var response = await _httpClient.PostAsync($"https://api.openai.com/v1/chat/completions", content);

                if (!response.IsSuccessStatusCode)
                {
                    return $"Api error: {response.StatusCode}";
                }
                
                var responseContent = await response.Content.ReadAsStringAsync();
                
                using var doc = JsonDocument.Parse(responseContent);
                return doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message").
                    GetProperty("content").
                    GetString();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error sending message to LLM", ex);
            }
        }
    }
}
