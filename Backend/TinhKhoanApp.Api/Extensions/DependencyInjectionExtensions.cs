using TinhKhoanApp.Api.Repositories;
using TinhKhoanApp.Api.Services.DataServices;

namespace TinhKhoanApp.Api.Extensions
{
    /// <summary>
    /// Extension methods to register all repositories and services
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Register all repositories
        /// </summary>
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Register generic repository
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Register specific repositories
            services.AddScoped<IDP01Repository, DP01Repository>();
            services.AddScoped<IGL01Repository, GL01Repository>();
            services.AddScoped<IGL02Repository, GL02Repository>();
            services.AddScoped<IDPDARepository, DPDARepository>();
            services.AddScoped<IEI01Repository, EI01Repository>();

            // TODO: Add other repositories as needed
            // services.AddScoped<ILN01Repository, LN01Repository>();

            return services;
        }

        /// <summary>
        /// Register all services
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register data services
            services.AddScoped<IDataPreviewService, DataPreviewService>();
            services.AddScoped<IGL01DataService, GL01DataService>();
            services.AddScoped<IGL02DataService, GL02DataService>();
            services.AddScoped<IDPDADataService, DPDADataService>();
            services.AddScoped<IEI01DataService, EI01DataService>();

            // TODO: Add other services as needed

            return services;
        }
    }
}
