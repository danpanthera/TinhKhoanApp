using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;
using System.Text;

namespace TinhKhoanApp.Api.Services
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
        Task<long> IncrementAsync(string key, long value = 1, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    }

    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<MemoryCacheService> _logger;

        public MemoryCacheService(IMemoryCache memoryCache, ILogger<MemoryCacheService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                return Task.FromResult(_memoryCache.Get<T>(key));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting value from memory cache for key: {Key}", key);
                return Task.FromResult<T?>(default);
            }
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var options = new MemoryCacheEntryOptions();
                if (expiration.HasValue)
                {
                    options.AbsoluteExpirationRelativeToNow = expiration;
                }

                _memoryCache.Set(key, value, options);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting value in memory cache for key: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                _memoryCache.Remove(key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing value from memory cache for key: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            // Memory cache doesn't support pattern-based removal easily
            _logger.LogWarning("Pattern-based removal not efficiently supported in memory cache");
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                return Task.FromResult(_memoryCache.TryGetValue(key, out _));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence in memory cache for key: {Key}", key);
                return Task.FromResult(false);
            }
        }

        public Task<long> IncrementAsync(string key, long value = 1, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var currentValue = _memoryCache.Get<long?>(key) ?? 0;
                var newValue = currentValue + value;
                
                var options = new MemoryCacheEntryOptions();
                if (expiration.HasValue)
                {
                    options.AbsoluteExpirationRelativeToNow = expiration;
                }

                _memoryCache.Set(key, newValue, options);
                return Task.FromResult(newValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing value in memory cache for key: {Key}", key);
                return Task.FromResult(0L);
            }
        }
    }

    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
        {
            _redis = redis;
            _database = _redis.GetDatabase();
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var value = await _database.StringGetAsync(key);
                if (!value.HasValue)
                {
                    return default;
                }

                if (typeof(T) == typeof(string))
                {
                    return (T)(object)value.ToString();
                }

                return JsonSerializer.Deserialize<T>(value!, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting value from Redis cache for key: {Key}", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            try
            {
                string serializedValue;
                if (typeof(T) == typeof(string))
                {
                    serializedValue = value?.ToString() ?? string.Empty;
                }
                else
                {
                    serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
                }

                await _database.StringSetAsync(key, serializedValue, expiration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting value in Redis cache for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                await _database.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing value from Redis cache for key: {Key}", key);
            }
        }

        public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            try
            {
                var server = _redis.GetServer(_redis.GetEndPoints().First());
                var keys = server.Keys(pattern: pattern);
                
                var keyArray = keys.ToArray();
                if (keyArray.Length > 0)
                {
                    await _database.KeyDeleteAsync(keyArray);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing values by pattern from Redis cache for pattern: {Pattern}", pattern);
            }
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _database.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence in Redis cache for key: {Key}", key);
                return false;
            }
        }

        public async Task<long> IncrementAsync(string key, long value = 1, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _database.StringIncrementAsync(key, value);
                
                if (expiration.HasValue)
                {
                    await _database.KeyExpireAsync(key, expiration);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing value in Redis cache for key: {Key}", key);
                return 0;
            }
        }
    }

    // Hybrid cache service that uses both memory and Redis
    public class HybridCacheService : ICacheService
    {
        private readonly ICacheService _memoryCache;
        private readonly ICacheService _redisCache;
        private readonly ILogger<HybridCacheService> _logger;
        
        private readonly TimeSpan _memoryTtl = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _redisTtl = TimeSpan.FromHours(1);

        public HybridCacheService(
            MemoryCacheService memoryCache, 
            RedisCacheService redisCache,
            ILogger<HybridCacheService> logger)
        {
            _memoryCache = memoryCache;
            _redisCache = redisCache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            // Try memory first
            var memoryValue = await _memoryCache.GetAsync<T>(key, cancellationToken);
            if (memoryValue != null)
            {
                return memoryValue;
            }

            // Try Redis
            var redisValue = await _redisCache.GetAsync<T>(key, cancellationToken);
            if (redisValue != null)
            {
                // Populate memory cache
                await _memoryCache.SetAsync(key, redisValue, _memoryTtl, cancellationToken);
                return redisValue;
            }

            return default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            // Set in both caches
            await _memoryCache.SetAsync(key, value, expiration ?? _memoryTtl, cancellationToken);
            await _redisCache.SetAsync(key, value, expiration ?? _redisTtl, cancellationToken);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _memoryCache.RemoveAsync(key, cancellationToken);
            await _redisCache.RemoveAsync(key, cancellationToken);
        }

        public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            await _memoryCache.RemoveByPatternAsync(pattern, cancellationToken);
            await _redisCache.RemoveByPatternAsync(pattern, cancellationToken);
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            var memoryExists = await _memoryCache.ExistsAsync(key, cancellationToken);
            if (memoryExists) return true;

            return await _redisCache.ExistsAsync(key, cancellationToken);
        }

        public async Task<long> IncrementAsync(string key, long value = 1, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            // Use Redis for atomic operations
            return await _redisCache.IncrementAsync(key, value, expiration, cancellationToken);
        }
    }
}
