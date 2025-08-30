-- 🔍 SCRIPT KIỂM TRA COLUMNS TRƯỚC KHI CHUYỂN ĐỔI
-- Ngày: 22/06/2025

USE TinhKhoanDB;
GO

PRINT '🔍 KIỂM TRA CỘT TEMPORAL ĐÃ TỒN TẠI'
PRINT '===================================='

-- Kiểm tra cột ValidFrom và ValidTo trong từng bảng
SELECT 
    t.name AS TableName,
    c.name AS ColumnName,
    ty.name AS DataType,
    c.is_hidden AS IsHidden,
    c.generated_always_type_desc AS GeneratedType
FROM sys.tables t
INNER JOIN sys.columns c ON t.object_id = c.object_id
INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
WHERE t.name IN ('Units', 'Positions', 'Roles', 'Employees', 'KPIDefinitions', 'KpiScoringRules', 'SalaryParameters', 'FinalPayouts', 'KhoanPeriods')
    AND c.name IN ('ValidFrom', 'ValidTo')
ORDER BY t.name, c.name;

-- Kiểm tra period columns
PRINT ''
PRINT 'PERIOD COLUMNS:'
SELECT 
    t.name AS TableName,
    p.start_column_id,
    p.end_column_id,
    cs.name AS StartColumn,
    ce.name AS EndColumn
FROM sys.tables t
INNER JOIN sys.periods p ON t.object_id = p.object_id
INNER JOIN sys.columns cs ON p.object_id = cs.object_id AND p.start_column_id = cs.column_id
INNER JOIN sys.columns ce ON p.object_id = ce.object_id AND p.end_column_id = ce.column_id
WHERE t.name IN ('Units', 'Positions', 'Roles', 'Employees', 'KPIDefinitions', 'KpiScoringRules', 'SalaryParameters', 'FinalPayouts', 'KhoanPeriods');

GO
