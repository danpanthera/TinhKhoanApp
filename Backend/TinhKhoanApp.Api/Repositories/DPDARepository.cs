using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Repositories
{
    /// <summary>
    /// Repository implementation cho DPDA table - Thẻ nộp đơn gửi tiết kiệm
    /// Implements business-specific data access methods cho DPDA data
    /// CSV Structure: 13 business columns mapping với existing interface
    /// </summary>
    public class DPDARepository : Repository<DPDA>, IDPDARepository
    {
        public DPDARepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy dữ liệu DPDA gần đây nhất, sắp xếp theo CREATED_DATE
        /// </summary>
        public new async Task<IEnumerable<DPDA>> GetRecentAsync(int count = 10)
        {
            return await _dbSet
                .OrderByDescending(dpda => dpda.CREATED_DATE)
                .Take(count)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy danh sách DPDA theo ngày dữ liệu với ordering
        /// </summary>
        /// <param name="date">Ngày dữ liệu (YYYY-MM-DD)</param>
        /// <returns>Danh sách DPDA records ordered by MA_CHI_NHANH, MA_KHACH_HANG</returns>
        public async Task<IEnumerable<DPDA>> GetByDateAsync(DateTime date)
        {
            return await _dbSet
                .Where(d => d.NGAY_DL.Date == date.Date)
                .OrderBy(d => d.MA_CHI_NHANH)
                .ThenBy(d => d.MA_KHACH_HANG)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy danh sách DPDA theo mã chi nhánh với filtering và ordering
        /// </summary>
        /// <param name="branchCode">Mã chi nhánh</param>
        /// <param name="maxResults">Số lượng tối đa</param>
        /// <returns>Danh sách DPDA theo chi nhánh ordered by latest NGAY_DL</returns>
        public async Task<IEnumerable<DPDA>> GetByBranchCodeAsync(string branchCode, int maxResults = 100)
        {
            return await _dbSet
                .Where(d => d.MA_CHI_NHANH == branchCode)
                .OrderByDescending(d => d.NGAY_DL)
                .ThenBy(d => d.MA_KHACH_HANG)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy danh sách DPDA theo mã khách hàng với temporal data
        /// </summary>
        /// <param name="customerCode">Mã khách hàng</param>
        /// <param name="maxResults">Số lượng tối đa</param>
        /// <returns>Danh sách DPDA theo khách hàng ordered by latest NGAY_DL</returns>
        public async Task<IEnumerable<DPDA>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100)
        {
            return await _dbSet
                .Where(d => d.MA_KHACH_HANG == customerCode)
                .OrderByDescending(d => d.NGAY_DL)
                .ThenBy(d => d.SO_TAI_KHOAN)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy danh sách DPDA theo số tài khoản với card tracking
        /// </summary>
        /// <param name="accountNumber">Số tài khoản</param>
        /// <param name="maxResults">Số lượng tối đa</param>
        /// <returns>Danh sách DPDA theo tài khoản ordered by card issuance date</returns>
        public async Task<IEnumerable<DPDA>> GetByAccountNumberAsync(string accountNumber, int maxResults = 100)
        {
            return await _dbSet
                .Where(d => d.SO_TAI_KHOAN == accountNumber)
                .OrderByDescending(d => d.NGAY_PHAT_HANH)
                .ThenBy(d => d.SO_THE)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy danh sách DPDA theo số thẻ
        /// </summary>
        /// <param name="cardNumber">Số thẻ</param>
        /// <param name="maxResults">Số lượng tối đa</param>
        /// <returns>Danh sách DPDA theo số thẻ</returns>
        public async Task<IEnumerable<DPDA>> GetByCardNumberAsync(string cardNumber, int maxResults = 100)
        {
            return await _dbSet
                .Where(d => d.SO_THE == cardNumber)
                .OrderByDescending(d => d.CREATED_DATE)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy danh sách DPDA theo trạng thái thẻ với business filtering
        /// </summary>
        /// <param name="status">Trạng thái thẻ</param>
        /// <param name="maxResults">Số lượng tối đa</param>
        /// <returns>Danh sách DPDA theo trạng thái ordered by NGAY_DL desc</returns>
        public async Task<IEnumerable<DPDA>> GetByStatusAsync(string status, int maxResults = 100)
        {
            return await _dbSet
                .Where(d => d.TRANG_THAI == status)
                .OrderByDescending(d => d.NGAY_DL)
                .ThenBy(d => d.MA_CHI_NHANH)
                .ThenBy(d => d.MA_KHACH_HANG)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy số lượng thẻ theo chi nhánh và trạng thái
        /// </summary>
        /// <param name="branchCode">Mã chi nhánh</param>
        /// <param name="status">Trạng thái</param>
        /// <returns>Số lượng thẻ theo điều kiện</returns>
        public async Task<int> GetCardCountByBranchAndStatusAsync(string branchCode, string status)
        {
            return await _dbSet
                .CountAsync(d => d.MA_CHI_NHANH == branchCode && d.TRANG_THAI == status);
        }

        /// <summary>
        /// Cập nhật nhiều DPDA records
        /// </summary>
        /// <param name="entities">Danh sách DPDA entities</param>
        public void UpdateRange(IEnumerable<DPDA> entities)
        {
            _dbSet.UpdateRange(entities);
        }
    }
}
