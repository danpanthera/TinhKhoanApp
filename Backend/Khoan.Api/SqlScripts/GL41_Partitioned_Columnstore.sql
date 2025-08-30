-- =====================================================================================
-- GL41 Partitioned Columnstore Implementation Script (Full SQL Server)
-- Created: 2025-08-22
-- Purpose: Complete GL41 partitioned columnstore implementation for production
-- Environment: SQL Server Standard/Enterprise (Not Azure SQL Edge)
-- =====================================================================================

-- Environment check
IF @@VERSION LIKE '%Azure SQL Edge%'
BEGIN
    PRINT '🔶 Azure SQL Edge detected – skipping partitioned columnstore plan.';
    PRINT '💡 Current GL41 configuration on Edge is optimal with rowstore analytics indexes.';
    PRINT '📋 Refer to GL41_COLUMNSTORE_STATUS_REPORT.md for migration guidance.';
    RETURN;
END

PRINT '🎯 Starting GL41 Partitioned Columnstore plan...';
PRINT '📊 Target: Partitioned NONCLUSTERED COLUMNSTORE on NGAY_DL';

-- Variables
DECLARE @sql NVARCHAR(MAX);
DECLARE @partitionFunction NVARCHAR(128) = 'PF_GL41_NGAYDL';
DECLARE @partitionScheme NVARCHAR(128) = 'PS_GL41_NGAYDL';

-- =====================================================================================
-- STEP 1: Create Partition Function by NGAY_DL (Monthly Partitions)
-- =====================================================================================
IF NOT EXISTS (SELECT 1 FROM sys.partition_functions WHERE name = @partitionFunction)
BEGIN
    PRINT '📅 Creating partition function for GL41 by NGAY_DL...';

    SET @sql = N'
    CREATE PARTITION FUNCTION ' + QUOTENAME(@partitionFunction) + ' (datetime2)
    AS RANGE RIGHT FOR VALUES (
        ''2024-01-01'', ''2024-02-01'', ''2024-03-01'', ''2024-04-01'',
        ''2024-05-01'', ''2024-06-01'', ''2024-07-01'', ''2024-08-01'',
        ''2024-09-01'', ''2024-10-01'', ''2024-11-01'', ''2024-12-01'',
        ''2025-01-01'', ''2025-02-01'', ''2025-03-01'', ''2025-04-01'',
        ''2025-05-01'', ''2025-06-01'', ''2025-07-01'', ''2025-08-01'',
        ''2025-09-01'', ''2025-10-01'', ''2025-11-01'', ''2025-12-01'',
        ''2026-01-01''
    )';

    EXEC sp_executesql @sql;
    PRINT '✅ Partition function created successfully.';
END
ELSE
BEGIN
    PRINT '📅 Partition function already exists.';
END

-- =====================================================================================
-- STEP 2: Create Partition Scheme
-- =====================================================================================
IF NOT EXISTS (SELECT 1 FROM sys.partition_schemes WHERE name = @partitionScheme)
BEGIN
    PRINT '🗂️ Creating partition scheme for GL41...';

    SET @sql = N'
    CREATE PARTITION SCHEME ' + QUOTENAME(@partitionScheme) + '
    AS PARTITION ' + QUOTENAME(@partitionFunction) + '
    ALL TO ([PRIMARY])';

    EXEC sp_executesql @sql;
    PRINT '✅ Partition scheme created successfully.';
END
ELSE
BEGIN
    PRINT '🗂️ Partition scheme already exists.';
END

-- =====================================================================================
-- STEP 3: Disable SYSTEM_VERSIONING if active (make sure GL41 is non-temporal)
-- =====================================================================================
IF EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID('dbo.GL41') AND temporal_type = 2)
BEGIN
    PRINT '⏸️ Disabling SYSTEM_VERSIONING on GL41...';
    ALTER TABLE dbo.GL41 SET (SYSTEM_VERSIONING = OFF);
    PRINT '✅ SYSTEM_VERSIONING disabled.';
END
ELSE
BEGIN
    PRINT '✅ GL41 is already non-temporal.';
END

-- =====================================================================================
-- STEP 4: Drop existing columnstore indexes if any
-- =====================================================================================
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.GL41') AND type IN (5, 6))
BEGIN
    PRINT '🗑️ Dropping existing columnstore indexes...';

    DECLARE index_cursor CURSOR FOR
    SELECT name FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.GL41') AND type IN (5, 6);

    DECLARE @indexName NVARCHAR(128);
    OPEN index_cursor;
    FETCH NEXT FROM index_cursor INTO @indexName;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @sql = 'DROP INDEX ' + QUOTENAME(@indexName) + ' ON dbo.GL41';
        EXEC sp_executesql @sql;
        PRINT '✅ Dropped columnstore index: ' + @indexName;

        FETCH NEXT FROM index_cursor INTO @indexName;
    END

    CLOSE index_cursor;
    DEALLOCATE index_cursor;
END

-- =====================================================================================
-- STEP 5: Move Primary Key to Partition Scheme (if it exists and is named PK_GL41)
-- =====================================================================================
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.GL41') AND name = 'PK_GL41' AND type = 1)
BEGIN
    PRINT '🔧 Moving primary key to partition scheme...';

    -- This requires dropping and recreating the PK
    -- Note: In production, consider if this step is necessary or skip if PK structure is complex
    PRINT '💡 Primary key detected. Manual review recommended for PK partitioning.';
END

-- =====================================================================================
-- STEP 6: Create Partitioned NONCLUSTERED COLUMNSTORE INDEX
-- =====================================================================================
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.GL41') AND name = 'NCCI_GL41_Partitioned')
BEGIN
    PRINT '🏗️ Creating partitioned NONCLUSTERED COLUMNSTORE INDEX...';

    SET @sql = N'
    CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_GL41_Partitioned
    ON dbo.GL41 (
        NGAY_DL, MA_CN, LOAI_TIEN, MA_TK, LOAI_BT,
        SO_TK, TEN_TK, SO_DU_DAU_KY, SO_DU_CUOI_KY,
        PS_NO_TRONG_KY, PS_CO_TRONG_KY, PHAT_SINH_NO, PHAT_SINH_CO
    )
    ON ' + QUOTENAME(@partitionScheme) + '(NGAY_DL)';

    EXEC sp_executesql @sql;
    PRINT '✅ Partitioned NONCLUSTERED COLUMNSTORE INDEX created successfully!';
END
ELSE
BEGIN
    PRINT '✅ Partitioned columnstore index already exists.';
END

-- =====================================================================================
-- STEP 7: Create supporting rowstore indexes for mixed workloads
-- =====================================================================================
PRINT '🔧 Ensuring supporting rowstore indexes...';

-- Individual column indexes for point queries
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.GL41') AND name = 'IX_GL41_NGAY_DL_Partitioned')
BEGIN
    CREATE NONCLUSTERED INDEX IX_GL41_NGAY_DL_Partitioned
    ON dbo.GL41 (NGAY_DL) ON [PS_GL41_NGAYDL](NGAY_DL);
    PRINT '✅ Created partitioned NGAY_DL index.';
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.GL41') AND name = 'IX_GL41_MA_CN_Partitioned')
BEGIN
    CREATE NONCLUSTERED INDEX IX_GL41_MA_CN_Partitioned
    ON dbo.GL41 (MA_CN, NGAY_DL) ON [PS_GL41_NGAYDL](NGAY_DL);
    PRINT '✅ Created partitioned MA_CN index.';
END

-- =====================================================================================
-- STEP 8: Verification and Status Report
-- =====================================================================================
PRINT '';
PRINT '🔍 VERIFICATION RESULTS:';
PRINT '========================';

-- Check partition function
IF EXISTS (SELECT 1 FROM sys.partition_functions WHERE name = @partitionFunction)
    PRINT '✅ Partition Function: ' + @partitionFunction
ELSE
    PRINT '❌ Partition Function: Missing';

-- Check partition scheme
IF EXISTS (SELECT 1 FROM sys.partition_schemes WHERE name = @partitionScheme)
    PRINT '✅ Partition Scheme: ' + @partitionScheme
ELSE
    PRINT '❌ Partition Scheme: Missing';

-- Check columnstore index
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.GL41') AND name = 'NCCI_GL41_Partitioned' AND type = 6)
    PRINT '✅ Partitioned Columnstore Index: NCCI_GL41_Partitioned'
ELSE
    PRINT '❌ Partitioned Columnstore Index: Missing';

-- Check table temporal status
IF EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID('dbo.GL41') AND temporal_type = 0)
    PRINT '✅ GL41 Table: Non-temporal (optimal for columnstore)'
ELSE
    PRINT '⚠️ GL41 Table: Still temporal';

-- Partition count
SELECT @sql = CAST(COUNT(*) AS NVARCHAR(10))
FROM sys.partitions
WHERE object_id = OBJECT_ID('dbo.GL41') AND index_id IN (
    SELECT index_id FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.GL41') AND name = 'NCCI_GL41_Partitioned'
);

IF @sql IS NOT NULL AND @sql > 0
    PRINT '✅ Active Partitions: ' + @sql
ELSE
    PRINT '⚠️ Partition count could not be determined';

PRINT '';
PRINT '🎉 GL41 Partitioned Columnstore implementation completed!';
PRINT '📊 Benefits:';
PRINT '   • 5-10x compression ratio';
PRINT '   • Optimized analytical queries';
PRINT '   • Partition elimination for date ranges';
PRINT '   • Mixed OLTP/OLAP workload support';
PRINT '';
PRINT '📋 Next Steps:';
PRINT '   1. Test query performance';
PRINT '   2. Monitor compression ratios';
PRINT '   3. Plan partition boundary management';
PRINT '   4. Setup partition switching for ETL';
