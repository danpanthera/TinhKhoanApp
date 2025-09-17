using Microsoft.AspNetCore.Mvc;
using Khoan.Api.Models.DTOs.RR01;
using Khoan.Api.Services.Interfaces;

namespace Khoan.Api.Controllers;

/// <summary>
/// RR01 Controller - Risk Report API endpoints
/// Hỗ trợ 25 business columns với CSV-first architecture
/// </summary>
[ApiController]
[Route("api/[controller]")]
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
    /// Lấy danh sách RR01 có phân trang
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("📊 [RR01] Get paged: page {PageNumber}, size {PageSize}", pageNumber, pageSize);
        var result = await _rr01Service.GetPagedAsync(pageNumber, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Lấy chi tiết RR01 theo ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("🔍 [RR01] Get by ID: {Id}", id);
        var result = await _rr01Service.GetByIdAsync(id);

        if (!result.Success && result.Message?.Contains("Không tìm thấy") == true)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Tạo RR01 record mới
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RR01CreateDto dto)
    {
        _logger.LogInformation("➕ [RR01] Create new record");
        var result = await _rr01Service.CreateAsync(dto);

        if (result.Success && result.Data != null)
            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);

        return BadRequest(result);
    }

    /// <summary>
    /// Cập nhật RR01 record
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] RR01UpdateDto dto)
    {
        _logger.LogInformation("✏️ [RR01] Update record: {Id}", id);
        var result = await _rr01Service.UpdateAsync(id, dto);

        if (!result.Success && result.Message?.Contains("Không tìm thấy") == true)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Xóa RR01 record
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("🗑️ [RR01] Delete record: {Id}", id);
        var result = await _rr01Service.DeleteAsync(id);

        if (!result.Success && result.Message?.Contains("Không tìm thấy") == true)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Lấy RR01 theo ngày
    /// </summary>
    [HttpGet("by-date/{date:datetime}")]
    public async Task<IActionResult> GetByDate(DateTime date)
    {
        _logger.LogInformation("📅 [RR01] Get by date: {Date}", date.ToString("yyyy-MM-dd"));
        var result = await _rr01Service.GetByDateAsync(date);
        return Ok(result);
    }

    /// <summary>
    /// Lấy RR01 theo chi nhánh
    /// </summary>
    [HttpGet("by-branch/{branchCode}")]
    public async Task<IActionResult> GetByBranch(string branchCode)
    {
        _logger.LogInformation("🏢 [RR01] Get by branch: {BranchCode}", branchCode);
        var result = await _rr01Service.GetByBranchAsync(branchCode);
        return Ok(result);
    }

    /// <summary>
    /// Lấy RR01 theo khách hàng
    /// </summary>
    [HttpGet("by-customer/{customerCode}")]
    public async Task<IActionResult> GetByCustomer(string customerCode)
    {
        _logger.LogInformation("👤 [RR01] Get by customer: {CustomerCode}", customerCode);
        var result = await _rr01Service.GetByCustomerAsync(customerCode);
        return Ok(result);
    }

    /// <summary>
    /// Lấy tóm tắt xử lý rủi ro theo ngày
    /// </summary>
    [HttpGet("processing-summary/{date:datetime}")]
    public async Task<IActionResult> GetProcessingSummary(DateTime date)
    {
        _logger.LogInformation("📊 [RR01] Get processing summary for date: {Date}", date.ToString("yyyy-MM-dd"));
        var result = await _rr01Service.GetProcessingSummaryAsync(date);
        return Ok(result);
    }

    /// <summary>
    /// Development endpoint - Self test với in-memory data
    /// </summary>
    [HttpGet("dev/self-test")]
    public async Task<IActionResult> SelfTest()
    {
        _logger.LogInformation("🧪 [RR01] Development self-test");

        try
        {
            // Test service với mock data
            var mockDate = DateTime.Today.AddDays(-1);
            var summaryResult = await _rr01Service.GetProcessingSummaryAsync(mockDate);

            var testResult = new
            {
                ServiceStatus = "OK",
                TestDate = mockDate.ToString("yyyy-MM-dd"),
                SummaryResult = summaryResult.Success ? "SUCCESS" : "FAILED",
                Message = "RR01 Service self-test completed",
                Details = summaryResult
            };

            return Ok(testResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [RR01] Self-test failed");
            return StatusCode(500, new { Error = "Self-test failed", Details = ex.Message });
        }
    }
}
