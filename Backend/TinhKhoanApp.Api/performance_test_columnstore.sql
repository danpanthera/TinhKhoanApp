-- Performance Testing Script for Columnstore Indexes
-- Compare query performance before/after columnstore indexes

USE TinhKhoanDB;
GO

-- Enable query execution time measurement
SET STATISTICS TIME ON;
SET STATISTICS IO ON;
GO

PRINT '🚀 Starting Performance Tests with Columnstore Indexes...'
PRINT '=================================================='

-- Test 1: Count records by date range (typical analytics query)
PRINT '📊 Test 1: Count records by NGAY_DL range'
PRINT '------------------------------------------'

-- DPDA Analytics Query
PRINT '🔍 DPDA: Count by date range (should use NCCI_DPDA_Analytics)'
SELECT
    YEAR(NGAY_DL) as Year,
    MONTH(NGAY_DL) as Month,
    COUNT(*) as RecordCount,
    COUNT(DISTINCT MA_CHI_NHANH) as BranchCount,
    COUNT(DISTINCT MA_KHACH_HANG) as CustomerCount
FROM DPDA
WHERE NGAY_DL >= '2024-01-01' AND NGAY_DL <= '2025-12-31'
GROUP BY YEAR(NGAY_DL), MONTH(NGAY_DL)
ORDER BY Year, Month;

-- GL01 Analytics Query
PRINT '🔍 GL01: Aggregation query (should use CCI_GL01_Partitioned)'
SELECT
    YEAR(NGAY_DL) as Year,
    COUNT(*) as RecordCount,
    -- Add more analytics as data grows
    MIN(NGAY_DL) as MinDate,
    MAX(NGAY_DL) as MaxDate
FROM GL01
WHERE NGAY_DL >= '2024-01-01'
GROUP BY YEAR(NGAY_DL)
ORDER BY Year;

-- LN01 Analytics Query
PRINT '🔍 LN01: Balance aggregation (should use NCCI_LN01_Analytics)'
SELECT
    YEAR(NGAY_DL) as Year,
    COUNT(*) as RecordCount,
    SUM(CASE WHEN DU_NO IS NOT NULL THEN CAST(DU_NO AS BIGINT) ELSE 0 END) as TotalBalance,
    AVG(CASE WHEN SO_TIEN_GIAI_NGAN_1 IS NOT NULL THEN CAST(SO_TIEN_GIAI_NGAN_1 AS BIGINT) ELSE 0 END) as AvgAmount1
FROM LN01
WHERE NGAY_DL >= '2024-01-01'
GROUP BY YEAR(NGAY_DL)
ORDER BY Year;

-- LN03 Analytics Query
PRINT '🔍 LN03: Branch-wise analysis (should use NCCI_LN03_Analytics)'
SELECT
    MACHINHANH,
    COUNT(*) as RecordCount,
    SUM(CASE WHEN DUNONOIBANG IS NOT NULL THEN CAST(DUNONOIBANG AS BIGINT) ELSE 0 END) as TotalDebt
FROM LN03
WHERE NGAY_DL >= '2024-01-01'
GROUP BY MACHINHANH
ORDER BY RecordCount DESC;

-- RR01 Analytics Query
PRINT '🔍 RR01: Risk analysis (should use NCCI_RR01_Analytics)'
SELECT
    YEAR(NGAY_DL) as Year,
    COUNT(*) as RecordCount,
    SUM(CASE WHEN DUNO_NGAN_HAN IS NOT NULL THEN CAST(DUNO_NGAN_HAN AS BIGINT) ELSE 0 END) as ShortTermDebt,
    SUM(CASE WHEN DUNO_DAI_HAN IS NOT NULL THEN CAST(DUNO_DAI_HAN AS BIGINT) ELSE 0 END) as LongTermDebt
FROM RR01
WHERE NGAY_DL >= '2024-01-01'
GROUP BY YEAR(NGAY_DL)
ORDER BY Year;

PRINT '=================================================='
PRINT '✅ Performance tests completed!'
PRINT '💡 Check execution times and I/O statistics above'
PRINT '📈 Columnstore indexes should show improved performance for analytics queries'

-- Disable statistics
SET STATISTICS TIME OFF;
SET STATISTICS IO OFF;

-- Show index usage stats
PRINT '📊 Index Usage Statistics:'
SELECT
    OBJECT_NAME(i.object_id) AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType,
    s.user_seeks,
    s.user_scans,
    s.user_lookups,
    s.user_updates,
    s.last_user_seek,
    s.last_user_scan
FROM sys.indexes i
LEFT JOIN sys.dm_db_index_usage_stats s
    ON i.object_id = s.object_id AND i.index_id = s.index_id
WHERE i.type IN (5, 6) -- Columnstore indexes only
    AND OBJECT_NAME(i.object_id) IN ('DPDA', 'GL01', 'LN01', 'LN03', 'RR01')
ORDER BY TableName, IndexName;
