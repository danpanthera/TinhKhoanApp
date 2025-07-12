using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KpiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public KpiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetKpiSystemStatus()
        {
            try
            {
                var query = @"
                    SELECT
                        (SELECT COUNT(*) FROM KpiAssignmentTables) as TotalTables,
                        (SELECT COUNT(*) FROM KpiIndicators) as TotalIndicators,
                        (SELECT COUNT(*) FROM KpiAssignmentTables WHERE TableType = 'EMPLOYEE') as EmployeeTables,
                        (SELECT COUNT(*) FROM KpiAssignmentTables WHERE TableType = 'UNIT') as UnitTables
                ";

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = query;

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return Ok(new
                    {
                        Status = "Healthy",
                        Message = "KPI System is operational",
                        Data = new
                        {
                            TotalTables = reader.IsDBNull(reader.GetOrdinal("TotalTables")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalTables")),
                            TotalIndicators = reader.IsDBNull(reader.GetOrdinal("TotalIndicators")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalIndicators")),
                            EmployeeTables = reader.IsDBNull(reader.GetOrdinal("EmployeeTables")) ? 0 : reader.GetInt32(reader.GetOrdinal("EmployeeTables")),
                            UnitTables = reader.IsDBNull(reader.GetOrdinal("UnitTables")) ? 0 : reader.GetInt32(reader.GetOrdinal("UnitTables"))
                        },
                        Timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new
                {
                    Status = "Healthy",
                    Message = "KPI System is operational",
                    Data = new { TotalTables = 0, TotalIndicators = 0 },
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "KPI System error",
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("tables")]
        public async Task<IActionResult> GetKpiTables()
        {
            try
            {
                var query = @"
                    SELECT Id, TableType, TableName, Description, Category
                    FROM KpiAssignmentTables
                    ORDER BY Id
                ";

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = query;

                var tables = new List<object>();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    tables.Add(new
                    {
                        Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
                        TableType = reader.IsDBNull(reader.GetOrdinal("TableType")) ? "" : reader.GetString(reader.GetOrdinal("TableType")),
                        TableName = reader.IsDBNull(reader.GetOrdinal("TableName")) ? "" : reader.GetString(reader.GetOrdinal("TableName")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString(reader.GetOrdinal("Description")),
                        Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? "" : reader.GetString(reader.GetOrdinal("Category"))
                    });
                }

                return Ok(new
                {
                    Status = "Success",
                    Count = tables.Count,
                    Data = tables,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "Failed to retrieve KPI tables",
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("indicators")]
        public async Task<IActionResult> GetKpiIndicators()
        {
            try
            {
                var result = await _context.Database.SqlQueryRaw<KpiIndicatorDto>(
                    @"SELECT i.Id, i.TableId, i.IndicatorCode, i.IndicatorName,
                             i.Description, i.Unit, i.IsActive, i.CreatedAt, i.UpdatedAt,
                             t.TableName, t.Category
                      FROM KpiIndicators i
                      INNER JOIN KpiAssignmentTables t ON i.TableId = t.Id
                      ORDER BY t.TableName, i.IndicatorCode"
                ).ToListAsync();

                return Ok(new
                {
                    Status = "Success",
                    Count = result.Count,
                    Data = result,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "Failed to retrieve KPI indicators",
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetKpiSystemSummary()
        {
            try
            {
                var employeeTables = await _context.Database.ExecuteSqlRawAsync(
                    "SELECT COUNT(*) FROM KpiAssignmentTables WHERE TableType = 'EMPLOYEE'"
                );

                var unitTables = await _context.Database.ExecuteSqlRawAsync(
                    "SELECT COUNT(*) FROM KpiAssignmentTables WHERE TableType = 'UNIT'"
                );

                var totalIndicators = await _context.Database.ExecuteSqlRawAsync(
                    "SELECT COUNT(*) FROM KpiIndicators"
                );

                var categories = await _context.Database.SqlQueryRaw<CategorySummaryDto>(
                    "SELECT Category, COUNT(*) as TableCount FROM KpiAssignmentTables GROUP BY Category ORDER BY Category"
                ).ToListAsync();

                return Ok(new
                {
                    Status = "Success",
                    Message = "KPI System Summary",
                    Data = new
                    {
                        EmployeeTables = employeeTables,
                        UnitTables = unitTables,
                        TotalTables = employeeTables + unitTables,
                        TotalIndicators = totalIndicators,
                        Categories = categories
                    },
                    SystemInfo = new
                    {
                        DatabaseStatus = "Connected",
                        SchemaVersion = "1.0",
                        LastUpdated = DateTime.UtcNow
                    },
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "Failed to retrieve KPI system summary",
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }

    // DTOs for API responses
    public class KpiAssignmentTableDto
    {
        public int Id { get; set; }
        public string TableType { get; set; } = "";
        public string TableName { get; set; } = "";
        public string Description { get; set; } = "";
        public string Category { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class KpiIndicatorDto
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public string IndicatorCode { get; set; } = "";
        public string IndicatorName { get; set; } = "";
        public string Description { get; set; } = "";
        public string Unit { get; set; } = "";
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string TableName { get; set; } = "";
        public string Category { get; set; } = "";
    }

    public class CategorySummaryDto
    {
        public string Category { get; set; } = "";
        public int TableCount { get; set; }
    }
}
