using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;
using Khoan.Api.Models;

namespace Khoan.Api.Controllers
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
        public async Task<ActionResult> GetKpiAssignmentTables()
        {
            try
            {
                var tables = await _context.KpiAssignmentTables.ToListAsync();

                var result = new List<object>();

                // Manually load indicators for all tables as DTOs to avoid circular reference
                foreach (var table in tables)
                {
                    var indicators = await _context.KpiIndicators
                        .Where(i => i.TableId == table.Id)
                        .Select(i => new
                        {
                            i.Id,
                            i.TableId,
                            Name = i.IndicatorName,
                            i.MaxScore,
                            i.Unit,
                            i.OrderIndex,
                            ValueType = i.ValueType.ToString(),
                            i.IsActive
                        })
                        .ToListAsync();

                    result.Add(new
                    {
                        table.Id,
                        table.TableType,
                        table.TableName,
                        table.Description,
                        table.Category,
                        table.IsActive,
                        table.CreatedDate,
                        Indicators = indicators,
                        IndicatorCount = indicators.Count
                    });
                }

                // Sắp xếp theo logic tùy chỉnh
                var sortedTables = result.OrderBy(t =>
                {
                    var tableObj = (dynamic)t;
                    return GetSortKey(new KpiAssignmentTable
                    {
                        Category = (string)tableObj.Category,
                        TableName = (string)tableObj.TableName
                    });
                }).ToList();

                _logger.LogInformation("Retrieved {Count} KPI assignment tables with manually loaded indicators as DTOs", sortedTables.Count);
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
                    .Include(t => t.Indicators)
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
        public async Task<ActionResult> GetKpiAssignmentTable(int id)
        {
            try
            {
                var kpiAssignmentTable = await _context.KpiAssignmentTables
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (kpiAssignmentTable == null)
                {
                    return NotFound();
                }

                // Manually load indicators for this table as anonymous objects to avoid circular reference
                var indicators = await _context.KpiIndicators
                    .Where(i => i.TableId == id)
                    .Select(i => new
                    {
                        i.Id,
                        i.TableId,
                        Name = i.IndicatorName,
                        i.MaxScore,
                        i.Unit,
                        i.OrderIndex,
                        ValueType = i.ValueType.ToString(),
                        i.IsActive
                    })
                    .ToListAsync();

                var result = new
                {
                    kpiAssignmentTable.Id,
                    kpiAssignmentTable.TableType,
                    kpiAssignmentTable.TableName,
                    kpiAssignmentTable.Description,
                    kpiAssignmentTable.Category,
                    kpiAssignmentTable.IsActive,
                    kpiAssignmentTable.CreatedDate,
                    Indicators = indicators,
                    IndicatorCount = indicators.Count
                };

                _logger.LogInformation($"✅ Table {id} loaded with {indicators.Count} indicators as DTO");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving KPI assignment table with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }        /// <summary>
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
