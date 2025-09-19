using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Khoan.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataImportController : ControllerBase
    {
        private readonly string _connStr;

        public DataImportController(IConfiguration configuration)
        {
            _connStr = configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Missing DefaultConnection");
        }

        public record ImportRecordDto(
            int Id,
            string FileName,
            string FileType,
            string Category,
            string Status,
            DateTime ImportDate,
            DateTime? StatementDate,
            int RecordsCount,
            string ImportedBy,
            string? Notes
        );

        [HttpGet("records")]
        public async Task<IActionResult> GetRecords([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? category = null, [FromQuery] string? status = null, [FromQuery] string? search = null)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 200) pageSize = 20;

            using var conn = new SqlConnection(_connStr);
            await conn.OpenAsync();

            // Build WHERE clause safely with parameters
            var where = new List<string>();
            var cmd = conn.CreateCommand();

            if (!string.IsNullOrWhiteSpace(category))
            {
                where.Add("Category = @category");
                cmd.Parameters.AddWithValue("@category", category);
            }
            if (!string.IsNullOrWhiteSpace(status))
            {
                where.Add("Status = @status");
                cmd.Parameters.AddWithValue("@status", status);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                where.Add("(FileName LIKE @search OR Notes LIKE @search)");
                cmd.Parameters.AddWithValue("@search", $"%{search.Trim()}%");
            }

            var whereSql = where.Count > 0 ? ("WHERE " + string.Join(" AND ", where)) : string.Empty;

            // Count query
            cmd.CommandText = $"SELECT COUNT(*) FROM ImportedDataRecords {whereSql}";
            var totalObj = await cmd.ExecuteScalarAsync();
            var total = Convert.ToInt32(totalObj ?? 0);

            // Items query
            var skip = (page - 1) * pageSize;
            cmd.Parameters.AddWithValue("@skip", skip);
            cmd.Parameters.AddWithValue("@take", pageSize);
            cmd.CommandText = $@"
SELECT Id, FileName, FileType, Category, Status, ImportDate, StatementDate, RecordsCount, ImportedBy, Notes
FROM ImportedDataRecords
{whereSql}
ORDER BY ImportDate DESC
OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY";

            var items = new List<ImportRecordDto>();
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    items.Add(new ImportRecordDto(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetString(4),
                        reader.GetDateTime(5),
                        reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                        reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                        reader.GetString(8),
                        reader.IsDBNull(9) ? null : reader.GetString(9)
                    ));
                }
            }

            return Ok(new { items, total, page, pageSize });
        }
    }
}
