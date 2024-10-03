using Jendi.AI.Models.Dtos;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Jendi.AI.Services
{
    public class SahhaIntegrationService
    {
        private readonly HttpClient _httpClient;

        public SahhaIntegrationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<IntegrationResponseDto>> GetProfileIntegrationsAsync(string profileToken)
        {
            // Clear any existing headers and set the profile token in the Authorization header.
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"profile {profileToken}");

            // Call the external API to get the integrations (replace the URL with Sahha's endpoint).
            var response = await _httpClient.GetAsync("https://sandbox-api.sahha.ai/api/v1/profile/integration");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error fetching integrations: {response.ReasonPhrase}");
            }

            // Deserialize the response content to the DTO.
            var responseData = await response.Content.ReadFromJsonAsync<IEnumerable<IntegrationResponseDto>>();
            return responseData;
        }


    }

}
