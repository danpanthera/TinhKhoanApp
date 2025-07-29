using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using TinhKhoanApp.Api.Services.Caching;

namespace TinhKhoanApp.Api.Extensions
{
    /// <summary>
    /// Extension methods để cấu hình cache
    /// </summary>
    public static class CacheExtensions
    {
        /// <summary>
        /// Đăng ký các dịch vụ cache cho ứng dụng
        /// </summary>
        public static IServiceCollection AddCachingServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register Memory Cache
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();

            // Redis cache configuration (if available)
            var redisConnection = configuration.GetConnectionString("RedisConnection");
            if (!string.IsNullOrEmpty(redisConnection))
            {
                // Redis implementation is commented out because the package is not available
                // To use Redis, add this NuGet package: Microsoft.Extensions.Caching.StackExchangeRedis
                // and uncomment the following code:

                // services.AddStackExchangeRedisCache(options =>
                // {
                //     options.Configuration = redisConnection;
                //     options.InstanceName = "TinhKhoanApp:";
                // });

                // Redis-based cache service could be implemented and registered here
            }

            return services;
        }
    }
}
