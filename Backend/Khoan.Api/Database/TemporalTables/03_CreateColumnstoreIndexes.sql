-- =============================================
-- Script: 03_CreateColumnstoreIndexes.sql
-- Purpose: Add columnstore indexes to history tables for improved performance
-- Author: Agribank Lai Châu Dev Team
-- Date: 2025-01-25
-- Notes: Significantly improves query performance and storage compression for historical data
-- =============================================

USE TinhKhoanDB;
GO

PRINT '===============================================================';
PRINT 'CREATING COLUMNSTORE INDEXES FOR HISTORY TABLES';
PRINT '===============================================================';

-- List of history tables that should have columnstore indexes
DECLARE @HistoryTables TABLE (TableName NVARCHAR(128));

-- KPI History Tables
INSERT INTO @HistoryTables (TableName) VALUES
('KPI_CN_MUONG_TE_History'),
('KPI_CN_NAM_NHUN_History'),
('KPI_CN_PHONG_THO_History'),
('KPI_CN_SIN_HO_History'),
('KPI_CN_TAN_UYEN_History'),
('KPI_CN_THAN_UYEN_History'),
('KPI_CN_TTNH_LAI_CHAU_History'),
('KPI_CN_TTNH_TP_LAI_CHAU_History'),
('KPI_CN_TUA_CHUA_History'),
('KPI_HOI_SO_History');

-- RawData History Tables
INSERT INTO @HistoryTables (TableName) VALUES
('RawDataImports_History'),
('RawDataRecords_History');

-- Loop through each history table to add columnstore indexes
DECLARE @TableName NVARCHAR(128);
DECLARE @SQL NVARCHAR(MAX);
DECLARE @IndexName NVARCHAR(128);

DECLARE table_cursor CURSOR FOR SELECT TableName FROM @HistoryTables;
OPEN table_cursor;
FETCH NEXT FROM table_cursor INTO @TableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @IndexName = 'CIDX_' + @TableName;
    
    PRINT '';
    PRINT '--------------------------------------------------------------';
    PRINT 'Processing history table: ' + @TableName;
    PRINT '--------------------------------------------------------------';
    
    -- 1. Check if table exists
    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[' + @TableName + ']') AND type in (N'U'))
    BEGIN
        PRINT 'Table ' + @TableName + ' does not exist. Skipping...';
        FETCH NEXT FROM table_cursor INTO @TableName;
        CONTINUE;
    END
    
    -- 2. Check if columnstore index already exists
    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = @IndexName AND object_id = OBJECT_ID(N'[dbo].[' + @TableName + ']'))
    BEGIN
        PRINT 'Columnstore index ' + @IndexName + ' already exists. Skipping...';
        FETCH NEXT FROM table_cursor INTO @TableName;
        CONTINUE;
    END
    
    BEGIN TRY
        -- 3. Create the columnstore index
        SET @SQL = 'CREATE CLUSTERED COLUMNSTORE INDEX ' + @IndexName + ' ON [dbo].[' + @TableName + ']
                   WITH (DROP_EXISTING = OFF, COMPRESSION_DELAY = 0)';
        
        EXEC sp_executesql @SQL;
        PRINT 'Created columnstore index ' + @IndexName + ' on ' + @TableName;
        
        -- 4. Add a nonclustered index on ValidFrom and ValidTo for efficient temporal queries
        SET @SQL = 'CREATE NONCLUSTERED INDEX IX_' + @TableName + '_ValidFromTo ON [dbo].[' + @TableName + '] 
                    ([ValidFrom], [ValidTo])
                    WITH (DATA_COMPRESSION = PAGE)';
        
        EXEC sp_executesql @SQL;
        PRINT 'Created nonclustered index on ValidFrom/ValidTo on ' + @TableName;
        
        -- 5. Add archiving metadata if needed
        IF @TableName LIKE '%RawData%'
        BEGIN
            -- For RawData tables, add indexes on important fields for data analysis
            IF @TableName = 'RawDataImports_History'
            BEGIN
                SET @SQL = 'CREATE NONCLUSTERED INDEX IX_' + @TableName + '_ImportDate ON [dbo].[' + @TableName + '] 
                           (ImportDate)
                           INCLUDE (BranchCode, DepartmentCode, CreatedDate)
                           WITH (DATA_COMPRESSION = PAGE)';
                
                EXEC sp_executesql @SQL;
                PRINT 'Created additional index for data analysis on ' + @TableName;
            END
        END
        
        PRINT 'Successfully optimized ' + @TableName + ' with columnstore technology';
    END TRY
    BEGIN CATCH
        PRINT 'Error creating columnstore index for ' + @TableName + ':';
        PRINT ERROR_MESSAGE();
    END CATCH
    
    FETCH NEXT FROM table_cursor INTO @TableName;
END

CLOSE table_cursor;
DEALLOCATE table_cursor;

PRINT '';
PRINT '===============================================================';
PRINT 'COLUMNSTORE INDEX CREATION COMPLETE';
PRINT '===============================================================';

-- Add performance monitoring and optimization tips
PRINT '';
PRINT 'PERFORMANCE MONITORING TIPS:';
PRINT '----------------------------';
PRINT '1. Monitor compression ratio: SELECT object_name(object_id), * FROM sys.dm_db_column_store_row_group_physical_stats';
PRINT '2. Check fragmentation: SELECT object_name(object_id), * FROM sys.dm_db_column_store_row_group_operational_stats';
PRINT '3. Rebuild if needed: ALTER INDEX CIDX_TableName_History ON TableName_History REBUILD';
PRINT '';
PRINT 'NEXT STEPS:';
PRINT '-----------';
PRINT '1. Run 04_VerifyTemporalConfiguration.sql to validate setup';
PRINT '2. Set up regular archiving jobs using the ArchiveTemporalData stored procedure';
PRINT '3. Consider partitioning strategy for very large history tables (>10M rows)';
GO
