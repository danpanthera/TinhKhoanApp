using Khoan.Api.Models.Common;
using Khoan.Api.Models.DTOs.LN01;

namespace Khoan.Api.Services.Interfaces
{
    /// <summary>
    /// Interface cho LN01 Service - cung cấp business logic cho LN01 (Loan Data)
    /// </summary>
    public interface ILN01Service
    {
        /// <summary>
        /// Lấy preview LN01 (danh sách với thông tin tóm tắt)
        /// </summary>
        Task<ApiResponse<IEnumerable<LN01PreviewDto>>> PreviewAsync(int take = 20);

        /// <summary>
        /// Lấy chi tiết LN01 theo ID
        /// </summary>
        Task<ApiResponse<LN01DetailsDto>> GetByIdAsync(long id);

        /// <summary>
        /// Tạo mới LN01
        /// </summary>
        Task<ApiResponse<LN01DetailsDto>> CreateAsync(LN01CreateDto createDto);

        /// <summary>
        /// Cập nhật LN01
        /// </summary>
        Task<ApiResponse<LN01DetailsDto>> UpdateAsync(LN01UpdateDto updateDto);

        /// <summary>
        /// Xóa LN01 theo ID
        /// </summary>
        Task<ApiResponse<bool>> DeleteAsync(long id);

        /// <summary>
        /// Lấy danh sách LN01 theo ngày
        /// </summary>
        Task<ApiResponse<IEnumerable<LN01PreviewDto>>> GetByDateAsync(DateTime date, int maxResults = 100);

        /// <summary>
        /// Lấy danh sách LN01 theo mã chi nhánh
        /// </summary>
        Task<ApiResponse<IEnumerable<LN01PreviewDto>>> GetByBranchCodeAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Lấy danh sách LN01 theo mã khách hàng
        /// </summary>
        Task<ApiResponse<IEnumerable<LN01PreviewDto>>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100);

        /// <summary>
        /// Lấy danh sách LN01 theo số tài khoản
        /// </summary>
        Task<ApiResponse<IEnumerable<LN01PreviewDto>>> GetByAccountNumberAsync(string accountNumber, int maxResults = 100);

        /// <summary>
        /// Lấy danh sách LN01 theo nhóm nợ
        /// </summary>
        Task<ApiResponse<IEnumerable<LN01PreviewDto>>> GetByDebtGroupAsync(string debtGroup, int maxResults = 100);

        /// <summary>
        /// Lấy danh sách LN01 trong khoảng thời gian
        /// </summary>
        Task<ApiResponse<IEnumerable<LN01PreviewDto>>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, int maxResults = 100);

        /// <summary>
        /// Lấy thống kê tổng quan LN01
        /// </summary>
        Task<ApiResponse<LN01SummaryDto>> GetSummaryAsync(DateTime? fromDate = null, DateTime? toDate = null);

        /// <summary>
        /// Lấy tổng số tiền vay theo chi nhánh
        /// </summary>
        Task<ApiResponse<decimal>> GetTotalLoanAmountByBranchAsync(string branchCode, DateTime? date = null);

        /// <summary>
        /// Lấy tổng số tiền giải ngân theo ngày
        /// </summary>
        Task<ApiResponse<decimal>> GetTotalDisbursementByDateAsync(DateTime date);

        /// <summary>
        /// Lấy số lượng khoản vay theo nhóm nợ
        /// </summary>
        Task<ApiResponse<Dictionary<string, int>>> GetLoanCountByDebtGroupAsync(DateTime? date = null);

        /// <summary>
        /// Lấy top customers có dư nợ cao nhất
        /// </summary>
        Task<ApiResponse<IEnumerable<LN01PreviewDto>>> GetTopCustomersByLoanAmountAsync(int topCount = 10, DateTime? date = null);
    }
}
