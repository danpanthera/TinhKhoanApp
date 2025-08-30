using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;

namespace Khoan.Api.Controllers
{
    /// <summary>
    /// Maintenance controller for fixing data issues
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MaintenanceController> _logger;

        public MaintenanceController(ApplicationDbContext context, ILogger<MaintenanceController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Fix Vietnamese encoding in KPI indicator names
        /// </summary>
        [HttpPost("fix-vietnamese-encoding")]
        public async Task<IActionResult> FixVietnameseEncoding()
        {
            try
            {
                _logger.LogInformation("🔧 Starting Vietnamese encoding fix for KPI Indicators...");

                var indicators = await _context.KpiIndicators.ToListAsync();
                int fixedCount = 0;

                // Dictionary to fix common corrupted Vietnamese characters
                var encodingFixes = new Dictionary<string, string>
                {
                    // Existing fixes
                    { "T?ng ngu?n v?n cu?i k?", "Tổng nguồn vốn cuối kỳ" },
                    { "T?ng ngu?n v?n huy d?ng BQ trong k?", "Tổng nguồn vốn huy động BQ trong kỳ" },
                    { "T?ng du n? cu?i k?", "Tổng dư nợ cuối kỳ" },
                    { "T?ng du n? BQ trong k?", "Tổng dư nợ BQ trong kỳ" },
                    { "T?ng du n? HSX&CN", "Tổng dư nợ HSX&CN" },
                    { "T? l? n? x?u n?i b?ng", "Tỷ lệ nợ xấu nội bảng" },
                    { "Thu n? dã XLRR", "Thu nợ đã XLRR" },
                    { "Phát tri?n Khách hàng", "Phát triển Khách hàng" },
                    { "L?i nhu?n khoán tài chính", "Lợi nhuận khoán tài chính" },
                    { "T?ng ngu?n v?n huy d?ng BQ", "Tổng nguồn vốn huy động BQ" },
                    { "T?ng ngu?n v?n BQ", "Tổng nguồn vốn BQ" },
                    { "T?ng ngu?n v?n", "Tổng nguồn vốn" },
                    { "T?ng du n?", "Tổng dư nợ" },
                    { "L?i nhu?n", "Lợi nhuận" },
                    { "Thu n?", "Thu nợ" },
                    { "N? x?u", "Nợ xấu" },
                    { "Phát tri?n", "Phát triển" },

                    // Additional comprehensive fixes for more patterns
                    { "T?ng Du n?", "Tổng Dư nợ" },
                    { "T? l?", "Tỷ lệ" },
                    { "n? x?u", "nợ xấu" },
                    { "dã XLRR", "đã XLRR" },
                    { "Doanh nghi?p", "Doanh nghiệp" },
                    { "nghi?p", "nghiệp" },
                    { "Ði?u hành", "Điều hành" },
                    { "i?u", "iều" },
                    { "Ch?p hành", "Chấp hành" },
                    { "?p hành", "ấp hành" },
                    { "quy ch?", "quy chế" },
                    { "nghi?p v?", "nghiệp vụ" },
                    { "k?t qu?", "kết quả" },
                    { "th?c hi?n", "thực hiện" },
                    { "ph? trách", "phụ trách" },

                    // Additional fixes for newly found corrupted characters
                    { "Thu d?ch v?", "Thu dịch vụ" },
                    { "d?ch v?", "dịch vụ" },
                    { "K?t qu?", "Kết quả" },
                    { "c?a", "của" },
                    { "nu?c", "nước" },
                    { "Th?c hi?n", "Thực hiện" },
                    { "ch?c nang", "chức năng" },
                    { "nhi?m v?", "nhiệm vụ" },
                    { "du?c", "được" },
                    { "thu?c", "thuộc" },
                    { "CB", "CB" }, // This one is already correct (Cán Bộ abbreviation)
                    { "BQ", "BQ" }  // This one is already correct (Bình Quân abbreviation)
                };

                var fixedIndicators = new List<object>();

                foreach (var indicator in indicators)
                {
                    var originalName = indicator.IndicatorName;
                    var fixedName = originalName;

                    // Apply encoding fixes
                    foreach (var fix in encodingFixes)
                    {
                        if (fixedName.Contains(fix.Key))
                        {
                            fixedName = fixedName.Replace(fix.Key, fix.Value);
                        }
                    }

                    if (fixedName != originalName)
                    {
                        _logger.LogInformation($"  🔤 Fixing: '{originalName}' → '{fixedName}'");
                        indicator.IndicatorName = fixedName;
                        fixedCount++;

                        fixedIndicators.Add(new
                        {
                            Id = indicator.Id,
                            TableId = indicator.TableId,
                            OriginalName = originalName,
                            FixedName = fixedName
                        });
                    }
                }

                if (fixedCount > 0)
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"✅ Fixed {fixedCount} indicator names with Vietnamese encoding issues");

                    return Ok(new
                    {
                        success = true,
                        message = $"Fixed {fixedCount} indicator names with Vietnamese encoding issues",
                        fixedCount = fixedCount,
                        fixedIndicators = fixedIndicators
                    });
                }
                else
                {
                    _logger.LogInformation("ℹ️ No encoding issues found in indicator names");
                    return Ok(new
                    {
                        success = true,
                        message = "No encoding issues found in indicator names",
                        fixedCount = 0,
                        fixedIndicators = new object[0]
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fixing Vietnamese encoding");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error fixing Vietnamese encoding",
                    error = ex.Message
                });
            }
        }
    }
}
