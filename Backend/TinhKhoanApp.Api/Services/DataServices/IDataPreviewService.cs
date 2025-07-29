using TinhKhoanApp.Api.Models.DTOs;

namespace TinhKhoanApp.Api.Services.DataServices
{
    /// <summary>
    /// Interface cho Data Preview Service - xử lý business logic liên quan đến preview dữ liệu
    /// </summary>
    public interface IDataPreviewService
    {
        /// <summary>
        /// Lấy preview dữ liệu DP01
        /// </summary>
        Task<IEnumerable<DP01PreviewDto>> GetDP01PreviewAsync(int count = 10);

        /// <summary>
        /// Lấy chi tiết bản ghi DP01
        /// </summary>
        Task<DP01DetailDto?> GetDP01DetailAsync(int id);

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
        /// Tìm kiếm DP01 theo nhiều tiêu chí
        /// </summary>
        Task<PagedApiResponse<DP01PreviewDto>> SearchDP01Async(
            string? keyword,
            string? branchCode,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1,
            int pageSize = 20);
    }
}
