using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;

// Read and execute restore_original_data.sql
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", optional: true)
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection");

var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
optionsBuilder.UseSqlServer(connectionString);

using var context = new ApplicationDbContext(optionsBuilder.Options);

try
{
    Console.WriteLine("🔄 Bắt đầu khôi phục dữ liệu gốc...");

    // Đọc script SQL
    var sqlScript = await File.ReadAllTextAsync("restore_original_data.sql");

    // Execute SQL script
    await context.Database.ExecuteSqlRawAsync(sqlScript);

    // Verify results
    var unitsCount = await context.Units.CountAsync(u => !u.IsDeleted);
    var positionsCount = await context.Positions.CountAsync();
    var employeesCount = await context.Employees.CountAsync(e => e.IsActive);

    Console.WriteLine("✅ Đã khôi phục dữ liệu gốc thành công!");
    Console.WriteLine($"   - Đơn vị: {unitsCount} records");
    Console.WriteLine($"   - Chức vụ: {positionsCount} records");
    Console.WriteLine($"   - Nhân viên: {employeesCount} records");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Lỗi khi khôi phục dữ liệu: {ex.Message}");
    Console.WriteLine($"Chi tiết: {ex}");
}
