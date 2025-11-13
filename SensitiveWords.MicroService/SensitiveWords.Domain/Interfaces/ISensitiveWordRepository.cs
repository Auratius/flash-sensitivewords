using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flash.SensitiveWords.Domain.Entities;

namespace Flash.SensitiveWords.Domain.Interfaces
{
    public interface ISensitiveWordRepository
    {
        Task<SensitiveWord> GetByIdAsync(Guid id);
        Task<IEnumerable<SensitiveWord>> GetAllAsync();
        Task<IEnumerable<SensitiveWord>> GetActiveWordsAsync();
        Task<SensitiveWord> GetByWordAsync(string word);
        Task<Guid> CreateAsync(SensitiveWord sensitiveWord);
        Task<bool> UpdateAsync(SensitiveWord sensitiveWord);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(string word);
        Task<int> BulkInsertAsync(IEnumerable<SensitiveWord> words);
    }
}
