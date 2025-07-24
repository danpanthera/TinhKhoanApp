-- Fix All 5 Temporal Tables Structure
-- Problem: All 5 newly created tables have CREATED_DATE/UPDATED_DATE as temporal columns instead of SysStartTime/SysEndTime

-- Fix EI01
DROP TABLE IF EXISTS EI01;
DROP TABLE IF EXISTS EI01_History;

CREATE TABLE EI01 (
    -- Business columns
    NGAY_DL datetime2 NOT NULL,
    MA_CN nvarchar(255) NOT NULL DEFAULT '',
    MA_DV nvarchar(255) NOT NULL DEFAULT '',
    MA_NHAN_VIEN nvarchar(255) NOT NULL DEFAULT '',
    TEN_NHAN_VIEN nvarchar(255) NOT NULL DEFAULT '',
    CMND nvarchar(255) NOT NULL DEFAULT '',
    CHUC_VU nvarchar(255) NOT NULL DEFAULT '',
    TRINH_DO nvarchar(255) NOT NULL DEFAULT '',
    LOAI_HD nvarchar(255) NOT NULL DEFAULT '',
    TRANG_THAI nvarchar(255) NOT NULL DEFAULT '',
    NGAY_VAO_LAM datetime2 NOT NULL DEFAULT '1900-01-01',

    -- System columns
    Id bigint IDENTITY(1,1) NOT NULL,
    CREATED_DATE datetime2 NOT NULL,
    UPDATED_DATE datetime2 NOT NULL,
    FILE_NAME nvarchar(255) NOT NULL DEFAULT '',

    -- Proper temporal columns
    SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN,
    SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),

    CONSTRAINT PK_EI01 PRIMARY KEY (Id)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.EI01_History));

-- Fix GL41
DROP TABLE IF EXISTS GL41;
DROP TABLE IF EXISTS GL41_History;

CREATE TABLE GL41 (
    -- Business columns
    NGAY_DL datetime2 NOT NULL,
    MA_CN nvarchar(255) NOT NULL DEFAULT '',
    TKKT nvarchar(255) NOT NULL DEFAULT '',
    TKDT nvarchar(255) NOT NULL DEFAULT '',
    CCYCODE nvarchar(255) NOT NULL DEFAULT '',
    BALANCE decimal(18,2) NOT NULL DEFAULT 0,

    -- System columns
    Id bigint IDENTITY(1,1) NOT NULL,
    CREATED_DATE datetime2 NOT NULL,
    UPDATED_DATE datetime2 NOT NULL,
    FILE_NAME nvarchar(255) NOT NULL DEFAULT '',

    -- Proper temporal columns
    SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN,
    SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),

    CONSTRAINT PK_GL41 PRIMARY KEY (Id)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.GL41_History));

-- Fix LN03
DROP TABLE IF EXISTS LN03;
DROP TABLE IF EXISTS LN03_History;

CREATE TABLE LN03 (
    -- Business columns
    NGAY_DL datetime2 NOT NULL,
    MA_CN nvarchar(255) NOT NULL DEFAULT '',
    MA_KH nvarchar(255) NOT NULL DEFAULT '',
    TEN_KH nvarchar(255) NOT NULL DEFAULT '',
    SO_HD nvarchar(255) NOT NULL DEFAULT '',
    NGAY_CAP_TD datetime2 NOT NULL DEFAULT '1900-01-01',
    NGAY_DAO_HAN datetime2 NOT NULL DEFAULT '1900-01-01',
    DU_NO decimal(18,2) NOT NULL DEFAULT 0,
    CCY nvarchar(255) NOT NULL DEFAULT '',
    NGANH_KT nvarchar(255) NOT NULL DEFAULT '',
    NGANH_EBANK nvarchar(255) NOT NULL DEFAULT '',
    PHAN_LOAI nvarchar(255) NOT NULL DEFAULT '',

    -- System columns
    Id bigint IDENTITY(1,1) NOT NULL,
    CREATED_DATE datetime2 NOT NULL,
    UPDATED_DATE datetime2 NOT NULL,
    FILE_NAME nvarchar(255) NOT NULL DEFAULT '',

    -- Proper temporal columns
    SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN,
    SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),

    CONSTRAINT PK_LN03 PRIMARY KEY (Id)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.LN03_History));

-- Fix RR01
DROP TABLE IF EXISTS RR01;
DROP TABLE IF EXISTS RR01_History;

CREATE TABLE RR01 (
    -- Business columns
    NGAY_DL datetime2 NOT NULL,
    MA_CN nvarchar(255) NOT NULL DEFAULT '',
    MA_KH nvarchar(255) NOT NULL DEFAULT '',
    TEN_KH nvarchar(255) NOT NULL DEFAULT '',
    CCYCODE nvarchar(255) NOT NULL DEFAULT '',
    SO_DU decimal(18,2) NOT NULL DEFAULT 0,
    NGANH_KT nvarchar(255) NOT NULL DEFAULT '',
    NGANH_EBANK nvarchar(255) NOT NULL DEFAULT '',
    LOAI_TK nvarchar(255) NOT NULL DEFAULT '',

    -- System columns
    Id bigint IDENTITY(1,1) NOT NULL,
    CREATED_DATE datetime2 NOT NULL,
    UPDATED_DATE datetime2 NOT NULL,
    FILE_NAME nvarchar(255) NOT NULL DEFAULT '',

    -- Proper temporal columns
    SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN,
    SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),

    CONSTRAINT PK_RR01 PRIMARY KEY (Id)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.RR01_History));

-- Create performance indexes for all 4 remaining tables
CREATE NONCLUSTERED INDEX IX_EI01_NGAY_DL ON EI01 (NGAY_DL);
CREATE NONCLUSTERED INDEX IX_GL41_NGAY_DL ON GL41 (NGAY_DL);
CREATE NONCLUSTERED INDEX IX_LN03_NGAY_DL ON LN03 (NGAY_DL);
CREATE NONCLUSTERED INDEX IX_RR01_NGAY_DL ON RR01 (NGAY_DL);

PRINT '✅ All 4 remaining temporal tables recreated successfully with proper SysStartTime/SysEndTime columns';
