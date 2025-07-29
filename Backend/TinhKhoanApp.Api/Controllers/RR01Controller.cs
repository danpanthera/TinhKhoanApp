using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Services;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/rr01")]
    public class RR01Controller : ControllerBase
    {
        private readonly IRR01Service _rr01Service;
        private readonly IDirectImportService _importService;
        private readonly ILogger<RR01Controller> _logger;

        public RR01Controller(
            IRR01Service rr01Service,
            IDirectImportService importService,
            ILogger<RR01Controller> logger)
        {
            _rr01Service = rr01Service;
            _importService = importService;
            _logger = logger;
        }

        /// <summary>
        /// Get all RR01 records
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RR01DTO>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var records = await _rr01Service.GetAllAsync();
            return Ok(records);
        }

        /// <summary>
        /// Get RR01 record by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RR01DTO), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(long id)
        {
            var record = await _rr01Service.GetByIdAsync(id);
            if (record == null)
            {
                return NotFound();
            }
            return Ok(record);
        }

        /// <summary>
        /// Get RR01 records by statement date
        /// </summary>
        [HttpGet("by-date/{date}")]
        [ProducesResponseType(typeof(IEnumerable<RR01DTO>), 200)]
        public async Task<IActionResult> GetByDate(DateTime date)
        {
            var records = await _rr01Service.GetByDateAsync(date);
            return Ok(records);
        }

        /// <summary>
        /// Get RR01 records by branch
        /// </summary>
        [HttpGet("by-branch/{branchCode}")]
        [ProducesResponseType(typeof(IEnumerable<RR01DTO>), 200)]
        public async Task<IActionResult> GetByBranch(string branchCode, [FromQuery] DateTime? date = null)
        {
            var records = await _rr01Service.GetByBranchAsync(branchCode, date);
            return Ok(records);
        }

        /// <summary>
        /// Get RR01 records by customer ID
        /// </summary>
        [HttpGet("by-customer/{customerId}")]
        [ProducesResponseType(typeof(IEnumerable<RR01DTO>), 200)]
        public async Task<IActionResult> GetByCustomer(string customerId, [FromQuery] DateTime? date = null)
        {
            var records = await _rr01Service.GetByCustomerAsync(customerId, date);
            return Ok(records);
        }

        /// <summary>
        /// Get paged RR01 records with optional filters
        /// </summary>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] DateTime? date = null,
            [FromQuery] string? branchCode = null,
            [FromQuery] string? customerId = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var (records, totalCount) = await _rr01Service.GetPagedAsync(
                pageNumber,
                pageSize,
                date,
                branchCode,
                customerId);

            return Ok(new
            {
                Records = records,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }

        /// <summary>
        /// Create a new RR01 record
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(RR01DTO), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CreateRR01DTO createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdRecord = await _rr01Service.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = createdRecord.Id }, createdRecord);
        }

        /// <summary>
        /// Update an existing RR01 record
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RR01DTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateRR01DTO updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedRecord = await _rr01Service.UpdateAsync(id, updateDto);
            if (updatedRecord == null)
            {
                return NotFound();
            }
            return Ok(updatedRecord);
        }

        /// <summary>
        /// Delete an RR01 record
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _rr01Service.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        /// <summary>
        /// Get distinct statement dates available for RR01 records
        /// </summary>
        [HttpGet("dates")]
        [ProducesResponseType(typeof(IEnumerable<DateTime>), 200)]
        public async Task<IActionResult> GetDistinctDates()
        {
            var dates = await _rr01Service.GetDistinctDatesAsync();
            return Ok(dates);
        }

        /// <summary>
        /// Get distinct branch codes for RR01 records
        /// </summary>
        [HttpGet("branches")]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        public async Task<IActionResult> GetDistinctBranches([FromQuery] DateTime? date = null)
        {
            var branches = await _rr01Service.GetDistinctBranchesAsync(date);
            return Ok(branches);
        }

        /// <summary>
        /// Get summary statistics for RR01 data
        /// </summary>
        [HttpGet("statistics/{date}")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> GetStatistics(DateTime date)
        {
            var statistics = await _rr01Service.GetSummaryStatisticsAsync(date);
            return Ok(statistics);
        }

        /// <summary>
        /// Import RR01 data from a CSV file
        /// </summary>
        [HttpPost("import")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ImportRR01([FromForm] IFormFile file, [FromQuery] string? statementDate = null)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (!file.FileName.ToLower().Contains("rr01"))
            {
                return BadRequest("Invalid file. File name must contain 'rr01'.");
            }

            try
            {
                _logger.LogInformation("Importing RR01 data from file {FileName}", file.FileName);
                var result = await _importService.ImportRR01DirectAsync(file, statementDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing RR01 data: {Message}", ex.Message);
                return BadRequest($"Error importing data: {ex.Message}");
            }
        }
    }
}
