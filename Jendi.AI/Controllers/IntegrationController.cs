using Jendi.AI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jendi.AI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IntegrationController : ControllerBase
    {
        private readonly SahhaIntegrationService _integrationService;

        public IntegrationController(SahhaIntegrationService integrationService)
        {
            _integrationService = integrationService;
        }

        [HttpGet("get-profile-integrations")]
        public async Task<IActionResult> GetProfileIntegrations([FromHeader(Name = "Authorization")] string authorization = null)
        {
            // Check if the header is missing or invalid.
            if (string.IsNullOrWhiteSpace(authorization) || !authorization.StartsWith("profile "))
            {
                return BadRequest("Invalid or missing Authorization header. Use 'profile {token}'.");
            }

            // Extract the token after 'profile '.
            var profileToken = authorization.Substring(8); // "profile " is 8 characters long.

            try
            {
                // Call the service to fetch the profile integrations.
                var integrations = await _integrationService.GetProfileIntegrationsAsync(profileToken);
                return Ok(integrations);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
