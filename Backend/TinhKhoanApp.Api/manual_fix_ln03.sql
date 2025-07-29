-- Manual SQL to remove R, S, T columns from LN03 table
-- This bypasses EF Core migrations

-- First, turn off system versioning for LN03
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LN03' AND temporal_type = 2)
BEGIN
    ALTER TABLE LN03 SET (SYSTEM_VERSIONING = OFF);
    PRINT 'System versioning disabled for LN03';
END

-- Drop the R, S, T columns if they exist
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('LN03') AND name = 'R')
BEGIN
    ALTER TABLE LN03 DROP COLUMN R;
    PRINT 'Dropped column R from LN03';
END

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('LN03') AND name = 'S')
BEGIN
    ALTER TABLE LN03 DROP COLUMN S;
    PRINT 'Dropped column S from LN03';
END

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('LN03') AND name = 'T')
BEGIN
    ALTER TABLE LN03 DROP COLUMN T;
    PRINT 'Dropped column T from LN03';
END

-- Drop the columns from history table if it exists
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LN03_History')
BEGIN
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('LN03_History') AND name = 'R')
    BEGIN
        ALTER TABLE LN03_History DROP COLUMN R;
        PRINT 'Dropped column R from LN03_History';
    END

    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('LN03_History') AND name = 'S')
    BEGIN
        ALTER TABLE LN03_History DROP COLUMN S;
        PRINT 'Dropped column S from LN03_History';
    END

    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('LN03_History') AND name = 'T')
    BEGIN
        ALTER TABLE LN03_History DROP COLUMN T;
        PRINT 'Dropped column T from LN03_History';
    END
END

-- Re-enable system versioning if both tables exist
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LN03') AND EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LN03_History')
BEGIN
    -- Add temporal columns if they don't exist
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('LN03') AND name = 'SysStartTime')
    BEGIN
        ALTER TABLE LN03 ADD SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT '1900-01-01';
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('LN03') AND name = 'SysEndTime')
    BEGIN
        ALTER TABLE LN03 ADD SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
    END

    -- Add period if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM sys.periods WHERE object_id = OBJECT_ID('LN03'))
    BEGIN
        ALTER TABLE LN03 ADD PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
    END

    -- Re-enable system versioning
    ALTER TABLE LN03 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.LN03_History));
    PRINT 'System versioning re-enabled for LN03';
END

-- Verify final structure
PRINT 'Final LN03 table structure:';
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'LN03'
ORDER BY ORDINAL_POSITION;
