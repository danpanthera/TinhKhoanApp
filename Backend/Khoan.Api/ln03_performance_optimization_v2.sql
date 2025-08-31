-- ========================================
-- 🚀 LN03 PERFORMANCE OPTIMIZATION SCRIPT (REVISED)  
-- ========================================
-- Tối ưu hiệu suất cho bảng LN03 với Nonclustered Columnstore Indexes

USE TinhKhoanDB;
GO

PRINT '🚀 BẮT ĐẦU PERFORMANCE OPTIMIZATION CHO LN03...';

-- ========================================
-- 1. TẠO NONCLUSTERED COLUMNSTORE INDEX CHO ANALYTICS
-- ========================================
PRINT '📊 Đang tạo Nonclustered Columnstore Index cho analytics...';

-- Tạo Nonclustered Columnstore Index cho analytics (không thay thế clustered index)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'NCCI_LN03_Analytics')
BEGIN
    CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_LN03_Analytics 
    ON LN03 (
        NGAY_DL, MACHINHANH, MAKH, SOTIENXLRR, 
        DUNONOIBANG, CONLAINGOAIBANG, NHOMNO
    );
    PRINT '✅ Nonclustered Columnstore Index NCCI_LN03_Analytics đã được tạo';
END
ELSE
BEGIN
    PRINT '⚠️ Nonclustered Columnstore Index đã tồn tại';
END

-- ========================================
-- 2. TẠO CÁC INDEXES THƯỜNG DÙNG (chỉ tạo nếu chưa có)
-- ========================================
PRINT '📋 Đang kiểm tra và tạo các Indexes thường dùng...';

-- Index cho NGAY_DL (truy vấn theo ngày)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LN03_NGAY_DL_OPTIMIZED')
BEGIN
    CREATE NONCLUSTERED INDEX IX_LN03_NGAY_DL_OPTIMIZED 
    ON LN03 (NGAY_DL DESC)
    INCLUDE (MACHINHANH, MAKH, SOTIENXLRR, DUNONOIBANG);
    PRINT '✅ Index IX_LN03_NGAY_DL_OPTIMIZED đã được tạo';
END
ELSE
    PRINT '⚠️ Index IX_LN03_NGAY_DL_OPTIMIZED đã tồn tại';

-- Index cho MACHINHANH (báo cáo theo chi nhánh)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LN03_MACHINHANH_OPTIMIZED')
BEGIN
    CREATE NONCLUSTERED INDEX IX_LN03_MACHINHANH_OPTIMIZED 
    ON LN03 (MACHINHANH)
    INCLUDE (NGAY_DL, SOTIENXLRR, DUNONOIBANG, NHOMNO);
    PRINT '✅ Index IX_LN03_MACHINHANH_OPTIMIZED đã được tạo';
END
ELSE
    PRINT '⚠️ Index IX_LN03_MACHINHANH_OPTIMIZED đã tồn tại';

-- ========================================
-- 3. KIỂM TRA HIỆU SUẤT
-- ========================================
PRINT '🔍 Đang kiểm tra hiệu suất...';

-- Test analytics query với columnstore
DECLARE @StartTime DATETIME2 = SYSDATETIME();

SELECT 
    MACHINHANH,
    COUNT(*) as TotalRecords,
    COUNT(DISTINCT MAKH) as UniqueCustomers
FROM LN03 
WHERE NGAY_DL >= '2024-01-01'
  AND IS_DELETED = 0
GROUP BY MACHINHANH
ORDER BY TotalRecords DESC;

DECLARE @EndTime DATETIME2 = SYSDATETIME();
DECLARE @Duration INT = DATEDIFF(MILLISECOND, @StartTime, @EndTime);

PRINT '✅ Analytics query hoàn thành trong ' + CAST(@Duration AS VARCHAR) + 'ms';

-- ========================================
-- 4. HIỂN THỊ THÔNG TIN INDEXES  
-- ========================================
PRINT '';
PRINT '📊 DANH SÁCH INDEXES TRÊN BẢNG LN03:';

SELECT 
    i.name AS IndexName,
    i.type_desc AS IndexType,
    CASE WHEN i.is_unique = 1 THEN 'UNIQUE' ELSE 'NON-UNIQUE' END AS Uniqueness
FROM sys.indexes i
JOIN sys.objects o ON i.object_id = o.object_id
WHERE o.name = 'LN03'
  AND i.name IS NOT NULL
ORDER BY i.type, i.name;

-- ========================================
-- 5. PERFORMANCE OPTIMIZATION COMPLETE
-- ========================================
PRINT '';
PRINT '🎉 PERFORMANCE OPTIMIZATION HOÀN THÀNH!';
PRINT '';
PRINT '📊 KẾT QUẢ:';
PRINT '   ✅ Nonclustered Columnstore Index: Tối ưu analytics';
PRINT '   ✅ Nonclustered Indexes: Tối ưu OLTP operations'; 
PRINT '   ✅ Performance Test: ' + CAST(@Duration AS VARCHAR) + 'ms';
PRINT '';
PRINT '🚀 LN03 SẴN SÀNG CHO SẢN XUẤT!';

GO
