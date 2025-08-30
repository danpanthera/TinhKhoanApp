using TinhKhoanApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace TinhKhoanApp.Api.Data
{
    public static class UnitsDataSeeder
    {
        public static async Task SeedCompleteUnitsAsync(ApplicationDbContext context)
        {
            Console.WriteLine("🗑️ Xóa tất cả Units hiện tại...");
            
            // Xóa tất cả units hiện tại
            var existingUnits = await context.Units.ToListAsync();
            context.Units.RemoveRange(existingUnits);
            await context.SaveChangesAsync();
            
            Console.WriteLine("🌱 Tạo dữ liệu Units mới theo danh sách...");

            var units = new List<Unit>
            {
                // Chi nhánh Lai Châu (root)
                new Unit { Id = 1, Code = "CnLaiChau", Name = "Chi nhánh Lai Châu", Type = "CNL1", ParentUnitId = null, IsDeleted = false },

                // Hội Sở
                new Unit { Id = 2, Code = "HoiSo", Name = "Hội Sở", Type = "CNL1", ParentUnitId = 1, IsDeleted = false },
                new Unit { Id = 3, Code = "HoiSoBgd", Name = "Ban Giám đốc", Type = "PNVL1", ParentUnitId = 2, IsDeleted = false },
                new Unit { Id = 4, Code = "HoiSoKhdn", Name = "Phòng Khách hàng Doanh nghiệp", Type = "PNVL1", ParentUnitId = 2, IsDeleted = false },
                new Unit { Id = 5, Code = "HoiSoKhcn", Name = "Phòng Khách hàng Cá nhân", Type = "PNVL1", ParentUnitId = 2, IsDeleted = false },
                new Unit { Id = 6, Code = "HoiSoKtnq", Name = "Phòng Kế toán & Ngân quỹ", Type = "PNVL1", ParentUnitId = 2, IsDeleted = false },
                new Unit { Id = 7, Code = "HoiSoTonghop", Name = "Phòng Tổng hợp", Type = "PNVL1", ParentUnitId = 2, IsDeleted = false },
                new Unit { Id = 8, Code = "HoiSoKhqlrr", Name = "Phòng Kế hoạch & Quản lý rủi ro", Type = "PNVL1", ParentUnitId = 2, IsDeleted = false },
                new Unit { Id = 9, Code = "HoiSoKtgs", Name = "Phòng Kiểm tra giám sát", Type = "PNVL1", ParentUnitId = 2, IsDeleted = false },

                // Chi nhánh Bình Lư
                new Unit { Id = 10, Code = "CnBinhLu", Name = "Chi nhánh Bình Lư", Type = "CNL2", ParentUnitId = 1, IsDeleted = false },
                new Unit { Id = 11, Code = "CnBinhLuBgd", Name = "Ban Giám đốc", Type = "PNVL2", ParentUnitId = 10, IsDeleted = false },
                new Unit { Id = 12, Code = "CnBinhLuKtnq", Name = "Phòng Kế toán & Ngân quỹ", Type = "PNVL2", ParentUnitId = 10, IsDeleted = false },
                new Unit { Id = 13, Code = "CnBinhLuKh", Name = "Phòng Khách hàng", Type = "PNVL2", ParentUnitId = 10, IsDeleted = false },

                // Chi nhánh Phong Thổ
                new Unit { Id = 14, Code = "CnPhongTho", Name = "Chi nhánh Phong Thổ", Type = "CNL2", ParentUnitId = 1, IsDeleted = false },
                new Unit { Id = 15, Code = "CnPhongThoBgd", Name = "Ban Giám đốc", Type = "PNVL2", ParentUnitId = 14, IsDeleted = false },
                new Unit { Id = 16, Code = "CnPhongThoKtnq", Name = "Phòng Kế toán & Ngân quỹ", Type = "PNVL2", ParentUnitId = 14, IsDeleted = false },
                new Unit { Id = 17, Code = "CnPhongThoKh", Name = "Phòng Khách hàng", Type = "PNVL2", ParentUnitId = 14, IsDeleted = false },
                new Unit { Id = 18, Code = "CnPhongThoPgdSo5", Name = "Phòng giao dịch Số 5", Type = "PGDL2", ParentUnitId = 14, IsDeleted = false },

                // Chi nhánh Sìn Hồ
                new Unit { Id = 19, Code = "CnSinHo", Name = "Chi nhánh Sìn Hồ", Type = "CNL2", ParentUnitId = 1, IsDeleted = false },
                new Unit { Id = 20, Code = "CnSinHoBgd", Name = "Ban Giám đốc", Type = "PNVL2", ParentUnitId = 19, IsDeleted = false },
                new Unit { Id = 21, Code = "CnSinHoKtnq", Name = "Phòng Kế toán & Ngân quỹ", Type = "PNVL2", ParentUnitId = 19, IsDeleted = false },
                new Unit { Id = 22, Code = "CnSinHoKh", Name = "Phòng Khách hàng", Type = "PNVL2", ParentUnitId = 19, IsDeleted = false },

                // Chi nhánh Bum Tở
                new Unit { Id = 23, Code = "CnBumTo", Name = "Chi nhánh Bum Tở", Type = "CNL2", ParentUnitId = 1, IsDeleted = false },
                new Unit { Id = 24, Code = "CnBumToBgd", Name = "Ban Giám đốc", Type = "PNVL2", ParentUnitId = 23, IsDeleted = false },
                new Unit { Id = 25, Code = "CnBumToKtnq", Name = "Phòng Kế toán & Ngân quỹ", Type = "PNVL2", ParentUnitId = 23, IsDeleted = false },
                new Unit { Id = 26, Code = "CnBumToKh", Name = "Phòng Khách hàng", Type = "PNVL2", ParentUnitId = 23, IsDeleted = false },

                // Chi nhánh Than Uyên
                new Unit { Id = 27, Code = "CnThanUyen", Name = "Chi nhánh Than Uyên", Type = "CNL2", ParentUnitId = 1, IsDeleted = false },
                new Unit { Id = 28, Code = "CnThanUyenBgd", Name = "Ban Giám đốc", Type = "PNVL2", ParentUnitId = 27, IsDeleted = false },
                new Unit { Id = 29, Code = "CnThanUyenKtnq", Name = "Phòng Kế toán & Ngân quỹ", Type = "PNVL2", ParentUnitId = 27, IsDeleted = false },
                new Unit { Id = 30, Code = "CnThanUyenKh", Name = "Phòng Khách hàng", Type = "PNVL2", ParentUnitId = 27, IsDeleted = false },
                new Unit { Id = 31, Code = "CnThanUyenPgdSo6", Name = "Phòng giao dịch số 6", Type = "PGDL2", ParentUnitId = 27, IsDeleted = false },

                // Chi nhánh Đoàn Kết
                new Unit { Id = 32, Code = "CnDoanKet", Name = "Chi nhánh Đoàn Kết", Type = "CNL2", ParentUnitId = 1, IsDeleted = false },
                new Unit { Id = 33, Code = "CnDoanKetBgd", Name = "Ban Giám đốc", Type = "PNVL2", ParentUnitId = 32, IsDeleted = false },
                new Unit { Id = 34, Code = "CnDoanKetKtnq", Name = "Phòng Kế toán & Ngân quỹ", Type = "PNVL2", ParentUnitId = 32, IsDeleted = false },
                new Unit { Id = 35, Code = "CnDoanKetKh", Name = "Phòng Khách hàng", Type = "PNVL2", ParentUnitId = 32, IsDeleted = false },
                new Unit { Id = 36, Code = "CnDoanKetPgdso1", Name = "Phòng giao dịch số 1", Type = "PGDL2", ParentUnitId = 32, IsDeleted = false },
                new Unit { Id = 37, Code = "CnDoanKetPgdso2", Name = "Phòng giao dịch số 2", Type = "PGDL2", ParentUnitId = 32, IsDeleted = false },

                // Chi nhánh Tân Uyên
                new Unit { Id = 38, Code = "CnTanUyen", Name = "Chi nhánh Tân Uyên", Type = "CNL2", ParentUnitId = 1, IsDeleted = false },
                new Unit { Id = 39, Code = "CnTanUyenBgd", Name = "Ban Giám đốc", Type = "PNVL2", ParentUnitId = 38, IsDeleted = false },
                new Unit { Id = 40, Code = "CnTanUyenKtnq", Name = "Phòng Kế toán & Ngân quỹ", Type = "PNVL2", ParentUnitId = 38, IsDeleted = false },
                new Unit { Id = 41, Code = "CnTanUyenKh", Name = "Phòng Khách hàng", Type = "PNVL2", ParentUnitId = 38, IsDeleted = false },
                new Unit { Id = 42, Code = "CnTanUyenPgdso3", Name = "Phòng giao dịch số 3", Type = "PGDL2", ParentUnitId = 38, IsDeleted = false },

                // Chi nhánh Nậm Hàng
                new Unit { Id = 43, Code = "CnNamHang", Name = "Chi nhánh Nậm Hàng", Type = "CNL2", ParentUnitId = 1, IsDeleted = false },
                new Unit { Id = 44, Code = "CnNamHangBgd", Name = "Ban Giám đốc", Type = "PNVL2", ParentUnitId = 43, IsDeleted = false },
                new Unit { Id = 45, Code = "CnNamHangKtnq", Name = "Phòng Kế toán & Ngân quỹ", Type = "PNVL2", ParentUnitId = 43, IsDeleted = false },
                new Unit { Id = 46, Code = "CnNamHangKh", Name = "Phòng Khách hàng", Type = "PNVL2", ParentUnitId = 43, IsDeleted = false }
            };

            // Tắt identity insert để có thể set ID cụ thể
            await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Units ON");
            
            context.Units.AddRange(units);
            await context.SaveChangesAsync();
            
            await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Units OFF");

            Console.WriteLine($"✅ Đã tạo {units.Count} units thành công!");
            Console.WriteLine("🎉 Seeding Units hoàn tất với dữ liệu mới!");
        }
    }
}
