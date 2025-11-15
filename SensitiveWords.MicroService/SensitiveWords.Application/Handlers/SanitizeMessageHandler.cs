using System;
using System.Threading.Tasks;
using Flash.SensitiveWords.Application.DTOs;
using Flash.SensitiveWords.Application.Queries;
using Flash.SensitiveWords.Domain.Services;
using Flash.SensitiveWords.Domain.Interfaces;

namespace Flash.SensitiveWords.Application.Handlers
{
    public class SanitizeMessageHandler
    {
        private readonly ISanitizationService _sanitizationService;
        private readonly IOperationStatsRepository _statsRepository;

        public SanitizeMessageHandler(
            ISanitizationService sanitizationService,
            IOperationStatsRepository statsRepository)
        {
            _sanitizationService = sanitizationService ?? throw new ArgumentNullException(nameof(sanitizationService));
            _statsRepository = statsRepository ?? throw new ArgumentNullException(nameof(statsRepository));
        }

        public async Task<SanitizeResponse> HandleAsync(SanitizeMessageQuery query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var result = await _sanitizationService.SanitizeMessageAsync(query.Message);

            // Track operation
            await _statsRepository.IncrementAsync(
                Flash.SensitiveWords.Domain.Entities.OperationType.Sanitize,
                Flash.SensitiveWords.Domain.Entities.ResourceType.Message);

            return new SanitizeResponse
            {
                OriginalMessage = result.OriginalMessage,
                SanitizedMessage = result.SanitizedText,
                WordsReplaced = result.WordsReplaced
            };
        }
    }
}
