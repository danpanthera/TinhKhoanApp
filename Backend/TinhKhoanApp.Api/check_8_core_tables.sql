-- Kiểm tra 8 bảng CORE DATA TABLES
-- DP01, LN01, LN03, GL01, GL41, DPDA, EI01, RR01

SELECT
    'DP01' as TableName,
    CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DP01')
         THEN 'EXISTS'
         ELSE 'NOT FOUND'
    END as Status,
    CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DP01')
         THEN (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_NAME = 'DP01'
               AND COLUMN_NAME NOT LIKE '%Period%'
               AND COLUMN_NAME NOT LIKE '%Valid%'
               AND COLUMN_NAME NOT LIKE '%Sys%'
               AND COLUMN_NAME != 'Id')
         ELSE 0
    END as BusinessColumns
UNION ALL
SELECT
    'LN01' as TableName,
    CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LN01')
         THEN 'EXISTS'
         ELSE 'NOT FOUND'
    END as Status,
    CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LN01')
         THEN (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_NAME = 'LN01'
               AND COLUMN_NAME NOT LIKE '%Period%'
               AND COLUMN_NAME NOT LIKE '%Valid%'
               AND COLUMN_NAME NOT LIKE '%Sys%'
               AND COLUMN_NAME != 'Id')
         ELSE 0
    END as BusinessColumns
UNION ALL
SELECT
    'LN03' as TableName,
    CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LN03')
         THEN 'EXISTS'
         ELSE 'NOT FOUND'
    END as Status,
    CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LN03')
         THEN (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_NAME = 'LN03'
               AND COLUMN_NAME NOT LIKE '%Period%'
               AND COLUMN_NAME NOT LIKE '%Valid%'
               AND COLUMN_NAME NOT LIKE '%Sys%'
               AND COLUMN_NAME != 'Id')
         ELSE 0
    END as BusinessColumns
UNION ALL
SELECT
    'GL01' as TableName,
    CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GL01')
         THEN 'EXISTS'
         ELSE 'NOT FOUND'
    END as Status,
    CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GL01')
         THEN (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_NAME = 'GL01'
               AND COLUMN_NAME NOT LIKE '%Period%'
               AND COLUMN_NAME NOT LIKE '%Valid%'
               AND COLUMN_NAME NOT LIKE '%Sys%'
               AND COLUMN_NAME != 'Id')
         ELSE 0
    END as BusinessColumns
UNION ALL
SELECT
    'GL41' as TableName,
    CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GL41')
         THEN 'EXISTS'
         ELSE 'NOT FOUND'
    END as Status,
    CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GL41')
         THEN (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_NAME = 'GL41'
               AND COLUMN_NAME NOT LIKE '%Period%'
               AND COLUMN_NAME NOT LIKE '%Valid%'
               AND COLUMN_NAME NOT LIKE '%Sys%'
               AND COLUMN_NAME != 'Id')
         ELSE 0
    END as BusinessColumns
UNION ALL
SELECT
    'DPDA' as TableName,
    CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DPDA')
         THEN 'EXISTS'
         ELSE 'NOT FOUND'
    END as Status,
    CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DPDA')
         THEN (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_NAME = 'DPDA'
               AND COLUMN_NAME NOT LIKE '%Period%'
               AND COLUMN_NAME NOT LIKE '%Valid%'
               AND COLUMN_NAME NOT LIKE '%Sys%'
               AND COLUMN_NAME != 'Id')
         ELSE 0
    END as BusinessColumns
UNION ALL
SELECT
    'EI01' as TableName,
    CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'EI01')
         THEN 'EXISTS'
         ELSE 'NOT FOUND'
    END as Status,
    CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'EI01')
         THEN (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_NAME = 'EI01'
               AND COLUMN_NAME NOT LIKE '%Period%'
               AND COLUMN_NAME NOT LIKE '%Valid%'
               AND COLUMN_NAME NOT LIKE '%Sys%'
               AND COLUMN_NAME != 'Id')
         ELSE 0
    END as BusinessColumns
UNION ALL
SELECT
    'RR01' as TableName,
    CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RR01')
         THEN 'EXISTS'
         ELSE 'NOT FOUND'
    END as Status,
    CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RR01')
         THEN (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_NAME = 'RR01'
               AND COLUMN_NAME NOT LIKE '%Period%'
               AND COLUMN_NAME NOT LIKE '%Valid%'
               AND COLUMN_NAME NOT LIKE '%Sys%'
               AND COLUMN_NAME != 'Id')
         ELSE 0
    END as BusinessColumns
ORDER BY TableName;
