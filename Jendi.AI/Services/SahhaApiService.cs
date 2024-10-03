using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Jendi.AI.Services
{
    public class SahhaApiService
    {
        private readonly HttpClient _httpClient;

        public SahhaApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WellbeingScoresResponse?> GetWellbeingScoresAsync(string token, string startDateTime, string endDateTime, string types = "wellbeing", int version = 1)
        {
            string url = $"https://api.sahha.ai/v1/profile/score?types={types}&startDateTime={startDateTime}&endDateTime={endDateTime}&version={version}";

            // Set Authorization header
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Make the GET request
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var wellbeingScores = JsonConvert.DeserializeObject<WellbeingScoresResponse>(jsonResponse);
                return wellbeingScores;
            }
            else
            {
                // Handle different status codes
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new Exception("Invalid profile ID.");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Invalid token.");
                }
                return null;
            }
        }
    }

    // DTO for deserializing the API response
    public class WellbeingScoresResponse
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string State { get; set; }
        public int Score { get; set; }
        public string[] Factors { get; set; }
        public string[] DataSources { get; set; }
        public DateTime ScoreDateTime { get; set; }
    }
}
