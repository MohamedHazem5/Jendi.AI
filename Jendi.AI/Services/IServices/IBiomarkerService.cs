using Jendi.AI.Models.Request;
using Jendi.AI.Models.Response;

namespace Jendi.AI.Services.IServices
{
    public interface IBiomarkerService
    {
        Task<ProfileAnalysisResponse> GetProfileAnalysisAsync(AnalysisRequest request, string bearerToken);
        Task<IEnumerable<Inference>> GetSleepBiomarkersAsync(AnalysisRequest request, string bearerToken);
        Task<IEnumerable<Inference>> GetStressBiomarkersAsync(AnalysisRequest request, string bearerToken);
        Task<IEnumerable<Inference>> GetWellbeingBiomarkersAsync(AnalysisRequest request, string bearerToken);
        Task<object> GetHeartRateAsync(string bearerToken);
        Task<object> GetCaloriesBurntAsync(string bearerToken);
        Task<object> GetSleepDataAsync(string bearerToken);
    }
}
