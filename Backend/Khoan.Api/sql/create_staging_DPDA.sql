-- Create DPDA_Stage staging table for high-throughput imports
IF OBJECT_ID('dbo.DPDA_Stage', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DPDA_Stage (
        NGAY_DL         datetime2      NOT NULL,
        MA_CHI_NHANH    nvarchar(100)  NULL,
        MA_KHACH_HANG   nvarchar(100)  NULL,
        TEN_KHACH_HANG  nvarchar(200)  NULL,
        SO_TAI_KHOAN    nvarchar(100)  NULL,
        LOAI_THE        nvarchar(100)  NULL,
        SO_THE          nvarchar(100)  NULL,
        NGAY_NOP_DON    datetime2      NULL,
        NGAY_PHAT_HANH  datetime2      NULL,
        USER_PHAT_HANH  nvarchar(100)  NULL,
        TRANG_THAI      nvarchar(100)  NULL,
        PHAN_LOAI       nvarchar(100)  NULL,
        GIAO_THE        nvarchar(100)  NULL,
        LOAI_PHAT_HANH  nvarchar(100)  NULL,
        CREATED_DATE    datetime2      NOT NULL,
        UPDATED_DATE    datetime2      NULL,
        FILE_NAME       nvarchar(500)  NOT NULL
    );
END
GO

-- Optional index to speed up replace-by-date operations
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DPDA_Stage_NGAY_DL' AND object_id = OBJECT_ID('dbo.DPDA_Stage'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_DPDA_Stage_NGAY_DL ON dbo.DPDA_Stage(NGAY_DL);
END
GO
