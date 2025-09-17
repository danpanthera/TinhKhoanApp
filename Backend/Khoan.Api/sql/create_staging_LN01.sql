-- Create LN01_Stage staging table for high-throughput imports
IF OBJECT_ID('dbo.LN01_Stage', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.LN01_Stage (
        NGAY_DL             datetime2      NOT NULL,
        BRCD                nvarchar(50)   NULL,
        CUSTSEQ             nvarchar(100)  NULL,
        CUSTNM              nvarchar(200)  NULL,
        TAI_KHOAN           nvarchar(100)  NULL,
        CCY                 nvarchar(10)   NULL,
        DU_NO               decimal(18,2)  NULL,
        DSBSSEQ             nvarchar(100)  NULL,
        TRANSACTION_DATE    datetime2      NULL,
        DISBURSEMENT_AMOUNT decimal(18,2)  NULL,
        CREATED_DATE        datetime2      NULL,
        UPDATED_DATE        datetime2      NULL,
        FILE_NAME           nvarchar(500)  NULL
    );
END
GO

-- Optional index to speed up replace-by-date operations
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LN01_Stage_NGAY_DL' AND object_id = OBJECT_ID('dbo.LN01_Stage'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_LN01_Stage_NGAY_DL ON dbo.LN01_Stage(NGAY_DL);
END
GO
