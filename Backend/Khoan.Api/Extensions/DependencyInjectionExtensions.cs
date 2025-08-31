using Khoan.Api.Repositories;
using Khoan.Api.Repositories.Interfaces;
using Khoan.Api.Services;
using Khoan.Api.Services.Interfaces;

namespace Khoan.Api.Extensions
{
    /// <summary>
    /// 🚀 Dependency Injection Extensions for TinhKhoan API
    /// Registers all repositories and services for dependency injection.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// 📦 Register all repositories
        /// </summary>
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Only register existing repositories for now
            services.AddScoped<ILN03Repository, LN03Repository>();
            
            return services;
        }

        /// <summary>
        /// 🎯 Register all application services
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Only register existing services for now
            services.AddScoped<ILN03Service, LN03Service>();
            
            return services;
        }

        /// <summary>
        /// 🌐 Add caching services (Memory Cache only for now)
        /// </summary>
        public static IServiceCollection AddCachingServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Memory Cache (always available)
            services.AddMemoryCache();

            return services;
        }
    }
}
