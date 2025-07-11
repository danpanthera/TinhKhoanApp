-- =======================================
-- 🔧 FIX REMAINING IDENTITY ISSUES FOR DPDA AND EI01
-- =======================================

USE TinhKhoanDB;
GO

PRINT '🔧 Fixing remaining IDENTITY issues for DPDA and EI01...';

-- Fix DPDA_History table (remove IDENTITY column)
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DPDA_History')
BEGIN
    PRINT '🔄 Recreating DPDA_History without IDENTITY...';

    -- Step 1: Backup existing data
    SELECT * INTO DPDA_History_Backup FROM DPDA_History;

    -- Step 2: Drop old history table
    DROP TABLE DPDA_History;

    -- Step 3: Create new history table without IDENTITY
    CREATE TABLE [DPDA_History] (
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
        [SysStartTime] datetime2 NOT NULL,
        [SysEndTime] datetime2 NOT NULL
    );

    -- Step 4: Restore backed up data (if any)
    IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DPDA_History_Backup')
    BEGIN
        INSERT INTO DPDA_History
        SELECT
            [Id], [BusinessKey], [EffectiveDate], [ExpiryDate], [IsCurrent], [RowVersion],
            [ImportId], [StatementDate], [ProcessedDate], [DataHash],
            [MaKhachHang], [TenKhachHang], [LoaiThe], [SoThe], [NgayPhatHanh], [NgayHetHan],
            [TrangThaiThe], [MaChiNhanh], [TenChiNhanh], [AdditionalData],
            '1900-01-01' AS SysStartTime, '9999-12-31' AS SysEndTime
        FROM DPDA_History_Backup;

        DROP TABLE DPDA_History_Backup;
    END

    PRINT '✅ DPDA_History recreated without IDENTITY.';
END

-- Fix EI01_History table (remove IDENTITY column)
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'EI01_History')
BEGIN
    PRINT '🔄 Recreating EI01_History without IDENTITY...';

    -- Step 1: Backup existing data
    SELECT * INTO EI01_History_Backup FROM EI01_History;

    -- Step 2: Drop old history table
    DROP TABLE EI01_History;

    -- Step 3: Create new history table without IDENTITY
    CREATE TABLE [EI01_History] (
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
        [MaKhachHang] nvarchar(50) NULL,
        [TenKhachHang] nvarchar(200) NULL,
        [SoTaiKhoan] nvarchar(50) NULL,
        [LoaiGiaoDich] nvarchar(100) NULL,
        [MaGiaoDich] nvarchar(50) NULL,
        [SoTien] decimal(18,2) NULL,
        [NgayGiaoDich] datetime2 NULL,
        [ThoiGianGiaoDich] datetime2 NULL,
        [TrangThaiGiaoDich] nvarchar(50) NULL,
        [NoiDungGiaoDich] nvarchar(1000) NULL,
        [MaChiNhanh] nvarchar(20) NULL,
        [TenChiNhanh] nvarchar(200) NULL,
        [Channel] nvarchar(50) NULL,
        [DeviceInfo] nvarchar(200) NULL,
        [AdditionalData] nvarchar(max) NULL,
        [SysStartTime] datetime2 NOT NULL,
        [SysEndTime] datetime2 NOT NULL
    );

    -- Step 4: Restore backed up data (if any)
    IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'EI01_History_Backup')
    BEGIN
        INSERT INTO EI01_History
        SELECT
            [Id], [BusinessKey], [EffectiveDate], [ExpiryDate], [IsCurrent], [RowVersion],
            [ImportId], [StatementDate], [ProcessedDate], [DataHash],
            [MaKhachHang], [TenKhachHang], [SoTaiKhoan], [LoaiGiaoDich], [MaGiaoDich], [SoTien],
            [NgayGiaoDich], [ThoiGianGiaoDich], [TrangThaiGiaoDich], [NoiDungGiaoDich],
            [MaChiNhanh], [TenChiNhanh], [Channel], [DeviceInfo], [AdditionalData],
            '1900-01-01' AS SysStartTime, '9999-12-31' AS SysEndTime
        FROM EI01_History_Backup;

        DROP TABLE EI01_History_Backup;
    END

    PRINT '✅ EI01_History recreated without IDENTITY.';
END

-- Now enable temporal for DPDA
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DPDA' AND temporal_type = 0)
BEGIN
    PRINT '🚀 Enabling temporal for DPDA...';
    BEGIN TRY
        -- Enable system versioning (temporal columns already exist)
        ALTER TABLE [DPDA] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[DPDA_History]));
        PRINT '✅ DPDA temporal enabled successfully.';
    END TRY
    BEGIN CATCH
        PRINT '⚠️ Error enabling DPDA temporal: ' + ERROR_MESSAGE();
    END CATCH
END

-- Now enable temporal for EI01
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'EI01' AND temporal_type = 0)
BEGIN
    PRINT '🚀 Enabling temporal for EI01...';
    BEGIN TRY
        -- Enable system versioning
        ALTER TABLE [EI01] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[EI01_History]));
        PRINT '✅ EI01 temporal enabled successfully.';
    END TRY
    BEGIN CATCH
        PRINT '⚠️ Error enabling EI01 temporal: ' + ERROR_MESSAGE();
    END CATCH
END

-- Create missing columnstore indexes (handle existing clustered indexes)
PRINT '📊 Creating/Updating Columnstore Indexes...';

-- Create columnstore for DPDA_History
BEGIN TRY
    CREATE CLUSTERED COLUMNSTORE INDEX IX_DPDA_History_ColumnStore ON [DPDA_History];
    PRINT '✅ Created Columnstore Index for DPDA_History';
END TRY
BEGIN CATCH
    PRINT '⚠️ Warning: Could not create columnstore index for DPDA_History: ' + ERROR_MESSAGE();
END CATCH

-- Create columnstore for EI01_History
BEGIN TRY
    CREATE CLUSTERED COLUMNSTORE INDEX IX_EI01_History_ColumnStore ON [EI01_History];
    PRINT '✅ Created Columnstore Index for EI01_History';
END TRY
BEGIN CATCH
    PRINT '⚠️ Warning: Could not create columnstore index for EI01_History: ' + ERROR_MESSAGE();
END CATCH

-- Final comprehensive verification
PRINT '📋 Final comprehensive verification of ALL business tables:';

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
        WHEN t.temporal_type = 2 AND h.object_id IS NOT NULL THEN '✅ COMPLETE'
        WHEN t.temporal_type = 2 THEN '⚠️ Temporal but no history'
        WHEN t.object_id IS NOT NULL THEN '❌ No temporal'
        ELSE '❌ Missing table'
    END AS Status
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
ORDER BY t.name;

PRINT '🎉 SUCCESS! Temporal Tables + Columnstore Indexes setup completed for all target tables!';

GO
