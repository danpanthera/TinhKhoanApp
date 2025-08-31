-- LN03 Table Creation Script (Manual Fix)
-- Drop existing if needed
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'LN03')
BEGIN
    -- Disable temporal if exists
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'LN03' AND temporal_type = 2)
        ALTER TABLE [LN03] SET (SYSTEM_VERSIONING = OFF)
    
    DROP TABLE IF EXISTS [LN03_History]
    DROP TABLE IF EXISTS [LN03]
    PRINT '🗑️ Existing LN03 tables dropped'
END

-- Create LN03 main table with proper structure
CREATE TABLE [LN03] (
    [Id] int IDENTITY(1,1) NOT NULL,
    
    -- Business columns (match CSV structure)
    [NGAY_DL] datetime2(7) NOT NULL,
    [MACHINHANH] nvarchar(50) NOT NULL,
    [TENCHINHANH] nvarchar(200) NULL,
    [MAKH] nvarchar(100) NOT NULL,
    [TENKH] nvarchar(200) NULL,
    [SOHOPDONG] nvarchar(100) NULL,
    [SOTIENXLRR] decimal(18,2) NULL,
    [NGAYPHATSINHXL] datetime2(7) NULL,
    [THUNOSAUXL] decimal(18,2) NULL,
    [CONLAINGOAIBANG] decimal(18,2) NULL,
    [DUNONOIBANG] decimal(18,2) NULL,
    [NHOMNO] nvarchar(50) NULL,
    [MACBTD] nvarchar(50) NULL,
    [TENCBTD] nvarchar(200) NULL,
    [MAPGD] nvarchar(50) NULL,
    [TAIKHOANHACHTOAN] nvarchar(50) NULL,
    [REFNO] nvarchar(200) NULL,
    [LOAINGUONVON] nvarchar(100) NULL,
    
    -- Additional columns to meet 20 column requirement
    [COLUMN_18] nvarchar(100) NULL,
    [COLUMN_19] decimal(18,2) NULL,
    [COLUMN_20] nvarchar(100) NULL,
    
    -- System fields
    [CREATED_DATE] datetime2(7) NOT NULL DEFAULT GETDATE(),
    [UPDATED_DATE] datetime2(7) NULL,
    
    -- Temporal columns (simplified)
    [ValidFrom] datetime2(7) GENERATED ALWAYS AS ROW START NOT NULL,
    [ValidTo] datetime2(7) GENERATED ALWAYS AS ROW END NOT NULL,
    
    CONSTRAINT [PK_LN03] PRIMARY KEY CLUSTERED ([Id] ASC),
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
)
WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[LN03_History]));

-- Create essential indexes
CREATE NONCLUSTERED INDEX [IX_LN03_NGAY_DL] ON [LN03] ([NGAY_DL] ASC);
CREATE NONCLUSTERED INDEX [IX_LN03_MACHINHANH] ON [LN03] ([MACHINHANH] ASC);
CREATE NONCLUSTERED INDEX [IX_LN03_MAKH] ON [LN03] ([MAKH] ASC);

PRINT '✅ LN03 table created successfully'
PRINT '📊 Essential indexes created'
PRINT '⏰ Temporal versioning enabled'

-- Insert sample data for testing
INSERT INTO [LN03] (
    [NGAY_DL], [MACHINHANH], [TENCHINHANH], [MAKH], [TENKH], 
    [SOHOPDONG], [SOTIENXLRR], [NGAYPHATSINHXL], [DUNONOIBANG], 
    [CONLAINGOAIBANG], [NHOMNO], [MACBTD], [TENCBTD], [MAPGD], 
    [TAIKHOANHACHTOAN], [REFNO], [LOAINGUONVON], 
    [COLUMN_18], [COLUMN_19], [COLUMN_20]
) VALUES 
('2024-12-31', '7800', 'Chi nhanh Tinh Lai Chau', '004065046', 'Bùi Thị Linh', 
 '7800-LAV-201500567', 1000000, '2019-06-28', 0, 1000000, '1', 
 '780000424', 'Nguyễn Văn Hùng', '00', '971103', 
 '78000040650467800-LAV-2015005677800LDS201500736', 'Cá nhân',
 'TEST_18', 500000, 'TEST_20'),
('2024-12-31', '7800', 'Chi nhanh Tinh Lai Chau', '004065047', 'Nguyễn Văn Nam',
 '7800-LAV-201600123', 5000000, '2020-01-15', 0, 5000000, '2',
 '780000425', 'Trần Thị Lan', '01', '971104',
 '78000040650477800-LAV-2016001237800LDS201600456', 'Cá nhân',
 'TEST_18', 750000, 'TEST_20'),
('2024-12-31', '7801', 'Chi nhanh Phong Tho', '004065048', 'Trần Thị Mai',
 '7801-LAV-202100789', 3000000, '2021-03-20', 0, 3000000, '1',
 '780100426', 'Lê Văn Minh', '00', '971105',
 '78010040650487801-LAV-2021007897801LDS202100234', 'Tổ chức',
 'TEST_18', 1000000, 'TEST_20');

PRINT '📋 Sample data inserted: 3 records'

-- Verify creation
SELECT 
    COUNT(*) as RecordCount,
    MIN([NGAY_DL]) as OldestDate,
    MAX([NGAY_DL]) as NewestDate
FROM [LN03];

PRINT '🎉 LN03 database fix completed successfully!'
