using TinhKhoanApp.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedKpiSimpleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SeedKpiSimpleController> _logger;

        public SeedKpiSimpleController(ApplicationDbContext context, ILogger<SeedKpiSimpleController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("create-standard-structure")]
        public async Task<IActionResult> CreateStandardKpiStructure()
        {
            try
            {
                _logger.LogInformation("=== Tạo cấu trúc KPI chuẩn 32 bảng ===");

                // 1. Reset current data
                _logger.LogInformation("🧹 Xóa dữ liệu KPI hiện tại...");
                _context.KpiIndicators.RemoveRange(_context.KpiIndicators);
                _context.KpiAssignmentTables.RemoveRange(_context.KpiAssignmentTables);
                await _context.SaveChangesAsync();

                // 2. Create 23 role tables + 9 branch tables
                var tables = new List<KpiAssignmentTable>();

                // 23 role tables (CANBO)
                var roleTables = new[]
                {
                    ("TruongphongKhdn", "Trưởng phòng KHDN"),
                    ("TruongphongKhcn", "Trưởng phòng KHCN"),
                    ("PhophongKhdn", "Phó phòng KHDN"),
                    ("PhophongKhcn", "Phó phòng KHCN"),
                    ("TruongphongKhqlrr", "Trưởng phòng Kế hoạch và Quản lý rủi ro"),
                    ("PhophongKhqlrr", "Phó phòng Kế hoạch và Quản lý rủi ro"),
                    ("Cbtd", "Cán bộ tín dụng"),
                    ("TruongphongKtnqCnl1", "Trưởng phòng Kế toán & Ngân quỹ CNL1"),
                    ("PhophongKtnqCnl1", "Phó phòng Kế toán & Ngân quỹ CNL1"),
                    ("Gdv", "Giao dịch viên"),
                    ("TqHkKtnb", "TQ/Hậu kiểm/Kế toán nội bộ"),
                    ("TruongphongItThKtgs", "Trưởng phòng IT/TH/KTGS"),
                    ("CbItThKtgsKhqlrr", "CB IT/TH/KTGS/KHQLRR"),
                    ("GiamdocPgd", "Giám đốc PGD"),
                    ("PhogiamdocPgd", "Phó giám đốc PGD"),
                    ("PhogiamdocPgdCbtd", "Phó giám đốc PGD kiêm CBTD"),
                    ("GiamdocCnl2", "Giám đốc CNL2"),
                    ("PhogiamdocCnl2Td", "Phó giám đốc CNL2 phụ trách Tín dụng"),
                    ("PhogiamdocCnl2Kt", "Phó giám đốc CNL2 phụ trách Kế toán"),
                    ("TruongphongKhCnl2", "Trưởng phòng KH CNL2"),
                    ("PhophongKhCnl2", "Phó phòng KH CNL2"),
                    ("TruongphongKtnqCnl2", "Trưởng phòng Kế toán & Ngân quỹ CNL2"),
                    ("PhophongKtnqCnl2", "Phó phòng Kế toán & Ngân quỹ CNL2")
                };

                foreach (var (tableName, description) in roleTables)
                {
                    tables.Add(new KpiAssignmentTable
                    {
                        TableName = tableName,
                        Description = $"Bảng KPI cho {description}",
                        Category = "CANBO",
                        TableType = KpiTableType.TruongphongKhdn, // Default, will be updated
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow
                    });
                }

                // 9 branch tables (CHINHANH)
                var branchTables = new[]
                {
                    ("HoiSo", "Hội sở"),
                    ("CnBinhLu", "Chi nhánh Bình Lư"),
                    ("CnPhongTho", "Chi nhánh Phong Thổ"),
                    ("CnSinHo", "Chi nhánh Sin Hồ"),
                    ("CnBumTo", "Chi nhánh Bum Tở"),
                    ("CnThanUyen", "Chi nhánh Than Uyên"),
                    ("CnDoanKet", "Chi nhánh Đoàn Kết"),
                    ("CnTanUyen", "Chi nhánh Tân Uyên"),
                    ("CnNamHang", "Chi nhánh Nậm Hàng")
                };

                foreach (var (tableName, description) in branchTables)
                {
                    tables.Add(new KpiAssignmentTable
                    {
                        TableName = tableName,
                        Description = $"Bảng KPI cho {description}",
                        Category = "CHINHANH",
                        TableType = KpiTableType.HoiSo, // Default, will be updated
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow
                    });
                }

                // Add all tables
                _context.KpiAssignmentTables.AddRange(tables);
                await _context.SaveChangesAsync();

                // 3. Create indicators for each table
                var indicators = new List<KpiIndicator>();
                foreach (var table in tables)
                {
                    // Determine number of indicators based on category and specific roles
                    int indicatorCount = GetIndicatorCountForTable(table.TableName);

                    for (int i = 1; i <= indicatorCount; i++)
                    {
                        indicators.Add(new KpiIndicator
                        {
                            TableId = table.Id,
                            IndicatorName = GetIndicatorNameForTable(table.TableName, i),
                            MaxScore = 100, // Điểm tối đa mặc định
                            Unit = "VND",
                            OrderIndex = i,
                            ValueType = KpiValueType.NUMBER,
                            IsActive = true
                        });
                    }
                }

                _context.KpiIndicators.AddRange(indicators);
                await _context.SaveChangesAsync();

                // 4. Get results
                var totalTables = await _context.KpiAssignmentTables.CountAsync();
                var totalIndicators = await _context.KpiIndicators.CountAsync();

                var summary = await _context.KpiAssignmentTables
                    .GroupBy(t => t.Category)
                    .Select(g => new
                    {
                        Category = g.Key,
                        Count = g.Count(),
                        TotalIndicators = _context.KpiIndicators.Count(i => g.Any(t => t.Id == i.TableId))
                    })
                    .ToListAsync();

                return Ok(new
                {
                    message = "Tạo cấu trúc KPI chuẩn thành công",
                    totalTables = totalTables,
                    totalIndicators = totalIndicators,
                    summary = summary,
                    structure = "23 vai trò CANBO + 9 chi nhánh CHINHANH",
                    timestamp = DateTime.UtcNow
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi tạo cấu trúc KPI chuẩn");
                return StatusCode(500, new { message = "Lỗi khi tạo cấu trúc KPI", error = ex.Message });
            }
        }

        private int GetIndicatorCountForTable(string tableName)
        {
            // Based on your requirements, specific indicator counts
            return tableName switch
            {
                "PhophongKhCnl2" => 11, // Bảng có nhiều chỉ tiêu nhất
                "CbItThKtgsKhqlrr" => 4, // Bảng có ít chỉ tiêu nhất
                "TruongphongItThKtgs" => 5,
                var name when name.StartsWith("Truongphong") && (name.Contains("Khdn") || name.Contains("Khcn")) => 8,
                var name when name.StartsWith("Phophong") && (name.Contains("Khdn") || name.Contains("Khcn")) => 8,
                var name when name.StartsWith("Giamdoc") || name.StartsWith("Phogiamdoc") => 9,
                var name when name.StartsWith("Cn") || name == "HoiSo" => 11, // Chi nhánh giống PhophongKhCnl2
                _ => 6 // Default for other roles
            };
        }

        private string GetIndicatorNameForTable(string tableName, int index)
        {
            var baseIndicators = new[]
            {
                "Doanh thu huy động",
                "Doanh thu cho vay",
                "Lãi suông",
                "Huy động vốn",
                "Cho vay khách hàng",
                "Quản lý nợ xấu",
                "Phát triển khách hàng",
                "Bán sản phẩm dịch vụ",
                "Hiệu suất công việc",
                "Chất lượng dịch vụ",
                "Tuân thủ quy định"
            };

            if (index <= baseIndicators.Length)
            {
                return baseIndicators[index - 1];
            }

            return $"Chỉ tiêu bổ sung {index - baseIndicators.Length}";
        }
    }
}
