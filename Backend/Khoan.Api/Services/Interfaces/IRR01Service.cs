using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.DTOs.RR01;

namespace TinhKhoanApp.Api.Services.Interfaces;

/// <summary>
/// Interface cho RR01 Service - Risk Report Business Logic
/// Hỗ trợ 25 business columns với CSV-first architecture
/// </summary>
public interface IRR01Service
{
    // CRUD Operations
    Task<ApiResponse<PagedResult<RR01PreviewDto>>> GetPagedAsync(int pageNumber = 1, int pageSize = 10);
    Task<ApiResponse<RR01DetailsDto?>> GetByIdAsync(int id);
    Task<ApiResponse<RR01DetailsDto>> CreateAsync(RR01CreateDto dto);
    Task<ApiResponse<RR01DetailsDto?>> UpdateAsync(int id, RR01UpdateDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);

    // Query Operations
    Task<ApiResponse<List<RR01PreviewDto>>> GetByDateAsync(DateTime date);
    Task<ApiResponse<List<RR01PreviewDto>>> GetByBranchAsync(string branchCode);
    Task<ApiResponse<List<RR01PreviewDto>>> GetByCustomerAsync(string customerCode);

    // Business Analytics
    Task<ApiResponse<RR01ProcessingSummaryDto>> GetProcessingSummaryAsync(DateTime date);
}
