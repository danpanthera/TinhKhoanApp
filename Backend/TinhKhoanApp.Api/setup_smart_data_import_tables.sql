-- =================================================================
-- 🚀 COMPLETE TEMPORAL TABLES + COLUMNSTORE INDEXES SETUP SCRIPT
-- Thiết lập đầy đủ các bảng dữ liệu với Temporal Tables và Columnstore Indexes
-- =================================================================

-- Enable system versioning for database if not already enabled
-- (This script assumes we have necessary permissions)

PRINT '📋 Starting Complete Temporal Tables + Columnstore Setup...';

-- =================================================================
-- 📊 CHECK CURRENT STATE
-- =================================================================
PRINT '📊 Checking current state of data tables...';

SELECT
    t.name AS TableName,
    CASE
        WHEN t.temporal_type_desc = 'SYSTEM_VERSIONED_TEMPORAL_TABLE' THEN 'Temporal'
        WHEN t.temporal_type_desc = 'HISTORY_TABLE' THEN 'History'
        ELSE 'Regular'
    END AS TableType,
    h.name AS HistoryTableName,
    CASE
        WHEN EXISTS (
            SELECT 1 FROM sys.indexes i
            WHERE i.object_id = t.object_id
            AND i.type IN (5, 6) -- COLUMNSTORE INDEX
        ) THEN 'Has Columnstore'
        ELSE 'No Columnstore'
    END AS ColumnstoreStatus
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
ORDER BY t.name;

-- =================================================================
-- 🔧 SETUP DP01_New (DP01 DATA TABLE)
-- =================================================================
PRINT '🔧 Setting up DP01_New table...';

-- Create main table if not exists
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DP01_New')
BEGIN
    PRINT '  Creating DP01_New table...';
    CREATE TABLE [DP01_New] (
        [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [NgayDL] nvarchar(10) NOT NULL,
        [FileName] nvarchar(255) NULL,
        [CreatedDate] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [MA_CN] nvarchar(20) NULL,
        [MA_PGD] nvarchar(20) NULL,
        [TAI_KHOAN_HACH_TOAN] nvarchar(50) NULL,
        [CURRENT_BALANCE] decimal(18,2) NULL,
        [NGAY_MO_TK] datetime2 NULL,
        [LOAI_TIEN_GUI] nvarchar(50) NULL,
        [KY_HAN] int NULL,
        [LAI_SUAT] decimal(5,2) NULL,
        [TRANG_THAI] nvarchar(20) NULL,

        -- Temporal columns
        [ValidFrom] datetime2 GENERATED ALWAYS AS ROW START NOT NULL,
        [ValidTo] datetime2 GENERATED ALWAYS AS ROW END NOT NULL,
        PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [DP01_New_History]));
END

-- Add columnstore index if not exists
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('DP01_New') AND type IN (5, 6))
BEGIN
    PRINT '  Adding columnstore index to DP01_New...';
    CREATE CLUSTERED COLUMNSTORE INDEX [CCI_DP01_New] ON [DP01_New];
END

-- =================================================================
-- 🔧 SETUP LN01 TABLE
-- =================================================================
PRINT '🔧 Setting up LN01 table...';

-- Create main table if not exists
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LN01')
BEGIN
    PRINT '  Creating LN01 table...';
    CREATE TABLE [LN01] (
        [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [NgayDL] nvarchar(10) NOT NULL,
        [FileName] nvarchar(255) NULL,
        [CreatedDate] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [MA_CN] nvarchar(20) NULL,
        [MA_PGD] nvarchar(20) NULL,
        [MA_KH] nvarchar(50) NULL,
        [SO_HD_CHO_VAY] nvarchar(50) NULL,
        [LOAI_HINH_CHO_VAY] nvarchar(50) NULL,
        [SO_TIEN_CHO_VAY] decimal(18,2) NULL,
        [DU_NO_GOC] decimal(18,2) NULL,
        [LAI_SUAT_CHO_VAY] decimal(5,2) NULL,
        [NGAY_GIAI_NGAN] datetime2 NULL,
        [NGAY_DEN_HAN] datetime2 NULL,
        [TRANG_THAI] nvarchar(20) NULL,

        -- Temporal columns
        [ValidFrom] datetime2 GENERATED ALWAYS AS ROW START NOT NULL,
        [ValidTo] datetime2 GENERATED ALWAYS AS ROW END NOT NULL,
        PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [LN01_History]));
END

-- Add columnstore index if not exists
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('LN01') AND type IN (5, 6))
BEGIN
    PRINT '  Adding columnstore index to LN01...';
    CREATE CLUSTERED COLUMNSTORE INDEX [CCI_LN01] ON [LN01];
END

-- =================================================================
-- 🔧 SETUP LN03 TABLE
-- =================================================================
PRINT '🔧 Setting up LN03 table...';

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LN03')
BEGIN
    PRINT '  Creating LN03 table...';
    CREATE TABLE [LN03] (
        [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [NgayDL] nvarchar(10) NOT NULL,
        [FileName] nvarchar(255) NULL,
        [CreatedDate] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [MA_CN] nvarchar(20) NULL,
        [MA_PGD] nvarchar(20) NULL,
        [MA_KH] nvarchar(50) NULL,
        [SO_HD_CHO_VAY] nvarchar(50) NULL,
        [NHOM_NO] nvarchar(20) NULL,
        [DU_NO_GOC] decimal(18,2) NULL,
        [DU_NO_LAI] decimal(18,2) NULL,
        [SO_NGAY_QUA_HAN] int NULL,
        [TY_LE_DU_PHONG] decimal(5,2) NULL,
        [SO_TIEN_DU_PHONG] decimal(18,2) NULL,
        [NGAY_PHAN_LOAI_NO] datetime2 NULL,
        [TRANG_THAI] nvarchar(20) NULL,

        -- Temporal columns
        [ValidFrom] datetime2 GENERATED ALWAYS AS ROW START NOT NULL,
        [ValidTo] datetime2 GENERATED ALWAYS AS ROW END NOT NULL,
        PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [LN03_History]));
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('LN03') AND type IN (5, 6))
BEGIN
    PRINT '  Adding columnstore index to LN03...';
    CREATE CLUSTERED COLUMNSTORE INDEX [CCI_LN03] ON [LN03];
END

-- =================================================================
-- 🔧 SETUP GL01 TABLE
-- =================================================================
PRINT '🔧 Setting up GL01 table...';

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GL01')
BEGIN
    PRINT '  Creating GL01 table...';
    CREATE TABLE [GL01] (
        [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [NgayDL] nvarchar(10) NOT NULL,
        [FileName] nvarchar(255) NULL,
        [CreatedDate] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [MA_CN] nvarchar(20) NULL,
        [MA_PGD] nvarchar(20) NULL,
        [MA_TK_HT] nvarchar(20) NULL,
        [TEN_TK_HT] nvarchar(255) NULL,
        [SO_DU_DAU_KY] decimal(18,2) NULL,
        [PHAT_SINH_NO] decimal(18,2) NULL,
        [PHAT_SINH_CO] decimal(18,2) NULL,
        [SO_DU_CUOI_KY] decimal(18,2) NULL,

        -- Temporal columns
        [ValidFrom] datetime2 GENERATED ALWAYS AS ROW START NOT NULL,
        [ValidTo] datetime2 GENERATED ALWAYS AS ROW END NOT NULL,
        PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [GL01_History]));
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('GL01') AND type IN (5, 6))
BEGIN
    PRINT '  Adding columnstore index to GL01...';
    CREATE CLUSTERED COLUMNSTORE INDEX [CCI_GL01] ON [GL01];
END

-- =================================================================
-- 🔧 SETUP GL41 TABLE
-- =================================================================
PRINT '🔧 Setting up GL41 table...';

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GL41')
BEGIN
    PRINT '  Creating GL41 table...';
    CREATE TABLE [GL41] (
        [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [NgayDL] nvarchar(10) NOT NULL,
        [FileName] nvarchar(255) NULL,
        [CreatedDate] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [MA_CN] nvarchar(20) NULL,
        [MA_PGD] nvarchar(20) NULL,
        [SO_TK] nvarchar(50) NULL,
        [TEN_TK] nvarchar(255) NULL,
        [SO_DU] decimal(18,2) NULL,
        [LOAI_TK] nvarchar(50) NULL,
        [NGAY_MO_TK] datetime2 NULL,
        [TRANG_THAI] nvarchar(20) NULL,

        -- Temporal columns
        [ValidFrom] datetime2 GENERATED ALWAYS AS ROW START NOT NULL,
        [ValidTo] datetime2 GENERATED ALWAYS AS ROW END NOT NULL,
        PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [GL41_History]));
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('GL41') AND type IN (5, 6))
BEGIN
    PRINT '  Adding columnstore index to GL41...';
    CREATE CLUSTERED COLUMNSTORE INDEX [CCI_GL41] ON [GL41];
END

-- =================================================================
-- 🔧 SETUP DB01 TABLE
-- =================================================================
PRINT '🔧 Setting up DB01 table...';

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DB01')
BEGIN
    PRINT '  Creating DB01 table...';
    CREATE TABLE [DB01] (
        [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [NgayDL] nvarchar(10) NOT NULL,
        [FileName] nvarchar(255) NULL,
        [CreatedDate] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [MA_CN] nvarchar(20) NULL,
        [MA_PGD] nvarchar(20) NULL,
        [MA_KH] nvarchar(50) NULL,
        [SO_HD_TSDB] nvarchar(50) NULL,
        [LOAI_TSDB] nvarchar(50) NULL,
        [GIA_TRI_TSDB] decimal(18,2) NULL,
        [GIA_TRI_DINH_GIA] decimal(18,2) NULL,
        [NGAY_DINH_GIA] datetime2 NULL,
        [TRANG_THAI] nvarchar(20) NULL,

        -- Temporal columns
        [ValidFrom] datetime2 GENERATED ALWAYS AS ROW START NOT NULL,
        [ValidTo] datetime2 GENERATED ALWAYS AS ROW END NOT NULL,
        PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [DB01_History]));
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('DB01') AND type IN (5, 6))
BEGIN
    PRINT '  Adding columnstore index to DB01...';
    CREATE CLUSTERED COLUMNSTORE INDEX [CCI_DB01] ON [DB01];
END

-- =================================================================
-- 🔧 SETUP DPDA TABLE
-- =================================================================
PRINT '🔧 Setting up DPDA table...';

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DPDA')
BEGIN
    PRINT '  Creating DPDA table...';
    CREATE TABLE [DPDA] (
        [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [NgayDL] nvarchar(10) NOT NULL,
        [FileName] nvarchar(255) NULL,
        [CreatedDate] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [MA_CN] nvarchar(20) NULL,
        [MA_PGD] nvarchar(20) NULL,
        [MA_KH] nvarchar(50) NULL,
        [SO_THE] nvarchar(20) NULL,
        [LOAI_THE] nvarchar(50) NULL,
        [NGAY_PHAT_HANH] datetime2 NULL,
        [NGAY_HET_HAN] datetime2 NULL,
        [TRANG_THAI] nvarchar(20) NULL,
        [PHI_THUONG_NIEN] decimal(18,2) NULL,

        -- Temporal columns
        [ValidFrom] datetime2 GENERATED ALWAYS AS ROW START NOT NULL,
        [ValidTo] datetime2 GENERATED ALWAYS AS ROW END NOT NULL,
        PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [DPDA_History]));
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('DPDA') AND type IN (5, 6))
BEGIN
    PRINT '  Adding columnstore index to DPDA...';
    CREATE CLUSTERED COLUMNSTORE INDEX [CCI_DPDA] ON [DPDA];
END

-- =================================================================
-- 🔧 SETUP EI01 TABLE
-- =================================================================
PRINT '🔧 Setting up EI01 table...';

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'EI01')
BEGIN
    PRINT '  Creating EI01 table...';
    CREATE TABLE [EI01] (
        [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [NgayDL] nvarchar(10) NOT NULL,
        [FileName] nvarchar(255) NULL,
        [CreatedDate] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [MA_CN] nvarchar(20) NULL,
        [MA_PGD] nvarchar(20) NULL,
        [MA_TK_HT] nvarchar(20) NULL,
        [TEN_TK_HT] nvarchar(255) NULL,
        [LOAI_THU_NHAP] nvarchar(50) NULL,
        [SO_TIEN_THU_NHAP] decimal(18,2) NULL,
        [NGAY_PHAT_SINH] datetime2 NULL,
        [DIEN_GIAI] nvarchar(500) NULL,

        -- Temporal columns
        [ValidFrom] datetime2 GENERATED ALWAYS AS ROW START NOT NULL,
        [ValidTo] datetime2 GENERATED ALWAYS AS ROW END NOT NULL,
        PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [EI01_History]));
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('EI01') AND type IN (5, 6))
BEGIN
    PRINT '  Adding columnstore index to EI01...';
    CREATE CLUSTERED COLUMNSTORE INDEX [CCI_EI01] ON [EI01];
END

-- =================================================================
-- =================================================================
-- 🔧 SETUP RR01 TABLE
-- =================================================================
PRINT '🔧 Setting up RR01 table...';

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RR01')
BEGIN
    PRINT '  Creating RR01 table...';
    CREATE TABLE [RR01] (
        [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [NgayDL] nvarchar(10) NOT NULL,
        [FileName] nvarchar(255) NULL,
        [CreatedDate] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [MA_CN] nvarchar(20) NULL,
        [MA_PGD] nvarchar(20) NULL,
        [LOAI_TY_LE] nvarchar(50) NULL,
        [TEN_CHI_TIEU] nvarchar(255) NULL,
        [GIA_TRI_TY_LE] decimal(18,2) NULL,
        [DON_VI_TINH] nvarchar(20) NULL,
        [NGAY_AP_DUNG] datetime2 NULL,
        [GHI_CHU] nvarchar(500) NULL,

        -- Temporal columns
        [ValidFrom] datetime2 GENERATED ALWAYS AS ROW START NOT NULL,
        [ValidTo] datetime2 GENERATED ALWAYS AS ROW END NOT NULL,
        PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [RR01_History]));
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('RR01') AND type IN (5, 6))
BEGIN
    PRINT '  Adding columnstore index to RR01...';
    CREATE CLUSTERED COLUMNSTORE INDEX [CCI_RR01] ON [RR01];
END

-- =================================================================
--  FINAL VERIFICATION
-- =================================================================
PRINT '📊 Final verification of all tables...';

SELECT
    t.name AS TableName,
    CASE
        WHEN t.temporal_type_desc = 'SYSTEM_VERSIONED_TEMPORAL_TABLE' THEN '✅ Temporal'
        WHEN t.temporal_type_desc = 'HISTORY_TABLE' THEN '📚 History'
        ELSE '❌ Regular'
    END AS TableType,
    h.name AS HistoryTableName,
    CASE
        WHEN EXISTS (
            SELECT 1 FROM sys.indexes i
            WHERE i.object_id = t.object_id
            AND i.type IN (5, 6) -- COLUMNSTORE INDEX
        ) THEN '✅ Has Columnstore'
        ELSE '❌ No Columnstore'
    END AS ColumnstoreStatus
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
ORDER BY t.name;

-- =================================================================
-- 🎯 COMPLETION SUMMARY
-- =================================================================
DECLARE @TotalTables INT = 12;
DECLARE @TemporalTables INT;
DECLARE @ColumnstoreTables INT;

SELECT @TemporalTables = COUNT(*)
FROM sys.tables t
WHERE t.name IN ('DP01_New', 'LN01', 'LN02', 'LN03', 'GL01', 'GL41', 'DB01', 'DPDA', 'EI01', 'KH03', 'RR01')
AND t.temporal_type_desc = 'SYSTEM_VERSIONED_TEMPORAL_TABLE';

SELECT @ColumnstoreTables = COUNT(DISTINCT t.name)
FROM sys.tables t
INNER JOIN sys.indexes i ON t.object_id = i.object_id
WHERE t.name IN ('DP01_New', 'LN01', 'LN02', 'LN03', 'GL01', 'GL41', 'DB01', 'DPDA', 'EI01', 'KH03', 'RR01')
AND i.type IN (5, 6);

PRINT '';
PRINT '🎯 SETUP COMPLETION SUMMARY:';
PRINT '================================';
PRINT 'Total Data Tables: ' + CAST(@TotalTables AS NVARCHAR(10));
PRINT 'Temporal Tables Configured: ' + CAST(@TemporalTables AS NVARCHAR(10)) + '/' + CAST(@TotalTables AS NVARCHAR(10));
PRINT 'Columnstore Indexes Created: ' + CAST(@ColumnstoreTables AS NVARCHAR(10)) + '/' + CAST(@TotalTables AS NVARCHAR(10));
PRINT '';

IF @TemporalTables = @TotalTables AND @ColumnstoreTables = @TotalTables
BEGIN
    PRINT '✅ SUCCESS: All temporal tables and columnstore indexes have been configured correctly!';
    PRINT '🚀 The Smart Data Import system is ready to use.';
END
ELSE
BEGIN
    PRINT '⚠️  WARNING: Some tables may not be fully configured.';
    PRINT '   Please check the verification results above.';
END

PRINT '';
PRINT '📋 Smart Data Import Ready! Files can now be automatically routed to:';
PRINT '   • Files with *DP01* → DP01_New table';
PRINT '   • Files with *LN01* → LN01 table';
PRINT '   • Files with *LN02* → LN02 table';
PRINT '   • Files with *LN03* → LN03 table';
PRINT '   • Files with *GL01* → GL01 table';
PRINT '   • Files with *GL41* → GL41 table';
PRINT '   • Files with *DB01* → DB01 table';
PRINT '   • Files with *DPDA* → DPDA table';
PRINT '   • Files with *EI01* → EI01 table';
PRINT '   • Files with *KH03* → KH03 table';
PRINT '   • Files with *RR01* → RR01 table';
PRINT '';
PRINT '🎉 Setup Complete!';
