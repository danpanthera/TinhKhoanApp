-- ===========================================
-- TinhKhoanApp - Simplified Temporal Tables Stored Procedures
-- Compatible với SQL Server temporal syntax
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
    DECLARE @RecordId INT;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Tạo record import chính
        INSERT INTO ImportedDataRecords (
            FileName, FileType, Category, ImportDate, ImportedBy, 
            Status, RecordsCount, Notes
        )
        VALUES (
            ISNULL(@FileName, 'API_Import_' + @ImportBatchId),
            @DataType,
            'API_IMPORT',
            @ImportDate,
            @ImportedBy,
            'Processing',
            0,
            'Imported via API on ' + CONVERT(VARCHAR, GETUTCDATE(), 120)
        );
        
        SET @RecordId = SCOPE_IDENTITY();
        
        -- Parse JSON và insert từng item
        INSERT INTO ImportedDataItems (
            ImportedDataRecordId, 
            RawData, 
            ProcessedDate,
            ProcessingNotes
        )
        SELECT 
            @RecordId as ImportedDataRecordId,
            value as RawData,
            GETUTCDATE() as ProcessedDate,
            'Processed via sp_ImportDailyRawData' as ProcessingNotes
        FROM OPENJSON(@JsonData);
        
        SET @ProcessedCount = @@ROWCOUNT;
        
        -- Cập nhật record count và status
        UPDATE ImportedDataRecords 
        SET RecordsCount = @ProcessedCount,
            Status = 'Completed'
        WHERE Id = @RecordId;
        
        COMMIT TRANSACTION;
        
        SELECT @ProcessedCount as ProcessedRecords, @ErrorCount as ErrorCount, @RecordId as ImportedDataRecordId;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        SET @ErrorCount = 1;
        
        -- Update status to failed
        IF @RecordId IS NOT NULL
        BEGIN
            UPDATE ImportedDataRecords 
            SET Status = 'Failed',
                Notes = ISNULL(Notes, '') + ' ERROR: ' + ERROR_MESSAGE()
            WHERE Id = @RecordId;
        END
        
        RAISERROR('Import failed', 16, 1);
        RETURN;
    END CATCH
END;
GO

-- Kiểm tra và tạo stored procedure: sp_GetDataAsOf (simplified)
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
        r.ImportDate,
        '' as EmployeeCode,
        JSON_VALUE(i.RawData, '$.Indicator') as KpiCode,
        JSON_VALUE(i.RawData, '$.Indicator') as KpiName,
        JSON_VALUE(i.RawData, '$.Unit') as BranchCode,
        '' as DepartmentCode,
        CAST(JSON_VALUE(i.RawData, '$.Q1') as DECIMAL(18,4)) as Value,
        JSON_VALUE(i.RawData, '$.Note') as Unit,
        'QUARTERLY' as DataType,
        CAST(r.Id as NVARCHAR(50)) as ImportBatchId,
        r.FileName,
        r.ImportedBy,
        r.ImportDate as ImportedAt,
        i.ValidFrom,
        i.ValidTo
    FROM ImportedDataRecords FOR SYSTEM_TIME AS OF @AsOfDate r
    INNER JOIN ImportedDataItems FOR SYSTEM_TIME AS OF @AsOfDate i ON r.Id = i.ImportedDataRecordId
    WHERE (@StartDate IS NULL OR r.ImportDate >= @StartDate)
      AND (@EndDate IS NULL OR r.ImportDate <= @EndDate)
      AND (@BranchCode IS NULL OR JSON_VALUE(i.RawData, '$.Unit') LIKE '%' + @BranchCode + '%')
      AND (@KpiCode IS NULL OR JSON_VALUE(i.RawData, '$.Indicator') LIKE '%' + @KpiCode + '%')
      AND JSON_VALUE(i.RawData, '$.Q1') IS NOT NULL
    
    UNION ALL
    
    SELECT 
        r.ImportDate,
        '' as EmployeeCode,
        JSON_VALUE(i.RawData, '$.Indicator') as KpiCode,
        JSON_VALUE(i.RawData, '$.Indicator') as KpiName,
        JSON_VALUE(i.RawData, '$.Unit') as BranchCode,
        '' as DepartmentCode,
        CAST(JSON_VALUE(i.RawData, '$.Q2') as DECIMAL(18,4)) as Value,
        JSON_VALUE(i.RawData, '$.Note') as Unit,
        'QUARTERLY' as DataType,
        CAST(r.Id as NVARCHAR(50)) as ImportBatchId,
        r.FileName,
        r.ImportedBy,
        r.ImportDate as ImportedAt,
        i.ValidFrom,
        i.ValidTo
    FROM ImportedDataRecords FOR SYSTEM_TIME AS OF @AsOfDate r
    INNER JOIN ImportedDataItems FOR SYSTEM_TIME AS OF @AsOfDate i ON r.Id = i.ImportedDataRecordId
    WHERE (@StartDate IS NULL OR r.ImportDate >= @StartDate)
      AND (@EndDate IS NULL OR r.ImportDate <= @EndDate)
      AND (@BranchCode IS NULL OR JSON_VALUE(i.RawData, '$.Unit') LIKE '%' + @BranchCode + '%')
      AND (@KpiCode IS NULL OR JSON_VALUE(i.RawData, '$.Indicator') LIKE '%' + @KpiCode + '%')
      AND JSON_VALUE(i.RawData, '$.Q2') IS NOT NULL
    
    ORDER BY ImportDate DESC, BranchCode, KpiCode;
END;
GO

-- Kiểm tra và tạo stored procedure: sp_GetRecordHistory
-- Lấy lịch sử thay đổi của một record cụ thể
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetRecordHistory')
    DROP PROCEDURE sp_GetRecordHistory;
GO

CREATE PROCEDURE sp_GetRecordHistory
    @EmployeeCode NVARCHAR(50) = NULL, -- Not used in current structure
    @KpiCode NVARCHAR(50),
    @BranchCode NVARCHAR(20),
    @ImportDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        r.ImportDate,
        '' as EmployeeCode,
        JSON_VALUE(i.RawData, '$.Indicator') as KpiCode,
        JSON_VALUE(i.RawData, '$.Indicator') as KpiName,
        JSON_VALUE(i.RawData, '$.Unit') as BranchCode,
        '' as DepartmentCode,
        i.RawData as Value, -- Return full JSON for history
        JSON_VALUE(i.RawData, '$.Note') as Unit,
        'QUARTERLY' as DataType,
        CAST(r.Id as NVARCHAR(50)) as ImportBatchId,
        r.FileName,
        r.ImportedBy,
        r.ImportDate as ImportedAt,
        i.ValidFrom,
        i.ValidTo
    FROM ImportedDataRecords FOR SYSTEM_TIME ALL r
    INNER JOIN ImportedDataItems FOR SYSTEM_TIME ALL i ON r.Id = i.ImportedDataRecordId
    WHERE JSON_VALUE(i.RawData, '$.Indicator') LIKE '%' + @KpiCode + '%'
      AND JSON_VALUE(i.RawData, '$.Unit') LIKE '%' + @BranchCode + '%'
      AND (@ImportDate IS NULL OR r.ImportDate = @ImportDate)
    ORDER BY i.ValidFrom DESC, r.ImportDate DESC;
END;
GO

-- Kiểm tra và tạo stored procedure: sp_CompareDataBetweenDates (simplified)
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
    
    -- Tạo temp table để store kết quả so sánh
    CREATE TABLE #ComparisonResult (
        ImportDate DATE,
        EmployeeCode NVARCHAR(50),
        KpiCode NVARCHAR(50),
        KpiName NVARCHAR(200),
        BranchCode NVARCHAR(50),
        Value1 DECIMAL(18,4),
        Value2 DECIMAL(18,4),
        ValueChange DECIMAL(18,4),
        PercentageChange DECIMAL(18,4),
        Unit NVARCHAR(200),
        ChangeType NVARCHAR(20)
    );
    
    -- Insert comparison results cho Q1
    INSERT INTO #ComparisonResult
    SELECT 
        COALESCE(d1.ImportDate, d2.ImportDate) as ImportDate,
        '' as EmployeeCode,
        COALESCE(d1.KpiCode, d2.KpiCode) as KpiCode,
        COALESCE(d1.KpiName, d2.KpiName) as KpiName,
        COALESCE(d1.BranchCode, d2.BranchCode) as BranchCode,
        d1.Q1Value as Value1,
        d2.Q1Value as Value2,
        CASE 
            WHEN d2.Q1Value IS NOT NULL AND d1.Q1Value IS NOT NULL THEN d2.Q1Value - d1.Q1Value
            ELSE NULL 
        END as ValueChange,
        CASE 
            WHEN d1.Q1Value IS NOT NULL AND d1.Q1Value <> 0 AND d2.Q1Value IS NOT NULL 
            THEN ROUND(((d2.Q1Value - d1.Q1Value) / ABS(d1.Q1Value)) * 100, 2)
            ELSE NULL 
        END as PercentageChange,
        COALESCE(d1.Unit, d2.Unit) as Unit,
        CASE 
            WHEN d1.Q1Value IS NULL AND d2.Q1Value IS NOT NULL THEN 'ADDED'
            WHEN d1.Q1Value IS NOT NULL AND d2.Q1Value IS NULL THEN 'REMOVED'
            WHEN d1.Q1Value <> d2.Q1Value THEN 'MODIFIED'
            ELSE 'UNCHANGED'
        END as ChangeType
    FROM (
        SELECT 
            r.ImportDate,
            JSON_VALUE(i.RawData, '$.Unit') as BranchCode,
            JSON_VALUE(i.RawData, '$.Indicator') as KpiCode,
            JSON_VALUE(i.RawData, '$.Indicator') as KpiName,
            CAST(JSON_VALUE(i.RawData, '$.Q1') as DECIMAL(18,4)) as Q1Value,
            JSON_VALUE(i.RawData, '$.Note') as Unit
        FROM ImportedDataRecords FOR SYSTEM_TIME AS OF @Date1 r
        INNER JOIN ImportedDataItems FOR SYSTEM_TIME AS OF @Date1 i ON r.Id = i.ImportedDataRecordId
        WHERE (@BranchCode IS NULL OR JSON_VALUE(i.RawData, '$.Unit') LIKE '%' + @BranchCode + '%')
          AND (@KpiCode IS NULL OR JSON_VALUE(i.RawData, '$.Indicator') LIKE '%' + @KpiCode + '%')
    ) d1
    FULL OUTER JOIN (
        SELECT 
            r.ImportDate,
            JSON_VALUE(i.RawData, '$.Unit') as BranchCode,
            JSON_VALUE(i.RawData, '$.Indicator') as KpiCode,
            JSON_VALUE(i.RawData, '$.Indicator') as KpiName,
            CAST(JSON_VALUE(i.RawData, '$.Q1') as DECIMAL(18,4)) as Q1Value,
            JSON_VALUE(i.RawData, '$.Note') as Unit
        FROM ImportedDataRecords FOR SYSTEM_TIME AS OF @Date2 r
        INNER JOIN ImportedDataItems FOR SYSTEM_TIME AS OF @Date2 i ON r.Id = i.ImportedDataRecordId
        WHERE (@BranchCode IS NULL OR JSON_VALUE(i.RawData, '$.Unit') LIKE '%' + @BranchCode + '%')
          AND (@KpiCode IS NULL OR JSON_VALUE(i.RawData, '$.Indicator') LIKE '%' + @KpiCode + '%')
    ) d2 ON d1.BranchCode = d2.BranchCode 
         AND d1.KpiCode = d2.KpiCode 
         AND d1.ImportDate = d2.ImportDate;
    
    -- Return results
    SELECT * FROM #ComparisonResult
    WHERE Value1 IS NOT NULL OR Value2 IS NOT NULL
    ORDER BY BranchCode, KpiCode, ImportDate;
    
    DROP TABLE #ComparisonResult;
END;
GO

-- Kiểm tra và tạo stored procedure: sp_GetDailyChangeSummary (simplified)
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
    
    -- Simplified comparison - count records only
    SELECT 
        @ReportDate as ReportDate,
        ISNULL(JSON_VALUE(i.RawData, '$.Unit'), 'UNKNOWN') as BranchCode,
        COUNT(*) as NewRecords,
        0 as DeletedRecords,
        0 as ModifiedRecords,
        0 as UnchangedRecords,
        COUNT(*) as TotalComparisons
    FROM ImportedDataRecords r
    INNER JOIN ImportedDataItems i ON r.Id = i.ImportedDataRecordId
    WHERE r.ImportDate = @ReportDate
      AND (@BranchCode IS NULL OR JSON_VALUE(i.RawData, '$.Unit') LIKE '%' + @BranchCode + '%')
    GROUP BY JSON_VALUE(i.RawData, '$.Unit')
    ORDER BY BranchCode;
END;
GO

-- Kiểm tra và tạo stored procedure: sp_GetPerformanceAnalytics (simplified)
-- Phân tích hiệu suất cơ bản
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
    
    SELECT 
        r.ImportDate,
        JSON_VALUE(i.RawData, '$.Unit') as BranchCode,
        '' as EmployeeCode,
        JSON_VALUE(i.RawData, '$.Indicator') as KpiCode,
        JSON_VALUE(i.RawData, '$.Indicator') as KpiName,
        CAST(JSON_VALUE(i.RawData, '$.Q1') as DECIMAL(18,4)) as Value,
        JSON_VALUE(i.RawData, '$.Note') as Unit,
        CAST(JSON_VALUE(i.RawData, '$.Q1') as DECIMAL(18,4)) as MovingAverage7Days, -- Simplified
        0.0 as DayOverDayChange, -- Simplified
        0.5 as PercentileRank, -- Simplified
        i.ValidFrom,
        i.ValidTo
    FROM ImportedDataRecords r
    INNER JOIN ImportedDataItems i ON r.Id = i.ImportedDataRecordId
    WHERE r.ImportDate BETWEEN @StartDate AND @EndDate
      AND (@BranchCode IS NULL OR JSON_VALUE(i.RawData, '$.Unit') LIKE '%' + @BranchCode + '%')
      AND JSON_VALUE(i.RawData, '$.Q1') IS NOT NULL
    ORDER BY r.ImportDate DESC, JSON_VALUE(i.RawData, '$.Unit'), JSON_VALUE(i.RawData, '$.Indicator');
END;
GO

PRINT 'Đã tạo thành công các stored procedures temporal (simplified version)!';
PRINT 'Các procedures đã được đơn giản hóa để tương thích với SQL Server:';
PRINT '- sp_ImportDailyRawData: Import dữ liệu JSON';
PRINT '- sp_GetDataAsOf: Truy vấn temporal với JSON parsing';
PRINT '- sp_CompareDataBetweenDates: So sánh temporal với temp table';
PRINT '- sp_GetRecordHistory: Lịch sử thay đổi temporal';
PRINT '- sp_GetDailyChangeSummary: Tổng hợp thay đổi đơn giản';
PRINT '- sp_GetPerformanceAnalytics: Phân tích hiệu suất cơ bản';
