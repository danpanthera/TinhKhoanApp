-- =============================================
-- LN01 Index Creation Script
-- Description: Optimized indexes for LN01 temporal table
-- Performance Focus: Date filtering, branch searches, customer lookups
-- =============================================

USE [tinhkhoan_db];
GO

PRINT 'Creating LN01 optimized indexes...';

-- 1. Primary clustered index on NGAY_DL (Date-first for temporal data)
-- This optimizes date-range queries which are the most common
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('LN01') AND name = 'IX_LN01_NGAY_DL_CLUSTERED')
BEGIN
    PRINT 'Creating clustered index on NGAY_DL...';
    CREATE CLUSTERED INDEX [IX_LN01_NGAY_DL_CLUSTERED] ON [dbo].[LN01]
    (
        [NGAY_DL] ASC
    ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);
END
ELSE
    PRINT 'Clustered index on NGAY_DL already exists.';

-- 2. Branch + Date composite index (Common query pattern)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('LN01') AND name = 'IX_LN01_BRCD_NGAY_DL')
BEGIN
    PRINT 'Creating composite index on BRCD + NGAY_DL...';
    CREATE NONCLUSTERED INDEX [IX_LN01_BRCD_NGAY_DL] ON [dbo].[LN01]
    (
        [BRCD] ASC,
        [NGAY_DL] ASC
    )
    INCLUDE ([CUSTSEQ], [CUSTNM], [TAI_KHOAN], [DU_NO], [DISBURSEMENT_AMOUNT])
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);
END
ELSE
    PRINT 'Composite index on BRCD + NGAY_DL already exists.';

-- 3. Customer sequence index (Customer lookups)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('LN01') AND name = 'IX_LN01_CUSTSEQ')
BEGIN
    PRINT 'Creating index on CUSTSEQ...';
    CREATE NONCLUSTERED INDEX [IX_LN01_CUSTSEQ] ON [dbo].[LN01]
    (
        [CUSTSEQ] ASC
    )
    INCLUDE ([NGAY_DL], [BRCD], [CUSTNM], [TAI_KHOAN], [DU_NO])
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);
END
ELSE
    PRINT 'Index on CUSTSEQ already exists.';

-- 4. Account number index (Account lookups)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('LN01') AND name = 'IX_LN01_TAI_KHOAN')
BEGIN
    PRINT 'Creating index on TAI_KHOAN...';
    CREATE NONCLUSTERED INDEX [IX_LN01_TAI_KHOAN] ON [dbo].[LN01]
    (
        [TAI_KHOAN] ASC
    )
    INCLUDE ([NGAY_DL], [BRCD], [CUSTSEQ], [CUSTNM], [DU_NO])
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);
END
ELSE
    PRINT 'Index on TAI_KHOAN already exists.';

-- 5. Debt group analysis index (Business reporting)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('LN01') AND name = 'IX_LN01_NHOM_NO_NGAY_DL')
BEGIN
    PRINT 'Creating index on NHOM_NO + NGAY_DL...';
    CREATE NONCLUSTERED INDEX [IX_LN01_NHOM_NO_NGAY_DL] ON [dbo].[LN01]
    (
        [NHOM_NO] ASC,
        [NGAY_DL] ASC
    )
    INCLUDE ([BRCD], [DU_NO], [DISBURSEMENT_AMOUNT], [INTEREST_AMOUNT])
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);
END
ELSE
    PRINT 'Index on NHOM_NO + NGAY_DL already exists.';

-- 6. Loan amount range index (Amount-based queries)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('LN01') AND name = 'IX_LN01_DU_NO_NGAY_DL')
BEGIN
    PRINT 'Creating index on DU_NO + NGAY_DL...';
    CREATE NONCLUSTERED INDEX [IX_LN01_DU_NO_NGAY_DL] ON [dbo].[LN01]
    (
        [DU_NO] ASC,
        [NGAY_DL] ASC
    )
    INCLUDE ([BRCD], [CUSTSEQ], [CUSTNM], [TAI_KHOAN])
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);
END
ELSE
    PRINT 'Index on DU_NO + NGAY_DL already exists.';

-- 7. Temporal table system columns index (History queries)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('LN01') AND name = 'IX_LN01_TEMPORAL_SYS')
BEGIN
    PRINT 'Creating index on temporal system columns...';
    CREATE NONCLUSTERED INDEX [IX_LN01_TEMPORAL_SYS] ON [dbo].[LN01]
    (
        [SysStartTime] ASC,
        [SysEndTime] ASC
    )
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);
END
ELSE
    PRINT 'Index on temporal system columns already exists.';

-- 8. Statistics update for better query optimization
PRINT 'Updating statistics for LN01 table...';
UPDATE STATISTICS [dbo].[LN01] WITH FULLSCAN;

-- Verification query
PRINT 'LN01 Index Summary:';
SELECT
    i.name AS IndexName,
    i.type_desc AS IndexType,
    i.is_unique AS IsUnique,
    STUFF((
        SELECT ', ' + c.name + ' (' + CASE WHEN ic.is_descending_key = 1 THEN 'DESC' ELSE 'ASC' END + ')'
        FROM sys.index_columns ic
        INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
        WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id AND ic.is_included_column = 0
        ORDER BY ic.key_ordinal
        FOR XML PATH('')
    ), 1, 2, '') AS KeyColumns,
    STUFF((
        SELECT ', ' + c.name
        FROM sys.index_columns ic
        INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
        WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id AND ic.is_included_column = 1
        FOR XML PATH('')
    ), 1, 2, '') AS IncludedColumns
FROM sys.indexes i
WHERE i.object_id = OBJECT_ID('LN01')
ORDER BY i.index_id;

PRINT 'LN01 indexes created successfully!';
GO
