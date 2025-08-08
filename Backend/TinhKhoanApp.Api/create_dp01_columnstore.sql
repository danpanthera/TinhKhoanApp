-- Script tạo Columnstore Index cho bảng DP01 và DP01_History (nếu chưa có)
-- Tác giả: GitHub Copilot
-- Ngày tạo: 08/08/2025

-- Kiểm tra xem đã có COLUMNSTORE INDEX chưa và tạo nếu cần
IF NOT EXISTS (
    SELECT *
    FROM sys.indexes
    WHERE name LIKE '%columnstore%'
        AND object_id = OBJECT_ID('dbo.DP01')
)
BEGIN
    PRINT '🔧 Tạo NONCLUSTERED COLUMNSTORE INDEX cho bảng DP01...'
    CREATE NONCLUSTERED COLUMNSTORE INDEX IX_DP01_Columnstore
    ON dbo.DP01;
    PRINT '✅ Đã tạo NONCLUSTERED COLUMNSTORE INDEX cho bảng DP01 thành công!'
END
ELSE
BEGIN
    PRINT '✅ Bảng DP01 đã có COLUMNSTORE INDEX!'
END

-- Kiểm tra xem đã có COLUMNSTORE INDEX cho History table chưa và tạo nếu cần
IF NOT EXISTS (
    SELECT *
    FROM sys.indexes
    WHERE name LIKE '%columnstore%'
        AND object_id = OBJECT_ID('dbo.DP01_History')
)
BEGIN
    PRINT '🔧 Tạo CLUSTERED COLUMNSTORE INDEX cho bảng DP01_History...'
    CREATE CLUSTERED COLUMNSTORE INDEX IX_DP01_History_Columnstore
    ON dbo.DP01_History;
    PRINT '✅ Đã tạo CLUSTERED COLUMNSTORE INDEX cho bảng DP01_History thành công!'
END
ELSE
BEGIN
    PRINT '✅ Bảng DP01_History đã có COLUMNSTORE INDEX!'
END

-- Thực hiện UPDATE STATISTICS sau khi tạo index để đảm bảo tối ưu hiệu suất
PRINT '📊 Cập nhật statistics cho DP01...'
UPDATE STATISTICS dbo.DP01;
PRINT '📊 Cập nhật statistics cho DP01_History...'
UPDATE STATISTICS dbo.DP01_History;
PRINT '✅ Đã cập nhật statistics thành công!'

-- Kiểm tra lại sau khi tạo
SELECT
    t.name AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType,
    'OK' AS Status
FROM sys.indexes i
JOIN sys.tables t ON i.object_id = t.object_id
WHERE t.name IN ('DP01', 'DP01_History')
    AND i.name LIKE '%columnstore%';

PRINT '✅ Quá trình hoàn tất!'
