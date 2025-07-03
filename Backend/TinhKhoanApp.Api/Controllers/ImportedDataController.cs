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
            return Ok(new { 
                message = "ImportedData API đang hoạt động", 
                timestamp = DateTime.Now,
                status = "OK"
            });
        }
    }
}
