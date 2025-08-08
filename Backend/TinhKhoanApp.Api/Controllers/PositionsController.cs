using TinhKhoanApp.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PositionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Positions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Position>>> GetPositions()
        {
            return await _context.Positions.OrderBy(p => p.Name).ToListAsync();
        }

        // GET: api/Positions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Position>> GetPosition(int id)
        {
            var position = await _context.Positions.FindAsync(id);

            if (position == null)
            {
                return NotFound();
            }

            return Ok(position);
        }

        // POST: api/Positions
        [HttpPost]
        public async Task<ActionResult<Position>> PostPosition([FromBody] Position position)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra xem tên chức vụ đã tồn tại chưa
            if (await _context.Positions.AnyAsync(p => p.Name == position.Name))
            {
                // Trả về lỗi 409 Conflict nếu tên đã tồn tại
                return Conflict(new { message = $"Tên chức vụ '{position.Name}' đã tồn tại." });
            }

            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPosition), new { id = position.Id }, position);
        }

        // PUT: api/Positions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPosition(int id, [FromBody] Position position)
        {
            if (id != position.Id)
            {
                return BadRequest("ID trong URL không khớp với ID trong nội dung request.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra xem tên chức vụ mới có trùng với tên chức vụ khác không (ngoại trừ chính nó)
            if (await _context.Positions.AnyAsync(p => p.Name == position.Name && p.Id != id))
            {
                return Conflict(new { message = $"Tên chức vụ '{position.Name}' đã tồn tại cho một chức vụ khác." });
            }

            _context.Entry(position).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Positions.Any(e => e.Id == id))
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

        // DELETE: api/Positions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePosition(int id)
        {
            var position = await _context.Positions.FindAsync(id);
            if (position == null)
            {
                return NotFound();
            }

            // Cân nhắc: Nếu Chức vụ này đang được gán cho Nhân viên nào đó, 
            // Sếp có thể muốn kiểm tra và ngăn chặn việc xóa hoặc xử lý logic nghiệp vụ trước.
            var isPositionInUseByEmployee = await _context.Employees.AnyAsync(e => e.PositionId == id);
            if (isPositionInUseByEmployee)
            {
                return BadRequest(new { message = $"Chức vụ này đang được sử dụng bởi một hoặc nhiều nhân viên và không thể xóa." });
            }

            _context.Positions.Remove(position);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}