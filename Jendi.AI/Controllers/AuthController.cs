using Jendi.AI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using Jendi.AI.Models;

namespace Jendi.AI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SahhaAuthService _authService;
        private readonly HttpClient _httpClient;
        private readonly ProfileService _sahhaService;



        public AuthController(SahhaAuthService authService, HttpClient httpClient, ProfileService sahhaService)
        {
            _authService = authService;
            _httpClient = httpClient;
            _sahhaService = sahhaService;
        }

        [HttpPost("get-token")]
        public async Task<IActionResult> GetToken()
        {
            try
            {
                var tokenResponse = await _authService.GetAccountTokenAsync();
                return Ok(tokenResponse);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("register-profile")]
        public async Task<IActionResult> RegisterProfile([FromBody] RegisterProfileRequest request)
        {
            try
            {
                // Get token from header
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                // Call the service method to register profile
                var result = await _sahhaService.RegisterProfileAsync(request.ExternalId, token);

                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("get-profile")]
        public async Task<IActionResult> GetProfile()
        {
            var bearerToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);

            if (string.IsNullOrEmpty(bearerToken))
            {
                return Unauthorized("Bearer token is required");
            }

            var requestUri = $"https://sandbox-api.sahha.ai/api/v1/profile/demographic";

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            try
            {
                var response = await _httpClient.GetAsync(requestUri);

                var responseContent = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, responseContent);
                }

                if (string.IsNullOrWhiteSpace(responseContent) || responseContent == "[]")
                {
                    // Log or return a message indicating the data is empty
                    return Ok("No biomarkers found for the given parameters.");
                }

                var biomarkers = JsonSerializer.Deserialize<object>(responseContent);
                return Ok(biomarkers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }


}
