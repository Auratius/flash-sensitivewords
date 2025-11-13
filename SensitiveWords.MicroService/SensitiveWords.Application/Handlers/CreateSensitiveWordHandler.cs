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

        public CreateSensitiveWordHandler(ISensitiveWordRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Guid> HandleAsync(CreateSensitiveWordCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var wordExists = await _repository.ExistsAsync(command.Word);
            if (wordExists)
                throw new InvalidOperationException($"Word '{command.Word}' already exists");

            var sensitiveWord = new SensitiveWord(command.Word);
            return await _repository.CreateAsync(sensitiveWord);
        }
    }
}
