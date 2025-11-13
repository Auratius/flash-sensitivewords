using System.ComponentModel.DataAnnotations;

namespace Flash.SensitiveWords.Application.DTOs
{
    public class SanitizeRequest
    {
        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; }
    }
}
