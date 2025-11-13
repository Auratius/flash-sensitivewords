using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Flash.SensitiveWords.Domain.Interfaces;
using Flash.SensitiveWords.Domain.ValueObjects;

namespace Flash.SensitiveWords.Domain.Services
{
    public class SanitizationService : ISanitizationService
    {
        private readonly ISensitiveWordRepository _repository;

        public SanitizationService(ISensitiveWordRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<SanitizedMessage> SanitizeMessageAsync(string message)
        {
            if (string.IsNullOrEmpty(message))
                return new SanitizedMessage(message ?? string.Empty, string.Empty, 0);

            var activeWords = await _repository.GetActiveWordsAsync();
            var wordList = activeWords.Select(w => w.Word).ToList();

            return SanitizeMessage(message, wordList);
        }

        public SanitizedMessage SanitizeMessage(string message, IEnumerable<string> sensitiveWords)
        {
            if (string.IsNullOrEmpty(message))
                return new SanitizedMessage(message ?? string.Empty, string.Empty, 0);

            var wordsList = sensitiveWords?.ToList() ?? new List<string>();
            if (!wordsList.Any())
                return new SanitizedMessage(message, message, 0);

            var sanitizedText = message;
            var wordsReplaced = 0;

            foreach (var word in wordsList.OrderByDescending(w => w.Length))
            {
                var pattern = $@"\b{Regex.Escape(word)}\b";
                var matches = Regex.Matches(sanitizedText, pattern, RegexOptions.IgnoreCase);

                if (matches.Count > 0)
                {
                    wordsReplaced += matches.Count;
                    sanitizedText = Regex.Replace(
                        sanitizedText,
                        pattern,
                        new string('*', word.Length),
                        RegexOptions.IgnoreCase
                    );
                }
            }

            return new SanitizedMessage(message, sanitizedText, wordsReplaced);
        }
    }
}
