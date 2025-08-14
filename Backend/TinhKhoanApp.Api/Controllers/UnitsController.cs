using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UnitsController> _logger;

        public UnitsController(ApplicationDbContext context, ILogger<UnitsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("Getting all units");
                
                var units = await _context.Units
                    .Where(u => !u.IsDeleted)
                    .Select(u => new
                    {
                        u.Id,
                        u.Code,
                        u.Name,
                        u.Type,
                        u.ParentUnitId
                    })
                    .OrderBy(u => u.Name)
                    .ToListAsync();

                _logger.LogInformation("Found {Count} units", units.Count);
                return Ok(units);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all units");
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }
    }
}
