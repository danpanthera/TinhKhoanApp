-- Create RR01_Stage staging table for high-throughput imports
IF OBJECT_ID('dbo.RR01_Stage', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.RR01_Stage (
        NGAY_DL                 datetime2      NOT NULL,
        CN_LOAI_I               nvarchar(50)   NULL,
        BRCD                    nvarchar(50)   NULL,
        MA_KH                   nvarchar(100)  NULL,
        TEN_KH                  nvarchar(200)  NULL,
        SO_LDS                  nvarchar(100)  NULL,
        CCY                     nvarchar(10)   NULL,
        SO_LAV                  nvarchar(100)  NULL,
        LOAI_KH                 nvarchar(50)   NULL,
        NGAY_GIAI_NGAN          datetime2      NULL,
        NGAY_DEN_HAN            datetime2      NULL,
        VAMC_FLG                nvarchar(10)   NULL,
        NGAY_XLRR               datetime2      NULL,
        DUNO_GOC_BAN_DAU        decimal(18,2)  NULL,
        DUNO_LAI_TICHLUY_BD     decimal(18,2)  NULL,
        DOC_DAUKY_DA_THU_HT     decimal(18,2)  NULL,
        DUNO_GOC_HIENTAI        decimal(18,2)  NULL,
        DUNO_LAI_HIENTAI        decimal(18,2)  NULL,
        DUNO_NGAN_HAN           decimal(18,2)  NULL,
        DUNO_TRUNG_HAN          decimal(18,2)  NULL,
        DUNO_DAI_HAN            decimal(18,2)  NULL,
        THU_GOC                 decimal(18,2)  NULL,
        THU_LAI                 decimal(18,2)  NULL,
        BDS                     decimal(18,2)  NULL,
        DS                      decimal(18,2)  NULL,
        TSK                     decimal(18,2)  NULL,
        FILE_NAME               nvarchar(500)  NULL,
        CREATED_DATE            datetime2      NULL,
        UPDATED_DATE            datetime2      NULL
    );
END
GO

-- Optional index to speed up replace-by-date operations
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RR01_Stage_NGAY_DL' AND object_id = OBJECT_ID('dbo.RR01_Stage'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_RR01_Stage_NGAY_DL ON dbo.RR01_Stage(NGAY_DL);
END
GO
