-- Create LN03 table without temporal features for testing
CREATE TABLE LN03 (
    Id int IDENTITY(1,1) PRIMARY KEY,
    NGAY_DL datetime2,
    MACHINHANH nvarchar(50),
    TENCHINHANH nvarchar(200),
    MAKH nvarchar(200),
    TENKH nvarchar(500),
    MACBTD nvarchar(50),
    TENCBTD nvarchar(200),
    TAIKHOANHACHTOAN nvarchar(20),
    SOHOPDONG nvarchar(100),
    REFNO nvarchar(100),
    NHOMNO nvarchar(50),
    NGAYPHATSINHXL datetime2,
    MAPGD nvarchar(50),
    LOAINGUONVON nvarchar(100),
    DUNONOIBANG decimal(18,2),
    CONLAINGOAIBANG decimal(18,2),
    SOTIENXLRR decimal(18,2),
    COLUMN_18 nvarchar(max),
    COLUMN_19 nvarchar(max),
    COLUMN_20 nvarchar(max),
    CREATED_DATE datetime2 DEFAULT GETDATE(),
    UPDATED_DATE datetime2 DEFAULT GETDATE()
);

-- Create indexes
CREATE INDEX IX_LN03_NGAY_DL ON LN03 (NGAY_DL);
CREATE INDEX IX_LN03_MACHINHANH ON LN03 (MACHINHANH);
CREATE INDEX IX_LN03_MAKH ON LN03 (MAKH);

PRINT 'LN03 table created successfully without temporal features.';
