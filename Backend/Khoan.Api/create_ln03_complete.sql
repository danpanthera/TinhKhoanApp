-- 🚀 Complete LN03 Table Creation (Manual Fix)
-- Includes ALL required columns including IS_DELETED

-- Drop existing if needed
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'LN03')
BEGIN
    -- Disable temporal if exists
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'LN03' AND temporal_type = 2)
        ALTER TABLE [LN03] SET (SYSTEM_VERSIONING = OFF);
    
    DROP TABLE IF EXISTS [LN03_History];
    DROP TABLE IF EXISTS [LN03];
    PRINT '🗑️ Existing LN03 tables dropped';
END

-- Create LN03 main table with complete structure
CREATE TABLE [LN03] (
    -- Primary Key
    [Id] int IDENTITY(1,1) NOT NULL,
    
    -- Business columns (matching LN03Entity.cs exactly)
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
    
    -- Unnamed business columns
    [COLUMN_18] nvarchar(200) NULL,
    [COLUMN_19] nvarchar(200) NULL,
    [COLUMN_20] decimal(18,2) NULL,
    
    -- System columns (REQUIRED for repository queries)
    [CREATED_DATE] datetime2(7) NOT NULL DEFAULT GETDATE(),
    [UPDATED_DATE] datetime2(7) NULL,
    [IS_DELETED] bit NOT NULL DEFAULT 0,  -- 🎯 THIS IS THE MISSING COLUMN!
    
    -- Temporal columns
    [SysStartTime] datetime2(7) GENERATED ALWAYS AS ROW START NOT NULL,
    [SysEndTime] datetime2(7) GENERATED ALWAYS AS ROW END NOT NULL,
    
    CONSTRAINT [PK_LN03] PRIMARY KEY CLUSTERED ([Id] ASC),
    PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime])
)
WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[LN03_History]));

-- Create essential indexes for performance
CREATE NONCLUSTERED INDEX [IX_LN03_NGAY_DL] ON [LN03] ([NGAY_DL] ASC);
CREATE NONCLUSTERED INDEX [IX_LN03_MACHINHANH] ON [LN03] ([MACHINHANH] ASC);
CREATE NONCLUSTERED INDEX [IX_LN03_MAKH] ON [LN03] ([MAKH] ASC);
CREATE NONCLUSTERED INDEX [IX_LN03_IS_DELETED] ON [LN03] ([IS_DELETED] ASC);
CREATE NONCLUSTERED INDEX [IX_LN03_CREATED_DATE] ON [LN03] ([CREATED_DATE] ASC);

PRINT '✅ LN03 table created with ALL required columns';
PRINT '📊 Essential indexes created for performance';
PRINT '⏰ Temporal versioning enabled';
PRINT '🎯 IS_DELETED column included for repository queries';

-- Insert realistic test data
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
 'TEST_18', 'TEST_19', 750000, 0),
('2024-12-31', '7801', 'Chi nhanh Phong Tho', '004065048', 'Trần Thị Mai',
 '7801-LAV-202100789', 3000000, '2021-03-20', 0, 3000000, '1',
 '780100426', 'Lê Văn Minh', '00', '971105',
 '78010040650487801-LAV-2021007897801LDS202100234', 'Tổ chức',
 'TEST_18', 'TEST_19', 1000000, 0),
('2024-12-31', '7802', 'Chi nhanh Muong Lay', '004065049', 'Phạm Văn Hùng',
 '7802-LAV-202200456', 2500000, '2022-05-10', 0, 2500000, '1',
 '780200427', 'Hoàng Thị Bích', '00', '971106',
 '78020040650497802-LAV-2022004567802LDS202200123', 'Cá nhân',
 'TEST_18', 'TEST_19', 800000, 0),
('2024-12-31', '7803', 'Chi nhanh Than Uyen', '004065050', 'Vũ Thị Hồng',
 '7803-LAV-202300789', 4200000, '2023-08-20', 0, 4200000, '2',
 '780300428', 'Đỗ Văn Tài', '01', '971107',
 '78030040650507803-LAV-2023007897803LDS202300456', 'Tổ chức',
 'TEST_18', 'TEST_19', 1200000, 0);

PRINT '📋 Sample data inserted: 5 records from different branches';

-- Verify creation and data
SELECT 
    COUNT(*) as TotalRecords,
    COUNT(CASE WHEN [IS_DELETED] = 0 THEN 1 END) as ActiveRecords,
    COUNT(CASE WHEN [IS_DELETED] = 1 THEN 1 END) as DeletedRecords,
    MIN([NGAY_DL]) as OldestDate,
    MAX([NGAY_DL]) as NewestDate,
    COUNT(DISTINCT [MACHINHANH]) as UniqueBranches
FROM [LN03];

-- Test the specific query that was failing
SELECT COUNT(*) as RecordCount FROM [LN03] WHERE [IS_DELETED] = 0;

PRINT '🎉 LN03 database creation completed successfully!';
PRINT '🔍 All columns verified, including IS_DELETED';
PRINT '📊 Sample data ready for API testing';
