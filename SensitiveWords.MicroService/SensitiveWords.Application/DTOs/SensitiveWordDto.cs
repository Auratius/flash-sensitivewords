using System;

namespace Flash.SensitiveWords.Application.DTOs
{
    public class SensitiveWordDto
    {
        public Guid Id { get; set; }
        public string Word { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
