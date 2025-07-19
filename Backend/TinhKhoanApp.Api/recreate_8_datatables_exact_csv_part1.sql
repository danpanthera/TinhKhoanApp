-- 🔄 RECREATE 8 DATATABLES TO MATCH EXACT CSV STRUCTURES
-- Tái tạo 8 bảng dữ liệu để khớp hoàn toàn với CSV headers gốc
-- Created: 2025-07-19

SET QUOTED_IDENTIFIER ON;
GO

PRINT '🔄 RECREATING 8 DATATABLES TO MATCH CSV STRUCTURES';
PRINT '==================================================';
PRINT '';

-- ==========================================
-- STEP 1: Drop existing tables safely
-- ==========================================
PRINT '🗑️ Step 1: Dropping existing tables...';

-- Drop temporal tables first
DECLARE @tables TABLE (name NVARCHAR(128));
INSERT INTO @tables VALUES ('DP01'), ('DPDA'), ('EI01'), ('GL41'), ('LN01'), ('LN03'), ('RR01');

DECLARE @tableName NVARCHAR(128);
DECLARE table_cursor CURSOR FOR SELECT name FROM @tables;

OPEN table_cursor;
FETCH NEXT FROM table_cursor INTO @tableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF EXISTS (SELECT * FROM sys.tables WHERE name = @tableName AND temporal_type = 2)
    BEGIN
        EXEC('ALTER TABLE ' + @tableName + ' SET (SYSTEM_VERSIONING = OFF)');
        PRINT '   - Temporal disabled for ' + @tableName;
    END

    IF EXISTS (SELECT * FROM sys.tables WHERE name = @tableName + '_History')
    BEGIN
        EXEC('DROP TABLE ' + @tableName + '_History');
        PRINT '   - History table dropped for ' + @tableName;
    END

    IF EXISTS (SELECT * FROM sys.tables WHERE name = @tableName)
    BEGIN
        EXEC('DROP TABLE ' + @tableName);
        PRINT '   - Table dropped: ' + @tableName;
    END

    FETCH NEXT FROM table_cursor INTO @tableName;
END

CLOSE table_cursor;
DEALLOCATE table_cursor;

-- Drop GL01 (partitioned table)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GL01')
BEGIN
    DROP TABLE GL01;
    PRINT '   - GL01 dropped';
END

PRINT '';

-- ==========================================
-- STEP 2: CREATE GL01 - PARTITIONED (27 columns from CSV)
-- ==========================================
PRINT '📊 Step 2: Creating GL01 (Partitioned) - 27 columns from CSV...';

-- Create GL01 with EXACT CSV structure
CREATE TABLE [dbo].[GL01] (
    -- System columns first
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [NGAY_DL] DATE NOT NULL, -- From TR_TIME column in CSV

    -- Business columns (from CSV, exact order)
    [STS] NVARCHAR(50) NULL,
    [NGAY_GD] NVARCHAR(50) NULL,
    [NGUOI_TAO] NVARCHAR(100) NULL,
    [DYSEQ] NVARCHAR(50) NULL,
    [TR_TYPE] NVARCHAR(50) NULL,
    [DT_SEQ] NVARCHAR(50) NULL,
    [TAI_KHOAN] NVARCHAR(100) NULL,
    [TEN_TK] NVARCHAR(500) NULL,
    [SO_TIEN_GD] DECIMAL(18,2) NULL,
    [POST_BR] NVARCHAR(50) NULL,
    [LOAI_TIEN] NVARCHAR(20) NULL,
    [DR_CR] NVARCHAR(10) NULL,
    [MA_KH] NVARCHAR(100) NULL,
    [TEN_KH] NVARCHAR(500) NULL,
    [CCA_USRID] NVARCHAR(100) NULL,
    [TR_EX_RT] DECIMAL(10,4) NULL,
    [REMARK] NVARCHAR(1000) NULL,
    [BUS_CODE] NVARCHAR(50) NULL,
    [UNIT_BUS_CODE] NVARCHAR(50) NULL,
    [TR_CODE] NVARCHAR(50) NULL,
    [TR_NAME] NVARCHAR(200) NULL,
    [REFERENCE] NVARCHAR(200) NULL,
    [VALUE_DATE] NVARCHAR(50) NULL,
    [DEPT_CODE] NVARCHAR(50) NULL,
    [TR_TIME] NVARCHAR(50) NULL,
    [COMFIRM] NVARCHAR(50) NULL,
    [TRDT_TIME] NVARCHAR(50) NULL,

    -- System columns last
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [IsDeleted] BIT NOT NULL DEFAULT 0,

    CONSTRAINT [PK_GL01] PRIMARY KEY CLUSTERED ([Id], [NGAY_DL])
) ON PS_GL01_Date([NGAY_DL]);

-- Create columnstore index for GL01
CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_GL01_Columnstore] ON [dbo].[GL01]
([NGAY_DL], [TAI_KHOAN], [MA_KH], [SO_TIEN_GD], [LOAI_TIEN], [DR_CR]);

PRINT '✅ GL01: Created with 27 CSV columns + partitioned + columnstore';
PRINT '';

-- ==========================================
-- STEP 3: CREATE DP01 - TEMPORAL (63 columns from CSV)
-- ==========================================
PRINT '📊 Step 3: Creating DP01 (Temporal) - 63 columns from CSV...';

CREATE TABLE [dbo].[DP01] (
    -- System columns first
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [NGAY_DL] DATE NOT NULL, -- From filename: 31/12/2024

    -- Business columns (from CSV, exact order - 63 columns)
    [MA_CN] NVARCHAR(50) NULL,
    [TAI_KHOAN_HACH_TOAN] NVARCHAR(100) NULL,
    [MA_KH] NVARCHAR(100) NULL,
    [TEN_KH] NVARCHAR(500) NULL,
    [DP_TYPE_NAME] NVARCHAR(200) NULL,
    [CCY] NVARCHAR(20) NULL,
    [CURRENT_BALANCE] DECIMAL(18,2) NULL,
    [RATE] DECIMAL(10,4) NULL,
    [SO_TAI_KHOAN] NVARCHAR(100) NULL,
    [OPENING_DATE] NVARCHAR(50) NULL,
    [MATURITY_DATE] NVARCHAR(50) NULL,
    [ADDRESS] NVARCHAR(500) NULL,
    [NOTENO] NVARCHAR(100) NULL,
    [MONTH_TERM] NVARCHAR(50) NULL,
    [TERM_DP_NAME] NVARCHAR(200) NULL,
    [TIME_DP_NAME] NVARCHAR(200) NULL,
    [MA_PGD] NVARCHAR(50) NULL,
    [TEN_PGD] NVARCHAR(200) NULL,
    [DP_TYPE_CODE] NVARCHAR(50) NULL,
    [RENEW_DATE] NVARCHAR(50) NULL,
    [CUST_TYPE] NVARCHAR(50) NULL,
    [CUST_TYPE_NAME] NVARCHAR(200) NULL,
    [CUST_TYPE_DETAIL] NVARCHAR(50) NULL,
    [CUST_DETAIL_NAME] NVARCHAR(200) NULL,
    [PREVIOUS_DP_CAP_DATE] NVARCHAR(50) NULL,
    [NEXT_DP_CAP_DATE] NVARCHAR(50) NULL,
    [ID_NUMBER] NVARCHAR(50) NULL,
    [ISSUED_BY] NVARCHAR(200) NULL,
    [ISSUE_DATE] NVARCHAR(50) NULL,
    [SEX_TYPE] NVARCHAR(20) NULL,
    [BIRTH_DATE] NVARCHAR(50) NULL,
    [TELEPHONE] NVARCHAR(50) NULL,
    [ACRUAL_AMOUNT] DECIMAL(18,2) NULL,
    [ACRUAL_AMOUNT_END] DECIMAL(18,2) NULL,
    [ACCOUNT_STATUS] NVARCHAR(50) NULL,
    [DRAMT] DECIMAL(18,2) NULL,
    [CRAMT] DECIMAL(18,2) NULL,
    [EMPLOYEE_NUMBER] NVARCHAR(50) NULL,
    [EMPLOYEE_NAME] NVARCHAR(200) NULL,
    [SPECIAL_RATE] DECIMAL(10,4) NULL,
    [AUTO_RENEWAL] NVARCHAR(20) NULL,
    [CLOSE_DATE] NVARCHAR(50) NULL,
    [LOCAL_PROVIN_NAME] NVARCHAR(200) NULL,
    [LOCAL_DISTRICT_NAME] NVARCHAR(200) NULL,
    [LOCAL_WARD_NAME] NVARCHAR(200) NULL,
    [TERM_DP_TYPE] NVARCHAR(50) NULL,
    [TIME_DP_TYPE] NVARCHAR(50) NULL,
    [STATES_CODE] NVARCHAR(50) NULL,
    [ZIP_CODE] NVARCHAR(20) NULL,
    [COUNTRY_CODE] NVARCHAR(20) NULL,
    [TAX_CODE_LOCATION] NVARCHAR(100) NULL,
    [MA_CAN_BO_PT] NVARCHAR(50) NULL,
    [TEN_CAN_BO_PT] NVARCHAR(200) NULL,
    [PHONG_CAN_BO_PT] NVARCHAR(100) NULL,
    [NGUOI_NUOC_NGOAI] NVARCHAR(20) NULL,
    [QUOC_TICH] NVARCHAR(100) NULL,
    [MA_CAN_BO_AGRIBANK] NVARCHAR(50) NULL,
    [NGUOI_GIOI_THIEU] NVARCHAR(50) NULL,
    [TEN_NGUOI_GIOI_THIEU] NVARCHAR(200) NULL,
    [CONTRACT_COUTS_DAY] NVARCHAR(50) NULL,
    [SO_KY_AD_LSDB] NVARCHAR(50) NULL,
    [UNTBUSCD] NVARCHAR(50) NULL,
    [TYGIA] DECIMAL(10,4) NULL,

    -- System columns
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [IsDeleted] BIT NOT NULL DEFAULT 0,

    -- Temporal columns last
    [SysStartTime] DATETIME2(7) GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [SysEndTime] DATETIME2(7) GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]),

    CONSTRAINT [PK_DP01] PRIMARY KEY CLUSTERED ([Id])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[DP01_History]));

-- Create columnstore index for DP01
CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_DP01_Columnstore] ON [dbo].[DP01]
([NGAY_DL], [MA_CN], [MA_KH], [TAI_KHOAN_HACH_TOAN], [CURRENT_BALANCE], [CCY]);

PRINT '✅ DP01: Created with 63 CSV columns + temporal + columnstore';
PRINT '';

-- ==========================================
-- STEP 4: CREATE DPDA - TEMPORAL (13 columns from CSV)
-- ==========================================
PRINT '📊 Step 4: Creating DPDA (Temporal) - 13 columns from CSV...';

CREATE TABLE [dbo].[DPDA] (
    -- System columns first
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [NGAY_DL] DATE NOT NULL, -- From filename: 31/03/2025

    -- Business columns (from CSV, exact order - 13 columns)
    [MA_CHI_NHANH] NVARCHAR(50) NULL,
    [MA_KHACH_HANG] NVARCHAR(100) NULL,
    [TEN_KHACH_HANG] NVARCHAR(500) NULL,
    [SO_TAI_KHOAN] NVARCHAR(100) NULL,
    [LOAI_THE] NVARCHAR(100) NULL,
    [SO_THE] NVARCHAR(50) NULL,
    [NGAY_NOP_DON] NVARCHAR(50) NULL,
    [NGAY_PHAT_HANH] NVARCHAR(50) NULL,
    [USER_PHAT_HANH] NVARCHAR(100) NULL,
    [TRANG_THAI] NVARCHAR(50) NULL,
    [PHAN_LOAI] NVARCHAR(100) NULL,
    [GIAO_THE] NVARCHAR(50) NULL,
    [LOAI_PHAT_HANH] NVARCHAR(100) NULL,

    -- System columns
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [IsDeleted] BIT NOT NULL DEFAULT 0,

    -- Temporal columns last
    [SysStartTime] DATETIME2(7) GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [SysEndTime] DATETIME2(7) GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]),

    CONSTRAINT [PK_DPDA] PRIMARY KEY CLUSTERED ([Id])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[DPDA_History]));

-- Create columnstore index for DPDA
CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_DPDA_Columnstore] ON [dbo].[DPDA]
([NGAY_DL], [MA_CHI_NHANH], [MA_KHACH_HANG], [SO_TAI_KHOAN]);

PRINT '✅ DPDA: Created with 13 CSV columns + temporal + columnstore';
PRINT '';

-- ==========================================
-- STEP 5: CREATE EI01 - TEMPORAL (24 columns from CSV)
-- ==========================================
PRINT '📊 Step 5: Creating EI01 (Temporal) - 24 columns from CSV...';

CREATE TABLE [dbo].[EI01] (
    -- System columns first
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [NGAY_DL] DATE NOT NULL, -- From filename: 31/12/2024

    -- Business columns (from CSV, exact order - 24 columns)
    [MA_CN] NVARCHAR(50) NULL,
    [MA_KH] NVARCHAR(100) NULL,
    [TEN_KH] NVARCHAR(500) NULL,
    [LOAI_KH] NVARCHAR(50) NULL,
    [SDT_EMB] NVARCHAR(50) NULL,
    [TRANG_THAI_EMB] NVARCHAR(50) NULL,
    [NGAY_DK_EMB] NVARCHAR(50) NULL,
    [SDT_OTT] NVARCHAR(50) NULL,
    [TRANG_THAI_OTT] NVARCHAR(50) NULL,
    [NGAY_DK_OTT] NVARCHAR(50) NULL,
    [SDT_SMS] NVARCHAR(50) NULL,
    [TRANG_THAI_SMS] NVARCHAR(50) NULL,
    [NGAY_DK_SMS] NVARCHAR(50) NULL,
    [SDT_SAV] NVARCHAR(50) NULL,
    [TRANG_THAI_SAV] NVARCHAR(50) NULL,
    [NGAY_DK_SAV] NVARCHAR(50) NULL,
    [SDT_LN] NVARCHAR(50) NULL,
    [TRANG_THAI_LN] NVARCHAR(50) NULL,
    [NGAY_DK_LN] NVARCHAR(50) NULL,
    [USER_EMB] NVARCHAR(100) NULL,
    [USER_OTT] NVARCHAR(100) NULL,
    [USER_SMS] NVARCHAR(100) NULL,
    [USER_SAV] NVARCHAR(100) NULL,
    [USER_LN] NVARCHAR(100) NULL,

    -- System columns
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [IsDeleted] BIT NOT NULL DEFAULT 0,

    -- Temporal columns last
    [SysStartTime] DATETIME2(7) GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [SysEndTime] DATETIME2(7) GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]),

    CONSTRAINT [PK_EI01] PRIMARY KEY CLUSTERED ([Id])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[EI01_History]));

-- Create columnstore index for EI01
CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_EI01_Columnstore] ON [dbo].[EI01]
([NGAY_DL], [MA_CN], [MA_KH], [LOAI_KH]);

PRINT '✅ EI01: Created with 24 CSV columns + temporal + columnstore';
PRINT '';

-- ==========================================
-- STEP 6: CREATE GL41 - TEMPORAL (13 columns from CSV)
-- ==========================================
PRINT '📊 Step 6: Creating GL41 (Temporal) - 13 columns from CSV...';

CREATE TABLE [dbo].[GL41] (
    -- System columns first
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [NGAY_DL] DATE NOT NULL, -- From filename: 30/06/2025

    -- Business columns (from CSV, exact order - 13 columns)
    [MA_CN] NVARCHAR(50) NULL,
    [LOAI_TIEN] NVARCHAR(20) NULL,
    [MA_TK] NVARCHAR(100) NULL,
    [TEN_TK] NVARCHAR(500) NULL,
    [LOAI_BT] NVARCHAR(50) NULL,
    [DN_DAUKY] DECIMAL(18,2) NULL,
    [DC_DAUKY] DECIMAL(18,2) NULL,
    [SBT_NO] DECIMAL(18,2) NULL,
    [ST_GHINO] DECIMAL(18,2) NULL,
    [SBT_CO] DECIMAL(18,2) NULL,
    [ST_GHICO] DECIMAL(18,2) NULL,
    [DN_CUOIKY] DECIMAL(18,2) NULL,
    [DC_CUOIKY] DECIMAL(18,2) NULL,

    -- System columns
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [IsDeleted] BIT NOT NULL DEFAULT 0,

    -- Temporal columns last
    [SysStartTime] DATETIME2(7) GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [SysEndTime] DATETIME2(7) GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]),

    CONSTRAINT [PK_GL41] PRIMARY KEY CLUSTERED ([Id])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[GL41_History]));

-- Create columnstore index for GL41
CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_GL41_Columnstore] ON [dbo].[GL41]
([NGAY_DL], [MA_CN], [MA_TK], [LOAI_TIEN], [DN_DAUKY], [DC_CUOIKY]);

PRINT '✅ GL41: Created with 13 CSV columns + temporal + columnstore';
PRINT '';

-- Continue with remaining tables in next part...
PRINT '🔄 Script continues with LN01, LN03, RR01...';
PRINT '';
