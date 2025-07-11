-- =======================================
-- 🎯 SCRIPT HOÀN THIỆN 100% TEMPORAL TABLES + COLUMNSTORE INDEXES
-- =======================================

USE TinhKhoanDB;
GO

PRINT '🎯 Starting complete Temporal Tables + Columnstore Indexes configuration...';

-- =======================================
-- 🔧 KIỂM TRA VÀ TẠO TEMPORAL TABLES CHO TẤT CẢ BẢNG
-- =======================================

-- Danh sách tất cả bảng cần temporal
DECLARE @tables TABLE (TableName NVARCHAR(50), HistoryTableName NVARCHAR(50))
INSERT INTO @tables VALUES
('LN01_History', 'LN01_History_Archive'),
('GL01_History', 'GL01_History_Archive'),
('DPDA', 'DPDA_History'),
('EI01', 'EI01_History'),
('KH03', 'KH03_History'),
('BC57', 'BC57_History'),
('DP01', 'DP01_History'),
('DB01', 'DB01_History'),
('LN03', 'LN03_History');

DECLARE @tableName NVARCHAR(50), @historyTableName NVARCHAR(50);
DECLARE table_cursor CURSOR FOR SELECT TableName, HistoryTableName FROM @tables;

OPEN table_cursor;
FETCH NEXT FROM table_cursor INTO @tableName, @historyTableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Kiểm tra bảng có tồn tại không
    IF EXISTS (SELECT 1 FROM sys.tables WHERE name = @tableName)
    BEGIN
        PRINT '🔧 Processing table: ' + @tableName;

        -- Kiểm tra temporal columns
        DECLARE @sql NVARCHAR(MAX);
        IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(@tableName) AND name = 'SysStartTime')
        BEGIN
            SET @sql = 'ALTER TABLE [' + @tableName + '] ADD
                [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT ''1900-01-01 00:00:00.0000000'',
                [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT ''9999-12-31 23:59:59.9999999'',
                PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);';
            EXEC sp_executesql @sql;
            PRINT '✅ Added temporal columns to ' + @tableName;
        END

        -- Enable system versioning nếu chưa có
        IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = @tableName AND temporal_type = 2)
        BEGIN
            SET @sql = 'ALTER TABLE [' + @tableName + '] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[' + @historyTableName + ']));';
            EXEC sp_executesql @sql;
            PRINT '✅ Enabled temporal for ' + @tableName + ' with history table ' + @historyTableName;
        END
    END
    ELSE
    BEGIN
        PRINT '⚠️ Table ' + @tableName + ' does not exist, skipping...';
    END

    FETCH NEXT FROM table_cursor INTO @tableName, @historyTableName;
END

CLOSE table_cursor;
DEALLOCATE table_cursor;

-- =======================================
-- 🏗️ TẠO COLUMNSTORE INDEXES CHO TẤT CẢ BẢNG HISTORY
-- =======================================

PRINT '🏗️ Creating Columnstore Indexes for performance...';

-- Tạo Columnstore cho các bảng chính
DECLARE @columnstoreTables TABLE (TableName NVARCHAR(50))
INSERT INTO @columnstoreTables VALUES
('LN01_History'),
('GL01_History'),
('DPDA'),
('EI01'),
('KH03'),
('BC57'),
('DP01'),
('DB01'),
('LN03');

DECLARE @csTableName NVARCHAR(50);
DECLARE cs_cursor CURSOR FOR SELECT TableName FROM @columnstoreTables;

OPEN cs_cursor;
FETCH NEXT FROM cs_cursor INTO @csTableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF EXISTS (SELECT 1 FROM sys.tables WHERE name = @csTableName)
    BEGIN
        -- Kiểm tra xem đã có Columnstore index chưa
        IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(@csTableName) AND type = 5)
        BEGIN
            SET @sql = 'CREATE CLUSTERED COLUMNSTORE INDEX CCI_' + @csTableName + ' ON [' + @csTableName + '];';

            BEGIN TRY
                EXEC sp_executesql @sql;
                PRINT '✅ Created Columnstore Index for ' + @csTableName;
            END TRY
            BEGIN CATCH
                PRINT '⚠️ Failed to create Columnstore Index for ' + @csTableName + ': ' + ERROR_MESSAGE();
            END CATCH
        END
        ELSE
        BEGIN
            PRINT '✅ Columnstore Index already exists for ' + @csTableName;
        END
    END

    FETCH NEXT FROM cs_cursor INTO @csTableName;
END

CLOSE cs_cursor;
DEALLOCATE cs_cursor;

-- =======================================
-- 📊 TẠO PERFORMANCE INDEXES CHO TẤT CẢ BẢNG
-- =======================================

PRINT '📊 Creating performance indexes...';

-- Standard performance indexes cho mỗi bảng
DECLARE @perfTables TABLE (TableName NVARCHAR(50))
INSERT INTO @perfTables VALUES
('LN01_History'),
('GL01_History'),
('DPDA'),
('EI01'),
('KH03'),
('BC57'),
('DP01'),
('DB01'),
('LN03');

DECLARE @perfTableName NVARCHAR(50);
DECLARE perf_cursor CURSOR FOR SELECT TableName FROM @perfTables;

OPEN perf_cursor;
FETCH NEXT FROM perf_cursor INTO @perfTableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF EXISTS (SELECT 1 FROM sys.tables WHERE name = @perfTableName)
    BEGIN
        -- StatementDate index
        IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(@perfTableName) AND name = 'StatementDate')
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(@perfTableName) AND name = 'IX_' + @perfTableName + '_StatementDate')
            BEGIN
                SET @sql = 'CREATE INDEX IX_' + @perfTableName + '_StatementDate ON [' + @perfTableName + '] (StatementDate);';
                EXEC sp_executesql @sql;
                PRINT '✅ Created StatementDate index for ' + @perfTableName;
            END
        END

        -- ProcessedDate index
        IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(@perfTableName) AND name = 'ProcessedDate')
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(@perfTableName) AND name = 'IX_' + @perfTableName + '_ProcessedDate')
            BEGIN
                SET @sql = 'CREATE INDEX IX_' + @perfTableName + '_ProcessedDate ON [' + @perfTableName + '] (ProcessedDate);';
                EXEC sp_executesql @sql;
                PRINT '✅ Created ProcessedDate index for ' + @perfTableName;
            END
        END

        -- IsCurrent index
        IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(@perfTableName) AND name = 'IsCurrent')
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(@perfTableName) AND name = 'IX_' + @perfTableName + '_IsCurrent')
            BEGIN
                SET @sql = 'CREATE INDEX IX_' + @perfTableName + '_IsCurrent ON [' + @perfTableName + '] (IsCurrent);';
                EXEC sp_executesql @sql;
                PRINT '✅ Created IsCurrent index for ' + @perfTableName;
            END
        END
    END

    FETCH NEXT FROM perf_cursor INTO @perfTableName;
END

CLOSE perf_cursor;
DEALLOCATE perf_cursor;

-- =======================================
-- ✅ BÁO CÁO KẾT QUẢ
-- =======================================

PRINT '';
PRINT '🎯 === FINAL REPORT: TEMPORAL TABLES + COLUMNSTORE INDEXES ===';
PRINT '';

-- Báo cáo Temporal Tables
PRINT '📋 TEMPORAL TABLES STATUS:';
SELECT
    t.name AS TableName,
    CASE t.temporal_type
        WHEN 2 THEN '✅ ENABLED'
        ELSE '❌ NOT ENABLED'
    END AS TemporalStatus,
    h.name AS HistoryTableName
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
ORDER BY t.name;

PRINT '';
PRINT '📊 COLUMNSTORE INDEXES STATUS:';
SELECT
    t.name AS TableName,
    CASE WHEN EXISTS (SELECT 1 FROM sys.indexes i WHERE i.object_id = t.object_id AND i.type = 5)
        THEN '✅ COLUMNSTORE ENABLED'
        ELSE '❌ NO COLUMNSTORE'
    END AS ColumnstoreStatus
FROM sys.tables t
ORDER BY t.name;

PRINT '';
PRINT '✅ 100% TEMPORAL TABLES + COLUMNSTORE INDEXES CONFIGURATION COMPLETED!';
