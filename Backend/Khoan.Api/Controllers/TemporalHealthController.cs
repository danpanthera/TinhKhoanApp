using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Khoan.Api.Data;
using Khoan.Api.Models.Common;

namespace Khoan.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemporalHealthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TemporalHealthController> _logger;

        public TemporalHealthController(ApplicationDbContext context, ILogger<TemporalHealthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public class TemporalStatusDto
        {
            public string Table { get; set; } = string.Empty;
            public bool IsTemporal { get; set; }
            public string? HistoryTable { get; set; }
            public int BaseColumnCount { get; set; }
            public int HistoryColumnCount { get; set; }
            public string? FirstBaseColumn { get; set; }
            public string? FirstHistoryColumn { get; set; }
            public bool IsPartitioned { get; set; }
            public bool HasColumnstore { get; set; }
            public string ColumnstoreType { get; set; } = "NONE"; // NONE | CLUSTERED | NONCLUSTERED
        }

        [HttpGet("database-info")]
        public async Task<IActionResult> GetDatabaseInfo()
        {
            try
            {
                await using var conn = _context.Database.GetDbConnection();
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                await using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT
                        @@VERSION as DatabaseVersion,
                        SERVERPROPERTY('Edition') as Edition,
                        SERVERPROPERTY('ProductLevel') as ProductLevel,
                        SERVERPROPERTY('EngineEdition') as EngineEdition,
                        CASE
                            WHEN SERVERPROPERTY('Edition') LIKE '%Azure SQL Edge%' THEN 1
                            ELSE 0
                        END as IsAzureSQLEdge
                ";

                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var info = new
                    {
                        DatabaseVersion = reader["DatabaseVersion"].ToString(),
                        Edition = reader["Edition"].ToString(),
                        ProductLevel = reader["ProductLevel"].ToString(),
                        EngineEdition = reader["EngineEdition"].ToString(),
                        IsAzureSQLEdge = Convert.ToBoolean(reader["IsAzureSQLEdge"])
                    };

                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Data = info,
                        Message = "Database info retrieved"
                    });
                }
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Could not retrieve database info"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving database info");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error retrieving database info"
                });
            }
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus([FromQuery] string table = "GL41")
        {
            var result = new TemporalStatusDto { Table = table };

            try
            {
                await using var conn = _context.Database.GetDbConnection();
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                await using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
DECLARE @objId INT = OBJECT_ID(CONCAT('dbo.', @tableName));

SELECT
    t.temporal_type AS TemporalType,
    ht.name AS HistoryTable
FROM sys.tables t
LEFT JOIN sys.tables ht ON t.history_table_id = ht.object_id
WHERE t.object_id = @objId;

SELECT
    (SELECT COUNT(*) FROM sys.columns WHERE object_id = @objId) AS BaseCount,
    (SELECT TOP 1 name FROM sys.columns WHERE object_id = @objId ORDER BY column_id) AS FirstBaseCol,
    (SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID((SELECT 'dbo.' + ht.name FROM sys.tables t LEFT JOIN sys.tables ht ON t.history_table_id = ht.object_id WHERE t.object_id = @objId))) AS HistCount,
    (SELECT TOP 1 name FROM sys.columns WHERE object_id = OBJECT_ID((SELECT 'dbo.' + ht.name FROM sys.tables t LEFT JOIN sys.tables ht ON t.history_table_id = ht.object_id WHERE t.object_id = @objId)) ORDER BY column_id) AS FirstHistCol;

SELECT CASE WHEN COUNT(DISTINCT partition_number) > 1 THEN 1 ELSE 0 END AS IsPartitioned
FROM sys.partitions WHERE object_id = @objId AND index_id IN (0,1);

SELECT TOP 1
    CASE
        WHEN i.type_desc = 'CLUSTERED COLUMNSTORE' THEN 'CLUSTERED'
        WHEN i.type_desc = 'NONCLUSTERED COLUMNSTORE' THEN 'NONCLUSTERED'
        ELSE 'NONE'
    END AS ColumnstoreType
FROM sys.indexes i
WHERE i.object_id = @objId AND i.type_desc LIKE '%COLUMNSTORE%';
";
                var p = cmd.CreateParameter();
                p.ParameterName = "@tableName";
                p.Value = table;
                cmd.Parameters.Add(p);

                await using var reader = await cmd.ExecuteReaderAsync();

                // 1) Temporal + history
                if (await reader.ReadAsync())
                {
                    var temporalType = reader.IsDBNull(0) ? 0 : Convert.ToInt32(reader.GetValue(0));
                    result.IsTemporal = temporalType == 2; // 2 = SYSTEM_VERSIONED_TEMPORAL_TABLE
                    result.HistoryTable = reader.IsDBNull(1) ? null : reader.GetString(1);
                }

                // 2) Column counts + first columns
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    result.BaseColumnCount = reader.IsDBNull(0) ? 0 : Convert.ToInt32(reader.GetValue(0));
                    result.FirstBaseColumn = reader.IsDBNull(1) ? null : reader.GetString(1);
                    result.HistoryColumnCount = reader.IsDBNull(2) ? 0 : Convert.ToInt32(reader.GetValue(2));
                    result.FirstHistoryColumn = reader.IsDBNull(3) ? null : reader.GetString(3);
                }

                // 3) Partitioned?
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    result.IsPartitioned = !reader.IsDBNull(0) && Convert.ToInt32(reader.GetValue(0)) == 1;
                }

                // 4) Columnstore type
                if (await reader.NextResultAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        result.ColumnstoreType = reader.IsDBNull(0) ? "NONE" : reader.GetString(0);
                        result.HasColumnstore = result.ColumnstoreType != "NONE";
                    }
                    else
                    {
                        result.ColumnstoreType = "NONE";
                        result.HasColumnstore = false;
                    }
                }

                var response = new ApiResponse<TemporalStatusDto>
                {
                    Success = true,
                    Message = $"Temporal health fetched for table {table}",
                    Data = result
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting temporal status for table {Table}", table);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Temporal health check failed",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
