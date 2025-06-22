-- ===========================================
-- TinhKhoanApp - Complete Temporal Tables Stored Procedures
-- Tạo đầy đủ các stored procedures cho Temporal Tables + Columnstore Indexes
-- Date: 2025-01-27
-- ===========================================

USE TinhKhoanDB;
GO

-- Kiểm tra và tạo stored procedure: sp_ImportDailyRawData
-- Import dữ liệu hàng ngày với batch processing tối ưu
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_ImportDailyRawData')
    DROP PROCEDURE sp_ImportDailyRawData;
GO

CREATE PROCEDURE sp_ImportDailyRawData
    @ImportDate DATE,
    @ImportBatchId NVARCHAR(50),
    @DataType NVARCHAR(20),
    @FileName NVARCHAR(200) = NULL,
    @ImportedBy NVARCHAR(100) = 'SYSTEM',
    @JsonData NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @StartTime DATETIME2 = GETUTCDATE();
    DECLARE @ProcessedCount INT = 0;
    DECLARE @ErrorCount INT = 0;
    DECLARE @ImportId UNIQUEIDENTIFIER = NEWID();
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Parse JSON và insert vào ImportedDataRecords
        WITH ParsedData AS (
            SELECT 
                @ImportDate as ImportDate,
                @ImportBatchId as ImportBatchId,
                @DataType as DataType,
                @FileName as FileName,
                @ImportedBy as ImportedBy,
                JSON_VALUE(value, '$.employeeCode') as EmployeeCode,
                JSON_VALUE(value, '$.kpiCode') as KpiCode,
                JSON_VALUE(value, '$.kpiName') as KpiName,
                JSON_VALUE(value, '$.branchCode') as BranchCode,
                JSON_VALUE(value, '$.departmentCode') as DepartmentCode,
                CAST(JSON_VALUE(value, '$.value') as DECIMAL(18,4)) as Value,
                JSON_VALUE(value, '$.unit') as Unit,
                GETUTCDATE() as ImportedAt
            FROM OPENJSON(@JsonData)
        )
        INSERT INTO ImportedDataRecords (
            ImportDate, ImportBatchId, DataType, FileName, ImportedBy,
            EmployeeCode, KpiCode, KpiName, BranchCode, DepartmentCode,
            Value, Unit, ImportedAt, CreatedDate, LastModifiedDate
        )
        SELECT 
            ImportDate, ImportBatchId, DataType, FileName, ImportedBy,
            EmployeeCode, KpiCode, KpiName, BranchCode, DepartmentCode,
            Value, Unit, ImportedAt, GETUTCDATE(), GETUTCDATE()
        FROM ParsedData
        WHERE EmployeeCode IS NOT NULL 
          AND KpiCode IS NOT NULL 
          AND BranchCode IS NOT NULL
          AND Value IS NOT NULL;
        
        SET @ProcessedCount = @@ROWCOUNT;
        
        -- Log import operation
        INSERT INTO ImportLogs (
            ImportBatchId, ImportDate, ImportSource, RecordsCount, 
            SuccessCount, ErrorCount, StartTime, EndTime, 
            ProcessingTimeMs, Status, CreatedBy
        )
        VALUES (
            CAST(@ImportBatchId as UNIQUEIDENTIFIER), @ImportDate, @DataType, @ProcessedCount,
            @ProcessedCount, @ErrorCount, @StartTime, GETUTCDATE(),
            DATEDIFF(MILLISECOND, @StartTime, GETUTCDATE()), 'SUCCESS', @ImportedBy
        );
        
        COMMIT TRANSACTION;
        
        SELECT @ProcessedCount as ProcessedRecords, @ErrorCount as ErrorCount;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        SET @ErrorCount = 1;
        
        -- Log error
        INSERT INTO ImportLogs (
            ImportBatchId, ImportDate, ImportSource, RecordsCount, 
            SuccessCount, ErrorCount, StartTime, EndTime, 
            ProcessingTimeMs, Status, ErrorDetails, CreatedBy
        )
        VALUES (
            CAST(@ImportBatchId as UNIQUEIDENTIFIER), @ImportDate, @DataType, 0,
            0, @ErrorCount, @StartTime, GETUTCDATE(),
            DATEDIFF(MILLISECOND, @StartTime, GETUTCDATE()), 'FAILED', 
            ERROR_MESSAGE(), @ImportedBy
        );
        
        THROW;
    END CATCH
END;
GO

-- Kiểm tra và tạo stored procedure: sp_GetDataAsOf (cập nhật)
-- Truy vấn dữ liệu tại thời điểm cụ thể
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetDataAsOf')
    DROP PROCEDURE sp_GetDataAsOf;
GO

CREATE PROCEDURE sp_GetDataAsOf
    @AsOfDate DATETIME2,
    @EmployeeCode NVARCHAR(50) = NULL,
    @BranchCode NVARCHAR(20) = NULL,
    @KpiCode NVARCHAR(50) = NULL,
    @StartDate DATE = NULL,
    @EndDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ImportDate,
        EmployeeCode,
        KpiCode,
        KpiName,
        BranchCode,
        DepartmentCode,
        Value,
        Unit,
        DataType,
        ImportBatchId,
        FileName,
        ImportedBy,
        ImportedAt,
        ValidFrom,
        ValidTo
    FROM ImportedDataRecords 
    FOR SYSTEM_TIME AS OF @AsOfDate
    WHERE (@EmployeeCode IS NULL OR EmployeeCode = @EmployeeCode)
      AND (@BranchCode IS NULL OR BranchCode = @BranchCode)
      AND (@KpiCode IS NULL OR KpiCode = @KpiCode)
      AND (@StartDate IS NULL OR ImportDate >= @StartDate)
      AND (@EndDate IS NULL OR ImportDate <= @EndDate)
      AND IsDeleted = 0
    ORDER BY ImportDate DESC, EmployeeCode, KpiCode;
END;
GO

-- Kiểm tra và tạo stored procedure: sp_CompareDataBetweenDates
-- So sánh dữ liệu giữa 2 thời điểm
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_CompareDataBetweenDates')
    DROP PROCEDURE sp_CompareDataBetweenDates;
GO

CREATE PROCEDURE sp_CompareDataBetweenDates
    @Date1 DATETIME2,
    @Date2 DATETIME2,
    @EmployeeCode NVARCHAR(50) = NULL,
    @BranchCode NVARCHAR(20) = NULL,
    @KpiCode NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    WITH Data1 AS (
        SELECT 
            ImportDate, EmployeeCode, KpiCode, KpiName, BranchCode, 
            Value, Unit, DataType
        FROM ImportedDataRecords 
        FOR SYSTEM_TIME AS OF @Date1
        WHERE (@EmployeeCode IS NULL OR EmployeeCode = @EmployeeCode)
          AND (@BranchCode IS NULL OR BranchCode = @BranchCode)
          AND (@KpiCode IS NULL OR KpiCode = @KpiCode)
          AND IsDeleted = 0
    ),
    Data2 AS (
        SELECT 
            ImportDate, EmployeeCode, KpiCode, KpiName, BranchCode, 
            Value, Unit, DataType
        FROM ImportedDataRecords 
        FOR SYSTEM_TIME AS OF @Date2
        WHERE (@EmployeeCode IS NULL OR EmployeeCode = @EmployeeCode)
          AND (@BranchCode IS NULL OR BranchCode = @BranchCode)
          AND (@KpiCode IS NULL OR KpiCode = @KpiCode)
          AND IsDeleted = 0
    )
    SELECT 
        COALESCE(d1.ImportDate, d2.ImportDate) as ImportDate,
        COALESCE(d1.EmployeeCode, d2.EmployeeCode) as EmployeeCode,
        COALESCE(d1.KpiCode, d2.KpiCode) as KpiCode,
        COALESCE(d1.KpiName, d2.KpiName) as KpiName,
        COALESCE(d1.BranchCode, d2.BranchCode) as BranchCode,
        d1.Value as Value1,
        d2.Value as Value2,
        CASE 
            WHEN d2.Value IS NOT NULL AND d1.Value IS NOT NULL THEN d2.Value - d1.Value
            ELSE NULL 
        END as ValueChange,
        CASE 
            WHEN d1.Value IS NOT NULL AND d1.Value <> 0 AND d2.Value IS NOT NULL 
            THEN ROUND(((d2.Value - d1.Value) / ABS(d1.Value)) * 100, 2)
            ELSE NULL 
        END as PercentageChange,
        COALESCE(d1.Unit, d2.Unit) as Unit,
        CASE 
            WHEN d1.Value IS NULL AND d2.Value IS NOT NULL THEN 'ADDED'
            WHEN d1.Value IS NOT NULL AND d2.Value IS NULL THEN 'REMOVED'
            WHEN d1.Value <> d2.Value THEN 'MODIFIED'
            ELSE 'UNCHANGED'
        END as ChangeType
    FROM Data1 d1
    FULL OUTER JOIN Data2 d2 ON d1.EmployeeCode = d2.EmployeeCode 
                             AND d1.KpiCode = d2.KpiCode 
                             AND d1.BranchCode = d2.BranchCode
                             AND d1.ImportDate = d2.ImportDate
    ORDER BY COALESCE(d1.EmployeeCode, d2.EmployeeCode), 
             COALESCE(d1.KpiCode, d2.KpiCode),
             COALESCE(d1.ImportDate, d2.ImportDate);
END;
GO

-- Kiểm tra và tạo stored procedure: sp_GetRecordHistory
-- Lấy lịch sử thay đổi của một record cụ thể
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetRecordHistory')
    DROP PROCEDURE sp_GetRecordHistory;
GO

CREATE PROCEDURE sp_GetRecordHistory
    @EmployeeCode NVARCHAR(50),
    @KpiCode NVARCHAR(50),
    @BranchCode NVARCHAR(20),
    @ImportDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ImportDate,
        EmployeeCode,
        KpiCode,
        KpiName,
        BranchCode,
        DepartmentCode,
        Value,
        Unit,
        DataType,
        ImportBatchId,
        FileName,
        ImportedBy,
        ImportedAt,
        ValidFrom,
        ValidTo
    FROM ImportedDataRecords 
    FOR SYSTEM_TIME ALL
    WHERE EmployeeCode = @EmployeeCode
      AND KpiCode = @KpiCode
      AND BranchCode = @BranchCode
      AND (@ImportDate IS NULL OR ImportDate = @ImportDate)
    ORDER BY ValidFrom DESC, ImportDate DESC;
END;
GO

-- Kiểm tra và tạo stored procedure: sp_GetDailyChangeSummary
-- Tổng hợp thay đổi theo ngày
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetDailyChangeSummary')
    DROP PROCEDURE sp_GetDailyChangeSummary;
GO

CREATE PROCEDURE sp_GetDailyChangeSummary
    @ReportDate DATE,
    @BranchCode NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @PreviousDate DATE = DATEADD(DAY, -1, @ReportDate);
    DECLARE @ReportDateTime DATETIME2 = CAST(@ReportDate as DATETIME2);
    DECLARE @PreviousDateTime DATETIME2 = CAST(@PreviousDate as DATETIME2);
    
    WITH CurrentData AS (
        SELECT DISTINCT
            EmployeeCode, KpiCode, BranchCode, ImportDate, Value
        FROM ImportedDataRecords 
        FOR SYSTEM_TIME AS OF @ReportDateTime
        WHERE ImportDate = @ReportDate
          AND (@BranchCode IS NULL OR BranchCode = @BranchCode)
          AND IsDeleted = 0
    ),
    PreviousData AS (
        SELECT DISTINCT
            EmployeeCode, KpiCode, BranchCode, ImportDate, Value
        FROM ImportedDataRecords 
        FOR SYSTEM_TIME AS OF @PreviousDateTime
        WHERE ImportDate = @PreviousDate
          AND (@BranchCode IS NULL OR BranchCode = @BranchCode)
          AND IsDeleted = 0
    ),
    Changes AS (
        SELECT 
            COALESCE(c.BranchCode, p.BranchCode) as BranchCode,
            CASE 
                WHEN p.Value IS NULL AND c.Value IS NOT NULL THEN 'NEW'
                WHEN p.Value IS NOT NULL AND c.Value IS NULL THEN 'DELETED'
                WHEN p.Value <> c.Value THEN 'MODIFIED'
                ELSE 'UNCHANGED'
            END as ChangeType
        FROM CurrentData c
        FULL OUTER JOIN PreviousData p ON c.EmployeeCode = p.EmployeeCode 
                                      AND c.KpiCode = p.KpiCode 
                                      AND c.BranchCode = p.BranchCode
    )
    SELECT 
        @ReportDate as ReportDate,
        BranchCode,
        SUM(CASE WHEN ChangeType = 'NEW' THEN 1 ELSE 0 END) as NewRecords,
        SUM(CASE WHEN ChangeType = 'DELETED' THEN 1 ELSE 0 END) as DeletedRecords,
        SUM(CASE WHEN ChangeType = 'MODIFIED' THEN 1 ELSE 0 END) as ModifiedRecords,
        SUM(CASE WHEN ChangeType = 'UNCHANGED' THEN 1 ELSE 0 END) as UnchangedRecords,
        COUNT(*) as TotalComparisons
    FROM Changes
    WHERE BranchCode IS NOT NULL
    GROUP BY BranchCode
    ORDER BY BranchCode;
END;
GO

-- Kiểm tra và tạo stored procedure: sp_GetPerformanceAnalytics
-- Phân tích hiệu suất với moving averages và trends
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetPerformanceAnalytics')
    DROP PROCEDURE sp_GetPerformanceAnalytics;
GO

CREATE PROCEDURE sp_GetPerformanceAnalytics
    @StartDate DATE,
    @EndDate DATE,
    @BranchCode NVARCHAR(20) = NULL,
    @EmployeeCode NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    WITH DailyData AS (
        SELECT 
            ImportDate,
            BranchCode,
            EmployeeCode,
            KpiCode,
            KpiName,
            Value,
            Unit,
            ValidFrom,
            ValidTo,
            ROW_NUMBER() OVER (PARTITION BY EmployeeCode, KpiCode, BranchCode, ImportDate ORDER BY ValidFrom DESC) as rn
        FROM ImportedDataRecords
        WHERE ImportDate BETWEEN @StartDate AND @EndDate
          AND (@BranchCode IS NULL OR BranchCode = @BranchCode)
          AND (@EmployeeCode IS NULL OR EmployeeCode = @EmployeeCode)
          AND IsDeleted = 0
    ),
    Analytics AS (
        SELECT 
            ImportDate,
            BranchCode,
            EmployeeCode,
            KpiCode,
            KpiName,
            Value,
            Unit,
            ValidFrom,
            ValidTo,
            AVG(Value) OVER (
                PARTITION BY EmployeeCode, KpiCode, BranchCode 
                ORDER BY ImportDate 
                ROWS BETWEEN 6 PRECEDING AND CURRENT ROW
            ) as MovingAverage7Days,
            Value - LAG(Value, 1) OVER (
                PARTITION BY EmployeeCode, KpiCode, BranchCode 
                ORDER BY ImportDate
            ) as DayOverDayChange,
            PERCENT_RANK() OVER (
                PARTITION BY KpiCode, ImportDate 
                ORDER BY Value
            ) as PercentileRank
        FROM DailyData
        WHERE rn = 1
    )
    SELECT 
        ImportDate,
        BranchCode,
        EmployeeCode,
        KpiCode,
        KpiName,
        Value,
        Unit,
        ISNULL(MovingAverage7Days, Value) as MovingAverage7Days,
        ISNULL(DayOverDayChange, 0) as DayOverDayChange,
        CAST(PercentileRank as DECIMAL(5,4)) as PercentileRank,
        ValidFrom,
        ValidTo
    FROM Analytics
    ORDER BY ImportDate DESC, EmployeeCode, KpiCode;
END;
GO

-- Kiểm tra và tạo stored procedure: sp_ArchiveOldTemporalData
-- Archive dữ liệu cũ và xóa dữ liệu quá cũ
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_ArchiveOldTemporalData')
    DROP PROCEDURE sp_ArchiveOldTemporalData;
GO

CREATE PROCEDURE sp_ArchiveOldTemporalData
    @ArchiveMonthsOld INT = 12,
    @DeleteMonthsOld INT = 24
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @RowsArchived INT = 0;
    DECLARE @RowsDeleted INT = 0;
    DECLARE @ArchiveDate DATE = DATEADD(MONTH, -@ArchiveMonthsOld, GETDATE());
    DECLARE @DeleteDate DATE = DATEADD(MONTH, -@DeleteMonthsOld, GETDATE());
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Archive old data (move to archive table if exists)
        IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ImportedDataRecordsArchive')
        BEGIN
            INSERT INTO ImportedDataRecordsArchive
            SELECT *, GETUTCDATE() as ArchivedDate, 'SYSTEM' as ArchivedBy
            FROM ImportedDataRecords
            WHERE ImportDate < @ArchiveDate
              AND ImportDate >= @DeleteDate;
            
            SET @RowsArchived = @@ROWCOUNT;
        END
        
        -- Delete very old data
        DELETE FROM ImportedDataRecords
        WHERE ImportDate < @DeleteDate;
        
        SET @RowsDeleted = @@ROWCOUNT;
        
        COMMIT TRANSACTION;
        
        SELECT @RowsArchived as RowsArchived, @RowsDeleted as RowsDeleted;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- Kiểm tra và tạo stored procedure: sp_MaintainTemporalIndexes
-- Maintain indexes cho tối ưu hiệu suất
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_MaintainTemporalIndexes')
    DROP PROCEDURE sp_MaintainTemporalIndexes;
GO

CREATE PROCEDURE sp_MaintainTemporalIndexes
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @sql NVARCHAR(MAX);
    DECLARE @tableName NVARCHAR(128);
    DECLARE @indexName NVARCHAR(128);
    
    -- Update statistics cho temporal tables
    DECLARE cursor_tables CURSOR FOR
    SELECT DISTINCT t.name
    FROM sys.tables t
    INNER JOIN sys.columns c ON t.object_id = c.object_id
    WHERE c.name IN ('ValidFrom', 'ValidTo')
      AND t.temporal_type IN (0, 2); -- Current và History tables
    
    OPEN cursor_tables;
    FETCH NEXT FROM cursor_tables INTO @tableName;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @sql = N'UPDATE STATISTICS [' + @tableName + '] WITH FULLSCAN;';
        EXEC sp_executesql @sql;
        
        -- Reorganize fragmented indexes
        SET @sql = N'ALTER INDEX ALL ON [' + @tableName + '] REORGANIZE;';
        EXEC sp_executesql @sql;
        
        FETCH NEXT FROM cursor_tables INTO @tableName;
    END
    
    CLOSE cursor_tables;
    DEALLOCATE cursor_tables;
    
    PRINT 'Temporal table index maintenance completed successfully.';
END;
GO

-- Kiểm tra và tạo stored procedure: sp_TemporalHealthCheck
-- Health check toàn diện cho Temporal Tables
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_TemporalHealthCheck')
    DROP PROCEDURE sp_TemporalHealthCheck;
GO

CREATE PROCEDURE sp_TemporalHealthCheck
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Result Set 1: Temporal Tables Status
    SELECT 
        t.name as TableName,
        t.temporal_type_desc as TemporalType,
        h.name as HistoryTableName,
        OBJECT_NAME(t.history_table_id) as HistoryTableFullName,
        t.is_memory_optimized as IsMemoryOptimized
    FROM sys.tables t
    LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
    WHERE t.temporal_type IN (0, 2)
    ORDER BY t.name;
    
    -- Result Set 2: Columnstore Indexes Status
    SELECT 
        t.name as TableName,
        i.name as IndexName,
        i.type_desc as IndexType,
        p.rows as RowCount,
        p.data_compression_desc as CompressionType
    FROM sys.tables t
    INNER JOIN sys.indexes i ON t.object_id = i.object_id
    INNER JOIN sys.partitions p ON i.object_id = p.object_id AND i.index_id = p.index_id
    WHERE i.type = 5 -- Columnstore
      AND t.name LIKE '%History'
    ORDER BY t.name;
    
    -- Result Set 3: Data Volume Summary
    SELECT 
        t.name as TableName,
        p.rows as RowCount,
        CAST(SUM(a.total_pages) * 8.0 / 1024 as DECIMAL(10,2)) as SizeMB,
        CAST(SUM(a.used_pages) * 8.0 / 1024 as DECIMAL(10,2)) as UsedMB
    FROM sys.tables t
    INNER JOIN sys.indexes i ON t.object_id = i.object_id
    INNER JOIN sys.partitions p ON i.object_id = p.object_id AND i.index_id = p.index_id
    INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
    WHERE t.temporal_type IN (0, 2)
    GROUP BY t.name, p.rows
    ORDER BY p.rows DESC;
    
    -- Result Set 4: Recent Activity
    SELECT TOP 10
        ImportDate,
        BranchCode,
        COUNT(*) as RecordCount,
        MIN(ValidFrom) as FirstChange,
        MAX(ValidFrom) as LastChange
    FROM ImportedDataRecords
    WHERE ImportDate >= DATEADD(DAY, -7, GETDATE())
    GROUP BY ImportDate, BranchCode
    ORDER BY ImportDate DESC, RecordCount DESC;
END;
GO

PRINT 'Đã tạo thành công tất cả stored procedures cho Temporal Tables!';
PRINT 'Danh sách procedures đã tạo:';
PRINT '- sp_ImportDailyRawData: Import dữ liệu hàng ngày';
PRINT '- sp_GetDataAsOf: Truy vấn dữ liệu tại thời điểm';
PRINT '- sp_CompareDataBetweenDates: So sánh dữ liệu giữa 2 thời điểm';
PRINT '- sp_GetRecordHistory: Lịch sử thay đổi record';
PRINT '- sp_GetDailyChangeSummary: Tổng hợp thay đổi theo ngày';
PRINT '- sp_GetPerformanceAnalytics: Phân tích hiệu suất';
PRINT '- sp_ArchiveOldTemporalData: Archive dữ liệu cũ';
PRINT '- sp_MaintainTemporalIndexes: Maintain indexes';
PRINT '- sp_TemporalHealthCheck: Health check toàn diện';
