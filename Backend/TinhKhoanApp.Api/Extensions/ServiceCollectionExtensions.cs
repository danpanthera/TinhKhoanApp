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
            services.AddScoped<Services.DataServices.IDP01DataService, Services.DataServices.DP01DataService>();
            services.AddScoped<Services.DataServices.IDPDADataService, Services.DataServices.DPDADataService>();
            services.AddScoped<Services.DataServices.IEI01DataService, Services.DataServices.EI01DataService>();
            services.AddScoped<Services.DataServices.IGL01DataService, Services.DataServices.GL01DataService>();
            services.AddScoped<Services.DataServices.IGL02DataService, Services.DataServices.GL02DataService>();
            services.AddScoped<Services.DataServices.IGL41DataService, Services.DataServices.GL41DataService>();
            services.AddScoped<Services.ILN01Service, Services.LN01Service>();
            services.AddScoped<Services.ILN03Service, Services.LN03Service>();
            services.AddScoped<Services.IRR01Service, Services.RR01Service>();

            // Import Service
            services.AddScoped<DirectImportService>();

            // Data Services
            services.AddScoped<Services.DataServices.DP01DataService>();
            services.AddScoped<Services.DataServices.DPDADataService>();
            services.AddScoped<Services.DataServices.EI01DataService>();
            services.AddScoped<Services.DataServices.GL01DataService>();
            services.AddScoped<Services.DataServices.GL02DataService>();
            services.AddScoped<Services.DataServices.GL41DataService>();
            services.AddScoped<Services.DirectImportService>();

            return services;
        }
    }
}
