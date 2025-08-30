-- =============================================
-- Script: 05_ArchivingStrategy.sql
-- Purpose: Automated Archiving Strategy for Temporal Tables
-- Author: System Architecture
-- Date: 2025-01-15
-- Notes: High-performance archiving with compression for long-term storage
-- =============================================

USE TinhKhoanDB;
GO

-- 1. Create Archive Configuration Table
CREATE TABLE dbo.ArchiveConfiguration
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TableName NVARCHAR(128) NOT NULL,
    RetentionMonths INT NOT NULL DEFAULT 24, -- Keep 2 years by default
    ArchiveToPartition BIT NOT NULL DEFAULT 1,
    CompressionEnabled BIT NOT NULL DEFAULT 1,
    LastArchiveDate DATETIME2 NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ModifiedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    
    CONSTRAINT UQ_ArchiveConfiguration_TableName UNIQUE (TableName)
);
GO

-- Insert default configuration
INSERT INTO dbo.ArchiveConfiguration (TableName, RetentionMonths, ArchiveToPartition, CompressionEnabled)
VALUES ('RawDataImports', 24, 1, 1);
GO

-- 2. Create Archive Statistics Table
CREATE TABLE dbo.ArchiveStatistics
(
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    TableName NVARCHAR(128) NOT NULL,
    ArchiveDate DATETIME2 NOT NULL,
    RecordsArchived BIGINT NOT NULL,
    DataSizeMB DECIMAL(18,2) NOT NULL,
    CompressionRatio DECIMAL(5,2) NULL,
    ArchiveDurationMs INT NOT NULL,
    Status NVARCHAR(20) NOT NULL, -- Success, Failed, InProgress
    ErrorMessage NVARCHAR(MAX) NULL,
    
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

-- 3. Main Archiving Procedure for Temporal Tables
CREATE PROCEDURE sp_ArchiveTemporalData
    @TableName NVARCHAR(128) = 'RawDataImports',
    @ForceArchive BIT = 0,
    @DryRun BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    
    DECLARE @StartTime DATETIME2 = SYSUTCDATETIME();
    DECLARE @RetentionMonths INT;
    DECLARE @ArchiveToPartition BIT;
    DECLARE @CompressionEnabled BIT;
    DECLARE @CutoffDate DATETIME2;
    DECLARE @RecordsToArchive BIGINT = 0;
    DECLARE @RecordsArchived BIGINT = 0;
    DECLARE @DataSizeMB DECIMAL(18,2) = 0;
    DECLARE @CompressionRatio DECIMAL(5,2);
    DECLARE @StatId BIGINT;
    
    BEGIN TRY
        -- Get archive configuration
        SELECT 
            @RetentionMonths = RetentionMonths,
            @ArchiveToPartition = ArchiveToPartition,
            @CompressionEnabled = CompressionEnabled
        FROM dbo.ArchiveConfiguration 
        WHERE TableName = @TableName AND IsActive = 1;
        
        IF @RetentionMonths IS NULL
        BEGIN
            RAISERROR('Archive configuration not found for table: %s', 16, 1, @TableName);
            RETURN;
        END
        
        -- Calculate cutoff date
        SET @CutoffDate = DATEADD(MONTH, -@RetentionMonths, SYSUTCDATETIME());
        
        PRINT 'Archive Configuration:';
        PRINT '- Table: ' + @TableName;
        PRINT '- Retention: ' + CAST(@RetentionMonths AS NVARCHAR(10)) + ' months';
        PRINT '- Cutoff Date: ' + CAST(@CutoffDate AS NVARCHAR(30));
        PRINT '- Dry Run: ' + CASE WHEN @DryRun = 1 THEN 'Yes' ELSE 'No' END;
        
        -- Count records to be archived from history table
        DECLARE @SQL NVARCHAR(MAX) = '
        SELECT @RecordsToArchive = COUNT(*), @DataSizeMB = SUM(DATALENGTH(*)) / 1024.0 / 1024.0
        FROM dbo.' + @TableName + '_History
        WHERE ValidTo < @CutoffDate';
        
        EXEC sp_executesql @SQL, 
            N'@CutoffDate DATETIME2, @RecordsToArchive BIGINT OUTPUT, @DataSizeMB DECIMAL(18,2) OUTPUT',
            @CutoffDate, @RecordsToArchive OUTPUT, @DataSizeMB OUTPUT;
        
        PRINT 'Records to archive: ' + CAST(@RecordsToArchive AS NVARCHAR(20));
        PRINT 'Estimated data size: ' + CAST(@DataSizeMB AS NVARCHAR(20)) + ' MB';
        
        -- If no records to archive, exit
        IF @RecordsToArchive = 0
        BEGIN
            PRINT 'No records found for archiving.';
            RETURN;
        END
        
        -- If dry run, just return statistics
        IF @DryRun = 1
        BEGIN
            PRINT 'DRY RUN COMPLETED - No data was actually archived.';
            RETURN;
        END
        
        -- Log archive start
        INSERT INTO dbo.ArchiveStatistics 
        (TableName, ArchiveDate, RecordsArchived, DataSizeMB, ArchiveDurationMs, Status)
        VALUES 
        (@TableName, @StartTime, 0, @DataSizeMB, 0, 'InProgress');
        
        SET @StatId = SCOPE_IDENTITY();
        
        -- Step 1: Move data to archive table with compression
        PRINT 'Step 1: Moving data to archive table...';
        
        SET @SQL = '
        INSERT INTO dbo.' + @TableName + '_Archive
        SELECT 
            Id, ImportDate, BranchCode, DepartmentCode, EmployeeCode, KpiCode,
            KpiValue, Unit, Target, Achievement, Score, Note,
            ImportBatchId, CreatedDate, CreatedBy, LastModifiedDate, LastModifiedBy, IsDeleted
        FROM dbo.' + @TableName + '_History
        WHERE ValidTo < @CutoffDate';
        
        EXEC sp_executesql @SQL, N'@CutoffDate DATETIME2', @CutoffDate;
        SET @RecordsArchived = @@ROWCOUNT;
        
        PRINT 'Records moved to archive: ' + CAST(@RecordsArchived AS NVARCHAR(20));
        
        -- Step 2: Enable compression on archive table if configured
        IF @CompressionEnabled = 1
        BEGIN
            PRINT 'Step 2: Enabling compression...';
            
            SET @SQL = 'ALTER INDEX CCI_' + @TableName + '_Archive ON dbo.' + @TableName + '_Archive REBUILD WITH (DATA_COMPRESSION = COLUMNSTORE)';
            
            BEGIN TRY
                EXEC sp_executesql @SQL;
                PRINT 'Columnstore compression enabled on archive table.';
            END TRY
            BEGIN CATCH
                PRINT 'Warning: Could not enable compression - ' + ERROR_MESSAGE();
            END CATCH
        END
        
        -- Step 3: Remove archived data from history table (requires turning off system versioning)
        PRINT 'Step 3: Cleaning up history table...';
        
        -- Temporarily disable system versioning
        SET @SQL = 'ALTER TABLE dbo.' + @TableName + ' SET (SYSTEM_VERSIONING = OFF)';
        EXEC sp_executesql @SQL;
        
        -- Delete archived records from history table
        SET @SQL = 'DELETE FROM dbo.' + @TableName + '_History WHERE ValidTo < @CutoffDate';
        EXEC sp_executesql @SQL, N'@CutoffDate DATETIME2', @CutoffDate;
        
        -- Re-enable system versioning
        SET @SQL = 'ALTER TABLE dbo.' + @TableName + ' SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.' + @TableName + '_History))';
        EXEC sp_executesql @SQL;
        
        PRINT 'History table cleanup completed.';
        
        -- Step 4: Update statistics
        PRINT 'Step 4: Updating statistics...';
        EXEC sp_updatestats;
        
        -- Update archive configuration
        UPDATE dbo.ArchiveConfiguration 
        SET LastArchiveDate = @StartTime,
            ModifiedDate = SYSUTCDATETIME()
        WHERE TableName = @TableName;
        
        -- Update archive statistics
        UPDATE dbo.ArchiveStatistics 
        SET RecordsArchived = @RecordsArchived,
            ArchiveDurationMs = DATEDIFF(MILLISECOND, @StartTime, SYSUTCDATETIME()),
            Status = 'Success'
        WHERE Id = @StatId;
        
        PRINT 'Archive completed successfully!';
        PRINT '- Records archived: ' + CAST(@RecordsArchived AS NVARCHAR(20));
        PRINT '- Duration: ' + CAST(DATEDIFF(MILLISECOND, @StartTime, SYSUTCDATETIME()) AS NVARCHAR(10)) + ' ms';
        
    END TRY
    BEGIN CATCH
        -- Update statistics with error
        IF @StatId IS NOT NULL
        BEGIN
            UPDATE dbo.ArchiveStatistics 
            SET Status = 'Failed',
                ErrorMessage = ERROR_MESSAGE(),
                ArchiveDurationMs = DATEDIFF(MILLISECOND, @StartTime, SYSUTCDATETIME())
            WHERE Id = @StatId;
        END
        
        -- Try to re-enable system versioning if it was disabled
        BEGIN TRY
            SET @SQL = 'ALTER TABLE dbo.' + @TableName + ' SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.' + @TableName + '_History))';
            EXEC sp_executesql @SQL;
        END TRY
        BEGIN CATCH
            PRINT 'Warning: Could not re-enable system versioning - please check manually';
        END CATCH
        
        PRINT 'Archive failed with error: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END;
GO

-- 4. Scheduled Archive Job Procedure
CREATE PROCEDURE sp_ScheduledArchiveJob
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TableName NVARCHAR(128);
    DECLARE @LastArchiveDate DATETIME2;
    DECLARE @RetentionMonths INT;
    
    -- Archive all configured tables
    DECLARE archive_cursor CURSOR FOR
    SELECT TableName, LastArchiveDate, RetentionMonths
    FROM dbo.ArchiveConfiguration 
    WHERE IsActive = 1;
    
    OPEN archive_cursor;
    FETCH NEXT FROM archive_cursor INTO @TableName, @LastArchiveDate, @RetentionMonths;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        PRINT 'Processing table: ' + @TableName;
        
        -- Check if archive is needed (weekly for monthly retention, daily for others)
        DECLARE @DaysSinceLastArchive INT = ISNULL(DATEDIFF(DAY, @LastArchiveDate, GETDATE()), 999);
        DECLARE @ArchiveFrequencyDays INT = CASE WHEN @RetentionMonths >= 12 THEN 7 ELSE 1 END;
        
        IF @DaysSinceLastArchive >= @ArchiveFrequencyDays
        BEGIN
            BEGIN TRY
                EXEC sp_ArchiveTemporalData @TableName = @TableName;
                PRINT 'Archive completed for: ' + @TableName;
            END TRY
            BEGIN CATCH
                PRINT 'Archive failed for ' + @TableName + ': ' + ERROR_MESSAGE();
                -- Continue with next table
            END CATCH
        END
        ELSE
        BEGIN
            PRINT 'Archive not needed for ' + @TableName + ' (last archive: ' + 
                  ISNULL(CAST(@LastArchiveDate AS NVARCHAR(30)), 'Never') + ')';
        END
        
        FETCH NEXT FROM archive_cursor INTO @TableName, @LastArchiveDate, @RetentionMonths;
    END
    
    CLOSE archive_cursor;
    DEALLOCATE archive_cursor;
    
    PRINT 'Scheduled archive job completed.';
END;
GO

-- 5. Archive Health Check Procedure
CREATE PROCEDURE sp_CheckArchiveHealth
AS
BEGIN
    SET NOCOUNT ON;
    
    PRINT 'Archive Health Check Report';
    PRINT '============================';
    
    -- Current configuration
    SELECT 
        'Configuration' AS ReportSection,
        TableName,
        RetentionMonths,
        LastArchiveDate,
        DATEDIFF(DAY, LastArchiveDate, GETDATE()) AS DaysSinceLastArchive,
        CASE 
            WHEN LastArchiveDate IS NULL THEN 'Never Archived'
            WHEN DATEDIFF(DAY, LastArchiveDate, GETDATE()) > 30 THEN 'Needs Attention'
            WHEN DATEDIFF(DAY, LastArchiveDate, GETDATE()) > 7 THEN 'Warning'
            ELSE 'Good'
        END AS Status
    FROM dbo.ArchiveConfiguration 
    WHERE IsActive = 1;
    
    -- Recent archive statistics
    SELECT TOP 10
        'Recent Archives' AS ReportSection,
        TableName,
        ArchiveDate,
        RecordsArchived,
        DataSizeMB,
        CompressionRatio,
        ArchiveDurationMs,
        Status
    FROM dbo.ArchiveStatistics 
    ORDER BY ArchiveDate DESC;
    
    PRINT 'Archive health check completed.';
END;
GO

PRINT 'Archiving strategy procedures created successfully';
PRINT 'Key procedures:';
PRINT '- sp_ArchiveTemporalData: Main archiving procedure';
PRINT '- sp_ScheduledArchiveJob: Automated scheduled archiving';
PRINT '- sp_CheckArchiveHealth: Health monitoring';
PRINT '';
PRINT 'Schedule sp_ScheduledArchiveJob to run weekly for optimal performance';
