-- Recreate Columnstore Indexes for 8 tables after NGAY_DL conversion
-- Simple version with basic columns only

USE TinhKhoanDB;
GO

-- 1. DPDA - Simple columnstore with key columns
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'NCCI_DPDA_Analytics' AND object_id = OBJECT_ID('DPDA'))
BEGIN
    PRINT '🔄 Creating NCCI_DPDA_Analytics on DPDA table...'
    CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_DPDA_Analytics
    ON DPDA (NGAY_DL, MA_CHI_NHANH, MA_KHACH_HANG, SO_TAI_KHOAN)
    WITH (DATA_COMPRESSION = COLUMNSTORE);
    PRINT '✅ NCCI_DPDA_Analytics created successfully'
END
ELSE
    PRINT '⚠️ NCCI_DPDA_Analytics already exists on DPDA'
GO

-- 2. GL01 - Already recreated, skip
PRINT '⚠️ GL01 CCI_GL01_Partitioned already exists'
GO

-- 3. LN01 - Simple columnstore with key columns
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'NCCI_LN01_Analytics' AND object_id = OBJECT_ID('LN01'))
BEGIN
    PRINT '🔄 Creating NCCI_LN01_Analytics on LN01 table...'
    CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_LN01_Analytics
    ON LN01 (NGAY_DL, MA_CHI_NHANH, DU_NO, SO_TIEN_GIAI_NGAN_1, SO_TIEN_GIAI_NGAN_2)
    WITH (DATA_COMPRESSION = COLUMNSTORE);
    PRINT '✅ NCCI_LN01_Analytics created successfully'
END
ELSE
    PRINT '⚠️ NCCI_LN01_Analytics already exists on LN01'
GO

-- 4. LN03 - Simple columnstore with key columns
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'NCCI_LN03_Analytics' AND object_id = OBJECT_ID('LN03'))
BEGIN
    PRINT '🔄 Creating NCCI_LN03_Analytics on LN03 table...'
    CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_LN03_Analytics
    ON LN03 (NGAY_DL, MA_CHI_NHANH, DUNONOIBANG, SOTIENXLRR)
    WITH (DATA_COMPRESSION = COLUMNSTORE);
    PRINT '✅ NCCI_LN03_Analytics created successfully'
END
ELSE
    PRINT '⚠️ NCCI_LN03_Analytics already exists on LN03'
GO

-- 5. RR01 - Simple columnstore with key columns
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'NCCI_RR01_Analytics' AND object_id = OBJECT_ID('RR01'))
BEGIN
    PRINT '🔄 Creating NCCI_RR01_Analytics on RR01 table...'
    CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_RR01_Analytics
    ON RR01 (NGAY_DL, MA_CHI_NHANH, DUNO_GOC_HIENTAI, DUNO_LAI_HIENTAI, DUNO_NGAN_HAN, DUNO_TRUNG_HAN, DUNO_DAI_HAN)
    WITH (DATA_COMPRESSION = COLUMNSTORE);
    PRINT '✅ NCCI_RR01_Analytics created successfully'
END
ELSE
    PRINT '⚠️ NCCI_RR01_Analytics already exists on RR01'
GO

-- Check all columnstore indexes
PRINT '📊 Current Columnstore Indexes:'
SELECT
    t.name AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType,
    CASE WHEN i.data_space_id IS NOT NULL THEN 'Active' ELSE 'Inactive' END AS Status
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE i.type IN (5, 6) -- Clustered and Nonclustered Columnstore
    AND t.name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')
ORDER BY t.name, i.name;

PRINT '✅ Columnstore indexes recreation completed!'
