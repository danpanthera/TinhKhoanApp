using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Seed complete units structure
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Clear existing units and seed with IDENTITY_INSERT
    await context.Database.ExecuteSqlRawAsync(@"
        DELETE FROM Units;

        SET IDENTITY_INSERT Units ON;

        INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
        -- Root unit
        (1, 'CNL1', 'Chi nhánh Lai Châu', 'CNL1', NULL, 0),

        -- Hội Sở
        (2, 'HOISO', 'Hội Sở', 'CNL1', 1, 0),

        -- Departments under Hội Sở
        (3, 'BGD', 'Ban Giám đốc', 'PNVL1', 2, 0),
        (4, 'PKHDN', 'Phòng Khách hàng Doanh nghiệp', 'PNVL1', 2, 0),
        (5, 'PKHCN', 'Phòng Khách hàng Cá nhân', 'PNVL1', 2, 0),
        (6, 'PKTNQ', 'Phòng Kế toán & Ngân quỹ', 'PNVL1', 2, 0),
        (7, 'PTH', 'Phòng Tổng hợp', 'PNVL1', 2, 0),
        (8, 'PKHQLRR', 'Phòng Kế hoạch & Quản lý rủi ro', 'PNVL1', 2, 0),
        (9, 'PKTGS', 'Phòng Kiểm tra giám sát', 'PNVL1', 2, 0),

        -- Chi nhánh Bình Lư
        (10, 'CNBL', 'Chi nhánh Bình Lư', 'CNL2', 1, 0),
        (20, 'BGDCNBL', 'Ban Giám đốc', 'PNVL2', 10, 0),
        (21, 'PKTNQCNBL', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 10, 0),
        (22, 'PKHCNBL', 'Phòng Khách hàng', 'PNVL2', 10, 0),

        -- Chi nhánh Phong Thổ
        (11, 'CNPT', 'Chi nhánh Phong Thổ', 'CNL2', 1, 0),
        (23, 'BGDCNPT', 'Ban Giám đốc', 'PNVL2', 11, 0),
        (24, 'PKTNQCNPT', 'Phòng KT&NQ', 'PNVL2', 11, 0),
        (25, 'PKHCNPT', 'Phòng KH', 'PNVL2', 11, 0),
        (26, 'PGD5CNPT', 'Phòng giao dịch Số 5', 'PGDL2', 11, 0),

        -- Chi nhánh Sìn Hồ
        (12, 'CNSH', 'Chi nhánh Sìn Hồ', 'CNL2', 1, 0),
        (27, 'BGDCNSH', 'Ban Giám đốc', 'PNVL2', 12, 0),
        (28, 'PKTNQCNSH', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 12, 0),
        (29, 'PKHCNSH', 'Phòng Khách hàng', 'PNVL2', 12, 0),

        -- Chi nhánh Bum Tở
        (13, 'CNBT', 'Chi nhánh Bum Tở', 'CNL2', 1, 0),
        (30, 'BGDCNBT', 'Ban Giám đốc', 'PNVL2', 13, 0),
        (31, 'PKTNQCNBT', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 13, 0),
        (32, 'PKHCNBT', 'Phòng Khách hàng', 'PNVL2', 13, 0),

        -- Chi nhánh Than Uyên
        (14, 'CNTU', 'Chi nhánh Than Uyên', 'CNL2', 1, 0),
        (33, 'BGDCNTU', 'Ban Giám đốc', 'PNVL2', 14, 0),
        (34, 'PKTNQCNTU', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 14, 0),
        (35, 'PKHCNTU', 'Phòng Khách hàng', 'PNVL2', 14, 0),
        (36, 'PGD6CNTU', 'Phòng giao dịch số 6', 'PGDL2', 14, 0),

        -- Chi nhánh Đoàn Kết
        (15, 'CNDK', 'Chi nhánh Đoàn Kết', 'CNL2', 1, 0),
        (37, 'BGDCNDK', 'Ban Giám đốc', 'PNVL2', 15, 0),
        (38, 'PKTNQCNDK', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 15, 0),
        (39, 'PKHCNDK', 'Phòng Khách hàng', 'PNVL2', 15, 0),
        (40, 'PGD1CNDK', 'Phòng giao dịch số 1', 'PGDL2', 15, 0),
        (41, 'PGD2CNDK', 'Phòng giao dịch số 2', 'PGDL2', 15, 0),

        -- Chi nhánh Tân Uyên
        (16, 'CNTUY', 'Chi nhánh Tân Uyên', 'CNL2', 1, 0),
        (42, 'BGDCNTUY', 'Ban Giám đốc', 'PNVL2', 16, 0),
        (43, 'PKTNQCNTUY', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 16, 0),
        (44, 'PKHCNTUY', 'Phòng Khách hàng', 'PNVL2', 16, 0),
        (45, 'PGD3CNTUY', 'Phòng giao dịch số 3', 'PGDL2', 16, 0),

        -- Chi nhánh Nậm Hàng
        (17, 'CNNH', 'Chi nhánh Nậm Hàng', 'CNL2', 1, 0),
        (46, 'BGDCNNH', 'Ban Giám đốc', 'PNVL2', 17, 0),
        (47, 'PKTNQCNNH', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 17, 0),
        (48, 'PKHCNNH', 'Phòng Khách hàng', 'PNVL2', 17, 0);

        SET IDENTITY_INSERT Units OFF;
    ");

    var unitsCount = await context.Units.CountAsync();
    Console.WriteLine($"✅ Successfully created {unitsCount} units!");

    // Show statistics
    var typeStats = await context.Units.GroupBy(u => u.Type)
                                     .Select(g => new { Type = g.Key, Count = g.Count() })
                                     .ToListAsync();

    Console.WriteLine("\n📊 Units by Type:");
    foreach (var stat in typeStats)
    {
        Console.WriteLine($"  {stat.Type}: {stat.Count} units");
    }

    var rootUnits = await context.Units.Where(u => u.ParentUnitId == null).ToListAsync();
    Console.WriteLine($"\n🌳 Root units: {rootUnits.Count}");

    var branchUnits = await context.Units.Where(u => u.Type == "CNL2").ToListAsync();
    Console.WriteLine($"🏢 Branch units (CNL2): {branchUnits.Count}");
}

Console.WriteLine("✅ Units structure seeding completed!");
