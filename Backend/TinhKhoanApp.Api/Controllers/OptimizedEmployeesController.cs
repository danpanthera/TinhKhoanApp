using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Models.Dtos;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Services;
using TinhKhoanApp.Api.Repositories;
using System.Diagnostics;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OptimizedEmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cache;
        private readonly OptimizedEmployeeRepository _repository;
        private readonly IPerformanceMonitorService _performanceMonitor;
        private readonly ILogger<OptimizedEmployeesController> _logger;

        public OptimizedEmployeesController(
            ApplicationDbContext context,
            ICacheService cache,
            OptimizedEmployeeRepository repository,
            IPerformanceMonitorService performanceMonitor,
            ILogger<OptimizedEmployeesController> logger)
        {
            _context = context;
            _cache = cache;
            _repository = repository;
            _performanceMonitor = performanceMonitor;
            _logger = logger;
        }

        /// <summary>
        /// Get paginated employees with caching and filtering
        /// </summary>
        [HttpGet]
        [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "pageNumber", "pageSize", "search", "unitId", "activeOnly" })]
        public async Task<ActionResult<ApiResponse<PagedResult<EmployeeListItemDto>>>> GetEmployees(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? search = null,
            [FromQuery] int? unitId = null,
            [FromQuery] bool activeOnly = true)
        {
            using var operation = _performanceMonitor.TrackOperation("GetEmployees", new Dictionary<string, object>
            {
                ["pageNumber"] = pageNumber,
                ["pageSize"] = pageSize,
                ["hasSearch"] = !string.IsNullOrEmpty(search),
                ["hasUnitFilter"] = unitId.HasValue
            });

            try
            {
                // Validate parameters
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1 || pageSize > 1000) pageSize = 50;

                // Build filter expression
                Expression<Func<Employee, bool>>? filter = null;
                if (activeOnly || !string.IsNullOrWhiteSpace(search) || unitId.HasValue)
                {
                    filter = e => (!activeOnly || e.IsActive) &&
                                 (!unitId.HasValue || e.UnitId == unitId.Value) &&
                                 (string.IsNullOrWhiteSpace(search) ||
                                  e.EmployeeCode.Contains(search) ||
                                  e.FullName.Contains(search) ||
                                  e.CBCode.Contains(search));
                }

                // Get data from optimized repository
                var employees = await _repository.GetPagedAsync(pageNumber, pageSize, filter);

                // Map to DTOs
                var result = new PagedResult<EmployeeListItemDto>
                {
                    Items = employees.Items.Select(e => new EmployeeListItemDto
                    {
                        Id = e.Id,
                        EmployeeCode = e.EmployeeCode ?? "",
                        CBCode = e.CBCode ?? "",
                        FullName = e.FullName ?? "",
                        Username = e.Username ?? "",
                        Email = e.Email ?? "",
                        PhoneNumber = e.PhoneNumber ?? "",
                        IsActive = e.IsActive,
                        UnitId = e.UnitId,
                        UnitName = e.Unit?.Name ?? "",
                        PositionId = e.PositionId,
                        PositionName = e.Position?.Name ?? "",
                        Roles = new List<RoleDto>() // Empty for list view performance
                    }).ToList(),
                    TotalCount = employees.TotalCount,
                    PageNumber = employees.PageNumber,
                    PageSize = employees.PageSize
                };

                var response = new ApiResponse<PagedResult<EmployeeListItemDto>>
                {
                    Success = true,
                    Data = result,
                    Message = $"Retrieved {result.Items.Count} employees (page {pageNumber} of {result.TotalPages})"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employees");
                return StatusCode(500, new ApiResponse<PagedResult<EmployeeListItemDto>>
                {
                    Success = false,
                    Message = "Error retrieving employees",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get employee by ID with caching
        /// </summary>
        [HttpGet("{id}")]
        [ResponseCache(Duration = 300)]
        public async Task<ActionResult<ApiResponse<EmployeeDetailDto>>> GetEmployee(int id)
        {
            using var operation = _performanceMonitor.TrackOperation("GetEmployeeById", new Dictionary<string, object>
            {
                ["employeeId"] = id
            });

            try
            {
                var employee = await _repository.GetByIdAsync(id);

                if (employee == null)
                {
                    return NotFound(new ApiResponse<EmployeeDetailDto>
                    {
                        Success = false,
                        Message = $"Employee with ID {id} not found"
                    });
                }

                // Get roles separately for detailed view
                var roles = await _context.EmployeeRoles
                    .AsNoTracking()
                    .Where(er => er.EmployeeId == id && er.IsActive)
                    .Include(er => er.Role)
                    .Select(er => new RoleDto
                    {
                        Id = er.Role!.Id,
                        Name = er.Role.Name,
                        Description = er.Role.Description ?? ""
                    })
                    .ToListAsync();

                var result = new EmployeeDetailDto
                {
                    Id = employee.Id,
                    EmployeeCode = employee.EmployeeCode ?? "",
                    CBCode = employee.CBCode ?? "",
                    FullName = employee.FullName ?? "",
                    Username = employee.Username ?? "",
                    Email = employee.Email ?? "",
                    PhoneNumber = employee.PhoneNumber ?? "",
                    IsActive = employee.IsActive,
                    UnitId = employee.UnitId,
                    UnitName = employee.Unit?.Name ?? "",
                    PositionId = employee.PositionId,
                    PositionName = employee.Position?.Name ?? "",
                    Roles = roles
                };

                return Ok(new ApiResponse<EmployeeDetailDto>
                {
                    Success = true,
                    Data = result,
                    Message = "Employee retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employee {EmployeeId}", id);
                return StatusCode(500, new ApiResponse<EmployeeDetailDto>
                {
                    Success = false,
                    Message = "Error retrieving employee",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get employees by unit with caching
        /// </summary>
        [HttpGet("by-unit/{unitId}")]
        [ResponseCache(Duration = 600)] // Cache longer for unit-based queries
        public async Task<ActionResult<ApiResponse<List<EmployeeListItemDto>>>> GetEmployeesByUnit(int unitId)
        {
            using var operation = _performanceMonitor.TrackOperation("GetEmployeesByUnit", new Dictionary<string, object>
            {
                ["unitId"] = unitId
            });

            try
            {
                var employees = await _repository.GetByFilterAsync(e => e.UnitId == unitId && e.IsActive);

                var result = employees.Select(e => new EmployeeListItemDto
                {
                    Id = e.Id,
                    EmployeeCode = e.EmployeeCode ?? "",
                    CBCode = e.CBCode ?? "",
                    FullName = e.FullName ?? "",
                    Username = e.Username ?? "",
                    Email = e.Email ?? "",
                    PhoneNumber = e.PhoneNumber ?? "",
                    IsActive = e.IsActive,
                    UnitId = e.UnitId,
                    UnitName = e.Unit?.Name ?? "",
                    PositionId = e.PositionId,
                    PositionName = e.Position?.Name ?? "",
                    Roles = new List<RoleDto>()
                }).ToList();

                return Ok(new ApiResponse<List<EmployeeListItemDto>>
                {
                    Success = true,
                    Data = result,
                    Message = $"Retrieved {result.Count} employees for unit {unitId}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employees for unit {UnitId}", unitId);
                return StatusCode(500, new ApiResponse<List<EmployeeListItemDto>>
                {
                    Success = false,
                    Message = "Error retrieving employees",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Create new employee
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<EmployeeDetailDto>>> CreateEmployee([FromBody] CreateEmployeeDto createDto)
        {
            using var operation = _performanceMonitor.TrackOperation("CreateEmployee");

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<EmployeeDetailDto>
                    {
                        Success = false,
                        Message = "Invalid employee data",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                // Check for duplicate employee code
                var existingByCode = await _context.Employees
                    .AnyAsync(e => e.EmployeeCode == createDto.EmployeeCode);

                if (existingByCode)
                {
                    return Conflict(new ApiResponse<EmployeeDetailDto>
                    {
                        Success = false,
                        Message = $"Employee with code {createDto.EmployeeCode} already exists"
                    });
                }

                // Check for duplicate CB code
                var existingByCB = await _context.Employees
                    .AnyAsync(e => e.CBCode == createDto.CBCode);

                if (existingByCB)
                {
                    return Conflict(new ApiResponse<EmployeeDetailDto>
                    {
                        Success = false,
                        Message = $"Employee with CB code {createDto.CBCode} already exists"
                    });
                }

                var employee = new Employee
                {
                    EmployeeCode = createDto.EmployeeCode,
                    CBCode = createDto.CBCode,
                    FullName = createDto.FullName,
                    Username = createDto.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(createDto.Password),
                    Email = createDto.Email,
                    PhoneNumber = createDto.PhoneNumber,
                    IsActive = true,
                    UnitId = createDto.UnitId,
                    PositionId = createDto.PositionId
                };

                var createdEmployee = await _repository.CreateAsync(employee);

                // Get complete employee data
                var result = await _repository.GetByIdAsync(createdEmployee.Id);

                var responseData = new EmployeeDetailDto
                {
                    Id = result!.Id,
                    EmployeeCode = result.EmployeeCode ?? "",
                    CBCode = result.CBCode ?? "",
                    FullName = result.FullName ?? "",
                    Username = result.Username ?? "",
                    Email = result.Email ?? "",
                    PhoneNumber = result.PhoneNumber ?? "",
                    IsActive = result.IsActive,
                    UnitId = result.UnitId,
                    UnitName = result.Unit?.Name ?? "",
                    PositionId = result.PositionId,
                    PositionName = result.Position?.Name ?? "",
                    Roles = new List<RoleDto>()
                };

                return CreatedAtAction(nameof(GetEmployee), new { id = createdEmployee.Id },
                    new ApiResponse<EmployeeDetailDto>
                    {
                        Success = true,
                        Data = responseData,
                        Message = "Employee created successfully"
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee");
                return StatusCode(500, new ApiResponse<EmployeeDetailDto>
                {
                    Success = false,
                    Message = "Error creating employee",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Update employee
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<EmployeeDetailDto>>> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto updateDto)
        {
            using var operation = _performanceMonitor.TrackOperation("UpdateEmployee", new Dictionary<string, object>
            {
                ["employeeId"] = id
            });

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<EmployeeDetailDto>
                    {
                        Success = false,
                        Message = "Invalid employee data",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                {
                    return NotFound(new ApiResponse<EmployeeDetailDto>
                    {
                        Success = false,
                        Message = $"Employee with ID {id} not found"
                    });
                }

                // Update properties
                employee.FullName = updateDto.FullName;
                employee.Email = updateDto.Email;
                employee.PhoneNumber = updateDto.PhoneNumber;
                employee.IsActive = updateDto.IsActive;
                employee.UnitId = updateDto.UnitId;
                employee.PositionId = updateDto.PositionId;

                // Update password if provided
                if (!string.IsNullOrEmpty(updateDto.Password))
                {
                    employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateDto.Password);
                }

                var updatedEmployee = await _repository.UpdateAsync(employee);
                var result = await _repository.GetByIdAsync(updatedEmployee.Id);

                var responseData = new EmployeeDetailDto
                {
                    Id = result!.Id,
                    EmployeeCode = result.EmployeeCode ?? "",
                    CBCode = result.CBCode ?? "",
                    FullName = result.FullName ?? "",
                    Username = result.Username ?? "",
                    Email = result.Email ?? "",
                    PhoneNumber = result.PhoneNumber ?? "",
                    IsActive = result.IsActive,
                    UnitId = result.UnitId,
                    UnitName = result.Unit?.Name ?? "",
                    PositionId = result.PositionId,
                    PositionName = result.Position?.Name ?? "",
                    Roles = new List<RoleDto>()
                };

                return Ok(new ApiResponse<EmployeeDetailDto>
                {
                    Success = true,
                    Data = responseData,
                    Message = "Employee updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee {EmployeeId}", id);
                return StatusCode(500, new ApiResponse<EmployeeDetailDto>
                {
                    Success = false,
                    Message = "Error updating employee",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Delete employee (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteEmployee(int id)
        {
            using var operation = _performanceMonitor.TrackOperation("DeleteEmployee", new Dictionary<string, object>
            {
                ["employeeId"] = id
            });

            try
            {
                var success = await _repository.DeleteAsync(id);

                if (!success)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = $"Employee with ID {id} not found"
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Employee deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee {EmployeeId}", id);
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error deleting employee",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get employee statistics
        /// </summary>
        [HttpGet("stats")]
        [ResponseCache(Duration = 1800)] // Cache for 30 minutes
        public async Task<ActionResult<ApiResponse<EmployeeStatsDto>>> GetEmployeeStats()
        {
            using var operation = _performanceMonitor.TrackOperation("GetEmployeeStats");

            try
            {
                var cacheKey = "employee:stats";
                var cachedStats = await _cache.GetAsync<EmployeeStatsDto>(cacheKey);

                if (cachedStats != null)
                {
                    return Ok(new ApiResponse<EmployeeStatsDto>
                    {
                        Success = true,
                        Data = cachedStats,
                        Message = "Employee statistics retrieved from cache"
                    });
                }

                // Calculate stats
                var totalEmployees = await _repository.CountAsync();
                var activeEmployees = await _repository.CountAsync(e => e.IsActive);
                var inactiveEmployees = totalEmployees - activeEmployees;

                var unitStats = await _context.Employees
                    .AsNoTracking()
                    .Include(e => e.Unit)
                    .GroupBy(e => new { e.UnitId, e.Unit!.Name })
                    .Select(g => new UnitEmployeeCountDto
                    {
                        UnitId = g.Key.UnitId,
                        UnitName = g.Key.Name,
                        EmployeeCount = g.Count(),
                        ActiveCount = g.Count(e => e.IsActive)
                    })
                    .OrderByDescending(s => s.EmployeeCount)
                    .ToListAsync();

                var stats = new EmployeeStatsDto
                {
                    TotalEmployees = totalEmployees,
                    ActiveEmployees = activeEmployees,
                    InactiveEmployees = inactiveEmployees,
                    UnitStats = unitStats
                };

                // Cache for 30 minutes
                await _cache.SetAsync(cacheKey, stats, TimeSpan.FromMinutes(30));

                return Ok(new ApiResponse<EmployeeStatsDto>
                {
                    Success = true,
                    Data = stats,
                    Message = "Employee statistics calculated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating employee statistics");
                return StatusCode(500, new ApiResponse<EmployeeStatsDto>
                {
                    Success = false,
                    Message = "Error calculating statistics",
                    Errors = { ex.Message }
                });
            }
        }
    }
}
