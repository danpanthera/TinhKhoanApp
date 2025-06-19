-- Xóa hết các chỉ tiêu KPI hiện tại từ tất cả bảng
-- Date: 2025-06-18

-- Backup dữ liệu trước khi xóa (optional)
CREATE TABLE IF NOT EXISTS KpiIndicators_Backup_20250618 AS 
SELECT * FROM KpiIndicators;

-- Xóa hết các chỉ tiêu từ tất cả 33 bảng KPI
DELETE FROM KpiIndicators;

-- Reset auto-increment
DELETE FROM sqlite_sequence WHERE name='KpiIndicators';

-- Kiểm tra kết quả
SELECT COUNT(*) as 'Total Indicators After Delete' FROM KpiIndicators;
SELECT COUNT(*) as 'Backup Records' FROM KpiIndicators_Backup_20250618;

-- Thống kê
SELECT 'BEFORE DELETE' as Status, COUNT(*) as Count FROM KpiIndicators_Backup_20250618
UNION ALL
SELECT 'AFTER DELETE' as Status, COUNT(*) as Count FROM KpiIndicators;
