using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Repositories
{
    /// <summary>
    /// Repository implementation for RR01 entity
    /// </summary>
    public class RR01Repository : Repository<RR01>, IRR01Repository
    {
        public RR01Repository(ApplicationDbContext context) : base(context)
        {
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RR01>> GetByDateAsync(DateTime statementDate)
        {
            return await _context.RR01
                .Where(r => r.NGAY_DL.Date == statementDate.Date)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RR01>> GetByBranchAsync(string branchCode, DateTime? statementDate = null)
        {
            var query = _context.RR01.AsQueryable();

            if (!string.IsNullOrEmpty(branchCode))
            {
                query = query.Where(r => r.BRCD.Trim() == branchCode.Trim());
            }

            if (statementDate.HasValue)
            {
                query = query.Where(r => r.NGAY_DL.Date == statementDate.Value.Date);
            }

            return await query.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RR01>> GetByCustomerAsync(string customerId, DateTime? statementDate = null)
        {
            var query = _context.RR01.AsQueryable();

            if (!string.IsNullOrEmpty(customerId))
            {
                query = query.Where(r => r.MA_KH.Trim() == customerId.Trim());
            }

            if (statementDate.HasValue)
            {
                query = query.Where(r => r.NGAY_DL.Date == statementDate.Value.Date);
            }

            return await query.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RR01>> GetByFileNameAsync(string fileName)
        {
            return await _context.RR01
                .Where(r => r.FILE_NAME == fileName)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<(IEnumerable<RR01> Records, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            DateTime? statementDate = null,
            string? branchCode = null,
            string? customerId = null)
        {
            var query = _context.RR01.AsQueryable();

            // Apply filters
            if (statementDate.HasValue)
            {
                query = query.Where(r => r.NGAY_DL.Date == statementDate.Value.Date);
            }

            if (!string.IsNullOrEmpty(branchCode))
            {
                query = query.Where(r => r.BRCD.Trim() == branchCode.Trim());
            }

            if (!string.IsNullOrEmpty(customerId))
            {
                query = query.Where(r => r.MA_KH.Trim() == customerId.Trim());
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Get the page of data
            var data = await query
                .OrderByDescending(r => r.NGAY_DL)
                .ThenBy(r => r.BRCD)
                .ThenBy(r => r.MA_KH)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }
    }
}
