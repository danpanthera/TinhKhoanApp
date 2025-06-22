-- 🚀 SCRIPT TẠO COLUMNSTORE INDEXES CHO TẤT CẢ HISTORY TABLES
-- Tối ưu hiệu suất truy vấn cho Temporal Tables
-- Ngày: 22/06/2025

USE TinhKhoanDB;
GO

PRINT '🚀 TẠO COLUMNSTORE INDEXES CHO HISTORY TABLES'
PRINT '============================================='

DECLARE @sql NVARCHAR(MAX)
DECLARE @historyTableName NVARCHAR(128)
DECLARE @indexName NVARCHAR(128)
DECLARE @counter INT = 0
DECLARE @totalTables INT

-- Đếm tổng số history tables
SELECT @totalTables = COUNT(*)
FROM sys.tables h
INNER JOIN sys.tables t ON h.object_id = t.history_table_id
WHERE t.temporal_type = 2

PRINT 'Tổng số History Tables cần tạo Columnstore Index: ' + CAST(@totalTables AS VARCHAR(10))
PRINT ''

-- Cursor để duyệt qua tất cả history tables
DECLARE history_cursor CURSOR FOR
SELECT h.name
FROM sys.tables h
INNER JOIN sys.tables t ON h.object_id = t.history_table_id
WHERE t.temporal_type = 2
ORDER BY h.name

OPEN history_cursor
FETCH NEXT FROM history_cursor INTO @historyTableName

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @counter = @counter + 1
    SET @indexName = 'CCI_' + @historyTableName
    
    PRINT CAST(@counter AS VARCHAR(3)) + '/' + CAST(@totalTables AS VARCHAR(3)) + ' 🔧 Tạo Columnstore Index cho ' + @historyTableName
    
    BEGIN TRY
        -- Kiểm tra xem đã có Columnstore Index chưa
        IF NOT EXISTS (
            SELECT * FROM sys.indexes 
            WHERE object_id = OBJECT_ID(@historyTableName) 
            AND type IN (5, 6)  -- Columnstore indexes
        )
        BEGIN
            -- Kiểm tra có clustered index thông thường không
            IF EXISTS (
                SELECT * FROM sys.indexes 
                WHERE object_id = OBJECT_ID(@historyTableName) 
                AND type = 1  -- Clustered index
                AND is_primary_key = 0
            )
            BEGIN
                -- Drop existing clustered index và tạo columnstore
                DECLARE @existingIndex NVARCHAR(128)
                SELECT @existingIndex = name 
                FROM sys.indexes 
                WHERE object_id = OBJECT_ID(@historyTableName) 
                AND type = 1 
                AND is_primary_key = 0
                
                SET @sql = 'DROP INDEX ' + @existingIndex + ' ON ' + @historyTableName
                EXEC sp_executesql @sql
                PRINT '  📝 Đã xóa index cũ: ' + @existingIndex
            END
            
            -- Tạo Columnstore Index
            SET @sql = 'CREATE CLUSTERED COLUMNSTORE INDEX ' + @indexName + ' ON dbo.' + @historyTableName
            EXEC sp_executesql @sql
            
            PRINT '  ✅ Đã tạo ' + @indexName + ' thành công!'
        END
        ELSE
        BEGIN
            PRINT '  ℹ️ ' + @historyTableName + ' đã có Columnstore Index'
        END
        
    END TRY
    BEGIN CATCH
        PRINT '  ❌ Lỗi: ' + ERROR_MESSAGE()
    END CATCH
    
    FETCH NEXT FROM history_cursor INTO @historyTableName
END

CLOSE history_cursor
DEALLOCATE history_cursor

-- Kiểm tra kết quả
PRINT ''
PRINT '📊 KIỂM TRA KẾT QUẢ COLUMNSTORE INDEXES'
PRINT '====================================='

SELECT 
    h.name AS HistoryTable,
    CASE 
        WHEN EXISTS (
            SELECT * FROM sys.indexes i 
            WHERE i.object_id = h.object_id 
            AND i.type IN (5, 6)
        ) THEN '✅ CÓ COLUMNSTORE'
        ELSE '❌ CHƯA CÓ COLUMNSTORE'
    END AS ColumnstoreStatus,
    i.name AS IndexName,
    CASE i.type
        WHEN 5 THEN 'CLUSTERED COLUMNSTORE'
        WHEN 6 THEN 'NONCLUSTERED COLUMNSTORE'
        ELSE 'UNKNOWN'
    END AS IndexType
FROM sys.tables h
INNER JOIN sys.tables t ON h.object_id = t.history_table_id
LEFT JOIN sys.indexes i ON h.object_id = i.object_id AND i.type IN (5, 6)
WHERE t.temporal_type = 2
ORDER BY h.name

-- Thống kê tổng quan
DECLARE @totalHistoryTables INT, @historyTablesWithColumnstore INT
SELECT @totalHistoryTables = COUNT(DISTINCT h.object_id)
FROM sys.tables h
INNER JOIN sys.tables t ON h.object_id = t.history_table_id
WHERE t.temporal_type = 2

SELECT @historyTablesWithColumnstore = COUNT(DISTINCT h.object_id)
FROM sys.tables h
INNER JOIN sys.tables t ON h.object_id = t.history_table_id
INNER JOIN sys.indexes i ON h.object_id = i.object_id AND i.type IN (5, 6)
WHERE t.temporal_type = 2

PRINT ''
PRINT '📈 THỐNG KÊ COLUMNSTORE INDEXES:'
PRINT 'Tổng History Tables: ' + CAST(@totalHistoryTables AS VARCHAR(10))
PRINT 'Có Columnstore Index: ' + CAST(@historyTablesWithColumnstore AS VARCHAR(10))
PRINT 'Tỷ lệ: ' + CAST((@historyTablesWithColumnstore * 100 / @totalHistoryTables) AS VARCHAR(10)) + '%'

PRINT ''
PRINT '🎉 HOÀN THÀNH TẠO COLUMNSTORE INDEXES!'
PRINT 'Thời gian: ' + CONVERT(VARCHAR, GETDATE(), 120)

GO
