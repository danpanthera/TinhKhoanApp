using TinhKhoanApp.Api.Models.DTOs;

namespace TinhKhoanApp.Api.Services.DataServices
{
    /// <summary>
    /// Interface cho EI01 Data Service - xử lý business logic liên quan đến dữ liệu EI01 (Thông tin dịch vụ ngân hàng điện tử)
    /// </summary>
    public interface IEI01DataService
    {
        /// <summary>
        /// Lấy preview dữ liệu EI01
        /// </summary>
        Task<IEnumerable<EI01PreviewDto>> GetEI01PreviewAsync(int count = 10);

        /// <summary>
        /// Lấy chi tiết bản ghi EI01
        /// </summary>
        Task<EI01DetailDto?> GetEI01DetailAsync(long id);

        /// <summary>
        /// Lấy dữ liệu EI01 theo ngày
        /// </summary>
        Task<IEnumerable<EI01PreviewDto>> GetEI01ByDateAsync(DateTime date, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 theo chi nhánh
        /// </summary>
        Task<IEnumerable<EI01PreviewDto>> GetEI01ByBranchAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 theo khách hàng
        /// </summary>
        Task<IEnumerable<EI01PreviewDto>> GetEI01ByCustomerAsync(string customerCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 theo loại khách hàng
        /// </summary>
        Task<IEnumerable<EI01PreviewDto>> GetEI01ByCustomerTypeAsync(string customerType, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 theo số điện thoại
        /// </summary>
        Task<IEnumerable<EI01PreviewDto>> GetEI01ByPhoneNumberAsync(string phoneNumber, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 theo trạng thái dịch vụ
        /// </summary>
        Task<IEnumerable<EI01PreviewDto>> GetEI01ByServiceStatusAsync(string serviceType, string status, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 theo khoảng thời gian đăng ký
        /// </summary>
        Task<IEnumerable<EI01PreviewDto>> GetEI01ByRegistrationDateRangeAsync(
            DateTime fromDate,
            DateTime toDate,
            string serviceType,
            int maxResults = 100);

        /// <summary>
        /// Lấy thống kê tổng hợp EI01 theo chi nhánh
        /// </summary>
        Task<EI01SummaryDto> GetEI01SummaryByBranchAsync(string branchCode, DateTime? date = null);

        /// <summary>
        /// Lấy thống kê tổng hợp EI01 theo ngày
        /// </summary>
        Task<EI01SummaryDto> GetEI01SummaryByDateAsync(DateTime date);

        /// <summary>
        /// Tìm kiếm EI01 theo nhiều tiêu chí
        /// </summary>
        Task<ApiResponse<PagedResult<EI01PreviewDto>>> SearchEI01Async(
            string? keyword,
            string? branchCode,
            string? customerCode,
            string? customerType,
            string? phoneNumber,
            string? serviceType,
            string? serviceStatus,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1,
            int pageSize = 20);
    }
}
