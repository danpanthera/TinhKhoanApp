using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Services.Interfaces;
using TinhKhoanApp.Api.Models.Common;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// Controller cho Direct Import - chỉ cho phép import file đúng format
    /// DP01: chỉ file chứa "dp01" trong filename
    /// DPDA: chỉ file chứa "dpda" trong filename
    /// EI01: chỉ file chứa "ei01" trong filename
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
        /// DP01/DPDA/EI01/LN03/GL01: chỉ cho phép file chứa mã tương ứng trong filename
        /// </summary>
        [HttpPost("smart")]
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
            $"Tên file '{file.FileName}' không hợp lệ. File phải chứa một trong các mã: dp01, dpda, ei01, gl01, ln03.",
                        "INVALID_FILENAME"));
                }

                _logger.LogInformation("🔍 Detected DataType: {DataType} từ filename: {FileName}",
                    dataType, file.FileName);

                // Import với dataType đã detect
                var result = await _directImportService.ImportGenericAsync(file, dataType, statementDate);

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

            if (lowerFileName.Contains("ln03"))
            {
                return "LN03";
            }

            // Không match filename pattern nào
            return null;
        }
    }
}
