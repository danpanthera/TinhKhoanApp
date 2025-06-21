-- =============================================
-- Script: 04_TemporalQueryProcedures.sql
-- Purpose: Temporal Query Procedures for Point-in-Time Analysis
-- Author: System Architecture
-- Date: 2025-01-15
-- Notes: AS OF and BETWEEN queries for change tracking analytics
-- =============================================

USE TinhKhoanDB;
GO

-- 1. Point-in-Time Data Query
CREATE PROCEDURE sp_GetDataAsOf
    @AsOfDate DATETIME2,
    @BranchCode NVARCHAR(10) = NULL,
    @EmployeeCode NVARCHAR(20) = NULL,
    @KpiCode NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        r.Id,
        r.ImportDate,
        r.BranchCode,
        r.DepartmentCode,
        r.EmployeeCode,
        r.KpiCode,
        r.KpiValue,
        r.Unit,
        r.Target,
        r.Achievement,
        r.Score,
        r.Note,
        r.ImportBatchId,
        r.CreatedDate,
        r.CreatedBy,
        r.LastModifiedDate,
        r.LastModifiedBy
    FROM dbo.RawDataImports FOR SYSTEM_TIME AS OF @AsOfDate r
    WHERE r.IsDeleted = 0
      AND (@BranchCode IS NULL OR r.BranchCode = @BranchCode)
      AND (@EmployeeCode IS NULL OR r.EmployeeCode = @EmployeeCode)
      AND (@KpiCode IS NULL OR r.KpiCode = @KpiCode)
    ORDER BY r.BranchCode, r.EmployeeCode, r.KpiCode;
END;
GO

-- 2. Change History Between Two Points in Time
CREATE PROCEDURE sp_GetChangesBetween
    @StartDate DATETIME2,
    @EndDate DATETIME2,
    @BranchCode NVARCHAR(10) = NULL,
    @EmployeeCode NVARCHAR(20) = NULL,
    @KpiCode NVARCHAR(20) = NULL,
    @ShowOnlyChangedRecords BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    @KpiCode NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    WITH Data_Date1 AS (
        SELECT 
            ImportDate,
            EmployeeCode,
            KpiCode,
            KpiName,
            BranchCode,
            Value as Value1,
            Unit
        FROM RawDataImports
        FOR SYSTEM_TIME AS OF @Date1
        WHERE IsDeleted = 0
            AND (@EmployeeCode IS NULL OR EmployeeCode = @EmployeeCode)
            AND (@BranchCode IS NULL OR BranchCode = @BranchCode)
            AND (@KpiCode IS NULL OR KpiCode = @KpiCode)
    ),
    Data_Date2 AS (
        SELECT 
            ImportDate,
            EmployeeCode,
            KpiCode,
            KpiName,
            BranchCode,
            Value as Value2,
            Unit
        FROM RawDataImports
        FOR SYSTEM_TIME AS OF @Date2
        WHERE IsDeleted = 0
            AND (@EmployeeCode IS NULL OR EmployeeCode = @EmployeeCode)
            AND (@BranchCode IS NULL OR BranchCode = @BranchCode)
            AND (@KpiCode IS NULL OR KpiCode = @KpiCode)
    )
    SELECT 
        COALESCE(d1.ImportDate, d2.ImportDate) as ImportDate,
        COALESCE(d1.EmployeeCode, d2.EmployeeCode) as EmployeeCode,
        COALESCE(d1.KpiCode, d2.KpiCode) as KpiCode,
        COALESCE(d1.KpiName, d2.KpiName) as KpiName,
        COALESCE(d1.BranchCode, d2.BranchCode) as BranchCode,
        d1.Value1,
        d2.Value2,
        d2.Value2 - d1.Value1 as ValueChange,
        CASE 
            WHEN d1.Value1 = 0 THEN NULL
            ELSE ((d2.Value2 - d1.Value1) / d1.Value1) * 100
        END as PercentageChange,
        COALESCE(d1.Unit, d2.Unit) as Unit,
        CASE 
            WHEN d1.Value1 IS NULL THEN 'ADDED'
            WHEN d2.Value2 IS NULL THEN 'REMOVED'
            WHEN d1.Value1 <> d2.Value2 THEN 'CHANGED'
            ELSE 'UNCHANGED'
        END as ChangeType
    FROM Data_Date1 d1
    FULL OUTER JOIN Data_Date2 d2 
        ON d1.ImportDate = d2.ImportDate
        AND d1.EmployeeCode = d2.EmployeeCode 
        AND d1.KpiCode = d2.KpiCode
        AND d1.BranchCode = d2.BranchCode
    WHERE (d1.Value1 IS NULL OR d2.Value2 IS NULL OR d1.Value1 <> d2.Value2)
    ORDER BY COALESCE(d1.ImportDate, d2.ImportDate) DESC, 
             COALESCE(d1.EmployeeCode, d2.EmployeeCode), 
             COALESCE(d1.KpiCode, d2.KpiCode);
END;
GO

-- 3. Daily Comparison Procedure (Today vs Yesterday)
CREATE PROCEDURE sp_GetDailyChanges
    @TargetDate DATE = NULL,
    @BranchCode NVARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @TargetDate IS NULL
        SET @TargetDate = CAST(GETDATE() AS DATE);
    
    DECLARE @TodayEnd DATETIME2 = DATEADD(DAY, 1, @TargetDate);
    DECLARE @YesterdayEnd DATETIME2 = @TargetDate;
    
    EXEC sp_GetChangesBetween 
        @StartDate = @YesterdayEnd,
        @EndDate = @TodayEnd,
        @BranchCode = @BranchCode,
        @ShowOnlyChangedRecords = 1;
END;
GO

-- 4. Complete History for Specific Record
CREATE PROCEDURE sp_GetRecordHistory
    @ImportDate DATE,
    @BranchCode NVARCHAR(10),
    @EmployeeCode NVARCHAR(20),
    @KpiCode NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get all changes for the specific record
    SELECT 
        r.KpiValue,
        r.Score,
        r.Achievement,
        r.Target,
        r.Unit,
        r.Note,
        r.ImportBatchId,
        r.LastModifiedDate,
        r.LastModifiedBy,
        r.ValidFrom,
        r.ValidTo,
        CASE 
            WHEN r.ValidTo = '9999-12-31 23:59:59.9999999' THEN 'CURRENT'
            ELSE 'HISTORICAL'
        END AS RecordStatus
    FROM dbo.RawDataImports FOR SYSTEM_TIME ALL r
    WHERE r.ImportDate = @ImportDate
      AND r.BranchCode = @BranchCode
      AND r.EmployeeCode = @EmployeeCode
      AND r.KpiCode = @KpiCode
    ORDER BY r.ValidFrom DESC;
END;
GO

-- 5. Top Performers Analysis with Historical Context
CREATE PROCEDURE sp_GetTopPerformersWithHistory
    @AnalysisDate DATE = NULL,
    @BranchCode NVARCHAR(10) = NULL,
    @TopN INT = 10,
    @ComparisonDays INT = 30
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @AnalysisDate IS NULL
        SET @AnalysisDate = CAST(GETDATE() AS DATE);
    
    DECLARE @ComparisonDate DATE = DATEADD(DAY, -@ComparisonDays, @AnalysisDate);
    DECLARE @CurrentSnapshot DATETIME2 = DATEADD(DAY, 1, @AnalysisDate);
    DECLARE @PreviousSnapshot DATETIME2 = DATEADD(DAY, 1, @ComparisonDate);
    
    WITH CurrentPerformers AS (
        SELECT TOP (@TopN)
            r.BranchCode,
            r.EmployeeCode,
            AVG(r.Score) AS CurrentAvgScore,
            SUM(r.KpiValue) AS CurrentTotalValue,
            COUNT(*) AS CurrentKpiCount
        FROM dbo.RawDataImports FOR SYSTEM_TIME AS OF @CurrentSnapshot r
        WHERE r.IsDeleted = 0
          AND r.ImportDate = @AnalysisDate
          AND (@BranchCode IS NULL OR r.BranchCode = @BranchCode)
        GROUP BY r.BranchCode, r.EmployeeCode
        ORDER BY AVG(r.Score) DESC
    ),
    PreviousPerformers AS (
        SELECT 
            r.BranchCode,
            r.EmployeeCode,
            AVG(r.Score) AS PreviousAvgScore,
            SUM(r.KpiValue) AS PreviousTotalValue,
            COUNT(*) AS PreviousKpiCount
        FROM dbo.RawDataImports FOR SYSTEM_TIME AS OF @PreviousSnapshot r
        WHERE r.IsDeleted = 0
          AND r.ImportDate = @ComparisonDate
          AND (@BranchCode IS NULL OR r.BranchCode = @BranchCode)
        GROUP BY r.BranchCode, r.EmployeeCode
    )
    SELECT 
        c.BranchCode,
        c.EmployeeCode,
        c.CurrentAvgScore,
        ISNULL(p.PreviousAvgScore, 0) AS PreviousAvgScore,
        c.CurrentAvgScore - ISNULL(p.PreviousAvgScore, 0) AS ScoreImprovement,
        c.CurrentTotalValue,
        ISNULL(p.PreviousTotalValue, 0) AS PreviousTotalValue,
        c.CurrentTotalValue - ISNULL(p.PreviousTotalValue, 0) AS ValueImprovement,
        c.CurrentKpiCount,
        ISNULL(p.PreviousKpiCount, 0) AS PreviousKpiCount,
        CASE 
            WHEN p.PreviousAvgScore IS NULL THEN 'NEW PERFORMER'
            WHEN c.CurrentAvgScore > p.PreviousAvgScore THEN 'IMPROVED'
            WHEN c.CurrentAvgScore = p.PreviousAvgScore THEN 'STABLE'
            ELSE 'DECLINED'
        END AS PerformanceTrend
    FROM CurrentPerformers c
    LEFT JOIN PreviousPerformers p 
        ON c.BranchCode = p.BranchCode 
        AND c.EmployeeCode = p.EmployeeCode
    ORDER BY c.CurrentAvgScore DESC;
END;
GO

-- 6. Branch Performance Comparison Over Time
CREATE PROCEDURE sp_GetBranchPerformanceTrend
    @StartDate DATE,
    @EndDate DATE,
    @BranchCode NVARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Generate date series for analysis
    WITH DateSeries AS (
        SELECT @StartDate AS AnalysisDate
        UNION ALL
        SELECT DATEADD(DAY, 1, AnalysisDate)
        FROM DateSeries
        WHERE AnalysisDate < @EndDate
    ),
    BranchPerformance AS (
        SELECT 
            d.AnalysisDate,
            r.BranchCode,
            AVG(r.Score) AS AvgScore,
            SUM(r.KpiValue) AS TotalValue,
            COUNT(DISTINCT r.EmployeeCode) AS EmployeeCount,
            COUNT(*) AS KpiCount
        FROM DateSeries d
        CROSS JOIN (
            SELECT DISTINCT BranchCode 
            FROM dbo.RawDataImports 
            WHERE (@BranchCode IS NULL OR BranchCode = @BranchCode)
        ) branches
        LEFT JOIN dbo.RawDataImports FOR SYSTEM_TIME AS OF DATEADD(DAY, 1, d.AnalysisDate) r
            ON r.BranchCode = branches.BranchCode
            AND r.ImportDate = d.AnalysisDate
            AND r.IsDeleted = 0
        GROUP BY d.AnalysisDate, r.BranchCode
    )
    SELECT 
        AnalysisDate,
        BranchCode,
        AvgScore,
        LAG(AvgScore) OVER (PARTITION BY BranchCode ORDER BY AnalysisDate) AS PreviousAvgScore,
        AvgScore - LAG(AvgScore) OVER (PARTITION BY BranchCode ORDER BY AnalysisDate) AS ScoreChange,
        TotalValue,
        EmployeeCount,
        KpiCount
    FROM BranchPerformance
    WHERE BranchCode IS NOT NULL
    ORDER BY BranchCode, AnalysisDate
    OPTION (MAXRECURSION 365);
END;
GO

PRINT 'Temporal query procedures created successfully';
PRINT 'Available procedures:';
PRINT '- sp_GetDataAsOf: Point-in-time data snapshot';
PRINT '- sp_GetChangesBetween: Compare data between two time points';
PRINT '- sp_GetDailyChanges: Daily change analysis';
PRINT '- sp_GetRecordHistory: Complete history of a specific record';
PRINT '- sp_GetTopPerformersWithHistory: Top performers with historical context';
PRINT '- sp_GetBranchPerformanceTrend: Branch performance over time';
