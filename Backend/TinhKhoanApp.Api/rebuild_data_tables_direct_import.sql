-- Script cấu hình lại 8 bảng dữ liệu theo chuẩn Direct Import
-- GL01: Partitioned Columnstore với NGAY_DL từ TR_TIME
-- 7 bảng khác: Temporal Table + Columnstore với NGAY_DL từ filename

-- Bước 1: Drop các bảng hiện tại và history tables
PRINT '🔄 Dropping existing tables and indexes...'

-- Drop temporal tables (cần disable temporal trước)
IF OBJECT_ID('DP01', 'U') IS NOT NULL
BEGIN
    IF OBJECTPROPERTY(OBJECT_ID('DP01'), 'TableTemporalType') = 2
        ALTER TABLE DP01 SET (SYSTEM_VERSIONING = OFF);
    DROP TABLE IF EXISTS DP01_History;
    DROP TABLE DP01;
END

IF OBJECT_ID('DPDA', 'U') IS NOT NULL
BEGIN
    IF OBJECTPROPERTY(OBJECT_ID('DPDA'), 'TableTemporalType') = 2
        ALTER TABLE DPDA SET (SYSTEM_VERSIONING = OFF);
    DROP TABLE IF EXISTS DPDA_History;
    DROP TABLE DPDA;
END

IF OBJECT_ID('EI01', 'U') IS NOT NULL
BEGIN
    IF OBJECTPROPERTY(OBJECT_ID('EI01'), 'TableTemporalType') = 2
        ALTER TABLE EI01 SET (SYSTEM_VERSIONING = OFF);
    DROP TABLE IF EXISTS EI01_History;
    DROP TABLE EI01;
END

-- GL01 special case - partitioned table
IF OBJECT_ID('GL01', 'U') IS NOT NULL
    DROP TABLE GL01;

IF OBJECT_ID('GL41', 'U') IS NOT NULL
BEGIN
    IF OBJECTPROPERTY(OBJECT_ID('GL41'), 'TableTemporalType') = 2
        ALTER TABLE GL41 SET (SYSTEM_VERSIONING = OFF);
    DROP TABLE IF EXISTS GL41_History;
    DROP TABLE GL41;
END

IF OBJECT_ID('LN01', 'U') IS NOT NULL
BEGIN
    IF OBJECTPROPERTY(OBJECT_ID('LN01'), 'TableTemporalType') = 2
        ALTER TABLE LN01 SET (SYSTEM_VERSIONING = OFF);
    DROP TABLE IF EXISTS LN01_History;
    DROP TABLE LN01;
END

IF OBJECT_ID('LN03', 'U') IS NOT NULL
BEGIN
    IF OBJECTPROPERTY(OBJECT_ID('LN03'), 'TableTemporalType') = 2
        ALTER TABLE LN03 SET (SYSTEM_VERSIONING = OFF);
    DROP TABLE IF EXISTS LN03_History;
    DROP TABLE LN03;
END

IF OBJECT_ID('RR01', 'U') IS NOT NULL
BEGIN
    IF OBJECTPROPERTY(OBJECT_ID('RR01'), 'TableTemporalType') = 2
        ALTER TABLE RR01 SET (SYSTEM_VERSIONING = OFF);
    DROP TABLE IF EXISTS RR01_History;
    DROP TABLE RR01;
END

PRINT '✅ Existing tables dropped successfully'

-- Bước 2: Tạo Partition Function và Partition Scheme cho GL01
PRINT '🔄 Creating partition function and scheme for GL01...'

-- Drop existing partition objects if they exist
IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'PS_GL01_Date')
    DROP PARTITION SCHEME PS_GL01_Date;

IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'PF_GL01_Date')
    DROP PARTITION FUNCTION PF_GL01_Date;

-- Create partition function for date-based partitioning (monthly)
CREATE PARTITION FUNCTION PF_GL01_Date (datetime)
AS RANGE RIGHT FOR VALUES (
    '2024-01-01', '2024-02-01', '2024-03-01', '2024-04-01', '2024-05-01', '2024-06-01',
    '2024-07-01', '2024-08-01', '2024-09-01', '2024-10-01', '2024-11-01', '2024-12-01',
    '2025-01-01', '2025-02-01', '2025-03-01', '2025-04-01', '2025-05-01', '2025-06-01',
    '2025-07-01', '2025-08-01', '2025-09-01', '2025-10-01', '2025-11-01', '2025-12-01'
);

-- Create partition scheme
CREATE PARTITION SCHEME PS_GL01_Date
AS PARTITION PF_GL01_Date
ALL TO ([PRIMARY]);

PRINT '✅ Partition function and scheme created for GL01'

-- Bước 3: Tạo lại các bảng với cấu trúc mới

-- 3.1: DP01 - Temporal Table
PRINT '🔄 Creating DP01 with Temporal + Columnstore...'
CREATE TABLE DP01 (
    -- Business columns (63 columns from CSV)
    MA_CN nvarchar(10),
    TAI_KHOAN_HACH_TOAN nvarchar(50),
    MA_KH nvarchar(50),
    TEN_KH nvarchar(255),
    SO_TK nvarchar(50),
    TEN_TK nvarchar(255),
    SO_DU_DAU_KY decimal(18,2),
    SO_DU_CUOI_KY decimal(18,2),
    PS_NO decimal(18,2),
    PS_CO decimal(18,2),
    LUY_KE_PS_NO decimal(18,2),
    LUY_KE_PS_CO decimal(18,2),
    LOAI_TIEN nvarchar(10),
    TY_GIA decimal(10,4),
    SO_DU_DAU_KY_QD decimal(18,2),
    SO_DU_CUOI_KY_QD decimal(18,2),
    PS_NO_QD decimal(18,2),
    PS_CO_QD decimal(18,2),
    LUY_KE_PS_NO_QD decimal(18,2),
    LUY_KE_PS_CO_QD decimal(18,2),
    MA_SP nvarchar(20),
    TEN_SP nvarchar(255),
    MA_KM nvarchar(20),
    NHOM_KH nvarchar(50),
    PH_KBNN nvarchar(10),
    PH_KBNN_CODE nvarchar(10),
    PH_KBNN_NAME nvarchar(255),
    HINH_THUC_TD nvarchar(50),
    HINH_THUC_TD_CODE nvarchar(10),
    HINH_THUC_TD_NAME nvarchar(255),
    DOI_TUONG_VAY nvarchar(50),
    DOI_TUONG_VAY_CODE nvarchar(10),
    DOI_TUONG_VAY_NAME nvarchar(255),
    NGANH_KINH_TE nvarchar(50),
    NGANH_KINH_TE_CODE nvarchar(10),
    NGANH_KINH_TE_NAME nvarchar(255),
    TINH_THANH nvarchar(50),
    TINH_THANH_CODE nvarchar(10),
    TINH_THANH_NAME nvarchar(255),
    QUAN_HUYEN nvarchar(50),
    QUAN_HUYEN_CODE nvarchar(10),
    QUAN_HUYEN_NAME nvarchar(255),
    XA_PHUONG nvarchar(50),
    XA_PHUONG_CODE nvarchar(10),
    XA_PHUONG_NAME nvarchar(255),
    MUC_DICH_VAY nvarchar(50),
    MUC_DICH_VAY_CODE nvarchar(10),
    MUC_DICH_VAY_NAME nvarchar(255),
    CO_CAU_KH nvarchar(50),
    CO_CAU_KH_CODE nvarchar(10),
    CO_CAU_KH_NAME nvarchar(255),
    LOAI_HOP_DONG nvarchar(50),
    LOAI_HOP_DONG_CODE nvarchar(10),
    LOAI_HOP_DONG_NAME nvarchar(255),
    NHOM_NO nvarchar(10),
    PHAN_LOAI_NO nvarchar(10),
    TINH_CHAT_SO_DU nvarchar(10),
    KIEM_SOAT_TD nvarchar(10),
    GHI_CHU nvarchar(500),
    NGUOI_QUAN_LY nvarchar(100),
    NGAY_SINH date,
    GIOI_TINH nvarchar(10),
    DIA_CHI nvarchar(500),
    SO_DIEN_THOAI nvarchar(20),
    EMAIL nvarchar(100),
    SO_CMT nvarchar(20),

    -- System columns
    Id bigint IDENTITY(1,1) PRIMARY KEY,
    NGAY_DL datetime NOT NULL, -- From filename, format dd/mm/yyyy
    CREATED_DATE datetime2 DEFAULT GETDATE(),
    UPDATED_DATE datetime2 DEFAULT GETDATE(),
    FILE_NAME nvarchar(255),

    -- Temporal columns
    ValidFrom datetime2 GENERATED ALWAYS AS ROW START,
    ValidTo datetime2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.DP01_History));

-- Create columnstore index
CREATE NONCLUSTERED COLUMNSTORE INDEX IX_DP01_Columnstore
ON DP01 (MA_CN, TAI_KHOAN_HACH_TOAN, MA_KH, SO_DU_CUOI_KY, NGAY_DL, LOAI_TIEN);

PRINT '✅ DP01 created with Temporal + Columnstore'

-- 3.2: DPDA - Temporal Table
PRINT '🔄 Creating DPDA with Temporal + Columnstore...'
CREATE TABLE DPDA (
    -- Business columns (13 columns from CSV)
    MA_CHI_NHANH nvarchar(10),
    MA_KHACH_HANG nvarchar(50),
    TEN_KHACH_HANG nvarchar(255),
    SO_TAI_KHOAN nvarchar(50),
    TEN_TAI_KHOAN nvarchar(255),
    SO_DU_CUOI_KY decimal(18,2),
    LOAI_TIEN nvarchar(10),
    TY_GIA decimal(10,4),
    SO_DU_CUOI_KY_QD decimal(18,2),
    NHOM_KHACH_HANG nvarchar(50),
    TINH_CHAT_SO_DU nvarchar(10),
    GHI_CHU nvarchar(500),
    NGUOI_QUAN_LY nvarchar(100),

    -- System columns
    Id bigint IDENTITY(1,1) PRIMARY KEY,
    NGAY_DL datetime NOT NULL,
    CREATED_DATE datetime2 DEFAULT GETDATE(),
    UPDATED_DATE datetime2 DEFAULT GETDATE(),
    FILE_NAME nvarchar(255),

    -- Temporal columns
    ValidFrom datetime2 GENERATED ALWAYS AS ROW START,
    ValidTo datetime2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.DPDA_History));

CREATE NONCLUSTERED COLUMNSTORE INDEX IX_DPDA_Columnstore
ON DPDA (MA_CHI_NHANH, MA_KHACH_HANG, SO_DU_CUOI_KY, NGAY_DL);

PRINT '✅ DPDA created with Temporal + Columnstore'

-- 3.3: EI01 - Temporal Table
PRINT '🔄 Creating EI01 with Temporal + Columnstore...'
CREATE TABLE EI01 (
    -- Business columns (24 columns from CSV)
    MA_CN nvarchar(10),
    MA_KH nvarchar(50),
    TEN_KH nvarchar(255),
    SO_TKTT nvarchar(50),
    TEN_TKTT nvarchar(255),
    SO_DU_CUOI_KY decimal(18,2),
    LAI_SUAT decimal(8,4),
    LOAI_TIEN nvarchar(10),
    TY_GIA decimal(10,4),
    SO_DU_CUOI_KY_QD decimal(18,2),
    MA_SP nvarchar(20),
    TEN_SP nvarchar(255),
    NHOM_KH nvarchar(50),
    NGAY_MO date,
    NGAY_DAO_HAN date,
    KY_HAN int,
    HINH_THUC_GD nvarchar(50),
    TINH_CHAT_SO_DU nvarchar(10),
    GHI_CHU nvarchar(500),
    NGUOI_QUAN_LY nvarchar(100),
    DIA_CHI nvarchar(500),
    SO_DIEN_THOAI nvarchar(20),
    EMAIL nvarchar(100),
    SO_CMT nvarchar(20),

    -- System columns
    Id bigint IDENTITY(1,1) PRIMARY KEY,
    NGAY_DL datetime NOT NULL,
    CREATED_DATE datetime2 DEFAULT GETDATE(),
    UPDATED_DATE datetime2 DEFAULT GETDATE(),
    FILE_NAME nvarchar(255),

    -- Temporal columns
    ValidFrom datetime2 GENERATED ALWAYS AS ROW START,
    ValidTo datetime2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.EI01_History));

CREATE NONCLUSTERED COLUMNSTORE INDEX IX_EI01_Columnstore
ON EI01 (MA_CN, MA_KH, SO_DU_CUOI_KY, NGAY_DL, LAI_SUAT);

PRINT '✅ EI01 created with Temporal + Columnstore'

-- 3.4: GL01 - Partitioned Columnstore (SPECIAL CASE)
PRINT '🔄 Creating GL01 with Partitioned Columnstore...'
CREATE TABLE GL01 (
    -- Business columns (27 columns from CSV)
    STS nvarchar(10),
    NGAY_GD datetime, -- This maps to TR_TIME from CSV
    SO_CHUNG_TU nvarchar(50),
    DIEN_GIAI nvarchar(500),
    TK_NO nvarchar(20),
    TK_CO nvarchar(20),
    SO_TIEN decimal(18,2),
    LOAI_TIEN nvarchar(10),
    TY_GIA decimal(10,4),
    SO_TIEN_QD decimal(18,2),
    MA_CN nvarchar(10),
    MA_KH nvarchar(50),
    SO_TK nvarchar(50),
    TEN_TK nvarchar(255),
    NHOM_TK nvarchar(50),
    MA_SP nvarchar(20),
    TEN_SP nvarchar(255),
    NGUOI_TAO nvarchar(100),
    NGAY_TAO datetime,
    NGUOI_DUYET nvarchar(100),
    NGAY_DUYET datetime,
    TRANG_THAI nvarchar(20),
    REF_ID nvarchar(50),
    BATCH_ID nvarchar(50),
    CHANNEL nvarchar(50),
    DEVICE_ID nvarchar(50),
    USER_ID nvarchar(50),

    -- System columns
    Id bigint IDENTITY(1,1),
    NGAY_DL datetime NOT NULL, -- Extracted from TR_TIME column, format dd/mm/yyyy
    CREATED_DATE datetime2 DEFAULT GETDATE(),
    UPDATED_DATE datetime2 DEFAULT GETDATE(),
    FILE_NAME nvarchar(255)
) ON PS_GL01_Date(NGAY_DL); -- Partitioned by NGAY_DL

-- Create clustered columnstore index on GL01
CREATE CLUSTERED COLUMNSTORE INDEX IX_GL01_Clustered_Columnstore ON GL01
ON PS_GL01_Date(NGAY_DL);

PRINT '✅ GL01 created with Partitioned Columnstore'

-- 3.5: GL41 - Temporal Table
PRINT '🔄 Creating GL41 with Temporal + Columnstore...'
CREATE TABLE GL41 (
    -- Business columns (13 columns from CSV)
    MA_CN nvarchar(10),
    LOAI_TIEN nvarchar(10),
    MA_TK nvarchar(20),
    TEN_TK nvarchar(255),
    SO_DU_DAU_KY decimal(18,2),
    SO_DU_CUOI_KY decimal(18,2),
    TY_GIA decimal(10,4),
    SO_DU_DAU_KY_QD decimal(18,2),
    SO_DU_CUOI_KY_QD decimal(18,2),
    NHOM_TK nvarchar(50),
    CAP_TK nvarchar(10),
    TINH_CHAT_TK nvarchar(20),
    GHI_CHU nvarchar(500),

    -- System columns
    Id bigint IDENTITY(1,1) PRIMARY KEY,
    NGAY_DL datetime NOT NULL,
    CREATED_DATE datetime2 DEFAULT GETDATE(),
    UPDATED_DATE datetime2 DEFAULT GETDATE(),
    FILE_NAME nvarchar(255),

    -- Temporal columns
    ValidFrom datetime2 GENERATED ALWAYS AS ROW START,
    ValidTo datetime2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.GL41_History));

CREATE NONCLUSTERED COLUMNSTORE INDEX IX_GL41_Columnstore
ON GL41 (MA_CN, LOAI_TIEN, SO_DU_CUOI_KY, NGAY_DL);

PRINT '✅ GL41 created with Temporal + Columnstore'

-- 3.6: LN01 - Temporal Table
PRINT '🔄 Creating LN01 with Temporal + Columnstore...'
CREATE TABLE LN01 (
    -- Business columns (79 columns from CSV)
    BRCD nvarchar(10),
    CUSTSEQ int,
    CUSTNM nvarchar(255),
    TAI_KHOAN nvarchar(50),
    TEN_TAI_KHOAN nvarchar(255),
    DU_NO decimal(18,2),
    LAI_SUAT decimal(8,4),
    CCY nvarchar(10),
    RATE decimal(10,4),
    DU_NO_QD decimal(18,2),
    PROD_CODE nvarchar(20),
    PROD_NAME nvarchar(255),
    CUST_TYPE nvarchar(50),
    OPEN_DATE date,
    MATURITY_DATE date,
    TERM_DAYS int,
    LENDING_TYPE nvarchar(50),
    PURPOSE_CODE nvarchar(10),
    PURPOSE_DESC nvarchar(255),
    INDUSTRY_CODE nvarchar(10),
    INDUSTRY_DESC nvarchar(255),
    PROVINCE_CODE nvarchar(10),
    PROVINCE_NAME nvarchar(100),
    DISTRICT_CODE nvarchar(10),
    DISTRICT_NAME nvarchar(100),
    WARD_CODE nvarchar(10),
    WARD_NAME nvarchar(100),
    COLLATERAL_TYPE nvarchar(50),
    COLLATERAL_VALUE decimal(18,2),
    LTV_RATIO decimal(8,4),
    OFFICER_CODE nvarchar(20),
    OFFICER_NAME nvarchar(100),
    RISK_GROUP nvarchar(10),
    CLASSIFICATION nvarchar(20),
    PROVISION_RATE decimal(8,4),
    PROVISION_AMOUNT decimal(18,2),
    OVERDUE_DAYS int,
    LAST_PAYMENT_DATE date,
    NEXT_PAYMENT_DATE date,
    PAYMENT_FREQUENCY nvarchar(20),
    PAYMENT_METHOD nvarchar(50),
    GUARANTOR_TYPE nvarchar(50),
    GUARANTOR_NAME nvarchar(255),
    CONTRACT_NO nvarchar(50),
    DISBURSEMENT_DATE date,
    ORIGINAL_AMOUNT decimal(18,2),
    OUTSTANDING_PRINCIPAL decimal(18,2),
    ACCRUED_INTEREST decimal(18,2),
    PENALTY_INTEREST decimal(18,2),
    FEES_CHARGES decimal(18,2),
    TOTAL_OUTSTANDING decimal(18,2),
    MONTHLY_PAYMENT decimal(18,2),
    PRINCIPAL_PAYMENT decimal(18,2),
    INTEREST_PAYMENT decimal(18,2),
    LAST_INTEREST_DATE date,
    RESTRUCTURE_FLAG nvarchar(1),
    RESTRUCTURE_DATE date,
    WRITEOFF_FLAG nvarchar(1),
    WRITEOFF_DATE date,
    WRITEOFF_AMOUNT decimal(18,2),
    RECOVERY_AMOUNT decimal(18,2),
    NPL_FLAG nvarchar(1),
    NPL_DATE date,
    WATCH_FLAG nvarchar(1),
    SPECIAL_MENTION_FLAG nvarchar(1),
    SUBSTANDARD_FLAG nvarchar(1),
    DOUBTFUL_FLAG nvarchar(1),
    LOSS_FLAG nvarchar(1),
    CURRENT_FLAG nvarchar(1),
    CUSTOMER_SINCE date,
    RELATIONSHIP_TYPE nvarchar(50),
    GROUP_EXPOSURE decimal(18,2),
    LIMIT_AMOUNT decimal(18,2),
    AVAILABLE_LIMIT decimal(18,2),
    UTILIZED_AMOUNT decimal(18,2),
    EXPOSURE_RATIO decimal(8,4),
    INCOME_VERIFICATION nvarchar(50),
    EMPLOYMENT_TYPE nvarchar(50),
    BUSINESS_TYPE nvarchar(50),
    ANNUAL_INCOME decimal(18,2),
    DEBT_TO_INCOME decimal(8,4),
    CREDIT_SCORE int,
    CREDIT_GRADE nvarchar(10),
    APPROVAL_AUTHORITY nvarchar(50),

    -- System columns
    Id bigint IDENTITY(1,1) PRIMARY KEY,
    NGAY_DL datetime NOT NULL,
    CREATED_DATE datetime2 DEFAULT GETDATE(),
    UPDATED_DATE datetime2 DEFAULT GETDATE(),
    FILE_NAME nvarchar(255),

    -- Temporal columns
    ValidFrom datetime2 GENERATED ALWAYS AS ROW START,
    ValidTo datetime2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.LN01_History));

CREATE NONCLUSTERED COLUMNSTORE INDEX IX_LN01_Columnstore
ON LN01 (BRCD, CUSTSEQ, DU_NO, NGAY_DL, LAI_SUAT, CCY);

PRINT '✅ LN01 created with Temporal + Columnstore'

-- 3.7: LN03 - Temporal Table
PRINT '🔄 Creating LN03 with Temporal + Columnstore...'
CREATE TABLE LN03 (
    -- Business columns (17 columns from CSV)
    MACHINHANH nvarchar(10),
    TENCHINHANH nvarchar(255),
    MAKHACHHANG nvarchar(50),
    TENKHACHHANG nvarchar(255),
    SOTAIKHOAN nvarchar(50),
    TENTAIKHOAN nvarchar(255),
    SODUNO decimal(18,2),
    LAISUAT decimal(8,4),
    LOAITIEN nvarchar(10),
    TYGIA decimal(10,4),
    SODUNOQD decimal(18,2),
    MASP nvarchar(20),
    TENSP nvarchar(255),
    NHOMKH nvarchar(50),
    NGAYMO date,
    NGAYDAOHAN date,
    GHICHU nvarchar(500),

    -- System columns
    Id bigint IDENTITY(1,1) PRIMARY KEY,
    NGAY_DL datetime NOT NULL,
    CREATED_DATE datetime2 DEFAULT GETDATE(),
    UPDATED_DATE datetime2 DEFAULT GETDATE(),
    FILE_NAME nvarchar(255),

    -- Temporal columns
    ValidFrom datetime2 GENERATED ALWAYS AS ROW START,
    ValidTo datetime2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.LN03_History));

CREATE NONCLUSTERED COLUMNSTORE INDEX IX_LN03_Columnstore
ON LN03 (MACHINHANH, MAKHACHHANG, SODUNO, NGAY_DL);

PRINT '✅ LN03 created with Temporal + Columnstore'

-- 3.8: RR01 - Temporal Table
PRINT '🔄 Creating RR01 with Temporal + Columnstore...'
CREATE TABLE RR01 (
    -- Business columns (25 columns from CSV)
    CN_LOAI_I nvarchar(10),
    BRCD nvarchar(10),
    MA_KH nvarchar(50),
    TEN_KH nvarchar(255),
    SO_LDS nvarchar(50),
    TEN_LDS nvarchar(255),
    DU_NO decimal(18,2),
    NHOM_NO nvarchar(10),
    PHAN_LOAI_NO nvarchar(10),
    TY_LE_RRRR decimal(8,4),
    DU_PHONG_RRRR decimal(18,2),
    LOAI_TIEN nvarchar(10),
    TY_GIA decimal(10,4),
    DU_NO_QD decimal(18,2),
    DU_PHONG_RRRR_QD decimal(18,2),
    MA_SP nvarchar(20),
    TEN_SP nvarchar(255),
    NHOM_KH nvarchar(50),
    NGAY_DAO_HAN date,
    SO_NGAY_QUA_HAN int,
    HINH_THUC_TD nvarchar(50),
    MUC_DICH_VAY nvarchar(255),
    NGANH_KT nvarchar(100),
    GHI_CHU nvarchar(500),
    NGUOI_QUAN_LY nvarchar(100),

    -- System columns
    Id bigint IDENTITY(1,1) PRIMARY KEY,
    NGAY_DL datetime NOT NULL,
    CREATED_DATE datetime2 DEFAULT GETDATE(),
    UPDATED_DATE datetime2 DEFAULT GETDATE(),
    FILE_NAME nvarchar(255),

    -- Temporal columns
    ValidFrom datetime2 GENERATED ALWAYS AS ROW START,
    ValidTo datetime2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.RR01_History));

CREATE NONCLUSTERED COLUMNSTORE INDEX IX_RR01_Columnstore
ON RR01 (CN_LOAI_I, BRCD, DU_NO, NGAY_DL, NHOM_NO);

PRINT '✅ RR01 created with Temporal + Columnstore'

-- Verification
PRINT '🔍 Verifying table creation...'
SELECT
    t.TABLE_NAME,
    CASE
        WHEN t.TABLE_NAME = 'GL01' THEN 'Partitioned Columnstore'
        ELSE 'Temporal + Columnstore'
    END AS TableType,
    COUNT(c.COLUMN_NAME) AS ColumnCount
FROM INFORMATION_SCHEMA.TABLES t
LEFT JOIN INFORMATION_SCHEMA.COLUMNS c ON t.TABLE_NAME = c.TABLE_NAME
WHERE t.TABLE_NAME IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')
GROUP BY t.TABLE_NAME
ORDER BY t.TABLE_NAME;

PRINT '🎉 All 8 data tables created successfully with new configuration!'
PRINT '📊 Summary:'
PRINT '   - GL01: Partitioned Columnstore with NGAY_DL from TR_TIME'
PRINT '   - 7 other tables: Temporal Tables + Columnstore with NGAY_DL from filename'
PRINT '   - All tables have datetime NGAY_DL column'
PRINT '   - Business columns positioned first, system columns last'
