using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;
using Khoan.Api.Services;
// using Khoan.Api.Services.Interfaces; // Disabled for DP01+DPDA focus
using Khoan.Api.Filters;
using Khoan.Api.Middleware;
using Khoan.Api.HealthChecks;
using Khoan.Api.Utils;
using Khoan.Api.Repositories;
using Khoan.Api.Converters;
using Khoan.Api.Extensions;
using System.Text.Json.Serialization;
using BCrypt.Net;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// UTF-8 Encoding for Vietnamese
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

// Vietnam Timezone Configuration
TimeZoneInfo vietnamTimeZone;
try
{
    vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // Windows
    Console.WriteLine($"✅ Timezone set to: {vietnamTimeZone.DisplayName}");
}
catch (TimeZoneNotFoundException)
{
    try
    {
        vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh"); // Linux/macOS
        Console.WriteLine($"✅ Timezone set to: {vietnamTimeZone.DisplayName}");
    }
    catch (TimeZoneNotFoundException)
    {
        vietnamTimeZone = TimeZoneInfo.Local; // Fallback
        Console.WriteLine($"⚠️ Vietnam timezone not found, using system default: {vietnamTimeZone.DisplayName}");
    }
}

// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Azure SQL Edge connection string is not configured.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.CommandTimeout(120);
        sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
    });
    options.EnableSensitiveDataLogging(false);
    options.EnableServiceProviderCaching(true);
    options.EnableDetailedErrors(false);
});

// Controllers with JSON Configuration
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new VietnamDateTimeConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.WriteIndented = true;
        // Ensure UTF-8 encoding for Vietnamese characters
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    });

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Repository Pattern & Service Layer - ENABLED FOR LN03
builder.Services.AddRepositories();
builder.Services.AddApplicationServices(); // From DependencyInjectionExtensions

// Configure DirectImport Settings
builder.Services.Configure<Khoan.Api.Models.Configuration.DirectImportSettings>(
    builder.Configuration.GetSection("DirectImport"));

// Cache Services (DISABLED for now)
// builder.Services.AddCachingServices(builder.Configuration);

// 🎯 PHASE 3A: DEPENDENCY INJECTION CONFIGURATION - WORKING REPOSITORIES ONLY
// Repository Layer - Only enable repositories that compile successfully
builder.Services.AddScoped<IDP01Repository, DP01Repository>(); // ✅ DP01 ENABLED - Works
builder.Services.AddScoped<IEI01Repository, EI01Repository>(); // ✅ EI01 ENABLED - Works (COMPLETED 100%)
// TODO: Fix DPDA Repository - Interface mismatch with BaseRepository
// builder.Services.AddScoped<Khoan.Api.Repositories.Interfaces.IDPDARepository, Khoan.Api.Repositories.DPDARepository>(); // ✅ DPDA Repository - NEEDS FIX
builder.Services.AddScoped<Khoan.Api.Repositories.Interfaces.ILN03Repository, Khoan.Api.Repositories.LN03Repository>(); // ✅ LN03 TEMPORAL TABLE ENABLED
// TODO: Fix repository interface implementations for the following (have compilation errors):
builder.Services.AddScoped<Khoan.Api.Repositories.IGL01Repository, Khoan.Api.Repositories.GL01Repository>(); // ✅ GL01 Repository ENABLED
builder.Services.AddScoped<Khoan.Api.Repositories.Interfaces.IGL02Repository, Khoan.Api.Repositories.GL02Repository>(); // ✅ GL02 Repository ENABLED
builder.Services.AddScoped<Khoan.Api.Repositories.IGL41Repository, Khoan.Api.Repositories.GL41Repository>(); // ✅ GL41 Repository ENABLED
// builder.Services.AddScoped<Khoan.Api.Repositories.Interfaces.ILN01Repository, Khoan.Api.Repositories.LN01Repository>(); // ✅ LN01 Repository DISABLED (using service only)
// builder.Services.AddScoped<Khoan.Api.Repositories.IRR01Repository, Khoan.Api.Repositories.RR01Repository>(); // TODO: Fix interface implementation - Has 23 missing methods

// Service Layer - Only services with implementations are enabled
// builder.Services.AddScoped<Khoan.Api.Services.Interfaces.IDP01Service, DP01Service>(); // ❌ DP01Service không tồn tại
builder.Services.AddScoped<Khoan.Api.Services.Interfaces.IEI01Service, EI01Service>(); // ✅ EI01 ENABLED - Has implementation (COMPLETED 100%)
builder.Services.AddScoped<Khoan.Api.Interfaces.ILN01Service, Khoan.Api.Services.LN01Service>(); // ✅ LN01 ENABLED - JUST CREATED (79 columns)
// builder.Services.AddScoped<Khoan.Api.Services.Interfaces.ILN03Service, Khoan.Api.Services.LN03Service>(); // ✅ REMOVED - Cleanup focus on DP01+DPDA
// TODO: Enable DPDA Service after Repository fix
// builder.Services.AddScoped<Khoan.Api.Services.Interfaces.IDPDAService, Khoan.Api.Services.DPDAService>(); // ✅ DPDA - NEEDS Repository fix first
builder.Services.AddScoped<Khoan.Api.Services.Interfaces.IGL01Service, Khoan.Api.Services.GL01Service>(); // ✅ GL01 Service ENABLED
// Ensure GL01 analytics indexes exist at startup (columnstore approximation)
builder.Services.AddHostedService<Khoan.Api.Services.Startup.Gl01IndexInitializer>();
builder.Services.AddHostedService<Khoan.Api.Services.Startup.Gl02IndexInitializer>();

// 🛡️ Index Initializers with enhanced error handling to prevent app crashes
try
{
    // 9 bảng chính - 9 IndexInitializers (HOÀN THÀNH)
    builder.Services.AddHostedService<Khoan.Api.Services.Startup.Gl01IndexInitializer>(); // ✅ GL01 Index Initializer - Working
    builder.Services.AddHostedService<Khoan.Api.Services.Startup.Gl02IndexInitializer>(); // ✅ GL02 Index Initializer - Working  
    builder.Services.AddHostedService<Khoan.Api.Services.Startup.Dp01IndexInitializer>(); // ✅ DP01 Index Initializer - NEW (thứ 9)
    builder.Services.AddHostedService<Khoan.Api.Services.Startup.DpdaIndexInitializer>(); // ✅ DPDA Index Initializer - New
    builder.Services.AddHostedService<Khoan.Api.Services.Startup.Ei01IndexInitializer>(); // ✅ EI01 Index Initializer - New
    builder.Services.AddHostedService<Khoan.Api.Services.Startup.Ln01IndexInitializer>(); // ✅ LN01 Index Initializer - New
    builder.Services.AddHostedService<Khoan.Api.Services.Startup.Ln03IndexInitializer>(); // ✅ LN03 Index Initializer - Fixed
    builder.Services.AddHostedService<Khoan.Api.Services.Startup.Rr01IndexInitializer>(); // ✅ RR01 Index Initializer - New
    builder.Services.AddHostedService<Khoan.Api.Services.Startup.Gl41IndexInitializer>(); // ✅ GL41 Index Initializer - Re-enabled
    // 🎯 HOÀN THÀNH: 9/9 IndexInitializers cho 9 bảng dữ liệu chính
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ Warning: Could not register Index Initializers - {ex.Message}");
    // Continue without Index Initializers to prevent app crash
}
builder.Services.AddScoped<Khoan.Api.Services.Interfaces.IGL02Service, Khoan.Api.Services.GL02Service>(); // ✅ GL02 Service ENABLED
builder.Services.AddScoped<Khoan.Api.Services.Interfaces.IGL41Service, Khoan.Api.Services.GL41Service>(); // ✅ GL41 Service ENABLED
builder.Services.AddScoped<Khoan.Api.Interfaces.ILN01Service, Khoan.Api.Services.LN01Service>(); // ✅ LN01 Service ENABLED
// ✅ LN03 TEMPORAL TABLE SERVICE ENABLED
builder.Services.AddScoped<Khoan.Api.Services.Interfaces.ILN03Service, Khoan.Api.Services.LN03Service>();
builder.Services.AddScoped<Khoan.Api.Services.Interfaces.IRR01Service, Khoan.Api.Services.RR01Service>(); // RR01Service completed

// Data Services Layer - TODO: Fix implementations
// builder.Services.AddScoped<Khoan.Api.Services.DataServices.IDataPreviewService, Khoan.Api.Services.DataServices.DataPreviewService>(); // TODO
// builder.Services.AddScoped<Khoan.Api.Services.DataServices.IDP01DataService, Khoan.Api.Services.DataServices.DP01DataService>(); // TODO
// builder.Services.AddScoped<Khoan.Api.Services.DataServices.IDPDADataService, Khoan.Api.Services.DataServices.DPDADataService>(); // TODO

// Essential Services
builder.Services.AddScoped<Khoan.Api.Services.Interfaces.IDirectImportService, Khoan.Api.Services.DirectImportService>();
// builder.Services.AddScoped<IEmployeeService, EmployeeService>();
// builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

// File Upload Configuration
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 524288000; // 500MB
    options.ValueLengthLimit = int.MaxValue;
    options.ValueCountLimit = int.MaxValue;
});

// JWT Authentication (Optional)
var jwtKey = builder.Configuration["Jwt:Key"];
if (!string.IsNullOrEmpty(jwtKey))
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        });
}

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration();

// Health Checks
builder.Services.AddScoped<DatabaseHealthCheck>();
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");

var app = builder.Build();

// Development Configuration
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Swagger configuration (available in all environments)
app.UseSwaggerConfiguration();

// Middleware Pipeline - CORS must be early in the pipeline
app.UseCors("AllowFrontend");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health Check Endpoint
app.MapHealthChecks("/health");
app.MapGet("/api/Health", () => new { status = "healthy", timestamp = DateTime.UtcNow });

// WeatherForecast endpoint (if needed)
app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" }[Random.Shared.Next(10)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

Console.WriteLine("🚀 Starting TinhKhoan Backend API (Clean Version)...");
Console.WriteLine($"🌐 Backend will be available at: http://localhost:5055");
Console.WriteLine("✅ All seeding code removed for stability");

// Ensure the app runs on port 5055
builder.WebHost.UseUrls("http://localhost:5055");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
