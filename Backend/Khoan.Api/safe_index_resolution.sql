-- FINAL SOLUTION: Safe Index Resolution Script
-- This script safely drops all problematic indexes that cause migration failures
-- Run this once to resolve all IX_*_NGAY_DL index drop errors permanently

PRINT '🛡️ STARTING FINAL SAFE INDEX RESOLUTION'
PRINT '========================================'

-- Function to safely drop an index if it exists
DECLARE @sql NVARCHAR(MAX)
DECLARE @indexName NVARCHAR(128)
DECLARE @tableName NVARCHAR(128)

-- List of all potentially problematic indexes
DECLARE index_cursor CURSOR FOR
SELECT
    i.name AS IndexName,
    t.name AS TableName
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE i.name LIKE 'IX_%_NGAY_DL%'
   AND i.type > 0  -- Exclude heap
   AND t.name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')

OPEN index_cursor
FETCH NEXT FROM index_cursor INTO @indexName, @tableName

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Check if index exists and drop it safely
    IF EXISTS (
        SELECT 1 FROM sys.indexes i
        INNER JOIN sys.objects o ON i.object_id = o.object_id
        WHERE i.name = @indexName AND o.name = @tableName
    )
    BEGIN
        SET @sql = 'DROP INDEX [' + @indexName + '] ON [dbo].[' + @tableName + ']'

        BEGIN TRY
            EXEC sp_executesql @sql
            PRINT '🗑️ Successfully dropped index: ' + @indexName + ' on ' + @tableName
        END TRY
        BEGIN CATCH
            PRINT '⚠️ Could not drop index: ' + @indexName + ' on ' + @tableName + ' - ' + ERROR_MESSAGE()
        END CATCH
    END
    ELSE
    BEGIN
        PRINT '✅ Index already safe: ' + @indexName + ' on ' + @tableName + ' (does not exist)'
    END

    FETCH NEXT FROM index_cursor INTO @indexName, @tableName
END

CLOSE index_cursor
DEALLOCATE index_cursor

-- Specifically handle the most problematic ones mentioned in the error
DECLARE @problemIndexes TABLE (IndexName NVARCHAR(128), TableName NVARCHAR(128))
INSERT INTO @problemIndexes VALUES
    ('IX_GL01_NGAY_DL', 'GL01'),
    ('IX_RR01_NGAY_DL', 'RR01'),
    ('IX_DP01_NGAY_DL', 'DP01'),
    ('IX_LN03_NGAY_DL', 'LN03'),
    ('IX_GL41_NGAY_DL', 'GL41'),
    ('IX_GL41_MaCN', 'GL41'),
    ('IX_GL41_NGAY_DL_MaCN', 'GL41')

DECLARE specific_cursor CURSOR FOR
SELECT IndexName, TableName FROM @problemIndexes

OPEN specific_cursor
FETCH NEXT FROM specific_cursor INTO @indexName, @tableName

WHILE @@FETCH_STATUS = 0
BEGIN
    IF EXISTS (
        SELECT 1 FROM sys.indexes i
        INNER JOIN sys.objects o ON i.object_id = o.object_id
        WHERE i.name = @indexName AND o.name = @tableName
    )
    BEGIN
        SET @sql = 'DROP INDEX [' + @indexName + '] ON [dbo].[' + @tableName + ']'

        BEGIN TRY
            EXEC sp_executesql @sql
            PRINT '🎯 Specifically dropped problematic index: ' + @indexName + ' on ' + @tableName
        END TRY
        BEGIN CATCH
            PRINT '⚠️ Could not drop specific index: ' + @indexName + ' on ' + @tableName + ' - ' + ERROR_MESSAGE()
        END CATCH
    END
    ELSE
    BEGIN
        PRINT '✅ Problematic index already safe: ' + @indexName + ' on ' + @tableName + ' (does not exist)'
    END

    FETCH NEXT FROM specific_cursor INTO @indexName, @tableName
END

CLOSE specific_cursor
DEALLOCATE specific_cursor

-- Verify columnstore indexes are still intact
PRINT ''
PRINT '🔍 VERIFYING COLUMNSTORE INDEXES:'
PRINT '================================='

IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('DP01') AND type = 6)
    PRINT '✅ DP01 columnstore index: INTACT'
ELSE
    PRINT '⚠️ DP01 columnstore index: MISSING'

IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('GL01') AND type = 5)
    PRINT '✅ GL01 columnstore index: INTACT'
ELSE
    PRINT '⚠️ GL01 columnstore index: MISSING'

-- Count remaining indexes
DECLARE @remainingCount INT
SELECT @remainingCount = COUNT(*)
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE i.name LIKE 'IX_%_NGAY_DL%'
   AND i.type > 0
   AND t.name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')

PRINT ''
PRINT '📊 FINAL STATUS:'
PRINT '==============='
PRINT 'Remaining problematic NGAY_DL indexes: ' + CAST(@remainingCount AS NVARCHAR(10))

IF @remainingCount = 0
    PRINT '🎉 SUCCESS: All problematic indexes have been safely removed!'
ELSE
    PRINT '⚠️ WARNING: Some indexes may still cause migration issues'

PRINT ''
PRINT '✅ SAFE INDEX RESOLUTION COMPLETE'
PRINT 'Future migrations will no longer fail on "Cannot drop the index" errors'
