-- Create GL02_Stage staging table for high-throughput imports
IF OBJECT_ID('dbo.GL02_Stage', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.GL02_Stage (
        NGAY_DL       datetime2     NOT NULL,
        TRBRCD        nvarchar(200) NULL,
        USERID        nvarchar(200) NULL,
        JOURSEQ       nvarchar(200) NULL,
        DYTRSEQ       nvarchar(200) NULL,
        LOCAC         nvarchar(200) NULL,
        CCY           nvarchar(200) NULL,
        BUSCD         nvarchar(200) NULL,
        UNIT          nvarchar(200) NULL,
        TRCD          nvarchar(200) NULL,
        CUSTOMER      nvarchar(200) NULL,
        TRTP          nvarchar(200) NULL,
        REFERENCE     nvarchar(200) NULL,
        REMARK        nvarchar(1000) NULL,
        DRAMOUNT      decimal(18,2) NULL,
        CRAMOUNT      decimal(18,2) NULL,
        CRTDTM        datetime2     NULL,
        CREATED_DATE  datetime2     NOT NULL,
        UPDATED_DATE  datetime2     NULL,
        FILE_NAME     nvarchar(500) NOT NULL
    );
END
GO

-- Optional index to speed up replace-by-date operations
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL02_Stage_NGAY_DL' AND object_id = OBJECT_ID('dbo.GL02_Stage'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_GL02_Stage_NGAY_DL ON dbo.GL02_Stage(NGAY_DL);
END
GO
