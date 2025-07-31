using Microsoft.Extensions.Caching.Memory;

namespace TinhKhoanApp.Api.Services.Caching
{
    /// <summary>
    /// Interface cho Cache Service - cung cấp các phương thức để cache dữ liệu
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Lấy dữ liệu từ cache, nếu không có thì thực thi hàm getDataFunction và cache kết quả
        /// </summary>
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> getDataFunction, TimeSpan? expiration = null);

        /// <summary>
        /// Lấy dữ liệu từ cache
        /// </summary>
        T Get<T>(string key);

        /// <summary>
        /// Thêm dữ liệu vào cache
        /// </summary>
        void Set<T>(string key, T data, TimeSpan? expiration = null);

        /// <summary>
        /// Xóa dữ liệu khỏi cache
        /// </summary>
        void Remove(string key);

        /// <summary>
        /// Xóa tất cả dữ liệu có prefix
        /// </summary>
        void RemoveByPrefix(string keyPrefix);
    }

    /// <summary>
    /// Cache Service sử dụng Memory Cache
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryCacheService> _logger;
        private readonly HashSet<string> _keys = new HashSet<string>();

        /// <summary>
        /// Khởi tạo cache service với MemoryCache
        /// </summary>
        public MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Lấy dữ liệu từ cache, nếu không có thì thực thi hàm getDataFunction và cache kết quả
        /// </summary>
        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> getDataFunction, TimeSpan? expiration = null)
        {
            if (_cache.TryGetValue(key, out T cachedData))
            {
                _logger.LogDebug("Cache hit for key: {Key}", key);
                return cachedData;
            }

            _logger.LogDebug("Cache miss for key: {Key}", key);
            var data = await getDataFunction();
            Set(key, data, expiration);
            return data;
        }

        /// <summary>
        /// Lấy dữ liệu từ cache
        /// </summary>
        public T Get<T>(string key)
        {
            if (_cache.TryGetValue(key, out T cachedData))
            {
                _logger.LogDebug("Cache hit for key: {Key}", key);
                return cachedData;
            }

            _logger.LogDebug("Cache miss for key: {Key}", key);
            return default;
        }

        /// <summary>
        /// Thêm dữ liệu vào cache
        /// </summary>
        public void Set<T>(string key, T data, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions();

            if (expiration.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiration;
            }
            else
            {
                // Default expiration: 1 hour
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            }

            // Keep track of keys for prefix-based removal
            lock (_keys)
            {
                _keys.Add(key);
            }

            options.RegisterPostEvictionCallback((k, _, _, _) =>
            {
                lock (_keys)
                {
                    _keys.Remove(k.ToString());
                }
            });

            _cache.Set(key, data, options);
            _logger.LogDebug("Added item to cache with key: {Key}", key);
        }

        /// <summary>
        /// Xóa dữ liệu khỏi cache
        /// </summary>
        public void Remove(string key)
        {
            _cache.Remove(key);

            lock (_keys)
            {
                _keys.Remove(key);
            }

            _logger.LogDebug("Removed item from cache with key: {Key}", key);
        }

        /// <summary>
        /// Xóa tất cả dữ liệu có prefix
        /// </summary>
        public void RemoveByPrefix(string keyPrefix)
        {
            List<string> keysToRemove;

            lock (_keys)
            {
                keysToRemove = _keys.Where(k => k.StartsWith(keyPrefix)).ToList();
            }

            foreach (var key in keysToRemove)
            {
                Remove(key);
            }

            _logger.LogDebug("Removed {Count} items from cache with prefix: {KeyPrefix}",
                keysToRemove.Count, keyPrefix);
        }
    }
}
