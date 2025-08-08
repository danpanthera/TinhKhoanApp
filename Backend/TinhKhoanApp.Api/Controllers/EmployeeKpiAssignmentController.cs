using TinhKhoanApp.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Models.Dtos;
using TinhKhoanApp.Api.Services;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeKpiAssignmentController : ControllerBase
    {
        private readonly IEmployeeKpiAssignmentService _service;

        public EmployeeKpiAssignmentController(IEmployeeKpiAssignmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeKpiAssignmentDto>>> GetAll()
        {
            try
            {
                var assignments = await _service.GetAllAsync();
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeKpiAssignmentDto>> GetById(int id)
        {
            try
            {
                var assignment = await _service.GetByIdAsync(id);
                if (assignment == null)
                    return NotFound(new { message = "Assignment not found" });

                return Ok(assignment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<EmployeeKpiAssignmentDto>>> GetByEmployeeId(int employeeId)
        {
            try
            {
                var assignments = await _service.GetByEmployeeIdAsync(employeeId);
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpGet("khoan-period/{khoanPeriodId}")]
        public async Task<ActionResult<IEnumerable<EmployeeKpiAssignmentDto>>> GetByKhoanPeriodId(int khoanPeriodId)
        {
            try
            {
                var assignments = await _service.GetByKhoanPeriodIdAsync(khoanPeriodId);
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeKpiAssignmentDto>> Create([FromBody] CreateEmployeeKpiAssignmentDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check if assignment already exists
                var exists = await _service.ExistsAsync(createDto.EmployeeId, createDto.KpiDefinitionId, createDto.KhoanPeriodId);
                if (exists)
                    return Conflict(new { message = "Assignment already exists for this employee, KPI, and period" });

                var assignment = await _service.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = assignment.Id }, assignment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpPost("bulk")]
        public async Task<ActionResult<IEnumerable<EmployeeKpiAssignmentDto>>> CreateBulk([FromBody] BulkCreateEmployeeKpiAssignmentDto bulkCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (bulkCreateDto.Assignments == null || !bulkCreateDto.Assignments.Any())
                    return BadRequest(new { message = "No assignments provided" });

                var assignments = await _service.CreateBulkAsync(bulkCreateDto.Assignments);
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EmployeeKpiAssignmentDto>> Update(int id, [FromBody] UpdateEmployeeKpiAssignmentDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var assignment = await _service.UpdateAsync(id, updateDto);
                if (assignment == null)
                    return NotFound(new { message = "Assignment not found" });

                return Ok(assignment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                    return NotFound(new { message = "Assignment not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpGet("exists")]
        public async Task<ActionResult<bool>> CheckExists([FromQuery] int employeeId, [FromQuery] int kpiDefinitionId, [FromQuery] int khoanPeriodId)
        {
            try
            {
                var exists = await _service.ExistsAsync(employeeId, kpiDefinitionId, khoanPeriodId);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Bulk assign KPIs cho nhiều nhân viên cùng lúc
        /// </summary>
        [HttpPost("bulk-assign")]
        public async Task<ActionResult<BulkAssignmentResultDto>> BulkAssignKpis([FromBody] BulkKpiAssignmentDto request)
        {
            try
            {
                if (request == null || !request.EmployeeIds.Any() || !request.KpiIds.Any())
                {
                    return BadRequest(new { message = "Employee IDs và KPI IDs không được để trống" });
                }

                var result = await _service.BulkAssignKpisAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Bulk update điểm KPI cho nhiều assignments
        /// </summary>
        [HttpPut("bulk-update-scores")]
        public async Task<ActionResult<BulkUpdateResultDto>> BulkUpdateScores([FromBody] BulkScoreUpdateDto request)
        {
            try
            {
                if (request == null || !request.ScoreUpdates.Any())
                {
                    return BadRequest(new { message = "Score updates không được để trống" });
                }

                var result = await _service.BulkUpdateScoresAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Bulk delete KPI assignments
        /// </summary>
        [HttpDelete("bulk-delete")]
        public async Task<ActionResult<BulkDeleteResultDto>> BulkDeleteAssignments([FromBody] BulkDeleteAssignmentDto request)
        {
            try
            {
                if (request == null || !request.AssignmentIds.Any())
                {
                    return BadRequest(new { message = "Assignment IDs không được để trống" });
                }

                var result = await _service.BulkDeleteAssignmentsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }
    }
}