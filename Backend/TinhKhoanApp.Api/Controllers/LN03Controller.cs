using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Models.Dtos;
using TinhKhoanApp.Api.Services.Interfaces;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LN03Controller : ControllerBase
    {
        private readonly ILN03Service _service;
        private readonly ILogger<LN03Controller> _logger;

        public LN03Controller(ILN03Service service, ILogger<LN03Controller> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Lấy tất cả bản ghi LN03
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LN03DTO>>> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all LN03 records");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy bản ghi LN03 theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<LN03DTO>> GetById(long id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                if (result == null)
                    return NotFound($"LN03 record with ID {id} not found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting LN03 record by ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy danh sách bản ghi gần đây nhất
        /// </summary>
        [HttpGet("recent")]
        public async Task<ActionResult<IEnumerable<LN03DTO>>> GetRecent(int count = 10)
        {
            try
            {
                var result = await _service.GetRecentAsync(count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent LN03 records");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy danh sách bản ghi theo ngày
        /// </summary>
        [HttpGet("by-date")]
        public async Task<ActionResult<IEnumerable<LN03DTO>>> GetByDate(DateTime date, int maxResults = 100)
        {
            try
            {
                var result = await _service.GetByDateAsync(date, maxResults);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting LN03 records by date: {Date}", date);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
