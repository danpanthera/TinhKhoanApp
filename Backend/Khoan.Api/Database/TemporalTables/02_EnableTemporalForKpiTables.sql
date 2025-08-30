-- =============================================
-- Script: 02_EnableTemporalForKpiTables.sql
-- Purpose: Enable Temporal Tables for all KPI tables
-- Author: Agribank Lai Châu Dev Team
-- Date: 2025-01-25
-- Notes: Implements system versioning for all 9 branch KPI tables
-- =============================================

USE TinhKhoanDB;
GO

PRINT '===============================================================';
PRINT 'ENABLING TEMPORAL TABLES FOR KPI ASSIGNMENT TABLES';
PRINT '===============================================================';

-- List of KPI tables to be converted to temporal tables
DECLARE @KpiTables TABLE (TableName NVARCHAR(128));
INSERT INTO @KpiTables (TableName) VALUES
('KPI_CN_MUONG_TE'),
('KPI_CN_NAM_NHUN'),
('KPI_CN_PHONG_THO'),
('KPI_CN_SIN_HO'),
('KPI_CN_TAN_UYEN'),
('KPI_CN_THAN_UYEN'),
('KPI_CN_TTNH_LAI_CHAU'),
('KPI_CN_TTNH_TP_LAI_CHAU'),
('KPI_CN_TUA_CHUA');

-- Add Hội Sở KPI table if needed
INSERT INTO @KpiTables (TableName) VALUES ('KPI_HOI_SO');

-- Loop through each KPI table to add temporal capabilities
DECLARE @TableName NVARCHAR(128);
DECLARE @SQL NVARCHAR(MAX);
DECLARE @HistoryTableName NVARCHAR(128);

DECLARE table_cursor CURSOR FOR SELECT TableName FROM @KpiTables;
OPEN table_cursor;
FETCH NEXT FROM table_cursor INTO @TableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @HistoryTableName = @TableName + '_History';
    
    PRINT '';
    PRINT '--------------------------------------------------------------';
    PRINT 'Processing table: ' + @TableName;
    PRINT '--------------------------------------------------------------';
    
    -- 1. Check if table exists
    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[' + @TableName + ']') AND type in (N'U'))
    BEGIN
        PRINT 'Table ' + @TableName + ' does not exist. Skipping...';
        FETCH NEXT FROM table_cursor INTO @TableName;
        CONTINUE;
    END
    
    -- 2. Check if table is already temporal
    IF EXISTS (SELECT 1 FROM sys.tables WHERE name = @TableName AND temporal_type = 2)
    BEGIN
        PRINT 'Table ' + @TableName + ' is already a temporal table. Skipping...';
        FETCH NEXT FROM table_cursor INTO @TableName;
        CONTINUE;
    END
    
    BEGIN TRY
        -- 3. First, disable any existing versioning if needed
        SET @SQL = 'IF EXISTS (SELECT 1 FROM sys.tables WHERE name = ''' + @TableName + ''' AND temporal_type = 2)
                   BEGIN
                       ALTER TABLE [dbo].[' + @TableName + '] SET (SYSTEM_VERSIONING = OFF);
                   END';
        
        EXEC sp_executesql @SQL;
        PRINT 'Disabled any existing system versioning';
        
        -- 4. Add temporal columns if they don't exist
        SET @SQL = 'IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(''[dbo].[' + @TableName + ']'') AND name = ''ValidFrom'')
                   BEGIN
                       ALTER TABLE [dbo].[' + @TableName + '] ADD
                           [ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL CONSTRAINT DF_' + @TableName + '_ValidFrom DEFAULT SYSUTCDATETIME(),
                           [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL CONSTRAINT DF_' + @TableName + '_ValidTo DEFAULT CONVERT(DATETIME2, ''9999-12-31 23:59:59.9999999''),
                           PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo]);
                   END';
        
        EXEC sp_executesql @SQL;
        PRINT 'Added temporal columns';
        
        -- 5. Enable system versioning
        SET @SQL = 'ALTER TABLE [dbo].[' + @TableName + '] 
                   SET (SYSTEM_VERSIONING = ON (
                       HISTORY_TABLE = [dbo].[' + @HistoryTableName + '],
                       DATA_CONSISTENCY_CHECK = ON,
                       HISTORY_RETENTION_PERIOD = 7 YEARS
                   ))';
        
        EXEC sp_executesql @SQL;
        PRINT 'Enabled system versioning with history table: ' + @HistoryTableName;
        
        -- 6. Add to archive configuration if not exists
        SET @SQL = 'IF NOT EXISTS (SELECT 1 FROM dbo.ArchiveConfiguration WHERE TableName = ''' + @TableName + ''')
                   BEGIN
                       INSERT INTO dbo.ArchiveConfiguration (TableName, RetentionMonths, ArchiveToPartition, CompressionEnabled)
                       VALUES (''' + @TableName + ''', 24, 1, 1);
                   END';
        
        IF OBJECT_ID('dbo.ArchiveConfiguration', 'U') IS NOT NULL
        BEGIN
            EXEC sp_executesql @SQL;
            PRINT 'Added to archive configuration';
        END
        
        PRINT 'Successfully enabled temporal features for ' + @TableName;
    END TRY
    BEGIN CATCH
        PRINT 'Error enabling temporal features for ' + @TableName + ':';
        PRINT ERROR_MESSAGE();
    END CATCH
    
    FETCH NEXT FROM table_cursor INTO @TableName;
END

CLOSE table_cursor;
DEALLOCATE table_cursor;

PRINT '';
PRINT '===============================================================';
PRINT 'TEMPORAL TABLE SETUP COMPLETE';
PRINT '===============================================================';
GO
