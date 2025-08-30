-- 🔍 SCRIPT KIỂM TRA TRẠNG THÁI TEMPORAL TABLES
-- Ngày: 22/06/2025

USE TinhKhoanDB;
GO

-- Kiểm tra tất cả tables và temporal status
PRINT '📊 TRẠNG THÁI HIỆN TẠI CỦA TẤT CẢ TABLES'
PRINT '==========================================='

SELECT 
    t.name AS TableName,
    CASE 
        WHEN t.temporal_type = 0 THEN 'NON_TEMPORAL'
        WHEN t.temporal_type = 1 THEN 'HISTORY_TABLE'  
        WHEN t.temporal_type = 2 THEN 'SYSTEM_VERSIONED_TEMPORAL_TABLE'
    END AS TemporalType,
    h.name AS HistoryTableName,
    t.object_id,
    t.temporal_type
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
WHERE t.name NOT IN ('__EFMigrationsHistory', 'sysdiagrams')
ORDER BY t.name;

-- Kiểm tra triggers có thể cản trở việc chuyển đổi
PRINT ''
PRINT '🚫 TRIGGERS CÓ THỂ CẢN TRỞ CHUYỂN ĐỔI'
PRINT '===================================='

SELECT 
    t.name AS TableName,
    tr.name AS TriggerName,
    tr.type_desc AS TriggerType
FROM sys.tables t
INNER JOIN sys.triggers tr ON t.object_id = tr.parent_id
WHERE t.name NOT IN ('__EFMigrationsHistory', 'sysdiagrams')
ORDER BY t.name;

-- Kiểm tra constraints có thể ảnh hưởng
PRINT ''
PRINT '🔗 FOREIGN KEY CONSTRAINTS'
PRINT '=========================='

SELECT 
    tp.name AS ParentTable,
    cp.name AS ParentColumn,
    tr.name AS ReferencedTable,
    cr.name AS ReferencedColumn,
    fk.name AS ConstraintName
FROM sys.foreign_keys fk
INNER JOIN sys.tables tp ON fk.parent_object_id = tp.object_id
INNER JOIN sys.tables tr ON fk.referenced_object_id = tr.object_id
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.columns cp ON fkc.parent_column_id = cp.column_id AND fkc.parent_object_id = cp.object_id
INNER JOIN sys.columns cr ON fkc.referenced_column_id = cr.column_id AND fkc.referenced_object_id = cr.object_id
ORDER BY tp.name;

PRINT ''
PRINT '📈 THỐNG KÊ TỔNG QUAN'
PRINT '==================='

DECLARE @TotalTables INT, @TemporalTables INT, @NonTemporalTables INT
SELECT @TotalTables = COUNT(*) FROM sys.tables WHERE name NOT IN ('__EFMigrationsHistory', 'sysdiagrams')
SELECT @TemporalTables = COUNT(*) FROM sys.tables WHERE temporal_type = 2
SELECT @NonTemporalTables = @TotalTables - @TemporalTables

PRINT 'Tổng số tables: ' + CAST(@TotalTables AS VARCHAR(10))
PRINT 'Đã là Temporal Tables: ' + CAST(@TemporalTables AS VARCHAR(10))
PRINT 'Chưa chuyển đổi: ' + CAST(@NonTemporalTables AS VARCHAR(10))
PRINT 'Tỷ lệ hoàn thành: ' + CAST((@TemporalTables * 100 / @TotalTables) AS VARCHAR(10)) + '%'

GO
