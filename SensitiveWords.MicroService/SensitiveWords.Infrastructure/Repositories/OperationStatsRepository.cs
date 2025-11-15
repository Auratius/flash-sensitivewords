using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Flash.SensitiveWords.Domain.Entities;
using Flash.SensitiveWords.Domain.Interfaces;

namespace Flash.SensitiveWords.Infrastructure.Repositories;

/// <summary>
/// SQL Server implementation of operation statistics repository
/// </summary>
public class OperationStatsRepository : IOperationStatsRepository
{
    private readonly string _connectionString;

    public OperationStatsRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task IncrementAsync(string operationType, string resourceType, CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var parameters = new
        {
            OperationType = operationType,
            ResourceType = resourceType
        };

        await connection.ExecuteAsync(
            "[dbo].[IncrementOperationCount]",
            parameters,
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<IEnumerable<OperationStat>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var stats = await connection.QueryAsync<OperationStat>(
            "[dbo].[GetAllOperationStats]",
            commandType: CommandType.StoredProcedure
        );

        return stats;
    }

    public async Task<IEnumerable<OperationStat>> GetByTypeAsync(string operationType, CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var parameters = new { OperationType = operationType };

        var stats = await connection.QueryAsync<OperationStat>(
            "[dbo].[GetOperationStatsByType]",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return stats;
    }

    public async Task ResetAsync(CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await connection.ExecuteAsync(
            "[dbo].[ResetOperationStats]",
            commandType: CommandType.StoredProcedure
        );
    }
}
