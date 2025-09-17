using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;
using Khoan.Api.Models.DataTables;

namespace Khoan.Api.Repositories
{
    /// <summary>
    /// GL41 Repository - triển khai IGL41Repository
    /// </summary>
    public class GL41Repository : Repository<GL41>, IGL41Repository
    {
        public GL41Repository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy dữ liệu GL41 gần đây nhất, sắp xếp theo CREATED_DATE
        /// </summary>
        public new async Task<IEnumerable<GL41>> GetRecentAsync(int count = 10)
        {
            return await _dbSet
                .OrderByDescending(gl41 => gl41.CREATED_DATE)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<GL41>> GetByDateAsync(DateTime date)
        {
            return await _dbSet
                .Where(gl41 => gl41.NGAY_DL.Date == date.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<GL41>> GetByUnitCodeAsync(string unitCode, int maxResults = 100)
        {
            return await _dbSet
                .Where(gl41 => gl41.MA_CN == unitCode)
                .OrderByDescending(gl41 => gl41.CREATED_DATE)
                .Take(maxResults)
                .ToListAsync();
        }

        public async Task<IEnumerable<GL41>> GetByAccountCodeAsync(string accountCode, int maxResults = 100)
        {
            return await _dbSet
                .Where(gl41 => gl41.MA_TK == accountCode)
                .OrderByDescending(gl41 => gl41.CREATED_DATE)
                .Take(maxResults)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalOpeningBalanceByUnitAsync(string unitCode, DateTime? date = null)
        {
            var query = _dbSet.Where(gl41 => gl41.MA_CN == unitCode);

            if (date.HasValue)
            {
                query = query.Where(gl41 => gl41.NGAY_DL.Date == date.Value.Date);
            }

            // Tính tổng dư đầu kỳ (phân biệt nợ/có)
            var result = await query.SumAsync(gl41 =>
                (gl41.DN_DAUKY.HasValue ? gl41.DN_DAUKY.Value : 0) -
                (gl41.DC_DAUKY.HasValue ? gl41.DC_DAUKY.Value : 0));

            return result;
        }

        public async Task<decimal> GetTotalClosingBalanceByUnitAsync(string unitCode, DateTime? date = null)
        {
            var query = _dbSet.Where(gl41 => gl41.MA_CN == unitCode);

            if (date.HasValue)
            {
                query = query.Where(gl41 => gl41.NGAY_DL.Date == date.Value.Date);
            }

            // Tính tổng dư cuối kỳ (phân biệt nợ/có)
            var result = await query.SumAsync(gl41 =>
                (gl41.DN_CUOIKY.HasValue ? gl41.DN_CUOIKY.Value : 0) -
                (gl41.DC_CUOIKY.HasValue ? gl41.DC_CUOIKY.Value : 0));

            return result;
        }

        public async Task<(decimal Debit, decimal Credit)> GetTotalTransactionsByUnitAsync(string unitCode, DateTime? date = null)
        {
            var query = _dbSet.Where(gl41 => gl41.MA_CN == unitCode);

            if (date.HasValue)
            {
                query = query.Where(gl41 => gl41.NGAY_DL.Date == date.Value.Date);
            }

            // Tính tổng phát sinh nợ và có
            var debitSum = await query.SumAsync(gl41 => gl41.ST_GHINO.HasValue ? gl41.ST_GHINO.Value : 0);
            var creditSum = await query.SumAsync(gl41 => gl41.ST_GHICO.HasValue ? gl41.ST_GHICO.Value : 0);

            return (debitSum, creditSum);
        }

        /// <summary>
        /// Cập nhật nhiều GL41
        /// </summary>
        public void UpdateRange(IEnumerable<GL41> entities)
        {
            _dbSet.UpdateRange(entities);
        }
    }
}
