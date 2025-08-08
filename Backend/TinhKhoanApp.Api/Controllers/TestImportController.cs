using TinhKhoanApp.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestImportController : ControllerBase
    {
        private readonly ILogger<TestImportController> _logger;

        public TestImportController(ILogger<TestImportController> logger)
        {
            _logger = logger;
        }

        // 🧪 TEST: Import file không cần database
        [HttpPost("simple")]
        public async Task<IActionResult> SimpleImport([FromForm] IFormFileCollection files)
        {
            try
            {
                if (files == null || files.Count == 0)
                {
                    return BadRequest(new { message = "Không có file nào được chọn" });
                }

                var results = new List<object>();

                foreach (var file in files)
                {
                    _logger.LogInformation($"Processing file: {file.FileName}, Size: {file.Length} bytes");

                    // Đọc nội dung file
                    using var reader = new StreamReader(file.OpenReadStream());
                    var content = await reader.ReadToEndAsync();

                    // Parse CSV đơn giản
                    var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    var recordCount = Math.Max(0, lines.Length - 1); // Trừ header

                    results.Add(new
                    {
                        fileName = file.FileName,
                        size = file.Length,
                        contentType = file.ContentType,
                        linesProcessed = lines.Length,
                        recordCount = recordCount,
                        success = true,
                        message = $"✅ Đã xử lý {recordCount} records từ file {file.FileName}",
                        preview = lines.Take(3).ToArray() // 3 dòng đầu
                    });
                }

                return Ok(new
                {
                    message = $"✅ Test import thành công {results.Count} file(s)",
                    results = results,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi test import");
                return StatusCode(500, new
                {
                    message = "❌ Lỗi test import",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }



        // 📊 TEST: Health check cho import system
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                status = "healthy",
                message = "✅ Test Import Controller đang hoạt động bình thường",
                timestamp = DateTime.UtcNow,
                supportedFormats = new[] { ".csv", ".xlsx", ".xls", ".pdf" }
            });
        }
    }
}
