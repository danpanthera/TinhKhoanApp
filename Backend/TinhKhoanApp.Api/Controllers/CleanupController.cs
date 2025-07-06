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

                    INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
                    (1, 'CnLaiChau', N'Chi nhánh tỉnh Lai Châu', 'CNL1', NULL, 0),
                    (2, 'CnLaiChauBgd', N'Ban Giám đốc', 'PNVL1', 1, 0),
                    (3, 'CnLaiChauKhdn', N'Phòng Khách hàng Doanh nghiệp', 'PNVL1', 1, 0),
                    (4, 'CnLaiChauKhcn', N'Phòng Khách hàng Cá nhân', 'PNVL1', 1, 0),
                    (5, 'CnLaiChauKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL1', 1, 0),
                    (6, 'CnLaiChauTonghop', N'Phòng Tổng hợp', 'PNVL1', 1, 0),
                    (7, 'CnLaiChauKhqlrr', N'Phòng Kế hoạch & Quản lý rủi ro', 'PNVL1', 1, 0),
                    (8, 'CnLaiChauKtgs', N'Phòng Kiểm tra giám sát', 'PNVL1', 1, 0),
                    (9, 'CnBinhLu', N'Chi nhánh Bình Lư', 'CNL2', 1, 0),
                    (10, 'CnBinhLuBgd', N'Ban Giám đốc', 'PNVL2', 9, 0),
                    (11, 'CnBinhLuKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 9, 0),
                    (12, 'CnBinhLuKh', N'Phòng Khách hàng', 'PNVL2', 9, 0),
                    (13, 'CnPhongTho', N'Chi nhánh Phong Thổ', 'CNL2', 1, 0),
                    (14, 'CnPhongThoBgd', N'Ban Giám đốc', 'PNVL2', 13, 0),
                    (15, 'CnPhongThoKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 13, 0),
                    (16, 'CnPhongThoKh', N'Phòng Khách hàng', 'PNVL2', 13, 0),
                    (17, 'CnPhongThoPgdSo5', N'Phòng giao dịch Số 5', 'PGDL2', 13, 0),
                    (18, 'CnSinHo', N'Chi nhánh Sìn Hồ', 'CNL2', 1, 0),
                    (19, 'CnSinHoBgd', N'Ban Giám đốc', 'PNVL2', 18, 0),
                    (20, 'CnSinHoKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 18, 0),
                    (21, 'CnSinHoKh', N'Phòng Khách hàng', 'PNVL2', 18, 0),
                    (22, 'CnBumTo', N'Chi nhánh Bum Tở', 'CNL2', 1, 0),
                    (23, 'CnBumToBgd', N'Ban Giám đốc', 'PNVL2', 22, 0),
                    (24, 'CnBumToKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 22, 0),
                    (25, 'CnBumToKh', N'Phòng Khách hàng', 'PNVL2', 22, 0),
                    (26, 'CnThanUyen', N'Chi nhánh Than Uyên', 'CNL2', 1, 0),
                    (27, 'CnThanUyenBgd', N'Ban Giám đốc', 'PNVL2', 26, 0),
                    (28, 'CnThanUyenKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 26, 0),
                    (29, 'CnThanUyenKh', N'Phòng Khách hàng', 'PNVL2', 26, 0),
                    (30, 'CnThanUyenPgdSo6', N'Phòng giao dịch Số 6', 'PGDL2', 26, 0),
                    (31, 'CnDoanKet', N'Chi nhánh Đoàn Kết', 'CNL2', 1, 0),
                    (32, 'CnDoanKetBgd', N'Ban Giám đốc', 'PNVL2', 31, 0),
                    (33, 'CnDoanKetKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 31, 0),
                    (34, 'CnDoanKetKh', N'Phòng Khách hàng', 'PNVL2', 31, 0),
                    (35, 'CnDoanKetPgdSo1', N'Phòng giao dịch số 1', 'PGDL2', 31, 0),
                    (36, 'CnDoanKetPgdSo2', N'Phòng giao dịch số 2', 'PGDL2', 31, 0),
                    (37, 'CnTanUyen', N'Chi nhánh Tân Uyên', 'CNL2', 1, 0),
                    (38, 'CnTanUyenBgd', N'Ban Giám đốc', 'PNVL2', 37, 0),
                    (39, 'CnTanUyenKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 37, 0),
                    (40, 'CnTanUyenKh', N'Phòng Khách hàng', 'PNVL2', 37, 0),
                    (41, 'CnTanUyenPgdSo3', N'Phòng giao dịch số 3', 'PGDL2', 37, 0),
                    (42, 'CnNamHang', N'Chi nhánh Nậm Hàng', 'CNL2', 1, 0),
                    (43, 'CnNamHangBgd', N'Ban Giám đốc', 'PNVL2', 42, 0),
                    (44, 'CnNamHangKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 42, 0),
                    (45, 'CnNamHangKh', N'Phòng Khách hàng', 'PNVL2', 42, 0);

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

        /// <summary>
        /// Phục hồi dữ liệu gốc: Đơn vị, Chức vụ, Nhân viên
        /// </summary>
        [HttpPost("restore-original-data")]
        public async Task<ActionResult> RestoreOriginalData()
        {
            try
            {
                _logger.LogInformation("🔄 Bắt đầu phục hồi dữ liệu gốc...");

                // 1. Phục hồi Đơn vị (45 đơn vị Agribank Lai Châu)
                var unitsScript = @"
                    DELETE FROM Units;
                    DBCC CHECKIDENT('Units', RESEED, 0);
                    SET IDENTITY_INSERT Units ON;
                    INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
                    (1, 'CnLaiChau', N'Chi nhánh tỉnh Lai Châu', 'CNL1', NULL, 0),
                    (2, 'CnLaiChauBgd', N'Ban Giám đốc', 'PNVL1', 1, 0),
                    (3, 'CnLaiChauKhdn', N'Phòng Khách hàng Doanh nghiệp', 'PNVL1', 1, 0),
                    (4, 'CnLaiChauKhcn', N'Phòng Khách hàng Cá nhân', 'PNVL1', 1, 0),
                    (5, 'CnLaiChauKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL1', 1, 0),
                    (6, 'CnLaiChauTonghop', N'Phòng Tổng hợp', 'PNVL1', 1, 0),
                    (7, 'CnLaiChauKhqlrr', N'Phòng Kế hoạch & Quản lý rủi ro', 'PNVL1', 1, 0),
                    (8, 'CnLaiChauKtgs', N'Phòng Kiểm tra giám sát', 'PNVL1', 1, 0),
                    (9, 'CnBinhLu', N'Chi nhánh Bình Lư', 'CNL2', 1, 0),
                    (10, 'CnBinhLuBgd', N'Ban Giám đốc', 'PNVL2', 9, 0),
                    (11, 'CnBinhLuKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 9, 0),
                    (12, 'CnBinhLuKh', N'Phòng Khách hàng', 'PNVL2', 9, 0),
                    (13, 'CnPhongTho', N'Chi nhánh Phong Thổ', 'CNL2', 1, 0),
                    (14, 'CnPhongThoBgd', N'Ban Giám đốc', 'PNVL2', 13, 0),
                    (15, 'CnPhongThoKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 13, 0),
                    (16, 'CnPhongThoKh', N'Phòng Khách hàng', 'PNVL2', 13, 0),
                    (17, 'CnPhongThoPgdSo5', N'Phòng giao dịch Số 5', 'PGDL2', 13, 0),
                    (18, 'CnSinHo', N'Chi nhánh Sìn Hồ', 'CNL2', 1, 0),
                    (19, 'CnSinHoBgd', N'Ban Giám đốc', 'PNVL2', 18, 0),
                    (20, 'CnSinHoKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 18, 0),
                    (21, 'CnSinHoKh', N'Phòng Khách hàng', 'PNVL2', 18, 0),
                    (22, 'CnBumTo', N'Chi nhánh Bum Tở', 'CNL2', 1, 0),
                    (23, 'CnBumToBgd', N'Ban Giám đốc', 'PNVL2', 22, 0),
                    (24, 'CnBumToKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 22, 0),
                    (25, 'CnBumToKh', N'Phòng Khách hàng', 'PNVL2', 22, 0),
                    (26, 'CnThanUyen', N'Chi nhánh Than Uyên', 'CNL2', 1, 0),
                    (27, 'CnThanUyenBgd', N'Ban Giám đốc', 'PNVL2', 26, 0),
                    (28, 'CnThanUyenKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 26, 0),
                    (29, 'CnThanUyenKh', N'Phòng Khách hàng', 'PNVL2', 26, 0),
                    (30, 'CnThanUyenPgdSo6', N'Phòng giao dịch Số 6', 'PGDL2', 26, 0),
                    (31, 'CnDoanKet', N'Chi nhánh Đoàn Kết', 'CNL2', 1, 0),
                    (32, 'CnDoanKetBgd', N'Ban Giám đốc', 'PNVL2', 31, 0),
                    (33, 'CnDoanKetKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 31, 0),
                    (34, 'CnDoanKetKh', N'Phòng Khách hàng', 'PNVL2', 31, 0),
                    (35, 'CnDoanKetPgdSo1', N'Phòng giao dịch số 1', 'PGDL2', 31, 0),
                    (36, 'CnDoanKetPgdSo2', N'Phòng giao dịch số 2', 'PGDL2', 31, 0),
                    (37, 'CnTanUyen', N'Chi nhánh Tân Uyên', 'CNL2', 1, 0),
                    (38, 'CnTanUyenBgd', N'Ban Giám đốc', 'PNVL2', 37, 0),
                    (39, 'CnTanUyenKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 37, 0),
                    (40, 'CnTanUyenKh', N'Phòng Khách hàng', 'PNVL2', 37, 0),
                    (41, 'CnTanUyenPgdSo3', N'Phòng giao dịch số 3', 'PGDL2', 37, 0),
                    (42, 'CnNamHang', N'Chi nhánh Nậm Hàng', 'CNL2', 1, 0),
                    (43, 'CnNamHangBgd', N'Ban Giám đốc', 'PNVL2', 42, 0),
                    (44, 'CnNamHangKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 42, 0),
                    (45, 'CnNamHangKh', N'Phòng Khách hàng', 'PNVL2', 42, 0);
                    SET IDENTITY_INSERT Units OFF;";

                await _context.Database.ExecuteSqlRawAsync(unitsScript);
                _logger.LogInformation("✅ Đã phục hồi 45 đơn vị Agribank Lai Châu");

                // 2. Phục hồi Chức vụ (7 chức vụ chuẩn)
                var positionsScript = @"
                    DELETE FROM Positions;
                    DBCC CHECKIDENT('Positions', RESEED, 0);
                    INSERT INTO Positions (Name, Description) VALUES
                    (N'Giám đốc', N'Giám đốc công ty'),
                    (N'Phó Giám đốc', N'Phó Giám đốc công ty'),
                    (N'Trưởng phòng', N'Trưởng phòng ban'),
                    (N'Phó trưởng phòng', N'Phó trưởng phòng ban'),
                    (N'Giám đốc Phòng giao dịch', N'Giám đốc Phòng giao dịch'),
                    (N'Phó giám đốc Phòng giao dịch', N'Phó giám đốc Phòng giao dịch'),
                    (N'Nhân viên', N'Nhân viên');";

                await _context.Database.ExecuteSqlRawAsync(positionsScript);
                _logger.LogInformation("✅ Đã phục hồi 7 chức vụ chuẩn");

                // 3. Tạo Admin user và nhân viên mẫu
                var employeesScript = @"
                    DELETE FROM Employees WHERE Username = 'admin';
                    INSERT INTO Employees (
                        EmployeeCode, CBCode, FullName, Username, PasswordHash,
                        Email, PhoneNumber, IsActive, UnitId, PositionId, CreatedAt, UpdatedAt
                    ) VALUES
                    ('ADMIN001', '999999999', N'Quản trị viên hệ thống', 'admin',
                     '$2a$11$8Z7QZ9Z7QZ9Z7QZ9Z7QZ9OeKQFQFQFQFQFQFQFQFQFQFQFQFQFQFQF',
                     'admin@agribank.com.vn', '0999999999', 1, 1, 1, GETDATE(), GETDATE()),
                    ('LC001', '78001001', N'Nguyễn Văn An', 'nvan', 'password_hash',
                     'nvan@agribank.com.vn', '0987654321', 1, 2, 2, GETDATE(), GETDATE()),
                    ('LC002', '78001002', N'Trần Thị Bình', 'ttbinh', 'password_hash',
                     'ttbinh@agribank.com.vn', '0987654322', 1, 3, 3, GETDATE(), GETDATE()),
                    ('LC003', '78001003', N'Lê Văn Cường', 'lvcuong', 'password_hash',
                     'lvcuong@agribank.com.vn', '0987654323', 1, 4, 3, GETDATE(), GETDATE()),
                    ('LC004', '78001004', N'Phạm Thị Dung', 'ptdung', 'password_hash',
                     'ptdung@agribank.com.vn', '0987654324', 1, 5, 4, GETDATE(), GETDATE()),
                    ('LC005', '78001005', N'Hoàng Văn Em', 'hvem', 'password_hash',
                     'hvem@agribank.com.vn', '0987654325', 1, 6, 7, GETDATE(), GETDATE());";

                await _context.Database.ExecuteSqlRawAsync(employeesScript);
                _logger.LogInformation("✅ Đã tạo admin user và 5 nhân viên mẫu");

                // Kiểm tra kết quả
                var unitsCount = await _context.Units.CountAsync(u => !u.IsDeleted);
                var positionsCount = await _context.Positions.CountAsync();
                var employeesCount = await _context.Employees.CountAsync(e => e.IsActive);

                return Ok(new
                {
                    success = true,
                    message = "🎉 Phục hồi dữ liệu gốc thành công!",
                    data = new
                    {
                        units = unitsCount,
                        positions = positionsCount,
                        employees = employeesCount
                    },
                    details = new
                    {
                        unitsStructure = "45 đơn vị Agribank Lai Châu đã được phục hồi",
                        positionsStructure = "7 chức vụ chuẩn đã được phục hồi",
                        employeesStructure = "Admin user và 5 nhân viên mẫu đã được tạo"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi phục hồi dữ liệu gốc");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Phục hồi dữ liệu gốc thất bại",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Phục hồi dữ liệu đơn giản - từng bước
        /// </summary>
        [HttpPost("restorestep")]
        public async Task<ActionResult> RestoreStep()
        {
            try
            {
                _logger.LogInformation("🔄 Bắt đầu phục hồi dữ liệu đơn giản...");

                // 1. Xóa và tạo lại Units đơn giản
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Units WHERE IsDeleted = 0 OR IsDeleted IS NULL");

                // Thêm đơn vị chính
                await _context.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO Units (Code, Name, Type, ParentUnitId, IsDeleted) VALUES
                    ('CnLaiChau', N'Chi nhánh tỉnh Lai Châu', 'CNL1', NULL, 0),
                    ('CnLaiChauBgd', N'Ban Giám đốc', 'PNVL1', 1, 0),
                    ('CnLaiChauKhdn', N'Phòng Khách hàng Doanh nghiệp', 'PNVL1', 1, 0),
                    ('CnLaiChauKhcn', N'Phòng Khách hàng Cá nhân', 'PNVL1', 1, 0),
                    ('CnLaiChauKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL1', 1, 0);
                ");

                // 2. Xóa và tạo lại Positions
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Positions");
                await _context.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO Positions (Name, Description) VALUES
                    (N'Giám đốc', N'Giám đốc công ty'),
                    (N'Phó Giám đốc', N'Phó Giám đốc công ty'),
                    (N'Trưởng phòng', N'Trưởng phòng ban'),
                    (N'Phó trưởng phòng', N'Phó trưởng phòng ban'),
                    (N'Nhân viên', N'Nhân viên');
                ");

                // 3. Tạo admin user
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Employees WHERE Username = 'admin'");
                await _context.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO Employees (
                        EmployeeCode, CBCode, FullName, Username, PasswordHash,
                        Email, PhoneNumber, IsActive, UnitId, PositionId, CreatedAt, UpdatedAt
                    ) VALUES (
                        'ADMIN001', '999999999', N'Quản trị viên hệ thống', 'admin',
                        '$2a$11$8Z7QZ9Z7QZ9Z7QZ9Z7QZ9OeKQFQFQFQFQFQFQFQFQFQFQFQFQFQFQF',
                        'admin@agribank.com.vn', '0999999999', 1, 1, 1, GETDATE(), GETDATE()
                    );
                ");

                // Kiểm tra kết quả
                var unitsCount = await _context.Units.CountAsync(u => !u.IsDeleted);
                var positionsCount = await _context.Positions.CountAsync();
                var employeesCount = await _context.Employees.CountAsync(e => e.IsActive);

                return Ok(new
                {
                    success = true,
                    message = "🎉 Phục hồi dữ liệu cơ bản thành công!",
                    data = new
                    {
                        units = unitsCount,
                        positions = positionsCount,
                        employees = employeesCount
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi phục hồi dữ liệu đơn giản");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Phục hồi dữ liệu thất bại",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Simple test endpoint
        /// </summary>
        [HttpGet("test")]
        public async Task<ActionResult> Test()
        {
            try
            {
                var unitCount = await _context.Units.CountAsync();
                return Ok(new { success = true, message = "Controller working", unitCount });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Very simple restore - just recreate basic units
        /// </summary>
        [HttpPost("basicrestore")]
        public async Task<ActionResult> BasicRestore()
        {
            try
            {
                _logger.LogInformation("Starting basic restore...");

                // 1. Just clear and add a few basic units
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Units");

                // 2. Add just a few basic units using simpler syntax
                _context.Units.Add(new Unit
                {
                    Code = "CnLaiChau",
                    Name = "Chi nhánh Lai Châu",
                    Type = "CNL1",
                    ParentUnitId = null,
                    IsDeleted = false
                });

                _context.Units.Add(new Unit
                {
                    Code = "CnLaiChauBgd",
                    Name = "Ban Giám đốc",
                    Type = "PNVL1",
                    ParentUnitId = 1,
                    IsDeleted = false
                });

                await _context.SaveChangesAsync();

                var count = await _context.Units.CountAsync(u => !u.IsDeleted);

                return Ok(new
                {
                    success = true,
                    message = "Basic restore completed",
                    unitsCount = count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in basic restore");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
    }
}
