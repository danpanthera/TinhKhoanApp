using Microsoft.AspNetCore.Mvc;
using Khoan.Api.Models.Common;

namespace Khoan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly ILogger<RolesController> _logger;

    public RolesController(ILogger<RolesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all roles
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetRoles()
    {
        try
        {
            _logger.LogInformation("🔐 [Roles] Fetching all roles");

            // Mock data for now - replace with real data source later
            var roles = new List<object>
            {
                new { 
                    Id = 1, 
                    Code = "ADMIN", 
                    Name = "Administrator", 
                    Description = "Full system access",
                    Permissions = new[] { "READ", "WRITE", "DELETE", "ADMIN" },
                    IsActive = true 
                },
                new { 
                    Id = 2, 
                    Code = "MANAGER", 
                    Name = "Manager", 
                    Description = "Management level access",
                    Permissions = new[] { "READ", "WRITE", "APPROVE" },
                    IsActive = true 
                },
                new { 
                    Id = 3, 
                    Code = "USER", 
                    Name = "User", 
                    Description = "Basic user access",
                    Permissions = new[] { "READ" },
                    IsActive = true 
                },
                new { 
                    Id = 4, 
                    Code = "ACCOUNTANT", 
                    Name = "Accountant", 
                    Description = "Accounting department access",
                    Permissions = new[] { "READ", "WRITE", "FINANCE" },
                    IsActive = true 
                },
                new { 
                    Id = 5, 
                    Code = "AUDITOR", 
                    Name = "Auditor", 
                    Description = "Read-only audit access",
                    Permissions = new[] { "READ", "AUDIT" },
                    IsActive = true 
                },
                new { 
                    Id = 6, 
                    Code = "DATAENTRY", 
                    Name = "Data Entry", 
                    Description = "Data import and entry access",
                    Permissions = new[] { "READ", "IMPORT", "ENTRY" },
                    IsActive = true 
                }
            };

            _logger.LogInformation("✅ [Roles] Returning {Count} roles", roles.Count);

            return Ok(ApiResponse<List<object>>.Ok(roles, "Roles retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [Roles] Error fetching roles: {Error}", ex.Message);
            return StatusCode(500, ApiResponse<List<object>>.Error("Internal server error while fetching roles", 500));
        }
    }

    /// <summary>
    /// Get role by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> GetRole(int id)
    {
        try
        {
            _logger.LogInformation("🔐 [Roles] Fetching role with ID: {RoleId}", id);

            // Mock data - replace with real lookup later
            if (id <= 0)
            {
                return NotFound(ApiResponse<object>.Error($"Role with ID {id} not found", 404));
            }

            var role = new { 
                Id = id, 
                Code = $"ROLE{id}", 
                Name = $"Role {id}", 
                Description = $"Role {id} description",
                Permissions = new[] { "READ" },
                IsActive = true 
            };

            _logger.LogInformation("✅ [Roles] Found role: {RoleName}", role.Name);

            return Ok(ApiResponse<object>.Ok(role, "Role retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [Roles] Error fetching role {RoleId}: {Error}", id, ex.Message);
            return StatusCode(500, ApiResponse<object>.Error("Internal server error while fetching role", 500));
        }
    }

    /// <summary>
    /// Get roles by permission
    /// </summary>
    [HttpGet("by-permission/{permission}")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetRolesByPermission(string permission)
    {
        try
        {
            _logger.LogInformation("🔐 [Roles] Fetching roles with permission: {Permission}", permission);

            // Mock data filtered by permission - replace with real query later
            var allRoles = await GetRoles();
            if (allRoles.Result is OkObjectResult okResult && 
                okResult.Value is ApiResponse<List<object>> response && 
                response.Data != null)
            {
                // Filter roles by permission (this is mock logic)
                var filteredRoles = response.Data.Where(role => 
                {
                    var roleObj = role as dynamic;
                    var permissions = roleObj?.Permissions as string[];
                    return permissions?.Contains(permission.ToUpper()) == true;
                }).ToList();

                _logger.LogInformation("✅ [Roles] Found {Count} roles with permission {Permission}", filteredRoles.Count, permission);

                return Ok(ApiResponse<List<object>>.Ok(filteredRoles, $"Roles with permission '{permission}' retrieved successfully"));
            }

            return Ok(ApiResponse<List<object>>.Ok(new List<object>(), "No roles found with this permission"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [Roles] Error fetching roles with permission {Permission}: {Error}", permission, ex.Message);
            return StatusCode(500, ApiResponse<List<object>>.Error("Internal server error while fetching roles by permission", 500));
        }
    }
}