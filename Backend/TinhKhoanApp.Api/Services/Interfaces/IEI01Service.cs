using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.DTOs.EI01;

namespace TinhKhoanApp.Api.Services.Interfaces
{
    /// <summary>
    /// EI01 Service Interface - E-Banking Info table
    /// 24 business columns with temporal tracking
    /// </summary>
    public interface IEI01Service
    {
        // Preview operations
        Task<ApiResponse<PagedResult<DPDAPreviewDto>>> GetPreviewAsync(int page = 1, int pageSize = 10, string? branchCode = null);

        // CRUD operations
        Task<ApiResponse<EI01DetailsDto>> GetByIdAsync(long id);
        Task<ApiResponse<EI01DetailsDto>> CreateAsync(EI01CreateDto createDto);
        Task<ApiResponse<EI01DetailsDto>> UpdateAsync(long id, EI01UpdateDto updateDto);
        Task<ApiResponse<bool>> DeleteAsync(long id);
        Task<ApiResponse<List<EI01DetailsDto>>> GetByBranchAsync(string branchCode);
        Task<ApiResponse<List<EI01DetailsDto>>> GetByCustomerAsync(string customerId);

        // Import operations
        Task<ApiResponse<EI01ImportResultDto>> ImportCsvAsync(IFormFile csvFile);
        Task<ApiResponse<CsvValidationResult>> ValidateCsvAsync(IFormFile csvFile);
        Task<ApiResponse<EI01ImportResultDto>> ImportBatchAsync(List<EI01CreateDto> createDtos);

        // Business logic operations
        Task<ApiResponse<EI01SummaryDto>> GetSummaryAsync();
        Task<ApiResponse<List<EI01PreviewDto>>> GetUsersByServiceAsync(string serviceType);
        Task<ApiResponse<Dictionary<string, long>>> GetServiceStatisticsAsync();
        Task<ApiResponse<long>> GetActiveUsersCountAsync(string serviceType);
        Task<ApiResponse<Dictionary<string, long>>> GetUsersByBranchAsync();

        // Temporal operations
        Task<ApiResponse<List<EI01DetailsDto>>> GetHistoryAsync(long id);
        Task<ApiResponse<EI01DetailsDto>> GetAsOfDateAsync(long id, DateTime asOfDate);

        // System operations
        Task<ApiResponse<bool>> IsHealthyAsync();
    }
}
