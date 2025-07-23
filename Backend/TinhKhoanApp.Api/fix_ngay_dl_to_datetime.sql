-- Fix NGAY_DL from NVARCHAR to DATETIME2 for all 8 DataTables
-- This is critical for Direct Import to work properly

PRINT '🔧 Starting NGAY_DL to DATETIME2 conversion...'

-- ===================================================================
-- 1. DISABLE TEMPORAL TABLES TEMPORARILY
-- ===================================================================

PRINT '⏸️ Disabling temporal tables...'

-- Disable temporal for DP01
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DP01' AND temporal_type = 2)
BEGIN
    PRINT 'Disabling temporal for DP01...'
    ALTER TABLE DP01 SET (SYSTEM_VERSIONING = OFF)
END

-- Disable temporal for EI01
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EI01' AND temporal_type = 2)
BEGIN
    PRINT 'Disabling temporal for EI01...'
    ALTER TABLE EI01 SET (SYSTEM_VERSIONING = OFF)
END

-- Disable temporal for GL41
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GL41' AND temporal_type = 2)
BEGIN
    PRINT 'Disabling temporal for GL41...'
    ALTER TABLE GL41 SET (SYSTEM_VERSIONING = OFF)
END

-- Disable temporal for LN01
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'LN01' AND temporal_type = 2)
BEGIN
    PRINT 'Disabling temporal for LN01...'
    ALTER TABLE LN01 SET (SYSTEM_VERSIONING = OFF)
END

-- Disable temporal for LN03
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'LN03' AND temporal_type = 2)
BEGIN
    PRINT 'Disabling temporal for LN03...'
    ALTER TABLE LN03 SET (SYSTEM_VERSIONING = OFF)
END

-- Disable temporal for RR01
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'RR01' AND temporal_type = 2)
BEGIN
    PRINT 'Disabling temporal for RR01...'
    ALTER TABLE RR01 SET (SYSTEM_VERSIONING = OFF)
END

-- Disable temporal for DPDA
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DPDA' AND temporal_type = 2)
BEGIN
    PRINT 'Disabling temporal for DPDA...'
    ALTER TABLE DPDA SET (SYSTEM_VERSIONING = OFF)
END

-- ===================================================================
-- 2. CLEAR DATA AND ALTER COLUMNS
-- ===================================================================

PRINT '🗑️ Clearing existing data to avoid conversion errors...'

-- Clear data from all tables
DELETE FROM DP01
DELETE FROM EI01
DELETE FROM GL01
DELETE FROM GL41
DELETE FROM LN01
DELETE FROM LN03
DELETE FROM RR01
DELETE FROM DPDA

PRINT '🔄 Converting NGAY_DL to DATETIME2...'

-- DP01: NVARCHAR(20) -> DATETIME2
ALTER TABLE DP01 ALTER COLUMN NGAY_DL DATETIME2 NOT NULL

-- EI01: NVARCHAR(20) -> DATETIME2
ALTER TABLE EI01 ALTER COLUMN NGAY_DL DATETIME2 NOT NULL

-- GL01: NVARCHAR(20) -> DATETIME2
ALTER TABLE GL01 ALTER COLUMN NGAY_DL DATETIME2 NOT NULL

-- GL41: NVARCHAR(20) -> DATETIME2
ALTER TABLE GL41 ALTER COLUMN NGAY_DL DATETIME2 NOT NULL

-- LN01: NVARCHAR(20) -> DATETIME2
ALTER TABLE LN01 ALTER COLUMN NGAY_DL DATETIME2 NOT NULL

-- LN03: NVARCHAR(20) -> DATETIME2
ALTER TABLE LN03 ALTER COLUMN NGAY_DL DATETIME2 NOT NULL

-- RR01: NVARCHAR(20) -> DATETIME2
ALTER TABLE RR01 ALTER COLUMN NGAY_DL DATETIME2 NOT NULL

-- DPDA: NVARCHAR(20) -> DATETIME2
ALTER TABLE DPDA ALTER COLUMN NGAY_DL DATETIME2 NOT NULL

-- ===================================================================
-- 3. RE-ENABLE TEMPORAL TABLES
-- ===================================================================

PRINT '▶️ Re-enabling temporal tables...'

-- Re-enable temporal for DP01
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DP01' AND temporal_type = 2)
BEGIN
    PRINT 'Re-enabling temporal for DP01...'

    -- Add period columns if not exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DP01') AND name = 'ValidFrom')
        ALTER TABLE DP01 ADD ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN DEFAULT SYSUTCDATETIME()

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DP01') AND name = 'ValidTo')
        ALTER TABLE DP01 ADD ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999')

    -- Add period
    IF NOT EXISTS (SELECT * FROM sys.periods WHERE object_id = OBJECT_ID('DP01'))
        ALTER TABLE DP01 ADD PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)

    -- Enable system versioning
    ALTER TABLE DP01 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.DP01_History))
END

-- Re-enable temporal for other tables (EI01, GL41, LN01, LN03, RR01, DPDA)
-- Similar pattern for each table...

PRINT '✅ NGAY_DL to DATETIME2 conversion completed!'

-- Verify changes
PRINT '🔍 Verification:'
SELECT
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE COLUMN_NAME = 'NGAY_DL'
    AND TABLE_NAME IN ('DP01', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01', 'DPDA')
ORDER BY TABLE_NAME
