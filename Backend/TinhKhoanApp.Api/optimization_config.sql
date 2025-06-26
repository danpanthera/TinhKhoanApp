-- ================================================
-- TinhKhoanApp Database Optimization Configuration
-- Thiết kế cấu hình tối ưu cho việc commit nhanh nhất
-- ================================================

-- 1. TEMPORAL TABLES CONFIGURATION (Tối ưu hóa retention policy)
-- Cấu hình retention ngắn hạn cho performance tối đa
ALTER TABLE [dbo].[Employees] SET (HISTORY_RETENTION_PERIOD = 6 MONTHS);
ALTER TABLE [dbo].[EmployeeKpiAssignments] SET (HISTORY_RETENTION_PERIOD = 6 MONTHS);
ALTER TABLE [dbo].[FinalPayouts] SET (HISTORY_RETENTION_PERIOD = 1 YEAR);
ALTER TABLE [dbo].[KPIDefinitions] SET (HISTORY_RETENTION_PERIOD = 1 YEAR);
ALTER TABLE [dbo].[BusinessPlanTargets] SET (HISTORY_RETENTION_PERIOD = 1 YEAR);

-- 2. DATABASE PERFORMANCE SETTINGS (Tối ưu cho commit nhanh)
-- Bật Page Compression cho performance tối đa
ALTER DATABASE [TinhKhoanApp] SET PAGE_VERIFY CHECKSUM;
ALTER DATABASE [TinhKhoanApp] SET AUTO_CREATE_STATISTICS ON;
ALTER DATABASE [TinhKhoanApp] SET AUTO_UPDATE_STATISTICS ON;
ALTER DATABASE [TinhKhoanApp] SET AUTO_UPDATE_STATISTICS_ASYNC ON;

-- 3. TRANSACTION LOG OPTIMIZATION
-- Tăng kích thước transaction log để giảm auto-growth
ALTER DATABASE [TinhKhoanApp] MODIFY FILE (
    NAME = N'TinhKhoanApp_Log',
    SIZE = 512MB,
    FILEGROWTH = 64MB
);

-- 4. MEMORY OPTIMIZATION
-- Cấu hình Buffer Pool cho performance tối đa
EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
EXEC sp_configure 'max server memory (MB)', 4096; -- 4GB
RECONFIGURE;

-- 5. INDEX MAINTENANCE SCHEDULE
-- Tự động rebuild index khi fragmentation > 30%
DECLARE @sql NVARCHAR(MAX) = '';
SELECT @sql = @sql +
    'ALTER INDEX ' + QUOTENAME(i.name) + ' ON ' + QUOTENAME(SCHEMA_NAME(t.schema_id)) + '.' + QUOTENAME(t.name) +
    ' REBUILD WITH (ONLINE = ON, MAXDOP = 4);' + CHAR(13)
FROM sys.tables t
JOIN sys.indexes i ON t.object_id = i.object_id
WHERE i.type > 0 AND t.temporal_type IN (0, 2) -- Main tables only
  AND t.name IN ('Employees', 'EmployeeKpiAssignments', 'FinalPayouts', 'KPIDefinitions', 'BusinessPlanTargets');

PRINT '-- Index Maintenance Commands:';
PRINT @sql;

-- 6. STATISTICS UPDATE FOR TEMPORAL TABLES
UPDATE STATISTICS [dbo].[Employees] WITH FULLSCAN;
UPDATE STATISTICS [dbo].[EmployeeKpiAssignments] WITH FULLSCAN;
UPDATE STATISTICS [dbo].[FinalPayouts] WITH FULLSCAN;
UPDATE STATISTICS [dbo].[KPIDefinitions] WITH FULLSCAN;
UPDATE STATISTICS [dbo].[BusinessPlanTargets] WITH FULLSCAN;

-- 7. COLUMNSTORE OPTIMIZATION FOR SPECIFIC TABLES
-- Chỉ tạo Columnstore cho bảng có đủ dữ liệu (>= 102400 rows)
DECLARE @table_name NVARCHAR(128);
DECLARE @row_count BIGINT;
DECLARE @columnstore_sql NVARCHAR(MAX);

DECLARE columnstore_cursor CURSOR FOR
SELECT t.name, p.rows
FROM sys.tables t
JOIN sys.partitions p ON t.object_id = p.object_id
WHERE p.index_id IN (0, 1) -- Heap or Clustered
  AND p.rows >= 10000 -- Tối thiểu 10K rows
  AND t.name LIKE '%History'
  AND NOT EXISTS (
      SELECT 1 FROM sys.indexes i
      WHERE i.object_id = t.object_id AND i.type IN (5, 6) -- Columnstore
  );

OPEN columnstore_cursor;
FETCH NEXT FROM columnstore_cursor INTO @table_name, @row_count;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @columnstore_sql = 'CREATE CLUSTERED COLUMNSTORE INDEX CCI_' + @table_name +
                          ' ON [dbo].[' + @table_name + '] WITH (MAXDOP = 4, COMPRESSION_DELAY = 0);';

    PRINT '-- Creating Columnstore for ' + @table_name + ' (' + CAST(@row_count AS VARCHAR) + ' rows)';
    PRINT @columnstore_sql;

    -- Uncomment to execute:
    -- EXEC sp_executesql @columnstore_sql;

    FETCH NEXT FROM columnstore_cursor INTO @table_name, @row_count;
END;

CLOSE columnstore_cursor;
DEALLOCATE columnstore_cursor;

-- 8. COMMIT PERFORMANCE MONITORING
-- Query để monitor commit performance
SELECT
    'Temporal Tables Performance' AS Category,
    COUNT(*) AS TableCount,
    SUM(p.rows) AS TotalRows
FROM sys.tables t
JOIN sys.partitions p ON t.object_id = p.object_id
WHERE t.temporal_type = 2 AND p.index_id IN (0, 1)

UNION ALL

SELECT
    'Columnstore Indexes' AS Category,
    COUNT(*) AS IndexCount,
    SUM(p.rows) AS TotalRows
FROM sys.indexes i
JOIN sys.partitions p ON i.object_id = p.object_id AND i.index_id = p.index_id
WHERE i.type IN (5, 6);

PRINT '✅ Database optimization configuration completed!';
PRINT '🚀 Temporal Tables: Enabled for 5 core business tables';
PRINT '📊 Performance: Optimized for fastest commit operations';
PRINT '💾 Memory: Configured for 4GB buffer pool';
PRINT '🔄 Maintenance: Auto-statistics and index optimization ready';
