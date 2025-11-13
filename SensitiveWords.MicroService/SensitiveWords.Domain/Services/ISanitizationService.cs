using System.Collections.Generic;
using System.Threading.Tasks;
using Flash.SensitiveWords.Domain.ValueObjects;

namespace Flash.SensitiveWords.Domain.Services
{
    public interface ISanitizationService
    {
        Task<SanitizedMessage> SanitizeMessageAsync(string message);
        SanitizedMessage SanitizeMessage(string message, IEnumerable<string> sensitiveWords);
    }
}
