using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

namespace Khoan.Api.Middleware
{
    public class PerformanceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceMiddleware> _logger;

        public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString();

            // Add request ID to headers only if response hasn't started
            if (!context.Response.HasStarted)
            {
                context.Response.Headers["X-Request-ID"] = requestId;
                context.Response.Headers["X-API-Version"] = "1.0";
            }

            // Log request start
            _logger.LogInformation("🚀 Request Started - ID: {RequestId}, Method: {Method}, Path: {Path}, User: {User}",
                requestId,
                context.Request.Method,
                context.Request.Path,
                context.User?.Identity?.Name ?? "Anonymous"
            );

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var elapsed = stopwatch.ElapsedMilliseconds;

                // Add performance headers only if response hasn't started
                if (!context.Response.HasStarted)
                {
                    context.Response.Headers["X-Response-Time"] = $"{elapsed}ms";
                }

                // Log request completion with performance metrics
                var logLevel = elapsed switch
                {
                    < 100 => LogLevel.Information,
                    < 500 => LogLevel.Warning,
                    _ => LogLevel.Error
                };

                _logger.Log(logLevel,
                    "✅ Request Completed - ID: {RequestId}, Status: {StatusCode}, Duration: {Duration}ms, Size: {Size} bytes",
                    requestId,
                    context.Response.StatusCode,
                    elapsed,
                    context.Response.ContentLength ?? 0
                );

                // Log slow requests
                if (elapsed > 1000)
                {
                    _logger.LogWarning("🐌 Slow Request Detected - ID: {RequestId}, Duration: {Duration}ms", 
                        requestId, elapsed);
                }
            }
        }
    }

    public class CacheControlMiddleware
    {
        private readonly RequestDelegate _next;

        public CacheControlMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            // Set cache headers based on endpoint only if response hasn't started
            if (!context.Response.HasStarted)
            {
                var path = context.Request.Path.Value?.ToLower();
                
                if (path?.Contains("/temporal/stats") == true)
                {
                    context.Response.Headers["Cache-Control"] = "public, max-age=300"; // 5 minutes
                }
                else if (path?.Contains("/temporal/query") == true)
                {
                    context.Response.Headers["Cache-Control"] = "public, max-age=180"; // 3 minutes
                }
                else if (path?.Contains("/temporal/") == true)
                {
                    context.Response.Headers["Cache-Control"] = "public, max-age=900"; // 15 minutes
                }
                else
                {
                    context.Response.Headers["Cache-Control"] = "no-cache";
                }

                // Add ETag for cache validation
                if (context.Response.StatusCode == 200 && context.Response.ContentLength > 0)
                {
                    var etag = $"\"{context.Response.ContentLength}_{DateTime.UtcNow.Ticks}\"";
                    context.Response.Headers["ETag"] = etag;
                }
            }
        }
    }

    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Add security headers only if response hasn't started
            if (!context.Response.HasStarted)
            {
                context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                context.Response.Headers["X-Frame-Options"] = "DENY";
                context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
                context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
                context.Response.Headers["Content-Security-Policy"] = 
                    "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'";
            }

            await _next(context);
        }
    }

    public static class CompressionExtensions
    {
        public static IServiceCollection AddAdvancedCompression(this IServiceCollection services)
        {
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    "application/json",
                    "application/javascript",
                    "text/css",
                    "text/html",
                    "text/json",
                    "text/plain"
                });
            });

            return services;
        }
    }

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UsePerformanceMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PerformanceMiddleware>();
        }

        public static IApplicationBuilder UseCacheControlMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CacheControlMiddleware>();
        }

        public static IApplicationBuilder UseSecurityHeadersMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }
}
