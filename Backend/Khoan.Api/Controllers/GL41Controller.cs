using Microsoft.AspNetCore.Mvc;
using Khoan.Api.Models.Common;
using Khoan.Api.Models.Dtos.GL41;
using Khoan.Api.Services.Interfaces;

namespace Khoan.Api.Controllers
{
    /// <summary>
    /// Controller for GL41 operations - Partitioned Columnstore Optimized
    /// Handles GL41 balance analytics with direct CSV import
    /// Only processes files containing "gl41" in filename
    /// NGAY_DL extracted from filename and converted to datetime2 (dd/mm/yyyy)
    /// 13 business columns + 4 system columns = 17 total columns
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GL41Controller : ControllerBase
    {
        private readonly IGL41Service _gl41Service;
        private readonly ILogger<GL41Controller> _logger;

        public GL41Controller(IGL41Service gl41Service, ILogger<GL41Controller> logger)
        {
            _gl41Service = gl41Service;
            _logger = logger;
        }

        /// <summary>
        /// Get paginated GL41 data
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<GL41PreviewDto>>>> GetAll(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            var result = await _gl41Service.GetAllAsync(page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Get GL41 record by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<GL41DetailsDto?>>> GetById(long id)
        {
            var result = await _gl41Service.GetByIdAsync(id);
            
            if (!result.Success && result.StatusCode == 404)
                return NotFound(result);
                
            return Ok(result);
        }

        /// <summary>
        /// Import GL41 CSV file with 2GB support and partitioned columnstore optimization
        /// Only processes files containing "gl41" in filename
        /// NGAY_DL extracted from filename (yyyyMMdd format)
        /// Supports #,###.00 format for AMOUNT/BALANCE columns
        /// </summary>
        [HttpPost("import")]
        [DisableRequestSizeLimit] // Support 2GB files
         // 15 minutes timeout
        public async Task<ActionResult<ApiResponse<GL41ImportResultDto>>> ImportCsv(
            IFormFile file, 
            [FromQuery] string? fileName = null)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<GL41ImportResultDto>.Error("File is required"));
            }

            var result = await _gl41Service.ImportCsvAsync(file, fileName);
            return Ok(result);
        }

        /// <summary>
        /// Delete GL41 records by date range
        /// </summary>
        [HttpDelete("date-range")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteByDateRange(
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            var result = await _gl41Service.DeleteByDateRangeAsync(fromDate, toDate);
            return Ok(result);
        }

        /// <summary>
        /// Get GL41 summary analytics by unit
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<ApiResponse<IEnumerable<GL41SummaryByUnitDto>>>> GetSummaryByUnit(
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            var result = await _gl41Service.GetSummaryByUnitAsync(fromDate, toDate);
            return Ok(result);
        }

        /// <summary>
        /// Get GL41 analytics configuration
        /// </summary>
        [HttpGet("config")]
        public async Task<ActionResult<ApiResponse<GL41AnalyticsConfigDto>>> GetAnalyticsConfig()
        {
            var result = await _gl41Service.GetAnalyticsConfigAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get GL41 records by unit code
        /// </summary>
        [HttpGet("unit/{unitCode}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<GL41PreviewDto>>>> GetByUnitCode(
            string unitCode,
            [FromQuery] int maxResults = 100)
        {
            var result = await _gl41Service.GetByUnitCodeAsync(unitCode, maxResults);
            return Ok(result);
        }

        /// <summary>
        /// Get GL41 records by account code
        /// </summary>
        [HttpGet("account/{accountCode}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<GL41PreviewDto>>>> GetByAccountCode(
            string accountCode,
            [FromQuery] int maxResults = 100)
        {
            var result = await _gl41Service.GetByAccountCodeAsync(accountCode, maxResults);
            return Ok(result);
        }

        /// <summary>
        /// Get GL41 balance summary for specific unit
        /// </summary>
        [HttpGet("balance/{unitCode}")]
        public async Task<ActionResult<ApiResponse<decimal>>> GetBalanceSummary(
            string unitCode,
            [FromQuery] DateTime? date = null)
        {
            var result = await _gl41Service.GetBalanceSummaryAsync(unitCode, date);
            return Ok(result);
        }

        /// <summary>
        /// Check if GL41 data exists for specific date
        /// </summary>
        [HttpGet("exists")]
        public async Task<ActionResult<ApiResponse<bool>>> HasDataForDate([FromQuery] DateTime date)
        {
            var result = await _gl41Service.HasDataForDateAsync(date);
            return Ok(result);
        }
    }
}
