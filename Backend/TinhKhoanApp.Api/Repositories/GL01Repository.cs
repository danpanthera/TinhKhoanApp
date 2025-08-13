using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.Entities;

namespace TinhKhoanApp.Api.Repositories
{
    /// <summary>
    /// GL01 Repository - triển khai IGL01Repository
    /// </summary>
    public class GL01Repository : Repository<GL01Entity>, IGL01Repository
    {
        /// <summary>
        /// Constructor khởi tạo GL01Repository với ApplicationDbContext
        /// </summary>
        public GL01Repository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy dữ liệu GL01 gần đây nhất, sắp xếp theo CreatedAt
        /// </summary>
        public new async Task<IEnumerable<GL01Entity>> GetRecentAsync(int count = 10)
        {
            return await _dbSet
                .OrderByDescending(gl01 => gl01.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy dữ liệu GL01 theo ngày
        /// </summary>
        public async Task<IEnumerable<GL01Entity>> GetByDateAsync(DateTime date)
        {
            return await _dbSet
                .Where(gl01 => gl01.NGAY_DL.Date == date.Date)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy dữ liệu GL01 theo mã đơn vị
        /// </summary>
        public async Task<IEnumerable<GL01Entity>> GetByUnitCodeAsync(string unitCode, int maxResults = 100)
        {
            return await _dbSet
                .Where(gl01 => gl01.POST_BR == unitCode)
                .OrderByDescending(gl01 => gl01.CreatedAt)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy dữ liệu GL01 theo tài khoản
        /// </summary>
        public async Task<IEnumerable<GL01Entity>> GetByAccountCodeAsync(string accountCode, int maxResults = 100)
        {
            return await _dbSet
                .Where(gl01 => gl01.TAI_KHOAN == accountCode)
                .OrderByDescending(gl01 => gl01.CreatedAt)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy tổng giao dịch theo mã đơn vị và loại giao dịch (DR/CR)
        /// </summary>
        public async Task<decimal> GetTotalTransactionsByUnitAsync(string unitCode, string drCrFlag, DateTime? date = null)
        {
            var query = _dbSet.Where(gl01 => gl01.POST_BR == unitCode && gl01.DR_CR == drCrFlag);

            if (date.HasValue)
            {
                query = query.Where(gl01 => gl01.NGAY_DL.Date == date.Value.Date);
            }

            return await query.SumAsync(gl01 => gl01.SO_TIEN_GD ?? 0);
        }

        /// <summary>
        /// Lấy tổng giao dịch theo ngày và loại giao dịch (DR/CR)
        /// </summary>
        public async Task<decimal> GetTotalTransactionsByDateAsync(DateTime date, string drCrFlag)
        {
            return await _dbSet
                .Where(gl01 => gl01.NGAY_DL.Date == date.Date && gl01.DR_CR == drCrFlag)
                .SumAsync(gl01 => gl01.SO_TIEN_GD ?? 0);
        }

        /// <summary>
        /// Cập nhật nhiều GL01
        /// </summary>
        public void UpdateRange(IEnumerable<GL01Entity> entities)
        {
            _dbSet.UpdateRange(entities);
        }
    }
}
