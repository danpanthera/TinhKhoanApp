using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Services;
// using TinhKhoanApp.Api.Services.Interfaces; // Disabled for DP01+DPDA focus
using TinhKhoanApp.Api.Filters;
using TinhKhoanApp.Api.Middleware;
using TinhKhoanApp.Api.HealthChecks;
using TinhKhoanApp.Api.Utils;
using TinhKhoanApp.Api.Repositories;
using TinhKhoanApp.Api.Converters;
using TinhKhoanApp.Api.Extensions;
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

// Repository Pattern & Service Layer - TEMP DISABLED FOR CLEAN BUILD
// builder.Services.AddRepositories();
// builder.Services.AddApplicationServices(); // From DependencyInjectionExtensions

// Configure DirectImport Settings
builder.Services.Configure<TinhKhoanApp.Api.Models.Configuration.DirectImportSettings>(
    builder.Configuration.GetSection("DirectImport"));

// Cache Services (DISABLED for now)
// builder.Services.AddCachingServices(builder.Configuration);

// 🎯 PHASE 3A: DEPENDENCY INJECTION CONFIGURATION - WORKING REPOSITORIES ONLY
// Repository Layer - Only enable repositories that compile successfully
builder.Services.AddScoped<IDP01Repository, DP01Repository>(); // ✅ DP01 ENABLED - Works
builder.Services.AddScoped<IEI01Repository, EI01Repository>(); // ✅ EI01 ENABLED - Works (COMPLETED 100%)
// TODO: Fix DPDA Repository - Interface mismatch with BaseRepository
// builder.Services.AddScoped<TinhKhoanApp.Api.Repositories.Interfaces.IDPDARepository, TinhKhoanApp.Api.Repositories.DPDARepository>(); // ✅ DPDA Repository - NEEDS FIX
// builder.Services.AddScoped<TinhKhoanApp.Api.Repositories.Interfaces.ILN03Repository, TinhKhoanApp.Api.Repositories.LN03Repository>(); // ✅ REMOVED - Cleanup focus on DP01+DPDA
// TODO: Fix repository interface implementations for the following (have compilation errors):
builder.Services.AddScoped<TinhKhoanApp.Api.Repositories.IGL01Repository, TinhKhoanApp.Api.Repositories.GL01Repository>(); // ✅ GL01 Repository ENABLED
builder.Services.AddScoped<TinhKhoanApp.Api.Repositories.IGL02Repository, TinhKhoanApp.Api.Repositories.GL02Repository>(); // ✅ GL02 Repository ENABLED
builder.Services.AddScoped<TinhKhoanApp.Api.Repositories.IGL41Repository, TinhKhoanApp.Api.Repositories.GL41Repository>(); // ✅ GL41 Repository ENABLED
builder.Services.AddScoped<TinhKhoanApp.Api.Repositories.ILN01Repository, TinhKhoanApp.Api.Repositories.LN01Repository>(); // ✅ LN01 Repository ENABLED
// builder.Services.AddScoped<TinhKhoanApp.Api.Repositories.IRR01Repository, TinhKhoanApp.Api.Repositories.RR01Repository>(); // TODO: Fix interface implementation - Has 23 missing methods

// Service Layer - Only services with implementations are enabled
builder.Services.AddScoped<TinhKhoanApp.Api.Services.Interfaces.IDP01Service, DP01Service>(); // ✅ DP01 ENABLED - Has implementation
builder.Services.AddScoped<TinhKhoanApp.Api.Services.Interfaces.IEI01Service, EI01Service>(); // ✅ EI01 ENABLED - Has implementation (COMPLETED 100%)
// builder.Services.AddScoped<TinhKhoanApp.Api.Services.Interfaces.ILN03Service, TinhKhoanApp.Api.Services.LN03Service>(); // ✅ REMOVED - Cleanup focus on DP01+DPDA
// TODO: Enable DPDA Service after Repository fix
// builder.Services.AddScoped<TinhKhoanApp.Api.Services.Interfaces.IDPDAService, TinhKhoanApp.Api.Services.DPDAService>(); // ✅ DPDA - NEEDS Repository fix first
builder.Services.AddScoped<TinhKhoanApp.Api.Services.Interfaces.IGL01Service, TinhKhoanApp.Api.Services.GL01Service>(); // ✅ GL01 Service ENABLED
// Ensure GL01 analytics indexes exist at startup (columnstore approximation)
builder.Services.AddHostedService<TinhKhoanApp.Api.Services.Startup.Gl01IndexInitializer>();
builder.Services.AddHostedService<TinhKhoanApp.Api.Services.Startup.Gl02IndexInitializer>();

// 🛡️ Index Initializers with enhanced error handling to prevent app crashes
try
{
    builder.Services.AddHostedService<TinhKhoanApp.Api.Services.Startup.Gl41IndexInitializer>(); // ✅ GL41 Index Initializer
    builder.Services.AddHostedService<TinhKhoanApp.Api.Services.Startup.Ln03IndexInitializer>(); // ✅ LN03 Index Initializer
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ Warning: Could not register Index Initializers - {ex.Message}");
    // Continue without Index Initializers to prevent app crash
}
builder.Services.AddScoped<TinhKhoanApp.Api.Services.Interfaces.IGL02Service, TinhKhoanApp.Api.Services.GL02Service>(); // ✅ GL02 Service ENABLED
builder.Services.AddScoped<TinhKhoanApp.Api.Services.Interfaces.IGL41Service, TinhKhoanApp.Api.Services.GL41Service>(); // ✅ GL41 Service ENABLED
builder.Services.AddScoped<TinhKhoanApp.Api.Services.Interfaces.ILN01Service, TinhKhoanApp.Api.Services.LN01Service>(); // ✅ LN01 Service ENABLED
// 🆕 LN03 wiring (CSV-first DataTables model)
builder.Services.AddScoped<TinhKhoanApp.Api.Repositories.ILN03DataRepository, TinhKhoanApp.Api.Repositories.LN03Repository>();
builder.Services.AddScoped<TinhKhoanApp.Api.Services.Interfaces.ILN03Service, TinhKhoanApp.Api.Services.LN03Service>();
builder.Services.AddScoped<TinhKhoanApp.Api.Services.Interfaces.IRR01Service, TinhKhoanApp.Api.Services.RR01Service>(); // RR01Service completed

// Data Services Layer - TODO: Fix implementations
// builder.Services.AddScoped<TinhKhoanApp.Api.Services.DataServices.IDataPreviewService, TinhKhoanApp.Api.Services.DataServices.DataPreviewService>(); // TODO
// builder.Services.AddScoped<TinhKhoanApp.Api.Services.DataServices.IDP01DataService, TinhKhoanApp.Api.Services.DataServices.DP01DataService>(); // TODO
// builder.Services.AddScoped<TinhKhoanApp.Api.Services.DataServices.IDPDADataService, TinhKhoanApp.Api.Services.DataServices.DPDADataService>(); // TODO

// Essential Services
builder.Services.AddScoped<TinhKhoanApp.Api.Services.Interfaces.IDirectImportService, TinhKhoanApp.Api.Services.DirectImportService>();
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

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
