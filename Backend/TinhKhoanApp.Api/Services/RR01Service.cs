using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;
using Khoan.Api.Models.Common;
using Khoan.Api.Models.DataTables;
using Khoan.Api.Models.DTOs.RR01;
using Khoan.Api.Services.Interfaces;

namespace Khoan.Api.Services;

/// <summary>
/// RR01 Service Implementation - Business logic cho Risk Report operations
/// Hỗ trợ 25 business columns với CSV-first architecture
/// </summary>
public class RR01Service : IRR01Service
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RR01Service> _logger;

    public RR01Service(ApplicationDbContext context, ILogger<RR01Service> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<RR01PreviewDto>>> GetPagedAsync(int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var query = _context.RR01
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CREATED_DATE);

            var totalRecords = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => ToPreview(x))
                .ToListAsync();

            var pagedResult = new PagedResult<RR01PreviewDto>
            {
                Items = items,
                TotalCount = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
                // TotalPages is calculated property - no assignment needed
            };

            return ApiResponse<PagedResult<RR01PreviewDto>>.Ok(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged RR01 data");
            return ApiResponse<PagedResult<RR01PreviewDto>>.Error("Lỗi khi lấy dữ liệu RR01 phân trang");
        }
    }

    public async Task<ApiResponse<RR01DetailsDto?>> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _context.RR01.FindAsync(id);
            if (entity == null)
                return ApiResponse<RR01DetailsDto?>.Error("Không tìm thấy RR01 record", "RR01_NOT_FOUND", 404);

            return ApiResponse<RR01DetailsDto?>.Ok(ToDetails(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting RR01 by ID: {Id}", id);
            return ApiResponse<RR01DetailsDto?>.Error("Lỗi khi lấy chi tiết RR01");
        }
    }

    public async Task<ApiResponse<RR01DetailsDto>> CreateAsync(RR01CreateDto dto)
    {
        try
        {
            var entity = ToEntity(dto);
            _context.RR01.Add(entity);
            await _context.SaveChangesAsync();

            return ApiResponse<RR01DetailsDto>.Ok(ToDetails(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating RR01 record");
            return ApiResponse<RR01DetailsDto>.Error("Lỗi khi tạo RR01 record");
        }
    }

    public async Task<ApiResponse<RR01DetailsDto?>> UpdateAsync(int id, RR01UpdateDto dto)
    {
        try
        {
            var entity = await _context.RR01.FindAsync(id);
            if (entity == null)
                return ApiResponse<RR01DetailsDto?>.Error("Không tìm thấy RR01 record", "RR01_NOT_FOUND", 404);

            UpdateEntity(entity, dto);
            await _context.SaveChangesAsync();

            return ApiResponse<RR01DetailsDto?>.Ok(ToDetails(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating RR01 record: {Id}", id);
            return ApiResponse<RR01DetailsDto?>.Error("Lỗi khi cập nhật RR01 record");
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _context.RR01.FindAsync(id);
            if (entity == null)
                return ApiResponse<bool>.Error("Không tìm thấy RR01 record", "RR01_NOT_FOUND", 404);

            _context.RR01.Remove(entity);
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting RR01 record: {Id}", id);
            return ApiResponse<bool>.Error("Lỗi khi xóa RR01 record");
        }
    }

    public async Task<ApiResponse<List<RR01PreviewDto>>> GetByDateAsync(DateTime date)
    {
        try
        {
            var items = await _context.RR01
                .Where(x => x.NGAY_DL.Date == date.Date)
                .OrderBy(x => x.BRCD)
                .ThenBy(x => x.MA_KH)
                .Select(x => ToPreview(x))
                .ToListAsync();

            return ApiResponse<List<RR01PreviewDto>>.Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting RR01 by date: {Date}", date);
            return ApiResponse<List<RR01PreviewDto>>.Error("Lỗi khi lấy dữ liệu RR01 theo ngày");
        }
    }

    public async Task<ApiResponse<List<RR01PreviewDto>>> GetByBranchAsync(string branchCode)
    {
        try
        {
            var items = await _context.RR01
                .Where(x => x.BRCD == branchCode)
                .OrderByDescending(x => x.NGAY_DL)
                .Select(x => ToPreview(x))
                .ToListAsync();

            return ApiResponse<List<RR01PreviewDto>>.Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting RR01 by branch: {BranchCode}", branchCode);
            return ApiResponse<List<RR01PreviewDto>>.Error("Lỗi khi lấy dữ liệu RR01 theo chi nhánh");
        }
    }

    public async Task<ApiResponse<List<RR01PreviewDto>>> GetByCustomerAsync(string customerCode)
    {
        try
        {
            var items = await _context.RR01
                .Where(x => x.MA_KH == customerCode)
                .OrderByDescending(x => x.NGAY_DL)
                .Select(x => ToPreview(x))
                .ToListAsync();

            return ApiResponse<List<RR01PreviewDto>>.Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting RR01 by customer: {CustomerCode}", customerCode);
            return ApiResponse<List<RR01PreviewDto>>.Error("Lỗi khi lấy dữ liệu RR01 theo khách hàng");
        }
    }

    public async Task<ApiResponse<RR01ProcessingSummaryDto>> GetProcessingSummaryAsync(DateTime date)
    {
        try
        {
            var list = await _context.RR01
                .Where(x => x.NGAY_DL.Date == date.Date)
                .ToListAsync();

            if (!list.Any())
            {
                return ApiResponse<RR01ProcessingSummaryDto>.Ok(new RR01ProcessingSummaryDto
                {
                    Date = date,
                    TotalProcessingRecords = 0
                });
            }

            var summary = new RR01ProcessingSummaryDto
            {
                Date = date,
                TotalProcessingRecords = list.Count,
                TotalProcessingAmount = list.Sum(x => (x.THU_GOC ?? 0) + (x.THU_LAI ?? 0)),
                AverageProcessingAmount = list.Any() ? list.Average(x => (x.THU_GOC ?? 0) + (x.THU_LAI ?? 0)) : 0,
                VAMCFlaggedCount = list.Count(x => x.VAMC_FLG == "Y"),
                ProcessingByBranch = list.GroupBy(x => x.BRCD ?? "Unknown")
                    .ToDictionary(g => g.Key, g => g.Count()),
                RecoveryByType = new Dictionary<string, decimal>
                {
                    ["THU_GOC"] = list.Sum(x => x.THU_GOC ?? 0),
                    ["THU_LAI"] = list.Sum(x => x.THU_LAI ?? 0),
                    ["BDS"] = list.Sum(x => x.BDS ?? 0),
                    ["DS"] = list.Sum(x => x.DS ?? 0),
                    ["TSK"] = list.Sum(x => x.TSK ?? 0)
                }
            };

            return ApiResponse<RR01ProcessingSummaryDto>.Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting RR01 processing summary for date: {Date}", date);
            return ApiResponse<RR01ProcessingSummaryDto>.Error("Lỗi khi lấy tóm tắt xử lý RR01");
        }
    }

    #region Mapping Methods

    private static RR01PreviewDto ToPreview(RR01 e)
    {
        return new RR01PreviewDto
        {
            Id = e.Id,
            NGAY_DL = e.NGAY_DL,
            CN_LOAI_I = e.CN_LOAI_I,
            BRCD = e.BRCD,
            MA_KH = e.MA_KH,
            TEN_KH = e.TEN_KH,
            SO_LDS = e.SO_LDS,
            CCY = e.CCY,
            DUNO_GOC_HIENTAI = e.DUNO_GOC_HIENTAI,
            DUNO_LAI_HIENTAI = e.DUNO_LAI_HIENTAI,
            THU_GOC = e.THU_GOC,
            THU_LAI = e.THU_LAI,
            CreatedAt = e.CREATED_DATE,
            FileName = e.FILE_NAME
        };
    }

    private static RR01DetailsDto ToDetails(RR01 e)
    {
        return new RR01DetailsDto
        {
            Id = e.Id,
            NGAY_DL = e.NGAY_DL,
            CN_LOAI_I = e.CN_LOAI_I,
            BRCD = e.BRCD,
            MA_KH = e.MA_KH,
            TEN_KH = e.TEN_KH,
            SO_LDS = e.SO_LDS,
            CCY = e.CCY,
            SO_LAV = e.SO_LAV,
            LOAI_KH = e.LOAI_KH,
            NGAY_GIAI_NGAN = e.NGAY_GIAI_NGAN,
            NGAY_DEN_HAN = e.NGAY_DEN_HAN,
            VAMC_FLG = e.VAMC_FLG,
            NGAY_XLRR = e.NGAY_XLRR,
            DUNO_GOC_BAN_DAU = e.DUNO_GOC_BAN_DAU,
            DUNO_LAI_TICHLUY_BD = e.DUNO_LAI_TICHLUY_BD,
            DOC_DAUKY_DA_THU_HT = e.DOC_DAUKY_DA_THU_HT,
            DUNO_GOC_HIENTAI = e.DUNO_GOC_HIENTAI,
            DUNO_LAI_HIENTAI = e.DUNO_LAI_HIENTAI,
            DUNO_NGAN_HAN = e.DUNO_NGAN_HAN,
            DUNO_TRUNG_HAN = e.DUNO_TRUNG_HAN,
            DUNO_DAI_HAN = e.DUNO_DAI_HAN,
            THU_GOC = e.THU_GOC,
            THU_LAI = e.THU_LAI,
            BDS = e.BDS,
            DS = e.DS,
            TSK = e.TSK,
            FILE_NAME = e.FILE_NAME,
            CreatedAt = e.CREATED_DATE,
            UpdatedAt = e.UPDATED_DATE
        };
    }

    private static RR01 ToEntity(RR01CreateDto d)
    {
        return new RR01
        {
            NGAY_DL = d.NGAY_DL,
            CN_LOAI_I = d.CN_LOAI_I,
            BRCD = d.BRCD,
            MA_KH = d.MA_KH,
            TEN_KH = d.TEN_KH,
            SO_LDS = d.SO_LDS,
            CCY = d.CCY,
            SO_LAV = d.SO_LAV,
            LOAI_KH = d.LOAI_KH,
            NGAY_GIAI_NGAN = d.NGAY_GIAI_NGAN,
            NGAY_DEN_HAN = d.NGAY_DEN_HAN,
            VAMC_FLG = d.VAMC_FLG,
            NGAY_XLRR = d.NGAY_XLRR,
            DUNO_GOC_BAN_DAU = d.DUNO_GOC_BAN_DAU,
            DUNO_LAI_TICHLUY_BD = d.DUNO_LAI_TICHLUY_BD,
            DOC_DAUKY_DA_THU_HT = d.DOC_DAUKY_DA_THU_HT,
            DUNO_GOC_HIENTAI = d.DUNO_GOC_HIENTAI,
            DUNO_LAI_HIENTAI = d.DUNO_LAI_HIENTAI,
            DUNO_NGAN_HAN = d.DUNO_NGAN_HAN,
            DUNO_TRUNG_HAN = d.DUNO_TRUNG_HAN,
            DUNO_DAI_HAN = d.DUNO_DAI_HAN,
            THU_GOC = d.THU_GOC,
            THU_LAI = d.THU_LAI,
            BDS = d.BDS,
            DS = d.DS,
            TSK = d.TSK,
            FILE_NAME = d.FILE_NAME,
            CREATED_DATE = DateTime.UtcNow
        };
    }

    private static void UpdateEntity(RR01 e, RR01UpdateDto d)
    {
        e.NGAY_DL = d.NGAY_DL;
        e.CN_LOAI_I = d.CN_LOAI_I;
        e.BRCD = d.BRCD;
        e.MA_KH = d.MA_KH;
        e.TEN_KH = d.TEN_KH;
        e.SO_LDS = d.SO_LDS;
        e.CCY = d.CCY;
        e.SO_LAV = d.SO_LAV;
        e.LOAI_KH = d.LOAI_KH;
        e.NGAY_GIAI_NGAN = d.NGAY_GIAI_NGAN;
        e.NGAY_DEN_HAN = d.NGAY_DEN_HAN;
        e.VAMC_FLG = d.VAMC_FLG;
        e.NGAY_XLRR = d.NGAY_XLRR;
        e.DUNO_GOC_BAN_DAU = d.DUNO_GOC_BAN_DAU;
        e.DUNO_LAI_TICHLUY_BD = d.DUNO_LAI_TICHLUY_BD;
        e.DOC_DAUKY_DA_THU_HT = d.DOC_DAUKY_DA_THU_HT;
        e.DUNO_GOC_HIENTAI = d.DUNO_GOC_HIENTAI;
        e.DUNO_LAI_HIENTAI = d.DUNO_LAI_HIENTAI;
        e.DUNO_NGAN_HAN = d.DUNO_NGAN_HAN;
        e.DUNO_TRUNG_HAN = d.DUNO_TRUNG_HAN;
        e.DUNO_DAI_HAN = d.DUNO_DAI_HAN;
        e.THU_GOC = d.THU_GOC;
        e.THU_LAI = d.THU_LAI;
        e.BDS = d.BDS;
        e.DS = d.DS;
        e.TSK = d.TSK;
        e.FILE_NAME = d.FILE_NAME;
        e.UPDATED_DATE = DateTime.UtcNow;
        // CREATED_DATE left as original creation timestamp
    }

    #endregion
}
