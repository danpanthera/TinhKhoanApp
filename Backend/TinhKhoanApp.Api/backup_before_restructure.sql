-- MANUAL MIGRATION: Restructure Column Order
-- Business columns FIRST, System columns MIDDLE, Temporal columns LAST
-- Backup data before restructuring

USE TinhKhoanDB;
GO

-- ===============================================
-- DISABLE TEMPORAL TABLES TEMPORARILY
-- ===============================================
PRINT '🔄 Step 1: Disable Temporal Tables...';

-- Disable temporal for 7 tables (keep GL01 as is - it's not temporal)
ALTER TABLE DP01 SET (SYSTEM_VERSIONING = OFF);
ALTER TABLE DPDA SET (SYSTEM_VERSIONING = OFF);
ALTER TABLE EI01 SET (SYSTEM_VERSIONING = OFF);
ALTER TABLE GL41 SET (SYSTEM_VERSIONING = OFF);
ALTER TABLE LN01 SET (SYSTEM_VERSIONING = OFF);
ALTER TABLE LN03 SET (SYSTEM_VERSIONING = OFF);
ALTER TABLE RR01 SET (SYSTEM_VERSIONING = OFF);

-- ===============================================
-- BACKUP EXISTING DATA
-- ===============================================
PRINT '🔄 Step 2: Backup existing data...';

-- GL01 Backup
SELECT * INTO GL01_BACKUP FROM GL01;
PRINT '✅ GL01 backed up';

-- DP01 Backup
SELECT * INTO DP01_BACKUP FROM DP01;
PRINT '✅ DP01 backed up';

-- LN01 Backup
SELECT * INTO LN01_BACKUP FROM LN01;
PRINT '✅ LN01 backed up';

-- LN03 Backup
SELECT * INTO LN03_BACKUP FROM LN03;
PRINT '✅ LN03 backed up';

-- GL41 Backup
SELECT * INTO GL41_BACKUP FROM GL41;
PRINT '✅ GL41 backed up';

-- DPDA Backup
SELECT * INTO DPDA_BACKUP FROM DPDA;
PRINT '✅ DPDA backed up';

-- EI01 Backup
SELECT * INTO EI01_BACKUP FROM EI01;
PRINT '✅ EI01 backed up';

-- RR01 Backup
SELECT * INTO RR01_BACKUP FROM RR01;
PRINT '✅ RR01 backed up';

PRINT '🎯 All data backed up successfully!';
PRINT '📋 Next: Run individual table restructure scripts';
