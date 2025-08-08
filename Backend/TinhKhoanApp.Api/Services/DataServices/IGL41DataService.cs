using TinhKhoanApp.Api.Models.DTOs;

namespace TinhKhoanApp.Api.Services.DataServices
{
    /// <summary>
    /// Interface cho GL41 Data Service - xử lý business logic liên quan đến dữ liệu GL41
    /// </summary>
    public interface IGL41DataService
    {
        /// <summary>
        /// Lấy preview dữ liệu GL41
        /// </summary>
        Task<IEnumerable<GL41PreviewDto>> GetGL41PreviewAsync(int count = 10);

        /// <summary>
        /// Lấy chi tiết bản ghi GL41
        /// </summary>
        Task<GL41DetailDto?> GetGL41DetailAsync(long id);

        /// <summary>
        /// Lấy dữ liệu GL41 theo chi nhánh
        /// </summary>
        Task<IEnumerable<GL41PreviewDto>> GetGL41ByUnitAsync(string unitCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu GL41 theo tài khoản
        /// </summary>
        Task<IEnumerable<GL41PreviewDto>> GetGL41ByAccountAsync(string accountCode, int maxResults = 100);

        /// <summary>
        /// Lấy thống kê tổng hợp GL41 theo chi nhánh
        /// </summary>
        Task<GL41SummaryDto> GetGL41SummaryByUnitAsync(string unitCode, DateTime? date = null);

        /// <summary>
        /// Tìm kiếm GL41 theo nhiều tiêu chí
        /// </summary>
        Task<ApiResponse<PagedResult<GL41PreviewDto>>> SearchGL41Async(
            string? keyword,
            string? unitCode,
            string? accountCode,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1,
            int pageSize = 20);
    }
}
