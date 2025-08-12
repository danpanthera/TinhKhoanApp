using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.DTOs.GL41;
using TinhKhoanApp.Api.Services.Interfaces;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// GL41 Controller - API endpoints cho bảng GL41 (Trial Balance)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GL41Controller : ControllerBase
    {
        private readonly IGL41Service _service;
        private readonly ILogger<GL41Controller> _logger;

        public GL41Controller(IGL41Service service, ILogger<GL41Controller> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Lấy preview GL41 (danh sách tóm tắt)
        /// </summary>
        [HttpGet("preview")]
        public async Task<IActionResult> Preview([FromQuery] int take = 20)
        {
            var response = await _service.PreviewAsync(take);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        /// <summary>
        /// Lấy chi tiết GL41 theo ID
        /// </summary>
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var response = await _service.GetByIdAsync(id);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        /// <summary>
        /// Lấy GL41 theo ngày dữ liệu
        /// </summary>
        [HttpGet("by-date/{date}")]
        public async Task<IActionResult> GetByDate(DateTime date, [FromQuery] int maxResults = 100)
        {
            var response = await _service.GetByDateAsync(date, maxResults);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        /// <summary>
        /// Lấy GL41 theo mã đơn vị
        /// </summary>
        [HttpGet("by-unit/{unitCode}")]
        public async Task<IActionResult> GetByUnit(string unitCode, [FromQuery] int maxResults = 100)
        {
            var response = await _service.GetByUnitAsync(unitCode, maxResults);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        /// <summary>
        /// Lấy GL41 theo mã tài khoản
        /// </summary>
        [HttpGet("by-account/{accountCode}")]
        public async Task<IActionResult> GetByAccountCode(string accountCode, [FromQuery] int maxResults = 100)
        {
            var response = await _service.GetByAccountCodeAsync(accountCode, maxResults);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        /// <summary>
        /// Tạo mới GL41
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GL41CreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Error("Dữ liệu đầu vào không hợp lệ"));
            }

            var response = await _service.CreateAsync(dto);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        /// <summary>
        /// Cập nhật GL41
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] GL41UpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Error("Dữ liệu đầu vào không hợp lệ"));
            }

            var response = await _service.UpdateAsync(dto);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        /// <summary>
        /// Xóa GL41
        /// </summary>
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await _service.DeleteAsync(id);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        /// <summary>
        /// Thống kê GL41 theo đơn vị
        /// </summary>
        [HttpGet("summary/unit/{unitCode}")]
        public async Task<IActionResult> SummaryByUnit(string unitCode, [FromQuery] DateTime? date = null)
        {
            var response = await _service.SummaryByUnitAsync(unitCode, date);
            return StatusCode(response.StatusCode ?? 200, response);
        }
    }
}
