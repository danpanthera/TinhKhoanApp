-- Recreate Columnstore Indexes for 8 tables after NGAY_DL conversion
-- Based on the indexes that were dropped during schema conversion

USE TinhKhoanDB;
GO

-- 1. DP01 - No specific columnstore index was mentioned, skip for now

-- 2. DPDA - Recreate NCCI_DPDA_Analytics
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'NCCI_DPDA_Analytics' AND object_id = OBJECT_ID('DPDA'))
BEGIN
    PRINT '🔄 Creating NCCI_DPDA_Analytics on DPDA table...'
    CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_DPDA_Analytics
    ON DPDA (NGAY_DL, MA_CHI_NHANH, LOAI_TIEN, SO_DU_NO, SO_DU_CO, SO_DU_CUOI_KY)
    WITH (DATA_COMPRESSION = COLUMNSTORE);
    PRINT '✅ NCCI_DPDA_Analytics created successfully'
END
ELSE
    PRINT '⚠️ NCCI_DPDA_Analytics already exists on DPDA'
GO

-- 3. EI01 - No specific columnstore index was mentioned, skip for now

-- 4. GL01 - Recreate CCI_GL01_Partitioned
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'CCI_GL01_Partitioned' AND object_id = OBJECT_ID('GL01'))
BEGIN
    PRINT '🔄 Creating CCI_GL01_Partitioned on GL01 table...'
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_GL01_Partitioned
    ON GL01
    WITH (DATA_COMPRESSION = COLUMNSTORE);
    PRINT '✅ CCI_GL01_Partitioned created successfully'
END
ELSE
    PRINT '⚠️ CCI_GL01_Partitioned already exists on GL01'
GO

-- 5. GL41 - No specific columnstore index was mentioned, skip for now

-- 6. LN01 - Recreate NCCI_LN01_Analytics
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'NCCI_LN01_Analytics' AND object_id = OBJECT_ID('LN01'))
BEGIN
    PRINT '🔄 Creating NCCI_LN01_Analytics on LN01 table...'
    CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_LN01_Analytics
    ON LN01 (NGAY_DL, MA_CHI_NHANH, LOAI_TIEN, DU_NO_DAU_KY, PS_TANG, PS_GIAM, DU_NO_CUOI_KY)
    WITH (DATA_COMPRESSION = COLUMNSTORE);
    PRINT '✅ NCCI_LN01_Analytics created successfully'
END
ELSE
    PRINT '⚠️ NCCI_LN01_Analytics already exists on LN01'
GO

-- 7. LN03 - Recreate NCCI_LN03_Analytics
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'NCCI_LN03_Analytics' AND object_id = OBJECT_ID('LN03'))
BEGIN
    PRINT '🔄 Creating NCCI_LN03_Analytics on LN03 table...'
    CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_LN03_Analytics
    ON LN03 (NGAY_DL, MA_CHI_NHANH, LOAI_TIEN, DU_NO_DAU_KY, PS_TANG, PS_GIAM, DU_NO_CUOI_KY)
    WITH (DATA_COMPRESSION = COLUMNSTORE);
    PRINT '✅ NCCI_LN03_Analytics created successfully'
END
ELSE
    PRINT '⚠️ NCCI_LN03_Analytics already exists on LN03'
GO

-- 8. RR01 - Recreate NCCI_RR01_Analytics
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'NCCI_RR01_Analytics' AND object_id = OBJECT_ID('RR01'))
BEGIN
    PRINT '🔄 Creating NCCI_RR01_Analytics on RR01 table...'
    CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_RR01_Analytics
    ON RR01 (NGAY_DL, MA_CHI_NHANH, LOAI_TIEN, SO_DU_DAU_KY, PS_TANG, PS_GIAM, SO_DU_CUOI_KY)
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
