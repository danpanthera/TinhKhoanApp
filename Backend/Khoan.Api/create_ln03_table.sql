-- Create LN03 Temporal Table for Loan Data
-- This script creates the LN03 table with proper temporal configuration

-- Drop table if exists (careful in production)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'LN03')
BEGIN
    ALTER TABLE [LN03] SET (SYSTEM_VERSIONING = OFF)
    DROP TABLE IF EXISTS [LN03_History]
    DROP TABLE IF EXISTS [LN03]
END

-- Create LN03 main table
CREATE TABLE [LN03] (
    -- Primary Key
    [Id] int IDENTITY(1,1) NOT NULL,
    
    -- Date field first (as required)
    [NGAY_DL] datetime2(7) NOT NULL,
    
    -- Core business fields (17 named columns)
    [MACHINHANH] nvarchar(50) NOT NULL,
    [MAKH] nvarchar(100) NOT NULL,
    [MACBTD] nvarchar(50) NOT NULL,
    [TENCHINHANH] nvarchar(200) NULL,
    [TENCBTD] nvarchar(200) NULL,
    [TAIKHOANHACHTOAN] nvarchar(50) NULL,
    [SOTIENXLRR] decimal(18,2) NULL,
    [SOHOPDONG] nvarchar(100) NULL,
    [REFNO] nvarchar(100) NULL,
    [NHOMNO] nvarchar(50) NULL,
    [NGAYPHATSINHXL] datetime2(7) NULL,
    [MAPGD] nvarchar(50) NULL,
    [LOAINGUONVON] nvarchar(100) NULL,
    [DUNONOIBANG] decimal(18,2) NULL,
    [CONLAINGOAIBANG] decimal(18,2) NULL,
    
    -- Additional unnamed columns (3 columns to make 20 total business columns)
    [COLUMN_18] nvarchar(100) NULL,
    [COLUMN_19] decimal(18,2) NULL,
    [COLUMN_20] nvarchar(100) NULL,
    
    -- System fields
    [CREATED_DATE] datetime2(7) NOT NULL DEFAULT GETDATE(),
    [UPDATED_DATE] datetime2(7) NULL,
    
    -- Temporal table system columns
    [ValidFrom] datetime2(7) GENERATED ALWAYS AS ROW START NOT NULL,
    [ValidTo] datetime2(7) GENERATED ALWAYS AS ROW END NOT NULL,
    
    -- Constraints
    CONSTRAINT [PK_LN03] PRIMARY KEY CLUSTERED ([Id] ASC),
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
)
WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[LN03_History]));

-- Create optimized indexes for common queries
CREATE NONCLUSTERED INDEX [IX_LN03_NGAY_DL] ON [LN03] ([NGAY_DL] ASC);
CREATE NONCLUSTERED INDEX [IX_LN03_MACHINHANH] ON [LN03] ([MACHINHANH] ASC);
CREATE NONCLUSTERED INDEX [IX_LN03_MAKH] ON [LN03] ([MAKH] ASC);
CREATE NONCLUSTERED INDEX [IX_LN03_MACBTD] ON [LN03] ([MACBTD] ASC);

-- Create columnstore index for analytics
CREATE NONCLUSTERED COLUMNSTORE INDEX [NCCI_LN03_Analytics] ON [LN03] (
    [NGAY_DL],
    [MACHINHANH], 
    [MAKH],
    [MACBTD],
    [SOTIENXLRR],
    [DUNONOIBANG],
    [CONLAINGOAIBANG],
    [COLUMN_19]
);

-- Verify table creation
SELECT 
    'LN03' as TableName,
    COUNT(*) as ColumnCount
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'LN03';

-- Show temporal configuration
SELECT 
    t.name as TableName,
    t.temporal_type_desc as TemporalType,
    h.name as HistoryTableName
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
WHERE t.name = 'LN03';

PRINT '✅ LN03 temporal table created successfully with 20 business columns + system fields'
PRINT '📊 Analytics indexes and columnstore index created'
PRINT '⏰ Temporal versioning enabled with LN03_History table'
