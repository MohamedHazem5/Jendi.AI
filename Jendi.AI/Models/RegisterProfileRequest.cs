using System.ComponentModel.DataAnnotations;

namespace Jendi.AI.Models
{
    public class RegisterProfileRequest
    {
        [Required]
        public string ExternalId { get; set; }
    }


}
