using TinhKhoanApp.Api.Models.DTOs;

namespace TinhKhoanApp.Api.Services.DataServices
{
    /// <summary>
    /// Interface cho DPDA Data Service - xử lý business logic liên quan đến dữ liệu DPDA (Dữ liệu thẻ tiền gửi)
    /// </summary>
    public interface IDPDADataService
    {
        /// <summary>
        /// Lấy preview dữ liệu DPDA
        /// </summary>
        Task<IEnumerable<DPDAPreviewDto>> GetDPDAPreviewAsync(int count = 10);

        /// <summary>
        /// Lấy chi tiết bản ghi DPDA
        /// </summary>
        Task<DPDADetailDto?> GetDPDADetailAsync(int id);

        /// <summary>
        /// Lấy dữ liệu DPDA theo ngày
        /// </summary>
        Task<IEnumerable<DPDAPreviewDto>> GetDPDAByDateAsync(DateTime date, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu DPDA theo chi nhánh
        /// </summary>
        Task<IEnumerable<DPDAPreviewDto>> GetDPDAByBranchAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu DPDA theo khách hàng
        /// </summary>
        Task<IEnumerable<DPDAPreviewDto>> GetDPDAByCustomerAsync(string customerCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu DPDA theo số tài khoản
        /// </summary>
        Task<IEnumerable<DPDAPreviewDto>> GetDPDAByAccountNumberAsync(string accountNumber, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu DPDA theo số thẻ
        /// </summary>
        Task<IEnumerable<DPDAPreviewDto>> GetDPDAByCardNumberAsync(string cardNumber, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu DPDA theo trạng thái
        /// </summary>
        Task<IEnumerable<DPDAPreviewDto>> GetDPDAByStatusAsync(string status, int maxResults = 100);

        /// <summary>
        /// Lấy thống kê tổng hợp DPDA theo chi nhánh
        /// </summary>
        Task<DPDASummaryDto> GetDPDASummaryByBranchAsync(string branchCode, DateTime? date = null);

        /// <summary>
        /// Lấy thống kê tổng hợp DPDA theo ngày
        /// </summary>
        Task<DPDASummaryDto> GetDPDASummaryByDateAsync(DateTime date);

        /// <summary>
        /// Tìm kiếm DPDA theo nhiều tiêu chí
        /// </summary>
        Task<ApiResponse<PagedResult<DPDAPreviewDto>>> SearchDPDAAsync(
            string? keyword,
            string? branchCode,
            string? customerCode,
            string? accountNumber,
            string? cardNumber,
            string? status,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1,
            int pageSize = 20);
    }
}
