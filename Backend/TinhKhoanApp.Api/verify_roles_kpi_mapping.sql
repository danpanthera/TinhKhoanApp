-- Mapping giữa 23 vai trò và 23 bảng KPI cán bộ
-- Date: 2025-06-18

-- Kiểm tra mapping 1:1 giữa Roles và KpiAssignmentTables (cho cán bộ)
SELECT 
    r.id as RoleId,
    r.name as RoleName,
    k.Id as KpiTableId,
    k.TableName as KpiTableName,
    k.Category
FROM Roles r
LEFT JOIN KpiAssignmentTables k ON r.id = k.Id AND k.Category = 'Dành cho Cán bộ'
WHERE r.id <= 23
ORDER BY r.id;

-- Thống kê
SELECT 
    'Roles Created' as Type, 
    COUNT(*) as Count 
FROM Roles
UNION ALL
SELECT 
    'KPI Tables (Cán bộ)' as Type, 
    COUNT(*) as Count 
FROM KpiAssignmentTables 
WHERE Category = 'Dành cho Cán bộ'
UNION ALL
SELECT 
    'KPI Tables (Chi nhánh)' as Type, 
    COUNT(*) as Count 
FROM KpiAssignmentTables 
WHERE Category = 'Dành cho Chi nhánh'
UNION ALL
SELECT 
    'KPI Indicators (After Delete)' as Type, 
    COUNT(*) as Count 
FROM KpiIndicators;
