using System;
using System.Threading.Tasks;
using Flash.SensitiveWords.Application.Commands;
using Flash.SensitiveWords.Domain.Interfaces;

namespace Flash.SensitiveWords.Application.Handlers
{
    public class UpdateSensitiveWordHandler
    {
        private readonly ISensitiveWordRepository _repository;

        public UpdateSensitiveWordHandler(ISensitiveWordRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<bool> HandleAsync(UpdateSensitiveWordCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var existingWord = await _repository.GetByIdAsync(command.Id);
            if (existingWord == null)
                throw new InvalidOperationException($"Word with ID '{command.Id}' not found");

            var wordWithSameText = await _repository.GetByWordAsync(command.Word);
            if (wordWithSameText != null && wordWithSameText.Id != command.Id)
                throw new InvalidOperationException($"Word '{command.Word}' already exists");

            existingWord.UpdateWord(command.Word);

            if (command.IsActive)
                existingWord.Activate();
            else
                existingWord.Deactivate();

            return await _repository.UpdateAsync(existingWord);
        }
    }
}
