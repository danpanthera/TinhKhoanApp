-- ========================================
-- 🚀 LN03 PERFORMANCE OPTIMIZATION SCRIPT
-- ========================================
-- Tối ưu hiệu suất cho bảng LN03 với Columnstore Indexes và Analytics

USE TinhKhoanDB;
GO

PRINT '🚀 BẮT ĐẦU PERFORMANCE OPTIMIZATION CHO LN03...';

-- ========================================
-- 1. TẠO COLUMNSTORE INDEX CHO ANALYTICS
-- ========================================
PRINT '📊 Đang tạo Columnstore Index cho analytics...';

-- Tạo Columnstore Index cho các truy vấn analytics
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'CCI_LN03_Analytics')
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_LN03_Analytics 
    ON LN03
    WITH (
        MAXDOP = 4,
        COMPRESSION_DELAY = 0
    );
    PRINT '✅ Columnstore Index CCI_LN03_Analytics đã được tạo';
END
ELSE
BEGIN
    PRINT '⚠️ Columnstore Index đã tồn tại';
END

-- ========================================
-- 2. TẠO CÁC INDEXES THƯỜNG DÙNG
-- ========================================
PRINT '📋 Đang tạo các Indexes thường dùng...';

-- Index cho NGAY_DL (truy vấn theo ngày)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LN03_NGAY_DL_OPTIMIZED')
BEGIN
    CREATE NONCLUSTERED INDEX IX_LN03_NGAY_DL_OPTIMIZED 
    ON LN03 (NGAY_DL DESC)
    INCLUDE (MACHINHANH, MAKH, SOTIENXLRR, DUNONOIBANG);
    PRINT '✅ Index IX_LN03_NGAY_DL_OPTIMIZED đã được tạo';
END

-- Index cho MACHINHANH (báo cáo theo chi nhánh)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LN03_MACHINHANH_OPTIMIZED')
BEGIN
    CREATE NONCLUSTERED INDEX IX_LN03_MACHINHANH_OPTIMIZED 
    ON LN03 (MACHINHANH)
    INCLUDE (NGAY_DL, SOTIENXLRR, DUNONOIBANG, NHOMNO);
    PRINT '✅ Index IX_LN03_MACHINHANH_OPTIMIZED đã được tạo';
END

-- Index cho NHOMNO (phân tích theo nhóm nợ)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LN03_NHOMNO_OPTIMIZED')
BEGIN
    CREATE NONCLUSTERED INDEX IX_LN03_NHOMNO_OPTIMIZED 
    ON LN03 (NHOMNO)
    INCLUDE (NGAY_DL, MACHINHANH, SOTIENXLRR, DUNONOIBANG);
    PRINT '✅ Index IX_LN03_NHOMNO_OPTIMIZED đã được tạo';
END

-- Composite Index cho các truy vấn phức tạp
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LN03_COMPOSITE_ANALYTICS')
BEGIN
    CREATE NONCLUSTERED INDEX IX_LN03_COMPOSITE_ANALYTICS 
    ON LN03 (NGAY_DL, MACHINHANH, NHOMNO)
    INCLUDE (MAKH, SOTIENXLRR, DUNONOIBANG, CONLAINGOAIBANG);
    PRINT '✅ Index IX_LN03_COMPOSITE_ANALYTICS đã được tạo';
END

-- ========================================
-- 3. TẠO STATISTICS CHO CBO
-- ========================================
PRINT '📈 Đang tạo Statistics cho Cost-Based Optimizer...';

-- Statistics cho các cột quan trọng
CREATE STATISTICS STAT_LN03_AMOUNTS 
ON LN03 (SOTIENXLRR, DUNONOIBANG, CONLAINGOAIBANG);

CREATE STATISTICS STAT_LN03_DATE_BRANCH 
ON LN03 (NGAY_DL, MACHINHANH);

PRINT '✅ Statistics đã được tạo';

-- ========================================
-- 4. KIỂM TRA HIỆU SUẤT
-- ========================================
PRINT '🔍 Đang kiểm tra hiệu suất...';

-- Test query performance
DECLARE @StartTime DATETIME2 = SYSDATETIME();

SELECT 
    MACHINHANH,
    COUNT(*) as TotalRecords,
    SUM(CAST(ISNULL(SOTIENXLRR, 0) AS BIGINT)) as TotalSOTIENXLRR,
    AVG(CAST(ISNULL(DUNONOIBANG, 0) AS BIGINT)) as AvgDUNONOIBANG
FROM LN03 
WHERE NGAY_DL >= '2024-01-01'
GROUP BY MACHINHANH
ORDER BY TotalRecords DESC;

DECLARE @EndTime DATETIME2 = SYSDATETIME();
DECLARE @Duration INT = DATEDIFF(MILLISECOND, @StartTime, @EndTime);

PRINT '✅ Test query hoàn thành trong ' + CAST(@Duration AS VARCHAR) + 'ms';

-- ========================================
-- 5. HIỂN THỊ THÔNG TIN INDEXES
-- ========================================
PRINT '📊 DANH SÁCH INDEXES TRÊN BẢNG LN03:';

SELECT 
    i.name AS IndexName,
    i.type_desc AS IndexType,
    CASE WHEN i.is_unique = 1 THEN 'UNIQUE' ELSE 'NON-UNIQUE' END AS Uniqueness,
    ds.name AS DataSpace
FROM sys.indexes i
JOIN sys.objects o ON i.object_id = o.object_id
LEFT JOIN sys.data_spaces ds ON i.data_space_id = ds.data_space_id
WHERE o.name = 'LN03'
ORDER BY i.name;

-- ========================================
-- 6. OPTIMIZATION COMPLETE
-- ========================================
PRINT '🎉 PERFORMANCE OPTIMIZATION HOÀN THÀNH!';
PRINT '';
PRINT '📊 KẾT QUẢ:';
PRINT '   ✅ Columnstore Index: Tối ưu cho analytics queries';
PRINT '   ✅ Nonclustered Indexes: Tối ưu cho OLTP queries'; 
PRINT '   ✅ Statistics: Tối ưu cho Query Optimizer';
PRINT '   ✅ Performance Test: Hoàn thành';
PRINT '';
PRINT '🚀 LN03 TEMPORAL TABLE READY FOR PRODUCTION!';

GO
