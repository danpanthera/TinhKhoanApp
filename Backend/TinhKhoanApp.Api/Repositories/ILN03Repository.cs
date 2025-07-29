using TinhKhoanApp.Api.Models.DataTables;
using System.Threading.Tasks;

namespace TinhKhoanApp.Api.Repositories
{
    /// <summary>
    /// Repository cho dữ liệu LN03 (Dữ liệu phục hồi khoản vay)
    /// </summary>
    public interface ILN03Repository : IRepository<LN03>
    {
        /// <summary>
        /// Trả về DbContext cho việc sử dụng trong service layer
        /// </summary>
        Data.ApplicationDbContext GetDbContext();

        /// <summary>
        /// Lấy danh sách LN03 gần đây
        /// </summary>
        Task<IEnumerable<LN03>> GetRecentAsync(int count = 10);

        /// <summary>
        /// Lấy LN03 theo ID
        /// </summary>
        Task<LN03?> GetByIdAsync(long id);

        /// <summary>
        /// Lấy danh sách LN03 theo ngày
        /// </summary>
        Task<IEnumerable<LN03>> GetByDateAsync(DateTime date, int maxResults = 100);

        /// <summary>
        /// Lấy danh sách LN03 theo mã chi nhánh
        /// </summary>
        Task<IEnumerable<LN03>> GetByBranchCodeAsync(string branchCode, int maxResults = 100);

        /// <summary>
        /// Lấy danh sách LN03 theo mã khách hàng
        /// </summary>
        Task<IEnumerable<LN03>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100);

        /// <summary>
        /// Lấy danh sách LN03 theo mã cán bộ tín dụng
        /// </summary>
        Task<IEnumerable<LN03>> GetByOfficerCodeAsync(string officerCode, int maxResults = 100);

        /// <summary>
        /// Lấy danh sách LN03 theo nhóm nợ
        /// </summary>
        Task<IEnumerable<LN03>> GetByDebtGroupAsync(string debtGroup, int maxResults = 100);

        /// <summary>
        /// Lấy danh sách LN03 theo khoảng thời gian
        /// </summary>
        Task<IEnumerable<LN03>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, int maxResults = 100);

        /// <summary>
        /// Lấy tổng số tiền xử lý rủi ro theo chi nhánh
        /// </summary>
        Task<decimal> GetTotalRiskAmountByBranchAsync(string branchCode, DateTime? date = null);

        /// <summary>
        /// Lấy tổng thu nợ sau xử lý theo chi nhánh
        /// </summary>
        Task<decimal> GetTotalDebtRecoveryByBranchAsync(string branchCode, DateTime? date = null);

        /// <summary>
        /// Lưu các thay đổi vào cơ sở dữ liệu
        /// </summary>
        /// <returns>Số lượng bản ghi bị ảnh hưởng</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Xóa một entity theo id
        /// </summary>
        /// <param name="entity">Entity cần xóa</param>
        Task DeleteAsync(LN03 entity);
    }
}
