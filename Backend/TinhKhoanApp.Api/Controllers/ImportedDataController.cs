using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using System.Text.Json;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportedDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ImportedDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("items")]
        public async Task<IActionResult> GetImportedDataItems()
        {
            try
            {
                var items = await _context.ImportedDataItems
                    .Include(i => i.ImportedDataRecord)
                    .Take(10)
                    .Select(i => new
                    {
                        i.Id,
                        i.ImportedDataRecordId,
                        i.RawData,
                        i.ProcessedDate,
                        ImportedDataRecord = new
                        {
                            i.ImportedDataRecord.FileName,
                            i.ImportedDataRecord.FileType,
                            i.ImportedDataRecord.Category,
                            i.ImportedDataRecord.ImportDate
                        }
                    })
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi truy vấn dữ liệu", error = ex.Message });
            }
        }

        [HttpGet("dp01-sample")]
        public async Task<IActionResult> GetDP01Sample()
        {
            try
            {
                // Tìm các bản ghi ImportedDataItems có chứa dữ liệu DP01
                var dp01Items = await _context.ImportedDataItems
                    .Include(i => i.ImportedDataRecord)
                    .Where(i => i.ImportedDataRecord.Category == "DP01" ||
                               i.ImportedDataRecord.FileName.Contains("DP01") ||
                               i.RawData.Contains("MA_CN") ||
                               i.RawData.Contains("TAI_KHOAN_HACH_TOAN"))
                    .Take(5)
                    .ToListAsync();

                var result = dp01Items.Select(item => new
                {
                    item.Id,
                    item.ImportedDataRecordId,
                    FileName = item.ImportedDataRecord.FileName,
                    Category = item.ImportedDataRecord.Category,
                    ImportDate = item.ImportedDataRecord.ImportDate,
                    RawDataSample = item.RawData.Length > 500 ? item.RawData.Substring(0, 500) + "..." : item.RawData,
                    ParsedData = TryParseJson(item.RawData)
                }).ToList();

                return Ok(new { count = result.Count, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi truy vấn dữ liệu DP01", error = ex.Message });
            }
        }

        private object? TryParseJson(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<object>(json);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new
            {
                message = "ImportedData API đang hoạt động",
                timestamp = DateTime.Now,
                status = "OK"
            });
        }

        [HttpGet("dp01-count")]
        public async Task<IActionResult> GetDP01Count()
        {
            try
            {
                var count = await _context.DP01s.CountAsync();
                var sampleData = await _context.DP01s.Take(5).ToListAsync();

                return Ok(new
                {
                    count = count,
                    message = $"Có {count:N0} bản ghi DP01",
                    sampleData = sampleData.Select(d => new
                    {
                        d.Id,
                        d.DATA_DATE,
                        d.MA_CN,
                        d.MA_PGD,
                        d.TAI_KHOAN_HACH_TOAN,
                        d.CURRENT_BALANCE
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi kiểm tra DP01", error = ex.Message });
            }
        }

        [HttpGet("dp01-simple")]
        public async Task<IActionResult> GetDP01Simple()
        {
            try
            {
                var count = await _context.DP01s.CountAsync();
                return Ok(new { count = count, message = $"Có {count:N0} bản ghi DP01" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi đếm DP01", error = ex.Message });
            }
        }

        [HttpGet("dp01-dates")]
        public async Task<IActionResult> GetDP01Dates()
        {
            try
            {
                var dates = await _context.DP01s
                    .Select(d => d.DATA_DATE.Date)
                    .Distinct()
                    .OrderByDescending(d => d)
                    .Take(10)
                    .ToListAsync();

                return Ok(new
                {
                    availableDates = dates.Select(d => d.ToString("yyyy-MM-dd")),
                    message = $"Có {dates.Count} ngày dữ liệu gần nhất"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy ngày DP01", error = ex.Message });
            }
        }

        [HttpGet("dp01-sample-debug")]
        public async Task<IActionResult> GetDP01SampleDebug()
        {
            try
            {
                var firstRecord = await _context.DP01s.FirstOrDefaultAsync();
                var totalCount = await _context.DP01s.CountAsync();

                var ma7800Count = await _context.DP01s.CountAsync(d => d.MA_CN == "7800");
                var ma7801Count = await _context.DP01s.CountAsync(d => d.MA_CN == "7801");

                return Ok(new
                {
                    totalCount = totalCount,
                    ma7800Count = ma7800Count,
                    ma7801Count = ma7801Count,
                    firstRecord = firstRecord != null ? new
                    {
                        firstRecord.Id,
                        firstRecord.DATA_DATE,
                        firstRecord.MA_CN,
                        firstRecord.MA_PGD,
                        firstRecord.TAI_KHOAN_HACH_TOAN,
                        firstRecord.CURRENT_BALANCE
                    } : null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi debug DP01", error = ex.Message });
            }
        }

        [HttpGet("dp01-quick-test")]
        public IActionResult GetDP01QuickTest()
        {
            try
            {
                // Thử query đơn giản nhất
                var hasData = _context.DP01s.Any();
                return Ok(new { hasData = hasData, message = hasData ? "Có dữ liệu DP01" : "Không có dữ liệu DP01" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi check DP01", error = ex.Message });
            }
        }
    }
}
