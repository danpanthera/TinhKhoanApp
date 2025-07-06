using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

// Simple script runner để chạy restore_original_data.sql
class RestoreDataRunner
{
    public static async Task Main(string[] args)
    {
        try
        {
            var connectionString = "Server=localhost;Database=TinhKhoanApp;Trusted_Connection=true;TrustServerCertificate=true";

            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(connectionString);

            using var context = new TinhKhoanAppDbContext(optionsBuilder.Options);

            // Đọc SQL script
            var sqlScript = await File.ReadAllTextAsync("restore_original_data.sql");

            Console.WriteLine("🔄 Bắt đầu khôi phục dữ liệu gốc...");

            // Thực thi SQL script
            await context.Database.ExecuteSqlRawAsync(sqlScript);

            Console.WriteLine("✅ Đã hoàn thành khôi phục dữ liệu!");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi khi khôi phục dữ liệu: {ex.Message}");
            Console.WriteLine($"Chi tiết: {ex}");
        }
    }
}

// Dummy DbContext để chạy script
public class TinhKhoanAppDbContext : DbContext
{
    public TinhKhoanAppDbContext(DbContextOptions options) : base(options) { }
}
