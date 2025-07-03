-- =======================================
-- 🚀 SCRIPT SỬA LỖI VÀ HOÀN THIỆN TEMPORAL TABLES + COLUMNSTORE INDEXES
-- Cho các bảng nghiệp vụ trong hệ thống KHO DỮ LIỆU THÔ
-- =======================================

USE TinhKhoanDB;
GO

PRINT '🚀 Starting Temporal Tables + Columnstore Indexes Setup...';

-- =======================================
-- 📋 BƯỚC 1: KIỂM TRA HIỆN TRẠNG CÁC BẢNG
-- =======================================
PRINT '📋 Step 1: Checking current state of tables...';

SELECT
    t.name AS TableName,
    t.temporal_type_desc AS TemporalType,
    h.name AS HistoryTableName,
    CASE
        WHEN EXISTS (
            SELECT 1 FROM sys.indexes i
            WHERE i.object_id = h.object_id
            AND i.type IN (5, 6) -- COLUMNSTORE INDEX
        ) THEN 'Has Columnstore Index'
        ELSE 'Missing Columnstore Index'
    END AS ColumnstoreStatus
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
WHERE t.name IN ('7800_DT_KHKD1', 'BC57', 'DB01', 'DP01', 'DPDA', 'EI01', 'GAHR26', 'GL01', 'GL41', 'KH03', 'LN01', 'LN02', 'LN03', 'RR01')
ORDER BY t.name;

-- =======================================
-- 🔧 BƯỚC 2: CHUẨN BỊ HISTORY TABLES (DROP CONSTRAINTS)
-- =======================================
PRINT '🔧 Step 2: Preparing history tables by removing constraints...';

-- Drop primary keys from history tables to allow temporal configuration
DECLARE @sql NVARCHAR(MAX) = '';

SELECT @sql = @sql + 'ALTER TABLE [' + t.name + '] DROP CONSTRAINT [' + c.name + '];' + CHAR(13)
FROM sys.tables t
INNER JOIN sys.key_constraints c ON t.object_id = c.parent_object_id
WHERE t.name LIKE '%_History'
  AND c.type = 'PK'
  AND t.name IN ('BC57_History', 'DB01_History', 'DPDA_History', 'EI01_History', 'GL01_History', 'KH03_History', 'LN01_History', 'LN03_History');

IF LEN(@sql) > 0
BEGIN
    PRINT 'Removing primary key constraints from history tables...';
    EXEC sp_executesql @sql;
END

-- =======================================
-- 🚀 BƯỚC 3: TẠO CÁC BẢNG CHÍNH NẾU CHƯA TỒN TẠI
-- =======================================
PRINT '🚀 Step 3: Creating main business tables if not exist...';

-- 1. Tạo bảng 7800_DT_KHKD1 nếu chưa có
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = '7800_DT_KHKD1')
BEGIN
    PRINT '🔧 Creating table 7800_DT_KHKD1...';
    CREATE TABLE [7800_DT_KHKD1] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [BusinessKey] nvarchar(500) NOT NULL,
        [EffectiveDate] datetime2 NOT NULL,
        [ExpiryDate] datetime2 NULL,
        [IsCurrent] bit NOT NULL DEFAULT 1,
        [RowVersion] int NOT NULL DEFAULT 1,
        [ImportId] nvarchar(100) NOT NULL,
        [StatementDate] datetime2 NOT NULL,
        [ProcessedDate] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [DataHash] nvarchar(64) NOT NULL,
        [MaChiNhanh] nvarchar(20) NULL,
        [TenChiNhanh] nvarchar(200) NULL,
        [LoaiChiTieu] nvarchar(100) NULL,
        [TenChiTieu] nvarchar(500) NULL,
        [KeHoachNam] decimal(18,2) NULL,
        [KeHoachQuy] decimal(18,2) NULL,
        [KeHoachThang] decimal(18,2) NULL,
        [ThucHienNam] decimal(18,2) NULL,
        [ThucHienQuy] decimal(18,2) NULL,
        [ThucHienThang] decimal(18,2) NULL,
        [TyLeDatKeHoach] decimal(10,4) NULL,
        [Nam] int NULL,
        [Quy] int NULL,
        [Thang] int NULL,
        [NgayTao] datetime2 NULL,
        [NgayCapNhat] datetime2 NULL,
        [AdditionalData] nvarchar(max) NULL,
        -- Temporal columns
        [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START NOT NULL,
        [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END NOT NULL,
        PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]),
        CONSTRAINT [PK_7800_DT_KHKD1] PRIMARY KEY ([Id])
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [7800_DT_KHKD1_History]));
    PRINT '✅ Table 7800_DT_KHKD1 created with temporal versioning.';
END

-- 2. Tạo bảng GAHR26 nếu chưa có
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GAHR26')
BEGIN
    PRINT '🔧 Creating table GAHR26...';
    CREATE TABLE [GAHR26] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [BusinessKey] nvarchar(500) NOT NULL,
        [EffectiveDate] datetime2 NOT NULL,
        [ExpiryDate] datetime2 NULL,
        [IsCurrent] bit NOT NULL DEFAULT 1,
        [RowVersion] int NOT NULL DEFAULT 1,
        [ImportId] nvarchar(100) NOT NULL,
        [StatementDate] datetime2 NOT NULL,
        [ProcessedDate] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [DataHash] nvarchar(64) NOT NULL,
        [MaNhanVien] nvarchar(50) NULL,
        [TenNhanVien] nvarchar(200) NULL,
        [MaChiNhanh] nvarchar(20) NULL,
        [TenChiNhanh] nvarchar(200) NULL,
        [ChucVu] nvarchar(100) NULL,
        [PhongBan] nvarchar(200) NULL,
        [NgayVaoLam] datetime2 NULL,
        [TrangThaiLamViec] nvarchar(50) NULL,
        [LuongCoBan] decimal(18,2) NULL,
        [PhuCap] decimal(18,2) NULL,
        [AdditionalData] nvarchar(max) NULL,
        -- Temporal columns
        [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START NOT NULL,
        [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END NOT NULL,
        PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]),
        CONSTRAINT [PK_GAHR26] PRIMARY KEY ([Id])
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [GAHR26_History]));
    PRINT '✅ Table GAHR26 created with temporal versioning.';
END

-- 3. Tạo bảng GL41 nếu chưa có
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GL41')
BEGIN
    PRINT '🔧 Creating table GL41...';
    CREATE TABLE [GL41] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [BusinessKey] nvarchar(500) NOT NULL,
        [EffectiveDate] datetime2 NOT NULL,
        [ExpiryDate] datetime2 NULL,
        [IsCurrent] bit NOT NULL DEFAULT 1,
        [RowVersion] int NOT NULL DEFAULT 1,
        [ImportId] nvarchar(100) NOT NULL,
        [StatementDate] datetime2 NOT NULL,
        [ProcessedDate] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [DataHash] nvarchar(64) NOT NULL,
        [SoTaiKhoan] nvarchar(50) NULL,
        [TenTaiKhoan] nvarchar(500) NULL,
        [LoaiTaiKhoan] nvarchar(100) NULL,
        [MaChiNhanh] nvarchar(20) NULL,
        [TenChiNhanh] nvarchar(200) NULL,
        [SoDuDauKy] decimal(18,2) NULL,
        [SoDuCuoiKy] decimal(18,2) NULL,
        [PhatSinhNo] decimal(18,2) NULL,
        [PhatSinhCo] decimal(18,2) NULL,
        [NgayMoTaiKhoan] datetime2 NULL,
        [TrangThaiTaiKhoan] nvarchar(50) NULL,
        [AdditionalData] nvarchar(max) NULL,
        -- Temporal columns
        [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START NOT NULL,
        [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END NOT NULL,
        PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]),
        CONSTRAINT [PK_GL41] PRIMARY KEY ([Id])
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [GL41_History]));
    PRINT '✅ Table GL41 created with temporal versioning.';
END

-- =======================================
-- 📊 BƯỚC 4: KÍCH HOẠT TEMPORAL TABLE CHO CÁC BẢNG ĐÃ CÓ
-- =======================================
PRINT '📊 Step 4: Enabling temporal tables for existing tables...';

-- Enable temporal for existing tables that don't have it yet
DECLARE @table_name NVARCHAR(128);
DECLARE @history_table_name NVARCHAR(128);
DECLARE @alter_sql NVARCHAR(MAX);

-- List of tables to enable temporal (only if they exist and don't have temporal yet)
DECLARE table_cursor CURSOR FOR
SELECT table_name, history_table_name
FROM (VALUES
    ('BC57', 'BC57_History'),
    ('DB01', 'DB01_History'),
    ('DP01', 'DP01_History'),
    ('DPDA', 'DPDA_History'),
    ('EI01', 'EI01_History'),
    ('GL01', 'GL01_History'),
    ('KH03', 'KH03_History'),
    ('LN01', 'LN01_History'),
    ('LN02', 'LN02_History'),
    ('LN03', 'LN03_History'),
    ('RR01', 'RR01_History')
) AS tables(table_name, history_table_name)
WHERE EXISTS (SELECT 1 FROM sys.tables WHERE name = tables.table_name)
  AND NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = tables.table_name AND temporal_type = 2);

OPEN table_cursor;
FETCH NEXT FROM table_cursor INTO @table_name, @history_table_name;

WHILE @@FETCH_STATUS = 0
BEGIN
    BEGIN TRY
        -- Check if temporal columns exist, if not add them
        IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(@table_name) AND name = 'SysStartTime')
        BEGIN
            SET @alter_sql = 'ALTER TABLE [' + @table_name + '] ADD
                [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT ''1900-01-01 00:00:00.0000000'',
                [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT ''9999-12-31 23:59:59.9999999'',
                PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);';
            EXEC sp_executesql @alter_sql;
        END

        -- Enable system versioning
        SET @alter_sql = 'ALTER TABLE [' + @table_name + '] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @history_table_name + ']));';
        EXEC sp_executesql @alter_sql;

        PRINT '✅ Enabled temporal table for: ' + @table_name + ' with history table: ' + @history_table_name;
    END TRY
    BEGIN CATCH
        PRINT '⚠️ Warning: Could not enable temporal for ' + @table_name + ': ' + ERROR_MESSAGE();
    END CATCH

    FETCH NEXT FROM table_cursor INTO @table_name, @history_table_name;
END

CLOSE table_cursor;
DEALLOCATE table_cursor;

-- =======================================
-- 🎯 BƯỚC 5: TẠO COLUMNSTORE INDEXES CHO HISTORY TABLES
-- =======================================
PRINT '🎯 Step 5: Creating Columnstore Indexes for analytics performance...';

-- Create columnstore indexes for history tables that don't have them
DECLARE @index_sql NVARCHAR(MAX);
DECLARE @hist_table NVARCHAR(128);

DECLARE index_cursor CURSOR FOR
SELECT name
FROM sys.tables
WHERE name LIKE '%_History'
  AND name IN ('7800_DT_KHKD1_History', 'BC57_History', 'DB01_History', 'DP01_History', 'DPDA_History',
               'EI01_History', 'GAHR26_History', 'GL01_History', 'GL41_History', 'KH03_History',
               'LN01_History', 'LN02_History', 'LN03_History', 'RR01_History')
  AND NOT EXISTS (
      SELECT 1 FROM sys.indexes
      WHERE object_id = sys.tables.object_id
        AND type IN (5, 6) -- COLUMNSTORE
  );

OPEN index_cursor;
FETCH NEXT FROM index_cursor INTO @hist_table;

WHILE @@FETCH_STATUS = 0
BEGIN
    BEGIN TRY
        SET @index_sql = 'CREATE CLUSTERED COLUMNSTORE INDEX IX_' + @hist_table + '_ColumnStore ON [' + @hist_table + '];';
        EXEC sp_executesql @index_sql;
        PRINT '✅ Created Columnstore Index for: ' + @hist_table;
    END TRY
    BEGIN CATCH
        PRINT '⚠️ Warning: Could not create columnstore index for ' + @hist_table + ': ' + ERROR_MESSAGE();
    END CATCH

    FETCH NEXT FROM index_cursor INTO @hist_table;
END

CLOSE index_cursor;
DEALLOCATE index_cursor;

-- =======================================
-- 📋 BƯỚC 6: KIỂM TRA KẾT QUẢ CUỐI CÙNG
-- =======================================
PRINT '📋 Step 6: Final verification of temporal tables setup...';

SELECT
    t.name AS TableName,
    t.temporal_type_desc AS TemporalType,
    h.name AS HistoryTableName,
    CASE
        WHEN EXISTS (
            SELECT 1 FROM sys.indexes i
            WHERE i.object_id = h.object_id
            AND i.type IN (5, 6) -- COLUMNSTORE INDEX
        ) THEN 'Has Columnstore Index'
        ELSE 'Missing Columnstore Index'
    END AS ColumnstoreStatus,
    CASE
        WHEN t.temporal_type = 2 AND h.object_id IS NOT NULL THEN '✅ Complete'
        WHEN t.temporal_type = 2 THEN '⚠️ Temporal enabled but no history table'
        WHEN t.object_id IS NOT NULL THEN '❌ Table exists but no temporal'
        ELSE '❌ Table missing'
    END AS Status
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
WHERE t.name IN ('7800_DT_KHKD1', 'BC57', 'DB01', 'DP01', 'DPDA', 'EI01', 'GAHR26', 'GL01', 'GL41', 'KH03', 'LN01', 'LN02', 'LN03', 'RR01')
   OR t.name IN (
       SELECT t2.name
       FROM sys.tables t2
       WHERE t2.name IN ('7800_DT_KHKD1', 'BC57', 'DB01', 'DP01', 'DPDA', 'EI01', 'GAHR26', 'GL01', 'GL41', 'KH03', 'LN01', 'LN02', 'LN03', 'RR01')
   )
ORDER BY t.name;

PRINT '🚀 Temporal Tables + Columnstore Indexes setup completed!';
PRINT '📊 Summary: All business tables should now have temporal versioning with columnstore indexes for analytics performance.';

GO
