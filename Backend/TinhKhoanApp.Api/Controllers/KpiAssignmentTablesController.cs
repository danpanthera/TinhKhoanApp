using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// Controller quản lý KPI Assignment Tables - Bảng cấu hình KPI
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class KpiAssignmentTablesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<KpiAssignmentTablesController> _logger;

        public KpiAssignmentTablesController(ApplicationDbContext context, ILogger<KpiAssignmentTablesController> logger)
        {
            _context = context;
            _logger = logger;
        }        /// <summary>
                 /// Lấy tất cả bảng KPI Assignment Tables
                 /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetKpiAssignmentTables()
        {
            try
            {
                var query = @"
                    SELECT
                        kat.Id,
                        kat.TableName,
                        kat.Description,
                        kat.Category,
                        COUNT(ki.Id) as IndicatorCount
                    FROM KpiAssignmentTables kat
                    LEFT JOIN KpiIndicators ki ON kat.Id = ki.TableId AND ki.IsActive = 1
                    WHERE kat.IsActive = 1
                    GROUP BY kat.Id, kat.TableName, kat.Description, kat.Category
                    ORDER BY kat.Id
                ";

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = query;

                var tables = new List<object>();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var id = reader.GetInt32(0);
                    var tableName = reader.GetString(1);
                    var description = reader.IsDBNull(2) ? "" : reader.GetString(2);
                    var originalCategory = reader.GetString(3);
                    var indicatorCount = reader.GetInt32(4);

                    // Map category để frontend hiểu đúng
                    string mappedCategory = MapCategory(originalCategory);

                    tables.Add(new
                    {
                        Id = id,
                        TableType = tableName, // Use TableName as TableType for consistency
                        TableName = tableName,
                        Description = description,
                        Category = mappedCategory,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        IndicatorCount = indicatorCount
                    });
                }

                _logger.LogInformation("Retrieved {Count} KPI assignment tables", tables.Count);
                return Ok(tables);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving KPI assignment tables");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Lấy bảng KPI theo category
        /// </summary>
        [HttpGet("category/{category}")]
        public async Task<ActionResult> GetKpiAssignmentTablesByCategory(string category)
        {
            try
            {
                var tables = new List<object>();

                var query = @"
                    SELECT Id, TableName, Description, Category
                    FROM KpiAssignmentTables
                    WHERE Category = @p0
                    ORDER BY Id
                ";

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = query;
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@p0";
                parameter.Value = category;
                command.Parameters.Add(parameter);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    tables.Add(new
                    {
                        Id = reader.GetInt32(0),
                        TableType = "",
                        TableName = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        Category = reader.GetString(3),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("Retrieved {Count} KPI assignment tables for category {Category}", tables.Count, category);
                return Ok(tables);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving KPI assignment tables for category {Category}", category);
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Tạo schema KPI nếu chưa có
        /// </summary>
        [HttpPost("create-schema")]
        public async Task<ActionResult> CreateKpiSchema()
        {
            try
            {
                // Tạo bảng KpiAssignmentTables với đúng schema
                await _context.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'KpiAssignmentTables')
                    BEGIN
                        CREATE TABLE KpiAssignmentTables (
                            Id int IDENTITY(1,1) PRIMARY KEY,
                            TableType int NOT NULL DEFAULT 0,
                            TableName nvarchar(200) NOT NULL,
                            Description nvarchar(500),
                            Category nvarchar(100) NOT NULL DEFAULT 'Dành cho Cán bộ',
                            IsActive bit NOT NULL DEFAULT 1,
                            CreatedDate datetime2 NOT NULL DEFAULT GETDATE()
                        );
                    END
                ");

                // Insert sample data
                await _context.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (SELECT * FROM KpiAssignmentTables)
                    BEGIN
                        INSERT INTO KpiAssignmentTables (TableName, Description, Category) VALUES
                        ('KPI Giao Dịch Viên', 'Bảng KPI cho Giao Dịch Viên', 'Dành cho Cán bộ'),
                        ('KPI Quản Lý Khách Hàng', 'Bảng KPI cho QLKH', 'Dành cho Cán bộ'),
                        ('KPI Phó Giám Đốc', 'Bảng KPI cho Phó Giám Đốc', 'Dành cho Cán bộ'),
                        ('KPI Chi Nhánh', 'Bảng KPI tổng thể cho Chi Nhánh', 'Dành cho Chi nhánh'),
                        ('KPI Phòng Giao Dịch', 'Bảng KPI cho Phòng Giao Dịch', 'Dành cho Chi nhánh');
                    END
                ");

                return Ok(new { message = "KPI schema created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating KPI schema");
                return StatusCode(500, new { message = "Error creating schema", details = ex.Message });
            }
        }

        /// <summary>
        /// Debug endpoint để test database connection
        /// </summary>
        [HttpGet("debug")]
        public async Task<ActionResult> Debug()
        {
            try
            {
                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM KpiAssignmentTables";
                var count = await command.ExecuteScalarAsync();

                return Ok(new
                {
                    message = "Database connection successful",
                    tableCount = count,
                    connectionString = connection.ConnectionString.Replace(_context.Database.GetConnectionString()?.Split("Password=")[1]?.Split(";")[0] ?? "", "***")
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    message = "Database connection failed",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        /// <summary>
        /// Map category từ database sang format mà frontend mong đợi
        /// </summary>
        private string MapCategory(string originalCategory)
        {
            // Database đã có category đúng rồi, return nguyên bản
            return originalCategory?.ToUpper() ?? "CANBO";
        }
    }
}
