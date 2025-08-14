using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KpiAssignmentTablesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<KpiAssignmentTablesController> _logger;

        public KpiAssignmentTablesController(ApplicationDbContext context, ILogger<KpiAssignmentTablesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Lấy tất cả KPI Assignment Tables với sắp xếp tùy chỉnh
        /// CANBO: Sắp xếp ABC (A-Z)
        /// CHINHANH: Sắp xếp theo thứ tự units (Hội Sở, Bình Lư, Phong Thổ, Sìn Hồ, Bum Tở, Than Uyên, Đoàn Kết, Tân Uyên, Nậm Hàng)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KpiAssignmentTable>>> GetKpiAssignmentTables()
        {
            try
            {
                var tables = await _context.KpiAssignmentTables.ToListAsync();

                // Sắp xếp theo logic tùy chỉnh
                var sortedTables = tables.OrderBy(t => GetSortKey(t)).ToList();

                _logger.LogInformation("Retrieved {Count} KPI assignment tables with custom sorting", sortedTables.Count);
                return Ok(sortedTables);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving KPI assignment tables");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy KPI Assignment Tables theo category với sắp xếp tùy chỉnh
        /// </summary>
        [HttpGet("by-category/{category}")]
        public async Task<ActionResult<IEnumerable<KpiAssignmentTable>>> GetKpiAssignmentTablesByCategory(string category)
        {
            try
            {
                var tables = await _context.KpiAssignmentTables
                    .Where(t => t.Category.ToUpper() == category.ToUpper())
                    .ToListAsync();

                // Sắp xếp theo logic tùy chỉnh
                var sortedTables = tables.OrderBy(t => GetSortKey(t)).ToList();

                _logger.LogInformation("Retrieved {Count} KPI assignment tables for category {Category}", sortedTables.Count, category);
                return Ok(sortedTables);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving KPI assignment tables for category {Category}", category);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Logic sắp xếp tùy chỉnh cho dropdown
        /// CANBO: A-Z alphabetical
        /// CHINHANH: Theo thứ tự units cụ thể
        /// </summary>
        private string GetSortKey(KpiAssignmentTable table)
        {
            if (table.Category?.ToUpper() == "CANBO")
            {
                // CANBO: Sắp xếp ABC
                return table.TableName ?? "";
            }
            else if (table.Category?.ToUpper() == "CHINHANH")
            {
                // CHINHANH: Sắp xếp theo thứ tự units
                var unitOrder = new Dictionary<string, int>
                {
                    { "HỘI SỞ", 1 },
                    { "BÌNH LƯ", 2 },
                    { "PHONG THỔ", 3 },
                    { "SÌN HỒ", 4 },
                    { "BUM TỞ", 5 },
                    { "THAN UYÊN", 6 },
                    { "ĐOÀN KẾT", 7 },
                    { "TÂN UYÊN", 8 },
                    { "NẬM HÀNG", 9 }
                };

                // Tìm unit name trong table name
                var tableName = table.TableName?.ToUpper() ?? "";
                foreach (var unit in unitOrder.Keys)
                {
                    if (tableName.Contains(unit.ToUpper()))
                    {
                        return $"{unitOrder[unit]:D2}_{table.TableName}";
                    }
                }

                // Nếu không tìm thấy, đặt ở cuối
                return $"99_{table.TableName}";
            }
            else
            {
                // Default: alphabetical
                return table.TableName ?? "";
            }
        }

        /// <summary>
        /// Tạo mới KPI Assignment Table
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<KpiAssignmentTable>> PostKpiAssignmentTable(KpiAssignmentTable kpiAssignmentTable)
        {
            try
            {
                _context.KpiAssignmentTables.Add(kpiAssignmentTable);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created new KPI assignment table: {TableName}", kpiAssignmentTable.TableName);
                return CreatedAtAction("GetKpiAssignmentTable", new { id = kpiAssignmentTable.Id }, kpiAssignmentTable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating KPI assignment table");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy KPI Assignment Table theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<KpiAssignmentTable>> GetKpiAssignmentTable(int id)
        {
            try
            {
                var kpiAssignmentTable = await _context.KpiAssignmentTables.FindAsync(id);

                if (kpiAssignmentTable == null)
                {
                    return NotFound();
                }

                return Ok(kpiAssignmentTable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving KPI assignment table with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Cập nhật KPI Assignment Table
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKpiAssignmentTable(int id, KpiAssignmentTable kpiAssignmentTable)
        {
            if (id != kpiAssignmentTable.Id)
            {
                return BadRequest();
            }

            _context.Entry(kpiAssignmentTable).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated KPI assignment table: {TableName}", kpiAssignmentTable.TableName);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!KpiAssignmentTableExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Concurrency error updating KPI assignment table with ID {Id}", id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating KPI assignment table with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }

            return NoContent();
        }

        /// <summary>
        /// Xóa KPI Assignment Table
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKpiAssignmentTable(int id)
        {
            try
            {
                var kpiAssignmentTable = await _context.KpiAssignmentTables.FindAsync(id);
                if (kpiAssignmentTable == null)
                {
                    return NotFound();
                }

                _context.KpiAssignmentTables.Remove(kpiAssignmentTable);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted KPI assignment table: {TableName}", kpiAssignmentTable.TableName);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting KPI assignment table with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        private bool KpiAssignmentTableExists(int id)
        {
            return _context.KpiAssignmentTables.Any(e => e.Id == id);
        }
    }
}
