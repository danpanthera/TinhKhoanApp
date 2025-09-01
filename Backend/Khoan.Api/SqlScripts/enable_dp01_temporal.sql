-- Enable temporal table for DP01
-- This script adds SysStartTime and SysEndTime columns to DP01 table and enables SYSTEM_VERSIONING

USE TinhKhoanDB;
GO

-- Step 1: Add temporal columns to the main table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DP01' AND COLUMN_NAME = 'SysStartTime')
BEGIN
    ALTER TABLE DP01
    ADD 
        SysStartTime datetime2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime datetime2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(datetime2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END

-- Step 2: Create history table if it doesn't exist
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DP01_History')
BEGIN
    -- Create history table with same structure as main table (except system columns)
    SELECT TOP 0 * 
    INTO DP01_History 
    FROM DP01;
    
    -- Add temporal columns to history table
    ALTER TABLE DP01_History
    ADD 
        SysStartTime datetime2 NOT NULL,
        SysEndTime datetime2 NOT NULL;
        
    -- Add clustered index on temporal columns for history table
    CREATE CLUSTERED INDEX IX_DP01_History_Period_Columns
    ON DP01_History (SysEndTime, SysStartTime, Id);
END

-- Step 3: Enable system versioning
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DP01' AND temporal_type = 2)
BEGIN
    ALTER TABLE DP01
    SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.DP01_History, DATA_CONSISTENCY_CHECK = ON));
END

PRINT '✅ DP01 temporal table enabled successfully!';
GO
