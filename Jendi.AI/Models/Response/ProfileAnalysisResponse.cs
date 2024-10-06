using System.Text.Json.Serialization;

namespace Jendi.AI.Models.Response
{
    public class ProfileAnalysisResponse
    {
        [JsonPropertyName("inferences")]
        public List<Inference> Inferences { get; set; }
    }
    public class Inference
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }

        [JsonPropertyName("factors")]
        public List<Factor> Factors { get; set; }

        [JsonPropertyName("inputData")]
        public List<string> InputData { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }

    // Factor DTO
    public class Factor
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
