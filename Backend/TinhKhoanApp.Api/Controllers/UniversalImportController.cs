using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Utils;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UniversalImportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UniversalImportController> _logger;

        public UniversalImportController(ApplicationDbContext context, ILogger<UniversalImportController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("import-csv/{tableName}")]
        public async Task<IActionResult> ImportCsv(string tableName, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var allowedTables = new[] { "DP01", "GL01", "GL41", "EI01", "DPDA", "RR01", "LN01", "LN03" };
                if (!allowedTables.Contains(tableName.ToUpper()))
                    return BadRequest($"Table {tableName} not supported.");

                _logger.LogInformation($"🔄 Starting universal import for table {tableName}");

                using var reader = new StreamReader(file.OpenReadStream());
                var csvContent = await reader.ReadToEndAsync();
                var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                if (lines.Length < 2)
                    return BadRequest("CSV must have header and at least one data row.");

                // Parse header
                var header = lines[0].Trim().Split(',').Select(h => h.Trim(' ', '"', '\uFEFF')).ToArray();
                _logger.LogInformation($"📊 CSV Headers: {string.Join(", ", header)}");

                // Build SQL bulk insert
                var connectionString = _context.Database.GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Create DataTable
                var dataTable = new DataTable();
                dataTable.Columns.Add("ImportedAt", typeof(DateTime));
                dataTable.Columns.Add("StatementDate", typeof(string));
                
                foreach (var col in header)
                {
                    dataTable.Columns.Add(col, typeof(string));
                }

                // Add common columns if not present
                if (!header.Contains("FILE_NAME"))
                    dataTable.Columns.Add("FILE_NAME", typeof(string));
                if (!header.Contains("NGAY_DL"))
                    dataTable.Columns.Add("NGAY_DL", typeof(string));

                // Parse data rows
                var importedCount = 0;
                var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
                
                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    var values = line.Split(',').Select(v => v.Trim(' ', '"')).ToArray();
                    
                    var row = dataTable.NewRow();
                    row["ImportedAt"] = DateTime.UtcNow;
                    row["StatementDate"] = today;
                    
                    // Map CSV values to columns
                    for (int j = 0; j < Math.Min(values.Length, header.Length); j++)
                    {
                        row[header[j]] = string.IsNullOrEmpty(values[j]) ? DBNull.Value : values[j];
                    }
                    
                    // Set common fields
                    row["FILE_NAME"] = file.FileName;
                    row["NGAY_DL"] = today;
                    
                    dataTable.Rows.Add(row);
                    importedCount++;
                }

                // Bulk insert
                using var bulkCopy = new SqlBulkCopy(connection)
                {
                    DestinationTableName = tableName.ToUpper(),
                    BatchSize = 1000,
                    BulkCopyTimeout = 300
                };

                // Map columns
                foreach (DataColumn column in dataTable.Columns)
                {
                    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }

                await bulkCopy.WriteToServerAsync(dataTable);

                _logger.LogInformation($"✅ Successfully imported {importedCount} rows to {tableName}");

                return Ok(new
                {
                    message = $"Successfully imported {importedCount} rows to {tableName}",
                    tableName = tableName,
                    rowsImported = importedCount,
                    fileName = file.FileName,
                    headers = header
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error importing to {tableName}: {ex.Message}");
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet("tables")]
        public IActionResult GetSupportedTables()
        {
            var tables = new[]
            {
                new { name = "DP01", description = "Tiền gửi" },
                new { name = "GL01", description = "Sổ cái giao dịch" },
                new { name = "GL41", description = "Số dư cuối kỳ" },
                new { name = "EI01", description = "Dịch vụ điện tử" },
                new { name = "DPDA", description = "Thẻ ATM" },
                new { name = "RR01", description = "Nợ xấu" },
                new { name = "LN01", description = "Tín dụng" },
                new { name = "LN03", description = "Cơ cấu lại nợ" }
            };

            return Ok(new { supportedTables = tables });
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "Universal Import API is working!", timestamp = DateTime.UtcNow });
        }
    }
}
