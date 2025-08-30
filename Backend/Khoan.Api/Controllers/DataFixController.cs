using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;
using Khoan.Api.Models;
using System.Text;

namespace Khoan.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataFixController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DataFixController> _logger;

        public DataFixController(ApplicationDbContext context, ILogger<DataFixController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("fix-utf8-corruption")]
        public async Task<IActionResult> FixUTF8Corruption()
        {
            try
            {
                _logger.LogInformation("Starting UTF-8 data corruption fix...");

                // Get all corrupted records
                var corruptedRecords = await _context.KpiAssignmentTables
                    .Where(k => k.Description.Contains("?") ||
                               k.Description.Contains("Ð") ||
                               k.Description.Contains("T?"))
                    .ToListAsync();

                _logger.LogInformation($"Found {corruptedRecords.Count} corrupted records");

                // Fix mapping dictionary
                var fixes = new Dictionary<int, string>
                {
                    {32, "Chi nhánh Bum Tở"},
                    {33, "Chi nhánh Đoàn Kết"},
                    {34, "Chi nhánh Nậm Hàng"},
                    {27, "Chi nhánh Phong Thổ"},
                    {28, "Chi nhánh Sìn Hồ"},
                    {26, "Hội Sở"},
                    {9, "Cán bộ tín dụng"},
                    {12, "Giao dịch viên"},
                    {19, "Giám đốc CNL2"},
                    {16, "Giám đốc PGD"},
                    {21, "Phó giám đốc CNL2/KT"},
                    {20, "Phó giám đốc CNL2/TD"},
                    {17, "Phó giám đốc PGD"},
                    {18, "Phó giám đốc PGD/CBTD"},
                    {13, "Tổ HK Kiểm tra nội bộ"},
                    {14, "Trưởng phòng IT/TH/KTGS"},
                    {4, "Trưởng phòng KHCN"},
                    {22, "Trưởng phòng KH CNL2"},
                    {3, "Trưởng phòng KHDN"},
                    {7, "Trưởng phòng KH&QLRR"},
                    {10, "Trưởng phòng KTNQ CNL1"},
                    {24, "Trưởng phòng KTNQ CNL2"}
                };

                int fixedCount = 0;
                var results = new List<object>();

                foreach (var record in corruptedRecords)
                {
                    if (fixes.ContainsKey(record.Id))
                    {
                        var oldDescription = record.Description;
                        record.Description = fixes[record.Id];

                        results.Add(new
                        {
                            Id = record.Id,
                            OldDescription = oldDescription,
                            NewDescription = record.Description,
                            Status = "Fixed"
                        });

                        fixedCount++;
                        _logger.LogInformation($"Fixed ID {record.Id}: '{oldDescription}' → '{record.Description}'");
                    }
                    else
                    {
                        results.Add(new
                        {
                            Id = record.Id,
                            Description = record.Description,
                            Status = "Needs manual fix"
                        });
                        _logger.LogWarning($"No fix mapping found for ID {record.Id}: '{record.Description}'");
                    }
                }

                // Save changes
                await _context.SaveChangesAsync();

                _logger.LogInformation($"UTF-8 corruption fix completed. Fixed {fixedCount} records.");

                return Ok(new
                {
                    message = $"UTF-8 corruption fix completed successfully",
                    totalCorrupted = corruptedRecords.Count,
                    totalFixed = fixedCount,
                    details = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fixing UTF-8 corruption");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("check-corruption")]
        public async Task<IActionResult> CheckCorruption()
        {
            try
            {
                var corruptedRecords = await _context.KpiAssignmentTables
                    .Where(k => k.Description.Contains("?") ||
                               k.Description.Contains("Ð") ||
                               k.Description.Contains("T?"))
                    .Select(k => new
                    {
                        k.Id,
                        k.Description,
                        k.Category
                    })
                    .ToListAsync();

                return Ok(new
                {
                    totalCorrupted = corruptedRecords.Count,
                    corruptedRecords = corruptedRecords
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking corruption");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("fix-place-names")]
        public async Task<IActionResult> FixPlaceNames()
        {
            try
            {
                _logger.LogInformation("Starting place names fix...");

                // Fix "Chi nhánh Bình Lu" to "Chi nhánh Bình Lư"
                var binhLuRecord = await _context.KpiAssignmentTables
                    .FirstOrDefaultAsync(k => k.Id == 31);

                if (binhLuRecord != null && binhLuRecord.Description == "Chi nhánh Bình Lu")
                {
                    var oldName = binhLuRecord.Description;
                    binhLuRecord.Description = "Chi nhánh Bình Lư";

                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Fixed place name: '{oldName}' → '{binhLuRecord.Description}'");

                    return Ok(new
                    {
                        message = "Place names fixed successfully",
                        fixedRecord = new
                        {
                            Id = binhLuRecord.Id,
                            OldName = oldName,
                            NewName = binhLuRecord.Description
                        }
                    });
                }
                else
                {
                    return Ok(new
                    {
                        message = "No place name fixes needed",
                        currentRecord = binhLuRecord?.Description ?? "Record not found"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fixing place names");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("fix-indicators-corruption")]
        public async Task<IActionResult> FixIndicatorsCorruption()
        {
            try
            {
                _logger.LogInformation("Starting KPI Indicators UTF-8 corruption fix...");

                // Get all corrupted indicator records
                var corruptedIndicators = await _context.KpiIndicators
                    .Where(i => i.IndicatorName.Contains("?") ||
                               i.Unit.Contains("?") ||
                               i.IndicatorName.Contains("T?") ||
                               i.Unit.Contains("Tri?u"))
                    .ToListAsync();

                _logger.LogInformation($"Found {corruptedIndicators.Count} corrupted indicator records");

                // Common fixes for indicators
                var nameFixMappings = new Dictionary<string, string>
                {
                    {"T?ng du n? BQ", "Tổng dư nợ BQ"},
                    {"T? l? n? x?u", "Tỷ lệ nợ xấu"},
                    {"Phát tri?n Khách hàng", "Phát triển Khách hàng"},
                    {"Du n? cho vay BQ", "Dư nợ cho vay BQ"},
                    {"T? l? n? x?u", "Tỷ lệ nợ xấu"},
                    {"T? l? n? nhóm 2", "Tỷ lệ nợ nhóm 2"},
                    {"Nguon v?n huy d?ng BQ", "Nguồn vốn huy động BQ"},
                    {"T? l? tái c?u trúc n?", "Tỷ lệ tái cấu trúc nợ"}
                };

                var unitFixMappings = new Dictionary<string, string>
                {
                    {"Tri?u VND", "Triệu VND"},
                    {"Ngàn VND", "Ngàn VND"}
                };

                int fixedCount = 0;
                var results = new List<object>();

                foreach (var indicator in corruptedIndicators)
                {
                    bool wasFixed = false;
                    var oldName = indicator.IndicatorName;
                    var oldUnit = indicator.Unit;

                    // Fix indicator name
                    foreach (var mapping in nameFixMappings)
                    {
                        if (indicator.IndicatorName == mapping.Key)
                        {
                            indicator.IndicatorName = mapping.Value;
                            wasFixed = true;
                            break;
                        }
                    }

                    // Fix unit
                    foreach (var mapping in unitFixMappings)
                    {
                        if (indicator.Unit == mapping.Key)
                        {
                            indicator.Unit = mapping.Value;
                            wasFixed = true;
                            break;
                        }
                    }

                    if (wasFixed)
                    {
                        results.Add(new
                        {
                            Id = indicator.Id,
                            TableId = indicator.TableId,
                            OldName = oldName,
                            NewName = indicator.IndicatorName,
                            OldUnit = oldUnit,
                            NewUnit = indicator.Unit,
                            Status = "Fixed"
                        });

                        fixedCount++;
                        _logger.LogInformation($"Fixed Indicator ID {indicator.Id}: '{oldName}' → '{indicator.IndicatorName}', Unit: '{oldUnit}' → '{indicator.Unit}'");
                    }
                    else
                    {
                        results.Add(new
                        {
                            Id = indicator.Id,
                            Name = indicator.IndicatorName,
                            Unit = indicator.Unit,
                            Status = "Needs manual fix"
                        });
                    }
                }

                // Save changes
                await _context.SaveChangesAsync();

                _logger.LogInformation($"KPI Indicators UTF-8 corruption fix completed. Fixed {fixedCount} records.");

                return Ok(new
                {
                    message = $"KPI Indicators UTF-8 corruption fix completed successfully",
                    totalCorrupted = corruptedIndicators.Count,
                    totalFixed = fixedCount,
                    details = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fixing KPI Indicators corruption");
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpPost("fix-remaining-indicators-corruption")]
        public async Task<IActionResult> FixRemainingIndicatorsCorruption()
        {
            try
            {
                _logger.LogInformation("Starting remaining indicators corruption fix...");

                // Get all indicators that still have corruption issues
                var corruptedIndicators = await _context.KpiIndicators
                    .Where(i => i.IndicatorName.Contains("?") || i.Unit.Contains("?"))
                    .ToListAsync();

                _logger.LogInformation($"Found {corruptedIndicators.Count} indicators with remaining corruption");

                // Additional comprehensive mapping for missed corruptions
                var additionalNameFixMappings = new Dictionary<string, string>
                {
                    // Complex sentences
                    {"Thu n? dã XLRR (n?u không có n? XLRR thì c?ng vào ch? tiêu Du n?)", "Thu nợ đã XLRR (nếu không có nợ XLRR thì cộng vào chỉ tiêu Dư nợ)"},
                    {"Th?c hi?n nhi?m v? theo chuong trình công tác", "Thực hiện nhiệm vụ theo chương trình công tác"},
                    {"Ch?p hành quy ch?, quy trình nghi?p v?, van hóa Agribank", "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank"},
                    {"Hoàn thành ch? tiêu giao khoán SPDV", "Hoàn thành chỉ tiêu giao khoán SPDV"},
                    {"T?ng ngu?n v?n huy d?ng BQ", "Tổng nguồn vốn huy động BQ"},

                    // Long complex indicators
                    {"Th?c hi?n nhi?m v? theo chuong trình công tác, các công vi?c theo ch?c nang nhi?m v? c?a phòng", "Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng"},
                    {"Ch?p hành quy ch?, quy trình nghi?p v?, n?i dung ch? d?o, di?u hành c?a CNL1, van hóa Agribank", "Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank"},
                    {"K?t qu? th?c hi?n BQ c?a CB trong phòng", "Kết quả thực hiện BQ của CB trong phòng"},
                    {"S? bút toán giao d?ch BQ", "Số bút toán giao dịch BQ"},
                    {"S? bút toán h?y", "Số bút toán hủy"},
                    {"Th?c hi?n nhi?m v? theo chuong trình công tác, ch?c nang nhi?m v? c?a phòng", "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng"}
                };

                int fixedCount = 0;
                var results = new List<object>();

                foreach (var indicator in corruptedIndicators)
                {
                    bool wasFixed = false;
                    var oldName = indicator.IndicatorName;
                    var oldUnit = indicator.Unit;

                    // Check for exact match first
                    if (additionalNameFixMappings.ContainsKey(indicator.IndicatorName))
                    {
                        indicator.IndicatorName = additionalNameFixMappings[indicator.IndicatorName];
                        wasFixed = true;
                    }

                    if (wasFixed)
                    {
                        results.Add(new
                        {
                            Id = indicator.Id,
                            TableId = indicator.TableId,
                            OldName = oldName,
                            NewName = indicator.IndicatorName,
                            OldUnit = oldUnit,
                            NewUnit = indicator.Unit,
                            Status = "Fixed"
                        });

                        fixedCount++;
                        _logger.LogInformation($"Fixed Indicator ID {indicator.Id}: '{oldName}' → '{indicator.IndicatorName}'");
                    }
                    else
                    {
                        results.Add(new
                        {
                            Id = indicator.Id,
                            IndicatorName = indicator.IndicatorName,
                            Unit = indicator.Unit,
                            Status = "Needs manual fix"
                        });
                    }
                }

                if (fixedCount > 0)
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Successfully fixed {fixedCount} remaining indicators with UTF-8 corruption");
                }

                return Ok(new
                {
                    message = $"Fixed {fixedCount} remaining indicators corruption",
                    totalProcessed = corruptedIndicators.Count,
                    fixedCount = fixedCount,
                    results = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fixing remaining indicators corruption");
                return StatusCode(500, $"Error fixing remaining indicators corruption: {ex.Message}");
            }
        }
    }
}
