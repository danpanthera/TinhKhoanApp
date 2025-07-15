using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebugController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly ILogger<DebugController> _logger;

        public DebugController(IConfiguration configuration, ILogger<DebugController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            _logger = logger;
        }

        /// <summary>
        /// Get table column schema for debugging column mapping issues
        /// </summary>
        [HttpGet("table-schema/{tableName}")]
        public async Task<IActionResult> GetTableSchema(string tableName)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"
                    SELECT
                        COLUMN_NAME,
                        DATA_TYPE,
                        IS_NULLABLE,
                        CHARACTER_MAXIMUM_LENGTH,
                        ORDINAL_POSITION
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = @TableName
                    ORDER BY ORDINAL_POSITION";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@TableName", tableName);

                var columns = new List<object>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    columns.Add(new
                    {
                        ColumnName = reader["COLUMN_NAME"].ToString(),
                        DataType = reader["DATA_TYPE"].ToString(),
                        IsNullable = reader["IS_NULLABLE"].ToString(),
                        MaxLength = reader["CHARACTER_MAXIMUM_LENGTH"].ToString(),
                        Position = reader["ORDINAL_POSITION"]
                    });
                }

                return Ok(new
                {
                    TableName = tableName,
                    TotalColumns = columns.Count,
                    Columns = columns
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting table schema for {TableName}", tableName);
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
