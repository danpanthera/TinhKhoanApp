using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KpiIndicatorsController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly ILogger<KpiIndicatorsController> _logger;

        public KpiIndicatorsController(IConfiguration configuration, ILogger<KpiIndicatorsController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        [HttpGet("table/{tableId}")]
        public async Task<IActionResult> GetIndicatorsByTableId(int tableId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT
                        ki.Id,
                        ki.TableId,
                        ki.IndicatorCode,
                        ki.IndicatorName,
                        ki.Description,
                        ki.Unit,
                        ki.IsActive,
                        ki.CreatedAt,
                        ki.UpdatedAt,
                        kat.TableName,
                        kat.Category
                    FROM KpiIndicators ki
                    INNER JOIN KpiAssignmentTables kat ON ki.TableId = kat.Id
                    WHERE ki.TableId = @TableId
                        AND ki.IsActive = 1
                        AND kat.IsActive = 1
                    ORDER BY ki.IndicatorCode";

                using var command = new SqlCommand(query, connection);
                command.Parameters.Add(new SqlParameter("@TableId", SqlDbType.Int) { Value = tableId });

                var indicators = new List<object>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    indicators.Add(new
                    {
                        Id = reader.GetInt32("Id"),
                        TableId = reader.GetInt32("TableId"),
                        IndicatorCode = reader.GetString("IndicatorCode"),
                        IndicatorName = reader.GetString("IndicatorName"),
                        Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                        Unit = reader.IsDBNull("Unit") ? null : reader.GetString("Unit"),
                        IsActive = reader.GetBoolean("IsActive"),
                        CreatedAt = reader.GetDateTime("CreatedAt"),
                        UpdatedAt = reader.GetDateTime("UpdatedAt"),
                        TableName = reader.GetString("TableName"),
                        Category = reader.GetString("Category")
                    });
                }

                return Ok(indicators);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting indicators for table {TableId}", tableId);
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách chỉ tiêu", error = ex.Message });
            }
        }

        [HttpGet("table/{tableId}/summary")]
        public async Task<IActionResult> GetTableWithIndicators(int tableId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Lấy thông tin bảng
                var tableQuery = @"
                    SELECT Id, TableName, Description, Category, CreatedAt, UpdatedAt
                    FROM KpiAssignmentTables
                    WHERE Id = @TableId AND IsActive = 1";

                using var tableCommand = new SqlCommand(tableQuery, connection);
                tableCommand.Parameters.Add(new SqlParameter("@TableId", SqlDbType.Int) { Value = tableId });

                object tableInfo = null;
                using (var tableReader = await tableCommand.ExecuteReaderAsync())
                {
                    if (await tableReader.ReadAsync())
                    {
                        tableInfo = new
                        {
                            Id = tableReader.GetInt32("Id"),
                            TableName = tableReader.GetString("TableName"),
                            Description = tableReader.IsDBNull("Description") ? null : tableReader.GetString("Description"),
                            Category = tableReader.GetString("Category"),
                            CreatedAt = tableReader.GetDateTime("CreatedAt"),
                            UpdatedAt = tableReader.GetDateTime("UpdatedAt")
                        };
                    }
                }

                if (tableInfo == null)
                {
                    return NotFound($"Không tìm thấy bảng KPI với ID: {tableId}");
                }

                // Lấy danh sách chỉ tiêu
                var indicatorQuery = @"
                    SELECT Id, IndicatorCode, IndicatorName, Description, Unit, IsActive, CreatedAt, UpdatedAt
                    FROM KpiIndicators
                    WHERE TableId = @TableId AND IsActive = 1
                    ORDER BY IndicatorCode";

                using var indicatorCommand = new SqlCommand(indicatorQuery, connection);
                indicatorCommand.Parameters.Add(new SqlParameter("@TableId", SqlDbType.Int) { Value = tableId });

                var indicators = new List<object>();
                using (var indicatorReader = await indicatorCommand.ExecuteReaderAsync())
                {
                    while (await indicatorReader.ReadAsync())
                    {
                        indicators.Add(new
                        {
                            Id = indicatorReader.GetInt32("Id"),
                            IndicatorCode = indicatorReader.GetString("IndicatorCode"),
                            IndicatorName = indicatorReader.GetString("IndicatorName"),
                            Description = indicatorReader.IsDBNull("Description") ? null : indicatorReader.GetString("Description"),
                            Unit = indicatorReader.IsDBNull("Unit") ? null : indicatorReader.GetString("Unit"),
                            IsActive = indicatorReader.GetBoolean("IsActive"),
                            CreatedAt = indicatorReader.GetDateTime("CreatedAt"),
                            UpdatedAt = indicatorReader.GetDateTime("UpdatedAt")
                        });
                    }
                }

                return Ok(new
                {
                    Table = tableInfo,
                    Indicators = indicators,
                    IndicatorCount = indicators.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting table with indicators for table {TableId}", tableId);
                return StatusCode(500, new { message = "Lỗi khi lấy thông tin bảng và chỉ tiêu", error = ex.Message });
            }
        }
    }
}
