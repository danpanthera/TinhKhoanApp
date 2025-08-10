using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.DTOs.DP01;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// Interface cho DP01 Service - xử lý business logic cho bảng DP01
    /// DP01: Dữ liệu tiền gửi có kỳ hạn với 63 business columns
    /// </summary>
    public interface IDP01Service
    {
        /// <summary>
        /// Lấy tất cả records DP01 với paging
        /// </summary>
        Task<IEnumerable<DP01PreviewDto>> GetAllAsync(int pageNumber = 1, int pageSize = 100);

        /// <summary>
        /// Lấy DP01 theo Id
        /// </summary>
        Task<DP01DetailsDto?> GetByIdAsync(int id);

        /// <summary>
        /// Lấy records gần nhất của DP01
        /// </summary>
        Task<IEnumerable<DP01PreviewDto>> GetRecentAsync(int count = 10);

        /// <summary>
        /// Lấy DP01 theo ngày dữ liệu
        /// </summary>
        Task<IEnumerable<DP01PreviewDto>> GetByDateAsync(DateTime date);

        /// <summary>
        /// Lấy DP01 theo mã chi nhánh (MA_CN)
        /// </summary>
        Task<IEnumerable<DP01PreviewDto>> GetByBranchCodeAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Lấy DP01 theo mã khách hàng (MA_KH)
        /// </summary>
        Task<IEnumerable<DP01PreviewDto>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100);

        /// <summary>
        /// Lấy DP01 theo số tài khoản (SO_TAI_KHOAN)
        /// </summary>
        Task<IEnumerable<DP01PreviewDto>> GetByAccountNumberAsync(string accountNumber, int maxResults = 100);

        /// <summary>
        /// Lấy tổng số dư theo chi nhánh
        /// </summary>
        Task<decimal> GetTotalBalanceByBranchAsync(string branchCode, DateTime? date = null);

        /// <summary>
        /// Lấy thống kê tổng quan DP01
        /// </summary>
        Task<DP01SummaryDto> GetStatisticsAsync(DateTime? date = null);

        /// <summary>
        /// Tạo mới DP01 record
        /// </summary>
        Task<DP01DetailsDto> CreateAsync(DP01CreateDto createDto);

        /// <summary>
        /// Cập nhật DP01 record
        /// </summary>
        Task<DP01DetailsDto?> UpdateAsync(int id, DP01UpdateDto updateDto);

        /// <summary>
        /// Xóa DP01 record
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Lấy total count cho paging
        /// </summary>
        Task<int> GetTotalCountAsync();
    }
}
