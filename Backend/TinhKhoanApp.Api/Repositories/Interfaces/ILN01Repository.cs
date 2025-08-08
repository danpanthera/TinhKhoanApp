using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Models.Entities;
using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Repositories.Interfaces
{
    /// <summary>
    /// LN01 Repository Interface - Loan Detail table
    /// Extends base repository with LN01Entity-specific operations
    /// </summary>
    public interface ILN01Repository : IBaseRepository<LN01Entity>
    {
        // LN01-specific query operations
        Task<List<LN01>> GetByLoanNumberAsync(string loanNumber);
        Task<List<LN01>> GetByCustomerIdAsync(string customerId, DateTime? ngayDL = null);
        Task<List<LN01>> GetByLoanStatusAsync(string loanStatus, DateTime ngayDL);
        Task<List<LN01>> GetByProductCodeAsync(string productCode, DateTime ngayDL);

        // LN01-specific aggregation operations
        Task<decimal> CalculateTotalOutstandingAsync(DateTime ngayDL);
        Task<decimal> CalculateOverdueAmountAsync(DateTime ngayDL);
        Task<decimal> GetTotalLoanAmountAsync(DateTime ngayDL);
        Task<Dictionary<string, decimal>> GetLoanAmountByProductAsync(DateTime ngayDL);
        Task<Dictionary<string, decimal>> GetOutstandingByStatusAsync(DateTime ngayDL);

        // LN01-specific search operations
        Task<List<LN01>> SearchByDonViAsync(string donVi, DateTime? ngayDL = null);
        Task<List<LN01>> GetOverdueLoansAsync(DateTime ngayDL, int overdueDays);
        Task<List<LN01>> GetLoansByAmountRangeAsync(DateTime ngayDL, decimal minAmount, decimal maxAmount);

        // LN01 portfolio analysis
        Task<Dictionary<string, int>> GetLoanCountByProductAsync(DateTime ngayDL);
        Task<List<LN01>> GetNewLoansAsync(DateTime ngayDL);
        Task<List<LN01>> GetMaturedLoansAsync(DateTime ngayDL);
    }
}
