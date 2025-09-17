-- Create GL01_Stage staging table for high-throughput imports
IF OBJECT_ID('dbo.GL01_Stage', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.GL01_Stage (
        NGAY_DL       datetime2      NOT NULL,
        STS           nvarchar(20)   NULL,
        NGAY_GD       datetime2      NULL,
        NGUOI_TAO     nvarchar(100)  NULL,
        DYSEQ         nvarchar(50)   NULL,
        TR_TYPE       nvarchar(50)   NULL,
        DT_SEQ        nvarchar(50)   NULL,
        TAI_KHOAN     nvarchar(100)  NULL,
        TEN_TK        nvarchar(200)  NULL,
        SO_TIEN_GD    decimal(18,2)  NULL,
        POST_BR       nvarchar(50)   NULL,
        LOAI_TIEN     nvarchar(10)   NULL,
        DR_CR         nvarchar(10)   NULL,
        MA_KH         nvarchar(100)  NULL,
        TEN_KH        nvarchar(200)  NULL,
        CCA_USRID     nvarchar(50)   NULL,
        TR_EX_RT      nvarchar(50)   NULL,
        REMARK        nvarchar(1000) NULL,
        BUS_CODE      nvarchar(50)   NULL,
        UNIT_BUS_CODE nvarchar(50)   NULL,
        TR_CODE       nvarchar(50)   NULL,
        TR_NAME       nvarchar(200)  NULL,
        REFERENCE     nvarchar(200)  NULL,
        VALUE_DATE    datetime2      NULL,
        DEPT_CODE     nvarchar(50)   NULL,
        TR_TIME       nvarchar(50)   NULL,
        COMFIRM       nvarchar(10)   NULL,
        TRDT_TIME     nvarchar(50)   NULL,
        CREATED_DATE  datetime2      NULL,
        UPDATED_DATE  datetime2      NULL,
        FILE_NAME     nvarchar(500)  NULL
    );
END
GO

-- Optional index to speed up replace-by-date operations
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL01_Stage_NGAY_DL' AND object_id = OBJECT_ID('dbo.GL01_Stage'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_GL01_Stage_NGAY_DL ON dbo.GL01_Stage(NGAY_DL);
END
GO
