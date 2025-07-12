-- ===============================================
-- CẤU HÌNH UTF-8 ENCODING CHO TIẾNG VIỆT
-- ===============================================

USE TinhKhoanDB;
GO

-- Kiểm tra và cấu hình collation UTF-8 cho các bảng quan trọng
PRINT '🔤 Cấu hình UTF-8 encoding cho tiếng Việt...';

-- Kiểm tra collation hiện tại
PRINT 'Database collation hiện tại:';
SELECT
    DB_NAME() AS DatabaseName,
    DATABASEPROPERTYEX(DB_NAME(), 'Collation') AS DatabaseCollation;

-- Thông tin về các collation hỗ trợ tiếng Việt
PRINT 'Các collation hỗ trợ UTF-8:';
SELECT name, description
FROM sys.fn_helpcollations()
WHERE name LIKE '%UTF8%' OR name LIKE '%Vietnamese%'
ORDER BY name;

-- Kiểm tra một số bảng quan trọng có collation UTF-8 chưa
PRINT 'Kiểm tra collation của các bảng dữ liệu:';
SELECT
    t.name AS TableName,
    c.name AS ColumnName,
    c.collation_name AS ColumnCollation,
    ty.name AS DataType,
    c.max_length
FROM sys.tables t
INNER JOIN sys.columns c ON t.object_id = c.object_id
INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
WHERE t.name IN ('DP01', 'DB01', 'Employees', 'Units', 'Roles')
AND c.collation_name IS NOT NULL
ORDER BY t.name, c.name;

PRINT '✅ Hoàn thành kiểm tra UTF-8 encoding configuration!';
