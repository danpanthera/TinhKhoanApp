-- Kiểm tra ImportedDataItems table usage
USE TinhKhoanDB;

-- Kiểm tra số lượng records
SELECT COUNT(*) as 'ImportedDataItems_Count' FROM ImportedDataItems;

-- Kiểm tra records gần đây
SELECT TOP 5 
    Id, ImportedDataRecordId, ProcessedDate, 
    LEFT(RawData, 100) as RawData_Preview
FROM ImportedDataItems 
ORDER BY ProcessedDate DESC;

-- Kiểm tra dependencies/references
SELECT 
    OBJECT_NAME(parent_object_id) as TableName,
    name as ConstraintName
FROM sys.foreign_keys 
WHERE referenced_object_id = OBJECT_ID('ImportedDataItems');

-- Kiểm tra index usage
SELECT 
    i.name as IndexName,
    i.type_desc as IndexType,
    s.user_seeks,
    s.user_scans,
    s.user_lookups,
    s.last_user_seek,
    s.last_user_scan
FROM sys.indexes i
LEFT JOIN sys.dm_db_index_usage_stats s 
    ON i.object_id = s.object_id AND i.index_id = s.index_id
WHERE i.object_id = OBJECT_ID('ImportedDataItems')
AND i.name IS NOT NULL;
