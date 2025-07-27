-- ================================================
-- Script sửa Category của 32 bảng KPI để match với frontend
-- Từ 'Staff'/'Branch' thành 'CANBO'/'CHINHANH'
-- ================================================

USE TinhKhoanDB;
GO

PRINT '🔧 Đang sửa Category của KPI Tables để match với frontend...';

-- Sửa Category cho 23 bảng cán bộ (TableType 1-23)
UPDATE KpiAssignmentTables
SET Category = 'CANBO'
WHERE TableType BETWEEN 1 AND 23;

-- Sửa Category cho 9 bảng chi nhánh (TableType 200-208)
UPDATE KpiAssignmentTables
SET Category = 'CHINHANH'
WHERE TableType BETWEEN 200 AND 208;

PRINT '✅ Hoàn thành sửa Category!';

-- Kiểm tra kết quả
PRINT '📊 Kết quả sau khi sửa:';
SELECT
    Category,
    COUNT(*) as Count
FROM KpiAssignmentTables
GROUP BY Category
ORDER BY Count DESC;

PRINT '📋 Chi tiết phân tab:';
SELECT
    CASE
        WHEN Category = 'CANBO' THEN 'Tab "Dành cho cán bộ"'
        WHEN Category = 'CHINHANH' THEN 'Tab "Dành cho chi nhánh"'
        ELSE 'Khác'
    END as TabName,
    COUNT(*) as Count
FROM KpiAssignmentTables
GROUP BY Category
ORDER BY
    CASE Category
        WHEN 'CANBO' THEN 1
        WHEN 'CHINHANH' THEN 2
        ELSE 3
    END;
