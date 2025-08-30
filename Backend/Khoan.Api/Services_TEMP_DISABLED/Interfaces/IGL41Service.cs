using TinhKhoanApp.Api.Models.Dtos;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.Dtos.GL41;

namespace TinhKhoanApp.Api.Services.Interfaces
{
    /// <summary>
    /// GL41 Service Interface - General Ledger Balance table
    /// 13 business columns with temporal tracking
    /// </summary>
    public interface IGL41Service
    {
        // Preview operations
        Task<ApiResponse<PagedResult<GL41PreviewDto>>> GetPreviewAsync(int page = 1, int pageSize = 10, string? branchCode = null);

        // CRUD operations
        Task<ApiResponse<GL41DetailsDto>> GetByIdAsync(long id);
        Task<ApiResponse<GL41DetailsDto>> CreateAsync(GL41CreateDto createDto);
        Task<ApiResponse<GL41DetailsDto>> UpdateAsync(long id, GL41UpdateDto updateDto);
        Task<ApiResponse<bool>> DeleteAsync(long id);
        Task<ApiResponse<List<GL41DetailsDto>>> GetByBranchAsync(string branchCode);
        Task<ApiResponse<List<GL41DetailsDto>>> GetByAccountAsync(string accountCode);

        // Import operations
        Task<ApiResponse<GL41ImportResultDto>> ImportCsvAsync(IFormFile csvFile);
        Task<ApiResponse<CsvValidationResult>> ValidateCsvAsync(IFormFile csvFile);
        Task<ApiResponse<GL41ImportResultDto>> ImportBatchAsync(List<GL41CreateDto> createDtos);

        // Business logic operations
        Task<ApiResponse<GL41SummaryDto>> GetSummaryAsync();
        Task<ApiResponse<List<GL41PreviewDto>>> GetBalancesByBranchAsync(string branchCode);
        Task<ApiResponse<decimal>> GetTotalOpeningBalanceAsync();
        Task<ApiResponse<decimal>> GetTotalClosingBalanceAsync();
        Task<ApiResponse<Dictionary<string, decimal>>> GetBalanceByAccountTypeAsync();

        // Temporal operations
        Task<ApiResponse<List<GL41DetailsDto>>> GetHistoryAsync(long id);
        Task<ApiResponse<GL41DetailsDto>> GetAsOfDateAsync(long id, DateTime asOfDate);

        // System operations
        Task<ApiResponse<bool>> IsHealthyAsync();
    }
}
