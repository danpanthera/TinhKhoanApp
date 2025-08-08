using TinhKhoanApp.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SeedController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("units")]
        public async Task<IActionResult> SeedUnits()
        {
            try
            {
                // Clear existing units
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Units");

                var units = new List<Unit>
                {
                    // Root unit
                    new Unit { Id = 1, Code = "CNL1", Name = "Chi nhánh Lai Châu", Type = "CNL1", ParentUnitId = null },

                    // Hội Sở
                    new Unit { Id = 2, Code = "HOISO", Name = "Hội Sở", Type = "CNL1", ParentUnitId = 1 },

                    // Departments under Hội Sở
                    new Unit { Id = 3, Code = "BGD", Name = "Ban Giám đốc", Type = "PNVL1", ParentUnitId = 2 },
                    new Unit { Id = 4, Code = "PKHDN", Name = "Phòng Khách hàng Doanh nghiệp", Type = "PNVL1", ParentUnitId = 2 },
                    new Unit { Id = 5, Code = "PKHCN", Name = "Phòng Khách hàng Cá nhân", Type = "PNVL1", ParentUnitId = 2 },
                    new Unit { Id = 6, Code = "PKTNQ", Name = "Phòng Kế toán & Ngân quỹ", Type = "PNVL1", ParentUnitId = 2 },
                    new Unit { Id = 7, Code = "PTH", Name = "Phòng Tổng hợp", Type = "PNVL1", ParentUnitId = 2 },
                    new Unit { Id = 8, Code = "PKHQLRR", Name = "Phòng Kế hoạch & Quản lý rủi ro", Type = "PNVL1", ParentUnitId = 2 },
                    new Unit { Id = 9, Code = "PKTGS", Name = "Phòng Kiểm tra giám sát", Type = "PNVL1", ParentUnitId = 2 },

                    // Chi nhánh Bình Lư
                    new Unit { Id = 10, Code = "CNBL", Name = "Chi nhánh Bình Lư", Type = "CNL2", ParentUnitId = 1 },
                    new Unit { Id = 20, Code = "BGDCNBL", Name = "Ban Giám đốc", Type = "PNVL2", ParentUnitId = 10 },
                    new Unit { Id = 21, Code = "PKTNQCNBL", Name = "Phòng Kế toán & Ngân quỹ", Type = "PNVL2", ParentUnitId = 10 },
                    new Unit { Id = 22, Code = "PKHCNBL", Name = "Phòng Khách hàng", Type = "PNVL2", ParentUnitId = 10 },

                    // Chi nhánh Phong Thổ
                    new Unit { Id = 11, Code = "CNPT", Name = "Chi nhánh Phong Thổ", Type = "CNL2", ParentUnitId = 1 },
                    new Unit { Id = 23, Code = "BGDCNPT", Name = "Ban Giám đốc", Type = "PNVL2", ParentUnitId = 11 },
                    new Unit { Id = 24, Code = "PKTNQCNPT", Name = "Phòng KT&NQ", Type = "PNVL2", ParentUnitId = 11 },
                    new Unit { Id = 25, Code = "PKHCNPT", Name = "Phòng KH", Type = "PNVL2", ParentUnitId = 11 },
                    new Unit { Id = 26, Code = "PGD5CNPT", Name = "Phòng giao dịch Số 5", Type = "PGDL2", ParentUnitId = 11 },

                    // Chi nhánh Sìn Hồ
                    new Unit { Id = 12, Code = "CNSH", Name = "Chi nhánh Sìn Hồ", Type = "CNL2", ParentUnitId = 1 },
                    new Unit { Id = 27, Code = "BGDCNSH", Name = "Ban Giám đốc", Type = "PNVL2", ParentUnitId = 12 },
                    new Unit { Id = 28, Code = "PKTNQCNSH", Name = "Phòng Kế toán & Ngân quỹ", Type = "PNVL2", ParentUnitId = 12 },
                    new Unit { Id = 29, Code = "PKHCNSH", Name = "Phòng Khách hàng", Type = "PNVL2", ParentUnitId = 12 },

                    // Chi nhánh Bum Tở
                    new Unit { Id = 13, Code = "CNBT", Name = "Chi nhánh Bum Tở", Type = "CNL2", ParentUnitId = 1 },
                    new Unit { Id = 30, Code = "BGDCNBT", Name = "Ban Giám đốc", Type = "PNVL2", ParentUnitId = 13 },
                    new Unit { Id = 31, Code = "PKTNQCNBT", Name = "Phòng Kế toán & Ngân quỹ", Type = "PNVL2", ParentUnitId = 13 },
                    new Unit { Id = 32, Code = "PKHCNBT", Name = "Phòng Khách hàng", Type = "PNVL2", ParentUnitId = 13 },

                    // Chi nhánh Than Uyên
                    new Unit { Id = 14, Code = "CNTU", Name = "Chi nhánh Than Uyên", Type = "CNL2", ParentUnitId = 1 },
                    new Unit { Id = 33, Code = "BGDCNTU", Name = "Ban Giám đốc", Type = "PNVL2", ParentUnitId = 14 },
                    new Unit { Id = 34, Code = "PKTNQCNTU", Name = "Phòng Kế toán & Ngân quỹ", Type = "PNVL2", ParentUnitId = 14 },
                    new Unit { Id = 35, Code = "PKHCNTU", Name = "Phòng Khách hàng", Type = "PNVL2", ParentUnitId = 14 },
                    new Unit { Id = 36, Code = "PGD6CNTU", Name = "Phòng giao dịch số 6", Type = "PGDL2", ParentUnitId = 14 },

                    // Chi nhánh Đoàn Kết
                    new Unit { Id = 15, Code = "CNDK", Name = "Chi nhánh Đoàn Kết", Type = "CNL2", ParentUnitId = 1 },
                    new Unit { Id = 37, Code = "BGDCNDK", Name = "Ban Giám đốc", Type = "PNVL2", ParentUnitId = 15 },
                    new Unit { Id = 38, Code = "PKTNQCNDK", Name = "Phòng Kế toán & Ngân quỹ", Type = "PNVL2", ParentUnitId = 15 },
                    new Unit { Id = 39, Code = "PKHCNDK", Name = "Phòng Khách hàng", Type = "PNVL2", ParentUnitId = 15 },
                    new Unit { Id = 40, Code = "PGD1CNDK", Name = "Phòng giao dịch số 1", Type = "PGDL2", ParentUnitId = 15 },
                    new Unit { Id = 41, Code = "PGD2CNDK", Name = "Phòng giao dịch số 2", Type = "PGDL2", ParentUnitId = 15 },

                    // Chi nhánh Tân Uyên
                    new Unit { Id = 16, Code = "CNTUY", Name = "Chi nhánh Tân Uyên", Type = "CNL2", ParentUnitId = 1 },
                    new Unit { Id = 42, Code = "BGDCNTUY", Name = "Ban Giám đốc", Type = "PNVL2", ParentUnitId = 16 },
                    new Unit { Id = 43, Code = "PKTNQCNTUY", Name = "Phòng Kế toán & Ngân quỹ", Type = "PNVL2", ParentUnitId = 16 },
                    new Unit { Id = 44, Code = "PKHCNTUY", Name = "Phòng Khách hàng", Type = "PNVL2", ParentUnitId = 16 },
                    new Unit { Id = 45, Code = "PGD3CNTUY", Name = "Phòng giao dịch số 3", Type = "PGDL2", ParentUnitId = 16 },

                    // Chi nhánh Nậm Hàng
                    new Unit { Id = 17, Code = "CNNH", Name = "Chi nhánh Nậm Hàng", Type = "CNL2", ParentUnitId = 1 },
                    new Unit { Id = 46, Code = "BGDCNNH", Name = "Ban Giám đốc", Type = "PNVL2", ParentUnitId = 17 },
                    new Unit { Id = 47, Code = "PKTNQCNNH", Name = "Phòng Kế toán & Ngân quỹ", Type = "PNVL2", ParentUnitId = 17 },
                    new Unit { Id = 48, Code = "PKHCNNH", Name = "Phòng Khách hàng", Type = "PNVL2", ParentUnitId = 17 }
                };

                await _context.Units.AddRangeAsync(units);
                await _context.SaveChangesAsync();

                // Get statistics
                var typeStats = await _context.Units.GroupBy(u => u.Type)
                                                 .Select(g => new { Type = g.Key, Count = g.Count() })
                                                 .ToListAsync();

                var rootUnits = await _context.Units.Where(u => u.ParentUnitId == null).CountAsync();
                var branchUnits = await _context.Units.Where(u => u.Type == "CNL2").CountAsync();

                return Ok(new
                {
                    message = $"✅ Successfully created {units.Count} units!",
                    totalUnits = units.Count,
                    typeStatistics = typeStats,
                    rootUnits = rootUnits,
                    branchUnits = branchUnits
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message, details = ex.InnerException?.Message });
            }
        }
    }
}
