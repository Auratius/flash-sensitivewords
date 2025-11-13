using System;
using System.Threading.Tasks;
using Flash.SensitiveWords.Application.Commands;
using Flash.SensitiveWords.Domain.Interfaces;

namespace Flash.SensitiveWords.Application.Handlers
{
    public class DeleteSensitiveWordHandler
    {
        private readonly ISensitiveWordRepository _repository;

        public DeleteSensitiveWordHandler(ISensitiveWordRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<bool> HandleAsync(DeleteSensitiveWordCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var existingWord = await _repository.GetByIdAsync(command.Id);
            if (existingWord == null)
                throw new InvalidOperationException($"Word with ID '{command.Id}' not found");

            return await _repository.DeleteAsync(command.Id);
        }
    }
}
