-- =======================================
-- 🚀 SCRIPT HOÀN THIỆN TEMPORAL TABLES + COLUMNSTORE INDEXES
-- Cho các bảng còn thiếu trong hệ thống KHO DỮ LIỆU THÔ
-- =======================================

-- Script đã được sửa để tương thích với hệ thống hiện tại
-- Removed database alteration to avoid permission issues

-- =======================================
-- 📋 KIỂM TRA HIỆN TRẠNG CÁC BẢNG
-- =======================================
PRINT '📋 Checking current state of tables...';

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
-- 🚀 TẠO CÁC BẢNG CHÍNH NẾU CHƯA TỒN TẠI
-- =======================================

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
        CONSTRAINT [PK_7800_DT_KHKD1] PRIMARY KEY ([Id])
    );
END

-- 2. Tạo bảng DPDA nếu chưa có
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DPDA')
BEGIN
    PRINT '🔧 Creating table DPDA...';
    CREATE TABLE [DPDA] (
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
        [MaKhachHang] nvarchar(50) NULL,
        [TenKhachHang] nvarchar(500) NULL,
        [LoaiThe] nvarchar(100) NULL,
        [SoThe] nvarchar(50) NULL,
        [NgayPhatHanh] datetime2 NULL,
        [NgayHetHan] datetime2 NULL,
        [TrangThaiThe] nvarchar(50) NULL,
        [MaChiNhanh] nvarchar(20) NULL,
        [TenChiNhanh] nvarchar(200) NULL,
        [AdditionalData] nvarchar(max) NULL,
        CONSTRAINT [PK_DPDA] PRIMARY KEY ([Id])
    );
END

-- 3. Tạo bảng EI01 nếu chưa có
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'EI01')
BEGIN
    PRINT '🔧 Creating table EI01...';
    CREATE TABLE [EI01] (
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
        [MaKhachHang] nvarchar(50) NULL,
        [TenKhachHang] nvarchar(500) NULL,
        [SoTaiKhoan] nvarchar(50) NULL,
        [LoaiGiaoDich] nvarchar(100) NULL,
        [MaGiaoDich] nvarchar(100) NULL,
        [SoTien] decimal(18,2) NULL,
        [NgayGiaoDich] datetime2 NULL,
        [ThoiGianGiaoDich] datetime2 NULL,
        [TrangThaiGiaoDich] nvarchar(50) NULL,
        [NoiDungGiaoDich] nvarchar(1000) NULL,
        [MaChiNhanh] nvarchar(20) NULL,
        [TenChiNhanh] nvarchar(200) NULL,
        [Channel] nvarchar(50) NULL,
        [DeviceInfo] nvarchar(500) NULL,
        [AdditionalData] nvarchar(max) NULL,
        CONSTRAINT [PK_EI01] PRIMARY KEY ([Id])
    );
END

-- 4. Tạo bảng GAHR26 nếu chưa có
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
        [TenNhanVien] nvarchar(500) NULL,
        [CMND] nvarchar(20) NULL,
        [ChucVu] nvarchar(100) NULL,
        [MaChiNhanh] nvarchar(20) NULL,
        [TenChiNhanh] nvarchar(200) NULL,
        [PhongBan] nvarchar(100) NULL,
        [NgayVaoLam] datetime2 NULL,
        [NgaySinh] datetime2 NULL,
        [GioiTinh] nvarchar(10) NULL,
        [DiaChi] nvarchar(500) NULL,
        [DienThoai] nvarchar(20) NULL,
        [Email] nvarchar(100) NULL,
        [TrangThai] nvarchar(50) NULL,
        [LuongCoBan] decimal(18,2) NULL,
        [PhuCap] decimal(18,2) NULL,
        [NgayTao] datetime2 NULL,
        [NgayCapNhat] datetime2 NULL,
        [AdditionalData] nvarchar(max) NULL,
        CONSTRAINT [PK_GAHR26] PRIMARY KEY ([Id])
    );
END

-- 5. Tạo bảng GL41 nếu chưa có
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
        [SoButToan] nvarchar(50) NULL,
        [TaiKhoanKeToan] nvarchar(50) NULL,
        [TenTaiKhoan] nvarchar(500) NULL,
        [MaKhachHang] nvarchar(50) NULL,
        [TenKhachHang] nvarchar(500) NULL,
        [NgayGiaoDich] datetime2 NULL,
        [NgayHachToan] datetime2 NULL,
        [DienGiai] nvarchar(1000) NULL,
        [SoTienNo] decimal(18,2) NULL,
        [SoTienCo] decimal(18,2) NULL,
        [SoDuNo] decimal(18,2) NULL,
        [SoDuCo] decimal(18,2) NULL,
        [MaChiNhanh] nvarchar(20) NULL,
        [TenChiNhanh] nvarchar(200) NULL,
        [LoaiGiaoDich] nvarchar(100) NULL,
        [MaGiaoDichGoc] nvarchar(50) NULL,
        [NgayTao] datetime2 NULL,
        [NgayCapNhat] datetime2 NULL,
        [AdditionalData] nvarchar(max) NULL,
        CONSTRAINT [PK_GL41] PRIMARY KEY ([Id])
    );
END

-- =======================================
-- 🚀 KÍCH HOẠT TEMPORAL TABLES
-- =======================================

-- 1. Kích hoạt Temporal cho 7800_DT_KHKD1
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = '7800_DT_KHKD1' AND temporal_type = 0)
BEGIN
    PRINT '🚀 Enabling Temporal Table for 7800_DT_KHKD1...';

    -- Thêm period columns nếu chưa có
    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = 'SysStartTime' AND Object_ID = Object_ID('7800_DT_KHKD1'))
    BEGIN
        ALTER TABLE [7800_DT_KHKD1] ADD
            SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
            SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
    END

    -- Kích hoạt system versioning
    ALTER TABLE [7800_DT_KHKD1] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.[7800_DT_KHKD1_History]));
END

-- 2. Kích hoạt Temporal cho DPDA
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DPDA' AND temporal_type = 0)
BEGIN
    PRINT '🚀 Enabling Temporal Table for DPDA...';

    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = 'SysStartTime' AND Object_ID = Object_ID('DPDA'))
    BEGIN
        ALTER TABLE [DPDA] ADD
            SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
            SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
    END

    ALTER TABLE [DPDA] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.DPDA_History));
END

-- 3. Kích hoạt Temporal cho EI01
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'EI01' AND temporal_type = 0)
BEGIN
    PRINT '🚀 Enabling Temporal Table for EI01...';

    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = 'SysStartTime' AND Object_ID = Object_ID('EI01'))
    BEGIN
        ALTER TABLE [EI01] ADD
            SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
            SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
    END

    ALTER TABLE [EI01] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.EI01_History));
END

-- 4. Kích hoạt Temporal cho GAHR26
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GAHR26' AND temporal_type = 0)
BEGIN
    PRINT '🚀 Enabling Temporal Table for GAHR26...';

    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = 'SysStartTime' AND Object_ID = Object_ID('GAHR26'))
    BEGIN
        ALTER TABLE [GAHR26] ADD
            SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
            SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
    END

    ALTER TABLE [GAHR26] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.GAHR26_History));
END

-- 5. Kích hoạt Temporal cho GL41
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GL41' AND temporal_type = 0)
BEGIN
    PRINT '🚀 Enabling Temporal Table for GL41...';

    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = 'SysStartTime' AND Object_ID = Object_ID('GL41'))
    BEGIN
        ALTER TABLE [GL41] ADD
            SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
            SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
    END

    ALTER TABLE [GL41] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.GL41_History));
END

-- 6. Kích hoạt Temporal cho KH03 (nếu chưa có)
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'KH03' AND temporal_type = 0)
BEGIN
    PRINT '🚀 Enabling Temporal Table for KH03...';

    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = 'SysStartTime' AND Object_ID = Object_ID('KH03'))
    BEGIN
        ALTER TABLE [KH03] ADD
            SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
            SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
    END

    ALTER TABLE [KH03] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.KH03_History));
END

-- =======================================
-- 📊 TẠO COLUMNSTORE INDEXES CHO HISTORY TABLES
-- =======================================

-- 1. Columnstore Index cho 7800_DT_KHKD1_History
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = '7800_DT_KHKD1_History')
    AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_7800_DT_KHKD1_History_ColumnStore' AND object_id = OBJECT_ID('7800_DT_KHKD1_History'))
BEGIN
    PRINT '📊 Creating Columnstore Index for 7800_DT_KHKD1_History...';
    CREATE CLUSTERED COLUMNSTORE INDEX IX_7800_DT_KHKD1_History_ColumnStore
    ON [7800_DT_KHKD1_History];
END

-- 2. Columnstore Index cho DPDA_History
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DPDA_History')
    AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DPDA_History_ColumnStore' AND object_id = OBJECT_ID('DPDA_History'))
BEGIN
    PRINT '📊 Creating Columnstore Index for DPDA_History...';
    CREATE CLUSTERED COLUMNSTORE INDEX IX_DPDA_History_ColumnStore
    ON DPDA_History;
END

-- 3. Columnstore Index cho EI01_History
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'EI01_History')
    AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EI01_History_ColumnStore' AND object_id = OBJECT_ID('EI01_History'))
BEGIN
    PRINT '📊 Creating Columnstore Index for EI01_History...';
    CREATE CLUSTERED COLUMNSTORE INDEX IX_EI01_History_ColumnStore
    ON EI01_History;
END

-- 4. Columnstore Index cho GAHR26_History
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GAHR26_History')
    AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GAHR26_History_ColumnStore' AND object_id = OBJECT_ID('GAHR26_History'))
BEGIN
    PRINT '📊 Creating Columnstore Index for GAHR26_History...';
    CREATE CLUSTERED COLUMNSTORE INDEX IX_GAHR26_History_ColumnStore
    ON GAHR26_History;
END

-- 5. Columnstore Index cho GL41_History
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GL41_History')
    AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL41_History_ColumnStore' AND object_id = OBJECT_ID('GL41_History'))
BEGIN
    PRINT '📊 Creating Columnstore Index for GL41_History...';
    CREATE CLUSTERED COLUMNSTORE INDEX IX_GL41_History_ColumnStore
    ON GL41_History;
END

-- 6. Columnstore Index cho KH03_History
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'KH03_History')
    AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_KH03_History_ColumnStore' AND object_id = OBJECT_ID('KH03_History'))
BEGIN
    PRINT '📊 Creating Columnstore Index for KH03_History...';
    CREATE CLUSTERED COLUMNSTORE INDEX IX_KH03_History_ColumnStore
    ON KH03_History;
END

-- =======================================
-- ✅ KIỂM TRA KẾT QUẢ CUỐI CÙNG
-- =======================================
PRINT '✅ Final verification of Temporal Tables + Columnstore Indexes:';

SELECT
    t.name AS TableName,
    t.temporal_type_desc AS TemporalType,
    h.name AS HistoryTableName,
    CASE
        WHEN EXISTS (
            SELECT 1 FROM sys.indexes i
            WHERE i.object_id = h.object_id
            AND i.type IN (5, 6) -- COLUMNSTORE INDEX
        ) THEN '✅ Has Columnstore Index'
        ELSE '❌ Missing Columnstore Index'
    END AS ColumnstoreStatus
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
WHERE t.name IN ('7800_DT_KHKD1', 'BC57', 'DB01', 'DP01', 'DPDA', 'EI01', 'GAHR26', 'GL01', 'GL41', 'KH03', 'LN01', 'LN02', 'LN03', 'RR01')
ORDER BY t.name;

PRINT '🎯 Temporal Tables + Columnstore Indexes setup completed successfully!';
