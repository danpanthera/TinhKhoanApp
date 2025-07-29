using TinhKhoanApp.Api.Models.DataTables;
using System.Threading.Tasks;

namespace TinhKhoanApp.Api.Repositories
{
    /// <summary>
    /// Interface Repository cho EI01 - mở rộng từ IRepository generic
    /// </summary>
    public interface IEI01Repository : IRepository<EI01>
    {
        /// <summary>
        /// Lấy danh sách dữ liệu EI01 với số lượng chỉ định
        /// </summary>
        Task<IEnumerable<EI01>> GetAsync(int take = 10, int skip = 0);

        /// <summary>
        /// Lấy dữ liệu EI01 mới nhất
        /// </summary>
        Task<IEnumerable<EI01>> GetRecentAsync(int count = 10);

        /// <summary>
        /// Lấy dữ liệu EI01 theo ngày
        /// </summary>
        Task<IEnumerable<EI01>> GetByDateAsync(DateTime date);

        /// <summary>
        /// Lấy dữ liệu EI01 theo mã chi nhánh
        /// </summary>
        Task<IEnumerable<EI01>> GetByBranchCodeAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 theo mã khách hàng
        /// </summary>
        Task<IEnumerable<EI01>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 theo loại khách hàng
        /// </summary>
        Task<IEnumerable<EI01>> GetByCustomerTypeAsync(string customerType, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 theo số điện thoại (bất kỳ loại dịch vụ nào)
        /// </summary>
        Task<IEnumerable<EI01>> GetByPhoneNumberAsync(string phoneNumber, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 theo trạng thái của dịch vụ EMB
        /// </summary>
        Task<IEnumerable<EI01>> GetByEMBStatusAsync(string status, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 theo trạng thái của dịch vụ OTT
        /// </summary>
        Task<IEnumerable<EI01>> GetByOTTStatusAsync(string status, int maxResults = 100);

        /// <summary>
        /// Lấy dữ liệu EI01 theo trạng thái của dịch vụ SMS
        /// </summary>
        Task<IEnumerable<EI01>> GetBySMSStatusAsync(string status, int maxResults = 100);

        /// <summary>
        /// Lấy số lượng khách hàng theo chi nhánh và loại dịch vụ+trạng thái
        /// </summary>
        Task<int> GetCustomerCountByBranchAndServiceAsync(string branchCode, string serviceType, string status);

        /// <summary>
        /// Lấy dữ liệu EI01 được đăng ký trong khoảng thời gian
        /// </summary>
        Task<IEnumerable<EI01>> GetByRegistrationDateRangeAsync(DateTime fromDate, DateTime toDate, string serviceType, int maxResults = 100);

        /// <summary>
        /// Cập nhật nhiều EI01
        /// </summary>
        void UpdateRange(IEnumerable<EI01> entities);

        /// <summary>
        /// Lấy dữ liệu EI01 theo trạng thái dịch vụ
        /// </summary>
        Task<IEnumerable<EI01>> GetByServiceStatusAsync(string serviceStatus, int maxResults = 100);

        /// <summary>
        /// Phân trang dữ liệu EI01 theo điều kiện lọc
        /// </summary>
        Task<(int totalCount, IEnumerable<EI01> items)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            System.Linq.Expressions.Expression<Func<EI01, bool>> predicate);

        /// <summary>
        /// Lưu các thay đổi vào cơ sở dữ liệu
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}
