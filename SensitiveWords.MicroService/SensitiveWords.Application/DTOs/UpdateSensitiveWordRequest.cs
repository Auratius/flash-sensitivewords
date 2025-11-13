using System.ComponentModel.DataAnnotations;

namespace Flash.SensitiveWords.Application.DTOs
{
    public class UpdateSensitiveWordRequest
    {
        [Required(ErrorMessage = "Word is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Word must be between 1 and 100 characters")]
        public string Word { get; set; }

        public bool IsActive { get; set; }
    }
}
