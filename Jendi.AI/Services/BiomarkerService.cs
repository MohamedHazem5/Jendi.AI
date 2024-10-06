using Jendi.AI.Services.IServices;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Web;

namespace Jendi.AI.Services
{
    public class BiomarkerService : IBiomarkerService
    {
        private readonly HttpClient _httpClient;

        public BiomarkerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<object> GetHeartRateAsync(string bearerToken)
        {
            return await GetBiomarkerDataAsync(bearerToken, new[] { "heart_rate" }, new[] { "activity" });
        }

        public async Task<object> GetCaloriesBurntAsync(string bearerToken)
        {
            return await GetBiomarkerDataAsync(bearerToken, new[] { "active_energy_burned" }, new[] { "activity" });
        }

        public async Task<object> GetSleepDataAsync(string bearerToken)
        {
            var types = new[]
            {
            "sleep_start_time", "sleep_midpoint_time", "sleep_end_time", "in_bed_duration"
        };
            return await GetBiomarkerDataAsync(bearerToken, types, new[] { "sleep" });
        }

        private async Task<object> GetBiomarkerDataAsync(string bearerToken, string[] types, string[] categories)
        {
            if (string.IsNullOrEmpty(bearerToken))
            {
                throw new UnauthorizedAccessException("Bearer token is required");
            }

            try
            {
                var requestUri = BuildBiomarkerRequestUri(types, categories);
                ConfigureHttpClientHeaders(bearerToken);
                var response = await SendBiomarkerRequestAsync(requestUri);

                return ParseBiomarkerResponse(response);
            }
            catch (Exception ex)
            {
                throw new Exception($"Internal server error: {ex.Message}", ex);
            }
        }

        private string BuildBiomarkerRequestUri(string[] types, string[] categories)
        {
            var startDateTime = DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var endDateTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            var uriBuilder = new UriBuilder("https://sandbox-api.sahha.ai/api/v1/profile/biomarker");
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["categories"] = $"[{string.Join(",", categories)}]";
            query["types"] = $"[{string.Join(",", types)}]";
            query["startDateTime"] = startDateTime;
            query["endDateTime"] = endDateTime;
            uriBuilder.Query = query.ToString();

            return uriBuilder.ToString();
        }


        private void ConfigureHttpClientHeaders(string bearerToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async Task<HttpResponseMessage> SendBiomarkerRequestAsync(string requestUri)
        {
            return await _httpClient.GetAsync(requestUri);
        }

        private object ParseBiomarkerResponse(HttpResponseMessage response)
        {
            var responseContent = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Request failed with status code {(int)response.StatusCode}: {responseContent}");
            }

            if (string.IsNullOrWhiteSpace(responseContent) || responseContent == "[]")
            {
                return new
                {
                    Message = "No biomarkers found for the given parameters.",
                    Body = response.Content.ReadAsStringAsync().Result
                };
            }

            return JsonConvert.DeserializeObject<object>(responseContent);
        }
    }
}
