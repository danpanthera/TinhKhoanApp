using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KPIDefinitionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public KPIDefinitionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/KPIDefinitions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KPIDefinition>>> GetKPIDefinitions([FromQuery] bool includeInactive = false)
        {
            if (includeInactive)
            {
                return await _context.KPIDefinitions.OrderBy(k => k.KpiCode).ThenBy(k => k.Version).ToListAsync();
            }
            return await _context.KPIDefinitions.Where(k => k.IsActive).OrderBy(k => k.KpiCode).ThenBy(k => k.Version).ToListAsync();
        }

        // GET: api/KPIDefinitions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KPIDefinition>> GetKPIDefinition(int id)
        {
            var kpiDefinition = await _context.KPIDefinitions.FindAsync(id);

            if (kpiDefinition == null)
            {
                return NotFound();
            }

            return Ok(kpiDefinition);
        }

        // POST: api/KPIDefinitions
        [HttpPost]
        public async Task<ActionResult<KPIDefinition>> PostKPIDefinition([FromBody] KPIDefinition kpiDefinition)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check for duplicate KpiCode and Version combination
            if (await _context.KPIDefinitions.AnyAsync(k => k.KpiCode == kpiDefinition.KpiCode && k.Version == kpiDefinition.Version))
            {
                return Conflict($"KPI với mã {kpiDefinition.KpiCode} phiên bản {kpiDefinition.Version} đã tồn tại");
            }

            _context.KPIDefinitions.Add(kpiDefinition);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetKPIDefinition), new { id = kpiDefinition.Id }, kpiDefinition);
        }

        // PUT: api/KPIDefinitions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKPIDefinition(int id, [FromBody] KPIDefinition kpiDefinition)
        {
            if (id != kpiDefinition.Id)
            {
                return BadRequest();
            }

            // Check for duplicate KpiCode and Version combination (excluding current record)
            if (await _context.KPIDefinitions.AnyAsync(k => k.KpiCode == kpiDefinition.KpiCode && k.Version == kpiDefinition.Version && k.Id != id))
            {
                return Conflict($"KPI với mã {kpiDefinition.KpiCode} phiên bản {kpiDefinition.Version} đã tồn tại");
            }

            _context.Entry(kpiDefinition).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.KPIDefinitions.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/KPIDefinitions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKPIDefinition(int id)
        {
            var kpiDefinition = await _context.KPIDefinitions.FindAsync(id);
            if (kpiDefinition == null)
            {
                return NotFound();
            }

            _context.KPIDefinitions.Remove(kpiDefinition);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/KPIDefinitions/by-cb-type?cbType=TruongphongKhdn
        [HttpGet("by-cb-type")]
        public Task<ActionResult<IEnumerable<KPIDefinition>>> GetKPIsByCBType([FromQuery] string cbType)
        {
            // TẠM THỜI VÔ HIỆU HÓA - Đang dọn sạch logic CBType cũ
            // Sẽ được thay thế bằng API mới cho 23 vai trò chuẩn
            return Task.FromResult<ActionResult<IEnumerable<KPIDefinition>>>(BadRequest(new 
            { 
                Message = "API tạm thời không khả dụng - đang cập nhật dữ liệu mới cho 23 vai trò chuẩn",
                RequestedCBType = cbType,
                Status = "Under maintenance"
            }));
            
            /*
            // LOGIC CŨ ĐÃ ĐƯỢC DỌN SẠCH:
            if (string.IsNullOrEmpty(cbType))
            {
                return BadRequest("Tham số cbType không được để trống");
            }

            var prefix = $"{cbType}_";
            var kpis = await _context.KPIDefinitions
                .Where(k => k.KpiCode != null && k.KpiCode.StartsWith(prefix) && k.IsActive)
                .OrderBy(k => k.KpiCode)
                .ToListAsync();

            return Ok(kpis);
            */
        }

        // POST: api/KPIDefinitions/sync-from-seed
        [HttpPost("sync-from-seed")]
        public async Task<IActionResult> SyncKPIFromSeed()
        {
            try
            {
                // TODO: Sẽ được cập nhật với dữ liệu KPI mới cho 23 vai trò
                // SeedKPIDefinitionMaxScore.Seed(_context);
                await _context.SaveChangesAsync();

                var count = await _context.KPIDefinitions.CountAsync();
                return Ok(new { 
                    message = "Đồng bộ dữ liệu KPI từ seed thành công", 
                    totalKPIs = count 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Lỗi khi đồng bộ dữ liệu KPI", 
                    error = ex.Message 
                });
            }
        }

        // REFACTORED METHOD: Use seed data instead of hardcoded values
        private Task<IActionResult> ResetKPIsByCBType(string cbType)
        {
            // TODO: Method tạm thời bị vô hiệu hóa - sẽ được cập nhật với dữ liệu KPI mới cho 23 vai trò
            return Task.FromResult<IActionResult>(BadRequest($"Chức năng reset KPI tạm thời không khả dụng - đang cập nhật dữ liệu mới cho vai trò: {cbType}"));
        }

        // POST: api/KPIDefinitions/reset-kpi-by-cb-type  
        [HttpPost("reset-kpi-by-cb-type")]
        public Task<IActionResult> ResetKPIsByCBTypeEndpoint([FromBody] dynamic request)
        {
            // TẠM THỜI VÔ HIỆU HÓA - Đang dọn sạch logic reset CBType cũ
            // Sẽ được thay thế bằng API mới cho 23 vai trò chuẩn
            return Task.FromResult<IActionResult>(BadRequest(new 
            { 
                Message = "API reset KPI tạm thời không khả dụng - đang cập nhật dữ liệu mới cho 23 vai trò chuẩn",
                Status = "Under maintenance"
            }));
            
            /*
            // LOGIC CŨ ĐÃ ĐƯỢC DỌN SẠCH:
            try
            {
                string cbType = request.cbType;
                if (string.IsNullOrEmpty(cbType))
                    return BadRequest("Tham số cbType không được để trống");

                var result = await ResetKPIsByCBType(cbType);
                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi khi reset KPI", Error = ex.Message });
            }
            */
        }

        // TẠM THỜI VÔ HIỆU HÓA - Đang dọn sạch các endpoint KPI cũ theo vai trò
        // Sẽ được thay thế bằng endpoints mới cho 23 vai trò chuẩn
        
        /*
        // Auto-generated endpoints for each CB type - ĐÃ ĐƯỢC DỌNG SẠCH
        // Các endpoints này sẽ được thay thế bằng API mới cho 23 vai trò chuẩn
        */

        // GET: api/KPIDefinitions/cb-types
        [HttpGet("cb-types")]
        public IActionResult GetCBTypes()
        {
            try
            {
                // TẠM THỜI VÔ HIỆU HÓA - Đang dọn sạch dữ liệu CBType cũ
                // Sẽ được thay thế bằng API mới cho 23 vai trò chuẩn
                
                var cbTypes = new List<object>
                {
                    new { 
                        Message = "API đang được cập nhật với dữ liệu mới cho 23 vai trò chuẩn",
                        Status = "Under maintenance"
                    }
                };

                return Ok(cbTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi khi lấy danh sách loại cán bộ", Error = ex.Message });
            }
        }

        /*
        // TẠM THỜI COMMENT OUT - Đang dọn sạch các CBType mapping cũ
        // Sẽ được thay thế bằng mapping mới cho 23 vai trò chuẩn
        // Helper method để chuyển đổi CbType thành tên hiển thị
        private string ConvertCbTypeToDisplayName(string cbType)
        {
            // Tất cả mapping cũ đã được dọn sạch
            // Sẽ được cài đặt lại với 23 vai trò chuẩn mới
            return cbType;
        }
        */
    }
}
