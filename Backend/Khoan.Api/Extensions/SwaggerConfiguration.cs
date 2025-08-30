using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace TinhKhoanApp.Api.Extensions
{
    /// <summary>
    /// Cấu hình Swagger cho API Documentation
    /// </summary>
    public static class SwaggerConfiguration
    {
        /// <summary>
        /// Thêm cấu hình Swagger/OpenAPI với JWT Authentication
        /// </summary>
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "TinhKhoan API",
                    Version = "v1",
                    Description = "REST API cho TinhKhoan App - Quản lý dữ liệu tài chính ngân hàng",
                    Contact = new OpenApiContact
                    {
                        Name = "Agribank IT Team",
                        Email = "it@agribank.com.vn"
                    }
                });

                // Cấu hình xác thực JWT
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // Thêm XML Comments để hiển thị mô tả API
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            return services;
        }

        /// <summary>
        /// Cấu hình Swagger UI Middleware
        /// </summary>
        public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "TinhKhoan API v1");
                options.RoutePrefix = "api-docs";
                options.DocumentTitle = "TinhKhoan API Documentation";
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                options.DefaultModelsExpandDepth(-1); // Hide schemas section
            });

            return app;
        }
    }
}
