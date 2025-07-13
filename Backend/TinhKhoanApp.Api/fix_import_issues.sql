-- =====================================================
-- FIX IMPORT ISSUES - JULY 13, 2025
-- Khắc phục vấn đề import CSV và cột tổng records
-- =====================================================

-- 1. KIỂM TRA THỰC TRẠNG CÁC BẢNG
PRINT '=== 1. KIỂM TRA THỰC TRẠNG CÁC BẢNG ===';

SELECT
    t.name as TableName,
    COUNT(c.COLUMN_NAME) as BusinessColumns,
    ISNULL(ps.row_count, 0) as RecordCount
FROM sys.tables t
LEFT JOIN sys.dm_db_partition_stats ps ON t.object_id = ps.object_id AND ps.index_id IN (0,1)
LEFT JOIN INFORMATION_SCHEMA.COLUMNS c ON t.name = c.TABLE_NAME
    AND c.COLUMN_NAME NOT IN ('Id', 'NGAY_DL', 'CREATED_DATE', 'UPDATED_DATE', 'ValidFrom', 'ValidTo')
WHERE t.name IN ('DP01', 'DP01_New', 'LN01', 'LN03', 'GL01', 'GL41', 'DPDA', 'EI01', 'RR01')
GROUP BY t.name, ps.row_count
ORDER BY t.name;

-- 2. KIỂM TRA MAPPING TÊN BẢNG
PRINT '=== 2. MAPPING TÊN BẢNG THỰC TẾ ===';

SELECT
    'DP01' as ExpectedTable,
    CASE WHEN EXISTS(SELECT 1 FROM sys.tables WHERE name = 'DP01') THEN 'DP01'
         WHEN EXISTS(SELECT 1 FROM sys.tables WHERE name = 'DP01_New') THEN 'DP01_New'
         ELSE 'NOT_FOUND' END as ActualTable
UNION ALL
SELECT 'LN01', CASE WHEN EXISTS(SELECT 1 FROM sys.tables WHERE name = 'LN01') THEN 'LN01' ELSE 'NOT_FOUND' END
UNION ALL
SELECT 'LN03', CASE WHEN EXISTS(SELECT 1 FROM sys.tables WHERE name = 'LN03') THEN 'LN03' ELSE 'NOT_FOUND' END
UNION ALL
SELECT 'GL01', CASE WHEN EXISTS(SELECT 1 FROM sys.tables WHERE name = 'GL01') THEN 'GL01' ELSE 'NOT_FOUND' END
UNION ALL
SELECT 'GL41', CASE WHEN EXISTS(SELECT 1 FROM sys.tables WHERE name = 'GL41') THEN 'GL41' ELSE 'NOT_FOUND' END
UNION ALL
SELECT 'DPDA', CASE WHEN EXISTS(SELECT 1 FROM sys.tables WHERE name = 'DPDA') THEN 'DPDA' ELSE 'NOT_FOUND' END
UNION ALL
SELECT 'EI01', CASE WHEN EXISTS(SELECT 1 FROM sys.tables WHERE name = 'EI01') THEN 'EI01' ELSE 'NOT_FOUND' END
UNION ALL
SELECT 'RR01', CASE WHEN EXISTS(SELECT 1 FROM sys.tables WHERE name = 'RR01') THEN 'RR01' ELSE 'NOT_FOUND' END;

-- 3. KIỂM TRA RAWDATAITEMS VÀ TOTALRECORDS
PRINT '=== 3. KIỂM TRA RAWDATAITEMS ===';

SELECT
    DataType,
    COUNT(*) as FileCount,
    SUM(TotalRecords) as TotalRecordsSum,
    AVG(TotalRecords) as AvgRecords
FROM RawDataItems
WHERE DataType IN ('DP01', 'LN01', 'LN03', 'GL01', 'GL41', 'DPDA', 'EI01', 'RR01')
GROUP BY DataType
ORDER BY DataType;

-- 4. KIỂM TRA DỮ LIỆU MẪU
PRINT '=== 4. DỮ LIỆU MẪU DP01_New ===';

SELECT TOP 3
    Id, NGAY_DL, MA_CN, CREATED_DATE,
    'DP01_New Sample' as TableInfo
FROM DP01_New
ORDER BY CREATED_DATE DESC;

PRINT '=== 5. TỔNG KẾT VẤN ĐỀ ===';
PRINT 'Issue 1: DP01 table mapping -> DP01_New';
PRINT 'Issue 2: Column counts mismatch expected values';
PRINT 'Issue 3: TotalRecords in RawDataItems not updated';
PRINT 'Issue 4: Need to verify data integrity after import';
