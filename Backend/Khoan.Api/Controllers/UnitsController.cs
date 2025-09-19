using Microsoft.AspNetCore.Mvc;
using Khoan.Api.Models.Common;

namespace Khoan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnitsController : ControllerBase
{
    private readonly ILogger<UnitsController> _logger;

    // DTO cho đơn vị
    public record UnitDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Code { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty; // CNL1, CNL2, PNVL1, PNVL2, PGDL2, ...
        public int? ParentId { get; init; }
    }

    // Danh sách chuẩn (xóa toàn bộ mock cũ)
    private static readonly List<UnitDto> s_units = new()
    {
        // Tên, Id, Code, Type, ParentId
        new UnitDto { Name = "Chi nhánh Lai Châu", Id = 1, Code = "CnLaiChau", Type = "CNL1", ParentId = null },

        new UnitDto { Name = "Hội Sở", Id = 2, Code = "HoiSo", Type = "CNL2", ParentId = 1 },
        new UnitDto { Name = "Ban Giám đốc", Id = 3, Code = "HoiSoBgd", Type = "PNVL1", ParentId = 2 },
        new UnitDto { Name = "Phòng Khách hàng Doanh nghiệp", Id = 4, Code = "HoiSoKhdn", Type = "PNVL1", ParentId = 2 },
        new UnitDto { Name = "Phòng Khách hàng Cá nhân", Id = 5, Code = "HoiSoKhcn", Type = "PNVL1", ParentId = 2 },
        new UnitDto { Name = "Phòng Kế toán & Ngân quỹ", Id = 6, Code = "HoiSoKtnq", Type = "PNVL1", ParentId = 2 },
        new UnitDto { Name = "Phòng Tổng hợp", Id = 7, Code = "HoiSoTonghop", Type = "PNVL1", ParentId = 2 },
        new UnitDto { Name = "Phòng Kế hoạch & Quản lý rủi ro", Id = 8, Code = "HoiSoKhqlrr", Type = "PNVL1", ParentId = 2 },
        new UnitDto { Name = "Phòng Kiểm tra giám sát", Id = 9, Code = "HoiSoKtgs", Type = "PNVL1", ParentId = 2 },

        new UnitDto { Name = "Chi nhánh Bình Lư", Id = 10, Code = "CnBinhLu", Type = "CNL2", ParentId = 1 },
        new UnitDto { Name = "Ban Giám đốc", Id = 11, Code = "CnBinhLuBgd", Type = "PNVL2", ParentId = 10 },
        new UnitDto { Name = "Phòng Kế toán & Ngân quỹ", Id = 12, Code = "CnBinhLuKtnq", Type = "PNVL2", ParentId = 10 },
        new UnitDto { Name = "Phòng Khách hàng", Id = 13, Code = "CnBinhLuKh", Type = "PNVL2", ParentId = 10 },

        new UnitDto { Name = "Chi nhánh Phong Thổ", Id = 14, Code = "CnPhongTho", Type = "CNL2", ParentId = 1 },
        new UnitDto { Name = "Ban Giám đốc", Id = 15, Code = "CnPhongThoBgd", Type = "PNVL2", ParentId = 14 },
        new UnitDto { Name = "Phòng Kế toán & Ngân quỹ", Id = 16, Code = "CnPhongThoKtnq", Type = "PNVL2", ParentId = 14 },
        new UnitDto { Name = "Phòng Khách hàng", Id = 17, Code = "CnPhongThoKh", Type = "PNVL2", ParentId = 14 },
        new UnitDto { Name = "Phòng giao dịch Số 5", Id = 18, Code = "CnPhongThoPgdSo5", Type = "PGDL2", ParentId = 14 },

        new UnitDto { Name = "Chi nhánh Sìn Hồ", Id = 19, Code = "CnSinHo", Type = "CNL2", ParentId = 1 },
        new UnitDto { Name = "Ban Giám đốc", Id = 20, Code = "CnSinHoBgd", Type = "PNVL2", ParentId = 19 },
        new UnitDto { Name = "Phòng Kế toán & Ngân quỹ", Id = 21, Code = "CnSinHoKtnq", Type = "PNVL2", ParentId = 19 },
        new UnitDto { Name = "Phòng Khách hàng", Id = 22, Code = "CnSinHoKh", Type = "PNVL2", ParentId = 19 },

        new UnitDto { Name = "Chi nhánh Bum Tở", Id = 23, Code = "CnBumTo", Type = "CNL2", ParentId = 1 },
        new UnitDto { Name = "Ban Giám đốc", Id = 24, Code = "CnBumToBgd", Type = "PNVL2", ParentId = 23 },
        new UnitDto { Name = "Phòng Kế toán & Ngân quỹ", Id = 25, Code = "CnBumToKtnq", Type = "PNVL2", ParentId = 23 },
        new UnitDto { Name = "Phòng Khách hàng", Id = 26, Code = "CnBumToKh", Type = "PNVL2", ParentId = 23 },

        new UnitDto { Name = "Chi nhánh Than Uyên", Id = 27, Code = "CnThanUyen", Type = "CNL2", ParentId = 1 },
        new UnitDto { Name = "Ban Giám đốc", Id = 28, Code = "CnThanUyenBgd", Type = "PNVL2", ParentId = 27 },
        new UnitDto { Name = "Phòng Kế toán & Ngân quỹ", Id = 29, Code = "CnThanUyenKtnq", Type = "PNVL2", ParentId = 27 },
        new UnitDto { Name = "Phòng Khách hàng", Id = 30, Code = "CnThanUyenKh", Type = "PNVL2", ParentId = 27 },
        new UnitDto { Name = "Phòng giao dịch số 6", Id = 31, Code = "CnThanUyenPgdSo6", Type = "PGDL2", ParentId = 27 },

        new UnitDto { Name = "Chi nhánh Đoàn Kết", Id = 32, Code = "CnDoanKet", Type = "CNL2", ParentId = 1 },
        new UnitDto { Name = "Ban Giám đốc", Id = 33, Code = "CnDoanKetBgd", Type = "PNVL2", ParentId = 32 },
        new UnitDto { Name = "Phòng Kế toán & Ngân quỹ", Id = 34, Code = "CnDoanKetKtnq", Type = "PNVL2", ParentId = 32 },
        new UnitDto { Name = "Phòng Khách hàng", Id = 35, Code = "CnDoanKetKh", Type = "PNVL2", ParentId = 32 },
        new UnitDto { Name = "Phòng giao dịch số 1", Id = 36, Code = "CnDoanKetPgdso1", Type = "PGDL2", ParentId = 32 },
        new UnitDto { Name = "Phòng giao dịch số 2", Id = 37, Code = "CnDoanKetPgdso2", Type = "PGDL2", ParentId = 32 },

        new UnitDto { Name = "Chi nhánh Tân Uyên", Id = 38, Code = "CnTanUyen", Type = "CNL2", ParentId = 1 },
        new UnitDto { Name = "Ban Giám đốc", Id = 39, Code = "CnTanUyenBgd", Type = "PNVL2", ParentId = 38 },
        new UnitDto { Name = "Phòng Kế toán & Ngân quỹ", Id = 40, Code = "CnTanUyenKtnq", Type = "PNVL2", ParentId = 38 },
        new UnitDto { Name = "Phòng Khách hàng", Id = 41, Code = "CnTanUyenKh", Type = "PNVL2", ParentId = 38 },
        new UnitDto { Name = "Phòng giao dịch số 3", Id = 42, Code = "CnTanUyenPgdso3", Type = "PGDL2", ParentId = 38 },

        new UnitDto { Name = "Chi nhánh Nậm Hàng", Id = 43, Code = "CnNamHang", Type = "CNL2", ParentId = 1 },
        new UnitDto { Name = "Ban Giám đốc", Id = 44, Code = "CnNamHangBgd", Type = "PNVL2", ParentId = 43 },
        new UnitDto { Name = "Phòng Kế toán & Ngân quỹ", Id = 45, Code = "CnNamHangKtnq", Type = "PNVL2", ParentId = 43 },
        new UnitDto { Name = "Phòng Khách hàng", Id = 46, Code = "CnNamHangKh", Type = "PNVL2", ParentId = 43 },
    };

    public UnitsController(ILogger<UnitsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all organizational units/departments
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<UnitDto>>>> GetUnits()
    {
        try
        {
            _logger.LogInformation("🏢 [Units] Trả về danh sách chuẩn: {Count} đơn vị", s_units.Count);
            return Ok(ApiResponse<List<UnitDto>>.Ok(s_units, "Lấy danh sách đơn vị thành công"));
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
            _logger.LogInformation("🏢 [Units] Tìm đơn vị theo ID: {UnitId}", id);

            var unit = s_units.FirstOrDefault(u => u.Id == id);
            if (unit is null)
            {
                return NotFound(ApiResponse<UnitDto>.Error($"Không tìm thấy đơn vị với ID {id}", 404));
            }

            _logger.LogInformation("✅ [Units] Tìm thấy đơn vị: {UnitName}", unit.Name);
            return Ok(ApiResponse<UnitDto>.Ok(unit, "Lấy đơn vị thành công"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [Units] Lỗi lấy đơn vị {UnitId}: {Error}", id, ex.Message);
            return StatusCode(500, ApiResponse<UnitDto>.Error("Lỗi hệ thống khi lấy đơn vị", 500));
        }
    }
}