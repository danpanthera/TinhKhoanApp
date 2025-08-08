using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Models.Entities;
using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Repositories.Interfaces
{
    /// <summary>
    /// LN03 Repository Interface - Phase 2B
    /// Extends base repository with LN03Entity-specific operations
    /// </summary>
    public interface ILN03Repository : IBaseRepository<LN03Entity>
    {
        // LN03-specific query operations
        Task<ApiResponse<IEnumerable<LN03Entity>>> GetByProductCodeAsync(string productCode, DateTime? ngayDL = null);
        Task<ApiResponse<IEnumerable<LN03Entity>>> GetByDonViAsync(string donVi, DateTime? ngayDL = null);
        Task<ApiResponse<IEnumerable<LN03Entity>>> GetByLoanTypeAsync(string loanType, DateTime ngayDL);

        // LN03-specific aggregation operations
        Task<ApiResponse<decimal>> CalculateTotalLoanAmountAsync(DateTime ngayDL);
        Task<ApiResponse<Dictionary<string, decimal>>> GetLoanAmountByProductAsync(DateTime ngayDL);
        Task<ApiResponse<Dictionary<string, decimal>>> GetLoanAmountByDonViAsync(DateTime ngayDL);
        Task<ApiResponse<Dictionary<string, int>>> GetLoanCountByProductAsync(DateTime ngayDL);

        // LN03-specific summary operations
        Task<ApiResponse<IEnumerable<LN03Entity>>> GetProductSummaryAsync(DateTime ngayDL);
        Task<ApiResponse<decimal>> GetAverageLoanAmountAsync(DateTime ngayDL);
        Task<ApiResponse<Dictionary<string, decimal>>> GetPortfolioDistributionAsync(DateTime ngayDL);

        // LN03-specific search operations
        Task<ApiResponse<IEnumerable<LN03Entity>>> GetLargeLoansAsync(DateTime ngayDL, decimal threshold);
        Task<ApiResponse<IEnumerable<LN03Entity>>> GetSmallLoansAsync(DateTime ngayDL, decimal threshold);
    }
}
