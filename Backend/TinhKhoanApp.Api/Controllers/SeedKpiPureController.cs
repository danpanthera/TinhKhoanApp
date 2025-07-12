using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedKpiPureController : ControllerBase
    {
        private readonly string _connectionString;

        public SeedKpiPureController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        [HttpPost("create-full-structure")]
        public async Task<IActionResult> CreateFullKpiStructure()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // 1. Xóa dữ liệu cũ
                using (var deleteCmd = new SqlCommand("DELETE FROM KpiIndicators; DELETE FROM KpiAssignmentTables;", connection))
                {
                    await deleteCmd.ExecuteNonQueryAsync();
                }

                // 2. Tạo 23 bảng KPI cho cán bộ với chỉ tiêu riêng
                var roleKpiTables = new[]
                {
                    ("TruongphongKhdn", "Trưởng phòng KHDN", 8),
                    ("TruongphongKhcn", "Trưởng phòng KHCN", 8),
                    ("PhophongKhdn", "Phó phòng KHDN", 8),
                    ("PhophongKhcn", "Phó phòng KHCN", 8),
                    ("TruongphongKhqlrr", "Trưởng phòng KH&QLRR", 6),
                    ("PhophongKhqlrr", "Phó phòng KH&QLRR", 6),
                    ("Cbtd", "Cán bộ tín dụng", 8),
                    ("TruongphongKtnqCnl1", "Trưởng phòng KTNQ CNL1", 6),
                    ("PhophongKtnqCnl1", "Phó phòng KTNQ CNL1", 6),
                    ("Gdv", "GDV", 6),
                    ("TqHkKtnb", "Thủ quỹ | Hậu kiểm | KTNB", 6),
                    ("TruongphoItThKtgs", "Trưởng phó IT | Tổng hợp | KTGS", 5),
                    ("CBItThKtgsKhqlrr", "Cán bộ IT | Tổng hợp | KTGS | KH&QLRR", 4),
                    ("GiamdocPgd", "Giám đốc Phòng giao dịch", 9),
                    ("PhogiamdocPgd", "Phó giám đốc Phòng giao dịch", 9),
                    ("PhogiamdocPgdCbtd", "Phó giám đốc PGD kiêm CBTD", 8),
                    ("GiamdocCnl2", "Giám đốc CNL2", 11),
                    ("PhogiamdocCnl2Td", "Phó giám đốc CNL2 phụ trách TD", 8),
                    ("PhogiamdocCnl2Kt", "Phó giám đốc CNL2 phụ trách KT", 6),
                    ("TruongphongKhCnl2", "Trưởng phòng KH CNL2", 9),
                    ("PhophongKhCnl2", "Phó phòng KH CNL2", 8),
                    ("TruongphongKtnqCnl2", "Trưởng phòng KTNQ CNL2", 6),
                    ("PhophongKtnqCnl2", "Phó phòng KTNQ CNL2", 5)
                };

                // 3. Tạo 9 bảng KPI cho chi nhánh (11 chỉ tiêu mỗi bảng)
                var branchKpiTables = new[]
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

                int totalIndicators = 0;

                // Tạo bảng KPI cho cán bộ
                foreach (var (tableName, description, indicatorCount) in roleKpiTables)
                {
                    // Tạo KpiAssignmentTable
                    var insertTableSql = @"
                        INSERT INTO KpiAssignmentTables (TableName, Description, Category, TableType, IsActive, CreatedDate)
                        OUTPUT INSERTED.Id
                        VALUES (@TableName, @Description, 'CANBO', 'TruongphongKhdn', 1, GETDATE())";

                    int insertedTableId;
                    using (var tableCommand = new SqlCommand(insertTableSql, connection))
                    {
                        tableCommand.Parameters.AddWithValue("@TableName", tableName);
                        tableCommand.Parameters.AddWithValue("@Description", $"Bảng KPI cho {description}");
                        insertedTableId = Convert.ToInt32(await tableCommand.ExecuteScalarAsync());
                    }

                    // Tạo KpiIndicators cho bảng này
                    for (int i = 1; i <= indicatorCount; i++)
                    {
                        var insertIndicatorSql = @"
                            INSERT INTO KpiIndicators (TableId, IndicatorCode, IndicatorName, Description, Unit, IsActive, CreatedAt, UpdatedAt)
                            VALUES (@TableId, @IndicatorCode, @IndicatorName, @Description, @Unit, 1, GETDATE(), GETDATE())";

                        using var indicatorCommand = new SqlCommand(insertIndicatorSql, connection);
                        indicatorCommand.Parameters.AddWithValue("@TableId", insertedTableId);
                        indicatorCommand.Parameters.AddWithValue("@IndicatorCode", $"IND_{tableName}_{i:D2}");
                        indicatorCommand.Parameters.AddWithValue("@IndicatorName", $"Chỉ tiêu {i} cho {description}");
                        indicatorCommand.Parameters.AddWithValue("@Description", $"Mô tả chỉ tiêu {i} - {description}");
                        indicatorCommand.Parameters.AddWithValue("@Unit", "VND");

                        await indicatorCommand.ExecuteNonQueryAsync();
                        totalIndicators++;
                    }
                }

                // Tạo bảng KPI cho chi nhánh (11 chỉ tiêu mỗi bảng)
                foreach (var (tableName, description) in branchKpiTables)
                {
                    // Tạo KpiAssignmentTable
                    var insertTableSql = @"
                        INSERT INTO KpiAssignmentTables (TableName, Description, Category, TableType, IsActive, CreatedDate)
                        OUTPUT INSERTED.Id
                        VALUES (@TableName, @Description, 'CHINHANH', 'HoiSo', 1, GETDATE())";

                    int insertedTableId;
                    using (var tableCommand = new SqlCommand(insertTableSql, connection))
                    {
                        tableCommand.Parameters.AddWithValue("@TableName", tableName);
                        tableCommand.Parameters.AddWithValue("@Description", $"Bảng KPI cho {description}");
                        insertedTableId = Convert.ToInt32(await tableCommand.ExecuteScalarAsync());
                    }

                    // Tạo 11 KpiIndicators cho bảng này (giống GiamdocCnl2)
                    for (int i = 1; i <= 11; i++)
                    {
                        var insertIndicatorSql = @"
                            INSERT INTO KpiIndicators (TableId, IndicatorCode, IndicatorName, Description, Unit, IsActive, CreatedAt, UpdatedAt)
                            VALUES (@TableId, @IndicatorCode, @IndicatorName, @Description, @Unit, 1, GETDATE(), GETDATE())";

                        using var indicatorCommand = new SqlCommand(insertIndicatorSql, connection);
                        indicatorCommand.Parameters.AddWithValue("@TableId", insertedTableId);
                        indicatorCommand.Parameters.AddWithValue("@IndicatorCode", $"IND_{tableName}_{i:D2}");
                        indicatorCommand.Parameters.AddWithValue("@IndicatorName", $"Chỉ tiêu {i} cho {description}");
                        indicatorCommand.Parameters.AddWithValue("@Description", $"Mô tả chỉ tiêu {i} - {description}");
                        indicatorCommand.Parameters.AddWithValue("@Unit", "VND");

                        await indicatorCommand.ExecuteNonQueryAsync();
                        totalIndicators++;
                    }
                }

                // 4. Thống kê kết quả
                int tableCount, indicatorDbCount;
                using (var countCommand = new SqlCommand("SELECT COUNT(*) FROM KpiAssignmentTables", connection))
                {
                    tableCount = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
                }

                using (var countCommand = new SqlCommand("SELECT COUNT(*) FROM KpiIndicators", connection))
                {
                    indicatorDbCount = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
                }

                var summary = new
                {
                    Success = true,
                    TotalTables = tableCount,
                    TotalIndicators = indicatorDbCount,
                    CanBoTables = 23,
                    ChiNhanhTables = 9,
                    ExpectedIndicators = 23 * 7 + 9 * 11, // Tính toán dự kiến
                    ActualIndicators = totalIndicators,
                    Message = "Tạo thành công cấu trúc KPI 32 bảng",
                    Details = new
                    {
                        RoleBasedTables = "23 bảng cho các vai trò cán bộ",
                        BranchBasedTables = "9 bảng cho các chi nhánh",
                        DatabaseSchema = "Sử dụng SqlCommand trực tiếp"
                    }
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    message = "Lỗi khi tạo cấu trúc KPI",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
    }
}
