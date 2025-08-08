using TinhKhoanApp.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// API Controller quản lý quy tắc tính điểm KPI chi nhánh
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class KpiScoringRulesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public KpiScoringRulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả quy tắc tính điểm
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KpiScoringRule>>> GetKpiScoringRules()
        {
            try
            {
                var rules = await _context.KpiScoringRules
                    .Where(r => r.IsActive)
                    .OrderBy(r => r.KpiIndicatorName)
                    .ToListAsync();

                return Ok(rules);
            }
            catch (Exception ex)
            {
                // Log lỗi và trả về dữ liệu mặc định nếu bảng chưa tồn tại hoặc có lỗi
                Console.WriteLine($"KpiScoringRules error: {ex.Message}");
                
                var defaultRules = new List<KpiScoringRule>
                {
                    new KpiScoringRule 
                    { 
                        Id = 1, 
                        KpiIndicatorName = "Nguồn vốn huy động bình quân", 
                        RuleType = "COMPLETION_RATE",
                        MinValue = 80,
                        MaxValue = 120,
                        BonusPoints = 2.0m,
                        PenaltyPoints = -1.0m,
                        IsActive = true
                    },
                    new KpiScoringRule 
                    { 
                        Id = 2, 
                        KpiIndicatorName = "Dư nợ cho vay bình quân", 
                        RuleType = "COMPLETION_RATE",
                        MinValue = 85,
                        MaxValue = 115,
                        BonusPoints = 1.5m,
                        PenaltyPoints = -0.5m,
                        IsActive = true
                    }
                };
                
                return Ok(defaultRules);
            }
        }

        /// <summary>
        /// Lấy danh sách quy tắc theo loại đơn vị
        /// </summary>
        [HttpGet("by-unit-type/{unitType}")]
        public async Task<ActionResult<IEnumerable<KpiScoringRule>>> GetRulesByUnitType(string unitType)
        {
            var rules = await _context.KpiScoringRules
                .Where(r => r.IsActive)
                .OrderBy(r => r.KpiIndicatorName)
                .ToListAsync();

            return Ok(rules);
        }

        /// <summary>
        /// Tìm quy tắc cho một chỉ tiêu cụ thể
        /// </summary>
        [HttpGet("find")]
        public async Task<ActionResult<KpiScoringRule>> FindRule(string kpiName, string unitType = "ALL")
        {
            var rule = await _context.KpiScoringRules
                .Where(r => r.IsActive)
                .Where(r => r.KpiIndicatorName.Contains(kpiName))
                .FirstOrDefaultAsync();

            if (rule == null)
            {
                return NotFound($"Không tìm thấy quy tắc cho chỉ tiêu: {kpiName}");
            }

            return Ok(rule);
        }

        /// <summary>
        /// Lấy chi tiết một quy tắc
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<KpiScoringRule>> GetKpiScoringRule(int id)
        {
            var rule = await _context.KpiScoringRules.FindAsync(id);
            if (rule == null) 
            {
                return NotFound($"Không tìm thấy quy tắc với ID: {id}");
            }
            return Ok(rule);
        }

        /// <summary>
        /// Cập nhật quy tắc tính điểm
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKpiScoringRule(int id, KpiScoringRule rule)
        {
            if (id != rule.Id) 
            {
                return BadRequest("ID không khớp");
            }

            // Cập nhật thông tin chỉnh sửa
            rule.UpdatedDate = DateTime.Now;
            // rule.UpdatedBy có thể lấy từ JWT token

            _context.Entry(rule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Cập nhật quy tắc thành công", rule });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KpiScoringRuleExists(id)) 
                {
                    return NotFound($"Quy tắc với ID {id} không tồn tại");
                }
                throw;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi cập nhật quy tắc", error = ex.Message });
            }
        }

        /// <summary>
        /// Tạo quy tắc tính điểm mới
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<KpiScoringRule>> PostKpiScoringRule(KpiScoringRule rule)
        {
            try
            {
                // Kiểm tra trùng lặp
                var existingRule = await _context.KpiScoringRules
                    .Where(r => r.IsActive)
                    .Where(r => r.KpiIndicatorName == rule.KpiIndicatorName)
                    .FirstOrDefaultAsync();

                if (existingRule != null)
                {
                    return Conflict($"Đã tồn tại quy tắc cho chỉ tiêu '{rule.KpiIndicatorName}'");
                }

                // Thiết lập thông tin tạo mới
                rule.CreatedDate = DateTime.Now;
                // rule.CreatedBy có thể lấy từ JWT token

                _context.KpiScoringRules.Add(rule);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetKpiScoringRule), new { id = rule.Id }, rule);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi tạo quy tắc mới", error = ex.Message });
            }
        }

        /// <summary>
        /// Xóa quy tắc (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKpiScoringRule(int id)
        {
            try
            {
                var rule = await _context.KpiScoringRules.FindAsync(id);
                if (rule == null) 
                {
                    return NotFound($"Không tìm thấy quy tắc với ID: {id}");
                }

                // Soft delete
                rule.IsActive = false;
                rule.UpdatedDate = DateTime.Now;
                // rule.UpdatedBy có thể lấy từ JWT token

                await _context.SaveChangesAsync();

                return Ok(new { message = "Đã vô hiệu hóa quy tắc thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi xóa quy tắc", error = ex.Message });
            }
        }

        /// <summary>
        /// Kích hoạt lại quy tắc
        /// </summary>
        [HttpPost("{id}/activate")]
        public async Task<IActionResult> ActivateRule(int id)
        {
            try
            {
                var rule = await _context.KpiScoringRules.FindAsync(id);
                if (rule == null) 
                {
                    return NotFound($"Không tìm thấy quy tắc với ID: {id}");
                }

                rule.IsActive = true;
                rule.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Đã kích hoạt quy tắc thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi kích hoạt quy tắc", error = ex.Message });
            }
        }

        /// <summary>
        /// Seed dữ liệu mẫu
        /// </summary>
        [HttpPost("seed-data")]
        public IActionResult SeedData()
        {
            try
            {
                SeedKpiScoringRules.SeedKpiRules(_context);
                return Ok(new { message = "Đã seed dữ liệu quy tắc tính điểm thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi seed dữ liệu", error = ex.Message });
            }
        }

        /// <summary>
        /// Kiểm tra quy tắc có tồn tại không
        /// </summary>
        private bool KpiScoringRuleExists(int id)
        {
            return _context.KpiScoringRules.Any(e => e.Id == id);
        }
    }
}
