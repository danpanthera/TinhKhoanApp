-- =======================================
-- ⚡ SIMPLE SCRIPT - ENABLE TEMPORAL FOR REMAINING TABLES
-- =======================================

USE TinhKhoanDB;
GO

PRINT '⚡ Enabling temporal for remaining tables...';

-- DPDA - Enable temporal (it already has temporal columns)
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DPDA' AND temporal_type = 0)
BEGIN
    PRINT '🔧 Enabling temporal for DPDA...';
    BEGIN TRY
        -- Simply enable system versioning since columns already exist
        ALTER TABLE [DPDA] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[DPDA_History]));
        PRINT '✅ DPDA temporal enabled.';
    END TRY
    BEGIN CATCH
        PRINT '⚠️ Error enabling DPDA temporal: ' + ERROR_MESSAGE();
    END CATCH
END

-- EI01 - Add temporal columns and enable
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'EI01' AND temporal_type = 0)
BEGIN
    PRINT '🔧 Enabling temporal for EI01...';
    BEGIN TRY
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
        PRINT '✅ EI01 temporal enabled.';
    END TRY
    BEGIN CATCH
        PRINT '⚠️ Error enabling EI01 temporal: ' + ERROR_MESSAGE();
    END CATCH
END

-- GAHR26 - Add temporal columns and enable
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GAHR26' AND temporal_type = 0)
BEGIN
    PRINT '🔧 Enabling temporal for GAHR26...';
    BEGIN TRY
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
        PRINT '✅ GAHR26 temporal enabled.';
    END TRY
    BEGIN CATCH
        PRINT '⚠️ Error enabling GAHR26 temporal: ' + ERROR_MESSAGE();
    END CATCH
END

-- GLCB41 - Add temporal columns and enable
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GLCB41' AND temporal_type = 0)
BEGIN
    PRINT '🔧 Enabling temporal for GLCB41...';
    BEGIN TRY
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
        PRINT '✅ GLCB41 temporal enabled.';
    END TRY
    BEGIN CATCH
        PRINT '⚠️ Error enabling GLCB41 temporal: ' + ERROR_MESSAGE();
    END CATCH
END

-- Create columnstore indexes for all history tables
PRINT '📊 Creating Columnstore Indexes...';

DECLARE @index_tables TABLE (TableName NVARCHAR(128));
INSERT INTO @index_tables VALUES
('DPDA_History'),
('EI01_History'),
('GAHR26_History'),
('GLCB41_History');

DECLARE @table NVARCHAR(128);
DECLARE @sql NVARCHAR(MAX);

DECLARE table_cursor CURSOR FOR
SELECT TableName FROM @index_tables
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

-- Final status check
PRINT '📋 Final status check:';

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
    END AS ColumnstoreStatus
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
ORDER BY t.name;

PRINT '🚀 Completed!';

GO
