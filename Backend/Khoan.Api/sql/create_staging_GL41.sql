-- Create GL41_Stage staging table for high-throughput imports
IF OBJECT_ID('dbo.GL41_Stage', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.GL41_Stage (
        NGAY_DL            datetime2      NOT NULL,
        MA_CN              nvarchar(50)   NULL,
        LOAI_TIEN          nvarchar(10)   NULL,
        MA_TK              nvarchar(50)   NULL,
        TEN_TK             nvarchar(200)  NULL,
        LOAI_BT            nvarchar(50)   NULL,
        DN_DAUKY           decimal(18,2)  NULL,
        DC_DAUKY           decimal(18,2)  NULL,
        SBT_NO             decimal(18,2)  NULL,
        ST_GHINO           decimal(18,2)  NULL,
        SBT_CO             decimal(18,2)  NULL,
        ST_GHICO           decimal(18,2)  NULL,
        DN_CUOIKY          decimal(18,2)  NULL,
        DC_CUOIKY          decimal(18,2)  NULL,
        FILE_NAME          nvarchar(500)  NULL,
        CREATED_DATE       datetime2      NULL,
        BATCH_ID           nvarchar(100)  NULL,
        IMPORT_SESSION_ID  nvarchar(100)  NULL
    );
END
GO

-- Optional index to speed up replace-by-date operations
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL41_Stage_NGAY_DL' AND object_id = OBJECT_ID('dbo.GL41_Stage'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_GL41_Stage_NGAY_DL ON dbo.GL41_Stage(NGAY_DL);
END
GO
