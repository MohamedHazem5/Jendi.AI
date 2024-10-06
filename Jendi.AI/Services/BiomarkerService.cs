using Jendi.AI.Models.Request;
using Jendi.AI.Models.Response;
using Jendi.AI.Services.IServices;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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
        public async Task<ProfileAnalysisResponse> GetProfileAnalysisAsync(AnalysisRequest request, string bearerToken)
        {
            var requestUri = "https://sandbox-api.sahha.ai/api/v1/profile/analysis";

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonRequest = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUri, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProfileAnalysisResponse>(responseContent);
        }

        public async Task<IEnumerable<Inference>> GetSleepBiomarkersAsync(AnalysisRequest request, string bearerToken)
        {
            var analysisResponse = await GetProfileAnalysisAsync(request, bearerToken);
            return analysisResponse.Inferences
                .Where(inference => inference.Type.Equals("sleep", StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Inference>> GetStressBiomarkersAsync(AnalysisRequest request, string bearerToken)
        {
            var analysisResponse = await GetProfileAnalysisAsync(request, bearerToken);
            return analysisResponse.Inferences
                .Where(inference => inference.Type.Equals("stress", StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Inference>> GetWellbeingBiomarkersAsync(AnalysisRequest request, string bearerToken)
        {
            var analysisResponse = await GetProfileAnalysisAsync(request, bearerToken);
            return analysisResponse.Inferences
                .Where(inference => inference.Type.Equals("wellbeing", StringComparison.OrdinalIgnoreCase));
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
                    Message = "No biomarkers found for the given ExternalId.",
                    Body = response.Content.ReadAsStringAsync().Result
                };
            }

            return Newtonsoft.Json.JsonConvert.DeserializeObject<object>(responseContent);
        }
    }
}
