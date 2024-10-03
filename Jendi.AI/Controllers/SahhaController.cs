using Jendi.AI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jendi.AI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SahhaController : ControllerBase
    {
        private readonly SahhaApiService _sahhaApiService;

        public SahhaController(SahhaApiService sahhaApiService)
        {
            _sahhaApiService = sahhaApiService;
        }

        [HttpGet("wellbeing-scores")]
        public async Task<IActionResult> GetWellbeingScores(string startDateTime, string endDateTime, string token)
        {
            try
            {
                var wellbeingScores = await _sahhaApiService.GetWellbeingScoresAsync(token, startDateTime, endDateTime);
                if (wellbeingScores != null)
                {
                    return Ok(wellbeingScores);
                }
                return NotFound("No scores found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
