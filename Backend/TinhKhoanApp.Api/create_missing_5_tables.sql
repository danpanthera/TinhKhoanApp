-- Tạo 5 bảng dữ liệu còn thiếu với cấu trúc temporal table đúng cách
-- Theo chuẩn: NGAY_DL + Business columns + System columns + Temporal columns

-- 1. Tạo bảng DPDA (13 business columns)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DPDA')
BEGIN
    CREATE TABLE [DPDA] (
        [NGAY_DL] datetime2(7) NOT NULL,
        [MA_CN] nvarchar(200) NOT NULL DEFAULT(''),
        [TAI_KHOAN_HACH_TOAN] nvarchar(200) NOT NULL DEFAULT(''),
        [MA_KH] nvarchar(200) NOT NULL DEFAULT(''),
        [TEN_KH] nvarchar(200) NOT NULL DEFAULT(''),
        [GIOITINH] nvarchar(200) NOT NULL DEFAULT(''),
        [NGAYSINH] nvarchar(200) NOT NULL DEFAULT(''),
        [SO_DIENTHOAI] nvarchar(200) NOT NULL DEFAULT(''),
        [CANCUOC] nvarchar(200) NOT NULL DEFAULT(''),
        [NGAYCAP] nvarchar(200) NOT NULL DEFAULT(''),
        [NOICAP] nvarchar(200) NOT NULL DEFAULT(''),
        [DIACHI] nvarchar(200) NOT NULL DEFAULT(''),
        [CCY] nvarchar(200) NOT NULL DEFAULT(''),
        [TIENGUI] decimal(18,2) NOT NULL DEFAULT(0),
        [Id] bigint IDENTITY(1,1) NOT NULL,
        [CREATED_DATE] datetime2(7) GENERATED ALWAYS AS ROW START NOT NULL,
        [UPDATED_DATE] datetime2(7) GENERATED ALWAYS AS ROW END NOT NULL,
        [FILE_NAME] nvarchar(500) NOT NULL DEFAULT(''),
        PERIOD FOR SYSTEM_TIME ([CREATED_DATE], [UPDATED_DATE]),
        CONSTRAINT [PK_DPDA] PRIMARY KEY ([Id])
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[DPDA_History]));
    PRINT 'Tạo bảng DPDA thành công';
END;

-- 2. Tạo bảng EI01 (24 business columns)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'EI01')
BEGIN
    CREATE TABLE [EI01] (
        [NGAY_DL] datetime2(7) NOT NULL,
        [MA_CN] nvarchar(200) NOT NULL DEFAULT(''),
        [TAI_KHOAN_HACH_TOAN] nvarchar(200) NOT NULL DEFAULT(''),
        [MA_KH] nvarchar(200) NOT NULL DEFAULT(''),
        [TEN_KH] nvarchar(200) NOT NULL DEFAULT(''),
        [SO_TAI_KHOAN] nvarchar(200) NOT NULL DEFAULT(''),
        [CCY] nvarchar(200) NOT NULL DEFAULT(''),
        [SODU_CO] decimal(18,2) NOT NULL DEFAULT(0),
        [SODU_NO] decimal(18,2) NOT NULL DEFAULT(0),
        [OPENING_DATE] datetime2(7) NULL,
        [CLOSING_DATE] datetime2(7) NULL,
        [LAST_TRAN_DATE] datetime2(7) NULL,
        [STATUS] nvarchar(200) NOT NULL DEFAULT(''),
        [PRODUCT_CODE] nvarchar(200) NOT NULL DEFAULT(''),
        [PRODUCT_NAME] nvarchar(200) NOT NULL DEFAULT(''),
        [BRANCH_CODE] nvarchar(200) NOT NULL DEFAULT(''),
        [ACCOUNT_OFFICER] nvarchar(200) NOT NULL DEFAULT(''),
        [CUSTOMER_TYPE] nvarchar(200) NOT NULL DEFAULT(''),
        [MATURITY_DATE] datetime2(7) NULL,
        [INTEREST_RATE] decimal(18,4) NOT NULL DEFAULT(0),
        [ACCRUED_INTEREST] decimal(18,2) NOT NULL DEFAULT(0),
        [LAST_INTEREST_DATE] datetime2(7) NULL,
        [FREEZE_CODE] nvarchar(200) NOT NULL DEFAULT(''),
        [WORKING_BALANCE] decimal(18,2) NOT NULL DEFAULT(0),
        [AVAILABLE_BALANCE] decimal(18,2) NOT NULL DEFAULT(0),
        [Id] bigint IDENTITY(1,1) NOT NULL,
        [CREATED_DATE] datetime2(7) GENERATED ALWAYS AS ROW START NOT NULL,
        [UPDATED_DATE] datetime2(7) GENERATED ALWAYS AS ROW END NOT NULL,
        [FILE_NAME] nvarchar(500) NOT NULL DEFAULT(''),
        PERIOD FOR SYSTEM_TIME ([CREATED_DATE], [UPDATED_DATE]),
        CONSTRAINT [PK_EI01] PRIMARY KEY ([Id])
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[EI01_History]));
    PRINT 'Tạo bảng EI01 thành công';
END;

-- 3. Tạo bảng GL41 (13 business columns)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'GL41')
BEGIN
    CREATE TABLE [GL41] (
        [NGAY_DL] datetime2(7) NOT NULL,
        [STS] nvarchar(200) NOT NULL DEFAULT(''),
        [NGAY_GD] datetime2(7) NULL,
        [NGUOI_TAO] nvarchar(200) NOT NULL DEFAULT(''),
        [DYSEQ] nvarchar(200) NOT NULL DEFAULT(''),
        [TR_TYPE] nvarchar(200) NOT NULL DEFAULT(''),
        [REFNO] nvarchar(200) NOT NULL DEFAULT(''),
        [BRCD] nvarchar(200) NOT NULL DEFAULT(''),
        [TRAD_ACCT] nvarchar(200) NOT NULL DEFAULT(''),
        [CUSTID] nvarchar(200) NOT NULL DEFAULT(''),
        [CUSTNM] nvarchar(200) NOT NULL DEFAULT(''),
        [TR_CURR] nvarchar(200) NOT NULL DEFAULT(''),
        [TR_AMOUNT] decimal(18,2) NOT NULL DEFAULT(0),
        [TRAN_DESC] nvarchar(200) NOT NULL DEFAULT(''),
        [Id] bigint IDENTITY(1,1) NOT NULL,
        [CREATED_DATE] datetime2(7) GENERATED ALWAYS AS ROW START NOT NULL,
        [UPDATED_DATE] datetime2(7) GENERATED ALWAYS AS ROW END NOT NULL,
        [FILE_NAME] nvarchar(500) NOT NULL DEFAULT(''),
        PERIOD FOR SYSTEM_TIME ([CREATED_DATE], [UPDATED_DATE]),
        CONSTRAINT [PK_GL41] PRIMARY KEY ([Id])
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[GL41_History]));
    PRINT 'Tạo bảng GL41 thành công';
END;

-- 4. Tạo bảng LN03 (17 business columns)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'LN03')
BEGIN
    CREATE TABLE [LN03] (
        [NGAY_DL] datetime2(7) NOT NULL,
        [BRCD] nvarchar(200) NOT NULL DEFAULT(''),
        [CUSTSEQ] nvarchar(200) NOT NULL DEFAULT(''),
        [CUSTNM] nvarchar(200) NOT NULL DEFAULT(''),
        [TAI_KHOAN] nvarchar(200) NOT NULL DEFAULT(''),
        [CCY] nvarchar(200) NOT NULL DEFAULT(''),
        [DU_NO] decimal(18,2) NOT NULL DEFAULT(0),
        [TRANSACTION_DATE] datetime2(7) NULL,
        [INTEREST_RATE] decimal(18,4) NOT NULL DEFAULT(0),
        [LOAN_TYPE] nvarchar(200) NOT NULL DEFAULT(''),
        [MATURITY_DATE] datetime2(7) NULL,
        [OFFICER_ID] nvarchar(200) NOT NULL DEFAULT(''),
        [OFFICER_NAME] nvarchar(200) NOT NULL DEFAULT(''),
        [CUSTOMER_TYPE_CODE] nvarchar(200) NOT NULL DEFAULT(''),
        [PROVINCE] nvarchar(200) NOT NULL DEFAULT(''),
        [DISTRICT] nvarchar(200) NOT NULL DEFAULT(''),
        [SECURED_PERCENT] decimal(18,2) NOT NULL DEFAULT(0),
        [NHOM_NO] nvarchar(200) NOT NULL DEFAULT(''),
        [Id] bigint IDENTITY(1,1) NOT NULL,
        [CREATED_DATE] datetime2(7) GENERATED ALWAYS AS ROW START NOT NULL,
        [UPDATED_DATE] datetime2(7) GENERATED ALWAYS AS ROW END NOT NULL,
        [FILE_NAME] nvarchar(500) NOT NULL DEFAULT(''),
        PERIOD FOR SYSTEM_TIME ([CREATED_DATE], [UPDATED_DATE]),
        CONSTRAINT [PK_LN03] PRIMARY KEY ([Id])
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[LN03_History]));
    PRINT 'Tạo bảng LN03 thành công';
END;

-- 5. Tạo bảng RR01 (25 business columns)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RR01')
BEGIN
    CREATE TABLE [RR01] (
        [NGAY_DL] datetime2(7) NOT NULL,
        [CN_LOAI_I] nvarchar(200) NOT NULL DEFAULT(''),
        [BRCD] nvarchar(200) NOT NULL DEFAULT(''),
        [MA_KH] nvarchar(200) NOT NULL DEFAULT(''),
        [TEN_KH] nvarchar(200) NOT NULL DEFAULT(''),
        [SO_LDS] nvarchar(200) NOT NULL DEFAULT(''),
        [SO_TKKD] nvarchar(200) NOT NULL DEFAULT(''),
        [LOAI_HINH_TD] nvarchar(200) NOT NULL DEFAULT(''),
        [PHAN_LOAI_NO] nvarchar(200) NOT NULL DEFAULT(''),
        [DU_NO_GOC] decimal(18,2) NOT NULL DEFAULT(0),
        [DU_LAI_PHAT_SINH] decimal(18,2) NOT NULL DEFAULT(0),
        [TONG_DU_NO] decimal(18,2) NOT NULL DEFAULT(0),
        [GIA_TRI_TSDB] decimal(18,2) NOT NULL DEFAULT(0),
        [TY_LE_TSDB] decimal(18,4) NOT NULL DEFAULT(0),
        [NGAY_TAO_HSD] datetime2(7) NULL,
        [NGAY_DAO_HAN] datetime2(7) NULL,
        [SO_NGAY_QUA_HAN] int NOT NULL DEFAULT(0),
        [NHOM_NO] nvarchar(200) NOT NULL DEFAULT(''),
        [MA_CB] nvarchar(200) NOT NULL DEFAULT(''),
        [TEN_CB] nvarchar(200) NOT NULL DEFAULT(''),
        [NGANH_KT] nvarchar(200) NOT NULL DEFAULT(''),
        [TINH_TP] nvarchar(200) NOT NULL DEFAULT(''),
        [QUAN_HUYEN] nvarchar(200) NOT NULL DEFAULT(''),
        [XA_PHUONG] nvarchar(200) NOT NULL DEFAULT(''),
        [LOAI_KH] nvarchar(200) NOT NULL DEFAULT(''),
        [MA_SP] nvarchar(200) NOT NULL DEFAULT(''),
        [Id] bigint IDENTITY(1,1) NOT NULL,
        [CREATED_DATE] datetime2(7) GENERATED ALWAYS AS ROW START NOT NULL,
        [UPDATED_DATE] datetime2(7) GENERATED ALWAYS AS ROW END NOT NULL,
        [FILE_NAME] nvarchar(500) NOT NULL DEFAULT(''),
        PERIOD FOR SYSTEM_TIME ([CREATED_DATE], [UPDATED_DATE]),
        CONSTRAINT [PK_RR01] PRIMARY KEY ([Id])
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[RR01_History]));
    PRINT 'Tạo bảng RR01 thành công';
END;

PRINT 'Hoàn thành tạo 5 bảng dữ liệu: DPDA, EI01, GL41, LN03, RR01';
