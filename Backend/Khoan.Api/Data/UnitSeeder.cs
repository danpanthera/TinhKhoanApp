using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;
using Khoan.Api.Models;

namespace Khoan.Api.Data
{
    public static class UnitSeeder
    {
        public static async Task SeedUnitsAsync(ApplicationDbContext context)
        {
            // Kiểm tra xem đã có dữ liệu Units chưa
            if (await context.Units.AnyAsync())
            {
                return; // Dữ liệu đã tồn tại, không cần seed
            }

            var units = new List<Unit>
            {
                // Tên, Id, Code, Type, ParentId
                new Unit { Id = 1, Name = "Chi nhánh Lai Châu", Code = "CnLaiChau", Type = "CNL1", ParentUnitId = null },

                new Unit { Id = 2, Name = "Hội Sở", Code = "HoiSo", Type = "CNL2", ParentUnitId = 1 },
                new Unit { Id = 3, Name = "Ban Giám đốc", Code = "HoiSoBgd", Type = "PNVL1", ParentUnitId = 2 },
                new Unit { Id = 4, Name = "Phòng Khách hàng Doanh nghiệp", Code = "HoiSoKhdn", Type = "PNVL1", ParentUnitId = 2 },
                new Unit { Id = 5, Name = "Phòng Khách hàng Cá nhân", Code = "HoiSoKhcn", Type = "PNVL1", ParentUnitId = 2 },
                new Unit { Id = 6, Name = "Phòng Kế toán & Ngân quỹ", Code = "HoiSoKtnq", Type = "PNVL1", ParentUnitId = 2 },
                new Unit { Id = 7, Name = "Phòng Tổng hợp", Code = "HoiSoTonghop", Type = "PNVL1", ParentUnitId = 2 },
                new Unit { Id = 8, Name = "Phòng Kế hoạch & Quản lý rủi ro", Code = "HoiSoKhqlrr", Type = "PNVL1", ParentUnitId = 2 },
                new Unit { Id = 9, Name = "Phòng Kiểm tra giám sát", Code = "HoiSoKtgs", Type = "PNVL1", ParentUnitId = 2 },

                new Unit { Id = 10, Name = "Chi nhánh Bình Lư", Code = "CnBinhLu", Type = "CNL2", ParentUnitId = 1 },
                new Unit { Id = 11, Name = "Ban Giám đốc", Code = "CnBinhLuBgd", Type = "PNVL2", ParentUnitId = 10 },
                new Unit { Id = 12, Name = "Phòng Kế toán & Ngân quỹ", Code = "CnBinhLuKtnq", Type = "PNVL2", ParentUnitId = 10 },
                new Unit { Id = 13, Name = "Phòng Khách hàng", Code = "CnBinhLuKh", Type = "PNVL2", ParentUnitId = 10 },

                new Unit { Id = 14, Name = "Chi nhánh Phong Thổ", Code = "CnPhongTho", Type = "CNL2", ParentUnitId = 1 },
                new Unit { Id = 15, Name = "Ban Giám đốc", Code = "CnPhongThoBgd", Type = "PNVL2", ParentUnitId = 14 },
                new Unit { Id = 16, Name = "Phòng Kế toán & Ngân quỹ", Code = "CnPhongThoKtnq", Type = "PNVL2", ParentUnitId = 14 },
                new Unit { Id = 17, Name = "Phòng Khách hàng", Code = "CnPhongThoKh", Type = "PNVL2", ParentUnitId = 14 },
                new Unit { Id = 18, Name = "Phòng giao dịch Số 5", Code = "CnPhongThoPgdSo5", Type = "PGDL2", ParentUnitId = 14 },

                new Unit { Id = 19, Name = "Chi nhánh Sìn Hồ", Code = "CnSinHo", Type = "CNL2", ParentUnitId = 1 },
                new Unit { Id = 20, Name = "Ban Giám đốc", Code = "CnSinHoBgd", Type = "PNVL2", ParentUnitId = 19 },
                new Unit { Id = 21, Name = "Phòng Kế toán & Ngân quỹ", Code = "CnSinHoKtnq", Type = "PNVL2", ParentUnitId = 19 },
                new Unit { Id = 22, Name = "Phòng Khách hàng", Code = "CnSinHoKh", Type = "PNVL2", ParentUnitId = 19 },

                new Unit { Id = 23, Name = "Chi nhánh Bum Tở", Code = "CnBumTo", Type = "CNL2", ParentUnitId = 1 },
                new Unit { Id = 24, Name = "Ban Giám đốc", Code = "CnBumToBgd", Type = "PNVL2", ParentUnitId = 23 },
                new Unit { Id = 25, Name = "Phòng Kế toán & Ngân quỹ", Code = "CnBumToKtnq", Type = "PNVL2", ParentUnitId = 23 },
                new Unit { Id = 26, Name = "Phòng Khách hàng", Code = "CnBumToKh", Type = "PNVL2", ParentUnitId = 23 },

                new Unit { Id = 27, Name = "Chi nhánh Than Uyên", Code = "CnThanUyen", Type = "CNL2", ParentUnitId = 1 },
                new Unit { Id = 28, Name = "Ban Giám đốc", Code = "CnThanUyenBgd", Type = "PNVL2", ParentUnitId = 27 },
                new Unit { Id = 29, Name = "Phòng Kế toán & Ngân quỹ", Code = "CnThanUyenKtnq", Type = "PNVL2", ParentUnitId = 27 },
                new Unit { Id = 30, Name = "Phòng Khách hàng", Code = "CnThanUyenKh", Type = "PNVL2", ParentUnitId = 27 },
                new Unit { Id = 31, Name = "Phòng giao dịch số 6", Code = "CnThanUyenPgdSo6", Type = "PGDL2", ParentUnitId = 27 },

                new Unit { Id = 32, Name = "Chi nhánh Đoàn Kết", Code = "CnDoanKet", Type = "CNL2", ParentUnitId = 1 },
                new Unit { Id = 33, Name = "Ban Giám đốc", Code = "CnDoanKetBgd", Type = "PNVL2", ParentUnitId = 32 },
                new Unit { Id = 34, Name = "Phòng Kế toán & Ngân quỹ", Code = "CnDoanKetKtnq", Type = "PNVL2", ParentUnitId = 32 },
                new Unit { Id = 35, Name = "Phòng Khách hàng", Code = "CnDoanKetKh", Type = "PNVL2", ParentUnitId = 32 },
                new Unit { Id = 36, Name = "Phòng giao dịch số 1", Code = "CnDoanKetPgdso1", Type = "PGDL2", ParentUnitId = 32 },
                new Unit { Id = 37, Name = "Phòng giao dịch số 2", Code = "CnDoanKetPgdso2", Type = "PGDL2", ParentUnitId = 32 },

                new Unit { Id = 38, Name = "Chi nhánh Tân Uyên", Code = "CnTanUyen", Type = "CNL2", ParentUnitId = 1 },
                new Unit { Id = 39, Name = "Ban Giám đốc", Code = "CnTanUyenBgd", Type = "PNVL2", ParentUnitId = 38 },
                new Unit { Id = 40, Name = "Phòng Kế toán & Ngân quỹ", Code = "CnTanUyenKtnq", Type = "PNVL2", ParentUnitId = 38 },
                new Unit { Id = 41, Name = "Phòng Khách hàng", Code = "CnTanUyenKh", Type = "PNVL2", ParentUnitId = 38 },
                new Unit { Id = 42, Name = "Phòng giao dịch số 3", Code = "CnTanUyenPgdso3", Type = "PGDL2", ParentUnitId = 38 },

                new Unit { Id = 43, Name = "Chi nhánh Nậm Hàng", Code = "CnNamHang", Type = "CNL2", ParentUnitId = 1 },
                new Unit { Id = 44, Name = "Ban Giám đốc", Code = "CnNamHangBgd", Type = "PNVL2", ParentUnitId = 43 },
                new Unit { Id = 45, Name = "Phòng Kế toán & Ngân quỹ", Code = "CnNamHangKtnq", Type = "PNVL2", ParentUnitId = 43 },
                new Unit { Id = 46, Name = "Phòng Khách hàng", Code = "CnNamHangKh", Type = "PNVL2", ParentUnitId = 43 },
            };

            // Sử dụng Identity Insert để set Id cụ thể
            await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Units ON");
            
            context.Units.AddRange(units);
            await context.SaveChangesAsync();
            
            await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Units OFF");
        }
    }
}