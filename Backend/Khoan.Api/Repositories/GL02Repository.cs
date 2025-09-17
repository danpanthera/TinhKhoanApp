using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;
using DataTables = Khoan.Api.Models.DataTables;

namespace Khoan.Api.Repositories
{
    /// <summary>
    /// Repository implementation cho DataTables.GL02 - General Ledger Summary (Partitioned Columnstore)
    /// </summary>
    public class GL02Repository : Repository<DataTables.GL02>, IGL02Repository
    {
        public GL02Repository(ApplicationDbContext context) : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DataTables.GL02>> GetRecentAsync(int count = 10)
        {
            return await _dbSet
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CRTDTM)
                .Take(count)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DataTables.GL02>> GetByDateAsync(DateTime date, int maxResults = 100)
        {
            return await _dbSet
                .Where(x => x.NGAY_DL.Date == date.Date)
                .OrderByDescending(x => x.CRTDTM)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DataTables.GL02>> GetByUnitAsync(string unit, int maxResults = 100)
        {
            return await _dbSet
                .Where(x => x.UNIT == unit)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CRTDTM)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DataTables.GL02>> GetByBranchCodeAsync(string branchCode, int maxResults = 100)
        {
            return await _dbSet
                .Where(x => x.TRBRCD == branchCode)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CRTDTM)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DataTables.GL02>> GetByLocalAccountAsync(string locac, int maxResults = 100)
        {
            return await _dbSet
                .Where(x => x.LOCAC == locac)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CRTDTM)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DataTables.GL02>> GetByTransactionCodeAsync(string trcd, int maxResults = 100)
        {
            return await _dbSet
                .Where(x => x.TRCD == trcd)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CRTDTM)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DataTables.GL02>> GetByCustomerAsync(string customer, int maxResults = 100)
        {
            return await _dbSet
                .Where(x => x.CUSTOMER == customer)
                .OrderByDescending(x => x.NGAY_DL)
                .ThenByDescending(x => x.CRTDTM)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<decimal> GetTotalTransactionsByUnitAsync(string unit, string type)
        {
            if (type.ToUpper() == "DR")
            {
                return await _dbSet
                    .Where(x => x.UNIT == unit && x.DRAMOUNT.HasValue)
                    .SumAsync(x => x.DRAMOUNT.Value);
            }
            else
            {
                return await _dbSet
                    .Where(x => x.UNIT == unit && x.CRAMOUNT.HasValue)
                    .SumAsync(x => x.CRAMOUNT.Value);
            }
        }
    }
}
