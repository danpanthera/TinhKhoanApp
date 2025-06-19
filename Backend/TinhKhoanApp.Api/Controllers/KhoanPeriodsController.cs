using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")] // Đường dẫn sẽ là "api/KhoanPeriods"
    [ApiController]
    public class KhoanPeriodsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public KhoanPeriodsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/KhoanPeriods (Lấy tất cả các Kỳ Khoán)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KhoanPeriod>>> GetKhoanPeriods()
        {
            return await _context.KhoanPeriods.OrderByDescending(p => p.StartDate).ToListAsync(); // Sắp xếp theo ngày bắt đầu giảm dần
        }

        // GET: api/KhoanPeriods/5 (Lấy một Kỳ Khoán theo Id)
        [HttpGet("{id}")]
        public async Task<ActionResult<KhoanPeriod>> GetKhoanPeriod(int id)
        {
            var khoanPeriod = await _context.KhoanPeriods.FindAsync(id);

            if (khoanPeriod == null)
            {
                return NotFound();
            }

            return Ok(khoanPeriod);
        }

        // POST: api/KhoanPeriods (Tạo một Kỳ Khoán mới)
        [HttpPost]
        public async Task<ActionResult<KhoanPeriod>> PostKhoanPeriod([FromBody] KhoanPeriod khoanPeriod)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra logic ngày bắt đầu và kết thúc
            if (khoanPeriod.StartDate >= khoanPeriod.EndDate)
            {
                ModelState.AddModelError("EndDate", "Ngày kết thúc phải sau ngày bắt đầu.");
                return BadRequest(ModelState);
            }

            // Kiểm tra trùng lặp kỳ khoán (dựa trên tên và loại - có thể cần logic phức tạp hơn)
            if (await _context.KhoanPeriods.AnyAsync(p => p.Name == khoanPeriod.Name && p.Type == khoanPeriod.Type))
            {
                return Conflict(new { message = $"Kỳ khoán với tên '{khoanPeriod.Name}' và loại '{khoanPeriod.Type}' đã tồn tại." });
            }

            _context.KhoanPeriods.Add(khoanPeriod);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetKhoanPeriod), new { id = khoanPeriod.Id }, khoanPeriod);
        }

        // PUT: api/KhoanPeriods/5 (Cập nhật một Kỳ Khoán đã có)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKhoanPeriod(int id, [FromBody] KhoanPeriod khoanPeriod)
        {
            if (id != khoanPeriod.Id)
            {
                return BadRequest("ID trong URL không khớp với ID trong nội dung request.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (khoanPeriod.StartDate >= khoanPeriod.EndDate)
            {
                ModelState.AddModelError("EndDate", "Ngày kết thúc phải sau ngày bắt đầu.");
                return BadRequest(ModelState);
            }

            // Kiểm tra trùng lặp tên kỳ khoán khi cập nhật (nếu tên thay đổi)
            if (await _context.KhoanPeriods.AnyAsync(p => p.Name == khoanPeriod.Name && p.Type == khoanPeriod.Type && p.Id != id))
            {
                 return Conflict(new { message = $"Kỳ khoán với tên '{khoanPeriod.Name}' và loại '{khoanPeriod.Type}' đã tồn tại." });
            }

            _context.Entry(khoanPeriod).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.KhoanPeriods.Any(e => e.Id == id))
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

        // DELETE: api/KhoanPeriods/5 (Xóa một Kỳ Khoán)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKhoanPeriod(int id)
        {
            var khoanPeriod = await _context.KhoanPeriods.FindAsync(id);
            if (khoanPeriod == null)
            {
                return NotFound();
            }

            try
            {
                // Kiểm tra xem kỳ khoán có đang được sử dụng không
                var hasEmployeeAssignments = await _context.EmployeeKpiAssignments.AnyAsync(eka => eka.KhoanPeriodId == id);
                var hasUnitScorings = await _context.UnitKpiScorings.AnyAsync(uks => uks.KhoanPeriodId == id);

                if (hasEmployeeAssignments || hasUnitScorings)
                {
                    // Xóa cascade: xóa tất cả dữ liệu liên quan trước
                    
                    // Xóa các giao khoán nhân viên liên quan
                    var employeeAssignments = await _context.EmployeeKpiAssignments
                        .Where(eka => eka.KhoanPeriodId == id)
                        .ToListAsync();
                    if (employeeAssignments.Any())
                    {
                        _context.EmployeeKpiAssignments.RemoveRange(employeeAssignments);
                    }

                    // Xóa các chấm điểm đơn vị liên quan  
                    var unitScorings = await _context.UnitKpiScorings
                        .Where(uks => uks.KhoanPeriodId == id)
                        .ToListAsync();
                    if (unitScorings.Any())
                    {
                        _context.UnitKpiScorings.RemoveRange(unitScorings);
                    }

                    // Lưu thay đổi xóa dữ liệu liên quan trước
                    await _context.SaveChangesAsync();
                }

                // Cuối cùng xóa kỳ khoán
                _context.KhoanPeriods.Remove(khoanPeriod);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết để debug
                Console.WriteLine($"Lỗi khi xóa kỳ khoán ID {id}: {ex.Message}");
                return BadRequest(new { 
                    message = "Không thể xóa kỳ khoán này. Vui lòng liên hệ quản trị viên.", 
                    error = ex.Message 
                });
            }
        }
    }
}