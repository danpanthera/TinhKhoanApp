using TinhKhoanApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace TinhKhoanApp.Api.Data
{
    public static class UnitSeeder
    {
        public static async Task SeedUnitsAsync(ApplicationDbContext context)
        {
            // Kiểm tra xem đã có Units chưa
            if (await context.Units.AnyAsync())
            {
                Console.WriteLine("✅ Units đã tồn tại, bỏ qua seeding.");
                return;
            }

            Console.WriteLine("�� Bắt đầu seeding Units...");

            var units = new List<Unit>
            {
                // Trụ sở chính
                new Unit
                {
                    Code = "TSC",
                    Name = "Trụ sở chính",
                    Type = "Headquarters",
                    ParentUnitId = null,
                    IsDeleted = false
                },
                
                // Chi nhánh
                new Unit
                {
                    Code = "CNL1LC",
                    Name = "Chi nhánh cấp I Lai Châu", 
                    Type = "Branch",
                    ParentUnitId = null, // Sẽ update sau khi insert TSC
                    IsDeleted = false
                },
                
                // Phòng giao dịch
                new Unit
                {
                    Code = "PGDLC01",
                    Name = "Phòng giao dịch Lai Châu",
                    Type = "Transaction Office",
                    ParentUnitId = null, // Sẽ update sau
                    IsDeleted = false
                },
                
                new Unit
                {
                    Code = "PGDTB01", 
                    Name = "Phòng giao dịch Tam Đường",
                    Type = "Transaction Office",
                    ParentUnitId = null, // Sẽ update sau
                    IsDeleted = false
                },
                
                // Khối nghiệp vụ
                new Unit
                {
                    Code = "Khdn",
                    Name = "Khối dân cư",
                    Type = "Department",
                    ParentUnitId = null, // Sẽ update sau
                    IsDeleted = false
                },
                
                new Unit
                {
                    Code = "Khcn",
                    Name = "Khối doanh nghiệp",
                    Type = "Department", 
                    ParentUnitId = null, // Sẽ update sau
                    IsDeleted = false
                },
                
                new Unit
                {
                    Code = "Khqlrr",
                    Name = "Khối quản lý rủi ro",
                    Type = "Department",
                    ParentUnitId = null, // Sẽ update sau
                    IsDeleted = false
                }
            };

            // Insert tất cả units trước
            context.Units.AddRange(units);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Đã insert {0} units", units.Count);

            // Lấy ID của trụ sở chính và chi nhánh để làm parent
            var tsc = await context.Units.FirstAsync(u => u.Code == "TSC");
            var cnl1lc = await context.Units.FirstAsync(u => u.Code == "CNL1LC");

            // Update parent relationships
            var cnl1lcUnit = await context.Units.FirstAsync(u => u.Code == "CNL1LC");
            cnl1lcUnit.ParentUnitId = tsc.Id;

            var pgdlc01 = await context.Units.FirstAsync(u => u.Code == "PGDLC01");
            pgdlc01.ParentUnitId = cnl1lc.Id;

            var pgdtb01 = await context.Units.FirstAsync(u => u.Code == "PGDTB01");  
            pgdtb01.ParentUnitId = cnl1lc.Id;

            var khdn = await context.Units.FirstAsync(u => u.Code == "Khdn");
            khdn.ParentUnitId = cnl1lc.Id;

            var khcn = await context.Units.FirstAsync(u => u.Code == "Khcn");
            khcn.ParentUnitId = cnl1lc.Id;

            var khqlrr = await context.Units.FirstAsync(u => u.Code == "Khqlrr");
            khqlrr.ParentUnitId = cnl1lc.Id;

            await context.SaveChangesAsync();
            Console.WriteLine("✅ Đã cập nhật parent relationships cho Units");
            Console.WriteLine("🎉 Seeding Units hoàn tất!");
        }
    }
}
