using System;
using System.Threading.Tasks;
using Flash.SensitiveWords.Application.DTOs;
using Flash.SensitiveWords.Application.Queries;
using Flash.SensitiveWords.Domain.Services;

namespace Flash.SensitiveWords.Application.Handlers
{
    public class SanitizeMessageHandler
    {
        private readonly ISanitizationService _sanitizationService;

        public SanitizeMessageHandler(ISanitizationService sanitizationService)
        {
            _sanitizationService = sanitizationService ?? throw new ArgumentNullException(nameof(sanitizationService));
        }

        public async Task<SanitizeResponse> HandleAsync(SanitizeMessageQuery query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var result = await _sanitizationService.SanitizeMessageAsync(query.Message);

            return new SanitizeResponse
            {
                OriginalMessage = result.OriginalMessage,
                SanitizedMessage = result.SanitizedText,
                WordsReplaced = result.WordsReplaced
            };
        }
    }
}
