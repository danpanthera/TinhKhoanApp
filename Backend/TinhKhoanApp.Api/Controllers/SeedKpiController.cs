using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedKpiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SeedKpiController> _logger;

        public SeedKpiController(ApplicationDbContext context, ILogger<SeedKpiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("reset-and-seed")]
        public async Task<IActionResult> ResetAndSeedKpiTables()
        {
            try
            {
                _logger.LogInformation("=== Bắt đầu reset và seed KPI tables ===");

                // 1. Reset current data
                _logger.LogInformation("🧹 Xóa dữ liệu KPI hiện tại...");
                _context.KpiIndicators.RemoveRange(_context.KpiIndicators);
                _context.KpiAssignmentTables.RemoveRange(_context.KpiAssignmentTables);
                await _context.SaveChangesAsync();

                // 2. Run seeder
                _logger.LogInformation("🌱 Chạy KpiAssignmentTableSeeder...");
                KpiAssignmentTableSeeder.SeedKpiAssignmentTables(_context);

                // 3. Get results
                var totalTables = await _context.KpiAssignmentTables.CountAsync();
                var totalIndicators = await _context.KpiIndicators.CountAsync();

                var tablesByCategory = await _context.KpiAssignmentTables
                    .GroupBy(t => t.Category)
                    .Select(g => new { Category = g.Key, Count = g.Count() })
                    .ToListAsync();

                _logger.LogInformation("✅ Hoàn thành seed KPI tables");

                return Ok(new
                {
                    message = "Seed KPI tables thành công",
                    totalTables = totalTables,
                    totalIndicators = totalIndicators,
                    categorySummary = tablesByCategory,
                    timestamp = DateTime.UtcNow
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi seed KPI tables");
                return StatusCode(500, new { message = "Lỗi khi seed KPI tables", error = ex.Message });
            }
        }

        [HttpGet("verify-structure")]
        public async Task<IActionResult> VerifyKpiStructure()
        {
            try
            {
                var totalTables = await _context.KpiAssignmentTables.CountAsync();
                var totalIndicators = await _context.KpiIndicators.CountAsync();

                var tableDetails = await _context.KpiAssignmentTables
                    .Select(t => new
                    {
                        t.Id,
                        t.TableName,
                        t.TableType,
                        t.Category,
                        t.Description,
                        IndicatorCount = _context.KpiIndicators.Count(i => i.TableId == t.Id)
                    })
                    .OrderBy(t => t.Id)
                    .ToListAsync();

                return Ok(new
                {
                    summary = new
                    {
                        totalTables = totalTables,
                        totalIndicators = totalIndicators,
                        expectedTables = 32, // 23 vai trò + 9 chi nhánh
                        isCorrect = totalTables == 32
                    },
                    tables = tableDetails
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi verify KPI structure");
                return StatusCode(500, new { message = "Lỗi khi verify KPI structure", error = ex.Message });
            }
        }

        [HttpPost("seed-kpi-definitions")]
        public async Task<IActionResult> SeedKpiDefinitions()
        {
            try
            {
                _logger.LogInformation("=== Bắt đầu seed KPI Definitions ===");

                // 1. Seed KPI Definitions
                _logger.LogInformation("🌱 Chạy SeedKPIDefinitionMaxScore...");
                SeedKPIDefinitionMaxScore.SeedKPIDefinitions(_context);

                // 2. Get results
                var totalDefinitions = await _context.KPIDefinitions.CountAsync();

                _logger.LogInformation($"✅ Hoàn thành seed {totalDefinitions} KPI definitions");

                return Ok(new
                {
                    message = "Seed KPI definitions thành công",
                    totalDefinitions = totalDefinitions,
                    timestamp = DateTime.UtcNow
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi seed KPI definitions");
                return StatusCode(500, new { message = "Lỗi khi seed KPI definitions", error = ex.Message });
            }
        }

        [HttpPost("populate-158-indicators")]
        public async Task<IActionResult> Populate158Indicators()
        {
            try
            {
                _logger.LogInformation("=== Bắt đầu populate 158 indicators ===");

                // 1. Xóa indicators cũ
                _logger.LogInformation("🧹 Xóa indicators cũ...");
                _context.KpiIndicators.RemoveRange(_context.KpiIndicators);
                await _context.SaveChangesAsync();

                // 2. Re-run seeder to populate indicators
                _logger.LogInformation("📊 Re-run seeder để populate indicators từ KPIDefinitions...");
                KpiAssignmentTableSeeder.SeedKpiAssignmentTables(_context);

                // 3. Get results
                var totalIndicators = await _context.KpiIndicators.CountAsync();

                _logger.LogInformation($"✅ Hoàn thành populate {totalIndicators} indicators");

                return Ok(new
                {
                    message = "Populate 158 indicators thành công",
                    totalIndicators = totalIndicators,
                    expectedIndicators = 158,
                    isCorrect = totalIndicators == 158,
                    timestamp = DateTime.UtcNow
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi populate indicators");
                return StatusCode(500, new { message = "Lỗi khi populate indicators", error = ex.Message });
            }
        }
    }
}
