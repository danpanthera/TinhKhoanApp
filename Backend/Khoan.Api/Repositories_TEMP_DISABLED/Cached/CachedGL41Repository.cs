using System.Linq.Expressions;
using Khoan.Api.Models.DataTables;
using Khoan.Api.Services.Caching;

namespace Khoan.Api.Repositories.Cached
{
    /// <summary>
    /// CachedGL41Repository - Triển khai IGL41Repository với caching
    /// </summary>
    public class CachedGL41Repository : IGL41Repository
    {
        private readonly IGL41Repository _repository;
        private readonly ICacheService _cache;
        private readonly ILogger<CachedGL41Repository> _logger;
        private readonly string _cachePrefix = "gl41_";
        private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(30);

        /// <summary>
        /// Khởi tạo repository với cache service
        /// </summary>
        public CachedGL41Repository(
            IGL41Repository repository,
            ICacheService cache,
            ILogger<CachedGL41Repository> logger)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Lấy dữ liệu GL41 gần đây nhất (với cache)
        /// </summary>
        public async Task<IEnumerable<GL41>> GetRecentAsync(int count = 10)
        {
            string cacheKey = $"{_cachePrefix}recent_{count}";
            return await _cache.GetOrCreateAsync(
                cacheKey,
                async () => await _repository.GetRecentAsync(count),
                _defaultExpiration);
        }

        /// <summary>
        /// Lấy dữ liệu GL41 theo ngày (với cache)
        /// </summary>
        public async Task<IEnumerable<GL41>> GetByDateAsync(DateTime date)
        {
            string cacheKey = $"{_cachePrefix}date_{date:yyyyMMdd}";
            return await _cache.GetOrCreateAsync(
                cacheKey,
                async () => await _repository.GetByDateAsync(date),
                _defaultExpiration);
        }

        /// <summary>
        /// Lấy dữ liệu GL41 theo đơn vị (với cache)
        /// </summary>
        public async Task<IEnumerable<GL41>> GetByUnitCodeAsync(string unitCode, int maxResults = 100)
        {
            string cacheKey = $"{_cachePrefix}unit_{unitCode}_{maxResults}";
            return await _cache.GetOrCreateAsync(
                cacheKey,
                async () => await _repository.GetByUnitCodeAsync(unitCode, maxResults),
                _defaultExpiration);
        }

        /// <summary>
        /// Lấy dữ liệu GL41 theo tài khoản (với cache)
        /// </summary>
        public async Task<IEnumerable<GL41>> GetByAccountCodeAsync(string accountCode, int maxResults = 100)
        {
            string cacheKey = $"{_cachePrefix}account_{accountCode}_{maxResults}";
            return await _cache.GetOrCreateAsync(
                cacheKey,
                async () => await _repository.GetByAccountCodeAsync(accountCode, maxResults),
                _defaultExpiration);
        }

        /// <summary>
        /// Lấy tổng dư đầu kỳ theo đơn vị (với cache)
        /// </summary>
        public async Task<decimal> GetTotalOpeningBalanceByUnitAsync(string unitCode, DateTime? date = null)
        {
            string dateStr = date.HasValue ? date.Value.ToString("yyyyMMdd") : "latest";
            string cacheKey = $"{_cachePrefix}opening_balance_{unitCode}_{dateStr}";
            return await _cache.GetOrCreateAsync(
                cacheKey,
                async () => await _repository.GetTotalOpeningBalanceByUnitAsync(unitCode, date),
                _defaultExpiration);
        }

        /// <summary>
        /// Lấy tổng dư cuối kỳ theo đơn vị (với cache)
        /// </summary>
        public async Task<decimal> GetTotalClosingBalanceByUnitAsync(string unitCode, DateTime? date = null)
        {
            string dateStr = date.HasValue ? date.Value.ToString("yyyyMMdd") : "latest";
            string cacheKey = $"{_cachePrefix}closing_balance_{unitCode}_{dateStr}";
            return await _cache.GetOrCreateAsync(
                cacheKey,
                async () => await _repository.GetTotalClosingBalanceByUnitAsync(unitCode, date),
                _defaultExpiration);
        }

        /// <summary>
        /// Lấy tổng phát sinh theo đơn vị (với cache)
        /// </summary>
        public async Task<(decimal Debit, decimal Credit)> GetTotalTransactionsByUnitAsync(string unitCode, DateTime? date = null)
        {
            string dateStr = date.HasValue ? date.Value.ToString("yyyyMMdd") : "latest";
            string cacheKey = $"{_cachePrefix}transactions_{unitCode}_{dateStr}";
            return await _cache.GetOrCreateAsync(
                cacheKey,
                async () => await _repository.GetTotalTransactionsByUnitAsync(unitCode, date),
                _defaultExpiration);
        }

        #region IRepository<GL41> Implementation

        /// <summary>
        /// Thêm GL41 mới và xóa cache liên quan
        /// </summary>
        public async Task AddAsync(GL41 entity)
        {
            await _repository.AddAsync(entity);
            InvalidateCache(entity);
        }

        /// <summary>
        /// Thêm nhiều GL41 và xóa cache
        /// </summary>
        public async Task AddRangeAsync(IEnumerable<GL41> entities)
        {
            await _repository.AddRangeAsync(entities);
            InvalidateAllCache();
        }

        /// <summary>
        /// Tìm GL41 theo điều kiện (không cache - dùng cho search)
        /// </summary>
        public async Task<IEnumerable<GL41>> FindAsync(Expression<Func<GL41, bool>> predicate)
        {
            return await _repository.FindAsync(predicate);
        }

        /// <summary>
        /// Lấy tất cả GL41 (với cache)
        /// </summary>
        public async Task<IEnumerable<GL41>> GetAllAsync()
        {
            string cacheKey = $"{_cachePrefix}all";
            return await _cache.GetOrCreateAsync(
                cacheKey,
                async () => await _repository.GetAllAsync(),
                _defaultExpiration);
        }

        /// <summary>
        /// Lấy GL41 theo ID (với cache)
        /// </summary>
        public async Task<GL41?> GetByIdAsync(int id)
        {
            string cacheKey = $"{_cachePrefix}id_{id}";
            return await _cache.GetOrCreateAsync(
                cacheKey,
                async () => await _repository.GetByIdAsync(id),
                _defaultExpiration);
        }

        /// <summary>
        /// Xóa GL41 và cache liên quan
        /// </summary>
        public void Remove(GL41 entity)
        {
            _repository.Remove(entity);
            InvalidateCache(entity);
        }

        /// <summary>
        /// Xóa nhiều GL41 và cache liên quan
        /// </summary>
        public void RemoveRange(IEnumerable<GL41> entities)
        {
            _repository.RemoveRange(entities);
            InvalidateAllCache();
        }

        /// <summary>
        /// Cập nhật GL41 và cache liên quan
        /// </summary>
        public void Update(GL41 entity)
        {
            _repository.Update(entity);
            InvalidateCache(entity);
        }

        /// <summary>
        /// Cập nhật nhiều GL41 và cache liên quan
        /// </summary>
        public void UpdateRange(IEnumerable<GL41> entities)
        {
            _repository.UpdateRange(entities);
            InvalidateAllCache();
        }

        /// <summary>
        /// Đếm số lượng GL41 trong hệ thống
        /// </summary>
        public async Task<int> CountAsync()
        {
            string cacheKey = $"{_cachePrefix}count";
            return await _cache.GetOrCreateAsync(
                cacheKey,
                async () => await _repository.CountAsync(),
                _defaultExpiration);
        }

        /// <summary>
        /// Đếm số lượng GL41 theo điều kiện
        /// </summary>
        public async Task<int> CountAsync(Expression<Func<GL41, bool>> predicate)
        {
            // Không cache kết quả này vì predicate có thể thay đổi liên tục
            return await _repository.CountAsync(predicate);
        }

        /// <summary>
        /// Kiểm tra GL41 tồn tại theo điều kiện
        /// </summary>
        public async Task<bool> ExistsAsync(Expression<Func<GL41, bool>> predicate)
        {
            // Không cache kết quả này vì predicate có thể thay đổi liên tục
            return await _repository.ExistsAsync(predicate);
        }

        #endregion

        #region Helper Methods

        private void InvalidateCache(GL41 entity)
        {
            _logger.LogDebug("Invalidating cache for GL41 entity with ID {Id}", entity.ID);

            // Clear specific caches related to this entity
            if (!string.IsNullOrEmpty(entity.MA_CN))
            {
                _cache.RemoveByPrefix($"{_cachePrefix}unit_{entity.MA_CN}");
                _cache.RemoveByPrefix($"{_cachePrefix}opening_balance_{entity.MA_CN}");
                _cache.RemoveByPrefix($"{_cachePrefix}closing_balance_{entity.MA_CN}");
                _cache.RemoveByPrefix($"{_cachePrefix}transactions_{entity.MA_CN}");
            }

            if (!string.IsNullOrEmpty(entity.MA_TK))
            {
                _cache.RemoveByPrefix($"{_cachePrefix}account_{entity.MA_TK}");
            }

            // Clear date-specific cache
            string dateStr = entity.NGAY_DL.ToString("yyyyMMdd");
            _cache.RemoveByPrefix($"{_cachePrefix}date_{dateStr}");

            // Clear general caches
            _cache.RemoveByPrefix($"{_cachePrefix}recent_");
            _cache.Remove($"{_cachePrefix}all");
            _cache.Remove($"{_cachePrefix}id_{entity.ID}");
        }

        private void InvalidateAllCache()
        {
            _logger.LogDebug("Invalidating all GL41 cache");
            _cache.RemoveByPrefix(_cachePrefix);
        }

        /// <summary>
        /// Lưu thay đổi và xóa cache
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            var result = await _repository.SaveChangesAsync();
            if (result > 0)
            {
                InvalidateAllCache();
            }
            return result;
        }

        #endregion
    }
}
