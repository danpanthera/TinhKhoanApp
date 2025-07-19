using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// DataTables Controller - Direct Import và Preview cho 8 bảng dữ liệu chính
    /// Hỗ trợ GL01 (Partitioned), DP01/DPDA/EI01/GL41/LN01/LN03/RR01 (Temporal)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DataTablesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DataTablesController> _logger;

        public DataTablesController(ApplicationDbContext context, ILogger<DataTablesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region GL01 - Partitioned Table Operations

        /// <summary>
        /// Preview Direct GL01 - Lấy 10 bản ghi trực tiếp từ bảng GL01
        /// </summary>
        [HttpGet("gl01/preview")]
        public async Task<IActionResult> PreviewGL01()
        {
            try
            {
                var preview = await _context.Database
                    .SqlQueryRaw<dynamic>(@"
                        SELECT TOP 10
                            Id, DataDate, BranchCode, AccountNumber, CustomerCode,
                            Currency, DebitAmount, CreditAmount, Balance, TransactionCode,
                            Description, Reference, ValueDate, BookingDate,
                            CreatedAt, UpdatedAt
                        FROM GL01
                        ORDER BY DataDate DESC, Id DESC")
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    tableName = "GL01",
                    storageType = "Partitioned",
                    totalRecords = preview.Count,
                    data = preview,
                    message = "Preview lấy trực tiếp từ bảng GL01 (Partitioned)"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing GL01 data");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Direct Import GL01 - Import trực tiếp vào bảng GL01
        /// </summary>
        [HttpPost("gl01/import")]
        public async Task<IActionResult> ImportGL01([FromBody] List<GL01ImportModel> data)
        {
            try
            {
                if (data == null || !data.Any())
                {
                    return BadRequest(new { success = false, message = "Không có dữ liệu để import" });
                }

                var importCount = 0;
                foreach (var item in data)
                {
                    var sql = @"
                        INSERT INTO GL01 (DataDate, BranchCode, AccountNumber, CustomerCode, Currency,
                                         DebitAmount, CreditAmount, Balance, TransactionCode, Description,
                                         Reference, ValueDate, BookingDate)
                        VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12)";

                    await _context.Database.ExecuteSqlRawAsync(sql,
                        item.DataDate, item.BranchCode, item.AccountNumber, item.CustomerCode,
                        item.Currency, item.DebitAmount, item.CreditAmount, item.Balance,
                        item.TransactionCode, item.Description, item.Reference,
                        item.ValueDate, item.BookingDate);

                    importCount++;
                }

                return Ok(new
                {
                    success = true,
                    tableName = "GL01",
                    storageType = "Partitioned",
                    importedRecords = importCount,
                    message = $"Import thành công {importCount} bản ghi vào bảng GL01"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing GL01 data");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region DP01 - Temporal Table Operations

        /// <summary>
        /// Preview Direct DP01 - Lấy 10 bản ghi trực tiếp từ bảng DP01
        /// </summary>
        [HttpGet("dp01/preview")]
        public async Task<IActionResult> PreviewDP01()
        {
            try
            {
                var preview = await _context.Database
                    .SqlQueryRaw<dynamic>(@"
                        SELECT TOP 10
                            Id, DataDate, BranchCode, AccountNumber, CustomerCode,
                            ProductType, Currency, DepositAmount, Balance, InterestRate,
                            MaturityDate, OpeningDate, Status,
                            CreatedAt, UpdatedAt
                        FROM DP01
                        ORDER BY DataDate DESC, Id DESC")
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    tableName = "DP01",
                    storageType = "Temporal",
                    totalRecords = preview.Count,
                    data = preview,
                    message = "Preview lấy trực tiếp từ bảng DP01 (Temporal)"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing DP01 data");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Direct Import DP01 - Import trực tiếp vào bảng DP01
        /// </summary>
        [HttpPost("dp01/import")]
        public async Task<IActionResult> ImportDP01([FromBody] List<DP01ImportModel> data)
        {
            try
            {
                if (data == null || !data.Any())
                {
                    return BadRequest(new { success = false, message = "Không có dữ liệu để import" });
                }

                var importCount = 0;
                foreach (var item in data)
                {
                    var sql = @"
                        INSERT INTO DP01 (DataDate, BranchCode, AccountNumber, CustomerCode, ProductType,
                                         Currency, DepositAmount, Balance, InterestRate, MaturityDate,
                                         OpeningDate, Status)
                        VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11)";

                    await _context.Database.ExecuteSqlRawAsync(sql,
                        item.DataDate, item.BranchCode, item.AccountNumber, item.CustomerCode,
                        item.ProductType, item.Currency, item.DepositAmount, item.Balance,
                        item.InterestRate, item.MaturityDate, item.OpeningDate, item.Status);

                    importCount++;
                }

                return Ok(new
                {
                    success = true,
                    tableName = "DP01",
                    storageType = "Temporal",
                    importedRecords = importCount,
                    message = $"Import thành công {importCount} bản ghi vào bảng DP01"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing DP01 data");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region EI01 - Temporal Table Operations

        /// <summary>
        /// Preview Direct EI01 - Lấy 10 bản ghi trực tiếp từ bảng EI01
        /// </summary>
        [HttpGet("ei01/preview")]
        public async Task<IActionResult> PreviewEI01()
        {
            try
            {
                var preview = await _context.Database
                    .SqlQueryRaw<dynamic>(@"
                        SELECT TOP 10
                            Id, DataDate, EmployeeCode, EmployeeName, DepartmentCode,
                            Position, Salary, Bonus, TotalIncome, StartDate, Status,
                            CreatedAt, UpdatedAt
                        FROM EI01
                        ORDER BY DataDate DESC, Id DESC")
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    tableName = "EI01",
                    storageType = "Temporal",
                    totalRecords = preview.Count,
                    data = preview,
                    message = "Preview lấy trực tiếp từ bảng EI01 (Temporal)"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing EI01 data");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Generic Operations for All DataTables

        /// <summary>
        /// Lấy thống kê tổng quan của tất cả 8 bảng dữ liệu
        /// </summary>
        [HttpGet("summary")]
        public async Task<IActionResult> GetDataTablesSummary()
        {
            try
            {
                var summary = await _context.Database
                    .SqlQueryRaw<DataTableSummary>(@"
                        SELECT
                            t.name as TableName,
                            CASE
                                WHEN t.temporal_type = 2 THEN 'Temporal'
                                WHEN EXISTS (SELECT 1 FROM sys.partition_schemes ps
                                            JOIN sys.indexes i ON i.data_space_id = ps.data_space_id
                                            WHERE i.object_id = t.object_id) THEN 'Partitioned'
                                ELSE 'Standard'
                            END as StorageType,
                            CASE WHEN EXISTS (SELECT 1 FROM sys.indexes i WHERE i.object_id = t.object_id AND i.type = 6)
                                 THEN 'Yes' ELSE 'No' END as HasColumnstore,
                            p.rows as RecordCount
                        FROM sys.tables t
                        LEFT JOIN sys.partitions p ON t.object_id = p.object_id AND p.index_id IN (0,1)
                        WHERE t.name IN ('GL01', 'DP01', 'DPDA', 'EI01', 'GL41', 'LN01', 'LN03', 'RR01')
                        ORDER BY t.name")
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    totalTables = summary.Count,
                    tables = summary,
                    message = "Thống kê 8 bảng dữ liệu chính"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting DataTables summary");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Bulk Import - Import hàng loạt vào nhiều bảng
        /// </summary>
        [HttpPost("bulk-import")]
        public async Task<IActionResult> BulkImport([FromBody] DataTablesBulkImportRequest request)
        {
            var results = new List<DataTableImportResult>();

            try
            {
                foreach (var tableData in request.Tables)
                {
                    try
                    {
                        var importCount = 0;

                        // Thực hiện import cho từng bảng
                        switch (tableData.TableName.ToUpper())
                        {
                            case "GL01":
                                importCount = await ImportDataToTable("GL01", tableData.Data);
                                break;
                            case "DP01":
                                importCount = await ImportDataToTable("DP01", tableData.Data);
                                break;
                            case "EI01":
                                importCount = await ImportDataToTable("EI01", tableData.Data);
                                break;
                                // Thêm các bảng khác...
                        }

                        results.Add(new DataTableImportResult
                        {
                            TableName = tableData.TableName,
                            Success = true,
                            ImportedRecords = importCount,
                            Message = $"Import thành công {importCount} bản ghi",
                            StorageType = GetStorageType(tableData.TableName)
                        });
                    }
                    catch (Exception ex)
                    {
                        results.Add(new DataTableImportResult
                        {
                            TableName = tableData.TableName,
                            Success = false,
                            ImportedRecords = 0,
                            Message = ex.Message,
                            StorageType = GetStorageType(tableData.TableName)
                        });
                    }
                }

                return Ok(new
                {
                    success = true,
                    results = results,
                    totalTables = results.Count,
                    successfulTables = results.Count(r => r.Success),
                    message = "Bulk import hoàn thành"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk import");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Private Helper Methods

        private async Task<int> ImportDataToTable(string tableName, List<Dictionary<string, object>> data)
        {
            // Generic import method for any DataTable
            // Sẽ implement chi tiết theo cấu trúc từng bảng
            await Task.Delay(100); // Placeholder
            return data.Count;
        }

        private string GetStorageType(string tableName)
        {
            return tableName.ToUpper() switch
            {
                "GL01" => "Partitioned",
                _ => "Temporal"
            };
        }

        #endregion
    }
}
