using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // For IConfiguration
using Microsoft.Extensions.DependencyInjection; // For IServiceCollection
using System;
using Khoan.Api.Data;
using Khoan.Api.Repositories;
// using Khoan.Api.Repositories.Cached; // TEMP DISABLED
// using Khoan.Api.Services.DataServices; // TEMP DISABLED
using Scrutor; // For the Decorate extension method

namespace Khoan.Api.Extensions
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
            services.AddScoped(typeof(Khoan.Api.Repositories.Interfaces.IBaseRepository<>), typeof(Khoan.Api.Repositories.GenericRepository<>));
            // All specific repositories are temporarily disabled for clean build

            // Register cached repositories (for production environments) - TEMP DISABLED
            // if (!configuration.GetValue<bool>("UseMemoryDatabase", false))
            // {
            //     // Use Scrutor extension method to decorate repositories with cache
            // }

            // Register data services - TEMP DISABLED
            // All specific services are temporarily disabled for clean build

            return services;
        }
    }
}
