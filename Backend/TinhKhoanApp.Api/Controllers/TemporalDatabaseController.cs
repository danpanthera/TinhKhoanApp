using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemporalDatabaseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TemporalDatabaseController> _logger;

        public TemporalDatabaseController(ApplicationDbContext context, ILogger<TemporalDatabaseController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/TemporalDatabase/check-status - Kiểm tra trạng thái Temporal Tables + Columnstore
        [HttpGet("check-status")]
        public async Task<IActionResult> CheckTemporalTablesStatus()
        {
            try
            {
                _logger.LogInformation("🔍 Kiểm tra toàn bộ trạng thái Temporal Tables + Columnstore Indexes");

                // 1. Kiểm tra Temporal Tables
                var temporalTablesQuery = @"
                    SELECT
                        t.name AS TableName,
                        CASE t.temporal_type
                            WHEN 0 THEN 'Non-temporal'
                            WHEN 1 THEN 'History table'
                            WHEN 2 THEN 'System-versioned temporal'
                        END AS TemporalType,
                        h.name AS HistoryTableName,
                        CASE WHEN t.history_retention_period IS NOT NULL
                             THEN CAST(t.history_retention_period AS VARCHAR) + ' ' +
                                  CASE t.history_retention_period_unit
                                      WHEN 3 THEN 'DAY'
                                      WHEN 4 THEN 'WEEK'
                                      WHEN 5 THEN 'MONTH'
                                      WHEN 6 THEN 'YEAR'
                                      ELSE 'UNKNOWN'
                                  END
                             ELSE 'No retention policy'
                        END AS RetentionPolicy
                    FROM sys.tables t
                    LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
                    WHERE t.name IN ('ImportedDataRecords', 'ImportedDataItems', 'RawDataImports')
                    ORDER BY t.name";

                var temporalTables = await _context.Database
                    .SqlQueryRaw<TemporalTableInfo>(temporalTablesQuery)
                    .ToListAsync();

                // 2. Kiểm tra Columnstore Indexes
                var columnstoreQuery = @"
                    SELECT
                        t.name AS TableName,
                        i.name AS IndexName,
                        CASE i.type
                            WHEN 5 THEN 'Clustered Columnstore'
                            WHEN 6 THEN 'Nonclustered Columnstore'
                            ELSE 'Other'
                        END AS IndexType,
                        p.[rows] AS RowCount,
                        p.data_compression_desc AS CompressionType
                    FROM sys.tables t
                    INNER JOIN sys.indexes i ON t.object_id = i.object_id
                    INNER JOIN sys.partitions p ON i.object_id = p.object_id AND i.index_id = p.index_id
                    WHERE i.type IN (5, 6) -- Columnstore indexes only
                      AND (t.name LIKE '%History' OR t.name IN ('ImportedDataRecords', 'ImportedDataItems', 'RawDataImports'))
                    ORDER BY t.name, i.name";

                var columnstoreIndexes = await _context.Database
                    .SqlQueryRaw<ColumnstoreIndexInfo>(columnstoreQuery)
                    .ToListAsync();

                // 3. Kiểm tra Index thông thường
                var regularIndexesQuery = @"
                    SELECT
                        t.name AS TableName,
                        i.name AS IndexName,
                        i.type_desc AS IndexType,
                        STRING_AGG(c.name, ', ') WITHIN GROUP (ORDER BY ic.key_ordinal) AS IndexColumns
                    FROM sys.tables t
                    INNER JOIN sys.indexes i ON t.object_id = i.object_id
                    INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                    INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                    WHERE t.name IN ('ImportedDataRecords', 'ImportedDataItems')
                      AND i.type NOT IN (0, 5, 6) -- Exclude heap, columnstore
                      AND i.is_primary_key = 0
                    GROUP BY t.name, i.name, i.type_desc
                    ORDER BY t.name, i.name";

                var regularIndexes = await _context.Database
                    .SqlQueryRaw<RegularIndexInfo>(regularIndexesQuery)
                    .ToListAsync();

                // 4. Kiểm tra Storage Compression
                var compressionQuery = @"
                    SELECT
                        OBJECT_NAME(p.object_id) AS TableName,
                        p.partition_number AS PartitionNumber,
                        p.data_compression_desc AS CompressionType,
                        p.[rows] AS RowCount,
                        CAST(SUM(a.total_pages) * 8.0 / 1024 AS DECIMAL(10,2)) AS SizeMB
                    FROM sys.partitions p
                    INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
                    WHERE OBJECT_NAME(p.object_id) IN ('ImportedDataRecords', 'ImportedDataItems',
                                                       'ImportedDataRecords_History', 'ImportedDataItems_History')
                    GROUP BY p.object_id, p.partition_number, p.data_compression_desc, p.[rows]
                    ORDER BY TableName";

                var compressionInfo = await _context.Database
                    .SqlQueryRaw<CompressionInfo>(compressionQuery)
                    .ToListAsync();

                // 5. Tính toán tóm tắt
                var summary = new
                {
                    TemporalTablesEnabled = temporalTables.Count(t => t.TemporalType == "System-versioned temporal"),
                    TotalTables = temporalTables.Count,
                    ColumnstoreIndexesCount = columnstoreIndexes.Count,
                    RegularIndexesCount = regularIndexes.Count,
                    CompressionEnabled = compressionInfo.Any(c => c.CompressionType != "NONE"),
                    TotalDataSize = compressionInfo.Sum(c => c.SizeMB)
                };

                _logger.LogInformation("📊 Database Status Summary: Temporal={Temporal}/{Total}, Columnstore={Columnstore}, Compression={Compression}",
                    summary.TemporalTablesEnabled, summary.TotalTables, summary.ColumnstoreIndexesCount, summary.CompressionEnabled);

                return Ok(new
                {
                    Summary = summary,
                    TemporalTables = temporalTables,
                    ColumnstoreIndexes = columnstoreIndexes,
                    RegularIndexes = regularIndexes,
                    CompressionInfo = compressionInfo,
                    Recommendations = GenerateRecommendations(temporalTables, columnstoreIndexes, compressionInfo)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi kiểm tra trạng thái database temporal");
                return StatusCode(500, new { message = "Lỗi kiểm tra database", error = ex.Message });
            }
        }

        // GET: api/TemporalDatabase/enable-temporal - Kích hoạt Temporal Tables
        [HttpPost("enable-temporal")]
        public async Task<IActionResult> EnableTemporalTables()
        {
            try
            {
                _logger.LogInformation("🚀 Kích hoạt Temporal Tables cho ImportedDataRecords và ImportedDataItems");

                var sql = @"
                    -- Kích hoạt Temporal Tables cho ImportedDataRecords
                    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ImportedDataRecords' AND temporal_type = 2)
                    BEGIN
                        -- Thêm temporal columns nếu chưa có
                        IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'SysStartTime' AND object_id = OBJECT_ID('ImportedDataRecords'))
                        BEGIN
                            ALTER TABLE [ImportedDataRecords] ADD
                                [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME(),
                                [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(datetime2, '9999-12-31 23:59:59.9999999'),
                                PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
                        END

                        -- Kích hoạt system versioning
                        ALTER TABLE [ImportedDataRecords]
                        SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[ImportedDataRecords_History]));

                        SELECT 'ImportedDataRecords temporal enabled' AS Result;
                    END
                    ELSE
                    BEGIN
                        SELECT 'ImportedDataRecords already temporal' AS Result;
                    END

                    -- Kích hoạt Temporal Tables cho ImportedDataItems
                    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ImportedDataItems' AND temporal_type = 2)
                    BEGIN
                        -- Thêm temporal columns nếu chưa có
                        IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'SysStartTime' AND object_id = OBJECT_ID('ImportedDataItems'))
                        BEGIN
                            ALTER TABLE [ImportedDataItems] ADD
                                [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME(),
                                [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(datetime2, '9999-12-31 23:59:59.9999999'),
                                PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
                        END

                        -- Kích hoạt system versioning
                        ALTER TABLE [ImportedDataItems]
                        SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[ImportedDataItems_History]));

                        SELECT 'ImportedDataItems temporal enabled' AS Result;
                    END
                    ELSE
                    BEGIN
                        SELECT 'ImportedDataItems already temporal' AS Result;
                    END";

                var results = await _context.Database.SqlQueryRaw<string>(sql).ToListAsync();

                return Ok(new { message = "Temporal Tables được kích hoạt thành công", results });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi kích hoạt Temporal Tables");
                return StatusCode(500, new { message = "Lỗi kích hoạt Temporal Tables", error = ex.Message });
            }
        }

        // POST: api/TemporalDatabase/create-columnstore - Tạo Columnstore Indexes
        [HttpPost("create-columnstore")]
        public async Task<IActionResult> CreateColumnstoreIndexes()
        {
            try
            {
                _logger.LogInformation("📊 Tạo Columnstore Indexes cho History Tables");

                var sql = @"
                    DECLARE @results TABLE (TableName NVARCHAR(128), IndexName NVARCHAR(128), Status NVARCHAR(50));

                    -- Tạo Columnstore Index cho ImportedDataRecords_History
                    IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ImportedDataRecords_History')
                    BEGIN
                        IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'CCI_ImportedDataRecords_History' AND object_id = OBJECT_ID('ImportedDataRecords_History'))
                        BEGIN
                            CREATE CLUSTERED COLUMNSTORE INDEX [CCI_ImportedDataRecords_History]
                            ON [dbo].[ImportedDataRecords_History];
                            INSERT @results VALUES ('ImportedDataRecords_History', 'CCI_ImportedDataRecords_History', 'Created');
                        END
                        ELSE
                        BEGIN
                            INSERT @results VALUES ('ImportedDataRecords_History', 'CCI_ImportedDataRecords_History', 'Already Exists');
                        END
                    END

                    -- Tạo Columnstore Index cho ImportedDataItems_History
                    IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ImportedDataItems_History')
                    BEGIN
                        IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'CCI_ImportedDataItems_History' AND object_id = OBJECT_ID('ImportedDataItems_History'))
                        BEGIN
                            CREATE CLUSTERED COLUMNSTORE INDEX [CCI_ImportedDataItems_History]
                            ON [dbo].[ImportedDataItems_History];
                            INSERT @results VALUES ('ImportedDataItems_History', 'CCI_ImportedDataItems_History', 'Created');
                        END
                        ELSE
                        BEGIN
                            INSERT @results VALUES ('ImportedDataItems_History', 'CCI_ImportedDataItems_History', 'Already Exists');
                        END
                    END

                    SELECT * FROM @results;";

                var results = await _context.Database
                    .SqlQueryRaw<ColumnstoreCreationResult>(sql)
                    .ToListAsync();

                return Ok(new { message = "Columnstore Indexes được tạo thành công", results });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi tạo Columnstore Indexes");
                return StatusCode(500, new { message = "Lỗi tạo Columnstore Indexes", error = ex.Message });
            }
        }

        // GET: api/TemporalDatabase/test - Test API đơn giản
        [HttpGet("test")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                // Test đơn giản với query database tables
                var simpleQuery = "SELECT name FROM sys.tables WHERE name IN ('ImportedDataRecords', 'ImportedDataItems') ORDER BY name";

                var tableNames = await _context.Database
                    .SqlQueryRaw<string>(simpleQuery)
                    .ToListAsync();

                return Ok(new
                {
                    message = "✅ Database connection working",
                    tables = tableNames,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi test database connection");
                return BadRequest(new { message = "Lỗi test database", error = ex.Message });
            }
        }

        // GET: api/TemporalDatabase/check-simple - Kiểm tra đơn giản temporal tables
        [HttpGet("check-simple")]
        public async Task<IActionResult> CheckTemporalTablesSimple()
        {
            try
            {
                _logger.LogInformation("🔍 Kiểm tra đơn giản Temporal Tables");

                // Query đơn giản chỉ kiểm tra temporal type
                var query = @"
                    SELECT
                        t.name AS TableName,
                        t.temporal_type AS TemporalType,
                        h.name AS HistoryTableName
                    FROM sys.tables t
                    LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
                    WHERE t.name IN ('ImportedDataRecords', 'ImportedDataItems', 'RawDataImports')
                    ORDER BY t.name";

                var results = await _context.Database
                    .SqlQueryRaw<TemporalTableSimple>(query)
                    .ToListAsync();

                return Ok(new
                {
                    message = "✅ Kiểm tra temporal tables thành công",
                    timestamp = DateTime.UtcNow,
                    tables = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi kiểm tra temporal tables");
                return BadRequest(new { message = "Lỗi kiểm tra temporal tables", error = ex.Message });
            }
        }

        // GET: api/TemporalDatabase/check-columnstore - Kiểm tra Columnstore Indexes
        [HttpGet("check-columnstore")]
        public async Task<IActionResult> CheckColumnstoreIndexes()
        {
            try
            {
                _logger.LogInformation("🔍 Kiểm tra Columnstore Indexes");

                var query = @"
                    SELECT
                        t.name AS TableName,
                        i.name AS IndexName,
                        i.type AS IndexType
                    FROM sys.tables t
                    INNER JOIN sys.indexes i ON t.object_id = i.object_id
                    WHERE i.type IN (5, 6) -- Columnstore indexes only
                      AND (t.name LIKE '%History' OR t.name IN ('ImportedDataRecords', 'ImportedDataItems', 'RawDataImports'))
                    ORDER BY t.name, i.name";

                var results = await _context.Database
                    .SqlQueryRaw<ColumnstoreIndexSimple>(query)
                    .ToListAsync();

                return Ok(new
                {
                    message = "✅ Kiểm tra columnstore indexes thành công",
                    timestamp = DateTime.UtcNow,
                    indexes = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi kiểm tra columnstore indexes");
                return BadRequest(new { message = "Lỗi kiểm tra columnstore indexes", error = ex.Message });
            }
        }

        // GET: api/TemporalDatabase/history-changes - Kiểm tra các thay đổi từ temporal history
        [HttpGet("history-changes")]
        public async Task<IActionResult> GetHistoryChanges()
        {
            try
            {
                _logger.LogInformation("🔍 Kiểm tra các thay đổi từ temporal history tables");

                // Kiểm tra ImportedDataRecords_History
                var recordsHistoryQuery = @"
                    SELECT TOP 20
                        Id,
                        FileName,
                        FileType,
                        Category,
                        ImportDate,
                        SysStartTime,
                        SysEndTime
                    FROM ImportedDataRecords_History
                    ORDER BY SysStartTime DESC";

                var recordsHistory = await _context.Database
                    .SqlQueryRaw<ImportedDataRecordHistory>(recordsHistoryQuery)
                    .ToListAsync();

                // Kiểm tra ImportedDataItems_History
                var itemsHistoryQuery = @"
                    SELECT TOP 20
                        Id,
                        ImportedDataRecordId,
                        RawData,
                        ProcessedDate,
                        SysStartTime,
                        SysEndTime
                    FROM ImportedDataItems_History
                    ORDER BY SysStartTime DESC";

                var itemsHistory = await _context.Database
                    .SqlQueryRaw<ImportedDataItemHistory>(itemsHistoryQuery)
                    .ToListAsync();

                return Ok(new
                {
                    message = "✅ Lấy lịch sử thay đổi thành công",
                    timestamp = DateTime.UtcNow,
                    recordsHistory = recordsHistory,
                    itemsHistory = itemsHistory,
                    totalRecordChanges = recordsHistory.Count,
                    totalItemChanges = itemsHistory.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi lấy lịch sử thay đổi");
                return BadRequest(new { message = "Lỗi lấy lịch sử thay đổi", error = ex.Message });
            }
        }

        // GET: api/TemporalDatabase/check-tables - Kiểm tra cấu trúc bảng
        [HttpGet("check-tables")]
        public async Task<IActionResult> CheckTableStructure()
        {
            try
            {
                _logger.LogInformation("🔍 Kiểm tra cấu trúc bảng history");

                var query = @"
                    SELECT
                        t.name AS TableName,
                        c.name AS ColumnName,
                        ty.name AS DataType
                    FROM sys.tables t
                    INNER JOIN sys.columns c ON t.object_id = c.object_id
                    INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
                    WHERE t.name IN ('ImportedDataRecords_History', 'ImportedDataItems_History')
                    ORDER BY t.name, c.column_id";

                var results = await _context.Database
                    .SqlQueryRaw<TableColumnInfo>(query)
                    .ToListAsync();

                return Ok(new
                {
                    message = "✅ Lấy cấu trúc bảng thành công",
                    timestamp = DateTime.UtcNow,
                    columns = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi lấy cấu trúc bảng");
                return BadRequest(new { message = "Lỗi lấy cấu trúc bảng", error = ex.Message });
            }
        }

        // GET: api/TemporalDatabase/scan-all - Rà soát toàn bộ database
        [HttpGet("scan-all")]
        public async Task<IActionResult> ScanAllDatabase()
        {
            try
            {
                _logger.LogInformation("🔍 Rà soát toàn bộ database");

                // 1. Lấy tất cả user tables
                var allTablesQuery = @"
                    SELECT
                        t.name AS TableName,
                        t.temporal_type AS TemporalType,
                        h.name AS HistoryTableName
                    FROM sys.tables t
                    LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
                    WHERE t.type = 'U'
                      AND t.name NOT LIKE '__EF%'
                      AND t.name NOT LIKE 'sysdiagrams'
                    ORDER BY t.name";

                var allTables = await _context.Database
                    .SqlQueryRaw<TableScanInfo>(allTablesQuery)
                    .ToListAsync();

                // 2. Lấy tất cả columnstore indexes
                var columnstoreQuery = @"
                    SELECT
                        t.name AS TableName,
                        i.name AS IndexName,
                        i.type AS IndexType
                    FROM sys.tables t
                    INNER JOIN sys.indexes i ON t.object_id = i.object_id
                    WHERE i.type IN (5, 6)
                      AND t.type = 'U'
                    ORDER BY t.name";

                var columnstoreIndexes = await _context.Database
                    .SqlQueryRaw<ColumnstoreIndexSimple>(columnstoreQuery)
                    .ToListAsync();

                // 3. Phân tích
                var nonTemporalTables = allTables.Where(t => t.TemporalType == 0 && !t.TableName.EndsWith("_History")).ToList();
                var tablesWithoutColumnstore = allTables.Where(t =>
                    (t.TableName.EndsWith("_History") || t.TableName.Contains("Import") || t.TableName.Contains("Data"))
                    && !columnstoreIndexes.Any(c => c.TableName == t.TableName)).ToList();

                return Ok(new
                {
                    message = "✅ Rà soát database thành công",
                    totalTables = allTables.Count,
                    temporalTables = allTables.Count(t => t.TemporalType == 2),
                    historyTables = allTables.Count(t => t.TemporalType == 1),
                    columnstoreIndexes = columnstoreIndexes.Count,
                    allTables = allTables,
                    columnstoreList = columnstoreIndexes,
                    recommendations = new
                    {
                        needTemporal = nonTemporalTables.Take(10).Select(t => t.TableName).ToList(),
                        needColumnstore = tablesWithoutColumnstore.Take(10).Select(t => t.TableName).ToList()
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi rà soát database");
                return BadRequest(new { message = "Lỗi rà soát database", error = ex.Message });
            }
        }

        private static List<string> GenerateRecommendations(
            List<TemporalTableInfo> temporalTables,
            List<ColumnstoreIndexInfo> columnstoreIndexes,
            List<CompressionInfo> compressionInfo)
        {
            var recommendations = new List<string>();

            // Kiểm tra Temporal Tables
            var nonTemporalTables = temporalTables.Where(t => t.TemporalType != "System-versioned temporal").ToList();
            if (nonTemporalTables.Any())
            {
                recommendations.Add($"❌ Cần kích hoạt Temporal Tables cho: {string.Join(", ", nonTemporalTables.Select(t => t.TableName))}");
            }

            // Kiểm tra Columnstore Indexes
            var expectedHistoryTables = new[] { "ImportedDataRecords_History", "ImportedDataItems_History" };
            var missingColumnstore = expectedHistoryTables.Where(t =>
                !columnstoreIndexes.Any(c => c.TableName == t)).ToList();
            if (missingColumnstore.Any())
            {
                recommendations.Add($"📊 Cần tạo Columnstore Indexes cho: {string.Join(", ", missingColumnstore)}");
            }

            // Kiểm tra Compression
            var uncompressedTables = compressionInfo.Where(c => c.CompressionType == "NONE").ToList();
            if (uncompressedTables.Any())
            {
                recommendations.Add($"🗜️ Cần bật compression cho: {string.Join(", ", uncompressedTables.Select(t => t.TableName))}");
            }

            if (!recommendations.Any())
            {
                recommendations.Add("✅ Tất cả Temporal Tables + Columnstore Indexes đã được cấu hình đúng chuẩn!");
            }

            return recommendations;
        }
    }

    // DTO Classes cho SQL query results
    public class TemporalTableInfo
    {
        public string TableName { get; set; } = "";
        public string TemporalType { get; set; } = "";
        public string? HistoryTableName { get; set; }
        public string RetentionPolicy { get; set; } = "";
    }

    public class ColumnstoreIndexInfo
    {
        public string TableName { get; set; } = "";
        public string IndexName { get; set; } = "";
        public string IndexType { get; set; } = "";
        public long RowCount { get; set; }
        public string CompressionType { get; set; } = "";
    }

    public class RegularIndexInfo
    {
        public string TableName { get; set; } = "";
        public string IndexName { get; set; } = "";
        public string IndexType { get; set; } = "";
        public string IndexColumns { get; set; } = "";
    }

    public class CompressionInfo
    {
        public string TableName { get; set; } = "";
        public int PartitionNumber { get; set; }
        public string CompressionType { get; set; } = "";
        public long RowCount { get; set; }
        public decimal SizeMB { get; set; }
    }

    public class ColumnstoreCreationResult
    {
        public string TableName { get; set; } = "";
        public string IndexName { get; set; } = "";
        public string Status { get; set; } = "";
    }

    public class TemporalTableSimple
    {
        public string TableName { get; set; } = "";
        public byte TemporalType { get; set; }
        public string? HistoryTableName { get; set; }
    }

    public class ColumnstoreIndexSimple
    {
        public string TableName { get; set; } = "";
        public string IndexName { get; set; } = "";
        public byte IndexType { get; set; }
    }

    public class ImportedDataRecordHistory
    {
        public int Id { get; set; }
        public string FileName { get; set; } = "";
        public string FileType { get; set; } = "";
        public string Category { get; set; } = "";
        public DateTime ImportDate { get; set; }
        public DateTime SysStartTime { get; set; }
        public DateTime SysEndTime { get; set; }
    }

    public class ImportedDataItemHistory
    {
        public int Id { get; set; }
        public int ImportedDataRecordId { get; set; }
        public string RawData { get; set; } = "";
        public DateTime? ProcessedDate { get; set; }
        public DateTime SysStartTime { get; set; }
        public DateTime SysEndTime { get; set; }
    }

    public class TableColumnInfo
    {
        public string TableName { get; set; } = "";
        public string ColumnName { get; set; } = "";
        public string DataType { get; set; } = "";
    }

    public class TableScanInfo
    {
        public string TableName { get; set; } = "";
        public byte TemporalType { get; set; }
        public string? HistoryTableName { get; set; }
    }
}
