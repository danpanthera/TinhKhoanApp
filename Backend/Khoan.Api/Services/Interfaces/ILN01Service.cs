using Khoan.Api.Dtos.LN01;
using Khoan.Api.Models.Common;

namespace Khoan.Api.Services.Interfaces
{
    public interface ILN01Service
    {
        // Basic CRUD Operations
        Task<ApiResponse<(IEnumerable<LN01PreviewDto> Data, int TotalCount)>> GetAllPagedAsync(int page, int pageSize, DateTime? fromDate = null, DateTime? toDate = null);
        Task<ApiResponse<LN01DetailsDto>> GetByIdAsync(int id);
        Task<ApiResponse<LN01DetailsDto>> CreateAsync(LN01CreateDto dto);
        Task<ApiResponse<LN01DetailsDto>> UpdateAsync(int id, LN01UpdateDto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<bool>> SoftDeleteAsync(int id);

        // Temporal Table Operations
        Task<ApiResponse<IEnumerable<LN01DetailsDto>>> GetHistoryAsync(int id);
        Task<ApiResponse<LN01DetailsDto>> GetAsOfDateAsync(int id, DateTime asOfDate);
        Task<ApiResponse<IEnumerable<LN01DetailsDto>>> GetChangedBetweenAsync(int id, DateTime startDate, DateTime endDate);

        // CSV Import Operations
        Task<ApiResponse<LN01ImportResultDto>> ImportFromCsvAsync(IFormFile file, bool replaceExistingData = false);
        Task<ApiResponse<LN01ImportResultDto>> ImportFromCsvStreamAsync(Stream csvStream, string fileName, bool replaceExistingData = false);

        // Analytics & Reports
        Task<ApiResponse<LN01SummaryDto>> GetSummaryAsync();
        Task<ApiResponse<Dictionary<string, decimal>>> GetLoanAmountsByProvinceAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<ApiResponse<Dictionary<string, int>>> GetLoanCountsByTypeAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<ApiResponse<Dictionary<string, decimal>>> GetInterestRateDistributionAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<ApiResponse<IEnumerable<dynamic>>> GetTopCustomersByLoanAmountAsync(int topCount = 10);
        Task<ApiResponse<IEnumerable<dynamic>>> GetLoanStatusDistributionAsync();

        // Officer & Branch Analytics
        Task<ApiResponse<Dictionary<string, int>>> GetLoansByOfficerAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<ApiResponse<Dictionary<string, decimal>>> GetLoanAmountsByBranchAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<ApiResponse<IEnumerable<dynamic>>> GetOfficerPerformanceAsync();

        // Date-based Operations
        Task<ApiResponse<Dictionary<string, int>>> GetRecordCountsByDateAsync();
        Task<ApiResponse<IEnumerable<DateTime>>> GetAvailableDatesAsync();
        Task<ApiResponse<DateTime?>> GetLatestDataDateAsync();
        Task<ApiResponse<DateTime?>> GetOldestDataDateAsync();

        // Search Operations
        Task<ApiResponse<(IEnumerable<LN01PreviewDto> Data, int TotalCount)>> SearchAsync(string searchTerm, int page, int pageSize);
        Task<ApiResponse<IEnumerable<LN01PreviewDto>>> GetByCustomerAsync(string custSeq);
        Task<ApiResponse<IEnumerable<LN01PreviewDto>>> GetByAccountAsync(string taiKhoan);
        Task<ApiResponse<IEnumerable<LN01PreviewDto>>> GetByOfficerAsync(string officerId);

        // Configuration
        Task<ApiResponse<LN01ConfigDto>> GetConfigAsync();
        Task<ApiResponse<LN01ConfigDto>> UpdateConfigAsync(LN01ConfigDto config);
    }
}
