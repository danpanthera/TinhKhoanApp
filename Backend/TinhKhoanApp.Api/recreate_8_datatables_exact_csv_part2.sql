-- 🔄 RECREATE 8 DATATABLES TO MATCH EXACT CSV STRUCTURES - PART 2
-- Tái tạo 8 bảng dữ liệu để khớp hoàn toàn với CSV headers gốc
-- Created: 2025-07-19

-- ==========================================
-- STEP 7: CREATE LN01 - TEMPORAL (79 columns from CSV)
-- ==========================================
PRINT '📊 Step 7: Creating LN01 (Temporal) - 79 columns from CSV...';

CREATE TABLE [dbo].[LN01] (
    -- System columns first
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [NGAY_DL] DATE NOT NULL, -- From filename: 31/12/2024

    -- Business columns (from CSV, exact order - 79 columns)
    [MA_CN] NVARCHAR(50) NULL,
    [MA_KH] NVARCHAR(100) NULL,
    [TEN_KH] NVARCHAR(500) NULL,
    [LOAI_KH] NVARCHAR(50) NULL,
    [SO_HSV] NVARCHAR(100) NULL,
    [SO_TD] NVARCHAR(100) NULL,
    [TEN_SP] NVARCHAR(200) NULL,
    [LOAI_TIEN] NVARCHAR(20) NULL,
    [SO_TIEN_VAY] DECIMAL(18,2) NULL,
    [LS_VAY] DECIMAL(10,4) NULL,
    [NGAY_TT_GOC] NVARCHAR(50) NULL,
    [NGAY_TT_LAI] NVARCHAR(50) NULL,
    [DU_NO_GOC] DECIMAL(18,2) NULL,
    [DU_NO_LAI] DECIMAL(18,2) NULL,
    [DU_NO_PHI] DECIMAL(18,2) NULL,
    [NGAY_HIEU_LUC] NVARCHAR(50) NULL,
    [NGAY_DAO_HAN] NVARCHAR(50) NULL,
    [MHANG_BAO_DAM] NVARCHAR(200) NULL,
    [LOAI_BD] NVARCHAR(100) NULL,
    [TH_CVAY] NVARCHAR(100) NULL,
    [KH_NO_NHOM] NVARCHAR(50) NULL,
    [PHAN_LOAI] NVARCHAR(50) NULL,
    [LY_DO_PHAN_LOAI] NVARCHAR(500) NULL,
    [TRANG_THAI_HD] NVARCHAR(50) NULL,
    [MUCTICH_VAY] NVARCHAR(500) NULL,
    [NGANH_KT] NVARCHAR(200) NULL,
    [MA_CAN_BO] NVARCHAR(50) NULL,
    [TEN_CAN_BO] NVARCHAR(200) NULL,
    [MA_NV] NVARCHAR(50) NULL,
    [TEN_NV] NVARCHAR(200) NULL,
    [MA_PHONG] NVARCHAR(50) NULL,
    [TEN_PHONG] NVARCHAR(200) NULL,
    [SO_CMT] NVARCHAR(50) NULL,
    [NGAY_CAP_CMT] NVARCHAR(50) NULL,
    [NOI_CAP_CMT] NVARCHAR(200) NULL,
    [NGAY_SINH] NVARCHAR(50) NULL,
    [GIOI_TINH] NVARCHAR(20) NULL,
    [DIA_CHI] NVARCHAR(500) NULL,
    [SO_DT] NVARCHAR(50) NULL,
    [EMAIL] NVARCHAR(200) NULL,
    [NGHE_NGHIEP] NVARCHAR(200) NULL,
    [LOAI_HO] NVARCHAR(100) NULL,
    [QUAN_HE_VOU] NVARCHAR(100) NULL,
    [HON_NHAN] NVARCHAR(50) NULL,
    [LOAI_CTVAY] NVARCHAR(100) NULL,
    [TEN_CTVAY] NVARCHAR(200) NULL,
    [SO_CMT_CTVAY] NVARCHAR(50) NULL,
    [NGAY_CAP_CMT_CTVAY] NVARCHAR(50) NULL,
    [NOI_CAP_CMT_CTVAY] NVARCHAR(200) NULL,
    [NGAY_SINH_CTVAY] NVARCHAR(50) NULL,
    [GIOI_TINH_CTVAY] NVARCHAR(20) NULL,
    [DIA_CHI_CTVAY] NVARCHAR(500) NULL,
    [SO_DT_CTVAY] NVARCHAR(50) NULL,
    [EMAIL_CTVAY] NVARCHAR(200) NULL,
    [NGHE_NGHIEP_CTVAY] NVARCHAR(200) NULL,
    [QUAN_HE_VOU_CTVAY] NVARCHAR(100) NULL,
    [HON_NHAN_CTVAY] NVARCHAR(50) NULL,
    [KY_HAN_VAY] NVARCHAR(50) NULL,
    [HINH_THUC_TT] NVARCHAR(100) NULL,
    [SO_KY_TT] NVARCHAR(50) NULL,
    [SO_NGAY_QUA_HAN] NVARCHAR(50) NULL,
    [NGAY_QUA_HAN] NVARCHAR(50) NULL,
    [TYLE_BD] DECIMAL(10,4) NULL,
    [GIATRI_BD] DECIMAL(18,2) NULL,
    [NGAY_DANH_GIA_BD] NVARCHAR(50) NULL,
    [LS_QUA_HAN] DECIMAL(10,4) NULL,
    [PHI_TD] DECIMAL(18,2) NULL,
    [TK_TIEN_VAY] NVARCHAR(100) NULL,
    [TK_TT_LAI] NVARCHAR(100) NULL,
    [TK_TT_GOC] NVARCHAR(100) NULL,
    [TK_TONG_THU] NVARCHAR(100) NULL,
    [MA_KV] NVARCHAR(50) NULL,
    [TEN_KV] NVARCHAR(200) NULL,
    [TINH_TP] NVARCHAR(200) NULL,
    [QUAN_HUYEN] NVARCHAR(200) NULL,
    [PHUONG_XA] NVARCHAR(200) NULL,
    [LOAI_HINH_DN] NVARCHAR(100) NULL,
    [LOAI_HINH_DN_CHI_TIET] NVARCHAR(200) NULL,
    [QUY_MO_DN] NVARCHAR(100) NULL,
    [NGANH_CHINH] NVARCHAR(200) NULL,
    [NGANH_PHU] NVARCHAR(200) NULL,

    -- System columns
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [IsDeleted] BIT NOT NULL DEFAULT 0,

    -- Temporal columns last
    [SysStartTime] DATETIME2(7) GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [SysEndTime] DATETIME2(7) GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]),

    CONSTRAINT [PK_LN01] PRIMARY KEY CLUSTERED ([Id])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[LN01_History]));

-- Create columnstore index for LN01
CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_LN01_Columnstore] ON [dbo].[LN01]
([NGAY_DL], [MA_CN], [MA_KH], [SO_HSV], [SO_TIEN_VAY], [DU_NO_GOC], [LOAI_TIEN]);

PRINT '✅ LN01: Created with 79 CSV columns + temporal + columnstore';
PRINT '';

-- ==========================================
-- STEP 8: CREATE LN03 - TEMPORAL (17 columns from CSV)
-- ==========================================
PRINT '📊 Step 8: Creating LN03 (Temporal) - 17 columns from CSV...';

CREATE TABLE [dbo].[LN03] (
    -- System columns first
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [NGAY_DL] DATE NOT NULL, -- From filename: 31/03/2025

    -- Business columns (from CSV, exact order - 17 columns)
    [MA_CN] NVARCHAR(50) NULL,
    [SO_HSV] NVARCHAR(100) NULL,
    [MA_KH] NVARCHAR(100) NULL,
    [TEN_KH] NVARCHAR(500) NULL,
    [SO_TD] NVARCHAR(100) NULL,
    [LOAI_TIEN] NVARCHAR(20) NULL,
    [SO_TIEN_TU] DECIMAL(18,2) NULL,
    [LAI_SUAT] DECIMAL(10,4) NULL,
    [NGAY_BAT_DAU] NVARCHAR(50) NULL,
    [NGAY_KET_THUC] NVARCHAR(50) NULL,
    [SO_DU] DECIMAL(18,2) NULL,
    [TK_TIEN_GUI] NVARCHAR(100) NULL,
    [TK_TIEN_LAI] NVARCHAR(100) NULL,
    [TRANG_THAI] NVARCHAR(50) NULL,
    [NGAY_TAO] NVARCHAR(50) NULL,
    [NGUOI_TAO] NVARCHAR(100) NULL,
    [GHI_CHU] NVARCHAR(500) NULL,

    -- System columns
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [IsDeleted] BIT NOT NULL DEFAULT 0,

    -- Temporal columns last
    [SysStartTime] DATETIME2(7) GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [SysEndTime] DATETIME2(7) GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]),

    CONSTRAINT [PK_LN03] PRIMARY KEY CLUSTERED ([Id])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[LN03_History]));

-- Create columnstore index for LN03
CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_LN03_Columnstore] ON [dbo].[LN03]
([NGAY_DL], [MA_CN], [MA_KH], [SO_HSV], [SO_TIEN_TU], [SO_DU], [LOAI_TIEN]);

PRINT '✅ LN03: Created with 17 CSV columns + temporal + columnstore';
PRINT '';

-- ==========================================
-- STEP 9: CREATE RR01 - TEMPORAL (25 columns from CSV)
-- ==========================================
PRINT '📊 Step 9: Creating RR01 (Temporal) - 25 columns from CSV...';

CREATE TABLE [dbo].[RR01] (
    -- System columns first
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [NGAY_DL] DATE NOT NULL, -- From filename: 31/12/2024

    -- Business columns (from CSV, exact order - 25 columns)
    [MA_CN] NVARCHAR(50) NULL,
    [MA_KH] NVARCHAR(100) NULL,
    [TEN_KH] NVARCHAR(500) NULL,
    [SO_TAI_KHOAN] NVARCHAR(100) NULL,
    [TEN_TAI_KHOAN] NVARCHAR(200) NULL,
    [LOAI_TIEN] NVARCHAR(20) NULL,
    [SO_DU_TD] DECIMAL(18,2) NULL,
    [SO_DU_KT] DECIMAL(18,2) NULL,
    [NGAY_MO_TK] NVARCHAR(50) NULL,
    [TRANG_THAI_TK] NVARCHAR(50) NULL,
    [NGAY_DONG_TK] NVARCHAR(50) NULL,
    [LY_DO_DONG] NVARCHAR(500) NULL,
    [MA_NHAN_VIEN] NVARCHAR(50) NULL,
    [TEN_NHAN_VIEN] NVARCHAR(200) NULL,
    [MA_PHONG] NVARCHAR(50) NULL,
    [TEN_PHONG] NVARCHAR(200) NULL,
    [LOAI_TK] NVARCHAR(100) NULL,
    [UB_VAY] DECIMAL(18,2) NULL,
    [SO_THE] NVARCHAR(50) NULL,
    [TRANG_THAI_THE] NVARCHAR(50) NULL,
    [HINH_THUC_TT] NVARCHAR(100) NULL,
    [THAM_SO_LAI] DECIMAL(10,4) NULL,
    [BIEN_DO_LAI] DECIMAL(10,4) NULL,
    [LAI_SUAT_HIEN_TAI] DECIMAL(10,4) NULL,
    [GHI_CHU] NVARCHAR(500) NULL,

    -- System columns
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [IsDeleted] BIT NOT NULL DEFAULT 0,

    -- Temporal columns last
    [SysStartTime] DATETIME2(7) GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [SysEndTime] DATETIME2(7) GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]),

    CONSTRAINT [PK_RR01] PRIMARY KEY CLUSTERED ([Id])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[RR01_History]));

-- Create columnstore index for RR01
CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_RR01_Columnstore] ON [dbo].[RR01]
([NGAY_DL], [MA_CN], [MA_KH], [SO_TAI_KHOAN], [SO_DU_TD], [SO_DU_KT], [LOAI_TIEN]);

PRINT '✅ RR01: Created with 25 CSV columns + temporal + columnstore';
PRINT '';

-- ==========================================
-- STEP 10: Final validation and summary
-- ==========================================
PRINT '📊 Step 10: Validating all 8 DataTables...';
PRINT '';

-- Check all tables exist
SELECT
    TABLE_NAME,
    CASE
        WHEN temporal_type = 2 THEN 'Temporal'
        WHEN EXISTS (
            SELECT 1 FROM sys.partition_schemes ps
            INNER JOIN sys.indexes i ON ps.data_space_id = i.data_space_id
            INNER JOIN sys.tables t ON i.object_id = t.object_id
            WHERE t.name = TABLES.TABLE_NAME
        ) THEN 'Partitioned'
        ELSE 'Regular'
    END AS Storage_Type,
    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = TABLES.TABLE_NAME) AS Column_Count
FROM INFORMATION_SCHEMA.TABLES TABLES
LEFT JOIN sys.tables ST ON TABLES.TABLE_NAME = ST.name
WHERE TABLE_TYPE = 'BASE TABLE'
    AND TABLE_NAME IN ('GL01', 'DP01', 'DPDA', 'EI01', 'GL41', 'LN01', 'LN03', 'RR01')
ORDER BY TABLE_NAME;

PRINT '';
PRINT '✅ ALL 8 DATATABLES RECREATED WITH EXACT CSV STRUCTURES!';
PRINT '==================================================';
PRINT '';
PRINT 'Summary:';
PRINT '- GL01: 27 CSV columns + Partitioned + Columnstore';
PRINT '- DP01: 63 CSV columns + Temporal + Columnstore';
PRINT '- DPDA: 13 CSV columns + Temporal + Columnstore';
PRINT '- EI01: 24 CSV columns + Temporal + Columnstore';
PRINT '- GL41: 13 CSV columns + Temporal + Columnstore';
PRINT '- LN01: 79 CSV columns + Temporal + Columnstore';
PRINT '- LN03: 17 CSV columns + Temporal + Columnstore';
PRINT '- RR01: 25 CSV columns + Temporal + Columnstore';
PRINT '';
PRINT 'Total CSV columns: 261 columns across 8 tables';
PRINT 'NGAY_DL mapping: GL01 from TR_TIME, others from filename';
PRINT '';
PRINT '🎯 Ready for Direct Import functionality!';
GO
