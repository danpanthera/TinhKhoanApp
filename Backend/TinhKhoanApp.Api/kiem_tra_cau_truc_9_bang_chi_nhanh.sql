-- Kiểm tra cấu trúc 9 bảng chi nhánh so với bảng reference
-- SQL Server compatible version

-- 1. Đếm số bảng KPI chi nhánh
SELECT 
    'Branch KPI Tables Count' as Check_Type,
    COUNT(*) as Total_Tables
FROM sys.tables 
WHERE name LIKE 'KPI_CN_%' OR name = 'KPI_HOI_SO';

-- 2. Liệt kê tất cả bảng KPI chi nhánh
SELECT 
    'Branch Tables List' as List_Type,
    name as Table_Name
FROM sys.tables 
WHERE name LIKE 'KPI_CN_%' OR name = 'KPI_HOI_SO'
ORDER BY name;

-- 3. So sánh số lượng cột của mỗi bảng với bảng reference (GiamdocCnl2_KPI_Assignment)
WITH table_column_counts AS (
    SELECT 
        t.name AS TableName,
        COUNT(c.name) AS ColumnCount
    FROM sys.tables t
    INNER JOIN sys.columns c ON t.object_id = c.object_id
    WHERE t.name IN ('GiamdocCnl2_KPI_Assignment', 'KPI_CN_MUONG_TE', 'KPI_CN_NAM_NHUN', 
                     'KPI_CN_PHONG_THO', 'KPI_CN_SIN_HO', 'KPI_CN_TAM_DUONG', 
                     'KPI_CN_TAN_UYEN', 'KPI_CN_THANH_PHO', 'KPI_CN_THAN_UYEN', 'KPI_HOI_SO')
    GROUP BY t.name
)
SELECT 
    TableName,
    ColumnCount,
    CASE 
        WHEN ColumnCount = (SELECT ColumnCount FROM table_column_counts WHERE TableName = 'GiamdocCnl2_KPI_Assignment')
        THEN 'MATCH'
        ELSE 'DIFFERENT'
    END as Structure_Status
FROM table_column_counts
ORDER BY TableName;

-- 4. Chi tiết cấu trúc bảng reference
SELECT 
    'GiamdocCnl2_KPI_Assignment Structure' as Table_Info,
    c.name AS ColumnName,
    ty.name AS DataType,
    c.max_length,
    c.is_nullable,
    c.column_id as Position
FROM sys.tables t
INNER JOIN sys.columns c ON t.object_id = c.object_id
INNER JOIN sys.types ty ON c.system_type_id = ty.system_type_id
WHERE t.name = 'GiamdocCnl2_KPI_Assignment'
ORDER BY c.column_id;

-- 5. Chi tiết cấu trúc một bảng chi nhánh (ví dụ KPI_CN_MUONG_TE)
SELECT 
    'KPI_CN_MUONG_TE Structure' as Table_Info,
    c.name AS ColumnName,
    ty.name AS DataType,
    c.max_length,
    c.is_nullable,
    c.column_id as Position
FROM sys.tables t
INNER JOIN sys.columns c ON t.object_id = c.object_id
INNER JOIN sys.types ty ON c.system_type_id = ty.system_type_id
WHERE t.name = 'KPI_CN_MUONG_TE'
ORDER BY c.column_id;

-- 6. Chi tiết cấu trúc bảng Hội sở
SELECT 
    'KPI_HOI_SO Structure' as Table_Info,
    c.name AS ColumnName,
    ty.name AS DataType,
    c.max_length,
    c.is_nullable,
    c.column_id as Position
FROM sys.tables t
INNER JOIN sys.columns c ON t.object_id = c.object_id
INNER JOIN sys.types ty ON c.system_type_id = ty.system_type_id
WHERE t.name = 'KPI_HOI_SO'
ORDER BY c.column_id;

-- 7. Kiểm tra tất cả bảng chi nhánh có cùng cấu trúc không
WITH structure_comparison AS (
    SELECT 
        t.name AS TableName,
        COUNT(c.name) AS ColumnCount,
        STRING_AGG(c.name + ':' + ty.name, '|') WITHIN GROUP (ORDER BY c.column_id) AS StructureHash
    FROM sys.tables t
    INNER JOIN sys.columns c ON t.object_id = c.object_id
    INNER JOIN sys.types ty ON c.system_type_id = ty.system_type_id
    WHERE t.name LIKE 'KPI_CN_%' OR t.name = 'KPI_HOI_SO'
    GROUP BY t.name
),
reference_structure AS (
    SELECT 
        COUNT(c.name) AS RefColumnCount,
        STRING_AGG(c.name + ':' + ty.name, '|') WITHIN GROUP (ORDER BY c.column_id) AS RefStructureHash
    FROM sys.tables t
    INNER JOIN sys.columns c ON t.object_id = c.object_id
    INNER JOIN sys.types ty ON c.system_type_id = ty.system_type_id
    WHERE t.name = 'GiamdocCnl2_KPI_Assignment'
)
SELECT 
    'Structure Verification' as Check_Type,
    sc.TableName,
    CASE 
        WHEN sc.ColumnCount = rs.RefColumnCount AND sc.StructureHash = rs.RefStructureHash
        THEN 'IDENTICAL'
        WHEN sc.ColumnCount = rs.RefColumnCount
        THEN 'SAME_COLUMN_COUNT'
        ELSE 'DIFFERENT'
    END as Comparison_Result
FROM structure_comparison sc
CROSS JOIN reference_structure rs
ORDER BY sc.TableName;

-- 8. Tóm tắt kết quả
DECLARE @total_branch_tables INT = (
    SELECT COUNT(*) 
    FROM sys.tables 
    WHERE name LIKE 'KPI_CN_%' OR name = 'KPI_HOI_SO'
);

SELECT 
    'Final Summary' as Status,
    @total_branch_tables as Total_Branch_Tables,
    9 as Expected_Tables,
    CASE 
        WHEN @total_branch_tables = 9 THEN 'COMPLETE'
        ELSE 'MISSING TABLES'
    END as Table_Count_Status;
