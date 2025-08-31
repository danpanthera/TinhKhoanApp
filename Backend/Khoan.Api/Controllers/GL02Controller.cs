using Khoan.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Khoan.Api.Models.Dtos.GL02;
using Khoan.Api.Services.Interfaces;

namespace Khoan.Api.Controllers
{
    /// <summary>
    /// Controller cho dữ liệu GL02 - Heavy File Optimized
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GL02Controller : ControllerBase
    {
        private readonly IGL02Service _gl02Service;
        private readonly ILogger<GL02Controller> _logger;

        public GL02Controller(IGL02Service gl02Service, ILogger<GL02Controller> logger)
        {
            _gl02Service = gl02Service;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách GL02 với phân trang
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GL02PreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetAllGL02([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Retrieving GL02 data, page: {Page}, pageSize: {PageSize}", page, pageSize);
                var result = await _gl02Service.GetAllAsync(page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL02 data");
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve GL02 data"));
            }
        }

        /// <summary>
        /// Lấy chi tiết GL02 theo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<GL02DetailsDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetGL02Detail(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving GL02 detail for ID {Id}", id);
                var result = await _gl02Service.GetByIdAsync(id);
                
                if (!result.Success)
                    return NotFound(result);
                    
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL02 detail for ID {Id}", id);
                return BadRequest(ApiResponse<object>.Error("Failed to retrieve GL02 detail"));
            }
        }

        /// <summary>
        /// Import CSV file GL02 - Heavy File Support (max 2GB)
        /// </summary>
        [HttpPost("import-csv")]
        [ProducesResponseType(typeof(ApiResponse<GL02ImportResultDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [RequestSizeLimit(2L * 1024 * 1024 * 1024)] // 2GB
        [DisableRequestSizeLimit] // Alternative approach for heavy files
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            try
            {
                _logger.LogInformation("🚀 [GL02] Starting CSV import: {FileName}", file?.FileName);
                
                var result = await _gl02Service.ImportCsvAsync(file);
                
                if (!result.Success)
                    return BadRequest(result);
                    
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing GL02 CSV");
                return BadRequest(ApiResponse<object>.Error("Failed to import GL02 CSV"));
            }
        }

        /// <summary>
        /// Xóa dữ liệu GL02 theo khoảng thời gian
        /// </summary>
        [HttpDelete("date-range")]
        [ProducesResponseType(typeof(ApiResponse<int>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> DeleteByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                _logger.LogInformation("Deleting GL02 records from {StartDate} to {EndDate}", startDate, endDate);
                
                var result = await _gl02Service.DeleteByDateRangeAsync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting GL02 records in date range");
                return BadRequest(ApiResponse<object>.Error("Failed to delete GL02 records"));
            }
        }

        /// <summary>
        /// Lấy tóm tắt GL02 theo đơn vị
        /// </summary>
        [HttpGet("summary/{unitCode}")]
        [ProducesResponseType(typeof(ApiResponse<GL02SummaryByUnitDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetSummaryByUnit(string unitCode, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                _logger.LogInformation("Getting GL02 summary for unit {UnitCode}", unitCode);
                
                var result = await _gl02Service.GetSummaryByUnitAsync(unitCode, startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting GL02 summary for unit {UnitCode}", unitCode);
                return BadRequest(ApiResponse<object>.Error("Failed to get GL02 summary"));
            }
        }

        /// <summary>
        /// Lấy cấu hình Heavy File cho GL02
        /// </summary>
        [HttpGet("heavy-file-config")]
        [ProducesResponseType(typeof(ApiResponse<GL02HeavyFileConfigDto>), 200)]
        public async Task<IActionResult> GetHeavyFileConfig()
        {
            try
            {
                var config = new GL02HeavyFileConfigDto
                {
                    MaxFileSizeBytes = 2L * 1024 * 1024 * 1024, // 2GB
                    MaxBatchSize = 10000,
                    TimeoutMinutes = 15,
                    AllowedFilePattern = "*gl02*.csv",
                    SupportedColumns = 17,
                    RequiredColumns = new[] { "TRDATE", "TRBRCD", "LOCAC", "CCY", "DRAMOUNT", "CRAMOUNT" }
                };

                return Ok(ApiResponse<GL02HeavyFileConfigDto>.Ok(config, "GL02 heavy file configuration"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting GL02 heavy file config");
                return BadRequest(ApiResponse<object>.Error("Failed to get heavy file config"));
            }
        }
    }
}
