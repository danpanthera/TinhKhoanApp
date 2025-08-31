-- ========================================
-- 📊 LN03 ADVANCED ANALYTICS & FEATURES
-- ========================================
-- Tạo stored procedures, views và functions cho LN03 analytics

USE TinhKhoanDB;
GO

PRINT '📊 BẮT ĐẦU TẠO ADVANCED FEATURES CHO LN03...';

-- ========================================
-- 1. VIEW: LN03 SUMMARY ANALYTICS  
-- ========================================
PRINT '📈 Đang tạo View analytics tổng quan...';

CREATE OR ALTER VIEW vw_LN03_Summary AS
SELECT 
    -- Tổng quan số lượng
    COUNT(*) as TotalRecords,
    COUNT(DISTINCT MACHINHANH) as UniqueBranches,
    COUNT(DISTINCT MAKH) as UniqueCustomers,
    
    -- Thống kê ngày
    MIN(NGAY_DL) as OldestDate,
    MAX(NGAY_DL) as LatestDate,
    
    -- Thống kê số tiền (chuyển đổi thành bigint để tránh lỗi)
    SUM(CASE WHEN SOTIENXLRR IS NOT NULL 
        THEN CAST(SOTIENXLRR AS BIGINT) ELSE 0 END) as TotalSOTIENXLRR,
    SUM(CASE WHEN DUNONOIBANG IS NOT NULL 
        THEN CAST(DUNONOIBANG AS BIGINT) ELSE 0 END) as TotalDUNONOIBANG,
    SUM(CASE WHEN CONLAINGOAIBANG IS NOT NULL 
        THEN CAST(CONLAINGOAIBANG AS BIGINT) ELSE 0 END) as TotalCONLAINGOAIBANG,
    
    -- Thống kê trung bình
    AVG(CASE WHEN SOTIENXLRR IS NOT NULL 
        THEN CAST(SOTIENXLRR AS BIGINT) ELSE NULL END) as AvgSOTIENXLRR,
    
    -- Thời gian cập nhật
    GETDATE() as LastUpdated
    
FROM LN03 
WHERE IS_DELETED = 0;

PRINT '✅ View vw_LN03_Summary đã được tạo.';

-- ========================================
-- 2. VIEW: LN03 BRANCH ANALYTICS
-- ========================================
PRINT '🏢 Đang tạo View analytics theo chi nhánh...';

CREATE OR ALTER VIEW vw_LN03_BranchAnalytics AS
SELECT 
    MACHINHANH as BranchCode,
    TENCHINHANH as BranchName,
    COUNT(*) as TotalRecords,
    COUNT(DISTINCT MAKH) as UniqueCustomers,
    
    -- Phân tích số tiền theo chi nhánh
    SUM(CASE WHEN SOTIENXLRR IS NOT NULL 
        THEN CAST(SOTIENXLRR AS BIGINT) ELSE 0 END) as BranchTotalSOTIENXLRR,
    AVG(CASE WHEN SOTIENXLRR IS NOT NULL 
        THEN CAST(SOTIENXLRR AS BIGINT) ELSE NULL END) as BranchAvgSOTIENXLRR,
    
    -- Phần trám so với tổng
    CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM LN03 WHERE IS_DELETED = 0) AS DECIMAL(5,2)) as PercentageOfTotal,
    
    MIN(NGAY_DL) as EarliestRecord,
    MAX(NGAY_DL) as LatestRecord

FROM LN03 
WHERE IS_DELETED = 0
GROUP BY MACHINHANH, TENCHINHANH;

PRINT '✅ View vw_LN03_BranchAnalytics đã được tạo.';

-- ========================================
-- 3. STORED PROCEDURE: LN03 DETAILED ANALYTICS
-- ========================================
PRINT '🔧 Đang tạo Stored Procedure analytics chi tiết...';

CREATE OR ALTER PROCEDURE sp_LN03_DetailedAnalytics
    @BranchCode NVARCHAR(50) = NULL,
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @TopN INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Thiết lập default dates nếu không có
    IF @FromDate IS NULL SET @FromDate = '2024-01-01';
    IF @ToDate IS NULL SET @ToDate = GETDATE();
    
    PRINT 'LN03 DETAILED ANALYTICS REPORT';
    PRINT '================================';
    PRINT 'Period: ' + CAST(@FromDate AS VARCHAR) + ' to ' + CAST(@ToDate AS VARCHAR);
    IF @BranchCode IS NOT NULL
        PRINT 'Branch: ' + @BranchCode;
    PRINT '';
    
    -- 1. Tổng quan
    SELECT 
        'TỔNG QUAN' as Section,
        COUNT(*) as TotalRecords,
        COUNT(DISTINCT MACHINHANH) as UniqueBranches,
        COUNT(DISTINCT MAKH) as UniqueCustomers,
        SUM(CASE WHEN SOTIENXLRR IS NOT NULL THEN CAST(SOTIENXLRR AS BIGINT) ELSE 0 END) as TotalAmount
    FROM LN03 
    WHERE IS_DELETED = 0
      AND NGAY_DL BETWEEN @FromDate AND @ToDate
      AND (@BranchCode IS NULL OR MACHINHANH = @BranchCode);
    
    -- 2. Top khách hàng theo số tiền
    SELECT 
        'TOP CUSTOMERS' as Section,
        ROW_NUMBER() OVER (ORDER BY SUM(CASE WHEN SOTIENXLRR IS NOT NULL THEN CAST(SOTIENXLRR AS BIGINT) ELSE 0 END) DESC) as Ranking,
        MAKH as CustomerCode,
        TENKH as CustomerName,
        MACHINHANH as BranchCode,
        COUNT(*) as RecordCount,
        SUM(CASE WHEN SOTIENXLRR IS NOT NULL THEN CAST(SOTIENXLRR AS BIGINT) ELSE 0 END) as TotalAmount
    FROM LN03 
    WHERE IS_DELETED = 0
      AND NGAY_DL BETWEEN @FromDate AND @ToDate
      AND (@BranchCode IS NULL OR MACHINHANH = @BranchCode)
    GROUP BY MAKH, TENKH, MACHINHANH
    ORDER BY TotalAmount DESC
    OFFSET 0 ROWS FETCH NEXT @TopN ROWS ONLY;
    
    -- 3. Phân tích theo nhóm nợ
    SELECT 
        'DEBT GROUP ANALYSIS' as Section,
        NHOMNO as DebtGroup,
        COUNT(*) as RecordCount,
        SUM(CASE WHEN SOTIENXLRR IS NOT NULL THEN CAST(SOTIENXLRR AS BIGINT) ELSE 0 END) as TotalAmount,
        AVG(CASE WHEN SOTIENXLRR IS NOT NULL THEN CAST(SOTIENXLRR AS BIGINT) ELSE NULL END) as AvgAmount
    FROM LN03 
    WHERE IS_DELETED = 0
      AND NGAY_DL BETWEEN @FromDate AND @ToDate
      AND (@BranchCode IS NULL OR MACHINHANH = @BranchCode)
      AND NHOMNO IS NOT NULL AND LTRIM(RTRIM(NHOMNO)) != ''
    GROUP BY NHOMNO
    ORDER BY TotalAmount DESC;
    
END;

PRINT '✅ Stored Procedure sp_LN03_DetailedAnalytics đã được tạo.';

-- ========================================
-- 4. FUNCTION: LN03 CUSTOMER RISK SCORE
-- ========================================
PRINT '🎯 Đang tạo Function tính Risk Score...';

CREATE OR ALTER FUNCTION fn_LN03_CustomerRiskScore(@CustomerCode NVARCHAR(200))
RETURNS DECIMAL(5,2)
AS
BEGIN
    DECLARE @RiskScore DECIMAL(5,2) = 0;
    DECLARE @TotalAmount BIGINT;
    DECLARE @RecordCount INT;
    DECLARE @AvgAmount BIGINT;
    
    -- Lấy thông tin khách hàng
    SELECT 
        @RecordCount = COUNT(*),
        @TotalAmount = SUM(CASE WHEN SOTIENXLRR IS NOT NULL THEN CAST(SOTIENXLRR AS BIGINT) ELSE 0 END),
        @AvgAmount = AVG(CASE WHEN SOTIENXLRR IS NOT NULL THEN CAST(SOTIENXLRR AS BIGINT) ELSE NULL END)
    FROM LN03 
    WHERE MAKH = @CustomerCode AND IS_DELETED = 0;
    
    -- Tính risk score dựa trên:
    -- 1. Số lượng records (nhiều records = rủi ro cao)
    -- 2. Tổng số tiền (số tiền lớn = rủi ro cao)  
    -- 3. Tần suất xuất hiện
    
    IF @RecordCount > 0
    BEGIN
        SET @RiskScore = 
            -- Base score từ số lượng records
            (CASE 
                WHEN @RecordCount >= 10 THEN 40
                WHEN @RecordCount >= 5 THEN 25
                WHEN @RecordCount >= 2 THEN 15
                ELSE 5 
            END) +
            -- Score từ tổng số tiền  
            (CASE 
                WHEN @TotalAmount >= 50000000 THEN 35
                WHEN @TotalAmount >= 10000000 THEN 25
                WHEN @TotalAmount >= 1000000 THEN 15
                ELSE 5
            END) +
            -- Score từ average amount
            (CASE
                WHEN @AvgAmount >= 10000000 THEN 25
                WHEN @AvgAmount >= 5000000 THEN 15
                WHEN @AvgAmount >= 1000000 THEN 10
                ELSE 0
            END);
            
        -- Đảm bảo score không vượt quá 100
        IF @RiskScore > 100 SET @RiskScore = 100;
    END
    
    RETURN @RiskScore;
END;

PRINT '✅ Function fn_LN03_CustomerRiskScore đã được tạo.';

-- ========================================
-- 5. TEST CÁC FEATURES MỚI
-- ========================================
PRINT '';
PRINT '🧪 TESTING ADVANCED FEATURES...';
PRINT '';

-- Test View Summary
PRINT '📊 Testing Summary View:';
SELECT * FROM vw_LN03_Summary;

PRINT '';
PRINT '🏢 Testing Branch Analytics (Top 3):';
SELECT TOP 3 * FROM vw_LN03_BranchAnalytics ORDER BY TotalRecords DESC;

-- Test Risk Score Function cho một số khách hàng
PRINT '';
PRINT '🎯 Testing Risk Score Function:';
SELECT TOP 5
    MAKH as CustomerCode,
    TENKH as CustomerName,
    COUNT(*) as RecordCount,
    dbo.fn_LN03_CustomerRiskScore(MAKH) as RiskScore
FROM LN03 
WHERE IS_DELETED = 0
GROUP BY MAKH, TENKH
ORDER BY RiskScore DESC;

-- ========================================
-- 6. TẠO INDEXES CHO PERFORMANCE
-- ========================================
PRINT '';
PRINT '⚡ Tạo indexes cho analytics performance...';

-- Index cho customer analytics
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LN03_Customer_Analytics')
BEGIN
    CREATE NONCLUSTERED INDEX IX_LN03_Customer_Analytics
    ON LN03 (MAKH, IS_DELETED)
    INCLUDE (SOTIENXLRR, DUNONOIBANG, TENKH);
    PRINT '✅ Customer analytics index created.';
END

-- Index cho date range queries
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LN03_DateRange_Analytics')  
BEGIN
    CREATE NONCLUSTERED INDEX IX_LN03_DateRange_Analytics
    ON LN03 (NGAY_DL, IS_DELETED)
    INCLUDE (MACHINHANH, MAKH, SOTIENXLRR);
    PRINT '✅ Date range analytics index created.';
END

-- ========================================
-- 7. ADVANCED FEATURES COMPLETE
-- ========================================
PRINT '';
PRINT '🎉 ADVANCED FEATURES & ANALYTICS HOÀN THÀNH!';
PRINT '';
PRINT '📊 FEATURES ĐÃ TẠO:';
PRINT '   ✅ Views: vw_LN03_Summary, vw_LN03_BranchAnalytics';
PRINT '   ✅ Stored Procedure: sp_LN03_DetailedAnalytics';
PRINT '   ✅ Function: fn_LN03_CustomerRiskScore';
PRINT '   ✅ Analytics Indexes: Customer & DateRange optimized';
PRINT '';
PRINT '🚀 LN03 ADVANCED ANALYTICS SYSTEM READY!';

GO
