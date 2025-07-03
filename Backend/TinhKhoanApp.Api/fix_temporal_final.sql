-- =======================================
-- 🔧 SCRIPT TẠO HISTORY TABLES ĐÚNG CHUẨN VÀ KÍCH HOẠT TEMPORAL
-- =======================================

USE TinhKhoanDB;
GO

PRINT '🔧 Creating proper history tables and enabling temporal...';

-- =======================================
-- 🗄️ TẠO HISTORY TABLES CHO GAHR26 VÀ GL41
-- =======================================

-- Create GAHR26_History table if not exist
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GAHR26_History')
BEGIN
    PRINT '🔧 Creating GAHR26_History table...';
    CREATE TABLE [GAHR26_History] (
        [Id] int NOT NULL,
        [BusinessKey] nvarchar(500) NOT NULL,
        [EffectiveDate] datetime2 NOT NULL,
        [ExpiryDate] datetime2 NULL,
        [IsCurrent] bit NOT NULL,
        [RowVersion] int NOT NULL,
        [ImportId] nvarchar(100) NOT NULL,
        [StatementDate] datetime2 NOT NULL,
        [ProcessedDate] datetime2 NOT NULL,
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
        [SysStartTime] datetime2 NOT NULL,
        [SysEndTime] datetime2 NOT NULL
    );
    PRINT '✅ GAHR26_History table created.';
END

-- Create GL41_History table if not exist
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GL41_History')
BEGIN
    PRINT '🔧 Creating GL41_History table...';
    CREATE TABLE [GL41_History] (
        [Id] int NOT NULL,
        [BusinessKey] nvarchar(500) NOT NULL,
        [EffectiveDate] datetime2 NOT NULL,
        [ExpiryDate] datetime2 NULL,
        [IsCurrent] bit NOT NULL,
        [RowVersion] int NOT NULL,
        [ImportId] nvarchar(100) NOT NULL,
        [StatementDate] datetime2 NOT NULL,
        [ProcessedDate] datetime2 NOT NULL,
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
        [SysStartTime] datetime2 NOT NULL,
        [SysEndTime] datetime2 NOT NULL
    );
    PRINT '✅ GL41_History table created.';
END

-- =======================================
-- 🔄 FIX EXISTING HISTORY TABLES (REMOVE IDENTITY, CONSTRAINTS)
-- =======================================

PRINT '🔄 Fixing existing history tables...';

-- Fix DPDA_History table
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DPDA_History')
BEGIN
    PRINT '🔧 Fixing DPDA_History table...';

    -- Create new history table without IDENTITY
    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DPDA_History_New')
    BEGIN
        SELECT TOP 0
            [Id], [BusinessKey], [EffectiveDate], [ExpiryDate], [IsCurrent], [RowVersion],
            [ImportId], [StatementDate], [ProcessedDate], [DataHash],
            [MaKhachHang], [TenKhachHang], [LoaiThe], [SoThe], [NgayPhatHanh], [NgayHetHan],
            [TrangThaiThe], [MaChiNhanh], [TenChiNhanh], [AdditionalData],
            CAST('1900-01-01' AS datetime2) AS [SysStartTime],
            CAST('9999-12-31' AS datetime2) AS [SysEndTime]
        INTO [DPDA_History_New]
        FROM [DPDA_History];

        -- Modify the Id column to remove identity
        ALTER TABLE [DPDA_History_New] ALTER COLUMN [Id] int NOT NULL;
    END

    -- Drop old history table and rename
    DROP TABLE [DPDA_History];
    EXEC sp_rename 'DPDA_History_New', 'DPDA_History';
    PRINT '✅ DPDA_History table fixed.';
END

-- Fix EI01_History table
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'EI01_History')
BEGIN
    PRINT '🔧 Fixing EI01_History table...';

    -- Create new history table without IDENTITY
    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'EI01_History_New')
    BEGIN
        SELECT TOP 0
            [Id], [BusinessKey], [EffectiveDate], [ExpiryDate], [IsCurrent], [RowVersion],
            [ImportId], [StatementDate], [ProcessedDate], [DataHash],
            [MaNhanVien], [TenNhanVien], [MaChiNhanh], [TenChiNhanh], [ChucVu], [PhongBan],
            [NgayVaoLam], [TrangThaiLamViec], [SoGioLamViec], [TyLeHoaHong], [AdditionalData],
            CAST('1900-01-01' AS datetime2) AS [SysStartTime],
            CAST('9999-12-31' AS datetime2) AS [SysEndTime]
        INTO [EI01_History_New]
        FROM [EI01_History];

        -- Modify the Id column to remove identity
        ALTER TABLE [EI01_History_New] ALTER COLUMN [Id] int NOT NULL;
    END

    -- Drop old history table and rename
    DROP TABLE [EI01_History];
    EXEC sp_rename 'EI01_History_New', 'EI01_History';
    PRINT '✅ EI01_History table fixed.';
END

-- =======================================
-- 🚀 ENABLE TEMPORAL TABLES
-- =======================================

PRINT '🚀 Enabling temporal configuration...';

-- Enable temporal for GAHR26
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GAHR26' AND temporal_type = 0)
BEGIN
    PRINT '🔧 Configuring temporal for GAHR26...';

    -- Add temporal columns
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('GAHR26') AND name = 'SysStartTime')
    BEGIN
        ALTER TABLE [GAHR26] ADD
            [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT '1900-01-01 00:00:00.0000000',
            [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
            PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
    END

    -- Enable system versioning
    ALTER TABLE [GAHR26] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[GAHR26_History]));
    PRINT '✅ GAHR26 temporal configuration completed.';
END

-- Enable temporal for GL41
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GL41' AND temporal_type = 0)
BEGIN
    PRINT '🔧 Configuring temporal for GL41...';

    -- Add temporal columns
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('GL41') AND name = 'SysStartTime')
    BEGIN
        ALTER TABLE [GL41] ADD
            [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT '1900-01-01 00:00:00.0000000',
            [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
            PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
    END

    -- Enable system versioning
    ALTER TABLE [GL41] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[GL41_History]));
    PRINT '✅ GL41 temporal configuration completed.';
END

-- Enable temporal for DPDA
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DPDA' AND temporal_type = 0)
BEGIN
    PRINT '🔧 Configuring temporal for DPDA...';

    -- Add temporal columns
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('DPDA') AND name = 'SysStartTime')
    BEGIN
        ALTER TABLE [DPDA] ADD
            [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT '1900-01-01 00:00:00.0000000',
            [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
            PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
    END

    -- Enable system versioning
    ALTER TABLE [DPDA] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[DPDA_History]));
    PRINT '✅ DPDA temporal configuration completed.';
END

-- Enable temporal for EI01
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'EI01' AND temporal_type = 0)
BEGIN
    PRINT '🔧 Configuring temporal for EI01...';

    -- Add temporal columns
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('EI01') AND name = 'SysStartTime')
    BEGIN
        ALTER TABLE [EI01] ADD
            [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT '1900-01-01 00:00:00.0000000',
            [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
            PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
    END

    -- Enable system versioning
    ALTER TABLE [EI01] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[EI01_History]));
    PRINT '✅ EI01 temporal configuration completed.';
END

-- =======================================
-- 📊 CREATE COLUMNSTORE INDEXES
-- =======================================

PRINT '📊 Creating Columnstore Indexes...';

-- List of history tables to add columnstore indexes
DECLARE @history_tables TABLE (TableName NVARCHAR(128));
INSERT INTO @history_tables VALUES
('7800_DT_KHKD1_History'),
('DPDA_History'),
('EI01_History'),
('GAHR26_History'),
('GL41_History');

DECLARE @table NVARCHAR(128);
DECLARE @sql NVARCHAR(MAX);

DECLARE table_cursor CURSOR FOR
SELECT TableName FROM @history_tables
WHERE EXISTS (SELECT 1 FROM sys.tables WHERE name = TableName)
  AND NOT EXISTS (
      SELECT 1 FROM sys.indexes i
      INNER JOIN sys.tables t ON i.object_id = t.object_id
      WHERE t.name = TableName AND i.type IN (5, 6)
  );

OPEN table_cursor;
FETCH NEXT FROM table_cursor INTO @table;

WHILE @@FETCH_STATUS = 0
BEGIN
    BEGIN TRY
        SET @sql = 'CREATE CLUSTERED COLUMNSTORE INDEX IX_' + @table + '_ColumnStore ON [' + @table + '];';
        EXEC sp_executesql @sql;
        PRINT '✅ Created Columnstore Index for: ' + @table;
    END TRY
    BEGIN CATCH
        PRINT '⚠️ Warning: Could not create columnstore index for ' + @table + ': ' + ERROR_MESSAGE();
    END CATCH

    FETCH NEXT FROM table_cursor INTO @table;
END

CLOSE table_cursor;
DEALLOCATE table_cursor;

-- =======================================
-- 📋 FINAL VERIFICATION
-- =======================================

PRINT '📋 Final verification of all tables...';

SELECT
    t.name AS TableName,
    t.temporal_type_desc AS TemporalType,
    h.name AS HistoryTableName,
    CASE
        WHEN EXISTS (
            SELECT 1 FROM sys.indexes i
            WHERE i.object_id = h.object_id
            AND i.type IN (5, 6)
        ) THEN '✅ Has Columnstore'
        ELSE '❌ Missing Columnstore'
    END AS ColumnstoreStatus,
    CASE
        WHEN t.temporal_type = 2 AND h.object_id IS NOT NULL THEN '✅ Complete'
        WHEN t.temporal_type = 2 THEN '⚠️ Temporal but no history'
        WHEN t.object_id IS NOT NULL THEN '❌ No temporal'
        ELSE '❌ Missing table'
    END AS Status
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
WHERE t.name IN ('7800_DT_KHKD1', 'DPDA', 'EI01', 'GAHR26', 'GL41')
ORDER BY t.name;

PRINT '🚀 Temporal Tables + Columnstore Indexes setup completed!';

GO
