using Newtonsoft.Json;

namespace Jendi.AI.Models
{
    public class AccountTokenResponseDto
    {
        [JsonProperty("accountToken")]
        public string AccountToken { get; set; }

        [JsonProperty("tokenType")]
        public string TokenType { get; set; }
    }
}
