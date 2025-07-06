-- BACKUP KPI ASSIGNMENT TABLES AND DEFINITIONS BEFORE RESET
-- Thực hiện lúc: 2025-07-06
-- Mục đích: Backup toàn bộ dữ liệu KPI trước khi xóa và tạo lại 23 bảng KPI cán bộ

-- Backup KpiAssignmentTables (Employee KPI only)
SELECT 'KpiAssignmentTables - Employee KPI' as TableType;
SELECT * FROM KpiAssignmentTables WHERE Category = 'CANBO';

-- Backup KpiDefinitions hiện tại
SELECT 'KpiDefinitions - All' as TableType;
SELECT * FROM KpiDefinitions ORDER BY TableName, Name;

-- Backup KpiIndicators (thuộc các bảng KPI cán bộ)
SELECT 'KpiIndicators - Employee KPI related' as TableType;
SELECT ki.*
FROM KpiIndicators ki
INNER JOIN KpiAssignmentTables kat ON ki.TableName = kat.TableName
WHERE kat.Category = 'CANBO'
ORDER BY ki.TableName, ki.Name;

-- Thống kê trước khi xóa
SELECT 'Statistics Before Reset' as Info;
SELECT
    'KpiAssignmentTables (CANBO)' as Entity,
    COUNT(*) as Count
FROM KpiAssignmentTables WHERE Category = 'CANBO'
UNION ALL
SELECT
    'KpiDefinitions' as Entity,
    COUNT(*) as Count
FROM KpiDefinitions
UNION ALL
SELECT
    'KpiIndicators (CANBO tables)' as Entity,
    COUNT(*) as Count
FROM KpiIndicators ki
INNER JOIN KpiAssignmentTables kat ON ki.TableName = kat.TableName
WHERE kat.Category = 'CANBO';
