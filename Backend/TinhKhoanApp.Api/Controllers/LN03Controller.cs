using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Services;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LN03Controller : ControllerBase
    {
        private readonly ILN03Service _ln03Service;
        private readonly DirectImportService _directImportService;
        private readonly ILogger<LN03Controller> _logger;

        public LN03Controller(
            ILN03Service ln03Service,
            DirectImportService directImportService,
            ILogger<LN03Controller> logger)
        {
            _ln03Service = ln03Service;
            _directImportService = directImportService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy tất cả dữ liệu LN03 (Dữ liệu phục hồi khoản vay)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LN03DTO>>> GetAll()
        {
            try
            {
                var result = await _ln03Service.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tất cả dữ liệu LN03");
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy dữ liệu LN03 theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<LN03DTO>> GetById(long id)
        {
            try
            {
                var result = await _ln03Service.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound($"Không tìm thấy bản ghi LN03 với ID: {id}");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu LN03 với ID: {ID}", id);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy dữ liệu LN03 gần đây
        /// </summary>
        [HttpGet("recent")]
        public async Task<ActionResult<IEnumerable<LN03DTO>>> GetRecent([FromQuery] int count = 10)
        {
            try
            {
                var result = await _ln03Service.GetRecentAsync(count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu LN03 gần đây");
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy dữ liệu LN03 theo ngày
        /// </summary>
        [HttpGet("by-date")]
        public async Task<ActionResult<IEnumerable<LN03DTO>>> GetByDate(
            [FromQuery] DateTime date,
            [FromQuery] int maxResults = 100)
        {
            try
            {
                var result = await _ln03Service.GetByDateAsync(date, maxResults);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu LN03 theo ngày: {Date}", date);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy dữ liệu LN03 theo khoảng thời gian
        /// </summary>
        [HttpGet("by-date-range")]
        public async Task<ActionResult<IEnumerable<LN03DTO>>> GetByDateRange(
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate,
            [FromQuery] int maxResults = 100)
        {
            try
            {
                if (fromDate > toDate)
                {
                    return BadRequest("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc");
                }

                var result = await _ln03Service.GetByDateRangeAsync(fromDate, toDate, maxResults);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu LN03 theo khoảng thời gian: {FromDate} - {ToDate}",
                    fromDate, toDate);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy dữ liệu LN03 theo mã chi nhánh
        /// </summary>
        [HttpGet("by-branch/{branchCode}")]
        public async Task<ActionResult<IEnumerable<LN03DTO>>> GetByBranchCode(
            string branchCode,
            [FromQuery] int maxResults = 100)
        {
            try
            {
                var result = await _ln03Service.GetByBranchCodeAsync(branchCode, maxResults);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu LN03 theo mã chi nhánh: {BranchCode}", branchCode);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy dữ liệu LN03 theo mã khách hàng
        /// </summary>
        [HttpGet("by-customer/{customerCode}")]
        public async Task<ActionResult<IEnumerable<LN03DTO>>> GetByCustomerCode(
            string customerCode,
            [FromQuery] int maxResults = 100)
        {
            try
            {
                var result = await _ln03Service.GetByCustomerCodeAsync(customerCode, maxResults);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu LN03 theo mã khách hàng: {CustomerCode}", customerCode);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy dữ liệu LN03 theo mã cán bộ tín dụng
        /// </summary>
        [HttpGet("by-officer/{officerCode}")]
        public async Task<ActionResult<IEnumerable<LN03DTO>>> GetByOfficerCode(
            string officerCode,
            [FromQuery] int maxResults = 100)
        {
            try
            {
                var result = await _ln03Service.GetByOfficerCodeAsync(officerCode, maxResults);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu LN03 theo mã cán bộ tín dụng: {OfficerCode}", officerCode);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy dữ liệu LN03 theo nhóm nợ
        /// </summary>
        [HttpGet("by-debt-group/{debtGroup}")]
        public async Task<ActionResult<IEnumerable<LN03DTO>>> GetByDebtGroup(
            string debtGroup,
            [FromQuery] int maxResults = 100)
        {
            try
            {
                var result = await _ln03Service.GetByDebtGroupAsync(debtGroup, maxResults);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu LN03 theo nhóm nợ: {DebtGroup}", debtGroup);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy tổng số tiền xử lý rủi ro theo chi nhánh
        /// </summary>
        [HttpGet("total-risk-amount/branch/{branchCode}")]
        public async Task<ActionResult<decimal>> GetTotalRiskAmountByBranch(
            string branchCode,
            [FromQuery] DateTime? date = null)
        {
            try
            {
                var result = await _ln03Service.GetTotalRiskAmountByBranchAsync(branchCode, date);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tổng số tiền xử lý rủi ro theo chi nhánh: {BranchCode}", branchCode);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Lấy tổng thu nợ sau xử lý theo chi nhánh
        /// </summary>
        [HttpGet("total-debt-recovery/branch/{branchCode}")]
        public async Task<ActionResult<decimal>> GetTotalDebtRecoveryByBranch(
            string branchCode,
            [FromQuery] DateTime? date = null)
        {
            try
            {
                var result = await _ln03Service.GetTotalDebtRecoveryByBranchAsync(branchCode, date);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tổng thu nợ sau xử lý theo chi nhánh: {BranchCode}", branchCode);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Tạo mới dữ liệu LN03
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<ActionResult<LN03DTO>> Create([FromBody] CreateLN03DTO createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _ln03Service.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mới dữ liệu LN03");
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Cập nhật dữ liệu LN03
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<ActionResult<LN03DTO>> Update(long id, [FromBody] UpdateLN03DTO updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var exists = await _ln03Service.ExistsAsync(id);
                if (!exists)
                {
                    return NotFound($"Không tìm thấy bản ghi LN03 với ID: {id}");
                }

                var result = await _ln03Service.UpdateAsync(id, updateDto);
                if (result == null)
                {
                    return NotFound($"Không tìm thấy bản ghi LN03 với ID: {id}");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật dữ liệu LN03 với ID: {ID}", id);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Xóa dữ liệu LN03
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var exists = await _ln03Service.ExistsAsync(id);
                if (!exists)
                {
                    return NotFound($"Không tìm thấy bản ghi LN03 với ID: {id}");
                }

                var result = await _ln03Service.DeleteAsync(id);
                if (!result)
                {
                    return StatusCode(500, "Xóa dữ liệu không thành công");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa dữ liệu LN03 với ID: {ID}", id);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        /// <summary>
        /// Import dữ liệu LN03 từ file CSV
        /// </summary>
        [HttpPost("import")]
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<IActionResult> ImportLN03File(IFormFile file, [FromQuery] string? statementDate = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Không có file được tải lên");
                }

                // Kiểm tra tên file có chứa ln03 không
                if (!file.FileName.ToLower().Contains("ln03"))
                {
                    return BadRequest("File không phải là file LN03. Tên file phải chứa 'ln03'");
                }

                // Sử dụng DirectImportService để import dữ liệu
                var result = await _directImportService.ImportLN03DirectAsync(file, statementDate);

                if (result.Success)
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = $"Import thành công {result.ProcessedRecords} bản ghi",
                        ImportedFileName = result.FileName,
                        RecordsCount = result.ProcessedRecords,
                        StatementDate = result.NgayDL,
                        ImportedBy = string.Empty, // Need to add this property or get from proper source
                        ImportId = result.ImportedDataRecordId
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = result.ErrorMessage,
                        ImportedFileName = result.FileName
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi import file LN03");
                return StatusCode(500, $"Đã xảy ra lỗi khi xử lý yêu cầu: {ex.Message}");
            }
        }
    }
}
