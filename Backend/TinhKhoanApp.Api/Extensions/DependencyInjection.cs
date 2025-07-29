using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // For IConfiguration
using Microsoft.Extensions.DependencyInjection; // For IServiceCollection
using System;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Repositories;
using TinhKhoanApp.Api.Repositories.Cached;
using TinhKhoanApp.Api.Services.DataServices;
using Scrutor; // For the Decorate extension method

namespace TinhKhoanApp.Api.Extensions
{
    /// <summary>
    /// Extension methods for dependency injection configuration
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Add application services to the service collection
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
            });

            // Register repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IDP01Repository, DP01Repository>();
            services.AddScoped<IGL41Repository, GL41Repository>();

            // Register cached repositories (for production environments)
            if (!configuration.GetValue<bool>("UseMemoryDatabase", false))
            {
                // Use Scrutor extension method to decorate repositories with cache
                services.Decorate<IGL41Repository, CachedGL41Repository>();
            }

            // Register data services
            services.AddScoped<IDataPreviewService, DataPreviewService>();
            services.AddScoped<IGL41DataService, GL41DataService>();
            services.AddScoped<IDP01DataService, DP01DataService>();

            return services;
        }
    }
}
