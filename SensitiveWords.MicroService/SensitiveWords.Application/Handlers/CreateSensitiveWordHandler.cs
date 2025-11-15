using System;
using System.Threading.Tasks;
using Flash.SensitiveWords.Application.Commands;
using Flash.SensitiveWords.Domain.Entities;
using Flash.SensitiveWords.Domain.Interfaces;

namespace Flash.SensitiveWords.Application.Handlers
{
    public class CreateSensitiveWordHandler
    {
        private readonly ISensitiveWordRepository _repository;
        private readonly IOperationStatsRepository _statsRepository;

        public CreateSensitiveWordHandler(
            ISensitiveWordRepository repository,
            IOperationStatsRepository statsRepository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _statsRepository = statsRepository ?? throw new ArgumentNullException(nameof(statsRepository));
        }

        public async Task<Guid> HandleAsync(CreateSensitiveWordCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var wordExists = await _repository.ExistsAsync(command.Word);
            if (wordExists)
                throw new InvalidOperationException($"Word '{command.Word}' already exists");

            var sensitiveWord = new SensitiveWord(command.Word);
            var result = await _repository.CreateAsync(sensitiveWord);

            // Track operation
            await _statsRepository.IncrementAsync(
                Flash.SensitiveWords.Domain.Entities.OperationType.Create,
                Flash.SensitiveWords.Domain.Entities.ResourceType.SensitiveWord);

            return result;
        }
    }
}
