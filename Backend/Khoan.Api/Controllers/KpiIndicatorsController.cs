using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;
using Khoan.Api.Models;
using Khoan.Api.Models.Common;

namespace Khoan.Api.Controllers
{
    /// <summary>
    /// Controller để quản lý KPI Indicators
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class KpiIndicatorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<KpiIndicatorsController> _logger;

        public KpiIndicatorsController(ApplicationDbContext context, ILogger<KpiIndicatorsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Lấy tất cả KPI indicators
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetKpiIndicators()
        {
            try
            {
                var indicators = await _context.KpiIndicators
                    .Select(k => new
                    {
                        k.Id,
                        Name = k.IndicatorName,
                        k.MaxScore,
                        k.Unit,
                        k.OrderIndex,
                        ValueType = k.ValueType.ToString(),
                        k.IsActive,
                        TableId = k.TableId
                    })
                    .ToListAsync();

                return Ok(ApiResponse<object>.Ok(indicators, "KPI indicators loaded successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Error getting KPI indicators: {ex.Message}", "LOAD_ERROR"));
            }
        }

        /// <summary>
        /// Lấy KPI indicators theo table ID
        /// </summary>
        [HttpGet("by-table/{tableId}")]
        public async Task<IActionResult> GetKpiIndicatorsByTable(int tableId)
        {
            try
            {
                var indicators = await _context.KpiIndicators
                    .Where(k => k.TableId == tableId)
                    .Select(k => new
                    {
                        k.Id,
                        Name = k.IndicatorName,
                        k.MaxScore,
                        k.Unit,
                        k.OrderIndex,
                        ValueType = k.ValueType.ToString(),
                        k.IsActive
                    })
                    .OrderBy(k => k.OrderIndex)
                    .ToListAsync();

                return Ok(ApiResponse<object>.Ok(indicators, "KPI indicators by table loaded successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Error getting KPI indicators by table: {ex.Message}", "LOAD_ERROR"));
            }
        }

        /// <summary>
        /// Lấy KPI indicators theo category
        /// </summary>
        [HttpGet("by-category/{category}")]
        public async Task<IActionResult> GetKpiIndicatorsByCategory(string category)
        {
            try
            {
                var indicators = await _context.KpiIndicators
                    .Where(k => k.Table.Category == category)
                    .Select(k => new
                    {
                        k.Id,
                        Name = k.IndicatorName,
                        k.MaxScore,
                        k.Unit,
                        k.OrderIndex,
                        ValueType = k.ValueType.ToString(),
                        k.IsActive,
                        TableId = k.TableId
                    })
                    .OrderBy(k => k.TableId)
                    .ThenBy(k => k.OrderIndex)
                    .ToListAsync();

                return Ok(ApiResponse<object>.Ok(indicators, "KPI indicators by category loaded successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Error getting KPI indicators by category: {ex.Message}", "LOAD_ERROR"));
            }
        }

        /// <summary>
        /// Lấy KPI indicator theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetKpiIndicator(int id)
        {
            try
            {
                var indicator = await _context.KpiIndicators
                    .Where(k => k.Id == id)
                    .Select(k => new
                    {
                        k.Id,
                        Name = k.IndicatorName,
                        k.MaxScore,
                        k.Unit,
                        k.OrderIndex,
                        ValueType = k.ValueType.ToString(),
                        k.IsActive,
                        TableId = k.TableId
                    })
                    .FirstOrDefaultAsync();

                if (indicator == null)
                    return NotFound(ApiResponse<object>.Error("KPI indicator not found", "NOT_FOUND"));

                return Ok(ApiResponse<object>.Ok(indicator, "KPI indicator loaded successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Error getting KPI indicator: {ex.Message}", "LOAD_ERROR"));
            }
        }
    }
}
