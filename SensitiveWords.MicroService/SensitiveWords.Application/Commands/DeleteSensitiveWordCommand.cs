using System;

namespace Flash.SensitiveWords.Application.Commands
{
    public class DeleteSensitiveWordCommand
    {
        public Guid Id { get; set; }

        public DeleteSensitiveWordCommand(Guid id)
        {
            Id = id;
        }
    }
}
