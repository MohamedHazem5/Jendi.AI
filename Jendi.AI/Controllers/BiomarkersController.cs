using Jendi.AI.Services;
using Jendi.AI.Services.IServices;
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
        private readonly IBiomarkerService _biomarkerService;

        public BiomarkersController(IBiomarkerService biomarkerService)
        {
            _biomarkerService = biomarkerService;
        }

        /// <summary>
        /// Retrieves the user's heart rate data using an authentication token.
        /// </summary>
        /// <returns>
        /// Returns the heart rate data of the user if available. If the user is unauthorized, returns an error message.
        /// </returns>
        /// <remarks>
        /// This endpoint fetches the user's heart rate data from the biomarker service. The bearer token must be included in the `Authorization` header in the format `Bearer {token}`.
        /// 
        /// ### Bearer Token Requirement
        /// - A valid bearer token is required for authentication.
        /// 
        /// ### Example Request
        /// ```
        /// GET /api/heart-rate
        /// Authorization: Bearer YOUR_BEARER_TOKEN_HERE
        /// ```
        /// 
        /// ### Data Fields
        /// The heart rate data includes metrics such as:
        /// - `heart_rate`: The user's heart rate in beats per minute (BPM).
        /// - Additional metrics may be available depending on the user's profile.
        ///
        /// ### Expected Responses
        /// - **200 OK**: Returns the user's heart rate data if successful.
        /// - **401 Unauthorized**: If the bearer token is missing or invalid.
        /// - **500 Internal Server Error**: If an unexpected error occurs while processing the request.
        /// </remarks>
        /// <response code="200">Returns the heart rate data successfully.</response>
        /// <response code="401">Bearer token is required or invalid.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpGet("heart-rate")]
        public async Task<IActionResult> GetHeartRate()
        {
            try
            {
                var bearerToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                var data = await _biomarkerService.GetHeartRateAsync(bearerToken);
                return Ok(data);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        /// <summary>
        /// Retrieves the user's calories burnt data using an authentication token.
        /// </summary>
        /// <returns>
        /// Returns the calories burnt data of the user if available. If the user is unauthorized, returns an error message.
        /// </returns>
        /// <remarks>
        /// This endpoint fetches the user's calories burnt data from the biomarker service. The bearer token must be included in the `Authorization` header in the format `Bearer {token}`.
        /// 
        /// ### Bearer Token Requirement
        /// - A valid bearer token is required for authentication.
        /// 
        /// ### Example Request
        /// ```
        /// GET /api/calories-burnt
        /// Authorization: Bearer YOUR_BEARER_TOKEN_HERE
        /// ```
        /// 
        /// ### Data Fields
        /// The calories burnt data includes metrics such as:
        /// - `active_energy_burned`: The total calories burned during physical activity.
        /// - `sedentary_duration`: The duration the user was sedentary (in minutes).
        /// - Additional metrics may be available depending on the user's profile.
        ///
        /// ### Expected Responses
        /// - **200 OK**: Returns the user's calories burnt data if successful.
        /// - **401 Unauthorized**: If the bearer token is missing or invalid.
        /// - **500 Internal Server Error**: If an unexpected error occurs while processing the request.
        /// </remarks>
        /// <response code="200">Returns the calories burnt data successfully.</response>
        /// <response code="401">Bearer token is required or invalid.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpGet("calories-burnt")]
        public async Task<IActionResult> GetCaloriesBurnt()
        {
            try
            {
                var bearerToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                var data = await _biomarkerService.GetCaloriesBurntAsync(bearerToken);
                return Ok(data);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        /// <summary>
        /// Retrieves the user's sleep data using an authentication token.
        /// </summary>
        /// <returns>
        /// Returns the sleep data of the user if available. If the user is unauthorized, returns an error message.
        /// </returns>
        /// <remarks>
        /// This endpoint fetches the user's sleep data from the biomarker service. The bearer token must be included in the `Authorization` header in the format `Bearer {token}`.
        /// 
        /// ### Bearer Token Requirement
        /// - A valid bearer token is required for authentication.
        /// 
        /// ### Example Request
        /// ```
        /// GET /api/sleep-data
        /// Authorization: Bearer YOUR_BEARER_TOKEN_HERE
        /// ```
        /// 
        /// ### Data Fields
        /// The sleep data includes metrics such as:
        /// - `sleep_start_time`: The time the user fell asleep.
        /// - `sleep_midpoint_time`: The midpoint of the user's sleep duration.
        /// - `sleep_end_time`: The time the user woke up.
        /// - `in_bed_duration`: The total duration spent in bed (in minutes).
        /// - Additional metrics may be available depending on the user's profile.
        ///
        /// ### Expected Responses
        /// - **200 OK**: Returns the user's sleep data if successful.
        /// - **401 Unauthorized**: If the bearer token is missing or invalid.
        /// - **500 Internal Server Error**: If an unexpected error occurs while processing the request.
        /// </remarks>
        /// <response code="200">Returns the sleep data successfully.</response>
        /// <response code="401">Bearer token is required or invalid.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpGet("sleep-data")]
        public async Task<IActionResult> GetSleepData()
        {
            try
            {
                var bearerToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                var data = await _biomarkerService.GetSleepDataAsync(bearerToken);
                return Ok(data);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
