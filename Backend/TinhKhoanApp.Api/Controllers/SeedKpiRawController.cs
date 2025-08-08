using TinhKhoanApp.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedKpiRawController : ControllerBase
    {
        private readonly string _connectionString;

        public SeedKpiRawController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        [HttpPost("create-structure-raw")]
        public async Task<IActionResult> CreateKpiStructureRaw()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // 1. Xóa dữ liệu cũ
                await connection.ExecuteNonQueryAsync("DELETE FROM KpiIndicators;");
                await connection.ExecuteNonQueryAsync("DELETE FROM KpiAssignmentTables;");

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

                // Tạo bảng KPI cho cán bộ
                foreach (var (tableName, description, indicatorCount) in roleKpiTables)
                {                    // Tạo KpiAssignmentTable và lấy ID được tạo
                    var insertTableSql = @"
                        INSERT INTO KpiAssignmentTables (TableName, Description, Category, TableType, IsActive, CreatedDate)
                        OUTPUT INSERTED.Id
                        VALUES (@TableName, @Description, 'CANBO', 'TruongphongKhdn', 1, GETDATE())";

                    using var tableCommand = new SqlCommand(insertTableSql, connection);
                    tableCommand.Parameters.AddWithValue("@TableName", tableName);
                    tableCommand.Parameters.AddWithValue("@Description", $"Bảng KPI cho {description}");

                    var insertedTableId = Convert.ToInt32(await tableCommand.ExecuteScalarAsync());

                    // Tạo KpiIndicators cho bảng này
                    for (int i = 1; i <= indicatorCount; i++)
                    {
                        var insertIndicatorSql = @"
                            INSERT INTO KpiIndicators (TableId, IndicatorCode, IndicatorName, Description, Unit, IsActive, CreatedAt, UpdatedAt)
                            VALUES (@TableId, @IndicatorCode, @IndicatorName, @Description, @Unit, 1, GETDATE(), GETDATE())";

                        await connection.ExecuteNonQueryAsync(insertIndicatorSql, new
                        {
                            TableId = insertedTableId,
                            IndicatorCode = $"IND_{tableName}_{i:D2}",
                            IndicatorName = $"Chỉ tiêu {i} cho {description}",
                            Description = $"Mô tả chỉ tiêu {i} - {description}",
                            Unit = "VND"
                        });
                    }
                }

                // Tạo bảng KPI cho chi nhánh (11 chỉ tiêu mỗi bảng)
                foreach (var (tableName, description) in branchKpiTables)
                {
                    // Tạo KpiAssignmentTable và lấy ID được tạo
                    var insertTableSql = @"
                        INSERT INTO KpiAssignmentTables (TableName, Description, Category, TableType, IsActive, CreatedDate)
                        OUTPUT INSERTED.Id
                        VALUES (@TableName, @Description, 'CHINHANH', 'HoiSo', 1, GETDATE())";

                    var insertedTableId = await connection.ExecuteScalarAsync<int>(insertTableSql, new
                    {
                        TableName = tableName,
                        Description = $"Bảng KPI cho {description}"
                    });

                    // Tạo 11 KpiIndicators cho bảng này (giống GiamdocCnl2)
                    for (int i = 1; i <= 11; i++)
                    {
                        var insertIndicatorSql = @"
                            INSERT INTO KpiIndicators (TableId, IndicatorCode, IndicatorName, Description, Unit, IsActive, CreatedAt, UpdatedAt)
                            VALUES (@TableId, @IndicatorCode, @IndicatorName, @Description, @Unit, 1, GETDATE(), GETDATE())";

                        await connection.ExecuteNonQueryAsync(insertIndicatorSql, new
                        {
                            TableId = insertedTableId,
                            IndicatorCode = $"IND_{tableName}_{i:D2}",
                            IndicatorName = $"Chỉ tiêu {i} cho {description}",
                            Description = $"Mô tả chỉ tiêu {i} - {description}",
                            Unit = "VND"
                        });
                    }
                }

                // 4. Thống kê kết quả
                var tableCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM KpiAssignmentTables");
                var indicatorTotalCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM KpiIndicators");

                var summary = new
                {
                    TotalTables = tableCount,
                    TotalIndicators = indicatorTotalCount,
                    CanBoTables = 23,
                    ChiNhanhTables = 9,
                    Message = "Tạo thành công cấu trúc KPI với Raw SQL",
                    Details = new
                    {
                        TableStructure = "32 bảng KPI (23 cán bộ + 9 chi nhánh)",
                        IndicatorStructure = "Tổng chỉ tiêu theo thiết kế",
                        DatabaseSchema = "Sử dụng cấu trúc database hiện tại"
                    }
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi tạo cấu trúc KPI", error = ex.Message });
            }
        }
    }

    // Extension method để thực hiện SQL commands
    public static class SqlConnectionExtensions
    {
        public static async Task<int> ExecuteNonQueryAsync(this SqlConnection connection, string sql, object? parameters = null)
        {
            using var command = new SqlCommand(sql, connection);

            if (parameters != null)
            {
                var props = parameters.GetType().GetProperties();
                foreach (var prop in props)
                {
                    command.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(parameters) ?? DBNull.Value);
                }
            }

            return await command.ExecuteNonQueryAsync();
        }

        public static async Task<T> ExecuteScalarAsync<T>(this SqlConnection connection, string sql, object? parameters = null)
        {
            using var command = new SqlCommand(sql, connection);

            if (parameters != null)
            {
                var props = parameters.GetType().GetProperties();
                foreach (var prop in props)
                {
                    command.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(parameters) ?? DBNull.Value);
                }
            }

            var result = await command.ExecuteScalarAsync();
            return (T)Convert.ChangeType(result!, typeof(T));
        }
    }
}
