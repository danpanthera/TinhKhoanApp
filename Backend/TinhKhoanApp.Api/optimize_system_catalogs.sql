-- Script tối ưu SQL Server system metadata để tăng tốc queries
-- Date: 2025-06-20

-- 1. Update statistics cho system tables
UPDATE STATISTICS sys.tables;
UPDATE STATISTICS sys.columns;
UPDATE STATISTICS sys.indexes;
UPDATE STATISTICS sys.objects;

-- 2. Rebuild system indexes nếu cần (chạy cẩn thận)
-- DBCC DBREINDEX('sys.objects');
-- DBCC DBREINDEX('sys.tables');

-- 3. Update statistics cho toàn bộ database
EXEC sp_updatestats;

-- 4. Reorganize fragmented indexes
DECLARE @sql NVARCHAR(MAX) = '';
SELECT @sql = @sql + 'ALTER INDEX ' + QUOTENAME(i.name) + ' ON ' + QUOTENAME(SCHEMA_NAME(t.schema_id)) + '.' + QUOTENAME(t.name) + ' REORGANIZE;' + CHAR(13)
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE i.index_id > 0 
  AND i.type_desc != 'CLUSTERED COLUMNSTORE'
  AND OBJECT_NAME(i.object_id) NOT LIKE 'sys%';

-- Uncomment to execute:
-- EXEC sp_executesql @sql;

PRINT 'SQL Server system optimization completed.';
