using System;

namespace Flash.SensitiveWords.Application.Commands
{
    public class UpdateSensitiveWordCommand
    {
        public Guid Id { get; set; }
        public string Word { get; set; }
        public bool IsActive { get; set; }

        public UpdateSensitiveWordCommand(Guid id, string word, bool isActive)
        {
            Id = id;
            Word = word;
            IsActive = isActive;
        }
    }
}
