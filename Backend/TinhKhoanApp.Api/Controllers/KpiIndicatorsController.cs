using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KpiIndicatorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<KpiIndicatorsController> _logger;

        public KpiIndicatorsController(ApplicationDbContext context, ILogger<KpiIndicatorsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/KpiIndicators
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KpiIndicator>>> GetKpiIndicators()
        {
            try
            {
                var indicators = await _context.KpiIndicators
                    .Include(k => k.Table)
                    .OrderBy(k => k.TableId)
                    .ThenBy(k => k.OrderIndex)
                    .ToListAsync();

                _logger.LogInformation($"✅ Returned {indicators.Count} KPI indicators");
                return Ok(indicators);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting KPI indicators");
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách chỉ tiêu KPI", error = ex.Message });
            }
        }

        // GET: api/KpiIndicators/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KpiIndicator>> GetKpiIndicator(int id)
        {
            try
            {
                var indicator = await _context.KpiIndicators
                    .Include(k => k.Table)
                    .FirstOrDefaultAsync(k => k.Id == id);

                if (indicator == null)
                {
                    return NotFound(new { message = $"Không tìm thấy chỉ tiêu KPI với ID {id}" });
                }

                return Ok(indicator);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error getting KPI indicator {id}");
                return StatusCode(500, new { message = "Lỗi khi lấy chỉ tiêu KPI", error = ex.Message });
            }
        }

        // GET: api/KpiIndicators/table/5
        [HttpGet("table/{tableId}")]
        public async Task<ActionResult<IEnumerable<KpiIndicator>>> GetIndicatorsByTable(int tableId)
        {
            try
            {
                // Kiểm tra table tồn tại
                var table = await _context.KpiAssignmentTables.FindAsync(tableId);
                if (table == null)
                {
                    return NotFound(new { message = $"Không tìm thấy bảng KPI với ID {tableId}" });
                }

                var indicators = await _context.KpiIndicators
                    .Where(k => k.TableId == tableId)
                    .OrderBy(k => k.OrderIndex)
                    .ToListAsync();

                _logger.LogInformation($"✅ Returned {indicators.Count} indicators for table {tableId}");
                return Ok(indicators);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error getting indicators for table {tableId}");
                return StatusCode(500, new { message = "Lỗi khi lấy chỉ tiêu theo bảng KPI", error = ex.Message });
            }
        }

        // POST: api/KpiIndicators
        [HttpPost]
        public async Task<ActionResult<KpiIndicator>> PostKpiIndicator([FromBody] KpiIndicator indicator)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Kiểm tra table tồn tại
                var table = await _context.KpiAssignmentTables.FindAsync(indicator.TableId);
                if (table == null)
                {
                    return BadRequest(new { message = $"Không tìm thấy bảng KPI với ID {indicator.TableId}" });
                }

                // Tự động tính OrderIndex nếu chưa có
                if (indicator.OrderIndex <= 0)
                {
                    var maxOrder = await _context.KpiIndicators
                        .Where(k => k.TableId == indicator.TableId)
                        .MaxAsync(k => (int?)k.OrderIndex) ?? 0;
                    indicator.OrderIndex = maxOrder + 1;
                }

                _context.KpiIndicators.Add(indicator);
                await _context.SaveChangesAsync();

                var created = await _context.KpiIndicators
                    .Include(k => k.Table)
                    .FirstOrDefaultAsync(k => k.Id == indicator.Id);

                _logger.LogInformation($"✅ Created KPI indicator: {indicator.IndicatorName}");
                return CreatedAtAction(nameof(GetKpiIndicator), new { id = indicator.Id }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error creating KPI indicator");
                return StatusCode(500, new { message = "Lỗi khi tạo chỉ tiêu KPI", error = ex.Message });
            }
        }

        // PUT: api/KpiIndicators/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKpiIndicator(int id, [FromBody] KpiIndicator indicator)
        {
            try
            {
                if (id != indicator.Id)
                {
                    return BadRequest(new { message = "ID không khớp" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Kiểm tra table tồn tại
                var table = await _context.KpiAssignmentTables.FindAsync(indicator.TableId);
                if (table == null)
                {
                    return BadRequest(new { message = $"Không tìm thấy bảng KPI với ID {indicator.TableId}" });
                }

                _context.Entry(indicator).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Updated KPI indicator: {indicator.IndicatorName}");
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await KpiIndicatorExists(id))
                {
                    return NotFound(new { message = $"Không tìm thấy chỉ tiêu KPI với ID {id}" });
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error updating KPI indicator {id}");
                return StatusCode(500, new { message = "Lỗi khi cập nhật chỉ tiêu KPI", error = ex.Message });
            }
        }

        // DELETE: api/KpiIndicators/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKpiIndicator(int id)
        {
            try
            {
                var indicator = await _context.KpiIndicators.FindAsync(id);
                if (indicator == null)
                {
                    return NotFound(new { message = $"Không tìm thấy chỉ tiêu KPI với ID {id}" });
                }

                // Kiểm tra xem có đang được sử dụng không
                var hasTargets = await _context.EmployeeKpiTargets.AnyAsync(t => t.IndicatorId == id);
                if (hasTargets)
                {
                    return BadRequest(new { message = "Không thể xóa chỉ tiêu này vì đã có dữ liệu giao khoán liên quan" });
                }

                _context.KpiIndicators.Remove(indicator);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Deleted KPI indicator: {indicator.IndicatorName}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error deleting KPI indicator {id}");
                return StatusCode(500, new { message = "Lỗi khi xóa chỉ tiêu KPI", error = ex.Message });
            }
        }

        // GET: api/KpiIndicators/summary
        [HttpGet("summary")]
        public async Task<ActionResult> GetIndicatorsSummary()
        {
            try
            {
                var summary = await _context.KpiIndicators
                    .Include(k => k.Table)
                    .GroupBy(k => k.Table.Category)
                    .Select(g => new
                    {
                        Category = g.Key,
                        TotalIndicators = g.Count(),
                        TotalTables = g.Select(k => k.TableId).Distinct().Count(),
                        AvgIndicatorsPerTable = g.Count() / (double)g.Select(k => k.TableId).Distinct().Count()
                    })
                    .ToListAsync();

                var totalIndicators = await _context.KpiIndicators.CountAsync();
                var totalTables = await _context.KpiAssignmentTables.CountAsync();

                return Ok(new
                {
                    TotalIndicators = totalIndicators,
                    TotalTables = totalTables,
                    CategoryBreakdown = summary,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting indicators summary");
                return StatusCode(500, new { message = "Lỗi khi lấy tổng quan chỉ tiêu KPI", error = ex.Message });
            }
        }

        private async Task<bool> KpiIndicatorExists(int id)
        {
            return await _context.KpiIndicators.AnyAsync(e => e.Id == id);
        }
    }
}
