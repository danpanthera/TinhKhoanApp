using TinhKhoanApp.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("execute-sql")]
        public async Task<IActionResult> ExecuteSql([FromBody] string sql)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sql))
                {
                    return BadRequest("SQL command is required");
                }

                // Execute raw SQL
                var result = await _context.Database.ExecuteSqlRawAsync(sql);

                return Ok(new
                {
                    Success = true,
                    Message = $"SQL executed successfully. Rows affected: {result}",
                    RowsAffected = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Error executing SQL",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("populate-branch-indicators")]
        public async Task<IActionResult> PopulateBranchIndicators()
        {
            try
            {
                // SQL script để populate 9 bảng KPI chi nhánh
                var sql = @"
                    -- Xóa chỉ tiêu cũ của 9 bảng chi nhánh (ID 24-32)
                    DELETE FROM KpiIndicators WHERE KpiAssignmentTableId BETWEEN 24 AND 32;

                    -- Bảng 24: KPI_CnBinhLu
                    INSERT INTO KpiIndicators (KpiAssignmentTableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
                    (24, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
                    (24, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
                    (24, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
                    (24, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
                    (24, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
                    (24, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
                    (24, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
                    (24, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1);

                    -- Bảng 25: KPI_CnPhongTho
                    INSERT INTO KpiIndicators (KpiAssignmentTableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
                    (25, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
                    (25, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
                    (25, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
                    (25, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
                    (25, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
                    (25, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
                    (25, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
                    (25, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1);

                    -- Bảng 26: KPI_CnSinHo
                    INSERT INTO KpiIndicators (KpiAssignmentTableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
                    (26, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
                    (26, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
                    (26, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
                    (26, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
                    (26, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
                    (26, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
                    (26, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
                    (26, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1);

                    -- Bảng 27: KPI_CnBumTo
                    INSERT INTO KpiIndicators (KpiAssignmentTableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
                    (27, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
                    (27, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
                    (27, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
                    (27, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
                    (27, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
                    (27, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
                    (27, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
                    (27, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1);

                    -- Bảng 28: KPI_CnThanUyen
                    INSERT INTO KpiIndicators (KpiAssignmentTableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
                    (28, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
                    (28, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
                    (28, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
                    (28, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
                    (28, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
                    (28, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
                    (28, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
                    (28, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1);

                    -- Bảng 29: KPI_CnDoanKet
                    INSERT INTO KpiIndicators (KpiAssignmentTableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
                    (29, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
                    (29, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
                    (29, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
                    (29, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
                    (29, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
                    (29, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
                    (29, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
                    (29, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1);

                    -- Bảng 30: KPI_CnTanUyen
                    INSERT INTO KpiIndicators (KpiAssignmentTableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
                    (30, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
                    (30, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
                    (30, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
                    (30, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
                    (30, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
                    (30, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
                    (30, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
                    (30, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1);

                    -- Bảng 31: KPI_CnNamHang
                    INSERT INTO KpiIndicators (KpiAssignmentTableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
                    (31, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
                    (31, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
                    (31, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
                    (31, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
                    (31, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
                    (31, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
                    (31, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
                    (31, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1);

                    -- Bảng 32: KPI_HoiSo
                    INSERT INTO KpiIndicators (KpiAssignmentTableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
                    (32, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
                    (32, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
                    (32, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
                    (32, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
                    (32, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
                    (32, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
                    (32, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
                    (32, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1);
                ";

                var result = await _context.Database.ExecuteSqlRawAsync(sql);

                return Ok(new
                {
                    Success = true,
                    Message = "Successfully populated indicators for 9 branch KPI tables",
                    RowsAffected = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Error populating branch indicators",
                    Error = ex.Message
                });
            }
        }
    }
}
