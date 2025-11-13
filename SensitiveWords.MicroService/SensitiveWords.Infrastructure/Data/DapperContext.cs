using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Flash.SensitiveWords.Infrastructure.Data
{
    public class DapperContext
    {
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("DefaultConnection not found in configuration");
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
