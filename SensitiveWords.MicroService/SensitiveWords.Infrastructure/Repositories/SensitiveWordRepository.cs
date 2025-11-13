using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Flash.SensitiveWords.Domain.Entities;
using Flash.SensitiveWords.Domain.Interfaces;
using Flash.SensitiveWords.Infrastructure.Data;

namespace Flash.SensitiveWords.Infrastructure.Repositories
{
    public class SensitiveWordRepository : ISensitiveWordRepository
    {
        private readonly DapperContext _context;

        public SensitiveWordRepository(DapperContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<SensitiveWord> GetByIdAsync(Guid id)
        {
            using var connection = _context.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_SensitiveWords_GetById",
                new { Id = id },
                commandType: CommandType.StoredProcedure);

            if (result == null)
                return null;

            return new SensitiveWord(
                result.Id,
                result.Word,
                result.IsActive,
                result.CreatedAt,
                result.UpdatedAt
            );
        }

        public async Task<IEnumerable<SensitiveWord>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();
            var results = await connection.QueryAsync<dynamic>(
                "sp_SensitiveWords_GetAll",
                commandType: CommandType.StoredProcedure);

            return results.Select(r => new SensitiveWord(
                r.Id,
                r.Word,
                r.IsActive,
                r.CreatedAt,
                r.UpdatedAt
            ));
        }

        public async Task<IEnumerable<SensitiveWord>> GetActiveWordsAsync()
        {
            using var connection = _context.CreateConnection();
            var results = await connection.QueryAsync<dynamic>(
                "sp_SensitiveWords_GetActive",
                commandType: CommandType.StoredProcedure);

            return results.Select(r => new SensitiveWord(
                r.Id,
                r.Word,
                r.IsActive,
                r.CreatedAt,
                r.UpdatedAt
            ));
        }

        public async Task<SensitiveWord> GetByWordAsync(string word)
        {
            using var connection = _context.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_SensitiveWords_GetByWord",
                new { Word = word },
                commandType: CommandType.StoredProcedure);

            if (result == null)
                return null;

            return new SensitiveWord(
                result.Id,
                result.Word,
                result.IsActive,
                result.CreatedAt,
                result.UpdatedAt
            );
        }

        public async Task<Guid> CreateAsync(SensitiveWord sensitiveWord)
        {
            if (sensitiveWord == null)
                throw new ArgumentNullException(nameof(sensitiveWord));

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                "sp_SensitiveWords_Create",
                new
                {
                    sensitiveWord.Id,
                    sensitiveWord.Word,
                    sensitiveWord.IsActive,
                    sensitiveWord.CreatedAt,
                    sensitiveWord.UpdatedAt
                },
                commandType: CommandType.StoredProcedure);

            return sensitiveWord.Id;
        }

        public async Task<bool> UpdateAsync(SensitiveWord sensitiveWord)
        {
            if (sensitiveWord == null)
                throw new ArgumentNullException(nameof(sensitiveWord));

            using var connection = _context.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_SensitiveWords_Update",
                new
                {
                    sensitiveWord.Id,
                    sensitiveWord.Word,
                    sensitiveWord.IsActive,
                    sensitiveWord.UpdatedAt
                },
                commandType: CommandType.StoredProcedure);

            return result?.RowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            using var connection = _context.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_SensitiveWords_Delete",
                new { Id = id },
                commandType: CommandType.StoredProcedure);

            return result?.RowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(string word)
        {
            using var connection = _context.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_SensitiveWords_Exists",
                new { Word = word },
                commandType: CommandType.StoredProcedure);

            return result?.Exists == 1;
        }

        public async Task<int> BulkInsertAsync(IEnumerable<SensitiveWord> words)
        {
            if (words == null || !words.Any())
                return 0;

            using var connection = _context.CreateConnection();

            // Create DataTable for table-valued parameter
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(Guid));
            dataTable.Columns.Add("Word", typeof(string));
            dataTable.Columns.Add("IsActive", typeof(bool));
            dataTable.Columns.Add("CreatedAt", typeof(DateTime));
            dataTable.Columns.Add("UpdatedAt", typeof(DateTime));

            foreach (var word in words)
            {
                dataTable.Rows.Add(word.Id, word.Word, word.IsActive, word.CreatedAt, word.UpdatedAt);
            }

            var parameters = new DynamicParameters();
            parameters.Add("@Words", dataTable.AsTableValuedParameter("dbo.SensitiveWordTableType"));

            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_SensitiveWords_BulkInsert",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result?.RowsAffected ?? 0;
        }
    }
}
