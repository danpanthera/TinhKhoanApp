-- Create EI01_Stage staging table for high-throughput imports
IF OBJECT_ID('dbo.EI01_Stage', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.EI01_Stage (
        NGAY_DL         datetime2     NOT NULL,
        MA_CN           nvarchar(200) NULL,
        MA_KH           nvarchar(200) NULL,
        TEN_KH          nvarchar(200) NULL,
        LOAI_KH         nvarchar(200) NULL,
        SDT_EMB         nvarchar(200) NULL,
        TRANG_THAI_EMB  nvarchar(200) NULL,
        NGAY_DK_EMB     datetime2     NULL,
        SDT_OTT         nvarchar(200) NULL,
        TRANG_THAI_OTT  nvarchar(200) NULL,
        NGAY_DK_OTT     datetime2     NULL,
        SDT_SMS         nvarchar(200) NULL,
        TRANG_THAI_SMS  nvarchar(200) NULL,
        NGAY_DK_SMS     datetime2     NULL,
        SDT_SAV         nvarchar(200) NULL,
        TRANG_THAI_SAV  nvarchar(200) NULL,
        NGAY_DK_SAV     datetime2     NULL,
        SDT_LN          nvarchar(200) NULL,
        TRANG_THAI_LN   nvarchar(200) NULL,
        NGAY_DK_LN      datetime2     NULL,
        USER_EMB        nvarchar(200) NULL,
        USER_OTT        nvarchar(200) NULL,
        USER_SMS        nvarchar(200) NULL,
        USER_SAV        nvarchar(200) NULL,
        USER_LN         nvarchar(200) NULL,
        CREATED_DATE    datetime2     NOT NULL,
        UPDATED_DATE    datetime2     NULL,
        FILE_NAME       nvarchar(500) NOT NULL
    );
END
GO

-- Optional index to speed up replace-by-date operations
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EI01_Stage_NGAY_DL' AND object_id = OBJECT_ID('dbo.EI01_Stage'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_EI01_Stage_NGAY_DL ON dbo.EI01_Stage(NGAY_DL);
END
GO
