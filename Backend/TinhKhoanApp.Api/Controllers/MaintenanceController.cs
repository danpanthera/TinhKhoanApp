using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("create-kpi-schema")]
        public async Task<IActionResult> CreateKpiSchema()
        {
            try
            {
                // Tạo bảng KpiAssignmentTables nếu chưa có
                await _context.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'KpiAssignmentTables')
                    BEGIN
                        CREATE TABLE KpiAssignmentTables (
                            Id int IDENTITY(1,1) PRIMARY KEY,
                            TableType nvarchar(50) NOT NULL,
                            TableName nvarchar(100) NOT NULL,
                            Description nvarchar(max),
                            Category nvarchar(50) NOT NULL,
                            CreatedAt datetime2 DEFAULT GETDATE(),
                            UpdatedAt datetime2 DEFAULT GETDATE()
                        );
                    END");

                // Tạo bảng KpiIndicators nếu chưa có
                await _context.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'KpiIndicators')
                    BEGIN
                        CREATE TABLE KpiIndicators (
                            Id int IDENTITY(1,1) PRIMARY KEY,
                            TableId int NOT NULL,
                            IndicatorCode nvarchar(50) NOT NULL,
                            IndicatorName nvarchar(200) NOT NULL,
                            Description nvarchar(max),
                            Unit nvarchar(50),
                            IsActive bit DEFAULT 1,
                            CreatedAt datetime2 DEFAULT GETDATE(),
                            UpdatedAt datetime2 DEFAULT GETDATE(),
                            FOREIGN KEY (TableId) REFERENCES KpiAssignmentTables(Id)
                        );
                    END");

                return Ok(new
                {
                    message = "✅ KPI Schema created successfully!",
                    tables = new[] { "KpiAssignmentTables", "KpiIndicators" }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "❌ Error creating KPI schema",
                    error = ex.Message
                });
            }
        }

        [HttpGet("verify-kpi-schema")]
        public async Task<IActionResult> VerifyKpiSchema()
        {
            try
            {
                var kpiTables = await _context.Database.SqlQueryRaw<string>(@"
                    SELECT TABLE_NAME
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_NAME IN ('KpiAssignmentTables', 'KpiIndicators')
                ").ToListAsync();

                return Ok(new
                {
                    message = "KPI Schema verification",
                    existingTables = kpiTables,
                    allTablesExist = kpiTables.Count == 2
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "❌ Error verifying KPI schema",
                    error = ex.Message
                });
            }
        }
    }
}
