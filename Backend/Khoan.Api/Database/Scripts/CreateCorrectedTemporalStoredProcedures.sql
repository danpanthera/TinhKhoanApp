-- ===========================================
-- TinhKhoanApp - Corrected Temporal Tables Stored Procedures
-- Dựa trên cấu trúc dữ liệu thực tế: ImportedDataItems với JSON RawData
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
        WITH ParsedData AS (
            SELECT 
                @RecordId as ImportedDataRecordId,
                value as RawDataJson,
                GETUTCDATE() as ProcessedDate
            FROM OPENJSON(@JsonData)
        )
        INSERT INTO ImportedDataItems (
            ImportedDataRecordId, 
            RawData, 
            ProcessedDate,
            ProcessingNotes
        )
        SELECT 
            ImportedDataRecordId,
            RawDataJson,
            ProcessedDate,
            'Processed via sp_ImportDailyRawData'
        FROM ParsedData;
        
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
    
    WITH HistoricalData AS (
        SELECT 
            r.ImportDate,
            JSON_VALUE(i.RawData, '$.Unit') as BranchCode,
            JSON_VALUE(i.RawData, '$.Indicator') as KpiCode,
            JSON_VALUE(i.RawData, '$.Indicator') as KpiName,
            JSON_VALUE(i.RawData, '$.Q1') as Q1Value,
            JSON_VALUE(i.RawData, '$.Q2') as Q2Value,
            JSON_VALUE(i.RawData, '$.Q3') as Q3Value,
            JSON_VALUE(i.RawData, '$.Q4') as Q4Value,
            JSON_VALUE(i.RawData, '$.Note') as Unit,
            r.FileName,
            r.ImportedBy,
            r.ImportDate as ImportedAt,
            i.ValidFrom,
            i.ValidTo,
            'QUARTERLY' as DataType,
            r.Id as ImportBatchId
        FROM ImportedDataRecords r
        INNER JOIN ImportedDataItems i ON r.Id = i.ImportedDataRecordId
        FOR SYSTEM_TIME AS OF @AsOfDate
        WHERE (@StartDate IS NULL OR r.ImportDate >= @StartDate)
          AND (@EndDate IS NULL OR r.ImportDate <= @EndDate)
          AND (@BranchCode IS NULL OR JSON_VALUE(i.RawData, '$.Unit') LIKE '%' + @BranchCode + '%')
          AND (@KpiCode IS NULL OR JSON_VALUE(i.RawData, '$.Indicator') LIKE '%' + @KpiCode + '%')
    )
    -- Unpivot quarterly data to individual records
    SELECT 
        ImportDate,
        '' as EmployeeCode, -- Not applicable for this data structure
        KpiCode,
        KpiName,
        BranchCode,
        '' as DepartmentCode, -- Not applicable
        CAST(Value as DECIMAL(18,4)) as Value,
        Unit,
        DataType,
        ImportBatchId,
        FileName,
        ImportedBy,
        ImportedAt,
        ValidFrom,
        ValidTo
    FROM HistoricalData
    UNPIVOT (
        Value FOR Quarter IN (Q1Value, Q2Value, Q3Value, Q4Value)
    ) as pvt
    WHERE Value IS NOT NULL AND Value != ''
    ORDER BY ImportDate DESC, BranchCode, KpiCode;
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
            r.ImportDate,
            JSON_VALUE(i.RawData, '$.Unit') as BranchCode,
            JSON_VALUE(i.RawData, '$.Indicator') as KpiCode,
            JSON_VALUE(i.RawData, '$.Indicator') as KpiName,
            JSON_VALUE(i.RawData, '$.Q1') as Q1Value,
            JSON_VALUE(i.RawData, '$.Q2') as Q2Value,
            JSON_VALUE(i.RawData, '$.Q3') as Q3Value,
            JSON_VALUE(i.RawData, '$.Q4') as Q4Value,
            JSON_VALUE(i.RawData, '$.Note') as Unit
        FROM ImportedDataRecords r
        INNER JOIN ImportedDataItems i ON r.Id = i.ImportedDataRecordId
        FOR SYSTEM_TIME AS OF @Date1
        WHERE (@BranchCode IS NULL OR JSON_VALUE(i.RawData, '$.Unit') LIKE '%' + @BranchCode + '%')
          AND (@KpiCode IS NULL OR JSON_VALUE(i.RawData, '$.Indicator') LIKE '%' + @KpiCode + '%')
    ),
    Data2 AS (
        SELECT 
            r.ImportDate,
            JSON_VALUE(i.RawData, '$.Unit') as BranchCode,
            JSON_VALUE(i.RawData, '$.Indicator') as KpiCode,
            JSON_VALUE(i.RawData, '$.Indicator') as KpiName,
            JSON_VALUE(i.RawData, '$.Q1') as Q1Value,
            JSON_VALUE(i.RawData, '$.Q2') as Q2Value,
            JSON_VALUE(i.RawData, '$.Q3') as Q3Value,
            JSON_VALUE(i.RawData, '$.Q4') as Q4Value,
            JSON_VALUE(i.RawData, '$.Note') as Unit
        FROM ImportedDataRecords r
        INNER JOIN ImportedDataItems i ON r.Id = i.ImportedDataRecordId
        FOR SYSTEM_TIME AS OF @Date2
        WHERE (@BranchCode IS NULL OR JSON_VALUE(i.RawData, '$.Unit') LIKE '%' + @BranchCode + '%')
          AND (@KpiCode IS NULL OR JSON_VALUE(i.RawData, '$.Indicator') LIKE '%' + @KpiCode + '%')
    ),
    Comparison AS (
        SELECT 
            COALESCE(d1.ImportDate, d2.ImportDate) as ImportDate,
            '' as EmployeeCode,
            COALESCE(d1.KpiCode, d2.KpiCode) as KpiCode,
            COALESCE(d1.KpiName, d2.KpiName) as KpiName,
            COALESCE(d1.BranchCode, d2.BranchCode) as BranchCode,
            d1.Q1Value as Value1_Q1, d2.Q1Value as Value2_Q1,
            d1.Q2Value as Value1_Q2, d2.Q2Value as Value2_Q2,
            d1.Q3Value as Value1_Q3, d2.Q3Value as Value2_Q3,
            d1.Q4Value as Value1_Q4, d2.Q4Value as Value2_Q4,
            COALESCE(d1.Unit, d2.Unit) as Unit
        FROM Data1 d1
        FULL OUTER JOIN Data2 d2 ON d1.BranchCode = d2.BranchCode 
                                 AND d1.KpiCode = d2.KpiCode 
                                 AND d1.ImportDate = d2.ImportDate
    )
    -- Return comparison for each quarter
    SELECT 
        ImportDate, EmployeeCode, KpiCode, KpiName, BranchCode,
        TRY_CAST(Value1 as DECIMAL(18,4)) as Value1,
        TRY_CAST(Value2 as DECIMAL(18,4)) as Value2,
        CASE 
            WHEN TRY_CAST(Value2 as DECIMAL(18,4)) IS NOT NULL AND TRY_CAST(Value1 as DECIMAL(18,4)) IS NOT NULL 
            THEN TRY_CAST(Value2 as DECIMAL(18,4)) - TRY_CAST(Value1 as DECIMAL(18,4))
            ELSE NULL 
        END as ValueChange,
        CASE 
            WHEN TRY_CAST(Value1 as DECIMAL(18,4)) IS NOT NULL AND TRY_CAST(Value1 as DECIMAL(18,4)) <> 0 AND TRY_CAST(Value2 as DECIMAL(18,4)) IS NOT NULL 
            THEN ROUND(((TRY_CAST(Value2 as DECIMAL(18,4)) - TRY_CAST(Value1 as DECIMAL(18,4))) / ABS(TRY_CAST(Value1 as DECIMAL(18,4)))) * 100, 2)
            ELSE NULL 
        END as PercentageChange,
        Unit,
        CASE 
            WHEN Value1 IS NULL AND Value2 IS NOT NULL THEN 'ADDED'
            WHEN Value1 IS NOT NULL AND Value2 IS NULL THEN 'REMOVED'
            WHEN Value1 <> Value2 THEN 'MODIFIED'
            ELSE 'UNCHANGED'
        END as ChangeType
    FROM Comparison
    UNPIVOT (
        (Value1, Value2) FOR Quarter IN ((Value1_Q1, Value2_Q1), (Value1_Q2, Value2_Q2), (Value1_Q3, Value2_Q3), (Value1_Q4, Value2_Q4))
    ) as pvt
    WHERE Value1 IS NOT NULL OR Value2 IS NOT NULL
    ORDER BY BranchCode, KpiCode, ImportDate;
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
        r.Id as ImportBatchId,
        r.FileName,
        r.ImportedBy,
        r.ImportDate as ImportedAt,
        i.ValidFrom,
        i.ValidTo
    FROM ImportedDataRecords r
    INNER JOIN ImportedDataItems i ON r.Id = i.ImportedDataRecordId
    FOR SYSTEM_TIME ALL
    WHERE JSON_VALUE(i.RawData, '$.Indicator') LIKE '%' + @KpiCode + '%'
      AND JSON_VALUE(i.RawData, '$.Unit') LIKE '%' + @BranchCode + '%'
      AND (@ImportDate IS NULL OR r.ImportDate = @ImportDate)
    ORDER BY i.ValidFrom DESC, r.ImportDate DESC;
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
            JSON_VALUE(i.RawData, '$.Unit') as BranchCode,
            JSON_VALUE(i.RawData, '$.Indicator') as KpiCode,
            r.ImportDate,
            i.RawData
        FROM ImportedDataRecords r
        INNER JOIN ImportedDataItems i ON r.Id = i.ImportedDataRecordId
        FOR SYSTEM_TIME AS OF @ReportDateTime
        WHERE r.ImportDate = @ReportDate
          AND (@BranchCode IS NULL OR JSON_VALUE(i.RawData, '$.Unit') LIKE '%' + @BranchCode + '%')
    ),
    PreviousData AS (
        SELECT DISTINCT
            JSON_VALUE(i.RawData, '$.Unit') as BranchCode,
            JSON_VALUE(i.RawData, '$.Indicator') as KpiCode,
            r.ImportDate,
            i.RawData
        FROM ImportedDataRecords r
        INNER JOIN ImportedDataItems i ON r.Id = i.ImportedDataRecordId
        FOR SYSTEM_TIME AS OF @PreviousDateTime
        WHERE r.ImportDate = @PreviousDate
          AND (@BranchCode IS NULL OR JSON_VALUE(i.RawData, '$.Unit') LIKE '%' + @BranchCode + '%')
    ),
    Changes AS (
        SELECT 
            COALESCE(c.BranchCode, p.BranchCode) as BranchCode,
            CASE 
                WHEN p.RawData IS NULL AND c.RawData IS NOT NULL THEN 'NEW'
                WHEN p.RawData IS NOT NULL AND c.RawData IS NULL THEN 'DELETED'
                WHEN p.RawData <> c.RawData THEN 'MODIFIED'
                ELSE 'UNCHANGED'
            END as ChangeType
        FROM CurrentData c
        FULL OUTER JOIN PreviousData p ON c.BranchCode = p.BranchCode 
                                      AND c.KpiCode = p.KpiCode
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
-- Phân tích hiệu suất với trends
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetPerformanceAnalytics')
    DROP PROCEDURE sp_GetPerformanceAnalytics;
GO

CREATE PROCEDURE sp_GetPerformanceAnalytics
    @StartDate DATE,
    @EndDate DATE,
    @BranchCode NVARCHAR(20) = NULL,
    @EmployeeCode NVARCHAR(50) = NULL -- Not used in current structure
AS
BEGIN
    SET NOCOUNT ON;
    
    WITH QuarterlyData AS (
        SELECT 
            r.ImportDate,
            JSON_VALUE(i.RawData, '$.Unit') as BranchCode,
            '' as EmployeeCode,
            JSON_VALUE(i.RawData, '$.Indicator') as KpiCode,
            JSON_VALUE(i.RawData, '$.Indicator') as KpiName,
            JSON_VALUE(i.RawData, '$.Q1') as Q1Value,
            JSON_VALUE(i.RawData, '$.Q2') as Q2Value,
            JSON_VALUE(i.RawData, '$.Q3') as Q3Value,
            JSON_VALUE(i.RawData, '$.Q4') as Q4Value,
            JSON_VALUE(i.RawData, '$.Note') as Unit,
            i.ValidFrom,
            i.ValidTo,
            ROW_NUMBER() OVER (PARTITION BY JSON_VALUE(i.RawData, '$.Unit'), JSON_VALUE(i.RawData, '$.Indicator'), r.ImportDate ORDER BY i.ValidFrom DESC) as rn
        FROM ImportedDataRecords r
        INNER JOIN ImportedDataItems i ON r.Id = i.ImportedDataRecordId
        WHERE r.ImportDate BETWEEN @StartDate AND @EndDate
          AND (@BranchCode IS NULL OR JSON_VALUE(i.RawData, '$.Unit') LIKE '%' + @BranchCode + '%')
    ),
    UnpivotedData AS (
        SELECT 
            ImportDate, BranchCode, EmployeeCode, KpiCode, KpiName,
            TRY_CAST(Value as DECIMAL(18,4)) as Value,
            Unit, ValidFrom, ValidTo,
            Quarter
        FROM QuarterlyData
        UNPIVOT (
            Value FOR Quarter IN (Q1Value, Q2Value, Q3Value, Q4Value)
        ) as pvt
        WHERE rn = 1 AND Value IS NOT NULL AND Value != ''
    ),
    Analytics AS (
        SELECT 
            ImportDate, BranchCode, EmployeeCode, KpiCode, KpiName,
            Value, Unit, ValidFrom, ValidTo,
            AVG(Value) OVER (
                PARTITION BY BranchCode, KpiCode 
                ORDER BY ImportDate 
                ROWS BETWEEN 6 PRECEDING AND CURRENT ROW
            ) as MovingAverage7Days,
            Value - LAG(Value, 1) OVER (
                PARTITION BY BranchCode, KpiCode 
                ORDER BY ImportDate
            ) as DayOverDayChange,
            PERCENT_RANK() OVER (
                PARTITION BY KpiCode, ImportDate 
                ORDER BY Value
            ) as PercentileRank
        FROM UnpivotedData
    )
    SELECT 
        ImportDate, BranchCode, EmployeeCode, KpiCode, KpiName,
        Value, Unit,
        ISNULL(MovingAverage7Days, Value) as MovingAverage7Days,
        ISNULL(DayOverDayChange, 0) as DayOverDayChange,
        CAST(PercentileRank as DECIMAL(5,4)) as PercentileRank,
        ValidFrom, ValidTo
    FROM Analytics
    ORDER BY ImportDate DESC, BranchCode, KpiCode;
END;
GO

-- Các procedures còn lại giữ nguyên vì không liên quan đến cấu trúc dữ liệu
-- sp_ArchiveOldTemporalData, sp_MaintainTemporalIndexes, sp_TemporalHealthCheck

PRINT 'Đã tạo thành công các stored procedures chính cho Temporal Tables!';
PRINT 'Các procedures đã được điều chỉnh để phù hợp với cấu trúc dữ liệu JSON thực tế:';
PRINT '- sp_ImportDailyRawData: Import dữ liệu JSON vào ImportedDataItems';
PRINT '- sp_GetDataAsOf: Truy vấn với JSON parsing và UNPIVOT';
PRINT '- sp_CompareDataBetweenDates: So sánh dữ liệu JSON giữa 2 thời điểm';
PRINT '- sp_GetRecordHistory: Lịch sử thay đổi với JSON format';
PRINT '- sp_GetDailyChangeSummary: Tổng hợp thay đổi từ JSON data';
PRINT '- sp_GetPerformanceAnalytics: Phân tích hiệu suất từ quarterly JSON data';
