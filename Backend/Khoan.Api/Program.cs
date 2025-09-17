using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
// Legacy namespaces (linked temporarily from Khoan.Api)
using Khoan.Api.Data;
using Khoan.Api.Services;
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
using Microsoft.Data.SqlClient; // For server-level attach without sqlcmd
using System.Diagnostics; // Stopwatch for startup timing
using Khoan.Api.Models.Configuration;

var builder = WebApplication.CreateBuilder(args);

// UTF-8 Encoding for Vietnamese
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

// Vietnam Timezone Configuration (always set regardless of DB attach logic)
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

// Common DB connection string (always needed for DbContext)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Azure SQL Edge connection string is not configured.");
}

var csbNormalized = new SqlConnectionStringBuilder(connectionString)
{
    ConnectRetryCount = 1,
    ConnectRetryInterval = 2,
    ConnectTimeout = Math.Min(30, new SqlConnectionStringBuilder(connectionString).ConnectTimeout > 0 ? new SqlConnectionStringBuilder(connectionString).ConnectTimeout : 30)
};

// Allow skipping heavy attach/repair logic via env var (export SKIP_DB_ATTACH=true)
// In Development, default to skipping heavy DB attach/repair unless explicitly forced.
var isDevelopment = builder.Environment.IsDevelopment();
var forceAttach = Environment.GetEnvironmentVariable("FORCE_DB_ATTACH")?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;
bool? skipAttachEnv = Environment.GetEnvironmentVariable("SKIP_DB_ATTACH")?.Equals("true", StringComparison.OrdinalIgnoreCase);
// Priority: FORCE_DB_ATTACH=true overrides everything; then explicit SKIP_DB_ATTACH; otherwise default skip in Development
var skipAttach = forceAttach ? false : (skipAttachEnv ?? (isDevelopment ? true : false));
var startupSw = Stopwatch.StartNew();
Console.WriteLine(skipAttach
    ? "⏭️  SKIP_DB_ATTACH=true => Bỏ qua logic ATTACH/REPAIR nặng, dùng connection string trực tiếp."
    : "🧪 Bắt đầu kiểm tra/attach database (có thể mất vài giây nếu cần sửa file)...");

// If skipping attach, further reduce connection retries/timeouts so startup never hangs
if (skipAttach)
{
    csbNormalized.ConnectRetryCount = 0;
    // Fail fast when DB isn't ready in dev
    csbNormalized.ConnectTimeout = Math.Min(5, csbNormalized.ConnectTimeout);
}

if (!skipAttach)
{
try
{
    var masterCsb = new SqlConnectionStringBuilder(csbNormalized.ConnectionString)
    {
        InitialCatalog = "master",
        ConnectTimeout = 30
    };

    using var masterConn = new SqlConnection(masterCsb.ConnectionString);
    masterConn.Open();

    // Ensure SA default DB does not cause login stalls
    using (var alterLogin = new SqlCommand("IF SUSER_SNAME(0x01) = 'sa' ALTER LOGIN [sa] WITH DEFAULT_DATABASE = [master];", masterConn))
    {
        await alterLogin.ExecuteNonQueryAsync();
    }

    using (var checkCmd = new SqlCommand("SELECT DB_ID(@db)", masterConn))
    {
        checkCmd.Parameters.AddWithValue("@db", "TinhKhoanDB");
        var exists = await checkCmd.ExecuteScalarAsync() is not DBNull and not null;
        if (!exists)
        {
            Console.WriteLine("⚠️ Database 'TinhKhoanDB' không tồn tại. Tiến hành ATTACH từ file .mdf/.ldf...");
            // Attempt attach from default SQL Edge data directory
            var attachSql = @"
IF DB_ID('TinhKhoanDB') IS NULL
BEGIN
    CREATE DATABASE [TinhKhoanDB] ON 
    (FILENAME = '/var/opt/mssql/data/TinhKhoanDB.mdf'),
    (FILENAME = '/var/opt/mssql/data/TinhKhoanDB_log.ldf')
    FOR ATTACH;
END
";
            using var attachCmd = new SqlCommand(attachSql, masterConn) { CommandTimeout = 120 };
            await attachCmd.ExecuteNonQueryAsync();
            Console.WriteLine("✅ ATTACH 'TinhKhoanDB' thành công.");
        }
        else
        {
            Console.WriteLine("✅ Database 'TinhKhoanDB' đã tồn tại.");
        }
    }

    // If database exists, verify physical file paths are correct (not referencing backup/mismatched names)
    var fileInfo = new List<(string LogicalName, string PhysicalName)>();
    using (var filesCmd = new SqlCommand(@"SELECT mf.name AS LogicalName, mf.physical_name AS PhysicalName
                                          FROM sys.master_files mf
                                          INNER JOIN sys.databases d ON d.database_id = mf.database_id
                                          WHERE d.name = 'TinhKhoanDB'", masterConn))
    {
        using var rdr = await filesCmd.ExecuteReaderAsync();
        while (await rdr.ReadAsync())
        {
            fileInfo.Add((rdr.GetString(0), rdr.GetString(1)));
        }
    }

    string expectedMdf = "/var/opt/mssql/data/TinhKhoanDB.mdf";
    string expectedLdf = "/var/opt/mssql/data/TinhKhoanDB_log.ldf";
    bool needsFix = fileInfo.Any(f => f.PhysicalName.Contains("_backup", StringComparison.OrdinalIgnoreCase))
                    || fileInfo.Any(f => f.PhysicalName.EndsWith(".mdf", StringComparison.OrdinalIgnoreCase) && !string.Equals(f.PhysicalName, expectedMdf, StringComparison.OrdinalIgnoreCase))
                    || fileInfo.Any(f => f.PhysicalName.EndsWith(".ldf", StringComparison.OrdinalIgnoreCase) && !string.Equals(f.PhysicalName, expectedLdf, StringComparison.OrdinalIgnoreCase));
    if (needsFix)
    {
        Console.WriteLine("🛠️ Phát hiện đường dẫn file không đúng (backup/mismatch). Tiến hành sửa về /var/opt/mssql/data...");
        // Build ALTER DATABASE MODIFY FILE statements
        var alterParts = new List<string>();
        foreach (var f in fileInfo)
        {
            var physical = f.PhysicalName.Replace("_backup", string.Empty, StringComparison.OrdinalIgnoreCase);
            string targetPath;
            if (physical.EndsWith(".ldf", StringComparison.OrdinalIgnoreCase))
            {
                targetPath = expectedLdf;
            }
            else if (physical.EndsWith(".mdf", StringComparison.OrdinalIgnoreCase))
            {
                targetPath = expectedMdf;
            }
            else
            {
                continue; // skip unexpected
            }
            alterParts.Add($"ALTER DATABASE [TinhKhoanDB] MODIFY FILE (NAME = [{f.LogicalName}], FILENAME = '{targetPath}');");
        }

    var fixFilesSql = $@"
ALTER DATABASE [TinhKhoanDB] SET OFFLINE WITH ROLLBACK IMMEDIATE;
{string.Join("\n", alterParts)}
ALTER DATABASE [TinhKhoanDB] SET ONLINE;
ALTER AUTHORIZATION ON DATABASE::[TinhKhoanDB] TO [sa];
ALTER DATABASE [TinhKhoanDB] SET MULTI_USER;
";

        using (var fixFilesCmd = new SqlCommand(fixFilesSql, masterConn) { CommandTimeout = 120 })
        {
            await fixFilesCmd.ExecuteNonQueryAsync();
        }
        Console.WriteLine("🔁 Đã cập nhật đường dẫn file database, chờ ONLINE...");

        // Wait briefly for ONLINE state
        for (int i = 0; i < 10; i++)
        {
            using var stateCheck = new SqlCommand("SELECT state_desc FROM sys.databases WHERE name='TinhKhoanDB'", masterConn);
            var st = (await stateCheck.ExecuteScalarAsync())?.ToString();
            if (string.Equals(st, "ONLINE", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }
            await Task.Delay(500);
        }
        Console.WriteLine("✅ Đã cập nhật đường dẫn file database và đưa ONLINE.");
    }

    // Ensure ownership and online state (avoid permission issues)
    var fixSql = @"
IF DB_ID('TinhKhoanDB') IS NOT NULL
BEGIN
    DECLARE @state_desc NVARCHAR(60);
    SELECT @state_desc = state_desc FROM sys.databases WHERE name = 'TinhKhoanDB';
    IF (@state_desc <> 'ONLINE')
    BEGIN
        ALTER DATABASE [TinhKhoanDB] SET ONLINE;
    END
    -- Ensure db owner is 'sa' to avoid login default db issues
    IF (SELECT SUSER_SNAME(owner_sid) FROM sys.databases WHERE name = 'TinhKhoanDB') <> 'sa'
    BEGIN
        ALTER AUTHORIZATION ON DATABASE::[TinhKhoanDB] TO [sa];
    END
END
";
    using var fixCmd = new SqlCommand(fixSql, masterConn) { CommandTimeout = 60 };
    await fixCmd.ExecuteNonQueryAsync();

    Console.WriteLine("🔧 Đảm bảo 'TinhKhoanDB' ONLINE và owner=sa hoàn tất.");

    // Warm up a direct connection to TinhKhoanDB to validate fast connectivity
    var appCsb = new SqlConnectionStringBuilder(csbNormalized.ConnectionString)
    {
        InitialCatalog = "TinhKhoanDB",
        ConnectRetryCount = 0,
        ConnectTimeout = 10
    };
    Exception? warmupEx = null;
    try
    {
        using var appConn = new SqlConnection(appCsb.ConnectionString);
        await appConn.OpenAsync();
        using (var ping = new SqlCommand("SELECT TOP 1 name FROM sys.tables ORDER BY name;", appConn))
        {
            try { await ping.ExecuteScalarAsync(); } catch { /* ignore */ }
        }
        Console.WriteLine("⚡ Kết nối tới 'TinhKhoanDB' đã sẵn sàng.");
    }
    catch (Exception ex)
    {
        warmupEx = ex;
        Console.WriteLine($"⚠️ Không thể mở 'TinhKhoanDB' ngay: {ex.Message}. Sẽ thử REPAIR/ATTACH.");
    }

    if (warmupEx != null)
    {
        // Check database state and attempt a safe repair/reattach sequence
        string? stateDesc = null;
        using (var stateCmd = new SqlCommand("SELECT state_desc FROM sys.databases WHERE name='TinhKhoanDB'", masterConn))
        {
            var res = await stateCmd.ExecuteScalarAsync();
            stateDesc = res?.ToString();
        }
        Console.WriteLine($"ℹ️ TinhKhoanDB state: {stateDesc ?? "unknown"}");

        var mdfPath = "/var/opt/mssql/data/TinhKhoanDB.mdf";
        var ldfPath = "/var/opt/mssql/data/TinhKhoanDB_log.ldf";

        var repairSql = $@"
BEGIN TRY
    IF DB_ID('TinhKhoanDB') IS NOT NULL
    BEGIN
        ALTER DATABASE [TinhKhoanDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
        EXEC sp_detach_db 'TinhKhoanDB';
    END
END TRY
BEGIN CATCH
    PRINT 'Detach failed or not needed: ' + ERROR_MESSAGE();
END CATCH;

BEGIN TRY
    CREATE DATABASE [TinhKhoanDB] ON (FILENAME = '{mdfPath}') FOR ATTACH_REBUILD_LOG;
    PRINT '✅ ATTACH_REBUILD_LOG completed';
END TRY
BEGIN CATCH
    PRINT 'ATTACH_REBUILD_LOG failed: ' + ERROR_MESSAGE();
    BEGIN TRY
        CREATE DATABASE [TinhKhoanDB]
        ON (FILENAME='{mdfPath}'), (FILENAME='{ldfPath}') FOR ATTACH;
        PRINT '✅ FOR ATTACH with MDF+LDF completed';
    END TRY
    BEGIN CATCH
        PRINT 'ATTACH with MDF+LDF failed: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END CATCH;

-- Ensure ONLINE and owner
ALTER AUTHORIZATION ON DATABASE::[TinhKhoanDB] TO [sa];
ALTER DATABASE [TinhKhoanDB] SET MULTI_USER;
ALTER DATABASE [TinhKhoanDB] SET ONLINE;
";

        using (var repairCmd = new SqlCommand(repairSql, masterConn) { CommandTimeout = 180 })
        {
            await repairCmd.ExecuteNonQueryAsync();
        }

        // Try warm up again
        try
        {
            using var appConn2 = new SqlConnection(appCsb.ConnectionString);
            await appConn2.OpenAsync();
            using var ping2 = new SqlCommand("SELECT TOP 1 name FROM sys.tables ORDER BY name;", appConn2);
            try { await ping2.ExecuteScalarAsync(); } catch { }
            Console.WriteLine("✅ Đã sửa và kết nối được tới 'TinhKhoanDB'.");
        }
        catch (Exception ex2)
        {
            Console.WriteLine($"❌ Sau khi sửa vẫn không thể kết nối 'TinhKhoanDB': {ex2.Message}");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ Không thể kiểm tra/attach database qua master: {ex.Message}");
}
}
else
{
    Console.WriteLine("ℹ️  Đã bỏ qua toàn bộ bước attach/repair (dùng cho dev nhanh). Nếu DB chưa tồn tại có thể lỗi kết nối ở bước tiếp theo.");
}
Console.WriteLine($"⏱️  Thời gian xử lý khối ATTACH/REPAIR: {startupSw.ElapsedMilliseconds} ms");

// Use the normalized connection string for DbContext to avoid long retries
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(csbNormalized.ConnectionString, sqlOptions =>
    {
        sqlOptions.CommandTimeout(120);
        sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
    });
    options.EnableSensitiveDataLogging(false);
    options.EnableServiceProviderCaching(true);
    options.EnableDetailedErrors(false);
});

// Import Metrics registration (in-memory lightweight)
builder.Services.AddSingleton<Khoan.Api.Services.InMemoryImportMetrics>();
builder.Services.AddSingleton<Khoan.Api.Services.DirectImportService.IImportMetrics>(sp => sp.GetRequiredService<Khoan.Api.Services.InMemoryImportMetrics>());

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

// NOTE: Legacy extension methods AddRepositories/AddApplicationServices not present after migration cleanup – temporarily omitted.

// Configure DirectImport Settings
builder.Services.Configure<Khoan.Api.Models.Configuration.DirectImportSettings>(
    builder.Configuration.GetSection("DirectImport"));

// Temporal mapping options (optional; defaults inside class)
builder.Services.Configure<TemporalMappingOptions>(builder.Configuration.GetSection("TemporalMapping"));

// Cache Services (DISABLED for now)
// builder.Services.AddCachingServices(builder.Configuration);

// 🎯 Repository Layer - đăng ký các Repository khả dụng (từng cặp Interface/Implementation)
builder.Services.AddScoped<Khoan.Api.Repositories.IGL01Repository, Khoan.Api.Repositories.GL01Repository>();
builder.Services.AddScoped<Khoan.Api.Repositories.IGL02Repository, Khoan.Api.Repositories.GL02Repository>();
builder.Services.AddScoped<Khoan.Api.Repositories.IGL41Repository, Khoan.Api.Repositories.GL41Repository>();
builder.Services.AddScoped<Khoan.Api.Repositories.IEI01Repository, Khoan.Api.Repositories.EI01Repository>();
builder.Services.AddScoped<Khoan.Api.Repositories.ILN01Repository, Khoan.Api.Repositories.LN01Repository>();
builder.Services.AddScoped<Khoan.Api.Repositories.ILN03Repository, Khoan.Api.Repositories.LN03Repository>();
builder.Services.AddScoped<Khoan.Api.Repositories.IRR01Repository, Khoan.Api.Repositories.RR01Repository>();

// Service Layer - Only services with implementations are enabled
// builder.Services.AddScoped<Khoan.Api.Services.Interfaces.IDP01Service, DP01Service>(); // ❌ DP01Service không tồn tại
builder.Services.AddScoped<Khoan.Api.Services.Interfaces.IEI01Service, EI01Service>();
// builder.Services.AddScoped<Khoan.Api.Interfaces.ILN01Service, Khoan.Api.Services.LN01Service>(); // Disabled pending interface presence
// builder.Services.AddScoped<Khoan.Api.Services.Interfaces.ILN03Service, Khoan.Api.Services.LN03Service>(); // ✅ REMOVED - Cleanup focus on DP01+DPDA
// TODO: Enable DPDA Service after Repository fix
// builder.Services.AddScoped<Khoan.Api.Services.Interfaces.IDPDAService, Khoan.Api.Services.DPDAService>(); // ✅ DPDA - NEEDS Repository fix first
builder.Services.AddScoped<Khoan.Api.Services.Interfaces.IGL01Service, Khoan.Api.Services.GL01Service>(); // ✅ GL01 Service ENABLED
// Optional: Ensure GL01/GL02 analytics indexes exist at startup (disabled by default)
// Now controlled by configuration: IndexInitializers:Enabled

// 🛡️ Index Initializers are heavy and can spike DB connections; disable by default
// Enable by setting configuration IndexInitializers:Enabled = true (e.g., in appsettings.json or environment)
// Index initializers disabled (code not present yet after migration)
builder.Services.AddScoped<Khoan.Api.Services.Interfaces.IGL02Service, Khoan.Api.Services.GL02Service>();
builder.Services.AddScoped<Khoan.Api.Services.Interfaces.IGL41Service, Khoan.Api.Services.GL41Service>();
builder.Services.AddScoped<Khoan.Api.Services.Interfaces.ILN03Service, Khoan.Api.Services.LN03Service>();
builder.Services.AddScoped<Khoan.Api.Services.Interfaces.IRR01Service, Khoan.Api.Services.RR01Service>();

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

// Ensure the app runs on port 5055 (set before Build for clarity)
builder.WebHost.UseUrls("http://localhost:5055");

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

// Import metrics endpoint (JSON)
app.MapGet("/metrics/import", (Khoan.Api.Services.InMemoryImportMetrics metrics) => Results.Ok(metrics.Snapshot()))
    .WithName("ImportMetrics")
    .WithDescription("Direct import in-memory metrics (total rows, last duration)")
    .Produces<object>(StatusCodes.Status200OK);

// Import metrics raw batches endpoint (JSON detailed recent batches)
app.MapGet("/metrics/import/raw", (Khoan.Api.Services.InMemoryImportMetrics metrics) => Results.Ok(metrics.Raw()))
    .WithName("ImportMetricsRaw")
    .WithDescription("Raw recent batch metrics for imports (ring buffer)")
    .Produces<object>(StatusCodes.Status200OK);

// Recent parse error samples (runtime diagnostics captured during streaming parsers)
app.MapGet("/metrics/import/errors", (Khoan.Api.Services.Interfaces.IDirectImportService svc, string? table, int? limit) =>
{
    var errors = svc.GetRecentParseErrors(table);
    if (limit.HasValue && limit.Value > 0) errors = errors.Take(limit.Value).ToList();
    return Results.Ok(new { table, count = errors.Count, errors });
})
.WithName("ImportParseErrors")
.WithDescription("Recent parse error samples (first 5 per file, up to 50 total) filtered optionally by table (e.g., DP01)")
.Produces<object>(StatusCodes.Status200OK);

// Clear runtime parse error samples
app.MapPost("/metrics/import/errors/clear", () =>
{
    Khoan.Api.Services.DirectImportService.ClearRuntimeParseErrors();
    return Results.Ok(new { cleared = true });
})
.WithName("ImportParseErrorsClear")
.WithDescription("Clear the in-memory queue of parse error samples")
.Produces<object>(StatusCodes.Status200OK);

// Prometheus metrics endpoint (text/plain) cho scraping
app.MapGet("/metrics", (Khoan.Api.Services.InMemoryImportMetrics metrics) =>
{
    var text = metrics.ToPrometheus();
    return Results.Text(text, "text/plain; version=0.0.4; charset=utf-8");
})
.WithName("ImportMetricsPrometheus")
.WithDescription("In-memory import metrics theo định dạng Prometheus (text/plain)")
.Produces(StatusCodes.Status200OK);

// Health Check Endpoint with detailed JSON
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = Khoan.Api.HealthChecks.HealthCheckExtensions.WriteResponse
});
app.MapGet("/api/Health", () => new { status = "healthy", timestamp = DateTime.UtcNow });

// Lightweight readiness endpoint (no EF access)
app.MapGet("/ready", () => Results.Ok(new { ready = true, ts = DateTimeOffset.UtcNow }))
   .WithName("Ready")
   .WithDescription("Readiness probe that does not touch EF/DB")
   .Produces<object>(StatusCodes.Status200OK);

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

// Ensure database exists (skip when SKIP_DB_ATTACH=true to avoid blocking dev startup)
if (!skipAttach)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Console.WriteLine("🔍 Checking database connection...");
        var canConnect = await context.Database.CanConnectAsync();

        if (!canConnect)
        {
            Console.WriteLine("⚠️ Cannot connect to database, attempting to create...");
            await context.Database.EnsureCreatedAsync();
            Console.WriteLine("✅ Database created successfully");
        }
        else
        {
            Console.WriteLine("✅ Database connection established");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database initialization error: {ex.Message}");
    }
}
else
{
    Console.WriteLine("⏭️  SKIP_DB_ATTACH=true => Bỏ qua EF CanConnect/EnsureCreated để app khởi động nhanh.");
}

Console.WriteLine("⏱️  Tổng thời gian trước khi Run(): " + startupSw.Elapsed);
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
