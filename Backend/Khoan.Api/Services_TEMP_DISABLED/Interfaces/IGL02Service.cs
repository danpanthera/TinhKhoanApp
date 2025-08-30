using Khoan.Api.Models.Dtos;
using Khoan.Api.Models.Common;
using Khoan.Api.Models.Dtos.GL02;

namespace Khoan.Api.Services.Interfaces
{
    /// <summary>
    /// GL02 Service Interface - General Ledger Summary table
    /// 17 business columns with temporal tracking
    /// </summary>
    public interface IGL02Service
    {
        // Preview operations
        Task<ApiResponse<PagedResult<GL02PreviewDto>>> GetPreviewAsync(int page = 1, int pageSize = 10, DateTime? trDate = null);

        // CRUD operations
        Task<ApiResponse<GL02DetailsDto>> GetByIdAsync(long id);
        Task<ApiResponse<GL02DetailsDto>> CreateAsync(GL02CreateDto createDto);
        Task<ApiResponse<GL02DetailsDto>> UpdateAsync(long id, GL02UpdateDto updateDto);
        Task<ApiResponse<bool>> DeleteAsync(long id);
        Task<ApiResponse<List<GL02DetailsDto>>> GetByDateAsync(DateTime trDate);
        Task<ApiResponse<List<GL02DetailsDto>>> GetByAccountAsync(string accountCode);

        // Import operations
        Task<ApiResponse<GL02ImportResultDto>> ImportCsvAsync(IFormFile csvFile);
        Task<ApiResponse<CsvValidationResult>> ValidateCsvAsync(IFormFile csvFile);
        Task<ApiResponse<GL02ImportResultDto>> ImportBatchAsync(List<GL02CreateDto> createDtos);

        // Business logic operations
        Task<ApiResponse<GL02SummaryDto>> GetSummaryAsync(DateTime trDate);
        Task<ApiResponse<List<GL02PreviewDto>>> GetTransactionsByBranchAsync(string branchCode, DateTime trDate);
        Task<ApiResponse<decimal>> GetTotalDebitAmountAsync(DateTime trDate);
        Task<ApiResponse<decimal>> GetTotalCreditAmountAsync(DateTime trDate);
        Task<ApiResponse<Dictionary<string, decimal>>> GetBalanceByAccountAsync(DateTime trDate);

        // Temporal operations
        Task<ApiResponse<List<GL02DetailsDto>>> GetHistoryAsync(long id);
        Task<ApiResponse<GL02DetailsDto>> GetAsOfDateAsync(long id, DateTime asOfDate);

        // System operations
        Task<ApiResponse<bool>> IsHealthyAsync();
    }
}
