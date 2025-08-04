-- Reset toàn bộ KpiIndicators cho 23 bảng KPI cán bộ
USE TinhKhoanDB;

-- Backup count trước khi xóa
SELECT 'Tổng chỉ tiêu trước khi xóa:' as Status, COUNT(*) as Count
FROM KpiIndicators ki
JOIN KpiAssignmentTables kat ON ki.TableId = kat.Id
WHERE kat.Category = 'CANBO';

-- Xóa tất cả KpiIndicators của 23 bảng KPI cán bộ
DELETE ki
FROM KpiIndicators ki
JOIN KpiAssignmentTables kat ON ki.TableId = kat.Id
WHERE kat.Category = 'CANBO';

-- Reset IDENTITY seed về 1
DBCC CHECKIDENT ('KpiIndicators', RESEED, 0);

-- Verify kết quả
SELECT 'Tổng chỉ tiêu sau khi xóa:' as Status, COUNT(*) as Count
FROM KpiIndicators ki
JOIN KpiAssignmentTables kat ON ki.TableId = kat.Id
WHERE kat.Category = 'CANBO';

PRINT 'Đã xóa toàn bộ KpiIndicators cho 23 bảng KPI cán bộ!';
