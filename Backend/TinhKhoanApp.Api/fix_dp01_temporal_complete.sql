-- Step 1: Disable system versioning for all temporal tables
-- This allows us to drop them safely

DECLARE @sql NVARCHAR(MAX) = '';

-- Get all temporal tables
SELECT @sql = @sql + 'ALTER TABLE [' + TABLE_SCHEMA + '].[' + TABLE_NAME + '] SET (SYSTEM_VERSIONING = OFF);' + CHAR(13)
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
AND TABLE_NAME IN (
    SELECT TABLE_NAME
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE COLUMN_NAME = 'PeriodStart' OR COLUMN_NAME = 'PeriodEnd'
);

PRINT 'Disabling system versioning for temporal tables:';
PRINT @sql;
EXEC sp_executesql @sql;

-- Step 2: Drop all DP01 related tables
SET @sql = '';
SELECT @sql = @sql + 'IF OBJECT_ID(''[' + TABLE_SCHEMA + '].[' + TABLE_NAME + ']'', ''U'') IS NOT NULL DROP TABLE [' + TABLE_SCHEMA + '].[' + TABLE_NAME + '];' + CHAR(13)
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME LIKE '%DP01%' OR TABLE_NAME LIKE '%dp01%'
ORDER BY TABLE_NAME;

PRINT 'Dropping DP01 related tables:';
PRINT @sql;
EXEC sp_executesql @sql;

-- Step 3: Drop history tables for temporal tables
SET @sql = '';
SELECT @sql = @sql + 'IF OBJECT_ID(''[dbo].[' + TABLE_NAME + '_History]'', ''U'') IS NOT NULL DROP TABLE [dbo].[' + TABLE_NAME + '_History];' + CHAR(13)
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME LIKE '%_History';

PRINT 'Dropping history tables:';
PRINT @sql;
EXEC sp_executesql @sql;

PRINT 'DP01 cleanup completed successfully!';
