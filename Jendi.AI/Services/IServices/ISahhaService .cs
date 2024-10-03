namespace Jendi.AI.Services.IServices
{
    public interface ISahhaService
    {
        Task<string> GetApiVersionAsync();
        Task<string> GetWellbeingScoresAsync(string token, string types, string startDateTime, string endDateTime, int version = 1);
        Task<string> RegisterProfileAsync(string externalId, string token);




    }
}
