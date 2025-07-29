using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Repositories
{
    /// <summary>
    /// DPDA Repository - triển khai IDPDARepository
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
        /// Lấy dữ liệu DPDA theo ngày
        /// </summary>
        public async Task<IEnumerable<DPDA>> GetByDateAsync(DateTime date)
        {
            return await _dbSet
                .Where(dpda => dpda.NGAY_DL.Date == date.Date)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy dữ liệu DPDA theo mã chi nhánh
        /// </summary>
        public async Task<IEnumerable<DPDA>> GetByBranchCodeAsync(string branchCode, int maxResults = 100)
        {
            return await _dbSet
                .Where(dpda => dpda.MA_CHI_NHANH == branchCode)
                .OrderByDescending(dpda => dpda.CREATED_DATE)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy dữ liệu DPDA theo mã khách hàng
        /// </summary>
        public async Task<IEnumerable<DPDA>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100)
        {
            return await _dbSet
                .Where(dpda => dpda.MA_KHACH_HANG == customerCode)
                .OrderByDescending(dpda => dpda.CREATED_DATE)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy dữ liệu DPDA theo số tài khoản
        /// </summary>
        public async Task<IEnumerable<DPDA>> GetByAccountNumberAsync(string accountNumber, int maxResults = 100)
        {
            return await _dbSet
                .Where(dpda => dpda.SO_TAI_KHOAN == accountNumber)
                .OrderByDescending(dpda => dpda.CREATED_DATE)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy dữ liệu DPDA theo số thẻ
        /// </summary>
        public async Task<IEnumerable<DPDA>> GetByCardNumberAsync(string cardNumber, int maxResults = 100)
        {
            return await _dbSet
                .Where(dpda => dpda.SO_THE == cardNumber)
                .OrderByDescending(dpda => dpda.CREATED_DATE)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy dữ liệu DPDA theo trạng thái
        /// </summary>
        public async Task<IEnumerable<DPDA>> GetByStatusAsync(string status, int maxResults = 100)
        {
            return await _dbSet
                .Where(dpda => dpda.TRANG_THAI == status)
                .OrderByDescending(dpda => dpda.CREATED_DATE)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy số lượng thẻ theo chi nhánh và trạng thái
        /// </summary>
        public async Task<int> GetCardCountByBranchAndStatusAsync(string branchCode, string status)
        {
            return await _dbSet
                .CountAsync(dpda => dpda.MA_CHI_NHANH == branchCode && dpda.TRANG_THAI == status);
        }

        /// <summary>
        /// Cập nhật nhiều DPDA
        /// </summary>
        public void UpdateRange(IEnumerable<DPDA> entities)
        {
            _dbSet.UpdateRange(entities);
        }
    }
}
