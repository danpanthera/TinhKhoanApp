using Microsoft.AspNetCore.Mvc;
using Khoan.Api.Models.Common;

namespace Khoan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(ILogger<EmployeesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all employees
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetEmployees()
    {
        try
        {
            _logger.LogInformation("👥 [Employees] Fetching all employees");

            // Mock data for now - replace with real data source later
            var employees = new List<object>
            {
                new { 
                    Id = 1, 
                    EmployeeCode = "EMP001", 
                    Name = "Nguyễn Văn A", 
                    Email = "nguyenvana@company.com",
                    Phone = "0901234567",
                    Department = "Kế toán", 
                    Position = "Nhân viên",
                    UnitId = 1,
                    UnitName = "Hội sở",
                    IsActive = true 
                },
                new { 
                    Id = 2, 
                    EmployeeCode = "EMP002", 
                    Name = "Trần Thị B", 
                    Email = "tranthib@company.com",
                    Phone = "0902345678",
                    Department = "Tín dụng", 
                    Position = "Trưởng phòng",
                    UnitId = 2,
                    UnitName = "Chi nhánh Hà Nội",
                    IsActive = true 
                },
                new { 
                    Id = 3, 
                    EmployeeCode = "EMP003", 
                    Name = "Lê Văn C", 
                    Email = "levanc@company.com",
                    Phone = "0903456789",
                    Department = "Kinh doanh", 
                    Position = "Nhân viên",
                    UnitId = 3,
                    UnitName = "Chi nhánh Hồ Chí Minh",
                    IsActive = true 
                },
                new { 
                    Id = 4, 
                    EmployeeCode = "EMP004", 
                    Name = "Phạm Thị D", 
                    Email = "phamthid@company.com",
                    Phone = "0904567890",
                    Department = "Rủi ro", 
                    Position = "Chuyên viên",
                    UnitId = 1,
                    UnitName = "Hội sở",
                    IsActive = true 
                },
                new { 
                    Id = 5, 
                    EmployeeCode = "EMP005", 
                    Name = "Hoàng Văn E", 
                    Email = "hoangvane@company.com",
                    Phone = "0905678901",
                    Department = "IT", 
                    Position = "Developer",
                    UnitId = 1,
                    UnitName = "Hội sở",
                    IsActive = false 
                }
            };

            _logger.LogInformation("✅ [Employees] Returning {Count} employees", employees.Count);

            return Ok(ApiResponse<List<object>>.Ok(employees, "Employees retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [Employees] Error fetching employees: {Error}", ex.Message);
            return StatusCode(500, ApiResponse<List<object>>.Error("Internal server error while fetching employees", 500));
        }
    }

    /// <summary>
    /// Get employee by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> GetEmployee(int id)
    {
        try
        {
            _logger.LogInformation("👥 [Employees] Fetching employee with ID: {EmployeeId}", id);

            // Mock data - replace with real lookup later
            if (id <= 0)
            {
                return NotFound(ApiResponse<object>.Error($"Employee with ID {id} not found", 404));
            }

            var employee = new { 
                Id = id, 
                EmployeeCode = $"EMP{id:000}", 
                Name = $"Employee {id}", 
                Email = $"employee{id}@company.com",
                Phone = $"090{id:0000000}",
                Department = "IT", 
                Position = "Developer",
                UnitId = 1,
                UnitName = "Hội sở",
                IsActive = true 
            };

            _logger.LogInformation("✅ [Employees] Found employee: {EmployeeName}", employee.Name);

            return Ok(ApiResponse<object>.Ok(employee, "Employee retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [Employees] Error fetching employee {EmployeeId}: {Error}", id, ex.Message);
            return StatusCode(500, ApiResponse<object>.Error("Internal server error while fetching employee", 500));
        }
    }

    /// <summary>
    /// Get employees by unit ID
    /// </summary>
    [HttpGet("by-unit/{unitId}")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetEmployeesByUnit(int unitId)
    {
        try
        {
            _logger.LogInformation("👥 [Employees] Fetching employees for unit ID: {UnitId}", unitId);

            // Mock data filtered by unit - replace with real query later
            var allEmployees = await GetEmployees();
            if (allEmployees.Result is OkObjectResult okResult && 
                okResult.Value is ApiResponse<List<object>> response && 
                response.Data != null)
            {
                // Filter employees by unitId (this is mock logic)
                var filteredEmployees = response.Data.Where(emp => 
                {
                    var empObj = emp as dynamic;
                    return empObj?.UnitId == unitId;
                }).ToList();

                _logger.LogInformation("✅ [Employees] Found {Count} employees for unit {UnitId}", filteredEmployees.Count, unitId);

                return Ok(ApiResponse<List<object>>.Ok(filteredEmployees, $"Employees for unit {unitId} retrieved successfully"));
            }

            return Ok(ApiResponse<List<object>>.Ok(new List<object>(), "No employees found for this unit"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [Employees] Error fetching employees for unit {UnitId}: {Error}", unitId, ex.Message);
            return StatusCode(500, ApiResponse<List<object>>.Error("Internal server error while fetching employees by unit", 500));
        }
    }
}