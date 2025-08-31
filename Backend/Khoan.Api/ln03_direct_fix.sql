-- Direct LN03 Table Creation (No Migration Dependencies)

-- Check if LN03 table exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'LN03')
BEGIN
    PRINT '⚠️ LN03 table already exists, dropping for fresh creation'
    
    -- Disable temporal if exists
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'LN03' AND temporal_type = 2)
        ALTER TABLE [LN03] SET (SYSTEM_VERSIONING = OFF);
    
    DROP TABLE IF EXISTS [LN03_History];
    DROP TABLE IF EXISTS [LN03];
END

-- Create LN03 table (matching Entity exactly)
CREATE TABLE [LN03] (
    [Id] int IDENTITY(1,1) NOT NULL,
    
    -- Business columns
    [NGAY_DL] datetime2(7) NULL,
    [MACHINHANH] nvarchar(200) NULL,
    [TENCHINHANH] nvarchar(200) NULL,
    [MAKH] nvarchar(200) NULL,
    [TENKH] nvarchar(200) NULL,
    [SOHOPDONG] nvarchar(200) NULL,
    [SOTIENXLRR] decimal(18,2) NULL,
    [NGAYPHATSINHXL] datetime2(7) NULL,
    [THUNOSAUXL] decimal(18,2) NULL,
    [CONLAINGOAIBANG] decimal(18,2) NULL,
    [DUNONOIBANG] decimal(18,2) NULL,
    [NHOMNO] nvarchar(200) NULL,
    [MACBTD] nvarchar(200) NULL,
    [TENCBTD] nvarchar(200) NULL,
    [MAPGD] nvarchar(200) NULL,
    [TAIKHOANHACHTOAN] nvarchar(200) NULL,
    [REFNO] nvarchar(200) NULL,
    [LOAINGUONVON] nvarchar(200) NULL,
    [COLUMN_18] nvarchar(200) NULL,
    [COLUMN_19] nvarchar(200) NULL,
    [COLUMN_20] decimal(18,2) NULL,
    
    -- System columns (CRITICAL - these are used by repository)
    [CREATED_DATE] datetime2(7) NOT NULL DEFAULT GETDATE(),
    [UPDATED_DATE] datetime2(7) NULL,
    [IS_DELETED] bit NOT NULL DEFAULT 0,
    
    -- Temporal columns (EF Core style)
    [SysStartTime] datetime2(7) GENERATED ALWAYS AS ROW START NOT NULL,
    [SysEndTime] datetime2(7) GENERATED ALWAYS AS ROW END NOT NULL,
    
    CONSTRAINT [PK_LN03] PRIMARY KEY CLUSTERED ([Id] ASC),
    PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime])
)
WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[LN03_History]));

-- Create indexes for performance
CREATE NONCLUSTERED INDEX [IX_LN03_NGAY_DL] ON [LN03] ([NGAY_DL] ASC);
CREATE NONCLUSTERED INDEX [IX_LN03_MACHINHANH] ON [LN03] ([MACHINHANH] ASC);
CREATE NONCLUSTERED INDEX [IX_LN03_MAKH] ON [LN03] ([MAKH] ASC);
CREATE NONCLUSTERED INDEX [IX_LN03_IS_DELETED] ON [LN03] ([IS_DELETED] ASC);

-- Insert test data
INSERT INTO [LN03] (
    [NGAY_DL], [MACHINHANH], [TENCHINHANH], [MAKH], [TENKH], 
    [SOHOPDONG], [SOTIENXLRR], [NGAYPHATSINHXL], [DUNONOIBANG], 
    [CONLAINGOAIBANG], [NHOMNO], [MACBTD], [TENCBTD], [MAPGD], 
    [TAIKHOANHACHTOAN], [REFNO], [LOAINGUONVON], 
    [COLUMN_18], [COLUMN_19], [COLUMN_20], [IS_DELETED]
) VALUES 
('2024-12-31', '7800', 'Chi nhanh Tinh Lai Chau', '004065046', 'Bùi Thị Linh', 
 '7800-LAV-201500567', 1000000, '2019-06-28', 0, 1000000, '1', 
 '780000424', 'Nguyễn Văn Hùng', '00', '971103', 
 '78000040650467800-LAV-2015005677800LDS201500736', 'Cá nhân',
 'TEST_18', 'TEST_19', 500000, 0),
('2024-12-31', '7800', 'Chi nhanh Tinh Lai Chau', '004065047', 'Nguyễn Văn Nam',
 '7800-LAV-201600123', 5000000, '2020-01-15', 0, 5000000, '2',
 '780000425', 'Trần Thị Lan', '01', '971104',
 '78000040650477800-LAV-2016001237800LDS201600456', 'Cá nhân',
 'TEST_18', 'TEST_19', 750000, 0);

-- Verify creation
SELECT COUNT(*) as RecordCount FROM [LN03] WHERE [IS_DELETED] = 0;

PRINT '✅ LN03 table created successfully with test data'
PRINT '🔍 IS_DELETED column included for repository compatibility'
PRINT '⏰ Temporal versioning enabled'
PRINT '📊 Ready for API testing'
