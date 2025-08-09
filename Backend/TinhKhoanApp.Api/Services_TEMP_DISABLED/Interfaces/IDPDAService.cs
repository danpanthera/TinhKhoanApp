using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.DTOs.DPDA;

namespace TinhKhoanApp.Api.Services.Interfaces
{
    /// <summary>
    /// DPDA Service Interface - Debit Card table
    /// 13 business columns with temporal tracking
    /// </summary>
    public interface IDPDAService
    {
        // Preview operations
        Task<ApiResponse<PagedResult<DPDAPreviewDto>>> GetPreviewAsync(int page = 1, int pageSize = 10, DateTime? ngayDL = null);

        // CRUD operations
        Task<ApiResponse<DPDADetailsDto>> GetByIdAsync(long id);
        Task<ApiResponse<DPDADetailsDto>> CreateAsync(DPDACreateDto createDto);
        Task<ApiResponse<DPDADetailsDto>> UpdateAsync(long id, DPDAUpdateDto updateDto);
        Task<ApiResponse<bool>> DeleteAsync(long id);
        Task<ApiResponse<List<DPDADetailsDto>>> GetByBranchAsync(string branchCode);
        Task<ApiResponse<List<DPDADetailsDto>>> GetByCustomerAsync(string customerId);

        // Import operations
        Task<ApiResponse<DPDAImportResultDto>> ImportCsvAsync(IFormFile csvFile);
        Task<ApiResponse<CsvValidationResult>> ValidateCsvAsync(IFormFile csvFile);
        Task<ApiResponse<DPDAImportResultDto>> ImportBatchAsync(List<DPDACreateDto> createDtos);

        // Business logic operations
        Task<ApiResponse<DPDASummaryDto>> GetSummaryAsync();
        Task<ApiResponse<List<DPDAPreviewDto>>> GetCardsByStatusAsync(string status);
        Task<ApiResponse<Dictionary<string, long>>> GetCardStatisticsAsync();
        Task<ApiResponse<long>> GetActiveCardCountAsync();

        // Temporal operations
        Task<ApiResponse<List<DPDADetailsDto>>> GetHistoryAsync(long id);
        Task<ApiResponse<DPDADetailsDto>> GetAsOfDateAsync(long id, DateTime asOfDate);

        // System operations
        Task<ApiResponse<bool>> IsHealthyAsync();
    }
}
