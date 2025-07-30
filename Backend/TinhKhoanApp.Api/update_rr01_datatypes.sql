-- SQL script to modify RR01 table structure while preserving data
-- Alter numeric columns from nvarchar to decimal(18,2)
BEGIN TRANSACTION;

-- Backup original data (in case we need to rollback)
SELECT * INTO RR01_Backup_BeforeTypeFix FROM RR01;

PRINT 'Backup created: RR01_Backup_BeforeTypeFix';

-- Turn off system versioning temporarily
ALTER TABLE RR01 SET (SYSTEM_VERSIONING = OFF);

PRINT 'System versioning turned off temporarily';

-- Alter numeric columns from nvarchar to decimal(18,2)
ALTER TABLE RR01 ALTER COLUMN SO_LDS decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DUNO_GOC_BAN_DAU decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DUNO_LAI_TICHLUY_BD decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DOC_DAUKY_DA_THU_HT decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DUNO_GOC_HIENTAI decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DUNO_LAI_HIENTAI decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DUNO_NGAN_HAN decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DUNO_TRUNG_HAN decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DUNO_DAI_HAN decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN THU_GOC decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN THU_LAI decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN BDS decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DS decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN TSK decimal(18,2) NULL;

PRINT 'All numeric columns converted to decimal(18,2)';

-- Alter date columns from nvarchar to datetime2
ALTER TABLE RR01 ALTER COLUMN NGAY_GIAI_NGAN datetime2 NULL;
ALTER TABLE RR01 ALTER COLUMN NGAY_DEN_HAN datetime2 NULL;
ALTER TABLE RR01 ALTER COLUMN NGAY_XLRR datetime2 NULL;

PRINT 'All date columns converted to datetime2';

-- Turn system versioning back on
ALTER TABLE RR01 SET (
    SYSTEM_VERSIONING = ON (
        HISTORY_TABLE = dbo.RR01_History
    )
);

PRINT 'System versioning turned back on';

PRINT 'RR01 table structure updated successfully';

COMMIT;
