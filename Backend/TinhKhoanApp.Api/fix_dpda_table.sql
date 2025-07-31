-- Fix DPDA Table Structure to Match Model
USE TinhKhoanDB;

-- Drop existing DPDA table if exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DPDA')
BEGIN
    DROP TABLE [DPDA];
    PRINT '🗑️ Dropped existing DPDA table';
END

-- Create new DPDA table with correct 18-column structure
CREATE TABLE [DPDA] (
    [NGAY_DL] datetime2(7) NULL,                    -- Order 0: Date field
    [MA_CHI_NHANH] nvarchar(200) NULL,             -- Order 1: Business columns start
    [MA_KHACH_HANG] nvarchar(200) NULL,
    [TEN_KHACH_HANG] nvarchar(200) NULL,
    [SO_TAI_KHOAN] nvarchar(200) NULL,
    [LOAI_THE] nvarchar(200) NULL,
    [SO_THE] nvarchar(200) NULL,
    [NGAY_NOP_DON] datetime2(7) NULL,
    [NGAY_PHAT_HANH] datetime2(7) NULL,
    [USER_PHAT_HANH] nvarchar(200) NULL,
    [TRANG_THAI] nvarchar(200) NULL,
    [PHAN_LOAI] nvarchar(200) NULL,
    [GIAO_THE] nvarchar(200) NULL,
    [LOAI_PHAT_HANH] nvarchar(200) NULL,           -- Business columns end
    [Id] int IDENTITY(1,1) NOT NULL,               -- System columns start
    [CREATED_DATE] datetime2(7) NOT NULL DEFAULT (getdate()),
    [UPDATED_DATE] datetime2(7) NOT NULL DEFAULT (getdate()),
    [FILE_NAME] nvarchar(255) NULL,
    -- Temporal columns (managed by EF)
    [ValidFrom] datetime2(7) GENERATED ALWAYS AS ROW START NOT NULL,
    [ValidTo] datetime2(7) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo]),
    
    CONSTRAINT [PK_DPDA] PRIMARY KEY CLUSTERED ([Id])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[DPDAHistory]));

-- Create performance index
CREATE NONCLUSTERED INDEX [IX_DPDA_NGAY_DL] ON [DPDA] ([NGAY_DL]);

PRINT '✅ Created new DPDA table with 18 columns (14 business + 4 system + temporal)';
PRINT '📊 Structure: NGAY_DL -> 13 Business Columns -> System Columns -> Temporal';
