using Jendi.AI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using Jendi.AI.Models.Request;

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

        /// <summary>
        /// Registers a user profile using a provided external ID and retrieves an authentication token.
        /// </summary>
        /// <param name="request">The request body containing the profile information, specifically the External ID.</param>
        /// <returns>
        /// Returns the profile token upon successful registration. If the request fails, returns an appropriate error message.
        /// </returns>
        /// <remarks>
        /// This endpoint performs two main actions:
        /// 1. **Obtains an authentication token**: The token is acquired by calling the internal authentication service (`_authService`).
        /// 2. **Registers the user profile**: The profile is registered with the provided External ID using the obtained token. The response includes a profile-specific token.
        ///
        /// ### Example Usage
        /// The endpoint requires an `ExternalID` in the request body. Below is a hardcoded sample value:
        /// 
        /// ```json
        /// {
        ///   "ExternalId": "aSJendiF21MJbOSMBBaqp5j74bC2"
        /// }
        /// ```
        /// This External ID represents a user test profile, while a sample profile from Sahha is represented by:
        /// - **Sample From Sohha External ID**: `aSJendiF21MJbOSMBBaqp5j74bC2`
        /// - **My External ID**: `aD7ymJCF21MJbOSMBBaqp5j74bC2`
        ///
        /// ### Expected Response
        /// - **Profile Token**: If successful, the response returns a profile token for the newly registered profile.
        ///
        /// ### Response Codes
        /// - **200 OK**: Successfully registered the user profile and retrieved the token.
        /// - **400 Bad Request**: Indicates a problem with the provided data (e.g., invalid External ID).
        /// - **500 Internal Server Error**: Indicates that an error occurred while processing the request.
        ///
        /// </remarks>
        /// <response code="200">Returns the profile token successfully upon registration.</response>
        /// <response code="400">Returns a bad request error if there is an issue with the provided External ID or data.</response>
        /// <response code="500">Returns an internal server error if an unexpected issue occurs.</response>
        [HttpPost("register-and-get-token")]
        public async Task<IActionResult> RegisterAndGetToken([FromBody] RegisterProfileRequest request)
        {
            try
            {
                // Get the token by calling the _authService
                var tokenResponse = await _authService.GetAccountTokenAsync();

                // Register the profile using the token obtained
                var registerResult = await _sahhaService.RegisterProfileAsync(request.ExternalId, tokenResponse.AccountToken);

                return Ok(new
                {
                    ProfileToken = registerResult.profileToken
                });
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves the demographic profile of the user using an authentication token.
        /// </summary>
        /// <returns>
        /// Returns the demographic profile data of the user as provided by Sahha. If the data is not found, returns an appropriate message.
        /// </returns>
        /// <remarks>
        /// This endpoint fetches the demographic profile for a user using the provided bearer token. The bearer token must be included in the `Authorization` header in the format `Bearer {token}`.
        /// 
        /// ### Bearer Token Requirement
        /// - The user must provide a valid bearer token in the request header to authenticate with the Sahha API.
        /// 
        /// ### Example Request
        /// ```
        /// GET /api/get-profile
        /// Authorization: Bearer YOUR_BEARER_TOKEN_HERE
        /// ```
        /// 
        /// ### Expected Responses
        /// - **200 OK**: Returns the demographic profile data if successful.
        /// - **200 OK (Empty Response)**: Returns "No biomarkers found for the given parameters." if the retrieved data is empty.
        /// - **401 Unauthorized**: If the bearer token is missing or invalid.
        /// - **500 Internal Server Error**: If an unexpected error occurs while processing the request.
        ///
        /// ### External API Usage
        /// The endpoint makes a GET request to the Sahha API endpoint (`https://sandbox-api.sahha.ai/api/v1/profile/demographic`) to retrieve user demographic data.
        /// 
        /// ### Important Note
        /// If the data returned from Sahha is an empty array (`[]`), an appropriate message is returned instead of the data.
        /// </remarks>
        /// <response code="200">Returns the demographic profile data if successful.</response>
        /// <response code="401">Bearer token is required or invalid.</response>
        /// <response code="500">Internal server error occurred.</response>
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
                    return Ok("No Data found for the given ExternalId.");
                }

                var profileData = JsonSerializer.Deserialize<object>(responseContent);
                return Ok(profileData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }


}
