-- Fix DPDA Table Structure - Correct Temporal Drop
USE TinhKhoanDB;

-- Disable system versioning first for temporal table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DPDA')
BEGIN
    -- Turn off system versioning first
    ALTER TABLE [DPDA] SET (SYSTEM_VERSIONING = OFF);
    PRINT '⚡ Disabled system versioning for DPDA';
    
    -- Drop history table if exists  
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DPDAHistory')
    BEGIN
        DROP TABLE [DPDAHistory];
        PRINT '🗑️ Dropped DPDAHistory table';
    END
    
    -- Drop main table
    DROP TABLE [DPDA];
    PRINT '🗑️ Dropped existing DPDA table';
END

-- Create new DPDA table with correct 18-column structure matching model
CREATE TABLE [DPDA] (
    [NGAY_DL] datetime2(7) NULL,                    -- Order 0: Date field from filename
    [MA_CHI_NHANH] nvarchar(200) NULL,             -- Order 1: Business columns start (13 total)
    [MA_KHACH_HANG] nvarchar(200) NULL,
    [TEN_KHACH_HANG] nvarchar(200) NULL,
    [SO_TAI_KHOAN] nvarchar(200) NULL,
    [LOAI_THE] nvarchar(200) NULL,
    [SO_THE] nvarchar(200) NULL,
    [NGAY_NOP_DON] datetime2(7) NULL,              -- DateTime field
    [NGAY_PHAT_HANH] datetime2(7) NULL,            -- DateTime field  
    [USER_PHAT_HANH] nvarchar(200) NULL,
    [TRANG_THAI] nvarchar(200) NULL,
    [PHAN_LOAI] nvarchar(200) NULL,
    [GIAO_THE] nvarchar(200) NULL,
    [LOAI_PHAT_HANH] nvarchar(200) NULL,           -- Business columns end
    [Id] int IDENTITY(1,1) NOT NULL,               -- System columns start
    [CREATED_DATE] datetime2(7) NOT NULL DEFAULT (getdate()),
    [UPDATED_DATE] datetime2(7) NOT NULL DEFAULT (getdate()),
    [FILE_NAME] nvarchar(255) NULL,                -- System columns end
    -- Temporal columns (shadow properties managed by EF)
    [ValidFrom] datetime2(7) GENERATED ALWAYS AS ROW START NOT NULL,
    [ValidTo] datetime2(7) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo]),
    
    CONSTRAINT [PK_DPDA] PRIMARY KEY CLUSTERED ([Id])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[DPDAHistory]));

-- Create performance index
CREATE NONCLUSTERED INDEX [IX_DPDA_NGAY_DL] ON [DPDA] ([NGAY_DL]);

-- Create columnstore index for analytics
CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_DPDA_Columnstore] ON [DPDA] 
([NGAY_DL], [MA_CHI_NHANH], [MA_KHACH_HANG], [NGAY_NOP_DON], [NGAY_PHAT_HANH]);

PRINT '✅ Created new DPDA table with correct structure:';
PRINT '📊 Total: 18 columns (NGAY_DL + 13 Business + 4 System + Temporal)';
PRINT '🔧 Features: Temporal versioning + Columnstore index';
PRINT '📋 Business columns exactly match CSV dpda structure';
