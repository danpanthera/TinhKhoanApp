using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedKpiTestController : ControllerBase
    {
        private readonly string _connectionString;

        public SeedKpiTestController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        [HttpPost("test-simple")]
        public async Task<IActionResult> TestSimpleKpiCreation()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Test tạo 1 bảng KPI
                var sql = @"
                    INSERT INTO KpiAssignmentTables (TableName, Description, Category, TableType, IsActive, CreatedDate)
                    VALUES (@TableName, @Description, @Category, @TableType, @IsActive, GETDATE())";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@TableName", "TestTable1");
                command.Parameters.AddWithValue("@Description", "Test Description 1");
                command.Parameters.AddWithValue("@Category", "CANBO");
                command.Parameters.AddWithValue("@TableType", "TruongphongKhdn");
                command.Parameters.AddWithValue("@IsActive", true);

                var result = await command.ExecuteNonQueryAsync();

                return Ok(new
                {
                    message = "Test thành công",
                    rowsAffected = result,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi test", error = ex.Message });
            }
        }

        [HttpPost("test-with-output")]
        public async Task<IActionResult> TestWithOutput()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Test tạo bảng với OUTPUT
                var sql = @"
                    INSERT INTO KpiAssignmentTables (TableName, Description, Category, TableType, IsActive, CreatedDate)
                    OUTPUT INSERTED.Id
                    VALUES (@TableName, @Description, @Category, @TableType, @IsActive, GETDATE())";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@TableName", "TestTable2");
                command.Parameters.AddWithValue("@Description", "Test Description 2");
                command.Parameters.AddWithValue("@Category", "CANBO");
                command.Parameters.AddWithValue("@TableType", "TruongphongKhdn");
                command.Parameters.AddWithValue("@IsActive", true);

                var insertedId = await command.ExecuteScalarAsync();

                return Ok(new
                {
                    message = "Test OUTPUT thành công",
                    insertedId = Convert.ToInt32(insertedId),
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi test OUTPUT", error = ex.Message });
            }
        }
    }
}
