using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flash.SensitiveWords.Domain.Entities;

namespace Flash.SensitiveWords.Domain.Interfaces;

/// <summary>
/// Repository interface for managing operation statistics
/// </summary>
public interface IOperationStatsRepository
{
    /// <summary>
    /// Increments the count for a specific operation
    /// </summary>
    Task IncrementAsync(string operationType, string resourceType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all operation statistics
    /// </summary>
    Task<IEnumerable<OperationStat>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves statistics for a specific operation type
    /// </summary>
    Task<IEnumerable<OperationStat>> GetByTypeAsync(string operationType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets all operation counts to zero
    /// </summary>
    Task ResetAsync(CancellationToken cancellationToken = default);
}
