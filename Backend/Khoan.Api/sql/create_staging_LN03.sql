-- Create LN03_Stage staging table for high-throughput imports
IF OBJECT_ID('dbo.LN03_Stage', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.LN03_Stage (
        NGAY_DL            datetime2      NOT NULL,
        MACHINHANH         nvarchar(50)   NULL,
        TENCHINHANH        nvarchar(200)  NULL,
        MAKH               nvarchar(100)  NULL,
        TENKH              nvarchar(200)  NULL,
        SOHOPDONG          nvarchar(100)  NULL,
        SOTIENXLRR         decimal(18,2)  NULL,
        NGAYPHATSINHXL     datetime2      NULL,
        THUNOSAUXL         decimal(18,2)  NULL,
        CONLAINGOAIBANG    decimal(18,2)  NULL,
        DUNONOIBANG        decimal(18,2)  NULL,
        NHOMNO             nvarchar(50)   NULL,
        MACBTD             nvarchar(50)   NULL,
        TENCBTD            nvarchar(200)  NULL,
        MAPGD              nvarchar(50)   NULL,
        TAIKHOANHACHTOAN   nvarchar(50)   NULL,
        REFNO              nvarchar(100)  NULL,
        LOAINGUONVON       nvarchar(50)   NULL,
        Column18           nvarchar(200)  NULL,
        Column19           nvarchar(200)  NULL,
        Column20           decimal(18,2)  NULL,
        CREATED_DATE       datetime2      NULL,
        FILE_ORIGIN        nvarchar(500)  NULL
    );
END
GO

-- Optional index to speed up replace-by-date operations
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LN03_Stage_NGAY_DL' AND object_id = OBJECT_ID('dbo.LN03_Stage'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_LN03_Stage_NGAY_DL ON dbo.LN03_Stage(NGAY_DL);
END
GO
