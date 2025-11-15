using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flash.SensitiveWords.Application.DTOs;
using Flash.SensitiveWords.Application.Queries;
using Flash.SensitiveWords.Domain.Interfaces;

namespace Flash.SensitiveWords.Application.Handlers
{
    public class GetAllSensitiveWordsHandler
    {
        private readonly ISensitiveWordRepository _repository;
        private readonly IOperationStatsRepository _statsRepository;

        public GetAllSensitiveWordsHandler(
            ISensitiveWordRepository repository,
            IOperationStatsRepository statsRepository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _statsRepository = statsRepository ?? throw new ArgumentNullException(nameof(statsRepository));
        }

        public async Task<IEnumerable<SensitiveWordDto>> HandleAsync(GetAllSensitiveWordsQuery query)
        {
            var words = query?.ActiveOnly == true
                ? await _repository.GetActiveWordsAsync()
                : await _repository.GetAllAsync();

            // Track operation
            await _statsRepository.IncrementAsync(
                Flash.SensitiveWords.Domain.Entities.OperationType.Read,
                Flash.SensitiveWords.Domain.Entities.ResourceType.SensitiveWord);

            return words.Select(w => new SensitiveWordDto
            {
                Id = w.Id,
                Word = w.Word,
                IsActive = w.IsActive,
                CreatedAt = w.CreatedAt,
                UpdatedAt = w.UpdatedAt
            });
        }
    }
}
