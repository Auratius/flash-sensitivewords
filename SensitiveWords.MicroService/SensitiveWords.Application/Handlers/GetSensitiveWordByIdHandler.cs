using System;
using System.Threading.Tasks;
using Flash.SensitiveWords.Application.DTOs;
using Flash.SensitiveWords.Application.Queries;
using Flash.SensitiveWords.Domain.Interfaces;

namespace Flash.SensitiveWords.Application.Handlers
{
    public class GetSensitiveWordByIdHandler
    {
        private readonly ISensitiveWordRepository _repository;

        public GetSensitiveWordByIdHandler(ISensitiveWordRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<SensitiveWordDto> HandleAsync(GetSensitiveWordByIdQuery query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var word = await _repository.GetByIdAsync(query.Id);
            if (word == null)
                return null;

            return new SensitiveWordDto
            {
                Id = word.Id,
                Word = word.Word,
                IsActive = word.IsActive,
                CreatedAt = word.CreatedAt,
                UpdatedAt = word.UpdatedAt
            };
        }
    }
}
