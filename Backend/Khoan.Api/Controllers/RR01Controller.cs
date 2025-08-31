using Microsoft.AspNetCore.Mvc;
using Khoan.Api.Services.Interfaces;
using Khoan.Api.Dtos.RR01;
using System.ComponentModel.DataAnnotations;

namespace Khoan.Api.Controllers
{
    /// <summary>
    /// RR01 Controller - Risk Report API endpoints
    /// Tuân thủ hoàn toàn business column names, không transformation tiếng Việt
    /// Hỗ trợ CSV import với exact header mapping
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Tags("RR01 - Risk Report Management")]
    public class RR01Controller : ControllerBase
    {
        private readonly IRR01Service _rr01Service;
        private readonly ILogger<RR01Controller> _logger;

        public RR01Controller(IRR01Service rr01Service, ILogger<RR01Controller> logger)
        {
            _rr01Service = rr01Service;
            _logger = logger;
        }

        /// <summary>
        /// Get RR01 records với filtering và pagination
        /// </summary>
        /// <param name="ngayDl">Filter theo ngày dữ liệu (optional)</param>
        /// <param name="branchCode">Filter theo mã chi nhánh (optional)</param>
        /// <param name="skip">Records to skip for pagination</param>
        /// <param name="take">Records to take (max 1000)</param>
        /// <returns>List RR01 preview records</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RR01PreviewDto>>> GetRR01Records(
            [FromQuery] DateTime? ngayDl = null,
            [FromQuery] string? branchCode = null,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 100)
        {
            try
            {
                // Validate pagination params
                if (take > 1000) take = 1000;
                if (skip < 0) skip = 0;

                var records = await _rr01Service.GetRR01PreviewAsync(ngayDl, branchCode, skip, take);
                
                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting RR01 records");
                return StatusCode(500, "Lỗi khi lấy dữ liệu RR01");
            }
        }

        /// <summary>
        /// Get RR01 record details by ID
        /// </summary>
        /// <param name="id">RR01 record ID</param>
        /// <returns>Full RR01 record với tất cả 25 business columns</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<RR01DetailsDto>> GetRR01ById([FromRoute] int id)
        {
            try
            {
                var record = await _rr01Service.GetRR01ByIdAsync(id);
                if (record == null)
                {
                    return NotFound($"Không tìm thấy RR01 record với ID: {id}");
                }

                return Ok(record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting RR01 record by ID: {id}");
                return StatusCode(500, "Lỗi khi lấy chi tiết RR01");
            }
        }

        /// <summary>
        /// Get RR01 summary analytics cho một ngày cụ thể
        /// </summary>
        /// <param name="ngayDl">Ngày dữ liệu cần phân tích</param>
        /// <returns>Tổng hợp analytics với recovery rates, amounts, counts</returns>
        [HttpGet("summary/{ngayDl}")]
        public async Task<ActionResult<RR01SummaryDto>> GetRR01Summary([FromRoute] DateTime ngayDl)
        {
            try
            {
                var summary = await _rr01Service.GetRR01SummaryAsync(ngayDl);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting RR01 summary for date: {ngayDl}");
                return StatusCode(500, "Lỗi khi lấy tóm tắt RR01");
            }
        }

        /// <summary>
        /// Validate CSV file structure trước khi import
        /// </summary>
        /// <param name="file">CSV file để validate</param>
        /// <returns>Validation result với header info và sample data</returns>
        [HttpPost("validate-csv")]
        public async Task<ActionResult> ValidateCsv([Required] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("File không được để trống");
                }

                // Basic filename validation
                if (!_rr01Service.ValidateFileName(file.FileName))
                {
                    return BadRequest(new 
                    { 
                        IsValid = false, 
                        Message = "Filename phải chứa 'RR01' và có extension .csv hoặc .txt" 
                    });
                }

                // Extract date validation
                var ngayDl = _rr01Service.ExtractDateFromFilename(file.FileName);
                if (!ngayDl.HasValue)
                {
                    return BadRequest(new 
                    { 
                        IsValid = false, 
                        Message = "Không thể extract NGAY_DL từ filename. Format: xxx_rr01_YYYYMMDD.csv" 
                    });
                }

                var tempPath = Path.GetTempFileName();
                try
                {
                    using (var stream = new FileStream(tempPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    using (var reader = new StreamReader(tempPath))
                    {
                        var firstLine = await reader.ReadLineAsync();
                        if (string.IsNullOrEmpty(firstLine))
                        {
                            return BadRequest(new 
                            { 
                                IsValid = false, 
                                Message = "CSV file trống" 
                            });
                        }

                        var headers = firstLine.Split(',').Select(h => h.Trim(' ', '"')).ToArray();
                        
                        var requiredColumns = new[] { 
                            "CN_LOAI_I", "BRCD", "MA_KH", "TEN_KH", "SO_LDS", "CCY",
                            "DUNO_GOC_HIENTAI", "DUNO_LAI_HIENTAI", "THU_GOC", "THU_LAI"
                        };

                        var missingColumns = requiredColumns.Where(col => !headers.Contains(col)).ToList();

                        return Ok(new 
                        {
                            IsValid = !missingColumns.Any(),
                            FileName = file.FileName,
                            ExtractedNGAY_DL = ngayDl,
                            Headers = headers,
                            RequiredColumnsFound = requiredColumns.Length - missingColumns.Count,
                            TotalColumns = headers.Length,
                            MissingColumns = missingColumns,
                            ValidationMessage = missingColumns.Any() 
                                ? $"Thiếu các columns bắt buộc: {string.Join(", ", missingColumns)}"
                                : "CSV structure hợp lệ với business column names"
                        });
                    }
                }
                finally
                {
                    if (System.IO.File.Exists(tempPath))
                    {
                        System.IO.File.Delete(tempPath);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating CSV: {file?.FileName}");
                return StatusCode(500, $"Lỗi validate CSV: {ex.Message}");
            }
        }
    }
}
