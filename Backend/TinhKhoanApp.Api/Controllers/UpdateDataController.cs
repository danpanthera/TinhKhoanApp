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

                int updatedUnits = 0;
                int updatedKpiTables = 0;

                // 1. Cập nhật bảng Units
                foreach (var mapping in branchNameMapping)
                {
                    var units = await _context.Units
                        .Where(u => u.Name == mapping.Key)
                        .ToListAsync();

                    foreach (var unit in units)
                    {
                        unit.Name = mapping.Value;
                        updatedUnits++;
                        _logger.LogInformation("✅ Updated Unit: {OldName} → {NewName}", mapping.Key, mapping.Value);
                    }
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
                var kpiDefinitionsWithTyVnd = await _context.KpiDefinitions
                    .Where(k => k.Unit.Contains("Tỷ VND") || k.Unit.Contains("tỷ VND"))
                    .ToListAsync();

                int updatedKpiDefinitions = 0;
                foreach (var kpi in kpiDefinitionsWithTyVnd)
                {
                    var oldUnit = kpi.Unit;
                    kpi.Unit = kpi.Unit.Replace("Tỷ VND", "Triệu VND").Replace("tỷ VND", "Triệu VND");
                    updatedKpiDefinitions++;
                    _logger.LogInformation("✅ Updated KpiDefinition Unit: {OldUnit} → {NewUnit}", oldUnit, kpi.Unit);
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
                        updatedKpiTables = updatedKpiTables,
                        updatedKpiDefinitions = updatedKpiDefinitions,
                        totalChanges = saveResult
                    },
                    mappings = branchNameMapping
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

                var kpiDefinitionsUnits = await _context.KpiDefinitions
                    .Where(k => k.Unit.Contains("VND"))
                    .Select(k => new { k.Name, k.Unit })
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
    }
}
