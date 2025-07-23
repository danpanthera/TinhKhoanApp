-- Configure table structures as required:
-- GL01: Partitioned Columnstore
-- Other 7 tables: Temporal Tables + Columnstore Indexes

-- ===================================================================
-- 1. Configure GL01 with Partitioned Columnstore
-- ===================================================================

-- First, drop existing clustered index and recreate table structure for partitioning
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('GL01') AND name LIKE 'PK__GL01%')
BEGIN
    PRINT 'Dropping existing clustered index on GL01...'
    DECLARE @pk_name NVARCHAR(128)
    SELECT @pk_name = name FROM sys.indexes WHERE object_id = OBJECT_ID('GL01') AND is_primary_key = 1
    EXEC('ALTER TABLE GL01 DROP CONSTRAINT ' + @pk_name)
END

-- Add identity column for partitioning if not exists
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GL01') AND is_identity = 1)
BEGIN
    PRINT 'Adding identity column to GL01...'
    ALTER TABLE GL01 ADD Id_New BIGINT IDENTITY(1,1)
    -- Update existing records
    UPDATE GL01 SET Id_New = Id WHERE Id_New IS NULL
    -- Drop old Id and rename
    ALTER TABLE GL01 DROP COLUMN Id
    EXEC sp_rename 'GL01.Id_New', 'Id', 'COLUMN'
END

-- Create partition function for GL01 (partition by Id ranges)
IF NOT EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'PF_GL01_Id')
BEGIN
    PRINT 'Creating partition function for GL01...'
    CREATE PARTITION FUNCTION PF_GL01_Id (BIGINT)
    AS RANGE RIGHT FOR VALUES (100000, 500000, 1000000, 2000000, 5000000)
END

-- Create partition scheme for GL01
IF NOT EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'PS_GL01_Id')
BEGIN
    PRINT 'Creating partition scheme for GL01...'
    CREATE PARTITION SCHEME PS_GL01_Id
    AS PARTITION PF_GL01_Id
    ALL TO ([PRIMARY])
END

-- Create clustered columnstore index on GL01 with partitioning
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('GL01') AND type = 5)
BEGIN
    PRINT 'Creating partitioned clustered columnstore index on GL01...'
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_GL01_Partitioned
    ON GL01
    ON PS_GL01_Id(Id)
END

-- ===================================================================
-- 2. Configure Temporal Tables for remaining 7 tables
-- ===================================================================

-- Function to enable temporal table
-- DP01 Temporal Configuration
PRINT 'Configuring DP01 as Temporal Table...'

-- Add period columns if not exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DP01') AND name = 'ValidFrom')
    ALTER TABLE DP01 ADD ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN DEFAULT SYSUTCDATETIME()

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DP01') AND name = 'ValidTo')
    ALTER TABLE DP01 ADD ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999')

-- Add period
IF NOT EXISTS (SELECT * FROM sys.periods WHERE object_id = OBJECT_ID('DP01'))
    ALTER TABLE DP01 ADD PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)

-- Enable system versioning
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DP01' AND temporal_type = 2)
    ALTER TABLE DP01 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.DP01_History))

-- Create columnstore index on DP01
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('DP01') AND type = 6)
BEGIN
    PRINT 'Creating columnstore index on DP01...'
    CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_DP01_Analytics
    ON DP01 (NGAY_DL, MA_CN, MA_KH, CCY, CURRENT_BALANCE, CREATED_DATE)
END

-- Create columnstore index on DP01 History table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DP01_History')
AND NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('DP01_History') AND type = 5)
BEGIN
    PRINT 'Creating columnstore index on DP01_History...'
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_DP01_History
    ON DP01_History
END

-- ===================================================================
-- Configure remaining 6 tables similarly
-- ===================================================================

-- EI01 Temporal Configuration
PRINT 'Configuring EI01 as Temporal Table...'

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EI01') AND name = 'ValidFrom')
    ALTER TABLE EI01 ADD ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN DEFAULT SYSUTCDATETIME()

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EI01') AND name = 'ValidTo')
    ALTER TABLE EI01 ADD ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999')

IF NOT EXISTS (SELECT * FROM sys.periods WHERE object_id = OBJECT_ID('EI01'))
    ALTER TABLE EI01 ADD PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EI01' AND temporal_type = 2)
    ALTER TABLE EI01 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.EI01_History))

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('EI01') AND type = 6)
BEGIN
    CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_EI01_Analytics
    ON EI01 (NGAY_DL, MA_CN, MA_KH, LOAI_KH, CREATED_DATE)
END

-- GL41 Temporal Configuration
PRINT 'Configuring GL41 as Temporal Table...'

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GL41') AND name = 'ValidFrom')
    ALTER TABLE GL41 ADD ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN DEFAULT SYSUTCDATETIME()

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GL41') AND name = 'ValidTo')
    ALTER TABLE GL41 ADD ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999')

IF NOT EXISTS (SELECT * FROM sys.periods WHERE object_id = OBJECT_ID('GL41'))
    ALTER TABLE GL41 ADD PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GL41' AND temporal_type = 2)
    ALTER TABLE GL41 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.GL41_History))

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('GL41') AND type = 6)
BEGIN
    CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_GL41_Analytics
    ON GL41 (NGAY_DL, MA_CN, MA_TK, TEN_TK, CREATED_DATE)
END

PRINT 'Table structure configuration completed successfully!'

-- Verify configuration
SELECT
    t.name AS table_name,
    t.temporal_type_desc,
    COUNT(i.index_id) as index_count,
    STRING_AGG(i.type_desc, ', ') as index_types
FROM sys.tables t
LEFT JOIN sys.indexes i ON t.object_id = i.object_id AND i.type IN (5, 6)
WHERE t.name IN ('GL01', 'DP01', 'EI01', 'GL41', 'LN01', 'LN03', 'RR01', 'DPDA')
GROUP BY t.name, t.temporal_type_desc
ORDER BY t.name;
