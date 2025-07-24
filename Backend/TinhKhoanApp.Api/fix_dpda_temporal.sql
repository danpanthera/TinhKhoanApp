-- Fix DPDA Temporal Table Structure
-- Problem: DPDA was created with CREATED_DATE/UPDATED_DATE as temporal columns instead of SysStartTime/SysEndTime

-- Drop existing table and history (no data to lose)
DROP TABLE IF EXISTS DPDA;
DROP TABLE IF EXISTS DPDA_History;

-- Recreate DPDA with proper temporal structure
CREATE TABLE DPDA (
    -- Business columns first (following your established pattern)
    NGAY_DL datetime2 NOT NULL,
    MA_CN nvarchar(255) NOT NULL DEFAULT '',
    TAI_KHOAN_HACH_TOAN nvarchar(255) NOT NULL DEFAULT '',
    MA_KH nvarchar(255) NOT NULL DEFAULT '',
    TEN_KH nvarchar(255) NOT NULL DEFAULT '',
    NGAY_SINH datetime2 NOT NULL DEFAULT '1900-01-01',
    GIOI_TINH nvarchar(255) NOT NULL DEFAULT '',
    SO_CMND nvarchar(255) NOT NULL DEFAULT '',
    NGAY_CAP datetime2 NOT NULL DEFAULT '1900-01-01',
    NOICAP nvarchar(255) NOT NULL DEFAULT '',
    DIACHI nvarchar(255) NOT NULL DEFAULT '',
    CCY nvarchar(255) NOT NULL DEFAULT '',
    TIENGUI decimal(18,2) NOT NULL DEFAULT 0,

    -- System columns
    Id bigint IDENTITY(1,1) NOT NULL,
    CREATED_DATE datetime2 NOT NULL,
    UPDATED_DATE datetime2 NOT NULL,
    FILE_NAME nvarchar(255) NOT NULL DEFAULT '',

    -- Proper temporal columns
    SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN,
    SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),

    CONSTRAINT PK_DPDA PRIMARY KEY (Id)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.DPDA_History));

-- Create performance index
CREATE NONCLUSTERED INDEX IX_DPDA_NGAY_DL ON DPDA (NGAY_DL);

PRINT '✅ DPDA temporal table recreated successfully with proper SysStartTime/SysEndTime columns';
