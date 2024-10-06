using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Web;

namespace Jendi.AI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BiomarkersController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public BiomarkersController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("get-biomarkers")]
        public async Task<IActionResult> GetBiomarkers()
        {
            var bearerToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);

            if (string.IsNullOrEmpty(bearerToken))
            {
                return Unauthorized("Bearer token is required");
            }

            var startDateTime = DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var endDateTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            var categories = new[] { "activity", "sleep" };
            var types = new[] { "steps", "active_duration", "low_intensity_activity_duration", "sedentary_duration", "active_energy_burned", "sleep_start_time", "sleep_midpoint_time", "sleep_end_time", "in_bed_duration" };
            // Use UriBuilder and HttpUtility to build the URL
            var uriBuilder = new UriBuilder($"https://sandbox-api.sahha.ai/api/v1/profile/biomarker");
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query["categories"] = $"[{string.Join(",", categories)}]";
            query["types"] = $"[{string.Join(",", types)}]";
            query["startDateTime"] = startDateTime;
            query["endDateTime"] = endDateTime;

            uriBuilder.Query = query.ToString();
            var requestUri = uriBuilder.ToString();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Log the constructed request URI for debugging purposes
                Console.WriteLine($"Request URI: {requestUri}");

                var response = await _httpClient.GetAsync(requestUri);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Log the response content for debugging purposes
                Console.WriteLine($"Response Content: {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, responseContent);
                }

                if (string.IsNullOrWhiteSpace(responseContent) || responseContent == "[]")
                {
                    return Ok("No biomarkers found for the given parameters.");
                }

                var biomarkers = JsonConvert.DeserializeObject<object>(responseContent);
                return Ok(biomarkers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
