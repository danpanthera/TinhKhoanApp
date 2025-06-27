-- =======================================
-- 🎯 SCRIPT HOÀN THIỆN TEMPORAL CONFIGURATION CHO CÁC BẢNG CÒN THIẾU
-- =======================================

USE TinhKhoanDB;
GO

PRINT '🎯 Completing temporal configuration for existing tables...';

-- =======================================
-- 🔧 THÊM TEMPORAL COLUMNS VÀ KÍCH HOẠT TEMPORAL CHO CÁC BẢNG CÒN THIẾU
-- =======================================

-- 1. DPDA Table
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DPDA' AND temporal_type = 0)
BEGIN
    PRINT '🔧 Configuring temporal for DPDA...';

    -- Add temporal columns if not exist
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('DPDA') AND name = 'SysStartTime')
    BEGIN
        ALTER TABLE [DPDA] ADD
            [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT '1900-01-01 00:00:00.0000000',
            [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
            PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
    END

    -- Enable system versioning
    ALTER TABLE [DPDA] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[DPDA_History]));
    PRINT '✅ DPDA temporal configuration completed.';
END

-- 2. EI01 Table
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'EI01' AND temporal_type = 0)
BEGIN
    PRINT '🔧 Configuring temporal for EI01...';

    -- Add temporal columns if not exist
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('EI01') AND name = 'SysStartTime')
    BEGIN
        ALTER TABLE [EI01] ADD
            [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT '1900-01-01 00:00:00.0000000',
            [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
            PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
    END

    -- Enable system versioning
    ALTER TABLE [EI01] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[EI01_History]));
    PRINT '✅ EI01 temporal configuration completed.';
END

-- 3. GAHR26 Table
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GAHR26' AND temporal_type = 0)
BEGIN
    PRINT '🔧 Configuring temporal for GAHR26...';

    -- Add temporal columns if not exist
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('GAHR26') AND name = 'SysStartTime')
    BEGIN
        ALTER TABLE [GAHR26] ADD
            [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT '1900-01-01 00:00:00.0000000',
            [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
            PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
    END

    -- Enable system versioning
    ALTER TABLE [GAHR26] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[GAHR26_History]));
    PRINT '✅ GAHR26 temporal configuration completed.';
END

-- 4. GLCB41 Table
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GLCB41' AND temporal_type = 0)
BEGIN
    PRINT '🔧 Configuring temporal for GLCB41...';

    -- Add temporal columns if not exist
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('GLCB41') AND name = 'SysStartTime')
    BEGIN
        ALTER TABLE [GLCB41] ADD
            [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT '1900-01-01 00:00:00.0000000',
            [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
            PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
    END

    -- Enable system versioning
    ALTER TABLE [GLCB41] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[GLCB41_History]));
    PRINT '✅ GLCB41 temporal configuration completed.';
END

-- =======================================
-- 📊 TẠO COLUMNSTORE INDEXES CHO HISTORY TABLES
-- =======================================
PRINT '📊 Creating Columnstore Indexes for performance...';

-- Create columnstore indexes for history tables if they don't exist
DECLARE @tables TABLE (TableName NVARCHAR(128));
INSERT INTO @tables VALUES
('7800_DT_KHKD1_History'),
('DPDA_History'),
('EI01_History'),
('GAHR26_History'),
('GLCB41_History');

DECLARE @table NVARCHAR(128);
DECLARE @sql NVARCHAR(MAX);

DECLARE table_cursor CURSOR FOR
SELECT TableName FROM @tables
WHERE EXISTS (SELECT 1 FROM sys.tables WHERE name = TableName)
  AND NOT EXISTS (
      SELECT 1 FROM sys.indexes i
      INNER JOIN sys.tables t ON i.object_id = t.object_id
      WHERE t.name = TableName AND i.type IN (5, 6)
  );

OPEN table_cursor;
FETCH NEXT FROM table_cursor INTO @table;

WHILE @@FETCH_STATUS = 0
BEGIN
    BEGIN TRY
        SET @sql = 'CREATE CLUSTERED COLUMNSTORE INDEX IX_' + @table + '_ColumnStore ON [' + @table + '];';
        EXEC sp_executesql @sql;
        PRINT '✅ Created Columnstore Index for: ' + @table;
    END TRY
    BEGIN CATCH
        PRINT '⚠️ Warning: Could not create columnstore index for ' + @table + ': ' + ERROR_MESSAGE();
    END CATCH

    FETCH NEXT FROM table_cursor INTO @table;
END

CLOSE table_cursor;
DEALLOCATE table_cursor;

-- =======================================
-- 📋 KIỂM TRA KẾT QUẢ CUỐI CÙNG
-- =======================================
PRINT '📋 Final verification...';

SELECT
    t.name AS TableName,
    t.temporal_type_desc AS TemporalType,
    h.name AS HistoryTableName,
    CASE
        WHEN EXISTS (
            SELECT 1 FROM sys.indexes i
            WHERE i.object_id = h.object_id
            AND i.type IN (5, 6)
        ) THEN '✅ Has Columnstore'
        ELSE '❌ Missing Columnstore'
    END AS ColumnstoreStatus,
    CASE
        WHEN t.temporal_type = 2 AND h.object_id IS NOT NULL THEN '✅ Complete'
        WHEN t.temporal_type = 2 THEN '⚠️ Temporal but no history'
        WHEN t.object_id IS NOT NULL THEN '❌ No temporal'
        ELSE '❌ Missing table'
    END AS Status
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
WHERE t.name IN ('7800_DT_KHKD1', 'DPDA', 'EI01', 'GAHR26', 'GLCB41')
ORDER BY t.name;

PRINT '🚀 Temporal configuration completed!';

GO
