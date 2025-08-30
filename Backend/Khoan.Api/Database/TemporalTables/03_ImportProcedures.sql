-- =============================================
-- Script: 03_ImportProcedures.sql
-- Purpose: High-Performance Import Procedures for Temporal Tables
-- Author: System Architecture
-- Date: 2025-01-15
-- Notes: Optimized MERGE operations for 100K-200K records/day
-- =============================================

USE TinhKhoanDB;
GO

-- 1. Create Staging Table Structure for Bulk Import
-- This table will be recreated each time for bulk operations
IF OBJECT_ID('tempdb..#StagingRawDataImports') IS NOT NULL
    DROP TABLE #StagingRawDataImports;

-- Create reusable staging table template procedure
CREATE PROCEDURE sp_CreateStagingTable
AS
BEGIN
    -- Create staging table with same structure as main table (minus temporal columns)
    CREATE TABLE #StagingRawDataImports
    (
        ImportDate DATE NOT NULL,
        BranchCode NVARCHAR(10) NOT NULL,
        DepartmentCode NVARCHAR(10) NOT NULL,
        EmployeeCode NVARCHAR(20) NOT NULL,
        KpiCode NVARCHAR(20) NOT NULL,
        KpiValue DECIMAL(18,4) NOT NULL,
        Unit NVARCHAR(10) NULL,
        Target DECIMAL(18,4) NULL,
        Achievement DECIMAL(18,4) NULL,
        Score DECIMAL(5,2) NULL,
        Note NVARCHAR(500) NULL,
        
        ImportBatchId UNIQUEIDENTIFIER NOT NULL,
        CreatedBy NVARCHAR(100) NOT NULL DEFAULT 'SYSTEM',
        
        -- Staging-specific columns
        RowNumber INT IDENTITY(1,1),
        ProcessingStatus NVARCHAR(20) DEFAULT 'Pending', -- Pending, Processed, Error
        ErrorMessage NVARCHAR(MAX) NULL,
        
        INDEX IX_Staging_Processing (ProcessingStatus, RowNumber)
    );
END;
GO

-- 2. High-Performance Bulk Import Procedure
CREATE PROCEDURE sp_BulkImportRawData
    @FilePath NVARCHAR(500),
    @ImportBatchId UNIQUEIDENTIFIER = NULL,
    @ImportedBy NVARCHAR(100) = 'SYSTEM',
    @SkipValidation BIT = 0,
    @BatchSize INT = 10000
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    
    -- Initialize variables
    IF @ImportBatchId IS NULL
        SET @ImportBatchId = NEWID();
    
    DECLARE @StartTime DATETIME2 = SYSUTCDATETIME();
    DECLARE @TotalRows INT = 0;
    DECLARE @SuccessRows INT = 0;
    DECLARE @ErrorRows INT = 0;
    DECLARE @LogId BIGINT;
    
    BEGIN TRY
        -- Create staging table
        CREATE TABLE #StagingRawDataImports
        (
            ImportDate DATE NOT NULL,
            BranchCode NVARCHAR(10) NOT NULL,
            DepartmentCode NVARCHAR(10) NOT NULL,
            EmployeeCode NVARCHAR(20) NOT NULL,
            KpiCode NVARCHAR(20) NOT NULL,
            KpiValue DECIMAL(18,4) NOT NULL,
            Unit NVARCHAR(10) NULL,
            Target DECIMAL(18,4) NULL,
            Achievement DECIMAL(18,4) NULL,
            Score DECIMAL(5,2) NULL,
            Note NVARCHAR(500) NULL,
            
            ImportBatchId UNIQUEIDENTIFIER NOT NULL DEFAULT @ImportBatchId,
            CreatedBy NVARCHAR(100) NOT NULL DEFAULT @ImportedBy,
            
            RowNumber INT IDENTITY(1,1),
            ProcessingStatus NVARCHAR(20) DEFAULT 'Pending',
            ErrorMessage NVARCHAR(MAX) NULL
        );
        
        -- Log import start
        INSERT INTO dbo.ImportLogs 
        (ImportBatchId, FileName, FileSize, RecordCount, SuccessCount, ErrorCount, 
         ImportStartTime, Status, ImportedBy)
        VALUES 
        (@ImportBatchId, @FilePath, 0, 0, 0, 0, @StartTime, 'InProgress', @ImportedBy);
        
        SET @LogId = SCOPE_IDENTITY();
        
        -- Bulk insert from CSV file
        DECLARE @SQL NVARCHAR(MAX) = '
        BULK INSERT #StagingRawDataImports
        FROM ''' + @FilePath + '''
        WITH (
            FORMAT = ''CSV'',
            FIRSTROW = 2,
            FIELDTERMINATOR = '','',
            ROWTERMINATOR = ''\n'',
            TABLOCK,
            ROWS_PER_BATCH = ' + CAST(@BatchSize AS NVARCHAR(10)) + ',
            MAXERRORS = 1000,
            ERRORFILE = ''' + @FilePath + '.errors'',
            KEEPNULLS
        )';
        
        EXEC sp_executesql @SQL;
        
        SET @TotalRows = @@ROWCOUNT;
            Value NVARCHAR(50) '$.value',
            Unit NVARCHAR(50) '$.unit'
        );
        
        -- 3. Validate data integrity
        IF EXISTS (SELECT 1 FROM #StagingData WHERE EmployeeCode IS NULL OR KpiCode IS NULL OR BranchCode IS NULL)
        BEGIN
            RAISERROR('Invalid data: EmployeeCode, KpiCode, and BranchCode are required', 16, 1);
            RETURN;
        END;
        
        -- 4. MERGE for optimal performance (only update changes)
        MERGE RawDataImports AS target
        USING (
            SELECT 
                @ImportDate as ImportDate,
                @ImportBatchId as ImportBatchId,
                EmployeeCode,
                KpiCode,
                KpiName,
                BranchCode,
                DepartmentCode,
                Value,
                Unit,
                @DataType as DataType,
                @FileName as FileName,
                @ImportedBy as ImportedBy,
                GETDATE() as ImportedAt,
                0 as IsDeleted
            FROM #StagingData
        ) AS source
        ON target.ImportDate = source.ImportDate 
           AND target.EmployeeCode = source.EmployeeCode 
           AND target.KpiCode = source.KpiCode
           AND target.BranchCode = source.BranchCode
           AND target.IsDeleted = 0
        WHEN MATCHED AND (
            target.Value <> source.Value OR 
            ISNULL(target.Unit, '') <> ISNULL(source.Unit, '') OR
            ISNULL(target.KpiName, '') <> ISNULL(source.KpiName, '') OR
            ISNULL(target.DepartmentCode, '') <> ISNULL(source.DepartmentCode, '')
        ) THEN
            UPDATE SET 
                Value = source.Value,
                Unit = source.Unit,
                KpiName = source.KpiName,
                DepartmentCode = source.DepartmentCode,
                ImportBatchId = source.ImportBatchId,
                FileName = source.FileName,
                ImportedBy = source.ImportedBy,
                ImportedAt = source.ImportedAt
        WHEN NOT MATCHED THEN
            INSERT (ImportDate, ImportBatchId, EmployeeCode, KpiCode, KpiName, 
                   BranchCode, DepartmentCode, Value, Unit, DataType, 
                   FileName, ImportedBy, ImportedAt, IsDeleted)
            VALUES (source.ImportDate, source.ImportBatchId, source.EmployeeCode, 
                   source.KpiCode, source.KpiName, source.BranchCode, 
                   source.DepartmentCode, source.Value, source.Unit, source.DataType,
                   source.FileName, source.ImportedBy, source.ImportedAt, source.IsDeleted);
        
        DECLARE @RowsAffected INT = @@ROWCOUNT;
        
        -- 5. Log import statistics
        INSERT INTO ImportLogs (ImportDate, ImportBatchId, DataType, FileName, 
                               RowsProcessed, ImportedBy, ImportedAt, Status)
        VALUES (@ImportDate, @ImportBatchId, @DataType, @FileName, 
                @RowsAffected, @ImportedBy, GETDATE(), 'SUCCESS');
        
        COMMIT TRANSACTION;
        
        SELECT @RowsAffected as RowsProcessed, 'SUCCESS' as Status;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        
        -- Log error
        INSERT INTO ImportLogs (ImportDate, ImportBatchId, DataType, FileName, 
                               RowsProcessed, ImportedBy, ImportedAt, Status, ErrorMessage)
        VALUES (@ImportDate, @ImportBatchId, @DataType, @FileName, 
                0, @ImportedBy, GETDATE(), 'ERROR', ERROR_MESSAGE());
        
        THROW;
    END CATCH;
END;
GO

-- 2. Bulk CSV Import Procedure
CREATE PROCEDURE sp_BulkImportFromCSV
    @ImportDate DATE,
    @FilePath NVARCHAR(500),
    @DataType NVARCHAR(20),
    @ImportedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @ImportBatchId NVARCHAR(50) = 'BULK_' + FORMAT(GETDATE(), 'yyyyMMdd_HHmmss');
        DECLARE @FileName NVARCHAR(200) = REVERSE(SUBSTRING(REVERSE(@FilePath), 1, CHARINDEX('\', REVERSE(@FilePath)) - 1));
        
        -- Create temp table for bulk insert
        CREATE TABLE #BulkData (
            EmployeeCode NVARCHAR(50),
            KpiCode NVARCHAR(50),
            KpiName NVARCHAR(200),
            BranchCode NVARCHAR(20),
            DepartmentCode NVARCHAR(20),
            Value NVARCHAR(50),
            Unit NVARCHAR(50)
        );
        
        -- Bulk insert from CSV
        DECLARE @SQL NVARCHAR(MAX) = '
        BULK INSERT #BulkData
        FROM ''' + @FilePath + '''
        WITH (
            FORMAT = ''CSV'',
            FIRSTROW = 2,
            FIELDTERMINATOR = '','',
            ROWTERMINATOR = ''\n'',
            ERRORFILE = ''C:\Temp\BulkErrors_' + @ImportBatchId + '.txt'',
            MAXERRORS = 100
        )';
        
        EXEC sp_executesql @SQL;
        
        -- Convert to proper data types and import
        MERGE RawDataImports AS target
        USING (
            SELECT 
                @ImportDate as ImportDate,
                @ImportBatchId as ImportBatchId,
                EmployeeCode,
                KpiCode,
                KpiName,
                BranchCode,
                DepartmentCode,
                TRY_CAST(Value AS DECIMAL(18,4)) as Value,
                Unit,
                @DataType as DataType,
                @FileName as FileName,
                @ImportedBy as ImportedBy,
                GETDATE() as ImportedAt,
                0 as IsDeleted
            FROM #BulkData
            WHERE TRY_CAST(Value AS DECIMAL(18,4)) IS NOT NULL
        ) AS source
        ON target.ImportDate = source.ImportDate 
           AND target.EmployeeCode = source.EmployeeCode 
           AND target.KpiCode = source.KpiCode
           AND target.BranchCode = source.BranchCode
           AND target.IsDeleted = 0
        WHEN MATCHED AND target.Value <> source.Value THEN
            UPDATE SET 
                Value = source.Value,
                Unit = source.Unit,
                ImportBatchId = source.ImportBatchId,
                ImportedBy = source.ImportedBy,
                ImportedAt = source.ImportedAt
        WHEN NOT MATCHED THEN
            INSERT (ImportDate, ImportBatchId, EmployeeCode, KpiCode, KpiName, 
                   BranchCode, DepartmentCode, Value, Unit, DataType, 
                   FileName, ImportedBy, ImportedAt, IsDeleted)
            VALUES (source.ImportDate, source.ImportBatchId, source.EmployeeCode, 
                   source.KpiCode, source.KpiName, source.BranchCode, 
                   source.DepartmentCode, source.Value, source.Unit, source.DataType,
                   source.FileName, source.ImportedBy, source.ImportedAt, source.IsDeleted);
        
        DECLARE @RowsAffected INT = @@ROWCOUNT;
        
        COMMIT TRANSACTION;
        
        SELECT @RowsAffected as RowsProcessed, 'SUCCESS' as Status;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- Data validation (if not skipped)
        IF @SkipValidation = 0
        BEGIN
            -- Validate required fields
            UPDATE #StagingRawDataImports 
            SET ProcessingStatus = 'Error',
                ErrorMessage = 'Missing required fields'
            WHERE ImportDate IS NULL 
               OR BranchCode IS NULL OR LTRIM(RTRIM(BranchCode)) = ''
               OR EmployeeCode IS NULL OR LTRIM(RTRIM(EmployeeCode)) = ''
               OR KpiCode IS NULL OR LTRIM(RTRIM(KpiCode)) = ''
               OR KpiValue IS NULL;
            
            -- Validate data ranges
            UPDATE #StagingRawDataImports 
            SET ProcessingStatus = 'Error',
                ErrorMessage = 'Invalid data ranges'
            WHERE ProcessingStatus = 'Pending'
              AND (KpiValue < -999999999 OR KpiValue > 999999999
                  OR Score < 0 OR Score > 100
                  OR ImportDate < '2020-01-01' OR ImportDate > DATEADD(YEAR, 1, GETDATE()));
            
            -- Check for duplicates within batch
            WITH DuplicateCheck AS (
                SELECT ImportDate, BranchCode, EmployeeCode, KpiCode,
                       ROW_NUMBER() OVER (PARTITION BY ImportDate, BranchCode, EmployeeCode, KpiCode ORDER BY RowNumber) as rn
                FROM #StagingRawDataImports
                WHERE ProcessingStatus = 'Pending'
            )
            UPDATE s
            SET ProcessingStatus = 'Error',
                ErrorMessage = 'Duplicate record in batch'
            FROM #StagingRawDataImports s
            INNER JOIN DuplicateCheck d ON s.ImportDate = d.ImportDate 
                AND s.BranchCode = d.BranchCode 
                AND s.EmployeeCode = d.EmployeeCode 
                AND s.KpiCode = d.KpiCode
            WHERE d.rn > 1;
        END
        
        -- Mark valid records as ready for processing
        UPDATE #StagingRawDataImports 
        SET ProcessingStatus = 'Ready'
        WHERE ProcessingStatus = 'Pending';
        
        -- High-performance MERGE operation in batches
        DECLARE @ProcessedBatch INT = 0;
        DECLARE @CurrentBatch INT = 1;
        
        WHILE EXISTS (SELECT 1 FROM #StagingRawDataImports WHERE ProcessingStatus = 'Ready')
        BEGIN
            BEGIN TRANSACTION;
            
            -- Process batch
            WITH BatchData AS (
                SELECT TOP (@BatchSize) *
                FROM #StagingRawDataImports 
                WHERE ProcessingStatus = 'Ready'
                ORDER BY RowNumber
            )
            MERGE dbo.RawDataImports AS target
            USING BatchData AS source
            ON target.ImportDate = source.ImportDate 
               AND target.BranchCode = source.BranchCode
               AND target.EmployeeCode = source.EmployeeCode 
               AND target.KpiCode = source.KpiCode
               AND target.IsDeleted = 0
            
            WHEN MATCHED AND (
                target.KpiValue <> source.KpiValue OR
                ISNULL(target.Unit, '') <> ISNULL(source.Unit, '') OR
                ISNULL(target.Target, 0) <> ISNULL(source.Target, 0) OR
                ISNULL(target.Achievement, 0) <> ISNULL(source.Achievement, 0) OR
                ISNULL(target.Score, 0) <> ISNULL(source.Score, 0) OR
                ISNULL(target.Note, '') <> ISNULL(source.Note, '')
            ) THEN
                UPDATE SET 
                    KpiValue = source.KpiValue,
                    Unit = source.Unit,
                    Target = source.Target,
                    Achievement = source.Achievement,
                    Score = source.Score,
                    Note = source.Note,
                    LastModifiedDate = SYSUTCDATETIME(),
                    LastModifiedBy = source.CreatedBy,
                    ImportBatchId = source.ImportBatchId
            
            WHEN NOT MATCHED THEN
                INSERT (ImportDate, BranchCode, DepartmentCode, EmployeeCode, KpiCode,
                       KpiValue, Unit, Target, Achievement, Score, Note,
                       ImportBatchId, CreatedBy, LastModifiedBy)
                VALUES (source.ImportDate, source.BranchCode, source.DepartmentCode,
                       source.EmployeeCode, source.KpiCode, source.KpiValue,
                       source.Unit, source.Target, source.Achievement, 
                       source.Score, source.Note, source.ImportBatchId,
                       source.CreatedBy, source.CreatedBy);
            
            -- Mark processed records
            UPDATE #StagingRawDataImports 
            SET ProcessingStatus = 'Processed'
            WHERE RowNumber IN (
                SELECT TOP (@BatchSize) RowNumber 
                FROM #StagingRawDataImports 
                WHERE ProcessingStatus = 'Ready'
                ORDER BY RowNumber
            );
            
            SET @ProcessedBatch = @@ROWCOUNT;
            SET @SuccessRows += @ProcessedBatch;
            
            COMMIT TRANSACTION;
            
            SET @CurrentBatch += 1;
            
            -- Progress logging every 10 batches
            IF @CurrentBatch % 10 = 0
            BEGIN
                PRINT 'Processed batch ' + CAST(@CurrentBatch AS NVARCHAR(10)) + 
                      ', Records: ' + CAST(@SuccessRows AS NVARCHAR(10));
            END
        END
        
        -- Count error rows
        SET @ErrorRows = (SELECT COUNT(*) FROM #StagingRawDataImports WHERE ProcessingStatus = 'Error');
        
        -- Update import log
        UPDATE dbo.ImportLogs 
        SET RecordCount = @TotalRows,
            SuccessCount = @SuccessRows,
            ErrorCount = @ErrorRows,
            ImportEndTime = SYSUTCDATETIME(),
            Status = CASE WHEN @ErrorRows = 0 THEN 'Completed' ELSE 'CompletedWithErrors' END
        WHERE Id = @LogId;
        
        -- Update daily summary
        EXEC sp_UpdateDailyImportSummary;
        
        -- Return results
        SELECT 
            @ImportBatchId AS ImportBatchId,
            @TotalRows AS TotalRows,
            @SuccessRows AS SuccessRows,
            @ErrorRows AS ErrorRows,
            DATEDIFF(MILLISECOND, @StartTime, SYSUTCDATETIME()) AS DurationMs,
            CASE WHEN @ErrorRows = 0 THEN 'Success' ELSE 'CompletedWithErrors' END AS Status;
        
        -- Return error details if any
        IF @ErrorRows > 0
        BEGIN
            SELECT RowNumber, ErrorMessage, ImportDate, BranchCode, EmployeeCode, KpiCode
            FROM #StagingRawDataImports 
            WHERE ProcessingStatus = 'Error'
            ORDER BY RowNumber;
        END
        
    END TRY
    BEGIN CATCH
        -- Rollback transaction if active
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        -- Update import log with error
        UPDATE dbo.ImportLogs 
        SET Status = 'Failed',
            ErrorMessage = ERROR_MESSAGE(),
            ImportEndTime = SYSUTCDATETIME()
        WHERE Id = @LogId;
        
        -- Re-throw error
        THROW;
    END CATCH
END;
GO

-- 3. JSON-based Import Procedure for API calls
CREATE PROCEDURE sp_ImportRawDataFromJson
    @JsonData NVARCHAR(MAX),
    @ImportedBy NVARCHAR(100) = 'API',
    @ImportBatchId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    
    IF @ImportBatchId IS NULL
        SET @ImportBatchId = NEWID();
    
    DECLARE @StartTime DATETIME2 = SYSUTCDATETIME();
    DECLARE @ProcessedRows INT = 0;
    
    BEGIN TRY
        -- Parse JSON and insert directly using MERGE
        WITH JsonData AS (
            SELECT 
                CAST(ImportDate AS DATE) AS ImportDate,
                BranchCode,
                DepartmentCode,
                EmployeeCode,
                KpiCode,
                CAST(KpiValue AS DECIMAL(18,4)) AS KpiValue,
                Unit,
                CAST(Target AS DECIMAL(18,4)) AS Target,
                CAST(Achievement AS DECIMAL(18,4)) AS Achievement,
                CAST(Score AS DECIMAL(5,2)) AS Score,
                Note,
                @ImportBatchId AS ImportBatchId,
                @ImportedBy AS CreatedBy
            FROM OPENJSON(@JsonData)
            WITH (
                ImportDate NVARCHAR(10) '$.importDate',
                BranchCode NVARCHAR(10) '$.branchCode',
                DepartmentCode NVARCHAR(10) '$.departmentCode',
                EmployeeCode NVARCHAR(20) '$.employeeCode',
                KpiCode NVARCHAR(20) '$.kpiCode',
                KpiValue NVARCHAR(20) '$.kpiValue',
                Unit NVARCHAR(10) '$.unit',
                Target NVARCHAR(20) '$.target',
                Achievement NVARCHAR(20) '$.achievement',
                Score NVARCHAR(10) '$.score',
                Note NVARCHAR(500) '$.note'
            )
        )
        MERGE dbo.RawDataImports AS target
        USING JsonData AS source
        ON target.ImportDate = source.ImportDate 
           AND target.BranchCode = source.BranchCode
           AND target.EmployeeCode = source.EmployeeCode 
           AND target.KpiCode = source.KpiCode
           AND target.IsDeleted = 0
        
        WHEN MATCHED AND (
            target.KpiValue <> source.KpiValue OR
            ISNULL(target.Unit, '') <> ISNULL(source.Unit, '') OR
            ISNULL(target.Target, 0) <> ISNULL(source.Target, 0) OR
            ISNULL(target.Achievement, 0) <> ISNULL(source.Achievement, 0) OR
            ISNULL(target.Score, 0) <> ISNULL(source.Score, 0) OR
            ISNULL(target.Note, '') <> ISNULL(source.Note, '')
        ) THEN
            UPDATE SET 
                KpiValue = source.KpiValue,
                Unit = source.Unit,
                Target = source.Target,
                Achievement = source.Achievement,
                Score = source.Score,
                Note = source.Note,
                LastModifiedDate = SYSUTCDATETIME(),
                LastModifiedBy = source.CreatedBy,
                ImportBatchId = source.ImportBatchId
        
        WHEN NOT MATCHED THEN
            INSERT (ImportDate, BranchCode, DepartmentCode, EmployeeCode, KpiCode,
                   KpiValue, Unit, Target, Achievement, Score, Note,
                   ImportBatchId, CreatedBy, LastModifiedBy)
            VALUES (source.ImportDate, source.BranchCode, source.DepartmentCode,
                   source.EmployeeCode, source.KpiCode, source.KpiValue,
                   source.Unit, source.Target, source.Achievement, 
                   source.Score, source.Note, source.ImportBatchId,
                   source.CreatedBy, source.CreatedBy);
        
        SET @ProcessedRows = @@ROWCOUNT;
        
        -- Log successful import
        INSERT INTO dbo.ImportLogs 
        (ImportBatchId, FileName, FileSize, RecordCount, SuccessCount, ErrorCount, 
         ImportStartTime, ImportEndTime, Status, ImportedBy)
        VALUES 
        (@ImportBatchId, 'JSON_IMPORT', LEN(@JsonData), @ProcessedRows, @ProcessedRows, 0, 
         @StartTime, SYSUTCDATETIME(), 'Completed', @ImportedBy);
        
        -- Return results
        SELECT 
            @ImportBatchId AS ImportBatchId,
            @ProcessedRows AS ProcessedRows,
            DATEDIFF(MILLISECOND, @StartTime, SYSUTCDATETIME()) AS DurationMs,
            'Success' AS Status;
        
    END TRY
    BEGIN CATCH
        -- Log failed import
        INSERT INTO dbo.ImportLogs 
        (ImportBatchId, FileName, FileSize, RecordCount, SuccessCount, ErrorCount, 
         ImportStartTime, ImportEndTime, Status, ErrorMessage, ImportedBy)
        VALUES 
        (@ImportBatchId, 'JSON_IMPORT', LEN(@JsonData), 0, 0, 1, 
         @StartTime, SYSUTCDATETIME(), 'Failed', ERROR_MESSAGE(), @ImportedBy);
        
        THROW;
    END CATCH
END;
GO

-- 4. Daily Summary Update Procedure
CREATE PROCEDURE sp_UpdateDailyImportSummary
    @TargetDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @TargetDate IS NULL
        SET @TargetDate = CAST(GETDATE() AS DATE);
    
    -- Update or insert daily summary
    MERGE dbo.DailyImportSummary AS target
    USING (
        SELECT 
            @TargetDate AS SummaryDate,
            BranchCode,
            COUNT(*) AS TotalRecords,
            COUNT(DISTINCT EmployeeCode) AS TotalEmployees,
            COUNT(DISTINCT KpiCode) AS TotalKpiTypes,
            AVG(Score) AS AverageScore,
            (SELECT TOP 1 EmployeeCode 
             FROM dbo.RawDataImports r2 
             WHERE r2.BranchCode = r.BranchCode 
               AND r2.ImportDate = @TargetDate 
               AND r2.IsDeleted = 0
             ORDER BY r2.Score DESC) AS TopPerformerEmployee,
            MAX(Score) AS TopPerformerScore
        FROM dbo.RawDataImports r
        WHERE ImportDate = @TargetDate AND IsDeleted = 0
        GROUP BY BranchCode
    ) AS source
    ON target.SummaryDate = source.SummaryDate 
       AND target.BranchCode = source.BranchCode
    
    WHEN MATCHED THEN
        UPDATE SET 
            TotalRecords = source.TotalRecords,
            TotalEmployees = source.TotalEmployees,
            TotalKpiTypes = source.TotalKpiTypes,
            AverageScore = source.AverageScore,
            TopPerformerEmployee = source.TopPerformerEmployee,
            TopPerformerScore = source.TopPerformerScore,
            LastUpdatedDate = SYSUTCDATETIME()
    
    WHEN NOT MATCHED THEN
        INSERT (SummaryDate, BranchCode, TotalRecords, TotalEmployees, TotalKpiTypes,
               AverageScore, TopPerformerEmployee, TopPerformerScore)
        VALUES (source.SummaryDate, source.BranchCode, source.TotalRecords,
               source.TotalEmployees, source.TotalKpiTypes, source.AverageScore,
               source.TopPerformerEmployee, source.TopPerformerScore);
    
    PRINT 'Daily summary updated for date: ' + CAST(@TargetDate AS NVARCHAR(10));
END;
GO

PRINT 'Import procedures created successfully';
PRINT 'Use sp_BulkImportRawData for CSV file imports';
PRINT 'Use sp_ImportRawDataFromJson for API-based imports';
PRINT 'Use sp_UpdateDailyImportSummary to refresh daily statistics';
