using Jendi.AI.Models.Request;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Jendi.AI.Services
{
    public class ProfileService 
    {
        private readonly HttpClient _httpClient;

        public ProfileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetApiVersionAsync()
        {
            // Set the request URL
            var requestUrl = "https://sandbox-api.sahha.ai/Version";

            // Make the GET request to the Sahha API
            var response = await _httpClient.GetAsync(requestUrl);

            // Ensure we received a successful response
            response.EnsureSuccessStatusCode();

            // Read the response content as a string
            var version = await response.Content.ReadAsStringAsync();

            return version;
        }

        public async Task<RegisterTokenDto> RegisterProfileAsync(string externalId, string token)
        {
            var requestBody = new
            {
                externalId = externalId
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://sandbox-api.sahha.ai/api/v1/oauth/profile/register");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<RegisterTokenDto>(responseBody);

                return tokenResponse; 
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error: {errorResponse}");
            }

        }

    }
}
