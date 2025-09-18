using Khoan.Api.Models.Dtos.DP01;
using Khoan.Api.Models.Common;
using Khoan.Api.Models.Dtos;

namespace Khoan.Api.Services.DataServices
{
    /// <summary>
    /// Interface cho DP01 Data Service - xử lý business logic liên quan đến dữ liệu DP01 (Tài khoản tiền gửi)
    /// </summary>
    public interface IDP01DataService
    {
        /// <summary>
        /// Lấy preview dữ liệu DP01
        /// </summary>
        Task<IEnumerable<DP01PreviewDto>> GetDP01PreviewAsync(int count = 10);

        /// <summary>
        /// Lấy chi tiết bản ghi DP01
        /// </summary>
        Task<DP01DetailsDto?> GetDP01DetailAsync(long id);

        /// <summary>
        /// Lấy dữ liệu DP01 theo chi nhánh
        /// </summary>
        Task<IEnumerable<DP01PreviewDto>> GetDP01ByBranchAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu DP01 theo khách hàng
        /// </summary>
        Task<IEnumerable<DP01PreviewDto>> GetDP01ByCustomerAsync(string customerCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu DP01 theo số tài khoản
        /// </summary>
        Task<IEnumerable<DP01PreviewDto>> GetDP01ByAccountNumberAsync(string accountNumber, int maxResults = 100);

        /// <summary>
        /// Lấy thống kê tổng hợp DP01 theo chi nhánh
        /// </summary>
        Task<DP01SummaryDto> GetDP01SummaryByBranchAsync(string branchCode, DateTime? date = null);

        /// <summary>
        /// Tìm kiếm DP01 theo nhiều tiêu chí
        /// </summary>
        Task<ApiResponse<PagedResult<DP01PreviewDto>>> SearchDP01Async(
            string? keyword,
            string? branchCode,
            string? customerCode,
            string? accountNumber,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1,
            int pageSize = 20);
    }
}
