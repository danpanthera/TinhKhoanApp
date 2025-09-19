using Microsoft.AspNetCore.Mvc;
using Khoan.Api.Models.Common;
using Khoan.Api.Repositories;
using Khoan.Api.Models;

namespace Khoan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnitsController : ControllerBase
{
    private readonly ILogger<UnitsController> _logger;
    private readonly IUnitsRepository _unitsRepository;

    public UnitsController(ILogger<UnitsController> logger, IUnitsRepository unitsRepository)
    {
        _logger = logger;
        _unitsRepository = unitsRepository;
    }

    // DTO cho đơn vị
    public record UnitDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Code { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty; // CNL1, CNL2, PNVL1, PNVL2, PGDL2, ...
        public int? ParentId { get; init; }
    }

    // DTO cho cây phân cấp
    public record UnitTreeDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Code { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty;
        public int? ParentId { get; init; }
        public List<UnitTreeDto> Children { get; init; } = new();
    }

    /// <summary>
    /// Get all organizational units/departments
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<UnitDto>>>> GetUnits()
    {
        try
        {
            _logger.LogInformation("🏢 [Units] Lấy danh sách từ DB");
            
            var units = await _unitsRepository.GetAllAsync();
            var unitDtos = units.Select(u => new UnitDto
            {
                Id = u.Id,
                Name = u.Name,
                Code = u.Code,
                Type = u.Type ?? string.Empty,
                ParentId = u.ParentUnitId
            }).ToList();

            _logger.LogInformation("✅ [Units] Trả về {Count} đơn vị từ DB", unitDtos.Count);
            return Ok(ApiResponse<List<UnitDto>>.Ok(unitDtos, "Lấy danh sách đơn vị thành công"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [Units] Lỗi lấy danh sách đơn vị: {Error}", ex.Message);
            return StatusCode(500, ApiResponse<List<UnitDto>>.Error("Lỗi hệ thống khi lấy danh sách đơn vị", 500));
        }
    }

    /// <summary>
    /// Get unit by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UnitDto>>> GetUnit(int id)
    {
        try
        {
            _logger.LogInformation("🏢 [Units] Tìm đơn vị theo ID từ DB: {UnitId}", id);

            var unit = await _unitsRepository.GetByIdAsync(id);
            if (unit is null)
            {
                return NotFound(ApiResponse<UnitDto>.Error($"Không tìm thấy đơn vị với ID {id}", 404));
            }

            var unitDto = new UnitDto
            {
                Id = unit.Id,
                Name = unit.Name,
                Code = unit.Code,
                Type = unit.Type ?? string.Empty,
                ParentId = unit.ParentUnitId
            };

            _logger.LogInformation("✅ [Units] Tìm thấy đơn vị: {UnitName}", unit.Name);
            return Ok(ApiResponse<UnitDto>.Ok(unitDto, "Lấy đơn vị thành công"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [Units] Lỗi lấy đơn vị {UnitId}: {Error}", id, ex.Message);
            return StatusCode(500, ApiResponse<UnitDto>.Error("Lỗi hệ thống khi lấy đơn vị", 500));
        }
    }

    /// <summary>
    /// Get units by parent ID (hierarchical query)
    /// </summary>
    [HttpGet("by-parent/{parentId}")]
    public async Task<ActionResult<ApiResponse<List<UnitDto>>>> GetUnitsByParent(int parentId)
    {
        try
        {
            _logger.LogInformation("🏢 [Units] Lấy đơn vị con từ DB của parent: {ParentId}", parentId);

            var childUnits = await _unitsRepository.GetByParentIdAsync(parentId);
            var childUnitDtos = childUnits.Select(u => new UnitDto
            {
                Id = u.Id,
                Name = u.Name,
                Code = u.Code,
                Type = u.Type ?? string.Empty,
                ParentId = u.ParentUnitId
            }).ToList();
            
            _logger.LogInformation("✅ [Units] Tìm thấy {Count} đơn vị con của parent {ParentId}", childUnitDtos.Count, parentId);
            return Ok(ApiResponse<List<UnitDto>>.Ok(childUnitDtos, $"Lấy {childUnitDtos.Count} đơn vị con thành công"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [Units] Lỗi lấy đơn vị con của {ParentId}: {Error}", parentId, ex.Message);
            return StatusCode(500, ApiResponse<List<UnitDto>>.Error("Lỗi hệ thống khi lấy đơn vị con", 500));
        }
    }

    /// <summary>
    /// Get root units (no parent)
    /// </summary>
    [HttpGet("by-parent/root")]
    public async Task<ActionResult<ApiResponse<List<UnitDto>>>> GetRootUnits()
    {
        try
        {
            _logger.LogInformation("🏢 [Units] Lấy đơn vị gốc từ DB (không có parent)");

            var rootUnits = await _unitsRepository.GetRootUnitsAsync();
            var rootUnitDtos = rootUnits.Select(u => new UnitDto
            {
                Id = u.Id,
                Name = u.Name,
                Code = u.Code,
                Type = u.Type ?? string.Empty,
                ParentId = u.ParentUnitId
            }).ToList();
            
            _logger.LogInformation("✅ [Units] Tìm thấy {Count} đơn vị gốc", rootUnitDtos.Count);
            return Ok(ApiResponse<List<UnitDto>>.Ok(rootUnitDtos, $"Lấy {rootUnitDtos.Count} đơn vị gốc thành công"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [Units] Lỗi lấy đơn vị gốc: {Error}", ex.Message);
            return StatusCode(500, ApiResponse<List<UnitDto>>.Error("Lỗi hệ thống khi lấy đơn vị gốc", 500));
        }
    }

    /// <summary>
    /// Get organizational unit tree (hierarchical structure)
    /// </summary>
    [HttpGet("tree")]
    public async Task<ActionResult<ApiResponse<List<UnitTreeDto>>>> GetUnitsTree()
    {
        try
        {
            _logger.LogInformation("🏢 [Units] Xây dựng cây phân cấp đơn vị từ DB");

            var tree = await BuildUnitTreeAsync();
            
            _logger.LogInformation("✅ [Units] Xây dựng cây thành công với {Count} nút gốc", tree.Count);
            return Ok(ApiResponse<List<UnitTreeDto>>.Ok(tree, "Lấy cây đơn vị thành công"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [Units] Lỗi xây dựng cây đơn vị: {Error}", ex.Message);
            return StatusCode(500, ApiResponse<List<UnitTreeDto>>.Error("Lỗi hệ thống khi xây dựng cây đơn vị", 500));
        }
    }

    private async Task<List<UnitTreeDto>> BuildUnitTreeAsync()
    {
        // Lấy tất cả units từ DB
        var allUnits = await _unitsRepository.GetAllAsync();
        
        // Tạo map từ Unit sang UnitTreeDto
        var treeNodes = allUnits.Select(u => new UnitTreeDto
        {
            Id = u.Id,
            Name = u.Name,
            Code = u.Code,
            Type = u.Type ?? string.Empty,
            ParentId = u.ParentUnitId,
            Children = new List<UnitTreeDto>()
        }).ToList();

        // Tạo dictionary để lookup nhanh
        var nodeDict = treeNodes.ToDictionary(n => n.Id);

        // Xây dựng cây bằng cách gán con vào cha
        var roots = new List<UnitTreeDto>();

        foreach (var node in treeNodes)
        {
            if (node.ParentId == null)
            {
                // Nút gốc
                roots.Add(node);
            }
            else if (nodeDict.TryGetValue(node.ParentId.Value, out var parent))
            {
                // Gán node làm con của parent
                parent.Children.Add(node);
            }
        }

        return roots;
    }
}