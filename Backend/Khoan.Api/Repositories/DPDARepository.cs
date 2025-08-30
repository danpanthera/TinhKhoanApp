using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;
using Khoan.Api.Models.Entities;
using Khoan.Api.Models.Common;
using Khoan.Api.Repositories.Interfaces;
using Khoan.Api.Repositories.Base;

namespace Khoan.Api.Repositories
{
    /// <summary>
    /// DPDA Repository Implementation - Thẻ nộp đơn gửi tiết kiệm
    /// Theo pattern DP01Repository với 13 business columns từ CSV
    /// CSV-First: Business columns từ CSV là chuẩn cho tất cả layers
    /// </summary>
    public class DPDARepository : Repository<DPDAEntity>, IDPDARepository
    {
        public DPDARepository(ApplicationDbContext context) : base(context)
        {
        }

        #region Paging Support

        /// <summary>
        /// Get DPDA with paging and optional search
        /// </summary>
        public async Task<(IEnumerable<DPDAEntity> entities, long totalCount)> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            var query = _context.Set<DPDAEntity>().AsQueryable();

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(d =>
                    EF.Functions.Like(d.MA_KHACH_HANG, $"%{searchTerm}%") ||
                    EF.Functions.Like(d.TEN_KHACH_HANG, $"%{searchTerm}%") ||
                    EF.Functions.Like(d.SO_TAI_KHOAN, $"%{searchTerm}%") ||
                    EF.Functions.Like(d.SO_THE, $"%{searchTerm}%"));
            }

            var totalCount = await query.LongCountAsync();

            var entities = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (entities, totalCount);
        }

        #endregion

        #region Business Key Searches - CSV column names preserved

        /// <summary>
        /// Search DPDA by customer code (MA_KHACH_HANG)
        /// </summary>
        public async Task<IEnumerable<DPDAEntity>> GetByCustomerCodeAsync(string customerCode, int limit = 100)
        {
            return await _context.Set<DPDAEntity>()
                .Where(x => x.MA_KHACH_HANG == customerCode)
                .OrderByDescending(x => x.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        /// <summary>
        /// Search DPDA by branch code (MA_CHI_NHANH)
        /// </summary>
        public async Task<IEnumerable<DPDAEntity>> GetByBranchCodeAsync(string branchCode, int limit = 100)
        {
            return await _context.Set<DPDAEntity>()
                .Where(x => x.MA_CHI_NHANH == branchCode)
                .OrderByDescending(x => x.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        /// <summary>
        /// Search DPDA by account number (SO_TAI_KHOAN)
        /// </summary>
        public async Task<IEnumerable<DPDAEntity>> GetByAccountNumberAsync(string accountNumber, int limit = 100)
        {
            return await _context.Set<DPDAEntity>()
                .Where(x => x.SO_TAI_KHOAN == accountNumber)
                .OrderByDescending(x => x.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        /// <summary>
        /// Search DPDA by card number (SO_THE)
        /// </summary>
        public async Task<IEnumerable<DPDAEntity>> GetByCardNumberAsync(string cardNumber, int limit = 100)
        {
            return await _context.Set<DPDAEntity>()
                .Where(x => x.SO_THE == cardNumber)
                .OrderByDescending(x => x.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        #endregion

        #region Analytics Support

        /// <summary>
        /// Get DPDA count by branch (MA_CHI_NHANH)
        /// </summary>
        public async Task<Dictionary<string, long>> GetCountByBranchAsync(DateTime? asOfDate = null)
        {
            var query = _context.Set<DPDAEntity>().AsQueryable();

            if (asOfDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt <= asOfDate.Value);
            }

            return await query
                .GroupBy(x => x.MA_CHI_NHANH)
                .Select(g => new { Branch = g.Key, Count = g.LongCount() })
                .ToDictionaryAsync(x => x.Branch, x => x.Count);
        }

        /// <summary>
        /// Get DPDA count by card type (LOAI_THE)
        /// </summary>
        public async Task<Dictionary<string, long>> GetCountByCardTypeAsync(DateTime? asOfDate = null)
        {
            var query = _context.Set<DPDAEntity>().AsQueryable();

            if (asOfDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt <= asOfDate.Value);
            }

            return await query
                .GroupBy(x => x.LOAI_THE)
                .Select(g => new { CardType = g.Key ?? "N/A", Count = g.LongCount() })
                .ToDictionaryAsync(x => x.CardType, x => x.Count);
        }

        /// <summary>
        /// Get DPDA count by status (TRANG_THAI)
        /// </summary>
        public async Task<Dictionary<string, long>> GetCountByStatusAsync(DateTime? asOfDate = null)
        {
            var query = _context.Set<DPDAEntity>().AsQueryable();

            if (asOfDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt <= asOfDate.Value);
            }

            return await query
                .GroupBy(x => x.TRANG_THAI)
                .Select(g => new { Status = g.Key ?? "N/A", Count = g.LongCount() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }

        #endregion

        #region Basic CRUD Operations

        /// <summary>
        /// Get all DPDA entities
        /// </summary>
        public async Task<IEnumerable<DPDAEntity>> GetAllAsync()
        {
            return await _context.Set<DPDAEntity>().ToListAsync();
        }

        /// <summary>
        /// Get DPDA entity by ID
        /// </summary>
        public async Task<DPDAEntity?> GetByIdAsync(long id)
        {
            return await _context.Set<DPDAEntity>().FindAsync(id);
        }

        /// <summary>
        /// Create new DPDA entity
        /// </summary>
        public async Task<DPDAEntity> CreateAsync(DPDAEntity entity)
        {
            _context.Set<DPDAEntity>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Update existing DPDA entity
        /// </summary>
        public async Task<DPDAEntity> UpdateAsync(DPDAEntity entity)
        {
            _context.Set<DPDAEntity>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Delete DPDA entity by ID
        /// </summary>
        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;

            _context.Set<DPDAEntity>().Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Get total count by branch code
        /// </summary>
        public async Task<long> GetTotalCountByBranchAsync(string branchCode, DateTime? date = null)
        {
            var query = _context.Set<DPDAEntity>().AsQueryable();

            // Filter by branch code
            query = query.Where(x => x.MA_CHI_NHANH == branchCode);

            // Apply date filter if provided
            if (date.HasValue)
            {
                var startOfDay = date.Value.Date;
                var endOfDay = startOfDay.AddDays(1);
                query = query.Where(x => x.CreatedAt >= startOfDay && x.CreatedAt < endOfDay);
            }

            return await query.LongCountAsync();
        }

        /// <summary>
        /// Get card type distribution - rename from GetCountByCardTypeAsync
        /// </summary>
        public async Task<Dictionary<string, long>> GetCardTypeDistributionAsync(DateTime? asOfDate = null)
        {
            return await GetCountByCardTypeAsync(asOfDate);
        }

        /// <summary>
        /// Get status distribution - rename from GetCountByStatusAsync
        /// </summary>
        public async Task<Dictionary<string, long>> GetStatusDistributionAsync(DateTime? asOfDate = null)
        {
            return await GetCountByStatusAsync(asOfDate);
        }

        /// <summary>
        /// Get total count of all DPDA records
        /// </summary>
        public async Task<long> GetTotalCountAsync()
        {
            return await _context.Set<DPDAEntity>().LongCountAsync();
        }

        /// <summary>
        /// Get card count by status - alias for GetStatusDistributionAsync
        /// </summary>
        public async Task<Dictionary<string, long>> GetCardCountByStatusAsync(DateTime? asOfDate = null)
        {
            return await GetStatusDistributionAsync(asOfDate);
        }

        /// <summary>
        /// Get card count by type - alias for GetCardTypeDistributionAsync
        /// </summary>
        public async Task<Dictionary<string, long>> GetCardCountByTypeAsync(DateTime? asOfDate = null)
        {
            return await GetCardTypeDistributionAsync(asOfDate);
        }

        /// <summary>
        /// Get card count by branch - same as GetCountByBranchAsync
        /// </summary>
        public async Task<Dictionary<string, long>> GetCardCountByBranchAsync(DateTime? asOfDate = null)
        {
            return await GetCountByBranchAsync(asOfDate);
        }

        /// <summary>
        /// Get card count by category - placeholder implementation
        /// </summary>
        public async Task<Dictionary<string, long>> GetCardCountByCategoryAsync(DateTime? asOfDate = null)
        {
            // Implement based on actual category field in DPDA
            // For now, return empty dictionary
            return new Dictionary<string, long>();
        }

        /// <summary>
        /// Get card count by date range
        /// </summary>
        public async Task<Dictionary<string, long>> GetCardCountByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var query = _context.Set<DPDAEntity>().AsQueryable();

            // Apply date range filter
            query = query.Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate);

            return await query
                .GroupBy(x => x.CreatedAt.Date)
                .Select(g => new { Date = g.Key.ToString("yyyy-MM-dd"), Count = g.LongCount() })
                .ToDictionaryAsync(x => x.Date, x => x.Count);
        }

        /// <summary>
        /// Search DPDA by status (TRANG_THAI)
        /// </summary>
        public async Task<IEnumerable<DPDAEntity>> GetByStatusAsync(string status, int maxResults = 100)
        {
            return await _context.Set<DPDAEntity>()
                .Where(x => x.TRANG_THAI == status)
                .OrderByDescending(x => x.CreatedAt)
                .Take(maxResults)
                .ToListAsync();
        }

        /// <summary>
        /// Get DPDA items by NGAY_DL date
        /// </summary>
        public async Task<IEnumerable<DPDAEntity>> GetByDateAsync(DateTime date, int maxResults = 100)
        {
            var start = date.Date;
            var end = start.AddDays(1);
            return await _context.Set<DPDAEntity>()
                .Where(x => x.NGAY_DL >= start && x.NGAY_DL < end)
                .OrderByDescending(x => x.CreatedAt)
                .Take(maxResults)
                .ToListAsync();
        }

        #endregion
    }
}
