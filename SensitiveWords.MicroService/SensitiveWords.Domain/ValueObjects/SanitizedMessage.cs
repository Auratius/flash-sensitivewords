using System;

namespace Flash.SensitiveWords.Domain.ValueObjects
{
    public class SanitizedMessage
    {
        public string OriginalMessage { get; }
        public string SanitizedText { get; }
        public int WordsReplaced { get; }

        public SanitizedMessage(string originalMessage, string sanitizedText, int wordsReplaced)
        {
            if (originalMessage == null)
                throw new ArgumentNullException(nameof(originalMessage));
            if (sanitizedText == null)
                throw new ArgumentNullException(nameof(sanitizedText));

            OriginalMessage = originalMessage;
            SanitizedText = sanitizedText;
            WordsReplaced = wordsReplaced;
        }
    }
}
