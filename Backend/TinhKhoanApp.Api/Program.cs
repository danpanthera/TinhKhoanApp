using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data; // Namespace chứa ApplicationDbContext
using TinhKhoanApp.Api.Models; // Namespace chứa các Model (cần cho WeatherForecast nếu Sếp muốn giữ lại)
using TinhKhoanApp.Api.Services; // Thêm namespace cho các Services
using TinhKhoanApp.Api.Services.Interfaces; // Thêm namespace cho Interface Services
using TinhKhoanApp.Api.Filters; // Thêm namespace cho GlobalExceptionFilter
using TinhKhoanApp.Api.Middleware; // Thêm namespace cho Middleware
using TinhKhoanApp.Api.HealthChecks; // Thêm namespace cho HealthChecks
using TinhKhoanApp.Api.Utils; // 🕐 Thêm Utils cho VietnamDateTime
using TinhKhoanApp.Api.Repositories; // Thêm namespace cho Repositories
using TinhKhoanApp.Api.Converters; // Thêm namespace cho DateTime Converters
using System.Text.Json.Serialization;
using BCrypt.Net;
using Microsoft.AspNetCore.Http.Features; // For FormOptions

internal partial class Program
{
    private static async Task Main(string[] args)
    {
        // 🕐 Cấu hình timezone cho Hà Nội (UTC+7) - SET SYSTEM DEFAULT
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
                vietnamTimeZone = TimeZoneInfo.Local; // Fallback to system timezone
                Console.WriteLine($"⚠️ Vietnam timezone not found, using system default: {vietnamTimeZone.DisplayName}");
            }
        }

        // 🔥 SET APPLICATION TIMEZONE GLOBALLY
        Environment.SetEnvironmentVariable("TZ", "Asia/Ho_Chi_Minh");
        Console.WriteLine($"🌏 Application timezone: {TimeZoneInfo.Local.DisplayName}");

        // Kiểm tra nếu có argument "seed" hoặc "reseed"
        if (args.Length > 0 && (args[0] == "seed" || args[0] == "reseed"))
        {
            await RunSeedOnly(args);
            return;
        }

        var builder = WebApplication.CreateBuilder(args);

        // Định nghĩa URL của Vue app dev server (Sếp thay 8080 bằng port thực tế của Vue app nếu khác)
        // var vueAppDevServerUrl = "http://localhost:8080";        // 1. Lấy connection string cho SQL Server
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("SQL Server connection string is not configured.");
        }        // 2. Đăng ký ApplicationDbContext với SQL Server provider - OPTIMIZED
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                // 🚀 PERFORMANCE OPTIMIZATIONS
                sqlOptions.CommandTimeout(120); // 2 phút timeout cho commands
                sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null); // Retry logic
            });

            // 🚀 EF Core Performance Settings
            options.EnableSensitiveDataLogging(false); // Tắt sensitive logging trong production
            options.EnableServiceProviderCaching(true); // Cache service provider
            options.EnableDetailedErrors(false); // Tắt detailed errors trong production
        });

        // 3. Đăng ký các dịch vụ cho Controllers (quan trọng nếu Sếp dùng API Controllers)
        /* builder.Services.AddControllers(); */
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                // Dòng này sẽ yêu cầu System.Text.Json đọc và ghi Enum dưới dạng chuỗi tên của chúng.
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

                // 🕐 Cấu hình DateTime serialization cho timezone Vietnam (UTC+7)
                options.JsonSerializerOptions.Converters.Add(new VietnamDateTimeConverter());
                options.JsonSerializerOptions.Converters.Add(new VietnamNullableDateTimeConverter());

                // --- KẾT THÚC PHẦN THÊM ---
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

                // 🇻🇳 CẤU HÌNH UTF-8 CHO TIẾNG VIỆT - Azure SQL Edge
                options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                options.JsonSerializerOptions.PropertyNamingPolicy = null; // Giữ nguyên tên property
            });

        // 🔧 Cấu hình cho file upload lớn
        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 500_000_000; // 500MB
            options.ValueLengthLimit = int.MaxValue;
            options.ValueCountLimit = int.MaxValue;
            options.KeyLengthLimit = int.MaxValue;
        });

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Limits.MaxRequestBodySize = 500_000_000; // 500MB
            // 🕐 Thêm timeout configuration cho file upload lớn
            options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(10);
            options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
        });        // 3.5. Đăng ký các business services        builder.Services.AddScoped<IKpiScoringService, KpiScoringService>();
        builder.Services.AddScoped<IEmployeeKpiAssignmentService, EmployeeKpiAssignmentService>();
        builder.Services.AddScoped<UnitKpiScoringService>();
        builder.Services.AddScoped<IStatementDateService, StatementDateService>();
        builder.Services.AddScoped<DashboardCalculationService>();
        builder.Services.AddScoped<IBranchCalculationService, BranchCalculationService>();
        // 💰 Đăng ký service tính toán nguồn vốn từ DP01 (dữ liệu thô)
        // ❌ LEGACY SERVICE - DISABLED: Depends on ImportedDataItems which has been removed
        // builder.Services.AddScoped<IRawDataService, RawDataService>();

        // 🚀 NEW: Smart Data Import Service for automatic file routing
        builder.Services.AddScoped<ISmartDataImportService, SmartDataImportService>();

        // 🏆 NEW: Direct Import Service - Import trực tiếp vào bảng riêng, bỏ ImportedDataItems
        builder.Services.AddScoped<IDirectImportService, DirectImportService>();

        // 4. Đăng ký các dịch vụ cho Swagger/OpenAPI (để tạo tài liệu API tự động)
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        // Thêm dịch vụ CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
        });        // Add JWT authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "default-secret-key"))
            };

            // Thêm logging cho authentication events
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"🔒 JWT Authentication failed: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Console.WriteLine($"✅ JWT Token validated for user: {context.Principal?.Identity?.Name}");
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    Console.WriteLine($"⚠️ JWT Challenge triggered: {context.Error} - {context.ErrorDescription}");
                    return Task.CompletedTask;
                }
            };
        });        // Register services        // KPI services removed during cleanup        // 🗄️ Đăng ký Raw Data Import Service
        builder.Services.AddScoped<IRawDataImportService, RawDataImportService>();
        builder.Services.AddScoped<IExtendedRawDataImportService, ExtendedRawDataImportService>();

        // 🔄 Đăng ký Raw Data Processing Service - để xử lý dữ liệu CSV thành History models
        builder.Services.AddScoped<IRawDataProcessingService, RawDataProcessingService>();

        // � CHUẨN HÓA: Đăng ký File Name Parsing Service để đồng nhất extract thông tin từ filename
        builder.Services.AddScoped<IFileNameParsingService, FileNameParsingService>();

        // 📊 Đăng ký Legacy Excel Reader Service để đọc file .xls (Excel 97-2003)
        builder.Services.AddScoped<ILegacyExcelReaderService, LegacyExcelReaderService>();

        // �🗄️ Đăng ký Temporal Data Service cho high-performance import
        builder.Services.AddScoped<ITemporalDataService, TemporalDataService>();

        // 🕒 Đăng ký Temporal Table Service cho SQL Server Temporal Tables
        builder.Services.AddScoped<ITemporalTableService, TemporalTableService>();

        // Add optimized memory caching
        builder.Services.AddMemoryCache(options =>
        {
            options.SizeLimit = 2000; // Increase cache entries limit
            options.CompactionPercentage = 0.75; // Compact when 75% full
            options.TrackStatistics = true; // Enable statistics tracking
        });

        // Add response caching
        builder.Services.AddResponseCaching(options =>
        {
            options.MaximumBodySize = 1024 * 1024 * 10; // 10MB
            options.UseCaseSensitivePaths = false;
        });

        // Add response compression
        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
            options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
        });

        // Register cache service based on configuration
        var useRedis = builder.Configuration.GetValue<bool>("UseRedis", false);

        // Always register MemoryCacheService
        builder.Services.AddScoped<MemoryCacheService>();

        if (useRedis)
        {
            // Add Redis connection first
            var redisConnection = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
            builder.Services.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>(sp =>
                StackExchange.Redis.ConnectionMultiplexer.Connect(redisConnection));

            // Then register Redis-dependent services
            builder.Services.AddSingleton<RedisCacheService>();
            builder.Services.AddScoped<HybridCacheService>();
            builder.Services.AddScoped<ICacheService, HybridCacheService>();
        }
        else
        {
            builder.Services.AddScoped<ICacheService, MemoryCacheService>();
        }
        builder.Services.AddScoped<IPerformanceMonitorService, PerformanceMonitorService>();
        builder.Services.AddScoped<IStreamingExportService, StreamingExportService>(); // ⚡ NEW: Streaming Export Service          // Register optimized repositories
        builder.Services.AddScoped<OptimizedEmployeeRepository>();

        // Add Global Exception Filter
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<GlobalExceptionFilter>();
            options.CacheProfiles.Add("Default", new Microsoft.AspNetCore.Mvc.CacheProfile
            {
                Duration = 300,
                Location = Microsoft.AspNetCore.Mvc.ResponseCacheLocation.Any,
                VaryByQueryKeys = new[] { "*" }
            });
        });        // Add Health Checks
        builder.Services.AddCustomHealthChecks();

        // Connection pooling is handled by the existing AddDbContext registration above

        // ... (AddDbContext, AddControllers, AddSwaggerGen, etc.) ...
        var app = builder.Build();

        // Cấu hình HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // ⚡ OPTIMIZED MIDDLEWARE PIPELINE

        // Add Response Compression (early in pipeline)
        app.UseResponseCompression();

        // Add Response Caching
        app.UseResponseCaching();

        // Add Performance Monitoring
        app.UsePerformanceMiddleware();        // Thêm CORS middleware
        app.UseCors("AllowAll");

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        // 🏥 Add Health Check Endpoints
        app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            ResponseWriter = HealthCheckExtensions.WriteResponse
        });

        app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = HealthCheckExtensions.WriteResponse
        });

        app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = HealthCheckExtensions.WriteResponse
        });

        // Thêm dòng này để kích hoạt việc sử dụng Controllers
        app.MapControllers();


        // --- PHẦN CODE MẪU WEATHERFORECAST (Sếp có thể giữ lại hoặc xóa nếu không cần) ---
        var summaries = new[]
        {
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

        app.MapGet("/weatherforecast", () =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(VietnamDateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast")
        .WithOpenApi();
        // --- KẾT THÚC PHẦN CODE MẪU WEATHERFORECAST ---        // SEEDING DISABLED DUE TO DATABASE TRIGGER ISSUES
        /*
        // Seed admin user nếu chưa có
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
              // Seed Units trước với cấu trúc phân cấp đúng
            // COMMENTED OUT DUE TO TRIGGER ISSUES
            if (!db.Units.Any())
            {                // Tạo CNL1 (chi nhánh cấp 1 - root)
                var cnl1Units = new[]
                {
                    new Unit { Code = "CnLaiChau", Name = "Agribank CN Lai Châu (9999)", Type = "CNL1", ParentUnitId = null }
                };
                db.Units.AddRange(cnl1Units);
                db.SaveChanges();

                // Lấy ID của CNL1 vừa tạo
                var cnl1 = db.Units.First(u => u.Code == "CnLaiChau");                // Tạo CNL2 (chi nhánh cấp 2 - con của CNL1) - cập nhật tên mới
                var cnl2Units = new[]
                {
                    new Unit { Code = "CnBinhLu", Name = "Chi nhánh Bình Lư", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnPhongTho", Name = "Chi nhánh Phong Thổ", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnSinHo", Name = "Chi nhánh Sìn Hồ", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnBumTo", Name = "Chi nhánh Bum Tở", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnThanUyen", Name = "Chi nhánh Than Uyên", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnDoanKet", Name = "Chi nhánh Đoàn Kết", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnTanUyen", Name = "Chi nhánh Tân Uyên", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnNamHang", Name = "Chi nhánh Nậm Hàng", Type = "CNL2", ParentUnitId = cnl1.Id }
                };
                db.Units.AddRange(cnl2Units);
                db.SaveChanges();                // Lấy ID của các CNL2 vừa tạo - cập nhật tên code mới
                var cnLaiChau = db.Units.First(u => u.Code == "CnLaiChau");
                var cnBinhLu = db.Units.First(u => u.Code == "CnBinhLu");
                var cnPhongTho = db.Units.First(u => u.Code == "CnPhongTho");
                var cnSinHo = db.Units.First(u => u.Code == "CnSinHo");
                var cnBumTo = db.Units.First(u => u.Code == "CnBumTo");
                var cnThanUyen = db.Units.First(u => u.Code == "CnThanUyen");
                var cnDoanKet = db.Units.First(u => u.Code == "CnDoanKet");
                var cnTanUyen = db.Units.First(u => u.Code == "CnTanUyen");
                var cnNamHang = db.Units.First(u => u.Code == "CnNamHang");// Tạo các phòng nghiệp vụ trực thuộc CNL1
                var cnl1Departments = new[]
                {
                    new Unit { Code = "Khdn", Name = "Phòng Khách hàng doanh nghiệp", Type = "Khdn", ParentUnitId = cnl1.Id },
                    new Unit { Code = "Khcn", Name = "Phòng Khách hàng cá nhân", Type = "Khcn", ParentUnitId = cnl1.Id },
                    new Unit { Code = "Khqlrr", Name = "Phòng Kế hoạch & Quản lý rủi ro", Type = "Khqlrr", ParentUnitId = cnl1.Id },
                    new Unit { Code = "KtnqCnl1", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnl1.Id },
                    new Unit { Code = "Ktgs", Name = "Phòng Kiểm tra giám sát", Type = "Ktgs", ParentUnitId = cnl1.Id },
                    new Unit { Code = "Tonghop", Name = "Phòng Tổng hợp", Type = "Tonghop", ParentUnitId = cnl1.Id }
                };
                db.Units.AddRange(cnl1Departments);                // Tạo các phòng nghiệp vụ trực thuộc CNL2
                var cnl2Departments = new[]
                {
                    // Chi nhánh Tam Đường
                    new Unit { Code = "KhCnTamDuong", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnBinhLu.Id },
                    new Unit { Code = "KtnqCnTamDuong", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnBinhLu.Id },

                    // Chi nhánh Phong Thổ
                    new Unit { Code = "KhCnPhongTho", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnPhongTho.Id },
                    new Unit { Code = "KtnqCnPhongTho", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnPhongTho.Id },
                    new Unit { Code = "PgdMuongSo", Name = "Phòng giao dịch Mường So", Type = "PGD", ParentUnitId = cnPhongTho.Id },

                    // Chi nhánh Sìn Hồ
                    new Unit { Code = "KhCnSinHo", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnSinHo.Id },
                    new Unit { Code = "KtnqCnSinHo", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnSinHo.Id },

                    // Chi nhánh Mường Tè
                    new Unit { Code = "KhCnMuongTe", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnBumTo.Id },
                    new Unit { Code = "KtnqCnMuongTe", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnBumTo.Id },

                    // Chi nhánh Than Uyên
                    new Unit { Code = "KhCnThanUyen", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnThanUyen.Id },
                    new Unit { Code = "KtnqCnThanUyen", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnThanUyen.Id },
                    new Unit { Code = "PgdMuongThan", Name = "Phòng giao dịch Mường Than", Type = "PGD", ParentUnitId = cnThanUyen.Id },

                    // Chi nhánh Thành Phố
                    new Unit { Code = "KhCnThanhPho", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnDoanKet.Id },
                    new Unit { Code = "KtnqCnThanhPho", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnDoanKet.Id },
                    new Unit { Code = "PgdSo1", Name = "Phòng giao dịch số 1", Type = "PGD", ParentUnitId = cnDoanKet.Id },
                    new Unit { Code = "PgdSo2", Name = "Phòng giao dịch số 2", Type = "PGD", ParentUnitId = cnDoanKet.Id },

                    // Chi nhánh Tân Uyên
                    new Unit { Code = "KhCnTanUyen", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnTanUyen.Id },
                    new Unit { Code = "KtnqCnTanUyen", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnTanUyen.Id },
                    new Unit { Code = "PgdSo3", Name = "Phòng giao dịch số 3", Type = "PGD", ParentUnitId = cnTanUyen.Id },

                    // Chi nhánh Nậm Nhùn
                    new Unit { Code = "KhCnNamNhun", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnNamHang.Id },
                    new Unit { Code = "KtnqCnNamNhun", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnNamHang.Id }
                };
                db.Units.AddRange(cnl2Departments);
                db.SaveChanges();
            }            // Seed Positions - check if they exist to avoid conflicts with cleanup
            if (!db.Positions.Any())
            {
                try
                {
                    // Use raw SQL to insert positions without explicit IDs to avoid identity conflicts
                    db.Database.ExecuteSqlRaw(@"
                        INSERT INTO Positions (Name, Description) VALUES
                        ('Giamdoc', 'Giám đốc'),
                        ('Phogiamdoc', 'Phó Giám đốc'),
                        ('Truongphong', 'Trưởng phòng'),
                        ('Photruongphong', 'Phó trưởng phòng'),
                        ('GiamdocPhonggiaodich', 'Giám đốc phòng giao dịch'),
                        ('PhogiamdocPhonggiaodich', 'Phó giám đốc phòng giao dịch'),
                        ('Nhanvien', 'Nhân viên')
                    ");
                }
                catch (Exception ex)
                {
                    // If raw SQL fails, try EF approach as fallback
                    Console.WriteLine($"Raw SQL insert failed, trying EF: {ex.Message}");
                    db.Positions.AddRange(new[]
                    {
                        new Position { Name = "Giamdoc", Description = "Giám đốc" },
                        new Position { Name = "Phogiamdoc", Description = "Phó Giám đốc" },
                        new Position { Name = "Truongphong", Description = "Trưởng phòng" },
                        new Position { Name = "Photruongphong", Description = "Phó trưởng phòng" },
                        new Position { Name = "GiamdocPhonggiaodich", Description = "Giám đốc phòng giao dịch" },
                        new Position { Name = "PhogiamdocPhonggiaodich", Description = "Phó giám đốc phòng giao dịch" },
                        new Position { Name = "Nhanvien", Description = "Nhân viên" }
                    });
                    db.SaveChanges();
                }
            }if (!db.Employees.Any(e => e.Username == "admin"))
            {
                // Find a basic position like "Nhanvien" (Nhân viên) for admin
                var basicPosition = db.Positions.FirstOrDefault(p => p.Name == "Nhanvien") ??
                                   db.Positions.FirstOrDefault();

                db.Employees.Add(new Employee
                {
                    EmployeeCode = "ADMIN",
                    FullName = "Quản trị viên",
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Email = "admin@tinhkhoan.local",
                    IsActive = true,
                    UnitId = db.Units.FirstOrDefault()?.Id ?? 1,
                    PositionId = basicPosition?.Id ?? 1
                });
                db.SaveChanges();            }// Seed dữ liệu vai trò (roles) cho 23 KPI table types
            RoleSeeder.SeedRoles(db);

            // Seed dữ liệu định nghĩa KPI cho 23 vai trò (QUAN TRỌNG: phải gọi trước KpiAssignmentTableSeeder)
            SeedKPIDefinitionMaxScore.SeedKPIDefinitions(db);

            // Seed dữ liệu cho 23 bảng giao khoán KPI chuẩn cho cán bộ
            KpiAssignmentTableSeeder.SeedKpiAssignmentTables(db);

            // Seed dữ liệu kỳ khoán mẫu
            // KhoanPeriodSeeder.SeedKhoanPeriods(db); // Tạm comment để test import

            // Seed dữ liệu nhân viên mẫu
            // await EmployeeSeeder.SeedEmployees(db); // Tạm comment để test import
        }

        */
        // END OF COMMENTED SEEDING SECTION

        app.Run();
    }
    private static async Task RunSeedOnly(string[] args)
    {
        Console.WriteLine("Chạy seeding dữ liệu...");

        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Nếu có args reseed thì xóa dữ liệu cũ trước
            if (args.Length > 0 && args[0] == "reseed")
            {
                Console.WriteLine("Xóa dữ liệu KPI cũ...");
                db.KpiIndicators.RemoveRange(db.KpiIndicators);
                db.KpiAssignmentTables.RemoveRange(db.KpiAssignmentTables);
                db.SaveChanges();
            }            // Seed Units và Positions trước (cần thiết cho Employee seeding)
            Console.WriteLine("Đang seed dữ liệu Units...");
            if (!db.Units.Any())
            {                // Copy logic seeding Units từ Main method
                var cnl1Units = new[]
                {
                    new Unit { Code = "CnLaiChau", Name = "Agribank CN Lai Châu (9999)", Type = "CNL1", ParentUnitId = null }
                };
                db.Units.AddRange(cnl1Units);
                db.SaveChanges();

                var cnl1 = db.Units.First(u => u.Code == "CnLaiChau"); var cnl2Units = new[]
                {
                    new Unit { Code = "CnBinhLu", Name = "Chi nhánh Bình Lư", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnPhongTho", Name = "Chi nhánh Phong Thổ", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnSinHo", Name = "Chi nhánh Sìn Hồ", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnBumTo", Name = "Chi nhánh Bum Tở", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnThanUyen", Name = "Chi nhánh Than Uyên", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnDoanKet", Name = "Chi nhánh Đoàn Kết", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnTanUyen", Name = "Chi nhánh Tân Uyên", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnNamHang", Name = "Chi nhánh Nậm Hàng", Type = "CNL2", ParentUnitId = cnl1.Id }
                };
                db.Units.AddRange(cnl2Units);
                db.SaveChanges(); var cnBinhLu = db.Units.First(u => u.Code == "CnBinhLu");
                var cnPhongTho = db.Units.First(u => u.Code == "CnPhongTho");
                var cnSinHo = db.Units.First(u => u.Code == "CnSinHo");
                var cnBumTo = db.Units.First(u => u.Code == "CnBumTo");
                var cnThanUyen = db.Units.First(u => u.Code == "CnThanUyen");
                var cnDoanKet = db.Units.First(u => u.Code == "CnDoanKet");
                var cnTanUyen = db.Units.First(u => u.Code == "CnTanUyen");
                var cnNamHang = db.Units.First(u => u.Code == "CnNamHang");// Tạo các phòng nghiệp vụ trực thuộc CNL1
                var cnl1Departments = new[]
                {
                    new Unit { Code = "Khdn", Name = "Phòng Khách hàng doanh nghiệp", Type = "Khdn", ParentUnitId = cnl1.Id },
                    new Unit { Code = "Khcn", Name = "Phòng Khách hàng cá nhân", Type = "Khcn", ParentUnitId = cnl1.Id },
                    new Unit { Code = "Khqlrr", Name = "Phòng Kế hoạch & Quản lý rủi ro", Type = "Khqlrr", ParentUnitId = cnl1.Id },
                    new Unit { Code = "KtnqCnl1", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnl1.Id },
                    new Unit { Code = "Ktgs", Name = "Phòng Kiểm tra giám sát", Type = "Ktgs", ParentUnitId = cnl1.Id },
                    new Unit { Code = "Tonghop", Name = "Phòng Tổng hợp", Type = "Tonghop", ParentUnitId = cnl1.Id }
                };
                db.Units.AddRange(cnl1Departments);                // Tạo các phòng nghiệp vụ trực thuộc CNL2
                var cnl2Departments = new[]
                {
                    // Chi nhánh Tam Đường
                    new Unit { Code = "KhCnTamDuong", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnBinhLu.Id },
                    new Unit { Code = "KtnqCnTamDuong", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnBinhLu.Id },

                    // Chi nhánh Phong Thổ
                    new Unit { Code = "KhCnPhongTho", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnPhongTho.Id },
                    new Unit { Code = "KtnqCnPhongTho", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnPhongTho.Id },
                    new Unit { Code = "PgdMuongSo", Name = "Phòng giao dịch Mường So", Type = "PGD", ParentUnitId = cnPhongTho.Id },

                    // Chi nhánh Sìn Hồ
                    new Unit { Code = "KhCnSinHo", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnSinHo.Id },
                    new Unit { Code = "KtnqCnSinHo", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnSinHo.Id },

                    // Chi nhánh Mường Tè
                    new Unit { Code = "KhCnMuongTe", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnBumTo.Id },
                    new Unit { Code = "KtnqCnMuongTe", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnBumTo.Id },

                    // Chi nhánh Than Uyên
                    new Unit { Code = "KhCnThanUyen", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnThanUyen.Id },
                    new Unit { Code = "KtnqCnThanUyen", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnThanUyen.Id },
                    new Unit { Code = "PgdMuongThan", Name = "Phòng giao dịch Mường Than", Type = "PGD", ParentUnitId = cnThanUyen.Id },

                    // Chi nhánh Thành Phố
                    new Unit { Code = "KhCnThanhPho", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnDoanKet.Id },
                    new Unit { Code = "KtnqCnThanhPho", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnDoanKet.Id },
                    new Unit { Code = "PgdSo1", Name = "Phòng giao dịch số 1", Type = "PGD", ParentUnitId = cnDoanKet.Id },
                    new Unit { Code = "PgdSo2", Name = "Phòng giao dịch số 2", Type = "PGD", ParentUnitId = cnDoanKet.Id },

                    // Chi nhánh Tân Uyên
                    new Unit { Code = "KhCnTanUyen", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnTanUyen.Id },
                    new Unit { Code = "KtnqCnTanUyen", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnTanUyen.Id },
                    new Unit { Code = "PgdSo3", Name = "Phòng giao dịch số 3", Type = "PGD", ParentUnitId = cnTanUyen.Id },

                    // Chi nhánh Nậm Nhùn
                    new Unit { Code = "KhCnNamNhun", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnNamHang.Id },
                    new Unit { Code = "KtnqCnNamNhun", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnNamHang.Id }
                };
                db.Units.AddRange(cnl2Departments);
                db.SaveChanges();
            }
            Console.WriteLine("Đang seed dữ liệu Positions...");
            if (!db.Positions.Any())
            {
                db.Positions.AddRange(new[]
                {
                    new Position { Name = "Giamdoc", Description = "Giám đốc" },
                    new Position { Name = "Phogiamdoc", Description = "Phó Giám đốc" },
                    new Position { Name = "Truongphong", Description = "Trưởng phòng" },
                    new Position { Name = "Photruongphong", Description = "Phó trưởng phòng" },
                    new Position { Name = "GiamdocPhonggiaodich", Description = "Giám đốc phòng giao dịch" },
                    new Position { Name = "PhogiamdocPhonggiaodich", Description = "Phó giám đốc phòng giao dịch" },
                    new Position { Name = "Nhanvien", Description = "Nhân viên" }
                    // Comment out old incorrect position names:
                    // new Position { Name = "Phophong", Description = "Phó phòng" },
                    // new Position { Name = "CB", Description = "Cán bộ" },
                    // new Position { Name = "Cbtd", Description = "Cán bộ tín dụng" },
                    // new Position { Name = "GDV", Description = "Giao dịch viên" },
                    // new Position { Name = "KeToan", Description = "Kế toán" },
                    // new Position { Name = "ThuQuy", Description = "Thủ quỹ" },
                    // new Position { Name = "Truongpho", Description = "Trưởng/Phó phòng" }
                });
                db.SaveChanges();
            }
            Console.WriteLine("Đang seed dữ liệu vai trò...");
            RoleSeeder.SeedRoles(db);
            Console.WriteLine("Hoàn thành seeding dữ liệu vai trò!");

            Console.WriteLine("Đang seed dữ liệu định nghĩa KPI...");
            SeedKPIDefinitionMaxScore.SeedKPIDefinitions(db);
            Console.WriteLine("Hoàn thành seeding dữ liệu định nghĩa KPI!");

            Console.WriteLine("Đang seed dữ liệu KPI...");
            KpiAssignmentTableSeeder.SeedKpiAssignmentTables(db);
            Console.WriteLine("Hoàn thành seeding dữ liệu KPI!");

            Console.WriteLine("Đang seed dữ liệu kỳ khoán...");
            KhoanPeriodSeeder.SeedKhoanPeriods(db);
            Console.WriteLine("Hoàn thành seeding dữ liệu kỳ khoán!");

            Console.WriteLine("Đang seed dữ liệu nhân viên...");
            await EmployeeSeeder.SeedEmployees(db);
            Console.WriteLine("Hoàn thành seeding dữ liệu nhân viên!");

            // Cập nhật terminology chuẩn hóa cuối cùng
            Console.WriteLine("Đang cập nhật terminology chuẩn hóa...");
            TerminologyUpdater.UpdateTerminology(db);
            Console.WriteLine("Hoàn thành cập nhật terminology!");
        }
    }
}

// Khai báo record WeatherForecast (phải nằm sau tất cả các top-level statements)
// Nếu Sếp không dùng đoạn code WeatherForecast ở trên thì có thể xóa luôn record này.
// Hoặc tốt hơn là đưa nó vào một file riêng trong thư mục Models nếu nó là một Model thực sự của dự án.
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
