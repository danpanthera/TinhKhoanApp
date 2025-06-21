using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// Controller for database cleanup operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CleanupController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CleanupController> _logger;

        public CleanupController(ApplicationDbContext context, ILogger<CleanupController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Perform database cleanup as requested
        /// </summary>
        [HttpPost("execute")]
        public async Task<ActionResult> ExecuteCleanup()
        {
            try
            {
                var results = new Dictionary<string, object>();

                // 1. Delete units (departments) with ID >= 1026
                var unitsDeleted = await _context.Database.ExecuteSqlRawAsync(
                    "DELETE FROM Units WHERE Id >= 1026");
                results["unitsDeleted"] = unitsDeleted;
                
                _logger.LogInformation("Deleted {Count} units with ID >= 1026", unitsDeleted);

                // 2. Delete all positions and recreate standard ones using raw SQL
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Positions");
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT('Positions', RESEED, 0)");
                
                // Insert standard positions using raw SQL
                await _context.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO Positions (Name, Description) VALUES 
                    ('Giám đốc', 'Giám đốc công ty'),
                    ('Phó Giám đốc', 'Phó Giám đốc công ty'),
                    ('Trưởng phòng', 'Trưởng phòng ban'),
                    ('Phó trưởng phòng', 'Phó trưởng phòng ban'),
                    ('Giám đốc Phòng giao dịch', 'Giám đốc Phòng giao dịch'),
                    ('Phó giám đốc Phòng giao dịch', 'Phó giám đốc Phòng giao dịch'),
                    ('Nhân viên', 'Nhân viên')
                ");
                
                results["positionsCreated"] = 7;
                _logger.LogInformation("Created 7 standard positions");

                // 4. Delete roles with spaces in name
                var rolesWithSpacesDeleted = await _context.Database.ExecuteSqlRawAsync(
                    "DELETE FROM Roles WHERE Name LIKE '% %'");
                results["rolesWithSpacesDeleted"] = rolesWithSpacesDeleted;
                
                _logger.LogInformation("Deleted {Count} roles with spaces in name", rolesWithSpacesDeleted);

                // 5. Get verification counts
                var unitCount = await _context.Units.CountAsync();
                var positionCount = await _context.Positions.CountAsync();
                var roleCount = await _context.Roles.CountAsync();
                var rolesWithoutSpaces = await _context.Roles.CountAsync(r => !r.Name.Contains(" "));

                results["verification"] = new
                {
                    totalUnits = unitCount,
                    totalPositions = positionCount,
                    totalRoles = roleCount,
                    rolesWithoutSpaces = rolesWithoutSpaces
                };

                // 6. Get current positions and roles sample
                var currentPositions = await _context.Positions
                    .OrderBy(p => p.Id)
                    .Select(p => new { p.Id, p.Name, p.Description })
                    .ToListAsync();

                var remainingRoles = await _context.Roles
                    .Where(r => !r.Name.Contains(" "))
                    .OrderBy(r => r.Name)
                    .Take(10)
                    .Select(r => new { r.Id, r.Name })
                    .ToListAsync();

                results["currentPositions"] = currentPositions;
                results["remainingRoles"] = remainingRoles;

                return Ok(new
                {
                    success = true,
                    message = "Database cleanup completed successfully",
                    results = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during database cleanup");
                return StatusCode(500, new 
                { 
                    success = false, 
                    message = "Database cleanup failed", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Get current database status
        /// </summary>
        [HttpGet("status")]
        public async Task<ActionResult> GetCleanupStatus()
        {
            try
            {
                var status = new
                {
                    totalUnits = await _context.Units.CountAsync(),
                    unitsWithHighId = await _context.Units.CountAsync(d => d.Id >= 1026),
                    totalPositions = await _context.Positions.CountAsync(),
                    totalRoles = await _context.Roles.CountAsync(),
                    rolesWithSpaces = await _context.Roles.CountAsync(r => r.Name.Contains(" ")),
                    rolesWithoutSpaces = await _context.Roles.CountAsync(r => !r.Name.Contains(" ")),
                    positions = await _context.Positions
                        .OrderBy(p => p.Id)
                        .Select(p => new { p.Id, p.Name })
                        .ToListAsync(),
                    sampleRolesWithSpaces = await _context.Roles
                        .Where(r => r.Name.Contains(" "))
                        .Take(5)
                        .Select(r => new { r.Name })
                        .ToListAsync()
                };

                return Ok(new { success = true, status });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cleanup status");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Replace entire organizational structure with new standardized units
        /// </summary>
        [HttpPost("replace-organizational-structure")]
        public async Task<ActionResult> ReplaceOrganizationalStructure()
        {
            try
            {
                _logger.LogInformation("Starting organizational structure replacement...");

                // Use a single transaction with all commands combined
                var combinedSql = @"
                    -- Start transaction
                    BEGIN TRANSACTION;

                    -- Disable all triggers on Units table
                    DISABLE TRIGGER ALL ON Units;

                    -- Delete all existing units
                    DELETE FROM Units;

                    -- Enable IDENTITY_INSERT and insert new data in one statement
                    SET IDENTITY_INSERT Units ON;

                    INSERT INTO Units (Id, Code, Name, Type, ParentUnitId) VALUES
                    (1, 'CnLaiChau', N'Chi nhánh tỉnh Lai Châu', 'CNL1', NULL),
                    (2, 'CnLaiChauBgd', N'Ban Giám đốc', 'PNVL1', 1),
                    (3, 'CnLaiChauKhdn', N'Phòng Khách hàng Doanh nghiệp', 'PNVL1', 1),
                    (4, 'CnLaiChauKhcn', N'Phòng Khách hàng Cá nhân', 'PNVL1', 1),
                    (5, 'CnLaiChauKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL1', 1),
                    (6, 'CnLaiChauTonghop', N'Phòng Tổng hợp', 'PNVL1', 1),
                    (7, 'CnLaiChauKhqlrr', N'Phòng Kế hoạch & Quản lý rủi ro', 'PNVL1', 1),
                    (8, 'CnLaiChauKtgs', N'Phòng Kiểm tra giám sát', 'PNVL1', 1),
                    (9, 'CnTamDuong', N'Chi nhánh Tam Đường', 'CNL2', 1),
                    (10, 'CnTamDuongBgd', N'Ban Giám đốc', 'PNVL2', 9),
                    (11, 'CnTamDuongKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 9),
                    (12, 'CnTamDuongKh', N'Phòng Khách hàng', 'PNVL2', 9),
                    (13, 'CnPhongTho', N'Chi nhánh Phong Thổ', 'CNL2', 1),
                    (14, 'CnPhongThoBgd', N'Ban Giám đốc', 'PNVL2', 13),
                    (15, 'CnPhongThoKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 13),
                    (16, 'CnPhongThoKh', N'Phòng Khách hàng', 'PNVL2', 13),
                    (17, 'CnPhongThoPgdMuongSo', N'Phòng giao dịch Mường So', 'PGDL2', 13),
                    (18, 'CnSinHo', N'Chi nhánh Sìn Hồ', 'CNL2', 1),
                    (19, 'CnSinHoBgd', N'Ban Giám đốc', 'PNVL2', 18),
                    (20, 'CnSinHoKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 18),
                    (21, 'CnSinHoKh', N'Phòng Khách hàng', 'PNVL2', 18),
                    (22, 'CnMuongTe', N'Chi nhánh Mường Tè', 'CNL2', 1),
                    (23, 'CnMuongTeBgd', N'Ban Giám đốc', 'PNVL2', 22),
                    (24, 'CnMuongTeKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 22),
                    (25, 'CnMuongTeKh', N'Phòng Khách hàng', 'PNVL2', 22),
                    (26, 'CnThanUyen', N'Chi nhánh Than Uyên', 'CNL2', 1),
                    (27, 'CnThanUyenBgd', N'Ban Giám đốc', 'PNVL2', 26),
                    (28, 'CnThanUyenKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 26),
                    (29, 'CnThanUyenKh', N'Phòng Khách hàng', 'PNVL2', 26),
                    (30, 'CnThanUyenPgdMuongThan', N'Phòng giao dịch Mường Than', 'PGDL2', 26),
                    (31, 'CnThanhPho', N'Chi nhánh Thành Phố', 'CNL2', 1),
                    (32, 'CnThanhPhoBgd', N'Ban Giám đốc', 'PNVL2', 31),
                    (33, 'CnThanhPhoKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 31),
                    (34, 'CnThanhPhoKh', N'Phòng Khách hàng', 'PNVL2', 31),
                    (35, 'CnThanhPhoPgdso1', N'Phòng giao dịch số 1', 'PGDL2', 31),
                    (36, 'CnThanhPhoPgdso2', N'Phòng giao dịch số 2', 'PGDL2', 31),
                    (37, 'CnTanUyen', N'Chi nhánh Tân Uyên', 'CNL2', 1),
                    (38, 'CnTanUyenBgd', N'Ban Giám đốc', 'PNVL2', 37),
                    (39, 'CnTanUyenKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 37),
                    (40, 'CnTanUyenKh', N'Phòng Khách hàng', 'PNVL2', 37),
                    (41, 'CnTanUyenPgdso3', N'Phòng giao dịch số 3', 'PGDL2', 37),
                    (42, 'CnNamNhun', N'Chi nhánh Nậm Nhùn', 'CNL2', 1),
                    (43, 'CnNamNhunBgd', N'Ban Giám đốc', 'PNVL2', 42),
                    (44, 'CnNamNhunKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 42),
                    (45, 'CnNamNhunKh', N'Phòng Khách hàng', 'PNVL2', 42);

                    -- Disable IDENTITY_INSERT after insertion
                    SET IDENTITY_INSERT Units OFF;

                    -- Re-enable all triggers on Units table
                    ENABLE TRIGGER ALL ON Units;

                    -- Commit transaction
                    COMMIT TRANSACTION;";

                await _context.Database.ExecuteSqlRawAsync(combinedSql);

                _logger.LogInformation("Successfully inserted 45 new units with fixed organizational structure");

                // Verify the new structure
                var totalUnits = await _context.Units.CountAsync();
                var unitsByType = await _context.Units
                    .GroupBy(u => u.Type)
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    message = "Organizational structure replaced successfully with 45 fixed units. No new units can be added.",
                    totalUnits = totalUnits,
                    unitsByType = unitsByType,
                    details = new
                    {
                        expectedTotal = 45,
                        expectedTypes = new { CNL1 = 1, CNL2 = 8, PNVL1 = 7, PNVL2 = 24, PGDL2 = 5 },
                        warning = "The organizational structure is now locked to these 45 units only."
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during organizational structure replacement");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Organizational structure replacement failed",
                    error = ex.Message
                });
            }
        }
    }
}
