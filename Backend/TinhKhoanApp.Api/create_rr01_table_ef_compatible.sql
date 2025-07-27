-- Create RR01 table with EF-compatible temporal configuration
-- 25 Business Columns + System Columns + Temporal Versioning + Columnstore Index

USE TinhKhoanDB;
GO

-- Create RR01 table with correct temporal columns for EF
CREATE TABLE RR01 (
    -- Primary Key
    Id BIGINT IDENTITY(1,1) NOT NULL,

    -- System column FIRST
    NGAY_DL DATETIME2 NOT NULL,

    -- Business columns (25 total) - CSV ORDER
    CN_LOAI_I NVARCHAR(200) NULL,
    BRCD NVARCHAR(200) NULL,
    MA_KH NVARCHAR(200) NULL,
    TEN_KH NVARCHAR(200) NULL,
    SO_LDS NVARCHAR(200) NULL,
    CCY NVARCHAR(200) NULL,
    SO_LAV NVARCHAR(200) NULL,
    LOAI_KH NVARCHAR(200) NULL,
    NGAY_GIAI_NGAN DATETIME2 NULL,
    NGAY_DEN_HAN DATETIME2 NULL,
    VAMC_FLG NVARCHAR(200) NULL,
    NGAY_XLRR DATETIME2 NULL,
    DUNO_GOC_BAN_DAU DECIMAL(18,2) NULL,
    DUNO_LAI_TICHLUY_BD DECIMAL(18,2) NULL,
    DOC_DAUKY_DA_THU_HT DECIMAL(18,2) NULL,
    DUNO_GOC_HIENTAI DECIMAL(18,2) NULL,
    DUNO_LAI_HIENTAI DECIMAL(18,2) NULL,
    DUNO_NGAN_HAN DECIMAL(18,2) NULL,
    DUNO_TRUNG_HAN DECIMAL(18,2) NULL,
    DUNO_DAI_HAN DECIMAL(18,2) NULL,
    THU_GOC DECIMAL(18,2) NULL,
    THU_LAI DECIMAL(18,2) NULL,
    BDS DECIMAL(18,2) NULL,
    DS DECIMAL(18,2) NULL,
    TSK DECIMAL(18,2) NULL,

    -- System columns
    FILE_NAME NVARCHAR(200) NULL,
    CREATED_DATE DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UPDATED_DATE DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IMPORT_BATCH_ID NVARCHAR(200) NULL,
    DATA_SOURCE NVARCHAR(200) NULL,
    PROCESSING_STATUS NVARCHAR(50) NULL,
    ERROR_MESSAGE NVARCHAR(1000) NULL,
    ROW_HASH NVARCHAR(100) NULL,

    -- Temporal columns (EF expects ValidFrom and ValidTo)
    ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,

    -- Primary key constraint
    CONSTRAINT PK_RR01 PRIMARY KEY CLUSTERED (Id),

    -- Temporal period definition
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
)
WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.RR01_History));
GO

-- Create business indexes
CREATE INDEX IX_RR01_NGAY_DL ON RR01 (NGAY_DL);
CREATE INDEX IX_RR01_MA_KH ON RR01 (MA_KH);
CREATE INDEX IX_RR01_NGAY_DL_MA_KH ON RR01 (NGAY_DL, MA_KH);
CREATE INDEX IX_RR01_FILE_NAME ON RR01 (FILE_NAME);
CREATE INDEX IX_RR01_IMPORT_BATCH_ID ON RR01 (IMPORT_BATCH_ID);
GO

-- Create columnstore index for analytical queries (non-clustered)
CREATE NONCLUSTERED COLUMNSTORE INDEX NCCX_RR01_Analytics
ON RR01 (
    NGAY_DL, MA_KH, TEN_KH,
    DUNO_GOC_BAN_DAU, DUNO_LAI_TICHLUY_BD, DOC_DAUKY_DA_THU_HT,
    DUNO_GOC_HIENTAI, DUNO_LAI_HIENTAI,
    DUNO_NGAN_HAN, DUNO_TRUNG_HAN, DUNO_DAI_HAN,
    THU_GOC, THU_LAI, BDS, DS, TSK
);
GO

-- Verify table structure
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH,
    NUMERIC_PRECISION,
    NUMERIC_SCALE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'RR01'
ORDER BY ORDINAL_POSITION;

-- Verify temporal configuration
SELECT
    t.name AS TableName,
    t.temporal_type_desc,
    SCHEMA_NAME(h.schema_id) + '.' + h.name AS HistoryTable
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
WHERE t.name = 'RR01';

-- Verify indexes
SELECT
    i.name AS IndexName,
    i.type_desc AS IndexType,
    STRING_AGG(c.name, ', ') AS Columns
FROM sys.indexes i
JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
WHERE i.object_id = OBJECT_ID('RR01')
GROUP BY i.name, i.type_desc, i.index_id
ORDER BY i.index_id;

PRINT '✅ RR01 table created successfully with 25 business columns, temporal versioning, and columnstore index!'
