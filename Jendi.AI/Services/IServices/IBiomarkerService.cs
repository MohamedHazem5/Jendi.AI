namespace Jendi.AI.Services.IServices
{
    public interface IBiomarkerService
    {
        Task<object> GetHeartRateAsync(string bearerToken);
        Task<object> GetCaloriesBurntAsync(string bearerToken);
        Task<object> GetSleepDataAsync(string bearerToken);
    }
}
