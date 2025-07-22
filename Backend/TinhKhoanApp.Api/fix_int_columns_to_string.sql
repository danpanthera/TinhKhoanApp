-- ================================================================
-- FIX INT COLUMNS TO NVARCHAR để phù hợp với CSV có giá trị rỗng
-- ================================================================

USE [TinhKhoanDB]
GO

-- ================================================================
-- LN01: Update các cột INT thành NVARCHAR
-- ================================================================

-- Disable temporal versioning
IF OBJECT_ID('dbo.LN01', 'U') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[LN01] SET (SYSTEM_VERSIONING = OFF);
    PRINT '🔄 Disabled temporal versioning for LN01';
END

-- Update columns to NVARCHAR
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.LN01') AND name = 'NHOM_NO')
BEGIN
    ALTER TABLE [dbo].[LN01] ALTER COLUMN NHOM_NO NVARCHAR(50);
    PRINT '✅ Updated LN01.NHOM_NO to NVARCHAR(50)';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.LN01') AND name = 'GRPNO')
BEGIN
    ALTER TABLE [dbo].[LN01] ALTER COLUMN GRPNO NVARCHAR(50);
    PRINT '✅ Updated LN01.GRPNO to NVARCHAR(50)';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.LN01') AND name = 'INTCMTH')
BEGIN
    ALTER TABLE [dbo].[LN01] ALTER COLUMN INTCMTH NVARCHAR(50);
    PRINT '✅ Updated LN01.INTCMTH to NVARCHAR(50)';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.LN01') AND name = 'INTRPYMTH')
BEGIN
    ALTER TABLE [dbo].[LN01] ALTER COLUMN INTRPYMTH NVARCHAR(50);
    PRINT '✅ Updated LN01.INTRPYMTH to NVARCHAR(50)';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.LN01') AND name = 'INTTRMMTH')
BEGIN
    ALTER TABLE [dbo].[LN01] ALTER COLUMN INTTRMMTH NVARCHAR(50);
    PRINT '✅ Updated LN01.INTTRMMTH to NVARCHAR(50)';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.LN01') AND name = 'YRDAYS')
BEGIN
    ALTER TABLE [dbo].[LN01] ALTER COLUMN YRDAYS NVARCHAR(50);
    PRINT '✅ Updated LN01.YRDAYS to NVARCHAR(50)';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.LN01') AND name = 'INT_PAYMENT_INTERVAL')
BEGIN
    ALTER TABLE [dbo].[LN01] ALTER COLUMN INT_PAYMENT_INTERVAL NVARCHAR(50);
    PRINT '✅ Updated LN01.INT_PAYMENT_INTERVAL to NVARCHAR(50)';
END

-- Update history table columns as well
IF OBJECT_ID('dbo.LN01_History', 'U') IS NOT NULL
BEGIN
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.LN01_History') AND name = 'NHOM_NO')
        ALTER TABLE [dbo].[LN01_History] ALTER COLUMN NHOM_NO NVARCHAR(50);

    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.LN01_History') AND name = 'GRPNO')
        ALTER TABLE [dbo].[LN01_History] ALTER COLUMN GRPNO NVARCHAR(50);

    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.LN01_History') AND name = 'INTCMTH')
        ALTER TABLE [dbo].[LN01_History] ALTER COLUMN INTCMTH NVARCHAR(50);

    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.LN01_History') AND name = 'INTRPYMTH')
        ALTER TABLE [dbo].[LN01_History] ALTER COLUMN INTRPYMTH NVARCHAR(50);

    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.LN01_History') AND name = 'INTTRMMTH')
        ALTER TABLE [dbo].[LN01_History] ALTER COLUMN INTTRMMTH NVARCHAR(50);

    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.LN01_History') AND name = 'YRDAYS')
        ALTER TABLE [dbo].[LN01_History] ALTER COLUMN YRDAYS NVARCHAR(50);

    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.LN01_History') AND name = 'INT_PAYMENT_INTERVAL')
        ALTER TABLE [dbo].[LN01_History] ALTER COLUMN INT_PAYMENT_INTERVAL NVARCHAR(50);

    PRINT '✅ Updated LN01_History columns to NVARCHAR(50)';
END

-- Re-enable temporal versioning
IF OBJECT_ID('dbo.LN01', 'U') IS NOT NULL AND OBJECT_ID('dbo.LN01_History', 'U') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[LN01] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.LN01_History));
    PRINT '🔄 Re-enabled temporal versioning for LN01';
END

-- ================================================================
-- Verify changes
-- ================================================================
SELECT
    'LN01' as TableName,
    c.COLUMN_NAME,
    c.DATA_TYPE,
    c.CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS c
WHERE c.TABLE_NAME = 'LN01'
  AND c.COLUMN_NAME IN ('NHOM_NO', 'GRPNO', 'INTCMTH', 'INTRPYMTH', 'INTTRMMTH', 'YRDAYS', 'INT_PAYMENT_INTERVAL')
ORDER BY c.ORDINAL_POSITION;

PRINT '🎉 Fixed INT columns to NVARCHAR for LN01 table successfully!';
