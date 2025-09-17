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

        #region LN01 - Temporal Table Operations

        /// <summary>
        /// Preview Direct LN01 - Lấy 10 bản ghi trực tiếp từ bảng LN01 theo thứ tự CSV gốc
        /// </summary>
        [HttpGet("ln01/preview")]
        public async Task<IActionResult> PreviewLN01()
        {
            try
            {
                // SELECT theo thứ tự columns CSV gốc: NGAY_DL + 79 business columns + system columns
                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT TOP 10
                        NGAY_DL,
                        BRCD, CUSTSEQ, CUSTNM, TAI_KHOAN, CCY, DU_NO, DSBSSEQ, TRANSACTION_DATE, DSBSDT, DISBUR_CCY,
                        DISBURSEMENT_AMOUNT, DSBSMATDT, BSRTCD, INTEREST_RATE, APPRSEQ, APPRDT, APPR_CCY, APPRAMT, APPRMATDT,
                        LOAN_TYPE, FUND_RESOURCE_CODE, FUND_PURPOSE_CODE, REPAYMENT_AMOUNT, NEXT_REPAY_DATE, NEXT_REPAY_AMOUNT,
                        NEXT_INT_REPAY_DATE, OFFICER_ID, OFFICER_NAME, INTEREST_AMOUNT, PASTDUE_INTEREST_AMOUNT, TOTAL_INTEREST_REPAY_AMOUNT,
                        CUSTOMER_TYPE_CODE, CUSTOMER_TYPE_CODE_DETAIL, TRCTCD, TRCTNM, ADDR1, PROVINCE, LCLPROVINNM, DISTRICT,
                        LCLDISTNM, COMMCD, LCLWARDNM, LAST_REPAY_DATE, SECURED_PERCENT, NHOM_NO, LAST_INT_CHARGE_DATE, EXEMPTINT,
                        EXEMPTINTTYPE, EXEMPTINTAMT, GRPNO, BUSCD, BUSNM, ACCRINTBASE, RATECD, RLSCD, RLSNM, INTERESTTYPE,
                        FEETYPE, CURRENCYTYPE, LOANSTATUS, MATURITYTYPE, REPAYTYPE, COLLATERALTYPE, CREDITLIMIT, AVAILABLELIMIT,
                        LOANORIGINALAMT, DOWNPAYMENT, INSTALMENTAMT, TOTALPAIDAMT, CHARGEOFFAMT, WRITEOFFAMT, RESERVEAMT,
                        ACCRUALBALANCE, OUTSRC, ORIGINAL_TRANS_ID, CREATED_DATE_CSV, UPDATED_DATE_CSV,
                        Id, FILE_NAME, CREATED_DATE, UPDATED_DATE
                    FROM LN01
                    ORDER BY Id DESC";

                var result = new List<Dictionary<string, object>>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var value = reader.GetValue(i);
                        row[reader.GetName(i)] = value == DBNull.Value ? null : value;
                    }
                    result.Add(row);
                }

                return Ok(new
                {
                    success = true,
                    tableName = "LN01",
                    storageType = "Temporal",
                    totalRecords = result.Count,
                    data = result,
                    message = "Preview LN01 - columns theo thứ tự CSV gốc: NGAY_DL + 79 business + system"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing LN01 data");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region LN03 - Temporal Table Operations

        /// <summary>
        /// Preview Direct LN03 - Lấy 10 bản ghi trực tiếp từ bảng LN03 theo thứ tự business columns
        /// </summary>
        [HttpGet("ln03/preview")]
        public async Task<IActionResult> PreviewLN03()
        {
            try
            {
                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT TOP 10 *
                    FROM LN03
                    ORDER BY Id DESC";

                var result = new List<Dictionary<string, object>>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var value = reader.GetValue(i);
                        row[reader.GetName(i)] = value == DBNull.Value ? null : value;
                    }
                    result.Add(row);
                }

                return Ok(new
                {
                    success = true,
                    tableName = "LN03",
                    storageType = "Temporal",
                    totalRecords = result.Count,
                    data = result,
                    message = "Preview LN03 - tất cả business columns theo thứ tự CSV"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing LN03 data");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region RR01 - Temporal Table Operations

        /// <summary>
        /// Preview Direct RR01 - Lấy 10 bản ghi trực tiếp từ bảng RR01 theo thứ tự business columns
        /// </summary>
        [HttpGet("rr01/preview")]
        public async Task<IActionResult> PreviewRR01()
        {
            try
            {
                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT TOP 10 *
                    FROM RR01
                    ORDER BY Id DESC";

                var result = new List<Dictionary<string, object>>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var value = reader.GetValue(i);
                        row[reader.GetName(i)] = value == DBNull.Value ? null : value;
                    }
                    result.Add(row);
                }

                return Ok(new
                {
                    success = true,
                    tableName = "RR01",
                    storageType = "Temporal",
                    totalRecords = result.Count,
                    data = result,
                    message = "Preview RR01 - tất cả business columns theo thứ tự CSV"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing RR01 data");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Generic Operations for All DataTables

        /// <summary>
        /// Lấy số lượng records của tất cả 8 bảng dữ liệu
        /// </summary>
        [HttpGet("table-counts")]
        public async Task<IActionResult> GetTableCounts()
        {
            try
            {
                var counts = new Dictionary<string, int>();

                // Count records in each table
                var tables = new[] { "DP01", "DPDA", "EI01", "GL01", "GL41", "LN01", "LN03", "RR01" };

                foreach (var table in tables)
                {
                    try
                    {
                        var countQuery = $"SELECT COUNT(*) AS Value FROM [{table}]";
                        var count = await _context.Database
                            .SqlQueryRaw<int>(countQuery)
                            .FirstOrDefaultAsync();
                        counts[table] = count;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Error counting {Table}: {Error}", table, ex.Message);
                        counts[table] = 0;
                    }
                }

                return Ok(counts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting table counts");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

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
        public async Task<ActionResult<DataTableImportResult>> BulkImport([FromBody] DataTablesBulkImportRequest request)
        {
            try
            {
                var results = new List<DataTableSummary>();
                int totalProcessed = 0;
                int totalErrors = 0;

                foreach (var tableData in request.Tables)
                {
                    try
                    {
                        // Process each table data
                        switch (tableData.TableName?.ToUpper())
                        {
                            case "DP01":
                                var dp01Data = ConvertToDP01Models(tableData.Data);
                                await ProcessDP01Data(dp01Data);
                                totalProcessed += dp01Data.Count;
                                results.Add(new DataTableSummary
                                {
                                    TableName = "DP01",
                                    RecordCount = dp01Data.Count,
                                    Status = "Success"
                                });
                                break;

                            case "LN01":
                                var ln01Data = ConvertToLN01Models(tableData.Data);
                                await ProcessLN01Data(ln01Data);
                                totalProcessed += ln01Data.Count;
                                results.Add(new DataTableSummary
                                {
                                    TableName = "LN01",
                                    RecordCount = ln01Data.Count,
                                    Status = "Success"
                                });
                                break;

                            default:
                                results.Add(new DataTableSummary
                                {
                                    TableName = tableData.TableName ?? "Unknown",
                                    RecordCount = 0,
                                    Status = $"Table {tableData.TableName} not supported"
                                });
                                totalErrors++;
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        results.Add(new DataTableSummary
                        {
                            TableName = tableData.TableName ?? "Unknown",
                            RecordCount = 0,
                            Status = $"Error: {ex.Message}"
                        });
                        totalErrors++;
                    }
                }

                return Ok(new DataTableImportResult
                {
                    Success = totalErrors == 0,
                    TotalRecordsProcessed = totalProcessed,
                    ErrorCount = totalErrors,
                    Message = totalErrors == 0 ? "All tables imported successfully" : $"Completed with {totalErrors} errors",
                    TableSummaries = results
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new DataTableImportResult
                {
                    Success = false,
                    TotalRecordsProcessed = 0,
                    ErrorCount = 1,
                    Message = $"Bulk import failed: {ex.Message}",
                    TableSummaries = new List<DataTableSummary>()
                });
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

        // Helper methods for data conversion and processing
        private List<DP01> ConvertToDP01Models(List<Dictionary<string, object>> data)
        {
            var result = new List<DP01>();
            foreach (var item in data)
            {
                var dp01 = new DP01();

                // Map properties from dictionary to DP01 model (using actual CSV column names)
                if (item.ContainsKey("MA_CN") && item["MA_CN"] != null)
                    dp01.MA_CN = item["MA_CN"].ToString();

                if (item.ContainsKey("TAI_KHOAN_HACH_TOAN") && item["TAI_KHOAN_HACH_TOAN"] != null)
                    dp01.TAI_KHOAN_HACH_TOAN = item["TAI_KHOAN_HACH_TOAN"].ToString();

                if (item.ContainsKey("MA_KH") && item["MA_KH"] != null)
                    dp01.MA_KH = item["MA_KH"].ToString();

                if (item.ContainsKey("TEN_KH") && item["TEN_KH"] != null)
                    dp01.TEN_KH = item["TEN_KH"].ToString();

                if (item.ContainsKey("CURRENT_BALANCE") && item["CURRENT_BALANCE"] != null)
                    dp01.CURRENT_BALANCE = Convert.ToDecimal(item["CURRENT_BALANCE"]);

                if (item.ContainsKey("DP_TYPE_NAME") && item["DP_TYPE_NAME"] != null)
                    dp01.DP_TYPE_NAME = item["DP_TYPE_NAME"].ToString();

                if (item.ContainsKey("SO_TAI_KHOAN") && item["SO_TAI_KHOAN"] != null)
                    dp01.SO_TAI_KHOAN = item["SO_TAI_KHOAN"].ToString();

                if (item.ContainsKey("OPENING_DATE") && item["OPENING_DATE"] != null)
                    dp01.OPENING_DATE = Convert.ToDateTime(item["OPENING_DATE"]);

                if (item.ContainsKey("FILE_NAME") && item["FILE_NAME"] != null)
                    dp01.FILE_NAME = item["FILE_NAME"].ToString() ?? string.Empty;

                // Set system fields
                dp01.CreatedAt = DateTime.UtcNow;
                dp01.UpdatedAt = DateTime.UtcNow;

                result.Add(dp01);
            }
            return result;
        }

        private List<LN01> ConvertToLN01Models(List<Dictionary<string, object>> data)
        {
            var result = new List<LN01>();
            foreach (var item in data)
            {
                var ln01 = new LN01();

                // Map properties from dictionary to LN01 model (using actual CSV column names)
                if (item.ContainsKey("BRCD") && item["BRCD"] != null)
                    ln01.BRCD = item["BRCD"].ToString();

                if (item.ContainsKey("CUSTSEQ") && item["CUSTSEQ"] != null)
                    ln01.CUSTSEQ = item["CUSTSEQ"].ToString();

                if (item.ContainsKey("CUSTNM") && item["CUSTNM"] != null)
                    ln01.CUSTNM = item["CUSTNM"].ToString();

                if (item.ContainsKey("TAI_KHOAN") && item["TAI_KHOAN"] != null)
                    ln01.TAI_KHOAN = item["TAI_KHOAN"].ToString();

                if (item.ContainsKey("DU_NO") && item["DU_NO"] != null)
                    ln01.DU_NO = Convert.ToDecimal(item["DU_NO"]);

                if (item.ContainsKey("TRANSACTION_DATE") && item["TRANSACTION_DATE"] != null)
                    ln01.TRANSACTION_DATE = Convert.ToDateTime(item["TRANSACTION_DATE"]);

                // Set system fields
                ln01.CreatedAt = DateTime.UtcNow;
                ln01.UpdatedAt = DateTime.UtcNow;

                result.Add(ln01);
            }
            return result;
        }

        private async Task ProcessDP01Data(List<DP01> data)
        {
            if (data == null || !data.Any()) return;

            try
            {
                _context.DP01.AddRange(data);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to process DP01 data: {ex.Message}", ex);
            }
        }

        private async Task ProcessLN01Data(List<LN01> data)
        {
            if (data == null || !data.Any()) return;

            try
            {
                _context.LN01.AddRange(data);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to process LN01 data: {ex.Message}", ex);
            }
        }

        #endregion
    }
}
