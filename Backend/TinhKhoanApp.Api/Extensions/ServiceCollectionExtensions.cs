            // Đăng ký các Repository
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IEI01Repository, EI01Repository>();
            services.AddScoped<IGL02Repository, GL02Repository>();
            services.AddScoped<ILN01Repository, LN01Repository>();
            services.AddScoped<ILN03Repository, LN03Repository>();
            services.AddScoped<IRR01Repository, RR01Repository>();

            // Đăng ký các Service
            services.AddScoped<IEI01Service, EI01Service>();
            services.AddScoped<IGL02Service, GL02Service>();
            services.AddScoped<ILN01Service, LN01Service>();
            services.AddScoped<ILN03Service, LN03Service>();
            services.AddScoped<IRR01Service, RR01Service>();oft.E            // Đăng ký các Repository
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IEI01Repository, EI01Repository>();
            services.AddScoped<IGL02Repository, GL02Repository>();
            services.AddScoped<ILN01Repository, LN01Repository>();
            services.AddScoped<ILN03Repository, LN03Repository>();

            // Đăng ký các Service
            services.AddScoped<IEI01Service, EI01Service>();
            services.AddScoped<IGL02Service, GL02Service>();
            services.AddScoped<ILN01Service, LN01Service>();
            services.AddScoped<ILN03Service, LN03Service>();DependencyInjection;
using Microsoft.Extensions.Configuration;
using TinhKhoanApp.Api.Repositories;
using TinhKhoanApp.Api.Services;

namespace TinhKhoanApp.Api.Extensions
{
    /// <summary>
    /// Các phương thức mở rộng để đăng ký dịch vụ vào DI Container
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Đăng ký tất cả các dịch vụ cho ứng dụng
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Đăng ký các Repository
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IEI01Repository, EI01Repository>();
            services.AddScoped<IGL02Repository, GL02Repository>();
            services.AddScoped<ILN01Repository, LN01Repository>();

            // Đăng ký các Service
            services.AddScoped<IEI01Service, EI01Service>();
            services.AddScoped<IGL02Service, GL02Service>();
            services.AddScoped<ILN01Service, LN01Service>();

            return services;
        }

        /// <summary>
        /// Đăng ký các dịch vụ caching
        /// </summary>
        public static IServiceCollection AddCachingServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Đăng ký Memory Cache
            services.AddMemoryCache();

            // Đăng ký Distributed Cache nếu cần
            if (configuration.GetValue<bool>("UseDistributedCache"))
            {
                services.AddDistributedMemoryCache();
            }

            return services;
        }
    }
}
