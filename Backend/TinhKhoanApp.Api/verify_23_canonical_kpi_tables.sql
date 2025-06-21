-- Danh sách 23 roleCode chuẩn từ SeedKPIDefinitionMaxScore.cs
-- 1. TruongphongKhdn
-- 2. TruongphongKhcn  
-- 3. PhophongKhdn
-- 4. PhophongKhcn
-- 5. TruongphongKhqlrr
-- 6. PhophongKhqlrr
-- 7. Cbtd
-- 8. TruongphongKtnqCnl1
-- 9. PhophongKtnqCnl1
-- 10. Gdv
-- 11. TqHkKtnb
-- 12. TruongphongItThKtgs
-- 13. CbItThKtgsKhqlrr
-- 14. GiamdocPgd
-- 15. PhogiamdocPgd
-- 16. PhogiamdocPgdCbtd
-- 17. GiamdocCnl2
-- 18. PhogiamdocCnl2Td
-- 19. PhogiamdocCnl2Kt
-- 20. TruongphongKhCnl2
-- 21. PhophongKhCnl2
-- 22. TruongphongKtnqCnl2
-- 23. PhophongKtnqCnl2
-- SQL Server compatible version

-- Kiểm tra xem các bảng KPI assignment có khớp với 23 roleCode chuẩn không
SELECT 
    'Checking KPI Assignment Tables' as Check_Type,
    COUNT(*) as Total_Tables
FROM sys.tables 
WHERE name LIKE '%_KPI_Assignment';

-- Liệt kê tất cả các bảng KPI assignment hiện có
SELECT 
    'Current KPI Assignment Tables' as List_Type,
    name as Table_Name
FROM sys.tables 
WHERE name LIKE '%_KPI_Assignment'
ORDER BY name;

-- Kiểm tra xem có thiếu bảng KPI nào không bằng cách so với danh sách 23 roleCode chuẩn
WITH canonical_roles AS (
    SELECT 'TruongphongKhdn_KPI_Assignment' as expected_table
    UNION SELECT 'TruongphongKhcn_KPI_Assignment'
    UNION SELECT 'PhophongKhdn_KPI_Assignment'
    UNION SELECT 'PhophongKhcn_KPI_Assignment'
    UNION SELECT 'TruongphongKhqlrr_KPI_Assignment'
    UNION SELECT 'PhophongKhqlrr_KPI_Assignment'
    UNION SELECT 'Cbtd_KPI_Assignment'
    UNION SELECT 'TruongphongKtnqCnl1_KPI_Assignment'
    UNION SELECT 'PhophongKtnqCnl1_KPI_Assignment'
    UNION SELECT 'Gdv_KPI_Assignment'
    UNION SELECT 'TqHkKtnb_KPI_Assignment'
    UNION SELECT 'TruongphongItThKtgs_KPI_Assignment'
    UNION SELECT 'CbItThKtgsKhqlrr_KPI_Assignment'
    UNION SELECT 'GiamdocPgd_KPI_Assignment'
    UNION SELECT 'PhogiamdocPgd_KPI_Assignment'
    UNION SELECT 'PhogiamdocPgdCbtd_KPI_Assignment'
    UNION SELECT 'GiamdocCnl2_KPI_Assignment'
    UNION SELECT 'PhogiamdocCnl2Td_KPI_Assignment'
    UNION SELECT 'PhogiamdocCnl2Kt_KPI_Assignment'
    UNION SELECT 'TruongphongKhCnl2_KPI_Assignment'
    UNION SELECT 'PhophongKhCnl2_KPI_Assignment'
    UNION SELECT 'TruongphongKtnqCnl2_KPI_Assignment'
    UNION SELECT 'PhophongKtnqCnl2_KPI_Assignment'
),
existing_tables AS (
    SELECT name as table_name
    FROM sys.tables 
    WHERE name LIKE '%_KPI_Assignment'
)
SELECT 
    'Missing Tables' as Status,
    cr.expected_table as Missing_Table
FROM canonical_roles cr
LEFT JOIN existing_tables et ON cr.expected_table = et.table_name
WHERE et.table_name IS NULL;

-- Kiểm tra cấu trúc của các bảng KPI assignment
SELECT 
    'Table Structure Check' as Status,
    t.name AS TableName,
    c.name AS ColumnName,
    ty.name AS DataType,
    c.max_length,
    c.is_nullable
FROM sys.tables t
INNER JOIN sys.columns c ON t.object_id = c.object_id
INNER JOIN sys.types ty ON c.system_type_id = ty.system_type_id
WHERE t.name LIKE '%_KPI_Assignment'
ORDER BY t.name, c.column_id;

-- Đếm số lượng records trong mỗi bảng KPI assignment
DECLARE @sql NVARCHAR(MAX) = '';
SELECT @sql = @sql + 'SELECT ''' + name + ''' as TableName, COUNT(*) as RecordCount FROM [' + name + '] UNION ALL '
FROM sys.tables
WHERE name LIKE '%_KPI_Assignment';

-- Remove the last UNION ALL
IF LEN(@sql) > 0
    SET @sql = LEFT(@sql, LEN(@sql) - 11);

IF LEN(@sql) > 0
    EXEC sp_executesql @sql;

-- Tóm tắt kiểm tra
SELECT 
    'Summary' as Status,
    (SELECT COUNT(*) FROM sys.tables WHERE name LIKE '%_KPI_Assignment') as Total_KPI_Tables,
    23 as Expected_Tables,
    CASE 
        WHEN (SELECT COUNT(*) FROM sys.tables WHERE name LIKE '%_KPI_Assignment') = 23 
        THEN 'COMPLETE' 
        ELSE 'MISSING TABLES' 
    END as Status_Check;
