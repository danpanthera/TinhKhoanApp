using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.DTOs.LN01;

namespace TinhKhoanApp.Api.Services.Interfaces
{
    /// <summary>
    /// LN01 Service Interface - Loan Detail table
    /// 79 business columns with temporal tracking
    /// </summary>
    public interface ILN01Service
    {
        // Preview operations
        Task<ApiResponse<PagedResult<LN01PreviewDto>>> GetPreviewAsync(int page = 1, int pageSize = 10, DateTime? ngayDL = null);

        // CRUD operations
        Task<ApiResponse<LN01DetailsDto>> GetByIdAsync(long id);
        Task<ApiResponse<LN01DetailsDto>> CreateAsync(LN01CreateDto createDto);
        Task<ApiResponse<LN01DetailsDto>> UpdateAsync(long id, LN01UpdateDto updateDto);
        Task<ApiResponse<bool>> DeleteAsync(long id);
        Task<ApiResponse<List<LN01DetailsDto>>> GetByDateAsync(DateTime ngayDL);
        Task<ApiResponse<List<LN01DetailsDto>>> GetByLoanNumberAsync(string loanNumber);

        // Import operations
        Task<ApiResponse<LN01ImportResultDto>> ImportCsvAsync(IFormFile csvFile);
        Task<ApiResponse<CsvValidationResult>> ValidateCsvAsync(IFormFile csvFile);
        Task<ApiResponse<LN01ImportResultDto>> ImportBatchAsync(List<LN01CreateDto> createDtos);

        // Business logic operations
        Task<ApiResponse<LN01SummaryDto>> GetSummaryAsync(DateTime ngayDL);
        Task<ApiResponse<List<LN01PreviewDto>>> GetLoanPortfolioAsync(DateTime ngayDL);
        Task<ApiResponse<decimal>> CalculateTotalOutstandingAsync(DateTime ngayDL);
        Task<ApiResponse<decimal>> CalculateOverdueAmountAsync(DateTime ngayDL);

        // Temporal operations
        Task<ApiResponse<List<LN01DetailsDto>>> GetHistoryAsync(long id);
        Task<ApiResponse<LN01DetailsDto>> GetAsOfDateAsync(long id, DateTime asOfDate);

        // System operations
        Task<ApiResponse<bool>> IsHealthyAsync();
    }
}
