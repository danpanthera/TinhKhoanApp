-- Tạo views tối ưu cho metadata queries - SQL Server version
-- Date: 2025-06-20

-- View nhanh cho danh sách tables (thay thế SHOW TABLES)
CREATE VIEW fast_tables AS
SELECT 
    s.name as schema_name,
    t.name as table_name,
    CAST(ROUND(((SUM(a.total_pages) * 8) / 1024.00), 2) AS NUMERIC(36,2)) AS total_size_mb,
    ep.value as description
FROM sys.tables t
INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id
INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
LEFT OUTER JOIN sys.schemas s ON t.schema_id = s.schema_id
LEFT OUTER JOIN sys.extended_properties ep ON t.object_id = ep.major_id AND ep.minor_id = 0 AND ep.name = 'MS_Description'
WHERE t.NAME NOT LIKE 'dt%' AND t.is_ms_shipped = 0 AND i.OBJECT_ID > 255
GROUP BY t.Name, s.Name, ep.value
GO

-- View cho table với row counts
CREATE VIEW fast_table_stats AS
SELECT 
    s.name AS schema_name,
    t.name AS table_name,
    i.rows AS row_count,
    CAST(ROUND(((SUM(a.total_pages) * 8) / 1024.00), 2) AS NUMERIC(36,2)) AS total_size_mb,
    CAST(ROUND(((SUM(a.used_pages) * 8) / 1024.00), 2) AS NUMERIC(36,2)) AS used_size_mb,
    CAST(ROUND(((SUM(a.total_pages) - SUM(a.used_pages)) * 8) / 1024.00, 2) AS NUMERIC(36,2)) AS unused_size_mb
FROM sys.tables t
INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id
INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
LEFT OUTER JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE t.NAME NOT LIKE 'dt%' AND t.is_ms_shipped = 0 AND i.OBJECT_ID > 255
GROUP BY t.Name, s.Name, i.rows
GO

-- View cho indexes
CREATE VIEW fast_indexes AS
SELECT 
    s.name AS schema_name,
    t.name AS table_name,
    i.name AS index_name,
    i.type_desc AS index_type,
    i.is_unique,
    i.is_primary_key,
    STRING_AGG(c.name, ', ') AS columns
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
WHERE t.is_ms_shipped = 0 AND i.name IS NOT NULL
GROUP BY s.name, t.name, i.name, i.type_desc, i.is_unique, i.is_primary_key
GO

-- View cho columns thông tin
CREATE VIEW fast_columns AS
SELECT 
    s.name AS schema_name,
    t.name AS table_name,
    c.name AS column_name,
    ty.name AS data_type,
    c.max_length,
    c.precision,
    c.scale,
    c.is_nullable,
    c.is_identity,
    dc.definition AS default_value,
    ep.value AS description
FROM sys.columns c
INNER JOIN sys.tables t ON c.object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
INNER JOIN sys.types ty ON c.system_type_id = ty.system_type_id AND c.user_type_id = ty.user_type_id
LEFT JOIN sys.default_constraints dc ON c.default_object_id = dc.object_id
LEFT JOIN sys.extended_properties ep ON c.object_id = ep.major_id AND c.column_id = ep.minor_id AND ep.name = 'MS_Description'
WHERE t.is_ms_shipped = 0
GO

-- View cho foreign keys
CREATE VIEW fast_foreign_keys AS
SELECT 
    s1.name AS schema_name,
    t1.name AS table_name,
    c1.name AS column_name,
    s2.name AS referenced_schema,
    t2.name AS referenced_table,
    c2.name AS referenced_column,
    fk.name AS constraint_name,
    fk.delete_referential_action_desc AS delete_rule,
    fk.update_referential_action_desc AS update_rule
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.tables t1 ON fkc.parent_object_id = t1.object_id
INNER JOIN sys.schemas s1 ON t1.schema_id = s1.schema_id
INNER JOIN sys.columns c1 ON fkc.parent_object_id = c1.object_id AND fkc.parent_column_id = c1.column_id
INNER JOIN sys.tables t2 ON fkc.referenced_object_id = t2.object_id
INNER JOIN sys.schemas s2 ON t2.schema_id = s2.schema_id
INNER JOIN sys.columns c2 ON fkc.referenced_object_id = c2.object_id AND fkc.referenced_column_id = c2.column_id
GO
