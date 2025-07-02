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
                    { "PgdSo2", "CnDoanKetPgdSo2" } // PGD số 2 → có mã CnDoanKetPgdSo2
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

        /// <summary>
        /// Cập nhật toàn diện đơn vị tính từ "Tỷ VND" thành "Triệu VND" trong tất cả bảng liên quan
        /// </summary>
        [HttpPost("update-unit-from-ty-to-trieu")]
        public async Task<ActionResult> UpdateUnitFromTyToTrieu()
        {
            try
            {
                _logger.LogInformation("🔄 Bắt đầu cập nhật đơn vị tính từ 'Tỷ VND' thành 'Triệu VND' trong toàn bộ hệ thống");

                var result = new
                {
                    kpiDefinitions = 0,
                    kpiIndicators = 0,
                    dashboardIndicators = 0,
                    businessPlanTargets = 0,
                    employeeKpiTargets = 0,
                    unitKhoanAssignmentDetails = 0,
                    employeeKhoanAssignmentDetails = 0,
                    kpiScoringRules = 0,
                    salaryParameters = 0
                };

                // 1. Cập nhật KPIDefinitions
                var kpiDefinitionsWithTy = await _context.KPIDefinitions
                    .Where(k => k.UnitOfMeasure != null &&
                               (k.UnitOfMeasure.Contains("Tỷ VND") ||
                                k.UnitOfMeasure.Contains("tỷ VND") ||
                                k.UnitOfMeasure.Contains("TỶ VND")))
                    .ToListAsync();

                foreach (var kpi in kpiDefinitionsWithTy)
                {
                    var oldUnit = kpi.UnitOfMeasure;
                    kpi.UnitOfMeasure = kpi.UnitOfMeasure?
                        .Replace("Tỷ VND", "Triệu VND")
                        .Replace("tỷ VND", "Triệu VND")
                        .Replace("TỶ VND", "Triệu VND");
                    result = result with { kpiDefinitions = result.kpiDefinitions + 1 };
                    _logger.LogInformation("✅ KPIDefinitions: {OldUnit} → {NewUnit}", oldUnit, kpi.UnitOfMeasure);
                }

                // 2. Cập nhật KpiIndicators
                var kpiIndicatorsWithTy = await _context.KpiIndicators
                    .Where(k => k.Unit != null &&
                               (k.Unit.Contains("Tỷ VND") ||
                                k.Unit.Contains("tỷ VND") ||
                                k.Unit.Contains("TỶ VND")))
                    .ToListAsync();

                foreach (var indicator in kpiIndicatorsWithTy)
                {
                    var oldUnit = indicator.Unit;
                    indicator.Unit = indicator.Unit
                        .Replace("Tỷ VND", "Triệu VND")
                        .Replace("tỷ VND", "Triệu VND")
                        .Replace("TỶ VND", "Triệu VND");
                    result = result with { kpiIndicators = result.kpiIndicators + 1 };
                    _logger.LogInformation("✅ KpiIndicators: {OldUnit} → {NewUnit}", oldUnit, indicator.Unit);
                }

                // 3. Cập nhật DashboardIndicators
                var dashboardIndicatorsWithTy = await _context.DashboardIndicators
                    .Where(d => d.Unit != null &&
                               (d.Unit.Contains("Tỷ VND") ||
                                d.Unit.Contains("tỷ VND") ||
                                d.Unit.Contains("TỶ VND")))
                    .ToListAsync();

                foreach (var dashboard in dashboardIndicatorsWithTy)
                {
                    var oldUnit = dashboard.Unit;
                    dashboard.Unit = dashboard.Unit?
                        .Replace("Tỷ VND", "Triệu VND")
                        .Replace("tỷ VND", "Triệu VND")
                        .Replace("TỶ VND", "Triệu VND");
                    result = result with { dashboardIndicators = result.dashboardIndicators + 1 };
                    _logger.LogInformation("✅ DashboardIndicators: {OldUnit} → {NewUnit}", oldUnit, dashboard.Unit);
                }

                // 4. Cập nhật BusinessPlanTargets notes nếu có chứa đơn vị
                var businessPlanTargetsWithTy = await _context.BusinessPlanTargets
                    .Where(b => b.Notes != null &&
                               (b.Notes.Contains("Tỷ VND") ||
                                b.Notes.Contains("tỷ VND") ||
                                b.Notes.Contains("TỶ VND")))
                    .ToListAsync();

                foreach (var target in businessPlanTargetsWithTy)
                {
                    var oldNotes = target.Notes;
                    target.Notes = target.Notes?
                        .Replace("Tỷ VND", "Triệu VND")
                        .Replace("tỷ VND", "Triệu VND")
                        .Replace("TỶ VND", "Triệu VND");
                    result = result with { businessPlanTargets = result.businessPlanTargets + 1 };
                    _logger.LogInformation("✅ BusinessPlanTargets Notes: {OldNotes} → {NewNotes}", oldNotes, target.Notes);
                }

                // 5. Cập nhật EmployeeKpiTargets notes
                var employeeKpiTargetsWithTy = await _context.EmployeeKpiTargets
                    .Where(e => e.Notes != null &&
                               (e.Notes.Contains("Tỷ VND") ||
                                e.Notes.Contains("tỷ VND") ||
                                e.Notes.Contains("TỶ VND")))
                    .ToListAsync();

                foreach (var empTarget in employeeKpiTargetsWithTy)
                {
                    var oldNotes = empTarget.Notes;
                    empTarget.Notes = empTarget.Notes?
                        .Replace("Tỷ VND", "Triệu VND")
                        .Replace("tỷ VND", "Triệu VND")
                        .Replace("TỶ VND", "Triệu VND");
                    result = result with { employeeKpiTargets = result.employeeKpiTargets + 1 };
                    _logger.LogInformation("✅ EmployeeKpiTargets Notes: {OldNotes} → {NewNotes}", oldNotes, empTarget.Notes);
                }

                // 6. Cập nhật UnitKhoanAssignmentDetails
                var unitKhoanDetailsWithTy = await _context.UnitKhoanAssignmentDetails
                    .Where(u => u.Note != null &&
                               (u.Note.Contains("Tỷ VND") ||
                                u.Note.Contains("tỷ VND") ||
                                u.Note.Contains("TỶ VND")))
                    .ToListAsync();

                foreach (var unitDetail in unitKhoanDetailsWithTy)
                {
                    var oldNote = unitDetail.Note;
                    unitDetail.Note = unitDetail.Note?
                        .Replace("Tỷ VND", "Triệu VND")
                        .Replace("tỷ VND", "Triệu VND")
                        .Replace("TỶ VND", "Triệu VND");
                    result = result with { unitKhoanAssignmentDetails = result.unitKhoanAssignmentDetails + 1 };
                    _logger.LogInformation("✅ UnitKhoanAssignmentDetails Note: {OldNote} → {NewNote}", oldNote, unitDetail.Note);
                }

                // 7. Cập nhật EmployeeKhoanAssignmentDetails
                var empKhoanDetailsWithTy = await _context.EmployeeKhoanAssignmentDetails
                    .Where(e => e.Note != null &&
                               (e.Note.Contains("Tỷ VND") ||
                                e.Note.Contains("tỷ VND") ||
                                e.Note.Contains("TỶ VND")))
                    .ToListAsync();

                foreach (var empDetail in empKhoanDetailsWithTy)
                {
                    var oldNote = empDetail.Note;
                    empDetail.Note = empDetail.Note?
                        .Replace("Tỷ VND", "Triệu VND")
                        .Replace("tỷ VND", "Triệu VND")
                        .Replace("TỶ VND", "Triệu VND");
                    result = result with { employeeKhoanAssignmentDetails = result.employeeKhoanAssignmentDetails + 1 };
                    _logger.LogInformation("✅ EmployeeKhoanAssignmentDetails Note: {OldNote} → {NewNote}", oldNote, empDetail.Note);
                }

                // 8. Cập nhật KpiScoringRules
                var kpiScoringRulesWithTy = await _context.KpiScoringRules
                    .Where(k => k.ScoreFormula != null &&
                               (k.ScoreFormula.Contains("Tỷ VND") ||
                                k.ScoreFormula.Contains("tỷ VND") ||
                                k.ScoreFormula.Contains("TỶ VND")))
                    .ToListAsync();

                foreach (var rule in kpiScoringRulesWithTy)
                {
                    var oldFormula = rule.ScoreFormula;
                    rule.ScoreFormula = rule.ScoreFormula?
                        .Replace("Tỷ VND", "Triệu VND")
                        .Replace("tỷ VND", "Triệu VND")
                        .Replace("TỶ VND", "Triệu VND");
                    result = result with { kpiScoringRules = result.kpiScoringRules + 1 };
                    _logger.LogInformation("✅ KpiScoringRules Formula: {OldFormula} → {NewFormula}", oldFormula, rule.ScoreFormula);
                }

                // 9. Cập nhật SalaryParameters
                var salaryParamsWithTy = await _context.SalaryParameters
                    .Where(s => s.Note != null &&
                               (s.Note.Contains("Tỷ VND") ||
                                s.Note.Contains("tỷ VND") ||
                                s.Note.Contains("TỶ VND")))
                    .ToListAsync();

                foreach (var param in salaryParamsWithTy)
                {
                    var oldNote = param.Note;
                    param.Note = param.Note?
                        .Replace("Tỷ VND", "Triệu VND")
                        .Replace("tỷ VND", "Triệu VND")
                        .Replace("TỶ VND", "Triệu VND");
                    result = result with { salaryParameters = result.salaryParameters + 1 };
                    _logger.LogInformation("✅ SalaryParameters Note: {OldNote} → {NewNote}", oldNote, param.Note);
                }

                // Lưu thay đổi
                var saveResult = await _context.SaveChangesAsync();

                var totalUpdated = result.kpiDefinitions + result.kpiIndicators + result.dashboardIndicators +
                                  result.businessPlanTargets + result.employeeKpiTargets + result.unitKhoanAssignmentDetails +
                                  result.employeeKhoanAssignmentDetails + result.kpiScoringRules + result.salaryParameters;

                return Ok(new
                {
                    success = true,
                    message = $"Cập nhật đơn vị tính từ 'Tỷ VND' thành 'Triệu VND' thành công",
                    summary = new
                    {
                        totalUpdatedRecords = totalUpdated,
                        databaseChanges = saveResult,
                        details = result
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi cập nhật đơn vị tính từ Tỷ VND thành Triệu VND");
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Kiểm tra toàn bộ hệ thống để tìm các trường chứa "Tỷ VND"
        /// </summary>
        [HttpGet("check-ty-vnd-usage")]
        public async Task<ActionResult> CheckTyVndUsage()
        {
            try
            {
                var result = new
                {
                    kpiDefinitions = await _context.KPIDefinitions
                        .Where(k => k.UnitOfMeasure != null &&
                                   (k.UnitOfMeasure.Contains("Tỷ VND") ||
                                    k.UnitOfMeasure.Contains("tỷ VND") ||
                                    k.UnitOfMeasure.Contains("TỶ VND")))
                        .Select(k => new { k.Id, k.KpiName, k.UnitOfMeasure })
                        .ToListAsync(),

                    kpiIndicators = await _context.KpiIndicators
                        .Where(k => k.Unit != null &&
                                   (k.Unit.Contains("Tỷ VND") ||
                                    k.Unit.Contains("tỷ VND") ||
                                    k.Unit.Contains("TỶ VND")))
                        .Select(k => new { k.Id, k.IndicatorName, k.Unit })
                        .ToListAsync(),

                    dashboardIndicators = await _context.DashboardIndicators
                        .Where(d => d.Unit != null &&
                                   (d.Unit.Contains("Tỷ VND") ||
                                    d.Unit.Contains("tỷ VND") ||
                                    d.Unit.Contains("TỶ VND")))
                        .Select(d => new { d.Id, d.Name, d.Unit })
                        .ToListAsync(),

                    businessPlanTargets = await _context.BusinessPlanTargets
                        .Where(b => b.Notes != null &&
                                   (b.Notes.Contains("Tỷ VND") ||
                                    b.Notes.Contains("tỷ VND") ||
                                    b.Notes.Contains("TỶ VND")))
                        .Select(b => new { b.Id, b.Notes })
                        .ToListAsync(),

                    employeeKpiTargets = await _context.EmployeeKpiTargets
                        .Where(e => e.Notes != null &&
                                   (e.Notes.Contains("Tỷ VND") ||
                                    e.Notes.Contains("tỷ VND") ||
                                    e.Notes.Contains("TỶ VND")))
                        .Select(e => new { e.Id, e.Notes })
                        .ToListAsync(),

                    unitKhoanAssignmentDetails = await _context.UnitKhoanAssignmentDetails
                        .Where(u => u.Note != null &&
                                   (u.Note.Contains("Tỷ VND") ||
                                    u.Note.Contains("tỷ VND") ||
                                    u.Note.Contains("TỶ VND")))
                        .Select(u => new { u.Id, u.Note })
                        .ToListAsync(),

                    employeeKhoanAssignmentDetails = await _context.EmployeeKhoanAssignmentDetails
                        .Where(e => e.Note != null &&
                                   (e.Note.Contains("Tỷ VND") ||
                                    e.Note.Contains("tỷ VND") ||
                                    e.Note.Contains("TỶ VND")))
                        .Select(e => new { e.Id, e.Note })
                        .ToListAsync(),

                    kpiScoringRules = await _context.KpiScoringRules
                        .Where(k => k.ScoreFormula != null &&
                                   (k.ScoreFormula.Contains("Tỷ VND") ||
                                    k.ScoreFormula.Contains("tỷ VND") ||
                                    k.ScoreFormula.Contains("TỶ VND")))
                        .Select(k => new { k.Id, k.KpiIndicatorName, k.ScoreFormula })
                        .ToListAsync(),

                    salaryParameters = await _context.SalaryParameters
                        .Where(s => s.Note != null &&
                                   (s.Note.Contains("Tỷ VND") ||
                                    s.Note.Contains("tỷ VND") ||
                                    s.Note.Contains("TỶ VND")))
                        .Select(s => new { s.Id, s.Name, s.Note })
                        .ToListAsync()
                };

                var totalCount = result.kpiDefinitions.Count + result.kpiIndicators.Count +
                               result.dashboardIndicators.Count + result.businessPlanTargets.Count +
                               result.employeeKpiTargets.Count + result.unitKhoanAssignmentDetails.Count +
                               result.employeeKhoanAssignmentDetails.Count + result.kpiScoringRules.Count +
                               result.salaryParameters.Count;

                return Ok(new
                {
                    success = true,
                    message = $"Tìm thấy {totalCount} bản ghi có chứa 'Tỷ VND'",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi kiểm tra usage của Tỷ VND");
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
    }
}
