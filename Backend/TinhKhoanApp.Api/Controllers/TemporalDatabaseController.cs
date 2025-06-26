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

        // POST: api/TemporalDatabase/create-all-columnstore - Tạo Columnstore cho tất cả bảng lịch sử
        [HttpPost("create-all-columnstore")]
        public async Task<IActionResult> CreateAllColumnstoreIndexes()
        {
            try
            {
                _logger.LogInformation("📊 Tạo Columnstore Indexes cho tất cả bảng lịch sử và bảng lớn");

                var sql = @"
                    DECLARE @results TABLE (TableName NVARCHAR(128), IndexName NVARCHAR(128), Status NVARCHAR(50), Message NVARCHAR(500));

                    -- Danh sách bảng cần tạo Columnstore Index
                    DECLARE @tables TABLE (TableName NVARCHAR(128), IndexName NVARCHAR(128));
                    INSERT INTO @tables VALUES
                        ('BC57_History', 'CCI_BC57_History'),
                        ('DB01_History', 'CCI_DB01_History'),
                        ('DPDA_History', 'CCI_DPDA_History'),
                        ('EI01_History', 'CCI_EI01_History'),
                        ('GL01_History', 'CCI_GL01_History'),
                        ('LN01_History', 'CCI_LN01_History'),
                        ('LN03_History', 'CCI_LN03_History'),
                        ('KH03_History', 'CCI_KH03_History'),
                        ('RawDataImports_History', 'CCI_RawDataImports_History'),
                        ('ImportedDataItems', 'CCI_ImportedDataItems_Main'),
                        ('ImportedDataRecords', 'CCI_ImportedDataRecords_Main'),
                        ('LegacyRawDataImports', 'CCI_LegacyRawDataImports'),
                        ('ImportLogs', 'CCI_ImportLogs'),
                        ('BusinessPlanTargets_History', 'CCI_BusinessPlanTargets_History'),
                        ('Employees_History', 'CCI_Employees_History'),
                        ('EmployeeKpiAssignments_History', 'CCI_EmployeeKpiAssignments_History'),
                        ('FinalPayouts_History', 'CCI_FinalPayouts_History'),
                        ('KPIDefinitions_History', 'CCI_KPIDefinitions_History');

                    DECLARE @tableName NVARCHAR(128);
                    DECLARE @indexName NVARCHAR(128);
                    DECLARE @sql NVARCHAR(MAX);

                    DECLARE table_cursor CURSOR FOR
                    SELECT TableName, IndexName FROM @tables;

                    OPEN table_cursor;
                    FETCH NEXT FROM table_cursor INTO @tableName, @indexName;

                    WHILE @@FETCH_STATUS = 0
                    BEGIN
                        BEGIN TRY
                            -- Kiểm tra bảng có tồn tại không
                            IF EXISTS (SELECT 1 FROM sys.tables WHERE name = @tableName)
                            BEGIN
                                -- Kiểm tra index đã tồn tại chưa
                                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = @indexName AND object_id = OBJECT_ID(@tableName))
                                BEGIN
                                    -- Tạo Clustered Columnstore Index
                                    SET @sql = 'CREATE CLUSTERED COLUMNSTORE INDEX [' + @indexName + '] ON [dbo].[' + @tableName + ']';
                                    EXEC sp_executesql @sql;

                                    INSERT INTO @results VALUES (@tableName, @indexName, 'SUCCESS', 'Columnstore index created successfully');
                                END
                                ELSE
                                BEGIN
                                    INSERT INTO @results VALUES (@tableName, @indexName, 'ALREADY_EXISTS', 'Index already exists');
                                END
                            END
                            ELSE
                            BEGIN
                                INSERT INTO @results VALUES (@tableName, @indexName, 'TABLE_NOT_FOUND', 'Table does not exist');
                            END
                        END TRY
                        BEGIN CATCH
                            INSERT INTO @results VALUES (@tableName, @indexName, 'ERROR', ERROR_MESSAGE());
                        END CATCH

                        FETCH NEXT FROM table_cursor INTO @tableName, @indexName;
                    END

                    CLOSE table_cursor;
                    DEALLOCATE table_cursor;

                    SELECT * FROM @results;";

                var results = await _context.Database
                    .SqlQueryRaw<ColumnstoreCreationResult>(sql)
                    .ToListAsync();

                var successCount = results.Count(r => r.Status == "SUCCESS");
                var alreadyExistsCount = results.Count(r => r.Status == "ALREADY_EXISTS");

                return Ok(new
                {
                    message = $"✅ Hoàn thành tạo Columnstore Indexes: {successCount} mới, {alreadyExistsCount} đã có",
                    timestamp = DateTime.UtcNow,
                    results = results,
                    summary = new
                    {
                        newlyCreated = successCount,
                        alreadyExists = alreadyExistsCount,
                        errors = results.Count(r => r.Status == "ERROR"),
                        tableNotFound = results.Count(r => r.Status == "TABLE_NOT_FOUND")
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi tạo Columnstore Indexes");
                return StatusCode(500, new { message = "Lỗi tạo Columnstore Indexes", error = ex.Message });
            }
        }

        // POST: api/TemporalDatabase/smart-columnstore - Tạo Columnstore Index thông minh
        [HttpPost("smart-columnstore")]
        public async Task<IActionResult> CreateSmartColumnstoreIndexes()
        {
            try
            {
                _logger.LogInformation("🧠 Smart Columnstore: Chỉ tạo index cho bảng có đủ dữ liệu");

                var results = new List<ColumnstoreIndexSimple>();
                var summary = new { created = 0, existing = 0, skipped = 0, errors = 0, total = 0 };

                // 1. Kiểm tra bảng nào có đủ dữ liệu (>= 10,000 rows) và chưa có Columnstore
                var candidateTablesQuery = @"
                    SELECT
                        t.name AS TableName,
                        p.rows AS RowCount,
                        CASE WHEN EXISTS (
                            SELECT 1 FROM sys.indexes i
                            WHERE i.object_id = t.object_id AND i.type IN (5, 6)
                        ) THEN 1 ELSE 0 END AS HasColumnstore
                    FROM sys.tables t
                    JOIN sys.partitions p ON t.object_id = p.object_id
                    WHERE p.index_id IN (0, 1) -- Heap or Clustered
                      AND p.rows >= 10000 -- Tối thiểu 10K rows
                      AND (t.name LIKE '%History' OR t.name IN ('ImportedDataItems', 'ImportedDataRecords', 'LegacyRawDataImports'))
                    ORDER BY p.rows DESC";

                var candidateTables = await _context.Database
                    .SqlQueryRaw<CandidateTableInfo>(candidateTablesQuery)
                    .ToListAsync();

                var created = 0;
                var existing = 0;
                var skipped = 0;
                var errors = 0;

                foreach (var table in candidateTables)
                {
                    var tableName = table.TableName;
                    var rowCount = table.RowCount;
                    var hasColumnstore = table.HasColumnstore;

                    if (hasColumnstore)
                    {
                        results.Add(new ColumnstoreIndexSimple
                        {
                            TableName = tableName,
                            IndexName = $"CCI_{tableName}",
                            Status = "Existing"
                        });
                        existing++;
                        continue;
                    }

                    if (rowCount < 10000)
                    {
                        results.Add(new ColumnstoreIndexSimple
                        {
                            TableName = tableName,
                            IndexName = $"CCI_{tableName}",
                            Status = "Skipped (insufficient data)"
                        });
                        skipped++;
                        continue;
                    }

                    // Tạo Columnstore Index
                    try
                    {
                        var indexName = $"CCI_{tableName}";
                        var createIndexSql = $@"
                            CREATE CLUSTERED COLUMNSTORE INDEX [{indexName}]
                            ON [dbo].[{tableName}]
                            WITH (MAXDOP = 4, COMPRESSION_DELAY = 0)";

                        await _context.Database.ExecuteSqlRawAsync(createIndexSql);

                        results.Add(new ColumnstoreIndexSimple
                        {
                            TableName = tableName,
                            IndexName = indexName,
                            Status = "Created"
                        });
                        created++;

                        _logger.LogInformation($"✅ Created Columnstore: {indexName} for {tableName} ({rowCount:N0} rows)");
                    }
                    catch (Exception ex)
                    {
                        results.Add(new ColumnstoreIndexSimple
                        {
                            TableName = tableName,
                            IndexName = $"CCI_{tableName}",
                            Status = "Error: " + ex.Message.Substring(0, Math.Min(100, ex.Message.Length))
                        });
                        errors++;
                        _logger.LogWarning($"❌ Failed to create Columnstore for {tableName}: {ex.Message}");
                    }
                }

                summary = new { created, existing, skipped, errors, total = results.Count };

                return Ok(new
                {
                    message = $"🧠 Smart Columnstore: {created} tạo mới, {existing} đã có, {skipped} bỏ qua, {errors} lỗi",
                    results,
                    summary,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi smart columnstore creation");
                return StatusCode(500, new { error = "Smart columnstore creation failed", details = ex.Message });
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

        // POST: api/TemporalDatabase/enable-all-temporal - Kích hoạt Temporal cho tất cả bảng quan trọng
        [HttpPost("enable-all-temporal")]
        public async Task<IActionResult> EnableAllTemporalTables()
        {
            try
            {
                _logger.LogInformation("🚀 Kích hoạt Temporal Tables cho tất cả bảng nghiệp vụ quan trọng");

                var sql = @"
                    DECLARE @results TABLE (TableName NVARCHAR(128), Status NVARCHAR(50), Message NVARCHAR(500));

                    -- Danh sách bảng cần kích hoạt Temporal
                    DECLARE @tables TABLE (TableName NVARCHAR(128));
                    INSERT INTO @tables VALUES
                        ('BusinessPlanTargets'),
                        ('DashboardCalculations'),
                        ('EmployeeKhoanAssignments'),
                        ('EmployeeKpiAssignments'),
                        ('EmployeeKpiTargets'),
                        ('Employees'),
                        ('FinalPayouts'),
                        ('KPIDefinitions'),
                        ('KpiScoringRules'),
                        ('UnitKpiScorings');

                    DECLARE @tableName NVARCHAR(128);
                    DECLARE @historyTableName NVARCHAR(128);
                    DECLARE @sql NVARCHAR(MAX);

                    DECLARE table_cursor CURSOR FOR
                    SELECT TableName FROM @tables;

                    OPEN table_cursor;
                    FETCH NEXT FROM table_cursor INTO @tableName;

                    WHILE @@FETCH_STATUS = 0
                    BEGIN
                        SET @historyTableName = @tableName + '_History';

                        BEGIN TRY
                            -- Kiểm tra bảng có tồn tại không
                            IF EXISTS (SELECT 1 FROM sys.tables WHERE name = @tableName)
                            BEGIN
                                -- Kiểm tra đã là temporal chưa
                                IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = @tableName AND temporal_type = 2)
                                BEGIN
                                    -- Thêm temporal columns nếu chưa có
                                    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'SysStartTime' AND object_id = OBJECT_ID(@tableName))
                                    BEGIN
                                        SET @sql = 'ALTER TABLE [' + @tableName + '] ADD
                                            [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME(),
                                            [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(datetime2, ''9999-12-31 23:59:59.9999999''),
                                            PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime])';
                                        EXEC sp_executesql @sql;
                                    END

                                    -- Kích hoạt system versioning
                                    SET @sql = 'ALTER TABLE [' + @tableName + ']
                                        SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[' + @historyTableName + ']))';
                                    EXEC sp_executesql @sql;

                                    INSERT INTO @results VALUES (@tableName, 'SUCCESS', 'Temporal enabled successfully');
                                END
                                ELSE
                                BEGIN
                                    INSERT INTO @results VALUES (@tableName, 'ALREADY_ENABLED', 'Already temporal table');
                                END
                            END
                            ELSE
                            BEGIN
                                INSERT INTO @results VALUES (@tableName, 'NOT_FOUND', 'Table does not exist');
                            END
                        END TRY
                        BEGIN CATCH
                            INSERT INTO @results VALUES (@tableName, 'ERROR', ERROR_MESSAGE());
                        END CATCH

                        FETCH NEXT FROM table_cursor INTO @tableName;
                    END

                    CLOSE table_cursor;
                    DEALLOCATE table_cursor;

                    SELECT * FROM @results;";

                var results = await _context.Database
                    .SqlQueryRaw<TemporalEnableResult>(sql)
                    .ToListAsync();

                var successCount = results.Count(r => r.Status == "SUCCESS");
                var alreadyEnabledCount = results.Count(r => r.Status == "ALREADY_ENABLED");

                return Ok(new
                {
                    message = $"✅ Hoàn thành kích hoạt Temporal Tables: {successCount} mới, {alreadyEnabledCount} đã có",
                    timestamp = DateTime.UtcNow,
                    results = results,
                    summary = new
                    {
                        newlyEnabled = successCount,
                        alreadyEnabled = alreadyEnabledCount,
                        errors = results.Count(r => r.Status == "ERROR"),
                        notFound = results.Count(r => r.Status == "NOT_FOUND")
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi kích hoạt Temporal Tables");
                return StatusCode(500, new { message = "Lỗi kích hoạt Temporal Tables", error = ex.Message });
            }
        }

        // POST: api/TemporalDatabase/optimize-all - Thực hiện tất cả tối ưu hóa một lần
        [HttpPost("optimize-all")]
        public async Task<IActionResult> OptimizeAllDatabase()
        {
            try
            {
                _logger.LogInformation("🎯 BẮT ĐẦU TỐI ƯU HOÁ TOÀN BỘ DATABASE - Temporal Tables + Columnstore Indexes");

                var startTime = DateTime.UtcNow;
                var results = new List<string>();

                // BƯỚC 1: Kích hoạt Temporal Tables
                _logger.LogInformation("🕐 BƯỚC 1: Kích hoạt Temporal Tables cho bảng nghiệp vụ...");
                var temporalResult = await EnableAllTemporalTablesInternal();
                results.Add($"✅ Temporal Tables: {temporalResult.NewlyEnabled} mới, {temporalResult.AlreadyEnabled} đã có");

                // BƯỚC 2: Tạo Columnstore Indexes
                _logger.LogInformation("📊 BƯỚC 2: Tạo Columnstore Indexes cho bảng lịch sử...");
                var columnstoreResult = await CreateAllColumnstoreIndexesInternal();
                results.Add($"✅ Columnstore Indexes: {columnstoreResult.NewlyCreated} mới, {columnstoreResult.AlreadyExists} đã có");

                // BƯỚC 3: Cập nhật statistics
                _logger.LogInformation("📈 BƯỚC 3: Cập nhật statistics cho tất cả bảng...");
                await UpdateAllStatistics();
                results.Add("✅ Statistics: Đã cập nhật cho tất cả bảng");

                var endTime = DateTime.UtcNow;
                var duration = endTime - startTime;

                var summary = new
                {
                    totalDuration = $"{duration.TotalSeconds:F2} giây",
                    temporalTables = new
                    {
                        newlyEnabled = temporalResult.NewlyEnabled,
                        alreadyEnabled = temporalResult.AlreadyEnabled,
                        errors = temporalResult.Errors
                    },
                    columnstoreIndexes = new
                    {
                        newlyCreated = columnstoreResult.NewlyCreated,
                        alreadyExists = columnstoreResult.AlreadyExists,
                        errors = columnstoreResult.Errors
                    }
                };

                _logger.LogInformation("🎉 HOÀN THÀNH TỐI ƯU HOÁ DATABASE trong {Duration} giây", duration.TotalSeconds);

                return Ok(new
                {
                    message = "🎉 HOÀN THÀNH TỐI ƯU HOÁ TOÀN BỘ DATABASE!",
                    timestamp = DateTime.UtcNow,
                    duration = duration.TotalSeconds,
                    results = results,
                    summary = summary
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi tối ưu hóa database");
                return StatusCode(500, new { message = "Lỗi tối ưu hóa database", error = ex.Message });
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

        private async Task<(int NewlyEnabled, int AlreadyEnabled, int Errors)> EnableAllTemporalTablesInternal()
        {
            var sql = @"
                DECLARE @newlyEnabled INT = 0;
                DECLARE @alreadyEnabled INT = 0;
                DECLARE @errors INT = 0;

                -- Danh sách bảng cần kích hoạt Temporal
                DECLARE @tables TABLE (TableName NVARCHAR(128));
                INSERT INTO @tables VALUES
                    ('BusinessPlanTargets'),
                    ('DashboardCalculations'),
                    ('EmployeeKhoanAssignments'),
                    ('EmployeeKpiAssignments'),
                    ('EmployeeKpiTargets'),
                    ('Employees'),
                    ('FinalPayouts'),
                    ('KPIDefinitions'),
                    ('KpiScoringRules'),
                    ('UnitKpiScorings');

                DECLARE @tableName NVARCHAR(128);
                DECLARE @historyTableName NVARCHAR(128);
                DECLARE @sql NVARCHAR(MAX);

                DECLARE table_cursor CURSOR FOR
                SELECT TableName FROM @tables;

                OPEN table_cursor;
                FETCH NEXT FROM table_cursor INTO @tableName;

                WHILE @@FETCH_STATUS = 0
                BEGIN
                    SET @historyTableName = @tableName + '_History';

                    BEGIN TRY
                        -- Kiểm tra bảng có tồn tại không
                        IF EXISTS (SELECT 1 FROM sys.tables WHERE name = @tableName)
                        BEGIN
                            -- Kiểm tra đã là temporal chưa
                            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = @tableName AND temporal_type = 2)
                            BEGIN
                                -- Thêm temporal columns nếu chưa có
                                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'SysStartTime' AND object_id = OBJECT_ID(@tableName))
                                BEGIN
                                    SET @sql = 'ALTER TABLE [' + @tableName + '] ADD
                                        [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME(),
                                        [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(datetime2, ''9999-12-31 23:59:59.9999999''),
                                        PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime])';
                                    EXEC sp_executesql @sql;
                                END

                                -- Kích hoạt system versioning
                                SET @sql = 'ALTER TABLE [' + @tableName + ']
                                    SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[' + @historyTableName + ']))';
                                EXEC sp_executesql @sql;

                                SET @newlyEnabled = @newlyEnabled + 1;
                            END
                            ELSE
                            BEGIN
                                SET @alreadyEnabled = @alreadyEnabled + 1;
                            END
                        END
                        ELSE
                        BEGIN
                            SET @errors = @errors + 1;
                        END
                    END TRY
                    BEGIN CATCH
                        SET @errors = @errors + 1;
                    END CATCH

                    FETCH NEXT FROM table_cursor INTO @tableName;
                END

                CLOSE table_cursor;
                DEALLOCATE table_cursor;

                SELECT @newlyEnabled AS NewlyEnabled, @alreadyEnabled AS AlreadyEnabled, @errors AS Errors;";

            var result = await _context.Database
                .SqlQueryRaw<(int NewlyEnabled, int AlreadyEnabled, int Errors)>(sql)
                .FirstOrDefaultAsync();

            return result;
        }

        private async Task<(int NewlyCreated, int AlreadyExists, int Errors)> CreateAllColumnstoreIndexesInternal()
        {
            var sql = @"
                DECLARE @newlyCreated INT = 0;
                DECLARE @alreadyExists INT = 0;
                DECLARE @errors INT = 0;

                -- Danh sách bảng cần tạo Columnstore Index
                DECLARE @tables TABLE (TableName NVARCHAR(128), IndexName NVARCHAR(128));
                INSERT INTO @tables VALUES
                    ('BC57_History', 'CCI_BC57_History'),
                    ('DB01_History', 'CCI_DB01_History'),
                    ('DPDA_History', 'CCI_DPDA_History'),
                    ('EI01_History', 'CCI_EI01_History'),
                    ('GL01_History', 'CCI_GL01_History'),
                    ('LN01_History', 'CCI_LN01_History'),
                    ('LN03_History', 'CCI_LN03_History'),
                    ('KH03_History', 'CCI_KH03_History'),
                    ('RawDataImports_History', 'CCI_RawDataImports_History'),
                    ('ImportedDataItems', 'CCI_ImportedDataItems_Main'),
                    ('ImportedDataRecords', 'CCI_ImportedDataRecords_Main'),
                    ('LegacyRawDataImports', 'CCI_LegacyRawDataImports'),
                    ('ImportLogs', 'CCI_ImportLogs'),
                    ('BusinessPlanTargets_History', 'CCI_BusinessPlanTargets_History'),
                    ('Employees_History', 'CCI_Employees_History'),
                    ('EmployeeKpiAssignments_History', 'CCI_EmployeeKpiAssignments_History'),
                    ('FinalPayouts_History', 'CCI_FinalPayouts_History'),
                    ('KPIDefinitions_History', 'CCI_KPIDefinitions_History');

                DECLARE @tableName NVARCHAR(128);
                DECLARE @indexName NVARCHAR(128);
                DECLARE @sql NVARCHAR(MAX);

                DECLARE table_cursor CURSOR FOR
                SELECT TableName, IndexName FROM @tables;

                OPEN table_cursor;
                FETCH NEXT FROM table_cursor INTO @tableName, @indexName;

                WHILE @@FETCH_STATUS = 0
                BEGIN
                    BEGIN TRY
                        -- Kiểm tra bảng có tồn tại không
                        IF EXISTS (SELECT 1 FROM sys.tables WHERE name = @tableName)
                        BEGIN
                            -- Kiểm tra index đã tồn tại chưa
                            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = @indexName AND object_id = OBJECT_ID(@tableName))
                            BEGIN
                                -- Tạo Clustered Columnstore Index
                                SET @sql = 'CREATE CLUSTERED COLUMNSTORE INDEX [' + @indexName + '] ON [dbo].[' + @tableName + ']';
                                EXEC sp_executesql @sql;

                                SET @newlyCreated = @newlyCreated + 1;
                            END
                            ELSE
                            BEGIN
                                SET @alreadyExists = @alreadyExists + 1;
                            END
                        END
                        ELSE
                        BEGIN
                            SET @errors = @errors + 1;
                        END
                    END TRY
                    BEGIN CATCH
                        SET @errors = @errors + 1;
                    END CATCH

                    FETCH NEXT FROM table_cursor INTO @tableName, @indexName;
                END

                CLOSE table_cursor;
                DEALLOCATE table_cursor;

                SELECT @newlyCreated AS NewlyCreated, @alreadyExists AS AlreadyExists, @errors AS Errors;";

            var result = await _context.Database
                .SqlQueryRaw<(int NewlyCreated, int AlreadyExists, int Errors)>(sql)
                .FirstOrDefaultAsync();

            return result;
        }

        private async Task UpdateAllStatistics()
        {
            var sql = @"
                EXEC sp_MSforeachtable 'UPDATE STATISTICS ? WITH FULLSCAN'";

            await _context.Database.ExecuteSqlRawAsync(sql);
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
        public string Status { get; set; } = "";
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

    public class TemporalEnableResult
    {
        public string TableName { get; set; } = "";
        public string Status { get; set; } = "";
        public string Message { get; set; } = "";
    }

    public class CandidateTableInfo
    {
        public string TableName { get; set; } = "";
        public long RowCount { get; set; }
        public bool HasColumnstore { get; set; }
    }
}
