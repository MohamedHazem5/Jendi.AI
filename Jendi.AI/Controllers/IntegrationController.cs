using Jendi.AI.Services;
using Jendi.AI.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Jendi.AI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IntegrationController : ControllerBase
    {
        private readonly ISahhaService _sahhaService;

        public IntegrationController(ISahhaService sahhaService)
        {
            _sahhaService = sahhaService;
        }

        [HttpGet("version")]
        public async Task<IActionResult> GetVersion()
        {
            try
            {
                // Call the service to get the API version
                var version = await _sahhaService.GetApiVersionAsync();
                return Ok(new { version });
            }
            catch (HttpRequestException ex)
            {
                // Return a 500 error if the request fails
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // Endpoint to retrieve Wellbeing Scores
        [HttpGet("profile/score")]
        public async Task<IActionResult> GetWellbeingScores([FromHeader(Name = "Authorization")] string authorizationToken, [FromQuery] string types, [FromQuery] string startDateTime, [FromQuery] string endDateTime, [FromQuery] int version = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(authorizationToken))
                {
                    return BadRequest("Authorization token is required.");
                }

                // Remove "Bearer " from the token if present
                var token = authorizationToken.StartsWith("Bearer ") ? authorizationToken.Substring(7) : authorizationToken;

                var wellbeingScores = await _sahhaService.GetWellbeingScoresAsync(token, types, startDateTime, endDateTime, version);
                return Ok(new { scores = wellbeingScores });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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




    }
    public class RegisterProfileRequest
    {
        [Required]
        public string ExternalId { get; set; }
    }
}
