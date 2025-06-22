-- =============================================
-- Script: 04_VerifyTemporalConfiguration.sql
-- Purpose: Verify all temporal tables are properly configured
-- Author: Agribank Lai Châu Dev Team
-- Date: 2025-01-25
-- Notes: Comprehensive validation of temporal tables and columnstore indexes
-- =============================================

USE TinhKhoanDB;
GO

PRINT '===============================================================';
PRINT 'VERIFYING TEMPORAL TABLE CONFIGURATION';
PRINT '===============================================================';

-- Create a temporary results table to store verification results
IF OBJECT_ID('tempdb..#TemporalVerification') IS NOT NULL
    DROP TABLE #TemporalVerification;

CREATE TABLE #TemporalVerification (
    Id INT IDENTITY(1,1),
    TableName NVARCHAR(128),
    HistoryTableName NVARCHAR(128) NULL,
    IsTemporalEnabled BIT,
    HasValidFromColumn BIT,
    HasValidToColumn BIT,
    HasSystemTimePeriod BIT,
    HasHistoryTable BIT,
    HasRetentionPeriod BIT,
    HistoryTableHasColumnstoreIndex BIT,
    Status NVARCHAR(20),
    Issues NVARCHAR(MAX) NULL
);

-- List of tables that should be temporal
DECLARE @ExpectedTemporalTables TABLE (TableName NVARCHAR(128));

-- KPI Tables
INSERT INTO @ExpectedTemporalTables (TableName) VALUES
('KPI_CN_MUONG_TE'),
('KPI_CN_NAM_NHUN'),
('KPI_CN_PHONG_THO'),
('KPI_CN_SIN_HO'),
('KPI_CN_TAN_UYEN'),
('KPI_CN_THAN_UYEN'),
('KPI_CN_TTNH_LAI_CHAU'),
('KPI_CN_TTNH_TP_LAI_CHAU'),
('KPI_CN_TUA_CHUA'),
('KPI_HOI_SO');

-- RawData Tables
INSERT INTO @ExpectedTemporalTables (TableName) VALUES
('RawDataImports'),
('RawDataRecords');

-- Check each expected temporal table
DECLARE @TableName NVARCHAR(128);
DECLARE @HistoryTableName NVARCHAR(128);
DECLARE @Issues NVARCHAR(MAX);

DECLARE table_cursor CURSOR FOR SELECT TableName FROM @ExpectedTemporalTables;
OPEN table_cursor;
FETCH NEXT FROM table_cursor INTO @TableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @Issues = '';
    SET @HistoryTableName = @TableName + '_History';
    
    -- Verify the table exists
    IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[' + @TableName + ']') AND type in (N'U'))
    BEGIN
        INSERT INTO #TemporalVerification (
            TableName, HistoryTableName, IsTemporalEnabled, HasValidFromColumn, HasValidToColumn,
            HasSystemTimePeriod, HasHistoryTable, HasRetentionPeriod, HistoryTableHasColumnstoreIndex,
            Status, Issues
        )
        VALUES (
            @TableName, NULL, 0, 0, 0, 0, 0, 0, 0, 'MISSING', 'Table does not exist'
        );
        
        FETCH NEXT FROM table_cursor INTO @TableName;
        CONTINUE;
    END
    
    -- Check if temporal is enabled
    DECLARE @IsTemporalEnabled BIT = 0;
    SELECT @IsTemporalEnabled = 1 FROM sys.tables WHERE name = @TableName AND temporal_type = 2;
    
    -- Check for ValidFrom/ValidTo columns
    DECLARE @HasValidFromColumn BIT = 0;
    DECLARE @HasValidToColumn BIT = 0;
    
    SELECT @HasValidFromColumn = 1
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[' + @TableName + ']')
    AND name = 'ValidFrom';
    
    SELECT @HasValidToColumn = 1
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[' + @TableName + ']')
    AND name = 'ValidTo';
    
    -- Check for SYSTEM_TIME period
    DECLARE @HasSystemTimePeriod BIT = 0;
    SELECT @HasSystemTimePeriod = 1
    FROM sys.periods
    WHERE object_id = OBJECT_ID(N'[dbo].[' + @TableName + ']');
    
    -- Check for history table
    DECLARE @HasHistoryTable BIT = 0;
    SELECT @HasHistoryTable = 1
    FROM sys.tables
    WHERE name = @HistoryTableName;
    
    -- Check for retention policy
    DECLARE @HasRetentionPeriod BIT = 0;
    SELECT @HasRetentionPeriod = 1
    FROM sys.tables t
    JOIN sys.periods p ON t.object_id = p.object_id
    WHERE t.name = @TableName
    AND t.history_retention_period_unit IS NOT NULL;
    
    -- Check for columnstore index on history table
    DECLARE @HistoryTableHasColumnstoreIndex BIT = 0;
    IF @HasHistoryTable = 1
    BEGIN
        SELECT @HistoryTableHasColumnstoreIndex = 1
        FROM sys.indexes i
        JOIN sys.tables t ON i.object_id = t.object_id
        WHERE t.name = @HistoryTableName
        AND i.type = 5; -- Clustered columnstore index
    END
    
    -- Build issues list
    IF @IsTemporalEnabled = 0 SET @Issues = @Issues + 'Temporal not enabled; ';
    IF @HasValidFromColumn = 0 SET @Issues = @Issues + 'Missing ValidFrom; ';
    IF @HasValidToColumn = 0 SET @Issues = @Issues + 'Missing ValidTo; ';
    IF @HasSystemTimePeriod = 0 SET @Issues = @Issues + 'No SYSTEM_TIME period; ';
    IF @HasHistoryTable = 0 SET @Issues = @Issues + 'No history table; ';
    IF @HasRetentionPeriod = 0 SET @Issues = @Issues + 'No retention policy; ';
    IF @HasHistoryTable = 1 AND @HistoryTableHasColumnstoreIndex = 0 SET @Issues = @Issues + 'No columnstore index on history; ';
    
    -- Determine status
    DECLARE @Status NVARCHAR(20);
    IF LEN(@Issues) = 0
        SET @Status = 'OK';
    ELSE IF @IsTemporalEnabled = 0
        SET @Status = 'NOT TEMPORAL';
    ELSE
        SET @Status = 'ISSUES';
    
    -- Insert verification result
    INSERT INTO #TemporalVerification (
        TableName, HistoryTableName, IsTemporalEnabled, HasValidFromColumn, HasValidToColumn,
        HasSystemTimePeriod, HasHistoryTable, HasRetentionPeriod, HistoryTableHasColumnstoreIndex,
        Status, Issues
    )
    VALUES (
        @TableName, @HistoryTableName, @IsTemporalEnabled, @HasValidFromColumn, @HasValidToColumn,
        @HasSystemTimePeriod, @HasHistoryTable, @HasRetentionPeriod, @HistoryTableHasColumnstoreIndex,
        @Status, CASE WHEN LEN(@Issues) > 0 THEN @Issues ELSE NULL END
    );
    
    FETCH NEXT FROM table_cursor INTO @TableName;
END

CLOSE table_cursor;
DEALLOCATE table_cursor;

-- Display verification results
PRINT '';
PRINT 'TEMPORAL TABLE VERIFICATION RESULTS:';
PRINT '------------------------------------';

-- First display summary
SELECT 
    SUM(CASE WHEN Status = 'OK' THEN 1 ELSE 0 END) AS [OK],
    SUM(CASE WHEN Status = 'ISSUES' THEN 1 ELSE 0 END) AS [HasIssues],
    SUM(CASE WHEN Status = 'NOT TEMPORAL' THEN 1 ELSE 0 END) AS [NotTemporal],
    SUM(CASE WHEN Status = 'MISSING' THEN 1 ELSE 0 END) AS [Missing],
    COUNT(*) AS [Total]
FROM #TemporalVerification;

-- Then display detailed results
SELECT 
    TableName,
    Status,
    CASE 
        WHEN IsTemporalEnabled = 1 THEN 'Yes' 
        ELSE 'No' 
    END AS [Temporal],
    CASE 
        WHEN HasHistoryTable = 1 THEN 'Yes' 
        ELSE 'No' 
    END AS [HasHistory],
    CASE 
        WHEN HasHistoryTable = 1 AND HistoryTableHasColumnstoreIndex = 1 THEN 'Yes' 
        WHEN HasHistoryTable = 1 THEN 'No' 
        ELSE 'N/A' 
    END AS [HasColumnstore],
    Issues
FROM #TemporalVerification
ORDER BY 
    CASE Status 
        WHEN 'MISSING' THEN 1 
        WHEN 'NOT TEMPORAL' THEN 2 
        WHEN 'ISSUES' THEN 3 
        ELSE 4 
    END,
    TableName;

-- Generate fix scripts for tables with issues
PRINT '';
PRINT 'FIX SCRIPTS FOR TABLES WITH ISSUES:';
PRINT '-----------------------------------';

DECLARE @FixSQL NVARCHAR(MAX);
DECLARE fix_cursor CURSOR FOR 
    SELECT TableName, Status, Issues FROM #TemporalVerification 
    WHERE Status IN ('ISSUES', 'NOT TEMPORAL')
    ORDER BY TableName;

OPEN fix_cursor;
FETCH NEXT FROM fix_cursor INTO @TableName, @Status, @Issues;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT '';
    PRINT '-- Fix for table: ' + @TableName;
    PRINT '-- Issues: ' + @Issues;
    
    SET @HistoryTableName = @TableName + '_History';
    
    -- Generate fix script based on issues
    SET @FixSQL = '';
    
    -- If not temporal at all, generate full temporal script
    IF @Status = 'NOT TEMPORAL'
    BEGIN
        SET @FixSQL = @FixSQL + '
-- Make ' + @TableName + ' temporal
BEGIN TRY
    -- Add temporal columns if they don''t exist
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(''[dbo].[' + @TableName + ']'') AND name = ''ValidFrom'')
    BEGIN
        ALTER TABLE [dbo].[' + @TableName + '] ADD
            [ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL CONSTRAINT DF_' + @TableName + '_ValidFrom DEFAULT SYSUTCDATETIME(),
            [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL CONSTRAINT DF_' + @TableName + '_ValidTo DEFAULT CONVERT(DATETIME2, ''9999-12-31 23:59:59.9999999''),
            PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo]);
    END
    
    -- Enable system versioning
    ALTER TABLE [dbo].[' + @TableName + '] 
    SET (SYSTEM_VERSIONING = ON (
        HISTORY_TABLE = [dbo].[' + @HistoryTableName + '],
        DATA_CONSISTENCY_CHECK = ON,
        HISTORY_RETENTION_PERIOD = 7 YEARS
    ));
    
    -- Create columnstore index on history table
    CREATE CLUSTERED COLUMNSTORE INDEX CIDX_' + @HistoryTableName + ' ON [dbo].[' + @HistoryTableName + ']
    WITH (DROP_EXISTING = OFF, COMPRESSION_DELAY = 0);
    
    -- Create nonclustered index on ValidFrom/ValidTo for temporal queries
    CREATE NONCLUSTERED INDEX IX_' + @HistoryTableName + '_ValidFromTo ON [dbo].[' + @HistoryTableName + '] 
    ([ValidFrom], [ValidTo])
    WITH (DATA_COMPRESSION = PAGE);
END TRY
BEGIN CATCH
    PRINT ''Error fixing ' + @TableName + ': '' + ERROR_MESSAGE();
END CATCH
';
    END
    ELSE
    BEGIN
        -- Fix specific issues
        IF CHARINDEX('No columnstore index on history', @Issues) > 0
        BEGIN
            SET @FixSQL = @FixSQL + '
-- Add columnstore index to ' + @HistoryTableName + '
CREATE CLUSTERED COLUMNSTORE INDEX CIDX_' + @HistoryTableName + ' ON [dbo].[' + @HistoryTableName + ']
WITH (DROP_EXISTING = OFF, COMPRESSION_DELAY = 0);
';
        END
        
        IF CHARINDEX('No retention policy', @Issues) > 0
        BEGIN
            SET @FixSQL = @FixSQL + '
-- Disable system versioning temporarily to add retention policy
ALTER TABLE [dbo].[' + @TableName + '] SET (SYSTEM_VERSIONING = OFF);

-- Re-enable with retention policy
ALTER TABLE [dbo].[' + @TableName + '] 
SET (SYSTEM_VERSIONING = ON (
    HISTORY_TABLE = [dbo].[' + @HistoryTableName + '],
    DATA_CONSISTENCY_CHECK = ON,
    HISTORY_RETENTION_PERIOD = 7 YEARS
));
';
        END
    END
    
    PRINT @FixSQL;
    
    FETCH NEXT FROM fix_cursor INTO @TableName, @Status, @Issues;
END

CLOSE fix_cursor;
DEALLOCATE fix_cursor;

-- Clean up
DROP TABLE #TemporalVerification;

PRINT '';
PRINT '===============================================================';
PRINT 'VERIFICATION COMPLETE';
PRINT '===============================================================';
GO
