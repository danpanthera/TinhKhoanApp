using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Repositories;
using TinhKhoanApp.Api.Services;
using TinhKhoanApp.Api.Services.DataServices;

namespace TinhKhoanApp.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Đăng ký database context
        /// </summary>
        public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                        sqlOptions.CommandTimeout(60);
                    }));

            return services;
        }

        /// <summary>
        /// Đăng ký các services của application
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped<IDP01Repository, DP01Repository>();
            services.AddScoped<IDPDARepository, DPDARepository>();
            services.AddScoped<IEI01Repository, EI01Repository>();
            services.AddScoped<IGL01Repository, GL01Repository>();
            services.AddScoped<IGL02Repository, GL02Repository>();
            services.AddScoped<IGL41Repository, GL41Repository>();
            services.AddScoped<ILN01Repository, LN01Repository>();
            services.AddScoped<ILN03Repository, LN03Repository>();
            services.AddScoped<IRR01Repository, RR01Repository>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Services
            services.AddScoped<IDP01Service, DP01Service>();
            services.AddScoped<IDPDAService, DPDAService>();
            services.AddScoped<IEI01Service, EI01Service>();
            services.AddScoped<IGL01Service, GL01Service>();
            services.AddScoped<IGL02Service, GL02Service>();
            services.AddScoped<IGL41Service, GL41Service>();
            services.AddScoped<ILN01Service, LN01Service>();
            services.AddScoped<ILN03Service, LN03Service>();
            services.AddScoped<IRR01Service, RR01Service>();

            // Import Service
            services.AddScoped<DirectImportService>();

            // Data Services
            services.AddScoped<DP01DataService>();
            services.AddScoped<DPDADataService>();
            services.AddScoped<EI01DataService>();
            services.AddScoped<GL01DataService>();
            services.AddScoped<GL02DataService>();
            services.AddScoped<GL41DataService>();
            services.AddScoped<LN01DataService>();
            services.AddScoped<LN03DataService>();
            services.AddScoped<RR01DataService>();

            return services;
        }
    }
}
