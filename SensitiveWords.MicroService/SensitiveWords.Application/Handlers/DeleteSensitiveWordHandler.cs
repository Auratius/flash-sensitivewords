using System;
using System.Threading.Tasks;
using Flash.SensitiveWords.Application.Commands;
using Flash.SensitiveWords.Domain.Interfaces;

namespace Flash.SensitiveWords.Application.Handlers
{
    public class DeleteSensitiveWordHandler
    {
        private readonly ISensitiveWordRepository _repository;
        private readonly IOperationStatsRepository _statsRepository;

        public DeleteSensitiveWordHandler(
            ISensitiveWordRepository repository,
            IOperationStatsRepository statsRepository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _statsRepository = statsRepository ?? throw new ArgumentNullException(nameof(statsRepository));
        }

        public async Task<bool> HandleAsync(DeleteSensitiveWordCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var existingWord = await _repository.GetByIdAsync(command.Id);
            if (existingWord == null)
                throw new InvalidOperationException($"Word with ID '{command.Id}' not found");

            var result = await _repository.DeleteAsync(command.Id);

            // Track operation
            await _statsRepository.IncrementAsync(
                Flash.SensitiveWords.Domain.Entities.OperationType.Delete,
                Flash.SensitiveWords.Domain.Entities.ResourceType.SensitiveWord);

            return result;
        }
    }
}
