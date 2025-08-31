-- ========================================
-- 📊 LN03 ADVANCED ANALYTICS & FEATURES (FIXED)
-- ========================================
USE TinhKhoanDB;
GO

PRINT '📊 BẮT ĐẦU TẠO ADVANCED FEATURES CHO LN03...';
GO

-- ========================================
-- 1. VIEW: LN03 SUMMARY ANALYTICS  
-- ========================================
CREATE OR ALTER VIEW vw_LN03_Summary AS
SELECT 
    COUNT(*) as TotalRecords,
    COUNT(DISTINCT MACHINHANH) as UniqueBranches,
    COUNT(DISTINCT MAKH) as UniqueCustomers,
    MIN(NGAY_DL) as OldestDate,
    MAX(NGAY_DL) as LatestDate,
    SUM(CASE WHEN SOTIENXLRR IS NOT NULL THEN CAST(SOTIENXLRR AS BIGINT) ELSE 0 END) as TotalSOTIENXLRR,
    AVG(CASE WHEN SOTIENXLRR IS NOT NULL THEN CAST(SOTIENXLRR AS BIGINT) ELSE NULL END) as AvgSOTIENXLRR,
    GETDATE() as LastUpdated
FROM LN03 WHERE IS_DELETED = 0;
GO

PRINT '✅ View vw_LN03_Summary đã được tạo.';
GO

-- ========================================
-- 2. VIEW: LN03 BRANCH ANALYTICS
-- ========================================  
CREATE OR ALTER VIEW vw_LN03_BranchAnalytics AS
SELECT 
    MACHINHANH as BranchCode,
    TENCHINHANH as BranchName,
    COUNT(*) as TotalRecords,
    COUNT(DISTINCT MAKH) as UniqueCustomers,
    SUM(CASE WHEN SOTIENXLRR IS NOT NULL THEN CAST(SOTIENXLRR AS BIGINT) ELSE 0 END) as BranchTotalSOTIENXLRR,
    AVG(CASE WHEN SOTIENXLRR IS NOT NULL THEN CAST(SOTIENXLRR AS BIGINT) ELSE NULL END) as BranchAvgSOTIENXLRR,
    MIN(NGAY_DL) as EarliestRecord,
    MAX(NGAY_DL) as LatestRecord
FROM LN03 
WHERE IS_DELETED = 0
GROUP BY MACHINHANH, TENCHINHANH;
GO

PRINT '✅ View vw_LN03_BranchAnalytics đã được tạo.';
GO

-- ========================================
-- 3. STORED PROCEDURE: SIMPLE ANALYTICS
-- ========================================
CREATE OR ALTER PROCEDURE sp_LN03_SimpleAnalytics
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Tổng quan
    SELECT 
        'TỔNG QUAN' as Section,
        COUNT(*) as TotalRecords,
        COUNT(DISTINCT MACHINHANH) as UniqueBranches,
        COUNT(DISTINCT MAKH) as UniqueCustomers
    FROM LN03 WHERE IS_DELETED = 0;
    
    -- Top 5 chi nhánh
    SELECT TOP 5
        'TOP BRANCHES' as Section,
        MACHINHANH as BranchCode,
        COUNT(*) as RecordCount
    FROM LN03 
    WHERE IS_DELETED = 0
    GROUP BY MACHINHANH
    ORDER BY RecordCount DESC;
END;
GO

PRINT '✅ Stored Procedure đã được tạo.';
GO

-- ========================================
-- 4. TEST FEATURES
-- ========================================
PRINT '🧪 Testing Summary View:';
SELECT * FROM vw_LN03_Summary;
GO

PRINT '🏢 Testing Branch Analytics:';
SELECT TOP 3 * FROM vw_LN03_BranchAnalytics ORDER BY TotalRecords DESC;
GO

PRINT '🔧 Testing Stored Procedure:';
EXEC sp_LN03_SimpleAnalytics;
GO

-- ========================================
-- 5. CREATE ANALYTICS INDEXES
-- ========================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LN03_Analytics_Customer')
BEGIN
    CREATE NONCLUSTERED INDEX IX_LN03_Analytics_Customer
    ON LN03 (MAKH, IS_DELETED)
    INCLUDE (SOTIENXLRR, TENKH, MACHINHANH);
    PRINT '✅ Customer analytics index created.';
END
GO

PRINT '🎉 ADVANCED ANALYTICS FEATURES HOÀN THÀNH!';
PRINT 'Views, Procedures và Indexes đã sẵn sàng cho production.';
GO
