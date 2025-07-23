-- Fix NGAY_DL with indexes handling
PRINT '🔧 Starting NGAY_DL to DATETIME2 conversion with index management...'

-- Drop dependent indexes first
PRINT '🗑️ Dropping dependent indexes...'

-- Drop columnstore indexes that depend on NGAY_DL
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('DP01') AND name = 'NCCI_DP01_Analytics')
    DROP INDEX NCCI_DP01_Analytics ON DP01

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('EI01') AND name = 'NCCI_EI01_Analytics')
    DROP INDEX NCCI_EI01_Analytics ON EI01

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('GL41') AND name = 'NCCI_GL41_Analytics')
    DROP INDEX NCCI_GL41_Analytics ON GL41

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('LN01') AND name = 'IX_LN01_NGAY_DL')
    DROP INDEX IX_LN01_NGAY_DL ON LN01

-- Drop any other indexes that might depend on NGAY_DL
DECLARE @sql NVARCHAR(MAX) = ''
SELECT @sql = @sql + 'DROP INDEX ' + i.name + ' ON ' + t.name + ';' + CHAR(13)
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
WHERE c.name = 'NGAY_DL'
    AND t.name IN ('DP01', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01', 'DPDA')
    AND i.type <> 1 -- Not clustered index (primary key)

IF LEN(@sql) > 0
BEGIN
    PRINT 'Executing: ' + @sql
    EXEC sp_executesql @sql
END

-- Clear existing data
PRINT '🗑️ Clearing existing data...'
DELETE FROM DP01
DELETE FROM EI01
DELETE FROM GL01
DELETE FROM GL41
DELETE FROM LN01
DELETE FROM LN03
DELETE FROM RR01
DELETE FROM DPDA

PRINT '🔄 Converting NGAY_DL columns to DATETIME2...'

-- Convert all NGAY_DL columns to DATETIME2
ALTER TABLE DP01 ALTER COLUMN NGAY_DL DATETIME2 NOT NULL
ALTER TABLE EI01 ALTER COLUMN NGAY_DL DATETIME2 NOT NULL
ALTER TABLE GL01 ALTER COLUMN NGAY_DL DATETIME2 NOT NULL
ALTER TABLE GL41 ALTER COLUMN NGAY_DL DATETIME2 NOT NULL
ALTER TABLE LN01 ALTER COLUMN NGAY_DL DATETIME2 NOT NULL
ALTER TABLE LN03 ALTER COLUMN NGAY_DL DATETIME2 NOT NULL
ALTER TABLE RR01 ALTER COLUMN NGAY_DL DATETIME2 NOT NULL
ALTER TABLE DPDA ALTER COLUMN NGAY_DL DATETIME2 NOT NULL

PRINT '✅ NGAY_DL conversion completed!'

-- Verify the changes
PRINT '🔍 Verification - All NGAY_DL columns should now be datetime2:'
SELECT
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE COLUMN_NAME = 'NGAY_DL'
    AND TABLE_NAME IN ('DP01', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01', 'DPDA')
ORDER BY TABLE_NAME

PRINT '📅 Ready for DD/MM/YYYY format imports!'
