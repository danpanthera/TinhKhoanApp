using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UpdateDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateDataController> _logger;

        public UpdateDataController(ApplicationDbContext context, ILogger<UpdateDataController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Cập nhật tên chi nhánh trong database theo mapping mới
        /// </summary>
        [HttpPost("update-branch-names")]
        public async Task<ActionResult> UpdateBranchNames()
        {
            try
            {
                _logger.LogInformation("🔄 Bắt đầu cập nhật tên chi nhánh trong database");

                // Mapping tên mới
                var branchNameMapping = new Dictionary<string, string>
                {
                    { "Chi nhánh Tam Đường", "Chi nhánh Bình Lư" },
                    { "Chi nhánh Mường Tè", "Chi nhánh Bum Tở" },
                    { "Chi nhánh Thành Phố", "Chi nhánh Đoàn Kết" },
                    { "Chi nhánh Thành phố", "Chi nhánh Đoàn Kết" }, // Variant
                    { "Chi nhánh Nậm Nhùn", "Chi nhánh Nậm Hàng" }
                };

                // Mapping cho PGD và code units
                var unitCodeMapping = new Dictionary<string, string>
                {
                    { "PgdMuongSo", "CnPhongThoPgdSo5" }, // Phòng giao dịch Mường So → PGD Số 5
                    { "PgdMuongThan", "CnThanUyenPgdSo6" }, // Phòng giao dịch Mường Than → PGD Số 6
                    { "PgdSo1", "CnDoanKetPgdSo1" }, // PGD số 1 → có mã CnDoanKetPgdSo1
                    { "PgdSo2", "CnDoanKetPgdSo2" ` } // PGD số 2 → có mã CnDoanKetPgdSo2
                };

                var unitNameMapping = new Dictionary<string, string>
                {
                    { "Phòng giao dịch Mường So", "PGD Số 5" },
                    { "Phòng giao dịch Mường Than", "PGD Số 6" }
                };

                int updatedUnits = 0;
                int updatedKpiTables = 0;
                int updatedUnitCodes = 0;

                // 1. Cập nhật bảng Units - tên chi nhánh
                foreach (var mapping in branchNameMapping)
                {
                    var units = await _context.Units
                        .Where(u => u.Name == mapping.Key)
                        .ToListAsync();

                    foreach (var unit in units)
                    {
                        unit.Name = mapping.Value;
                        updatedUnits++;
                        _logger.LogInformation("✅ Updated Unit Name: {OldName} → {NewName}", mapping.Key, mapping.Value);
                    }
                }

                // 1.1 Cập nhật Unit Codes cho PGD
                foreach (var mapping in unitCodeMapping)
                {
                    var units = await _context.Units
                        .Where(u => u.Code == mapping.Key)
                        .ToListAsync();

                    foreach (var unit in units)
                    {
                        unit.Code = mapping.Value;
                        updatedUnitCodes++;
                        _logger.LogInformation("✅ Updated Unit Code: {OldCode} → {NewCode}", mapping.Key, mapping.Value);
                    }
                }

                // 1.2 Cập nhật Unit Names cho PGD
                foreach (var mapping in unitNameMapping)
                {
                    var units = await _context.Units
                        .Where(u => u.Name == mapping.Key)
                        .ToListAsync();

                    foreach (var unit in units)
                    {
                        unit.Name = mapping.Value;
                        updatedUnits++;
                        _logger.LogInformation("✅ Updated Unit Name: {OldName} → {NewName}", mapping.Key, mapping.Value);
                    }
                }

                // 1.3 Cập nhật trực tiếp PGD bằng ID (fallback nếu name matching không work)
                try
                {
                    var pgdMuongSo = await _context.Units.FindAsync(20); // ID 20 = Phòng giao dịch Mường So
                    if (pgdMuongSo != null && pgdMuongSo.Name.Contains("Mường So"))
                    {
                        pgdMuongSo.Name = "PGD Số 5";
                        updatedUnits++;
                        _logger.LogInformation("✅ Direct update ID 20: Phòng giao dịch Mường So → PGD Số 5");
                    }

                    var pgdMuongThan = await _context.Units.FindAsync(27); // ID 27 = Phòng giao dịch Mường Than
                    if (pgdMuongThan != null && pgdMuongThan.Name.Contains("Mường Than"))
                    {
                        pgdMuongThan.Name = "PGD Số 6";
                        updatedUnits++;
                        _logger.LogInformation("✅ Direct update ID 27: Phòng giao dịch Mường Than → PGD Số 6");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "⚠️ Không thể update trực tiếp PGD");
                }

                // 2. Cập nhật bảng KpiAssignmentTables
                foreach (var mapping in branchNameMapping)
                {
                    var kpiTables = await _context.KpiAssignmentTables
                        .Where(k => k.TableName == mapping.Key)
                        .ToListAsync();

                    foreach (var kpiTable in kpiTables)
                    {
                        kpiTable.TableName = mapping.Value;
                        updatedKpiTables++;
                        _logger.LogInformation("✅ Updated KpiAssignmentTable: {OldName} → {NewName}", mapping.Key, mapping.Value);
                    }
                }

                // 3. Cập nhật đơn vị "Tỷ VND" → "Triệu VND" trong KPI definitions nếu có
                var kpiDefinitionsWithTyVnd = await _context.KPIDefinitions
                    .Where(k => k.UnitOfMeasure != null && (k.UnitOfMeasure.Contains("Tỷ VND") || k.UnitOfMeasure.Contains("tỷ VND")))
                    .ToListAsync();

                int updatedKpiDefinitions = 0;
                foreach (var kpi in kpiDefinitionsWithTyVnd)
                {
                    var oldUnit = kpi.UnitOfMeasure;
                    kpi.UnitOfMeasure = kpi.UnitOfMeasure?.Replace("Tỷ VND", "Triệu VND").Replace("tỷ VND", "Triệu VND");
                    updatedKpiDefinitions++;
                    _logger.LogInformation("✅ Updated KpiDefinition Unit: {OldUnit} → {NewUnit}", oldUnit, kpi.UnitOfMeasure);
                }

                // Lưu thay đổi
                var saveResult = await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật tên chi nhánh và đơn vị tính thành công",
                    summary = new
                    {
                        updatedUnits = updatedUnits,
                        updatedUnitCodes = updatedUnitCodes,
                        updatedKpiTables = updatedKpiTables,
                        updatedKpiDefinitions = updatedKpiDefinitions,
                        totalChanges = saveResult
                    },
                    mappings = new { branchNameMapping, unitCodeMapping, unitNameMapping }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi cập nhật tên chi nhánh");
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Kiểm tra tên chi nhánh hiện tại trong database
        /// </summary>
        [HttpGet("check-branch-names")]
        public async Task<ActionResult> CheckBranchNames()
        {
            try
            {
                var units = await _context.Units
                    .Where(u => u.Type == "CNL2")
                    .Select(u => new { u.Code, u.Name, u.Type })
                    .ToListAsync();

                var kpiTables = await _context.KpiAssignmentTables
                    .Where(k => k.TableName.Contains("Chi nhánh"))
                    .Select(k => new { k.TableType, k.TableName })
                    .ToListAsync();

                var kpiDefinitionsUnits = await _context.KPIDefinitions
                    .Where(k => k.UnitOfMeasure != null && k.UnitOfMeasure.Contains("VND"))
                    .Select(k => new { k.KpiName, k.UnitOfMeasure })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    units = units,
                    kpiTables = kpiTables,
                    kpiDefinitionsWithVnd = kpiDefinitionsUnits
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi kiểm tra tên chi nhánh");
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật tên PGD cụ thể theo yêu cầu mới
        /// </summary>
        [HttpPost("update-pgd-names")]
        public async Task<ActionResult> UpdatePgdNames()
        {
            try
            {
                _logger.LogInformation("🔄 Bắt đầu cập nhật tên PGD trong database");

                int updatedCount = 0;

                // Cập nhật Phòng giao dịch Mường So → PGD Số 5
                var pgdMuongSo = await _context.Units
                    .Where(u => u.Name == "Phòng giao dịch Mường So")
                    .ToListAsync();

                foreach (var unit in pgdMuongSo)
                {
                    unit.Name = "PGD Số 5";
                    updatedCount++;
                    _logger.LogInformation("✅ Updated: {OldName} → {NewName}", "Phòng giao dịch Mường So", "PGD Số 5");
                }

                // Cập nhật Phòng giao dịch Mường Than → PGD Số 6
                var pgdMuongThan = await _context.Units
                    .Where(u => u.Name == "Phòng giao dịch Mường Than")
                    .ToListAsync();

                foreach (var unit in pgdMuongThan)
                {
                    unit.Name = "PGD Số 6";
                    updatedCount++;
                    _logger.LogInformation("✅ Updated: {OldName} → {NewName}", "Phòng giao dịch Mường Than", "PGD Số 6");
                }

                // Lưu thay đổi
                var saveResult = await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật tên PGD thành công",
                    summary = new
                    {
                        updatedUnits = updatedCount,
                        totalChanges = saveResult
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi cập nhật tên PGD");
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
    }
}
