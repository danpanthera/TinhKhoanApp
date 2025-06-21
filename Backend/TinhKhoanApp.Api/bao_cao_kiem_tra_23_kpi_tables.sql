-- BÁO CÁO KIỂM TRA MỤC CẤU HÌNH KPI - 23 BẢNG CHUẨN - SQL SERVER VERSION
-- Ngày kiểm tra: $(date '+%Y-%m-%d %H:%M:%S')

-- 1. KIỂM TRA TỔNG QUAN
SELECT 'TỔNG QUAN KIỂM TRA' as Status;
SELECT 'Total Roles:' as Metric, COUNT(*) as Count FROM Roles;
SELECT 'Total KPI Assignment Tables:' as Metric, COUNT(*) as Count FROM sys.tables WHERE name LIKE '%_KPI_Assignment';

-- 2. KIỂM TRA 23 VAI TRÒ CÁN BỘ CHUẨN
SELECT 'DANH SÁCH 23 VAI TRÒ CHUẨN' as Status;
SELECT ROW_NUMBER() OVER (ORDER BY Name) as STT, Name as RoleCode, Description 
FROM Roles 
ORDER BY Name;

-- 3. KIỂM TRA 23 BẢNG KPI ASSIGNMENT TƯƠNG ỨNG
SELECT 'DANH SÁCH 23 BẢNG KPI ASSIGNMENT' as Status;
SELECT ROW_NUMBER() OVER (ORDER BY name) as STT, name as TableName
FROM sys.tables 
WHERE name LIKE '%_KPI_Assignment'
ORDER BY name;

-- 4. XÁC MINH TÍNH KHỚP GIỮA ROLES VÀ KPI ASSIGNMENT TABLES
SELECT 'KIỂM TRA TÍNH KHỚP' as Status;
WITH role_tables AS (
    SELECT Name || '_KPI_Assignment' as expected_table 
    FROM Roles
),
actual_tables AS (
    SELECT name as table_name
    FROM sys.tables 
    WHERE name LIKE '%_KPI_Assignment'
)
SELECT 
    CASE 
        WHEN rt.expected_table IS NOT NULL AND at.table_name IS NOT NULL THEN 'KHỚP'
        WHEN rt.expected_table IS NOT NULL AND at.table_name IS NULL THEN 'THIẾU BẢNG'
        WHEN rt.expected_table IS NULL AND at.table_name IS NOT NULL THEN 'BẢNG THỪA'
    END as Status,
    COALESCE(rt.expected_table, at.table_name) as TableName
FROM role_tables rt
FULL OUTER JOIN actual_tables at ON rt.expected_table = at.table_name
ORDER BY Status, TableName;

-- 5. KẾT LUẬN
SELECT 'KẾT LUẬN KIỂM TRA' as Status;
SELECT 
    CASE 
        WHEN (SELECT COUNT(*) FROM Roles) = 23 
         AND (SELECT COUNT(*) FROM sys.tables WHERE name LIKE '%_KPI_Assignment') = 23
        THEN '✅ HOÀN THÀNH: Đã có đủ 23 vai trò và 23 bảng KPI Assignment chuẩn'
        ELSE '❌ CHƯA HOÀN THÀNH: Thiếu vai trò hoặc bảng KPI Assignment'
    END as FinalResult;
