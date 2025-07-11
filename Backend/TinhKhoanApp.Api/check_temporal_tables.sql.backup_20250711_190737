-- 🔍 SCRIPT KIỂM TRA TEMPORAL TABLES + COLUMNSTORE INDEXES
-- Kiểm tra tất cả bảng dữ liệu có sử dụng Temporal Tables và Columnstore Indexes không

-- 1. Kiểm tra Temporal Tables
SELECT
    SCHEMA_NAME(t.schema_id) AS SchemaName,
    t.name AS TableName,
    t.temporal_type_desc AS TemporalType,
    CASE
        WHEN t.temporal_type = 2 THEN 'System-Versioned Temporal Table'
        WHEN t.temporal_type = 1 THEN 'History Table'
        ELSE 'Regular Table'
    END AS TableType,
    th.name AS HistoryTableName
FROM sys.tables t
LEFT JOIN sys.tables th ON t.history_table_id = th.object_id
WHERE t.name LIKE '%History' OR t.name IN (
    'LN01_History', 'LN02_History', 'LN03_History', 'DP01_History',
    'EI01_History', 'GL01_History', 'DPDA_History', 'DB01_History',
    'KH03_History', 'BC57_History', 'RR01_History', 'GL41_History',
    'DT_KHKD1_History', 'ImportedDataRecords', 'ImportedDataItems'
)
ORDER BY SchemaName, TableName;

-- 2. Kiểm tra Columnstore Indexes
SELECT
    SCHEMA_NAME(t.schema_id) AS SchemaName,
    t.name AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType,
    CASE
        WHEN i.type = 5 THEN 'Clustered Columnstore'
        WHEN i.type = 6 THEN 'Nonclustered Columnstore'
        ELSE 'Other'
    END AS ColumnstoreType
FROM sys.tables t
INNER JOIN sys.indexes i ON t.object_id = i.object_id
WHERE i.type IN (5, 6) -- Columnstore indexes
   OR t.name LIKE '%History'
ORDER BY SchemaName, TableName, IndexName;

-- 3. Tổng hợp trạng thái tối ưu hóa
SELECT
    t.name AS TableName,
    CASE
        WHEN t.temporal_type = 2 THEN '✅ Temporal'
        ELSE '❌ Not Temporal'
    END AS TemporalStatus,
    CASE
        WHEN EXISTS (SELECT 1 FROM sys.indexes i WHERE i.object_id = t.object_id AND i.type IN (5,6))
        THEN '✅ Has Columnstore'
        ELSE '❌ No Columnstore'
    END AS ColumnstoreStatus
FROM sys.tables t
WHERE t.name LIKE '%History' OR t.name IN (
    'ImportedDataRecords', 'ImportedDataItems'
)
ORDER BY TableName;
