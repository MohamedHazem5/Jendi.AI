namespace Jendi.AI.Models.Dtos
{
    public class IntegrationResponseDto
    {
        public string IntegrationType { get; set; }  
        public IntegrationValuesDto IntegrationValues { get; set; } 
        public string IntegrationIdentifier { get; set; }  
    }
    public class IntegrationValuesDto
    {
        public string UserId { get; set; }  
    }
}
