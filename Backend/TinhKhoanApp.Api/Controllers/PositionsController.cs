using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

            return position;
        }

        // PUT: api/Positions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPosition(int id, UpdatePositionDto positionDto)
        {
            var position = await _context.Positions.FindAsync(id);
            if (position == null)
            {
                return NotFound();
            }

            position.Name = positionDto.Name;
            position.Description = positionDto.Description;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PositionExists(id))
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

        // POST: api/Positions
        [HttpPost]
        public async Task<ActionResult<Position>> PostPosition(CreatePositionDto positionDto)
        {
            var position = new Position
            {
                Name = positionDto.Name,
                Description = positionDto.Description
            };

            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPosition", new { id = position.Id }, position);
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

            _context.Positions.Remove(position);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Positions/batch
        [HttpDelete("batch")]
        public async Task<IActionResult> DeletePositions([FromBody] int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                return BadRequest("No position IDs provided");
            }

            var positions = await _context.Positions
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();

            if (positions.Count == 0)
            {
                return NotFound("No positions found with the provided IDs");
            }

            _context.Positions.RemoveRange(positions);
            await _context.SaveChangesAsync();

            return Ok($"Deleted {positions.Count} positions");
        }

        // POST: api/Positions/seed
        [HttpPost("seed")]
        public async Task<IActionResult> SeedPositions()
        {
            // Kiểm tra xem đã có positions chưa
            var existingPositionCount = await _context.Positions.CountAsync();
            if (existingPositionCount > 0)
            {
                return Ok($"Already have {existingPositionCount} positions. No seeding needed.");
            }

            var positions = new List<Position>
            {
                new Position { Name = "Giám đốc", Description = "Giám đốc chi nhánh, đơn vị" },
                new Position { Name = "Phó Giám đốc", Description = "Phó giám đốc chi nhánh, đơn vị" },
                new Position { Name = "Trưởng phòng", Description = "Trưởng các phòng nghiệp vụ" },
                new Position { Name = "Phó trưởng phòng", Description = "Phó trưởng các phòng nghiệp vụ" },
                new Position { Name = "Chuyên viên chính", Description = "Chuyên viên có nhiều kinh nghiệm" },
                new Position { Name = "Chuyên viên", Description = "Chuyên viên nghiệp vụ" },
                new Position { Name = "Nhân viên", Description = "Nhân viên thực hiện công việc cụ thể" },
                new Position { Name = "Thực tập sinh", Description = "Sinh viên thực tập" }
            };

            _context.Positions.AddRange(positions);
            await _context.SaveChangesAsync();

            return Ok($"Successfully seeded {positions.Count} positions");
        }

        private bool PositionExists(int id)
        {
            return _context.Positions.Any(e => e.Id == id);
        }
    }

    // DTOs for Position operations
    public class CreatePositionDto
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
    }

    public class UpdatePositionDto
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
