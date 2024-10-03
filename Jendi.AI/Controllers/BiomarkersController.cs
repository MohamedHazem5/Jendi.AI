using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

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
        public async Task<IActionResult> GetBiomarkers(
            [FromQuery] string[] categories,
            [FromQuery] string[] types,
            [FromQuery] DateTime startDateTime,
            [FromQuery] DateTime endDateTime,
            string externalId
            )
        {
            var bearerToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);

            if (string.IsNullOrEmpty(bearerToken))
            {
                return Unauthorized("Bearer token is required");
            }

            var requestUri = $"https://sandbox-api.sahha.ai/api/v1/profile/biomarker/{externalId}" +
                             $"?categories={string.Join(",", categories)}" +
                             $"&types={string.Join(",", types)}" +
                             $"&startDateTime={startDateTime:O}" +
                             $"&endDateTime={endDateTime:O}";

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            try
            {
                var response = await _httpClient.GetAsync(requestUri);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                }

                var biomarkers = await response.Content.ReadAsStringAsync();
                var parsedResponse = JsonSerializer.Deserialize<object>(biomarkers);
                return Ok(parsedResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
