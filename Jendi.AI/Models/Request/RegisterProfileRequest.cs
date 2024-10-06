using System.ComponentModel.DataAnnotations;

namespace Jendi.AI.Models.Request
{
    public class RegisterProfileRequest
    {
        [Required]
        public string ExternalId { get; set; }
    }


}
