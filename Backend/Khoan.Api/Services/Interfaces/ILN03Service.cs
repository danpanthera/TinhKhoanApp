using Khoan.Api.Dtos.LN03;
using Khoan.Api.Models.Common;

namespace Khoan.Api.Services.Interfaces
{
    public interface ILN03Service
    {
        // Basic CRUD Operations
        Task<ApiResponse<(IEnumerable<LN03PreviewDto> Data, int TotalCount)>> GetAllPagedAsync(int page, int pageSize, DateTime? fromDate = null, DateTime? toDate = null);
        Task<ApiResponse<LN03DetailsDto>> GetByIdAsync(int id);
        Task<ApiResponse<LN03DetailsDto>> CreateAsync(CreateLN03Dto dto);
        Task<ApiResponse<LN03DetailsDto>> UpdateAsync(int id, UpdateLN03Dto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<bool>> SoftDeleteAsync(int id);

        // CSV Import Operations
        Task<ApiResponse<LN03ImportResultDto>> ImportFromCsvStreamAsync(Stream csvStream, string fileName);
        Task<ApiResponse<LN03ImportResultDto>> ImportFromCsvFileAsync(string filePath);
        Task<ApiResponse<bool>> ValidateCsvFormatAsync(Stream csvStream);

        // Temporal Table Operations
        Task<ApiResponse<LN03DetailsDto>> GetAsOfDateAsync(int id, DateTime asOfDate);
        Task<ApiResponse<IEnumerable<LN03DetailsDto>>> GetHistoryAsync(int id);
        Task<ApiResponse<IEnumerable<LN03DetailsDto>>> GetChangedBetweenAsync(DateTime startDate, DateTime endDate);

        // Business Logic Queries
        Task<ApiResponse<IEnumerable<LN03DetailsDto>>> GetByBranchCodeAsync(string branchCode);
        Task<ApiResponse<IEnumerable<LN03DetailsDto>>> GetByCustomerCodeAsync(string customerCode);
        Task<ApiResponse<IEnumerable<LN03DetailsDto>>> GetByContractNumberAsync(string contractNumber);
        Task<ApiResponse<IEnumerable<LN03DetailsDto>>> GetByDebtGroupAsync(string debtGroup);
        Task<ApiResponse<IEnumerable<LN03DetailsDto>>> GetByDateAsync(DateTime date);
        Task<ApiResponse<IEnumerable<LN03DetailsDto>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Analytics and Reporting
        Task<ApiResponse<LN03SummaryDto>> GetSummaryAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<ApiResponse<Dictionary<string, decimal>>> GetTotalAmountByBranchAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<ApiResponse<Dictionary<string, decimal>>> GetOutstandingBalanceByDateAsync(DateTime date);
        Task<ApiResponse<Dictionary<string, int>>> GetContractCountByDebtGroupAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<ApiResponse<IEnumerable<(string CustomerCode, string CustomerName, decimal TotalAmount)>>> GetTopCustomersByAmountAsync(int topCount = 10, DateTime? fromDate = null, DateTime? toDate = null);
        Task<ApiResponse<Dictionary<string, decimal>>> GetTotalAmountByDateRangeAsync(DateTime startDate, DateTime endDate, string groupBy = "month");

        // Data Management
        Task<ApiResponse<bool>> TruncateDataAsync();
        Task<ApiResponse<int>> GetRecordCountAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<ApiResponse<IEnumerable<string>>> ValidateDataIntegrityAsync();
        Task<ApiResponse<DateTime?>> GetLatestDataDateAsync();
        Task<ApiResponse<DateTime?>> GetOldestDataDateAsync();

        // Configuration
        Task<ApiResponse<LN03ConfigDto>> GetConfigurationAsync();
        Task<ApiResponse<bool>> UpdateConfigurationAsync(LN03ConfigDto config);
    }
}
