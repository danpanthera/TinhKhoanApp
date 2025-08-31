using Khoan.Api.Data;
using Khoan.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Khoan.Api.Repositories
{
    /// <summary>
    /// Repository implementation for GL41 - Partitioned Columnstore Optimized
    /// Handles 13 business columns + 4 system columns = 17 total columns
    /// Direct import from CSV files containing "gl41"
    /// NGAY_DL extracted from filename and converted to datetime2 (dd/mm/yyyy)
    /// </summary>
    public class GL41Repository : Repository<GL41Entity>, IGL41Repository
    {
        public GL41Repository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Get paginated GL41 data with total count (partitioned columnstore optimized)
        /// </summary>
        public async Task<(IEnumerable<GL41Entity> Data, int TotalCount)> GetAllPagedAsync(int page = 1, int pageSize = 10)
        {
            var query = _context.GL41.OrderByDescending(x => x.NGAY_DL).ThenBy(x => x.MA_CN);
            
            var totalCount = await query.CountAsync();
            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }

        /// <summary>
        /// Get GL41 records by date range (partitioned columnstore optimized)
        /// </summary>
        public async Task<IEnumerable<GL41Entity>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.GL41
                .Where(x => x.NGAY_DL >= fromDate && x.NGAY_DL <= toDate)
                .OrderBy(x => x.NGAY_DL)
                .ThenBy(x => x.MA_CN)
                .ToListAsync();
        }

        /// <summary>
        /// Get GL41 records by unit code
        /// </summary>
        public async Task<IEnumerable<GL41Entity>> GetByUnitCodeAsync(string unitCode, int maxResults = 100)
        {
            return await _context.GL41
                .Where(x => x.MA_CN == unitCode)
                .OrderByDescending(x => x.NGAY_DL)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <summary>
        /// Get GL41 records by account code
        /// </summary>
        public async Task<IEnumerable<GL41Entity>> GetByAccountCodeAsync(string accountCode, int maxResults = 100)
        {
            return await _context.GL41
                .Where(x => x.MA_TK == accountCode)
                .OrderByDescending(x => x.NGAY_DL)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <summary>
        /// Bulk insert GL41 records (partitioned columnstore optimized)
        /// </summary>
        public async Task<int> BulkInsertAsync(IEnumerable<GL41Entity> entities)
        {
            var entityList = entities.ToList();
            if (!entityList.Any()) return 0;

            // Set system columns
            foreach (var entity in entityList)
            {
                entity.CREATED_DATE = DateTime.UtcNow;
                entity.UPDATED_DATE = DateTime.UtcNow;
            }

            await _context.GL41.AddRangeAsync(entityList);
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Delete GL41 records by date range
        /// </summary>
        public async Task<int> DeleteByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var recordsToDelete = await _context.GL41
                .Where(x => x.NGAY_DL >= fromDate && x.NGAY_DL <= toDate)
                .ToListAsync();

            if (recordsToDelete.Any())
            {
                _context.GL41.RemoveRange(recordsToDelete);
                return await _context.SaveChangesAsync();
            }

            return 0;
        }

        /// <summary>
        /// Get summary analytics by unit for GL41 balance data
        /// </summary>
        public async Task<IEnumerable<dynamic>> GetSummaryByUnitAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.GL41
                .Where(x => x.NGAY_DL >= fromDate && x.NGAY_DL <= toDate)
                .GroupBy(x => x.MA_CN)
                .Select(g => new
                {
                    UnitCode = g.Key,
                    TotalRecords = g.Count(),
                    TotalDebitBalance = g.Sum(x => x.DN_DAUKY ?? 0) + g.Sum(x => x.DN_CUOIKY ?? 0),
                    TotalCreditBalance = g.Sum(x => x.DC_DAUKY ?? 0) + g.Sum(x => x.DC_CUOIKY ?? 0),
                    TotalDebitTransaction = g.Sum(x => x.SBT_NO ?? 0),
                    TotalCreditTransaction = g.Sum(x => x.SBT_CO ?? 0),
                    Currencies = g.Select(x => x.LOAI_TIEN).Distinct().ToList()
                })
                .ToListAsync();
        }

        /// <summary>
        /// Get total opening balance by unit
        /// </summary>
        public async Task<decimal> GetTotalOpeningBalanceByUnitAsync(string unitCode, DateTime? date = null)
        {
            var query = _context.GL41.Where(x => x.MA_CN == unitCode);
            
            if (date.HasValue)
                query = query.Where(x => x.NGAY_DL == date.Value);

            return await query.SumAsync(x => (x.DN_DAUKY ?? 0) - (x.DC_DAUKY ?? 0));
        }

        /// <summary>
        /// Get total closing balance by unit
        /// </summary>
        public async Task<decimal> GetTotalClosingBalanceByUnitAsync(string unitCode, DateTime? date = null)
        {
            var query = _context.GL41.Where(x => x.MA_CN == unitCode);
            
            if (date.HasValue)
                query = query.Where(x => x.NGAY_DL == date.Value);

            return await query.SumAsync(x => (x.DN_CUOIKY ?? 0) - (x.DC_CUOIKY ?? 0));
        }

        /// <summary>
        /// Get debit/credit transaction summary by unit
        /// </summary>
        public async Task<(decimal DebitTotal, decimal CreditTotal)> GetTransactionSummaryByUnitAsync(string unitCode, DateTime? date = null)
        {
            var query = _context.GL41.Where(x => x.MA_CN == unitCode);
            
            if (date.HasValue)
                query = query.Where(x => x.NGAY_DL == date.Value);

            var debitTotal = await query.SumAsync(x => x.SBT_NO ?? 0);
            var creditTotal = await query.SumAsync(x => x.SBT_CO ?? 0);

            return (debitTotal, creditTotal);
        }

        /// <summary>
        /// Check if GL41 data exists for specific date
        /// </summary>
        public async Task<bool> HasDataForDateAsync(DateTime date)
        {
            return await _context.GL41.AnyAsync(x => x.NGAY_DL == date);
        }

        /// <summary>
        /// Get distinct currencies for GL41 analytics
        /// </summary>
        public async Task<List<string>> GetDistinctCurrenciesAsync()
        {
            return await _context.GL41
                .Where(x => !string.IsNullOrEmpty(x.LOAI_TIEN))
                .Select(x => x.LOAI_TIEN)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();
        }

        /// <summary>
        /// Update range of GL41 entities
        /// </summary>
        public void UpdateRange(IEnumerable<GL41Entity> entities)
        {
            foreach (var entity in entities)
            {
                entity.UPDATED_DATE = DateTime.UtcNow;
            }
            _context.GL41.UpdateRange(entities);
        }
    }
}
