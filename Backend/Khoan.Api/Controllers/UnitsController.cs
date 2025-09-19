using Microsoft.AspNetCore.Mvc;
using Khoan.Api.Models.Common;

namespace Khoan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnitsController : ControllerBase
{
    private readonly ILogger<UnitsController> _logger;

    public UnitsController(ILogger<UnitsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all organizational units/departments
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetUnits()
    {
        try
        {
            _logger.LogInformation("🏢 [Units] Fetching all units");

            // Mock data for now - replace with real data source later
            var units = new List<object>
            {
                new { Id = 1, Code = "HQ", Name = "Hội sở", Description = "Hội sở chính", IsActive = true },
                new { Id = 2, Code = "BR001", Name = "Chi nhánh Hà Nội", Description = "Chi nhánh Hà Nội", IsActive = true },
                new { Id = 3, Code = "BR002", Name = "Chi nhánh Hồ Chí Minh", Description = "Chi nhánh TP.HCM", IsActive = true },
                new { Id = 4, Code = "BR003", Name = "Chi nhánh Đà Nẵng", Description = "Chi nhánh Đà Nẵng", IsActive = true },
                new { Id = 5, Code = "BR004", Name = "Chi nhánh Cần Thơ", Description = "Chi nhánh Cần Thơ", IsActive = true }
            };

            _logger.LogInformation("✅ [Units] Returning {Count} units", units.Count);

            return Ok(ApiResponse<List<object>>.Ok(units, "Units retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [Units] Error fetching units: {Error}", ex.Message);
            return StatusCode(500, ApiResponse<List<object>>.Error("Internal server error while fetching units", 500));
        }
    }

    /// <summary>
    /// Get unit by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> GetUnit(int id)
    {
        try
        {
            _logger.LogInformation("🏢 [Units] Fetching unit with ID: {UnitId}", id);

            // Mock data - replace with real lookup later
            if (id <= 0)
            {
                return NotFound(ApiResponse<object>.Error($"Unit with ID {id} not found", 404));
            }

            var unit = new { Id = id, Code = $"UNIT{id:000}", Name = $"Unit {id}", Description = $"Unit description {id}", IsActive = true };

            _logger.LogInformation("✅ [Units] Found unit: {UnitName}", unit.Name);

            return Ok(ApiResponse<object>.Ok(unit, "Unit retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [Units] Error fetching unit {UnitId}: {Error}", id, ex.Message);
            return StatusCode(500, ApiResponse<object>.Error("Internal server error while fetching unit", 500));
        }
    }
}