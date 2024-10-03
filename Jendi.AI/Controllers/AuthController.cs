using Jendi.AI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jendi.AI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SahhaAuthService _authService;

        public AuthController(SahhaAuthService authService)
        {
            _authService = authService;
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
    }

/*    public class TokenRequestDto
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }*/

}
