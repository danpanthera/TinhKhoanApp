using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Khoan.Api.Services.Interfaces;
using Khoan.Api.Models.Common;

namespace Khoan.Api.Controllers
{
    /// <summary>
    /// Controller cho Direct Import - chỉ cho phép import file đúng format
    /// DP01: chỉ file chứa "dp01" trong filename
    /// DPDA: chỉ file chứa "dpda" trong filename
    /// EI01: chỉ file chứa "ei01" trong filename
    /// GL01: chỉ file chứa "gl01" trong filename
    /// GL02: chỉ file chứa "gl02" trong filename
    /// GL41: chỉ file chứa "gl41" trong filename
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DirectImportController : ControllerBase
    {
        private readonly IDirectImportService _directImportService;
        private readonly ILogger<DirectImportController> _logger;

        public DirectImportController(
            IDirectImportService directImportService,
            ILogger<DirectImportController> logger)
        {
            _directImportService = directImportService;
            _logger = logger;
        }

        /// <summary>
        /// Smart Import endpoint - tự động detect dataType từ filename
        /// DP01/DPDA/EI01/GL01/GL02/GL41/LN03/LN01/RR01: chỉ cho phép file chứa mã tương ứng trong filename
        /// </summary>
        [HttpPost("smart")]
        [DisableRequestSizeLimit] // allow large multipart uploads
        [RequestSizeLimit(2L * 1024 * 1024 * 1024)] // 2GB cap
        [RequestFormLimits(MultipartBodyLengthLimit = 2L * 1024 * 1024 * 1024, ValueCountLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
        public async Task<IActionResult> SmartImport(
            IFormFile file,
            [FromForm] string? statementDate = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(ApiResponse<object>.Error("No file uploaded", "NO_FILE"));
                }

                // Validate filename theo quy tắc strict
                var dataType = DetectDataTypeFromFilename(file.FileName);
                if (dataType == null)
                {
                    return BadRequest(ApiResponse<object>.Error(
            $"Tên file '{file.FileName}' không hợp lệ. File phải chứa một trong các mã: dp01, dpda, ei01, gl01, gl02, gl41, ln03, ln01, rr01.",
                        "INVALID_FILENAME"));
                }

                _logger.LogInformation("🔍 Detected DataType: {DataType} từ filename: {FileName}",
                    dataType, file.FileName);

                // Import với dataType đã detect
                var result = await _directImportService.ImportGenericAsync(file, dataType, statementDate);

                if (result == null || !result.Success || result.ProcessedRecords <= 0)
                {
                    var message = result == null
                        ? "Import failed"
                        : (result.Errors?.Any() == true ? string.Join("; ", result.Errors) : "No records imported");
                    return BadRequest(ApiResponse<object>.Error(message, "IMPORT_FAILED"));
                }

                return Ok(ApiResponse<object>.Ok(result, "Import completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Smart import failed");
                return BadRequest(ApiResponse<object>.Error($"Smart import failed: {ex.Message}", "SMART_IMPORT_ERROR"));
            }
        }

        /// <summary>
        /// Detect DataType từ filename - STRICT validation
        /// DP01: phải chứa "dp01"
        /// </summary>
        private string? DetectDataTypeFromFilename(string fileName)
        {
            var lowerFileName = fileName.ToLower();

            // DP01: chỉ cho phép file chứa "dp01"
            if (lowerFileName.Contains("dp01"))
            {
                return "DP01";
            }

            // Add other data types as needed
            if (lowerFileName.Contains("dpda"))
            {
                return "DPDA";
            }

            if (lowerFileName.Contains("ei01"))
            {
                return "EI01";
            }

            if (lowerFileName.Contains("gl01"))
            {
                return "GL01";
            }

            if (lowerFileName.Contains("gl02"))
            {
                return "GL02";
            }

            if (lowerFileName.Contains("gl41"))
            {
                return "GL41";
            }

            if (lowerFileName.Contains("ln03"))
            {
                return "LN03";
            }

            // Add LN01 support
            if (lowerFileName.Contains("ln01"))
            {
                return "LN01";
            }

            // Add RR01 support - đã có sẵn trong code
            if (lowerFileName.Contains("rr01"))
            {
                return "RR01";
            }

            // Không match filename pattern nào
            return null;
        }

        /// <summary>
        /// Lấy số lượng records trong các bảng dữ liệu
        /// </summary>
        [HttpGet("table-counts")]
        public async Task<IActionResult> GetTableCounts()
        {
            try
            {
                var counts = await _directImportService.GetTableRecordCountsAsync();
                _logger.LogInformation("✅ Retrieved table record counts successfully");
                return Ok(ApiResponse<Dictionary<string, long>>.Ok(counts, "Table record counts retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error retrieving table record counts");
                return StatusCode(500, ApiResponse<object>.Error("Error retrieving table record counts", "TABLE_COUNTS_ERROR"));
            }
        }
    }
}
