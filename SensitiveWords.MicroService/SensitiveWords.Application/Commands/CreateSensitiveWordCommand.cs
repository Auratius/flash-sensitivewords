using System;

namespace Flash.SensitiveWords.Application.Commands
{
    public class CreateSensitiveWordCommand
    {
        public string Word { get; set; }

        public CreateSensitiveWordCommand(string word)
        {
            Word = word;
        }
    }
}
