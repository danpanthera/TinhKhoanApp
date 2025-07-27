-- ============================================
-- RR01 TABLE CREATION SCRIPT
-- Risk Rating/Recovery Report - 25 Business Columns
-- Temporal Table + Columnstore Index Configuration
-- Direct Import from *rr01* CSV files only
-- ============================================

USE TinhKhoanDB;
GO

-- Drop existing table if it exists (for clean recreation)
IF OBJECT_ID('dbo.RR01', 'U') IS NOT NULL
BEGIN
    PRINT '🗑️ Dropping existing RR01 table...'

    -- Disable temporal if exists
    IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RR01' AND temporal_type = 2)
    BEGIN
        ALTER TABLE [dbo].[RR01] SET (SYSTEM_VERSIONING = OFF);
        DROP TABLE IF EXISTS [dbo].[RR01_History];
    END

    DROP TABLE [dbo].[RR01];
    PRINT '✅ RR01 table dropped successfully'
END

-- Create RR01 table with proper structure
PRINT '🏗️ Creating RR01 table with 25 business columns + system columns...'

CREATE TABLE [dbo].[RR01] (
    -- Primary key
    [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,

    -- SYSTEM COLUMN FIRST - NGAY_DL (extracted from filename)
    [NGAY_DL] DATETIME2 NOT NULL,

    -- =================== BUSINESS COLUMNS (25 total) - CSV ORDER ===================

    -- String columns (8 + VAMC_FLG)
    [CN_LOAI_I] NVARCHAR(200) NULL,
    [BRCD] NVARCHAR(200) NULL,
    [MA_KH] NVARCHAR(200) NULL,
    [TEN_KH] NVARCHAR(200) NULL,
    [SO_LDS] NVARCHAR(200) NULL,
    [CCY] NVARCHAR(200) NULL,
    [SO_LAV] NVARCHAR(200) NULL,
    [LOAI_KH] NVARCHAR(200) NULL,
    [VAMC_FLG] NVARCHAR(200) NULL,

    -- Date columns (3 datetime2 columns)
    [NGAY_GIAI_NGAN] DATETIME2 NULL,
    [NGAY_DEN_HAN] DATETIME2 NULL,
    [NGAY_XLRR] DATETIME2 NULL,

    -- Amount/Financial columns (13 decimal columns)
    [DUNO_GOC_BAN_DAU] DECIMAL(18,2) NULL,
    [DUNO_LAI_TICHLUY_BD] DECIMAL(18,2) NULL,
    [DOC_DAUKY_DA_THU_HT] DECIMAL(18,2) NULL,
    [DUNO_GOC_HIENTAI] DECIMAL(18,2) NULL,
    [DUNO_LAI_HIENTAI] DECIMAL(18,2) NULL,
    [DUNO_NGAN_HAN] DECIMAL(18,2) NULL,
    [DUNO_TRUNG_HAN] DECIMAL(18,2) NULL,
    [DUNO_DAI_HAN] DECIMAL(18,2) NULL,
    [THU_GOC] DECIMAL(18,2) NULL,
    [THU_LAI] DECIMAL(18,2) NULL,
    [BDS] DECIMAL(18,2) NULL,
    [DS] DECIMAL(18,2) NULL,
    [TSK] DECIMAL(18,2) NULL,

    -- =================== SYSTEM COLUMNS ===================
    [FILE_NAME] NVARCHAR(200) NULL,
    [IMPORT_BATCH_ID] NVARCHAR(200) NULL,
    [DATA_SOURCE] NVARCHAR(200) NULL DEFAULT 'DirectImport',
    [PROCESSING_STATUS] NVARCHAR(50) NULL DEFAULT 'Completed',
    [ERROR_MESSAGE] NVARCHAR(1000) NULL,
    [ROW_HASH] NVARCHAR(100) NULL,

    -- Temporal columns (will be configured as shadow properties)
    [CREATED_DATE] DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT GETUTCDATE(),
    [UPDATED_DATE] DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT GETUTCDATE(),

    PERIOD FOR SYSTEM_TIME ([CREATED_DATE], [UPDATED_DATE])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[RR01_History]));

PRINT '✅ RR01 table created successfully with temporal support'

-- Create indexes for optimal performance
PRINT '🔍 Creating indexes for RR01...'

-- Business logic indexes
CREATE NONCLUSTERED INDEX [IX_RR01_NGAY_DL] ON [dbo].[RR01] ([NGAY_DL] DESC);
CREATE NONCLUSTERED INDEX [IX_RR01_MA_KH] ON [dbo].[RR01] ([MA_KH]);
CREATE NONCLUSTERED INDEX [IX_RR01_FILE_NAME] ON [dbo].[RR01] ([FILE_NAME]);
CREATE NONCLUSTERED INDEX [IX_RR01_IMPORT_BATCH_ID] ON [dbo].[RR01] ([IMPORT_BATCH_ID]);

-- Composite indexes for common queries
CREATE NONCLUSTERED INDEX [IX_RR01_NGAY_DL_MA_KH] ON [dbo].[RR01] ([NGAY_DL] DESC, [MA_KH]);

PRINT '✅ RR01 indexes created successfully'

-- Create columnstore index for analytical queries
PRINT '🏛️ Creating columnstore index for RR01...'

CREATE NONCLUSTERED COLUMNSTORE INDEX [NCCI_RR01_Analytics]
ON [dbo].[RR01] (
    [NGAY_DL], [MA_KH], [TEN_KH],
    [DUNO_GOC_BAN_DAU], [DUNO_GOC_HIENTAI], [DUNO_LAI_HIENTAI],
    [THU_GOC], [THU_LAI], [BDS], [DS], [TSK],
    [FILE_NAME], [CREATED_DATE]
);

PRINT '✅ RR01 columnstore index created successfully'

-- Verify table structure
PRINT '🔍 Verifying RR01 table structure...'

SELECT
    '✅ Table Verification' as Status,
    COUNT(*) as TotalColumns,
    SUM(CASE WHEN DATA_TYPE IN ('datetime2') THEN 1 ELSE 0 END) as DateTimeColumns,
    SUM(CASE WHEN DATA_TYPE IN ('decimal', 'numeric') THEN 1 ELSE 0 END) as DecimalColumns,
    SUM(CASE WHEN DATA_TYPE IN ('nvarchar') THEN 1 ELSE 0 END) as StringColumns
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'RR01' AND TABLE_SCHEMA = 'dbo';

-- Show column details
PRINT '📋 RR01 Column Structure:'
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    NUMERIC_PRECISION,
    NUMERIC_SCALE,
    IS_NULLABLE,
    ORDINAL_POSITION
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'RR01' AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;

PRINT '🎉 RR01 table creation completed successfully!'
PRINT '📊 Ready for direct import from *rr01* CSV files'
PRINT '🔄 Temporal versioning enabled for data history tracking'
PRINT '🏛️ Columnstore index optimized for analytical queries'
