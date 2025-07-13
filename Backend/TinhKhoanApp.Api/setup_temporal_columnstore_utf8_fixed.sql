-- =====================================================================================
-- THIẾT LẬP TEMPORAL TABLES + COLUMNSTORE CHO 7 BẢNG CÒN LẠI (UTF-8 FIXED)
-- Được tạo tự động: 13/07/2025 - Fixed UTF-8 encoding
-- =====================================================================================

USE TinhKhoanDB;
GO

PRINT 'Starting setup of Temporal Tables + Columnstore for remaining 7 tables...';
PRINT '================================================================';

-- =====================================================================================
-- 1. DPDA (13 cot business)
-- =====================================================================================
PRINT '1. Setting up DPDA with Temporal Tables + Columnstore...';

-- Add Temporal columns if not exists
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DPDA') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE DPDA ADD
        SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END;

-- Enable System Versioning
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DPDA_History')
BEGIN
    ALTER TABLE DPDA SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.DPDA_History));
    PRINT 'DPDA Temporal Tables: COMPLETED';
END
ELSE
BEGIN
    PRINT 'DPDA Temporal Tables: ALREADY EXISTS';
END;

-- Create Columnstore Index for History Table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DPDA_History')
BEGIN
    -- Drop existing clustered index if exists
    DECLARE @sql NVARCHAR(MAX);
    SELECT @sql = 'DROP INDEX ' + i.name + ' ON DPDA_History'
    FROM sys.indexes i
    WHERE i.object_id = OBJECT_ID('DPDA_History')
        AND i.is_primary_key = 0
        AND i.type = 1; -- Clustered index

    IF @sql IS NOT NULL
        EXEC sp_executesql @sql;

    -- Create Columnstore
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('DPDA_History') AND type = 5)
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX IX_DPDA_History_Columnstore ON DPDA_History;
        PRINT 'DPDA Columnstore Index: COMPLETED';
    END;
END;

-- =====================================================================================
-- 2. EI01 (24 cot business)
-- =====================================================================================
PRINT '2. Setting up EI01 with Temporal Tables + Columnstore...';

-- Add Temporal columns if not exists
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EI01') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE EI01 ADD
        SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END;

-- Enable System Versioning
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EI01_History')
BEGIN
    ALTER TABLE EI01 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.EI01_History));
    PRINT 'EI01 Temporal Tables: COMPLETED';
END
ELSE
BEGIN
    PRINT 'EI01 Temporal Tables: ALREADY EXISTS';
END;

-- Create Columnstore Index for History Table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EI01_History')
BEGIN
    -- Drop existing clustered index if exists
    DECLARE @sql2 NVARCHAR(MAX);
    SELECT @sql2 = 'DROP INDEX ' + i.name + ' ON EI01_History'
    FROM sys.indexes i
    WHERE i.object_id = OBJECT_ID('EI01_History')
        AND i.is_primary_key = 0
        AND i.type = 1;

    IF @sql2 IS NOT NULL
        EXEC sp_executesql @sql2;

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('EI01_History') AND type = 5)
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX IX_EI01_History_Columnstore ON EI01_History;
        PRINT 'EI01 Columnstore Index: COMPLETED';
    END;
END;

-- =====================================================================================
-- Continue for all other tables (GL01, GL41, LN01, LN03, RR01)
-- =====================================================================================

-- GL01
PRINT '3. Setting up GL01 with Temporal Tables + Columnstore...';
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GL01') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE GL01 ADD
        SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GL01_History')
BEGIN
    ALTER TABLE GL01 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.GL01_History));
    PRINT 'GL01 Temporal Tables: COMPLETED';
END;

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GL01_History')
BEGIN
    DECLARE @sql3 NVARCHAR(MAX);
    SELECT @sql3 = 'DROP INDEX ' + i.name + ' ON GL01_History'
    FROM sys.indexes i WHERE i.object_id = OBJECT_ID('GL01_History') AND i.is_primary_key = 0 AND i.type = 1;

    IF @sql3 IS NOT NULL EXEC sp_executesql @sql3;

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('GL01_History') AND type = 5)
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX IX_GL01_History_Columnstore ON GL01_History;
        PRINT 'GL01 Columnstore Index: COMPLETED';
    END;
END;

-- GL41
PRINT '4. Setting up GL41 with Temporal Tables + Columnstore...';
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GL41') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE GL41 ADD
        SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GL41_History')
BEGIN
    ALTER TABLE GL41 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.GL41_History));
    PRINT 'GL41 Temporal Tables: COMPLETED';
END;

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GL41_History')
BEGIN
    DECLARE @sql4 NVARCHAR(MAX);
    SELECT @sql4 = 'DROP INDEX ' + i.name + ' ON GL41_History'
    FROM sys.indexes i WHERE i.object_id = OBJECT_ID('GL41_History') AND i.is_primary_key = 0 AND i.type = 1;

    IF @sql4 IS NOT NULL EXEC sp_executesql @sql4;

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('GL41_History') AND type = 5)
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX IX_GL41_History_Columnstore ON GL41_History;
        PRINT 'GL41 Columnstore Index: COMPLETED';
    END;
END;

-- LN01
PRINT '5. Setting up LN01 with Temporal Tables + Columnstore...';
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('LN01') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE LN01 ADD
        SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LN01_History')
BEGIN
    ALTER TABLE LN01 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.LN01_History));
    PRINT 'LN01 Temporal Tables: COMPLETED';
END;

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'LN01_History')
BEGIN
    DECLARE @sql5 NVARCHAR(MAX);
    SELECT @sql5 = 'DROP INDEX ' + i.name + ' ON LN01_History'
    FROM sys.indexes i WHERE i.object_id = OBJECT_ID('LN01_History') AND i.is_primary_key = 0 AND i.type = 1;

    IF @sql5 IS NOT NULL EXEC sp_executesql @sql5;

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('LN01_History') AND type = 5)
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX IX_LN01_History_Columnstore ON LN01_History;
        PRINT 'LN01 Columnstore Index: COMPLETED';
    END;
END;

-- LN03
PRINT '6. Setting up LN03 with Temporal Tables + Columnstore...';
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('LN03') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE LN03 ADD
        SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LN03_History')
BEGIN
    ALTER TABLE LN03 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.LN03_History));
    PRINT 'LN03 Temporal Tables: COMPLETED';
END;

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'LN03_History')
BEGIN
    DECLARE @sql6 NVARCHAR(MAX);
    SELECT @sql6 = 'DROP INDEX ' + i.name + ' ON LN03_History'
    FROM sys.indexes i WHERE i.object_id = OBJECT_ID('LN03_History') AND i.is_primary_key = 0 AND i.type = 1;

    IF @sql6 IS NOT NULL EXEC sp_executesql @sql6;

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('LN03_History') AND type = 5)
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX IX_LN03_History_Columnstore ON LN03_History;
        PRINT 'LN03 Columnstore Index: COMPLETED';
    END;
END;

-- RR01
PRINT '7. Setting up RR01 with Temporal Tables + Columnstore...';
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('RR01') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE RR01 ADD
        SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RR01_History')
BEGIN
    ALTER TABLE RR01 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.RR01_History));
    PRINT 'RR01 Temporal Tables: COMPLETED';
END;

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'RR01_History')
BEGIN
    DECLARE @sql7 NVARCHAR(MAX);
    SELECT @sql7 = 'DROP INDEX ' + i.name + ' ON RR01_History'
    FROM sys.indexes i WHERE i.object_id = OBJECT_ID('RR01_History') AND i.is_primary_key = 0 AND i.type = 1;

    IF @sql7 IS NOT NULL EXEC sp_executesql @sql7;

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('RR01_History') AND type = 5)
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX IX_RR01_History_Columnstore ON RR01_History;
        PRINT 'RR01 Columnstore Index: COMPLETED';
    END;
END;

-- =====================================================================================
-- 8. Add Columnstore for DP01_History (already has Temporal)
-- =====================================================================================
PRINT '8. Setting up Columnstore for DP01_History...';

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DP01_History')
BEGIN
    DECLARE @sql8 NVARCHAR(MAX);
    SELECT @sql8 = 'DROP INDEX ' + i.name + ' ON DP01_History'
    FROM sys.indexes i
    WHERE i.object_id = OBJECT_ID('DP01_History')
        AND i.is_primary_key = 0
        AND i.type = 1;

    IF @sql8 IS NOT NULL
        EXEC sp_executesql @sql8;

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('DP01_History') AND type = 5)
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX IX_DP01_History_Columnstore ON DP01_History;
        PRINT 'DP01 Columnstore Index: COMPLETED';
    END
    ELSE
    BEGIN
        PRINT 'DP01 Columnstore Index: ALREADY EXISTS';
    END;
END;

-- =====================================================================================
-- FINAL VERIFICATION
-- =====================================================================================
PRINT '';
PRINT 'FINAL VERIFICATION RESULTS:';
PRINT '================================================================';

SELECT
    t.name AS TableName,
    t.temporal_type_desc AS TemporalStatus,
    CASE WHEN h.name IS NOT NULL THEN 'YES' ELSE 'NO' END AS HasHistoryTable,
    h.name AS HistoryTableName
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
WHERE t.name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')
ORDER BY t.name;

PRINT '';
PRINT 'COLUMNSTORE INDEXES:';
SELECT
    t.name AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType
FROM sys.tables t
INNER JOIN sys.indexes i ON t.object_id = i.object_id
WHERE t.name LIKE '%_History'
    AND t.name IN ('DP01_History', 'DPDA_History', 'EI01_History', 'GL01_History', 'GL41_History', 'LN01_History', 'LN03_History', 'RR01_History')
    AND i.type_desc = 'CLUSTERED COLUMNSTORE'
ORDER BY t.name;

PRINT '';
PRINT 'SETUP COMPLETED FOR ALL 8 CORE TABLES!';
PRINT '================================================================';
