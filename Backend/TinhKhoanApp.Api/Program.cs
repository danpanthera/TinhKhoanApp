using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data; // Namespace chứa ApplicationDbContext
using TinhKhoanApp.Api.Models; // Namespace chứa các Model (cần cho WeatherForecast nếu Sếp muốn giữ lại)
using TinhKhoanApp.Api.Services; // Thêm namespace cho các Services
using TinhKhoanApp.Api.Filters; // Thêm namespace cho GlobalExceptionFilter
using TinhKhoanApp.Api.Middleware; // Thêm namespace cho Middleware
using TinhKhoanApp.Api.HealthChecks; // Thêm namespace cho HealthChecks
using TinhKhoanApp.Api.Repositories; // Thêm namespace cho Repositories
using System.Text.Json.Serialization;
using BCrypt.Net;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // Kiểm tra nếu có argument "seed" hoặc "reseed"
        if (args.Length > 0 && (args[0] == "seed" || args[0] == "reseed"))
        {
            await RunSeedOnly(args);
            return;
        }

        var builder = WebApplication.CreateBuilder(args);

        // Định nghĩa URL của Vue app dev server (Sếp thay 8080 bằng port thực tế của Vue app nếu khác)
        // var vueAppDevServerUrl = "http://localhost:8080";

        // 1. Lấy chuỗi kết nối từ appsettings.json
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        // Log đường dẫn tuyệt đối file DB
        // --- SIÊU LẬP TRÌNH VIÊN: GHI LOG ĐƯỜNG DẪN DB ĐỂ ANH DỄ KIỂM TRA ---
        if (!string.IsNullOrEmpty(connectionString))
        {
            try {
                var dbPath = connectionString.Replace("Data Source=", "").Trim();
                var absPath = System.IO.Path.GetFullPath(dbPath);
                Console.WriteLine($"[DEBUG] Đường dẫn tuyệt đối file SQLite DB: {absPath}");
            } catch (Exception ex) {
                Console.WriteLine($"[DEBUG] Lỗi khi lấy đường dẫn DB: {ex.Message}");
            }
        }
        // --- HẾT PHẦN GHI LOG DB ---
        // 2. Đăng ký ApplicationDbContext với SQLite provider
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));

        // 3. Đăng ký các dịch vụ cho Controllers (quan trọng nếu Sếp dùng API Controllers)
        /* builder.Services.AddControllers(); */
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                // Dòng này sẽ yêu cầu System.Text.Json đọc và ghi Enum dưới dạng chuỗi tên của chúng.
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                // --- KẾT THÚC PHẦN THÊM ---
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            });        // 3.5. Đăng ký các business services        builder.Services.AddScoped<IKpiScoringService, KpiScoringService>();
        builder.Services.AddScoped<IEmployeeKpiAssignmentService, EmployeeKpiAssignmentService>();
        builder.Services.AddScoped<UnitKpiScoringService>();
        builder.Services.AddScoped<ICompressionService, CompressionService>();
        builder.Services.AddScoped<IStatementDateService, StatementDateService>();

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
        });        // Register services        // KPI services removed during cleanup        // 🗄️ Đăng ký Raw Data Import Service
        builder.Services.AddScoped<IRawDataImportService, RawDataImportService>();
        builder.Services.AddScoped<IExtendedRawDataImportService, ExtendedRawDataImportService>();
          // 🗄️ Đăng ký SCD Type 2 Service
        builder.Services.AddScoped<SCDType2Service>();        // ⚡ PERFORMANCE OPTIMIZATIONS - Enhanced Services
        
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
            options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();        });
        
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
        builder.Services.AddScoped<IOptimizedSCDType2Service, OptimizedSCDType2Service>();
        builder.Services.AddScoped<IStreamingExportService, StreamingExportService>(); // ⚡ NEW: Streaming Export Service
        
        // Register optimized repositories
        builder.Services.AddScoped<OptimizedEmployeeRepository>();
        builder.Services.AddScoped<OptimizedSCDRepository>();// Optimize Entity Framework (remove query splitting for SQLite)
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite(connectionString, sqliteOptions =>
            {
                sqliteOptions.CommandTimeout(30);
            });
            
            // Enable connection pooling
            options.EnableServiceProviderCaching();
            options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
        });

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
        });

        // Add Health Checks
        builder.Services.AddCustomHealthChecks();// Connection pooling is handled by the existing AddDbContext registration above

        // ... (AddDbContext, AddControllers, AddSwaggerGen, etc.) ...
        var app = builder.Build();        // Cấu hình HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }        // ⚡ OPTIMIZED MIDDLEWARE PIPELINE
        
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
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast")
        .WithOpenApi();
        // --- KẾT THÚC PHẦN CODE MẪU WEATHERFORECAST ---        // Seed admin user nếu chưa có
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
              // Seed Units trước với cấu trúc phân cấp đúng            if (!db.Units.Any())
            {                // Tạo CNL1 (chi nhánh cấp 1 - root)
                var cnl1Units = new[]
                {
                    new Unit { Code = "CnLaiChau", Name = "Agribank CN Lai Châu (7800)", Type = "CNL1", ParentUnitId = null }
                };
                db.Units.AddRange(cnl1Units);
                db.SaveChanges();

                // Lấy ID của CNL1 vừa tạo
                var cnl1 = db.Units.First(u => u.Code == "CnLaiChau");                // Tạo CNL2 (chi nhánh cấp 2 - con của CNL1)
                var cnl2Units = new[]
                {
                    new Unit { Code = "CnTamDuong", Name = "Chi nhánh Tam Đường", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnPhongTho", Name = "Chi nhánh Phong Thổ", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnSinHo", Name = "Chi nhánh Sìn Hồ", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnMuongTe", Name = "Chi nhánh Mường Tè", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnThanUyen", Name = "Chi nhánh Than Uyên", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnThanhPho", Name = "Chi nhánh Thành Phố", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnTanUyen", Name = "Chi nhánh Tân Uyên", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnNamNhun", Name = "Chi nhánh Nậm Nhùn", Type = "CNL2", ParentUnitId = cnl1.Id }
                };
                db.Units.AddRange(cnl2Units);
                db.SaveChanges();                // Lấy ID của các CNL2 vừa tạo
                var cnLaiChau = db.Units.First(u => u.Code == "CnLaiChau");
                var cnTamDuong = db.Units.First(u => u.Code == "CnTamDuong");
                var cnPhongTho = db.Units.First(u => u.Code == "CnPhongTho");
                var cnSinHo = db.Units.First(u => u.Code == "CnSinHo");
                var cnMuongTe = db.Units.First(u => u.Code == "CnMuongTe");
                var cnThanUyen = db.Units.First(u => u.Code == "CnThanUyen");
                var cnThanhPho = db.Units.First(u => u.Code == "CnThanhPho");
                var cnTanUyen = db.Units.First(u => u.Code == "CnTanUyen");
                var cnNamNhun = db.Units.First(u => u.Code == "CnNamNhun");// Tạo các phòng nghiệp vụ trực thuộc CNL1
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
                    new Unit { Code = "KhCnTamDuong", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnTamDuong.Id },
                    new Unit { Code = "KtnqCnTamDuong", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnTamDuong.Id },

                    // Chi nhánh Phong Thổ
                    new Unit { Code = "KhCnPhongTho", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnPhongTho.Id },
                    new Unit { Code = "KtnqCnPhongTho", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnPhongTho.Id },
                    new Unit { Code = "PgdMuongSo", Name = "Phòng giao dịch Mường So", Type = "PGD", ParentUnitId = cnPhongTho.Id },

                    // Chi nhánh Sìn Hồ
                    new Unit { Code = "KhCnSinHo", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnSinHo.Id },
                    new Unit { Code = "KtnqCnSinHo", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnSinHo.Id },

                    // Chi nhánh Mường Tè
                    new Unit { Code = "KhCnMuongTe", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnMuongTe.Id },
                    new Unit { Code = "KtnqCnMuongTe", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnMuongTe.Id },

                    // Chi nhánh Than Uyên
                    new Unit { Code = "KhCnThanUyen", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnThanUyen.Id },
                    new Unit { Code = "KtnqCnThanUyen", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnThanUyen.Id },
                    new Unit { Code = "PgdMuongThan", Name = "Phòng giao dịch Mường Than", Type = "PGD", ParentUnitId = cnThanUyen.Id },

                    // Chi nhánh Thành Phố
                    new Unit { Code = "KhCnThanhPho", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnThanhPho.Id },
                    new Unit { Code = "KtnqCnThanhPho", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnThanhPho.Id },
                    new Unit { Code = "PgdSo1", Name = "Phòng giao dịch số 1", Type = "PGD", ParentUnitId = cnThanhPho.Id },
                    new Unit { Code = "PgdSo2", Name = "Phòng giao dịch số 2", Type = "PGD", ParentUnitId = cnThanhPho.Id },

                    // Chi nhánh Tân Uyên
                    new Unit { Code = "KhCnTanUyen", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnTanUyen.Id },
                    new Unit { Code = "KtnqCnTanUyen", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnTanUyen.Id },
                    new Unit { Code = "PgdSo3", Name = "Phòng giao dịch số 3", Type = "PGD", ParentUnitId = cnTanUyen.Id },

                    // Chi nhánh Nậm Nhùn
                    new Unit { Code = "KhCnNamNhun", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnNamNhun.Id },
                    new Unit { Code = "KtnqCnNamNhun", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnNamNhun.Id }
                };
                db.Units.AddRange(cnl2Departments);
                db.SaveChanges();
            }
              // Seed Positions trước            if (!db.Positions.Any())
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
            }          if (!db.Employees.Any(e => e.Username == "admin"))
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

        app.Run();    }    private static async Task RunSeedOnly(string[] args)
    {
        Console.WriteLine("Chạy seeding dữ liệu...");
        
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
          builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));
            
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
                    new Unit { Code = "CnLaiChau", Name = "Agribank CN Lai Châu (7800)", Type = "CNL1", ParentUnitId = null }
                };
                db.Units.AddRange(cnl1Units);
                db.SaveChanges();

                var cnl1 = db.Units.First(u => u.Code == "CnLaiChau");                var cnl2Units = new[]
                {
                    new Unit { Code = "CnTamDuong", Name = "Chi nhánh Tam Đường", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnPhongTho", Name = "Chi nhánh Phong Thổ", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnSinHo", Name = "Chi nhánh Sìn Hồ", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnMuongTe", Name = "Chi nhánh Mường Tè", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnThanUyen", Name = "Chi nhánh Than Uyên", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnThanhPho", Name = "Chi nhánh Thành Phố", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnTanUyen", Name = "Chi nhánh Tân Uyên", Type = "CNL2", ParentUnitId = cnl1.Id },
                    new Unit { Code = "CnNamNhun", Name = "Chi nhánh Nậm Nhùn", Type = "CNL2", ParentUnitId = cnl1.Id }
                };
                db.Units.AddRange(cnl2Units);
                db.SaveChanges();                var cnTamDuong = db.Units.First(u => u.Code == "CnTamDuong");
                var cnPhongTho = db.Units.First(u => u.Code == "CnPhongTho");
                var cnSinHo = db.Units.First(u => u.Code == "CnSinHo");
                var cnMuongTe = db.Units.First(u => u.Code == "CnMuongTe");
                var cnThanUyen = db.Units.First(u => u.Code == "CnThanUyen");
                var cnThanhPho = db.Units.First(u => u.Code == "CnThanhPho");
                var cnTanUyen = db.Units.First(u => u.Code == "CnTanUyen");
                var cnNamNhun = db.Units.First(u => u.Code == "CnNamNhun");var cnl1Departments = new[]
                {
                    new Unit { Code = "Khdn", Name = "Phòng Khách hàng doanh nghiệp", Type = "Khdn", ParentUnitId = cnl1.Id },
                    new Unit { Code = "Khcn", Name = "Phòng Khách hàng cá nhân", Type = "Khcn", ParentUnitId = cnl1.Id },
                    new Unit { Code = "Khqlrr", Name = "Phòng Kế hoạch & Quản lý rủi ro", Type = "Khqlrr", ParentUnitId = cnl1.Id },
                    new Unit { Code = "KtnqCnl1", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnl1.Id },
                    new Unit { Code = "Ktgs", Name = "Phòng Kiểm tra giám sát", Type = "Ktgs", ParentUnitId = cnl1.Id },
                    new Unit { Code = "Tonghop", Name = "Phòng Tổng hợp", Type = "Tonghop", ParentUnitId = cnl1.Id }
                };
                db.Units.AddRange(cnl1Departments);                var cnl2Departments = new[]
                {
                    // Chi nhánh Tam Đường
                    new Unit { Code = "KhCnTamDuong", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnTamDuong.Id },
                    new Unit { Code = "KtnqCnTamDuong", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnTamDuong.Id },

                    // Chi nhánh Phong Thổ
                    new Unit { Code = "KhCnPhongTho", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnPhongTho.Id },
                    new Unit { Code = "KtnqCnPhongTho", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnPhongTho.Id },
                    new Unit { Code = "PgdMuongSo", Name = "Phòng giao dịch Mường So", Type = "PGD", ParentUnitId = cnPhongTho.Id },

                    // Chi nhánh Sìn Hồ
                    new Unit { Code = "KhCnSinHo", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnSinHo.Id },
                    new Unit { Code = "KtnqCnSinHo", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnSinHo.Id },

                    // Chi nhánh Mường Tè
                    new Unit { Code = "KhCnMuongTe", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnMuongTe.Id },
                    new Unit { Code = "KtnqCnMuongTe", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnMuongTe.Id },

                    // Chi nhánh Than Uyên
                    new Unit { Code = "KhCnThanUyen", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnThanUyen.Id },
                    new Unit { Code = "KtnqCnThanUyen", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnThanUyen.Id },
                    new Unit { Code = "PgdMuongThan", Name = "Phòng giao dịch Mường Than", Type = "PGD", ParentUnitId = cnThanUyen.Id },

                    // Chi nhánh Thành Phố
                    new Unit { Code = "KhCnThanhPho", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnThanhPho.Id },
                    new Unit { Code = "KtnqCnThanhPho", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnThanhPho.Id },
                    new Unit { Code = "PgdSo1", Name = "Phòng giao dịch số 1", Type = "PGD", ParentUnitId = cnThanhPho.Id },
                    new Unit { Code = "PgdSo2", Name = "Phòng giao dịch số 2", Type = "PGD", ParentUnitId = cnThanhPho.Id },

                    // Chi nhánh Tân Uyên
                    new Unit { Code = "KhCnTanUyen", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnTanUyen.Id },
                    new Unit { Code = "KtnqCnTanUyen", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnTanUyen.Id },
                    new Unit { Code = "PgdSo3", Name = "Phòng giao dịch số 3", Type = "PGD", ParentUnitId = cnTanUyen.Id },

                    // Chi nhánh Nậm Nhùn
                    new Unit { Code = "KhCnNamNhun", Name = "Phòng Khách hàng", Type = "Kh", ParentUnitId = cnNamNhun.Id },
                    new Unit { Code = "KtnqCnNamNhun", Name = "Phòng Kế toán & Ngân quỹ", Type = "Ktnq", ParentUnitId = cnNamNhun.Id }
                };
                db.Units.AddRange(cnl2Departments);
                db.SaveChanges();
            }            Console.WriteLine("Đang seed dữ liệu Positions...");
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
                db.SaveChanges();            }
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