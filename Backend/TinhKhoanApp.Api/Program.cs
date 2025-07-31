using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Services;
using TinhKhoanApp.Api.Services.Interfaces;
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

// Repository Pattern & Service Layer
builder.Services.AddRepositories();
builder.Services.AddApplicationServices(); // From DependencyInjectionExtensions// Cache Services
builder.Services.AddCachingServices(builder.Configuration);

// Essential Services
// builder.Services.AddScoped<IDirectImportService, DirectImportService>();
// builder.Services.AddScoped<IEmployeeService, EmployeeService>();
// builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

// File Upload Configuration
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1073741824; // 1GB for large GL01 files
    options.ValueLengthLimit = int.MaxValue;
    options.ValueCountLimit = int.MaxValue;
});

// Kestrel Server Configuration for large files
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 1073741824; // 1GB
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1073741824; // 1GB
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(5);
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(30);
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
