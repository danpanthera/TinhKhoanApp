using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.DTOs.GL01;

namespace TinhKhoanApp.Api.Services.Interfaces
{
    /// <summary>
    /// GL01 Service Interface - General Ledger Detail table
    /// 27 business columns with temporal tracking
    /// </summary>
    public interface IGL01Service
    {
        // Preview operations
        Task<ApiResponse<PagedResult<GL01PreviewDto>>> GetPreviewAsync(int page = 1, int pageSize = 10, DateTime? ngayGD = null);

        // CRUD operations
        Task<ApiResponse<GL01DetailsDto>> GetByIdAsync(long id);
        Task<ApiResponse<GL01DetailsDto>> CreateAsync(GL01CreateDto createDto);
        Task<ApiResponse<GL01DetailsDto>> UpdateAsync(long id, GL01UpdateDto updateDto);
        Task<ApiResponse<bool>> DeleteAsync(long id);
        Task<ApiResponse<List<GL01DetailsDto>>> GetByDateAsync(DateTime ngayGD);
        Task<ApiResponse<List<GL01DetailsDto>>> GetByAccountAsync(string accountNumber);

        // Import operations
        Task<ApiResponse<GL01ImportResultDto>> ImportCsvAsync(IFormFile csvFile);
        Task<ApiResponse<CsvValidationResult>> ValidateCsvAsync(IFormFile csvFile);
        Task<ApiResponse<GL01ImportResultDto>> ImportBatchAsync(List<GL01CreateDto> createDtos);

        // Business logic operations
        Task<ApiResponse<GL01SummaryDto>> GetSummaryAsync(DateTime ngayGD);
        Task<ApiResponse<List<GL01PreviewDto>>> GetTransactionsByTypeAsync(string transactionType, DateTime ngayGD);
        Task<ApiResponse<decimal>> GetTotalDebitAmountAsync(DateTime ngayGD);
        Task<ApiResponse<decimal>> GetTotalCreditAmountAsync(DateTime ngayGD);
        Task<ApiResponse<Dictionary<string, decimal>>> GetAmountByCurrencyAsync(DateTime ngayGD);

        // Temporal operations
        Task<ApiResponse<List<GL01DetailsDto>>> GetHistoryAsync(long id);
        Task<ApiResponse<GL01DetailsDto>> GetAsOfDateAsync(long id, DateTime asOfDate);

        // System operations
        Task<ApiResponse<bool>> IsHealthyAsync();
    }
}
