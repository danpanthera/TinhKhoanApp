using TinhKhoanApp.Api.Models.DTOs;

namespace TinhKhoanApp.Api.Services.DataServices
{
    /// <summary>
    /// Interface cho GL01 Data Service - xử lý business logic liên quan đến dữ liệu GL01 (Sổ tổng kế toán)
    /// </summary>
    public interface IGL01DataService
    {
        /// <summary>
        /// Lấy preview dữ liệu GL01
        /// </summary>
        Task<IEnumerable<GL01PreviewDto>> GetGL01PreviewAsync(int count = 10);

        /// <summary>
        /// Lấy chi tiết bản ghi GL01
        /// </summary>
        Task<GL01DetailDto?> GetGL01DetailAsync(long id);

        /// <summary>
        /// Lấy dữ liệu GL01 theo ngày
        /// </summary>
        Task<IEnumerable<GL01PreviewDto>> GetGL01ByDateAsync(DateTime date, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu GL01 theo mã đơn vị
        /// </summary>
        Task<IEnumerable<GL01PreviewDto>> GetGL01ByUnitCodeAsync(string unitCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu GL01 theo mã tài khoản
        /// </summary>
        Task<IEnumerable<GL01PreviewDto>> GetGL01ByAccountCodeAsync(string accountCode, int maxResults = 100);

        /// <summary>
        /// Lấy thống kê tổng hợp GL01 theo đơn vị
        /// </summary>
        Task<GL01SummaryDto> GetGL01SummaryByUnitAsync(string unitCode, DateTime? date = null);

        /// <summary>
        /// Lấy thống kê tổng hợp GL01 theo ngày
        /// </summary>
        Task<GL01SummaryDto> GetGL01SummaryByDateAsync(DateTime date);

        /// <summary>
        /// Tìm kiếm GL01 theo nhiều tiêu chí
        /// </summary>
        Task<PagedApiResponse<GL01PreviewDto>> SearchGL01Async(
            string? keyword,
            string? unitCode,
            string? accountCode,
            string? transactionType,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1,
            int pageSize = 20);
    }
}
