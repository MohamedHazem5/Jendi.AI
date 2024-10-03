using Jendi.AI.Controllers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace Jendi.AI.Services
{
    public class SahhaAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly SahhaKeys _sahhaKeys;

        public SahhaAuthService(HttpClient httpClient, IOptions<SahhaKeys> sahhaKeys)
        {
            _httpClient = httpClient;
            _sahhaKeys = sahhaKeys.Value;
        }

        public async Task<AccountTokenResponseDto> GetAccountTokenAsync()
        {
            // Prepare the request payload
            var payload = new
            {
                clientId = _sahhaKeys.ClientId,
                clientSecret = _sahhaKeys.ClientSecret
            };

            // Convert payload to JSON
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            // Send the POST request
            HttpResponseMessage response = await _httpClient.PostAsync("https://sandbox-api.sahha.ai/api/v1/oauth/account/token", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<AccountTokenResponseDto>(jsonResponse);

                // Return the token and token type
                return tokenResponse;
            }
            else
            {
                throw new HttpRequestException($"Error: {response.StatusCode}");
            }
        }

    }

    public class AccountTokenResponseDto
    {
        [JsonProperty("accountToken")]
        public string AccountToken { get; set; }

        [JsonProperty("tokenType")]
        public string TokenType { get; set; }
    }

    public class SahhaKeys
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
