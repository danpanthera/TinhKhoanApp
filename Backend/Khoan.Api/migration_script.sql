IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [7800_DT_KHKD1_History] (
    [Id] int NOT NULL IDENTITY,
    [BusinessKey] nvarchar(500) NOT NULL,
    [EffectiveDate] datetime2 NOT NULL,
    [ExpiryDate] datetime2 NULL,
    [IsCurrent] bit NOT NULL,
    [RowVersion] int NOT NULL,
    [ImportId] nvarchar(100) NOT NULL,
    [StatementDate] datetime2 NOT NULL,
    [ProcessedDate] datetime2 NOT NULL,
    [DataHash] nvarchar(64) NOT NULL,
    [BRCD] nvarchar(20) NULL,
    [BRANCH_NAME] nvarchar(200) NULL,
    [INDICATOR_TYPE] nvarchar(100) NULL,
    [INDICATOR_NAME] nvarchar(500) NULL,
    [PLAN_YEAR] decimal(18,2) NULL,
    [PLAN_QUARTER] decimal(18,2) NULL,
    [PLAN_MONTH] decimal(18,2) NULL,
    [ACTUAL_YEAR] decimal(18,2) NULL,
    [ACTUAL_QUARTER] decimal(18,2) NULL,
    [ACTUAL_MONTH] decimal(18,2) NULL,
    [ACHIEVEMENT_RATE] decimal(10,4) NULL,
    [YEAR] int NULL,
    [QUARTER] int NULL,
    [MONTH] int NULL,
    [CREATED_DATE] datetime2 NULL,
    [UPDATED_DATE] datetime2 NULL,
    [RawDataJson] nvarchar(max) NULL,
    [AdditionalData] nvarchar(max) NULL,
    CONSTRAINT [PK_7800_DT_KHKD1_History] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [BC57_History] (
    [Id] int NOT NULL IDENTITY,
    [BusinessKey] nvarchar(500) NOT NULL,
    [EffectiveDate] datetime2 NOT NULL,
    [ExpiryDate] datetime2 NULL,
    [IsCurrent] bit NOT NULL,
    [RowVersion] int NOT NULL,
    [ImportId] nvarchar(100) NOT NULL,
    [StatementDate] datetime2 NOT NULL,
    [ProcessedDate] datetime2 NOT NULL,
    [DataHash] nvarchar(64) NOT NULL,
    [MaKhachHang] nvarchar(50) NULL,
    [TenKhachHang] nvarchar(500) NULL,
    [SoTaiKhoan] nvarchar(50) NULL,
    [MaHopDong] nvarchar(50) NULL,
    [LoaiSanPham] nvarchar(100) NULL,
    [SoTienGoc] decimal(18,2) NULL,
    [LaiSuat] decimal(5,4) NULL,
    [SoNgayTinhLai] int NULL,
    [TienLaiDuThu] decimal(18,2) NULL,
    [TienLaiQuaHan] decimal(18,2) NULL,
    [NgayBatDau] datetime2 NULL,
    [NgayKetThuc] datetime2 NULL,
    [TrangThai] nvarchar(50) NULL,
    [MaChiNhanh] nvarchar(20) NULL,
    [TenChiNhanh] nvarchar(200) NULL,
    [NgayTinhLai] datetime2 NULL,
    [AdditionalData] nvarchar(max) NULL,
    CONSTRAINT [PK_BC57_History] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [DashboardIndicators] (
    [Id] int NOT NULL IDENTITY,
    [Code] nvarchar(50) NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Unit] nvarchar(50) NULL,
    [Icon] nvarchar(100) NULL,
    [Color] nvarchar(10) NULL,
    [SortOrder] int NOT NULL,
    [IsActive] bit NOT NULL,
    [Description] nvarchar(500) NULL,
    [CreatedDate] datetime2 NOT NULL DEFAULT (GETDATE()),
    [ModifiedDate] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_DashboardIndicators] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [DB01_History] (
    [Id] int NOT NULL IDENTITY,
    [BusinessKey] nvarchar(500) NOT NULL,
    [EffectiveDate] datetime2 NOT NULL,
    [ExpiryDate] datetime2 NULL,
    [IsCurrent] bit NOT NULL,
    [RowVersion] int NOT NULL,
    [ImportId] nvarchar(100) NOT NULL,
    [StatementDate] datetime2 NOT NULL,
    [ProcessedDate] datetime2 NOT NULL,
    [DataHash] nvarchar(64) NOT NULL,
    [MaKhachHang] nvarchar(50) NULL,
    [TenKhachHang] nvarchar(500) NULL,
    [SoTaiKhoan] nvarchar(50) NULL,
    [LoaiTaiKhoan] nvarchar(100) NULL,
    [SoDu] decimal(18,2) NULL,
    [SoDuKhaDung] decimal(18,2) NULL,
    [NgayMoTK] datetime2 NULL,
    [TrangThaiTK] nvarchar(50) NULL,
    [LaiSuat] decimal(5,4) NULL,
    [KyHan] int NULL,
    [NgayDaoHan] datetime2 NULL,
    [SoTienGocGuy] decimal(18,2) NULL,
    [TienLaiDuThu] decimal(18,2) NULL,
    [MaChiNhanh] nvarchar(20) NULL,
    [TenChiNhanh] nvarchar(200) NULL,
    [LoaiHinhDB] nvarchar(50) NULL,
    [AdditionalData] nvarchar(max) NULL,
    CONSTRAINT [PK_DB01_History] PRIMARY KEY ([Id])
);
GO

DECLARE @historyTableSchema sysname = SCHEMA_NAME()
EXEC(N'CREATE TABLE [DPDA] (
    [Id] int NOT NULL IDENTITY,
    [BusinessKey] nvarchar(500) NOT NULL,
    [EffectiveDate] datetime2 NOT NULL,
    [ExpiryDate] datetime2 NULL,
    [IsCurrent] bit NOT NULL,
    [RowVersion] int NOT NULL,
    [ImportId] nvarchar(100) NOT NULL,
    [StatementDate] datetime2 NOT NULL,
    [ProcessedDate] datetime2 NOT NULL,
    [DataHash] nvarchar(64) NOT NULL,
    [MaKhachHang] nvarchar(50) NULL,
    [TenKhachHang] nvarchar(500) NULL,
    [SoThe] nvarchar(50) NULL,
    [LoaiThe] nvarchar(100) NULL,
    [TrangThaiThe] nvarchar(50) NULL,
    [NgayPhatHanh] datetime2 NULL,
    [NgayHetHan] datetime2 NULL,
    [HanMucThe] decimal(18,2) NULL,
    [SoDuHienTai] decimal(18,2) NULL,
    [SoTienDaSD] decimal(18,2) NULL,
    [MaChiNhanh] nvarchar(20) NULL,
    [TenChiNhanh] nvarchar(200) NULL,
    [NgayTao] datetime2 NULL,
    [NgayCapNhat] datetime2 NULL,
    [AdditionalData] nvarchar(max) NULL,
    [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    CONSTRAINT [PK_DPDA] PRIMARY KEY ([Id]),
    PERIOD FOR SYSTEM_TIME([SysStartTime], [SysEndTime])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + N'].[DPDA_History]))');
GO

DECLARE @historyTableSchema sysname = SCHEMA_NAME()
EXEC(N'CREATE TABLE [EI01] (
    [Id] int NOT NULL IDENTITY,
    [BusinessKey] nvarchar(500) NOT NULL,
    [EffectiveDate] datetime2 NOT NULL,
    [ExpiryDate] datetime2 NULL,
    [IsCurrent] bit NOT NULL,
    [RowVersion] int NOT NULL,
    [ImportId] nvarchar(100) NOT NULL,
    [StatementDate] datetime2 NOT NULL,
    [ProcessedDate] datetime2 NOT NULL,
    [DataHash] nvarchar(64) NOT NULL,
    [MaKhachHang] nvarchar(50) NULL,
    [TenKhachHang] nvarchar(500) NULL,
    [SoTaiKhoan] nvarchar(50) NULL,
    [LoaiGiaoDich] nvarchar(100) NULL,
    [MaGiaoDich] nvarchar(100) NULL,
    [SoTien] decimal(18,2) NULL,
    [NgayGiaoDich] datetime2 NULL,
    [ThoiGianGiaoDich] datetime2 NULL,
    [TrangThaiGiaoDich] nvarchar(50) NULL,
    [NoiDungGiaoDich] nvarchar(1000) NULL,
    [MaChiNhanh] nvarchar(20) NULL,
    [TenChiNhanh] nvarchar(200) NULL,
    [Channel] nvarchar(50) NULL,
    [DeviceInfo] nvarchar(500) NULL,
    [AdditionalData] nvarchar(max) NULL,
    [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    CONSTRAINT [PK_EI01] PRIMARY KEY ([Id]),
    PERIOD FOR SYSTEM_TIME([SysStartTime], [SysEndTime])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + N'].[EI01_History]))');
GO

CREATE TABLE [GAHR26_History] (
    [Id] int NOT NULL IDENTITY,
    [BusinessKey] nvarchar(500) NOT NULL,
    [EffectiveDate] datetime2 NOT NULL,
    [ExpiryDate] datetime2 NULL,
    [IsCurrent] bit NOT NULL,
    [RowVersion] int NOT NULL,
    [ImportId] nvarchar(100) NOT NULL,
    [StatementDate] datetime2 NOT NULL,
    [ProcessedDate] datetime2 NOT NULL,
    [DataHash] nvarchar(64) NOT NULL,
    [EMP_ID] nvarchar(50) NULL,
    [EMP_NAME] nvarchar(500) NULL,
    [ID_NUMBER] nvarchar(20) NULL,
    [POSITION] nvarchar(100) NULL,
    [BRCD] nvarchar(20) NULL,
    [BRANCH_NAME] nvarchar(200) NULL,
    [DEPARTMENT] nvarchar(100) NULL,
    [JOIN_DATE] datetime2 NULL,
    [BIRTH_DATE] datetime2 NULL,
    [GENDER] nvarchar(10) NULL,
    [ADDRESS] nvarchar(500) NULL,
    [PHONE] nvarchar(20) NULL,
    [EMAIL] nvarchar(100) NULL,
    [STATUS] nvarchar(50) NULL,
    [BASIC_SALARY] decimal(18,2) NULL,
    [ALLOWANCE] decimal(18,2) NULL,
    [CREATED_DATE] datetime2 NULL,
    [UPDATED_DATE] datetime2 NULL,
    [AdditionalData] nvarchar(max) NULL,
    CONSTRAINT [PK_GAHR26_History] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [GL01_History] (
    [HistoryID] bigint NOT NULL IDENTITY,
    [MANDT] nvarchar(3) NOT NULL,
    [BUKRS] nvarchar(4) NOT NULL,
    [GJAHR] nvarchar(4) NOT NULL,
    [BELNR] nvarchar(10) NOT NULL,
    [BUZEI] nvarchar(3) NOT NULL,
    [AUGDT] nvarchar(8) NOT NULL,
    [AUGCP] nvarchar(6) NOT NULL,
    [AUGBL] nvarchar(10) NOT NULL,
    [BSCHL] nvarchar(2) NOT NULL,
    [KOART] nvarchar(1) NOT NULL,
    [UMSKZ] nvarchar(1) NOT NULL,
    [UMSKS] nvarchar(1) NOT NULL,
    [ZUMSK] nvarchar(1) NOT NULL,
    [SHKZG] nvarchar(1) NOT NULL,
    [GSBER] nvarchar(4) NOT NULL,
    [PARGB] nvarchar(4) NOT NULL,
    [MWSKZ] nvarchar(2) NOT NULL,
    [QSSKZ] nvarchar(2) NOT NULL,
    [DMBTR] decimal(13,2) NULL,
    [WRBTR] decimal(13,2) NULL,
    [KZBTR] decimal(13,2) NULL,
    [PSWBT] decimal(13,2) NULL,
    [PSWSL] nvarchar(5) NOT NULL,
    [TXBHW] decimal(13,2) NULL,
    [TXBFW] decimal(13,2) NULL,
    [MWSTS] decimal(13,2) NULL,
    [MWSTV] decimal(13,2) NULL,
    [HWBAS] decimal(13,2) NULL,
    [FWBAS] decimal(13,2) NULL,
    [HWSTE] decimal(13,2) NULL,
    [FWSTE] decimal(13,2) NULL,
    [STBLG] nvarchar(10) NOT NULL,
    [STGRD] nvarchar(2) NOT NULL,
    [VALUT] nvarchar(8) NOT NULL,
    [ZUONR] nvarchar(18) NOT NULL,
    [SGTXT] nvarchar(50) NOT NULL,
    [ZINKZ] nvarchar(1) NOT NULL,
    [VBUND] nvarchar(6) NOT NULL,
    [BEWAR] nvarchar(3) NOT NULL,
    [ALTKT] nvarchar(10) NOT NULL,
    [VORGN] nvarchar(4) NOT NULL,
    [FDLEV] nvarchar(2) NOT NULL,
    [FDGRP] nvarchar(10) NOT NULL,
    [HKONT] nvarchar(10) NOT NULL,
    [KUNNR] nvarchar(10) NOT NULL,
    [LIFNR] nvarchar(10) NOT NULL,
    [FILKD] nvarchar(10) NOT NULL,
    [XBILK] nvarchar(1) NOT NULL,
    [GVTYP] nvarchar(2) NOT NULL,
    [HZUON] nvarchar(18) NOT NULL,
    [ZFBDT] nvarchar(8) NOT NULL,
    [ZTERM] nvarchar(4) NOT NULL,
    [ZBD1T] int NULL,
    [ZBD2T] int NULL,
    [ZBD3T] int NULL,
    [ZBD1P] decimal(5,3) NULL,
    [ZBD2P] decimal(5,3) NULL,
    [SKFBT] decimal(13,2) NULL,
    [SKNTO] decimal(13,2) NULL,
    [WSKTO] decimal(13,2) NULL,
    [ZLSCH] nvarchar(1) NOT NULL,
    [ZLSPR] nvarchar(1) NOT NULL,
    [ZBFIX] nvarchar(1) NOT NULL,
    [HBKID] nvarchar(5) NOT NULL,
    [BVTYP] nvarchar(4) NOT NULL,
    [NEBTR] decimal(13,2) NULL,
    [MWART] nvarchar(1) NOT NULL,
    [DMBE2] decimal(13,2) NULL,
    [DMBE3] decimal(13,2) NULL,
    [PPRCT] nvarchar(3) NOT NULL,
    [XREF1] nvarchar(12) NOT NULL,
    [XREF2] nvarchar(12) NOT NULL,
    [KOST1] nvarchar(12) NOT NULL,
    [KOST2] nvarchar(12) NOT NULL,
    [VBEL2] nvarchar(10) NOT NULL,
    [POSN2] nvarchar(6) NOT NULL,
    [KKBER] nvarchar(4) NOT NULL,
    [EMPFB] nvarchar(12) NOT NULL,
    [SourceID] nvarchar(50) NOT NULL,
    [ValidFrom] datetime2 NOT NULL,
    [ValidTo] datetime2 NOT NULL,
    [IsCurrent] bit NOT NULL,
    [VersionNumber] int NOT NULL,
    [RecordHash] nvarchar(max) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [ModifiedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_GL01_History] PRIMARY KEY ([HistoryID])
);
GO

CREATE TABLE [GLCB41_History] (
    [Id] int NOT NULL IDENTITY,
    [BusinessKey] nvarchar(500) NOT NULL,
    [EffectiveDate] datetime2 NOT NULL,
    [ExpiryDate] datetime2 NULL,
    [IsCurrent] bit NOT NULL,
    [RowVersion] int NOT NULL,
    [ImportId] nvarchar(100) NOT NULL,
    [StatementDate] datetime2 NOT NULL,
    [ProcessedDate] datetime2 NOT NULL,
    [DataHash] nvarchar(64) NOT NULL,
    [JOURNAL_NO] nvarchar(50) NULL,
    [ACCOUNT_NO] nvarchar(50) NULL,
    [ACCOUNT_NAME] nvarchar(500) NULL,
    [CUSTOMER_ID] nvarchar(50) NULL,
    [CUSTOMER_NAME] nvarchar(500) NULL,
    [TRANSACTION_DATE] datetime2 NULL,
    [POSTING_DATE] datetime2 NULL,
    [DESCRIPTION] nvarchar(1000) NULL,
    [DEBIT_AMOUNT] decimal(18,2) NULL,
    [CREDIT_AMOUNT] decimal(18,2) NULL,
    [DEBIT_BALANCE] decimal(18,2) NULL,
    [CREDIT_BALANCE] decimal(18,2) NULL,
    [BRCD] nvarchar(20) NULL,
    [BRANCH_NAME] nvarchar(200) NULL,
    [TRANSACTION_TYPE] nvarchar(100) NULL,
    [ORIGINAL_TRANS_ID] nvarchar(50) NULL,
    [CREATED_DATE] datetime2 NULL,
    [UPDATED_DATE] datetime2 NULL,
    [RawDataJson] nvarchar(max) NULL,
    [AdditionalData] nvarchar(max) NULL,
    CONSTRAINT [PK_GLCB41_History] PRIMARY KEY ([Id])
);
GO

DECLARE @historyTableSchema sysname = SCHEMA_NAME()
EXEC(N'CREATE TABLE [ImportedDataRecords] (
    [Id] int NOT NULL IDENTITY,
    [FileName] nvarchar(255) NOT NULL,
    [FileType] nvarchar(100) NOT NULL,
    [Category] nvarchar(100) NOT NULL,
    [ImportDate] datetime2 NOT NULL,
    [StatementDate] datetime2 NULL,
    [ImportedBy] nvarchar(100) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [RecordsCount] int NOT NULL,
    [OriginalFileData] varbinary(max) NULL,
    [Notes] nvarchar(1000) NULL,
    [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    CONSTRAINT [PK_ImportedDataRecords] PRIMARY KEY ([Id]),
    PERIOD FOR SYSTEM_TIME([SysStartTime], [SysEndTime])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + N'].[ImportedDataRecords_History]))');
GO

CREATE TABLE [ImportLogs] (
    [Id] bigint NOT NULL IDENTITY,
    [ImportBatchId] uniqueidentifier NOT NULL,
    [ImportDate] datetime2 NOT NULL,
    [ImportSource] nvarchar(50) NOT NULL,
    [RecordsCount] int NOT NULL,
    [SuccessCount] int NOT NULL,
    [ErrorCount] int NOT NULL,
    [StartTime] datetime2 NOT NULL,
    [EndTime] datetime2 NOT NULL,
    [ProcessingTimeMs] bigint NOT NULL,
    [ErrorDetails] nvarchar(1000) NULL,
    [Status] nvarchar(20) NOT NULL,
    [CreatedBy] nvarchar(100) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [BatchId] nvarchar(50) NOT NULL,
    [TableName] nvarchar(50) NOT NULL,
    [TotalRecords] int NOT NULL,
    [ProcessedRecords] int NOT NULL,
    [NewRecords] int NOT NULL,
    [UpdatedRecords] int NOT NULL,
    [DeletedRecords] int NOT NULL,
    [ErrorMessage] nvarchar(max) NULL,
    [Duration] int NULL,
    CONSTRAINT [PK_ImportLogs] PRIMARY KEY ([Id])
);
GO

DECLARE @historyTableSchema sysname = SCHEMA_NAME()
EXEC(N'CREATE TABLE [KH03] (
    [Id] int NOT NULL IDENTITY,
    [BusinessKey] nvarchar(500) NOT NULL,
    [EffectiveDate] datetime2 NOT NULL,
    [ExpiryDate] datetime2 NULL,
    [IsCurrent] bit NOT NULL,
    [RowVersion] int NOT NULL,
    [ImportId] nvarchar(100) NOT NULL,
    [StatementDate] datetime2 NOT NULL,
    [ProcessedDate] datetime2 NOT NULL,
    [DataHash] nvarchar(64) NOT NULL,
    [MaKhachHang] nvarchar(50) NULL,
    [TenKhachHang] nvarchar(500) NULL,
    [MaSoThue] nvarchar(50) NULL,
    [DiaChi] nvarchar(1000) NULL,
    [SoDienThoai] nvarchar(50) NULL,
    [Email] nvarchar(200) NULL,
    [NguoiDaiDien] nvarchar(500) NULL,
    [ChucVuNDD] nvarchar(200) NULL,
    [NgaySinh] datetime2 NULL,
    [CMND_CCCD] nvarchar(50) NULL,
    [NgayCapCMND] datetime2 NULL,
    [NoiCapCMND] nvarchar(200) NULL,
    [LoaiKhachHang] nvarchar(100) NULL,
    [PhanKhuc] nvarchar(100) NULL,
    [MaChiNhanh] nvarchar(20) NULL,
    [TenChiNhanh] nvarchar(200) NULL,
    [TrangThaiKH] nvarchar(50) NULL,
    [NgayTao] datetime2 NULL,
    [NgayCapNhat] datetime2 NULL,
    [AdditionalData] nvarchar(max) NULL,
    [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    CONSTRAINT [PK_KH03] PRIMARY KEY ([Id]),
    PERIOD FOR SYSTEM_TIME([SysStartTime], [SysEndTime])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + N'].[KH03_History]))');
GO

CREATE TABLE [KhoanPeriods] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Type] int NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_KhoanPeriods] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [KpiAssignmentTables] (
    [Id] int NOT NULL IDENTITY,
    [TableType] int NOT NULL,
    [TableName] nvarchar(200) NOT NULL,
    [Description] nvarchar(500) NULL,
    [Category] nvarchar(100) NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_KpiAssignmentTables] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [KPIDefinitions] (
    [Id] int NOT NULL IDENTITY,
    [KpiCode] nvarchar(100) NOT NULL,
    [KpiName] nvarchar(500) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [MaxScore] decimal(18,2) NOT NULL,
    [ValueType] int NOT NULL,
    [UnitOfMeasure] nvarchar(50) NULL,
    [IsActive] bit NOT NULL,
    [Version] nvarchar(20) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NULL,
    CONSTRAINT [PK_KPIDefinitions] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [KpiScoringRules] (
    [Id] int NOT NULL IDENTITY,
    [KpiIndicatorName] nvarchar(200) NOT NULL,
    [RuleType] nvarchar(50) NOT NULL DEFAULT N'COMPLETION_RATE',
    [MinValue] decimal(18,2) NULL,
    [MaxValue] decimal(18,2) NULL,
    [ScoreFormula] nvarchar(max) NULL,
    [BonusPoints] decimal(18,2) NOT NULL,
    [PenaltyPoints] decimal(18,2) NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NULL,
    CONSTRAINT [PK_KpiScoringRules] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [LegacyRawDataImports] (
    [Id] int NOT NULL IDENTITY,
    [FileName] nvarchar(200) NOT NULL,
    [DataType] nvarchar(50) NOT NULL,
    [ImportDate] datetime2 NOT NULL,
    [StatementDate] datetime2 NOT NULL,
    [ImportedBy] nvarchar(100) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [RecordsCount] int NOT NULL,
    [OriginalFileData] varbinary(max) NULL,
    [Notes] nvarchar(500) NULL,
    CONSTRAINT [PK_LegacyRawDataImports] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [LN01_CsvHistory] (
    [Id] int NOT NULL IDENTITY,
    [BusinessKey] nvarchar(500) NOT NULL,
    [EffectiveDate] datetime2 NOT NULL,
    [ExpiryDate] datetime2 NULL,
    [IsCurrent] bit NOT NULL,
    [RowVersion] int NOT NULL,
    [ImportId] nvarchar(100) NOT NULL,
    [StatementDate] datetime2 NOT NULL,
    [ProcessedDate] datetime2 NOT NULL,
    [DataHash] nvarchar(64) NOT NULL,
    [BRCD] nvarchar(20) NULL,
    [CUSTSEQ] nvarchar(50) NULL,
    [CUSTNM] nvarchar(200) NULL,
    [TAI_KHOAN] nvarchar(50) NULL,
    [CCY] nvarchar(3) NULL,
    [DU_NO] decimal(18,2) NULL,
    [DSBSSEQ] nvarchar(50) NULL,
    [TRANSACTION_DATE] datetime2 NULL,
    [DSBSDT] datetime2 NULL,
    [DISBUR_CCY] nvarchar(3) NULL,
    [DISBURSEMENT_AMOUNT] decimal(18,2) NULL,
    [RawDataJson] nvarchar(max) NULL,
    [AdditionalData] nvarchar(max) NULL,
    CONSTRAINT [PK_LN01_CsvHistory] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [LN01_History] (
    [HistoryID] bigint NOT NULL IDENTITY,
    [MANDT] nvarchar(3) NOT NULL,
    [BUKRS] nvarchar(4) NOT NULL,
    [LAND1] nvarchar(3) NOT NULL,
    [WAERS] nvarchar(5) NOT NULL,
    [SPRAS] nvarchar(1) NOT NULL,
    [KTOPL] nvarchar(4) NOT NULL,
    [WAABW] nvarchar(2) NOT NULL,
    [PERIV] nvarchar(2) NOT NULL,
    [KOKFI] nvarchar(1) NOT NULL,
    [RCOMP] nvarchar(6) NOT NULL,
    [ADRNR] nvarchar(10) NOT NULL,
    [STCEG] nvarchar(20) NOT NULL,
    [FIKRS] nvarchar(4) NOT NULL,
    [XFMCO] nvarchar(1) NOT NULL,
    [XFMCB] nvarchar(1) NOT NULL,
    [XFMCA] nvarchar(1) NOT NULL,
    [TXJCD] nvarchar(15) NOT NULL,
    [SourceID] nvarchar(50) NOT NULL,
    [ValidFrom] datetime2 NOT NULL,
    [ValidTo] datetime2 NOT NULL,
    [IsCurrent] bit NOT NULL,
    [VersionNumber] int NOT NULL,
    [RecordHash] nvarchar(max) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [ModifiedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_LN01_History] PRIMARY KEY ([HistoryID])
);
GO

CREATE TABLE [LN03_History] (
    [Id] int NOT NULL IDENTITY,
    [BusinessKey] nvarchar(500) NOT NULL,
    [EffectiveDate] datetime2 NOT NULL,
    [ExpiryDate] datetime2 NULL,
    [IsCurrent] bit NOT NULL,
    [RowVersion] int NOT NULL,
    [ImportId] nvarchar(100) NOT NULL,
    [StatementDate] datetime2 NOT NULL,
    [ProcessedDate] datetime2 NOT NULL,
    [DataHash] nvarchar(64) NOT NULL,
    [MaKhachHang] nvarchar(50) NULL,
    [TenKhachHang] nvarchar(500) NULL,
    [MaHopDong] nvarchar(50) NULL,
    [LoaiHopDong] nvarchar(100) NULL,
    [SoTienGoc] decimal(18,2) NULL,
    [SoTienLai] decimal(18,2) NULL,
    [TongNo] decimal(18,2) NULL,
    [NgayDaoHan] datetime2 NULL,
    [TinhTrangNo] nvarchar(100) NULL,
    [NhomNo] int NULL,
    [MaChiNhanh] nvarchar(20) NULL,
    [TenChiNhanh] nvarchar(200) NULL,
    [NgayTao] datetime2 NULL,
    [NgayCapNhat] datetime2 NULL,
    [AdditionalData] nvarchar(max) NULL,
    CONSTRAINT [PK_LN03_History] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [OptimizedRawDataImports] (
    [Id] bigint NOT NULL IDENTITY,
    [ImportDate] datetime2 NOT NULL,
    [BranchCode] nvarchar(10) NOT NULL,
    [DepartmentCode] nvarchar(10) NOT NULL,
    [EmployeeCode] nvarchar(20) NOT NULL,
    [KpiCode] nvarchar(20) NOT NULL,
    [KpiValue] decimal(18,4) NOT NULL,
    [Unit] nvarchar(10) NULL,
    [Target] decimal(18,4) NULL,
    [Achievement] decimal(18,4) NULL,
    [Score] decimal(5,2) NULL,
    [Note] nvarchar(500) NULL,
    [ImportBatchId] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar(100) NOT NULL,
    [LastModifiedDate] datetime2 NOT NULL,
    [LastModifiedBy] nvarchar(100) NOT NULL,
    [IsDeleted] bit NOT NULL,
    [ValidFrom] datetime2 NOT NULL,
    [ValidTo] datetime2 NOT NULL,
    [KpiName] nvarchar(200) NOT NULL,
    [DataType] nvarchar(50) NOT NULL,
    [FileName] nvarchar(500) NOT NULL,
    CONSTRAINT [PK_OptimizedRawDataImports] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Positions] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(150) NOT NULL,
    [Description] nvarchar(500) NULL,
    CONSTRAINT [PK_Positions] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [RawDataImportArchives] (
    [Id] bigint NOT NULL IDENTITY,
    [ImportDate] datetime2 NOT NULL,
    [BranchCode] nvarchar(10) NOT NULL,
    [DepartmentCode] nvarchar(10) NOT NULL,
    [EmployeeCode] nvarchar(20) NOT NULL,
    [KpiCode] nvarchar(20) NOT NULL,
    [KpiValue] decimal(18,4) NOT NULL,
    [Unit] nvarchar(10) NULL,
    [Target] decimal(18,4) NULL,
    [Achievement] decimal(18,4) NULL,
    [Score] decimal(5,2) NULL,
    [Note] nvarchar(500) NULL,
    [ImportBatchId] uniqueidentifier NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar(100) NOT NULL,
    [LastModifiedDate] datetime2 NOT NULL,
    [LastModifiedBy] nvarchar(100) NOT NULL,
    [IsDeleted] bit NOT NULL,
    [ArchivedDate] datetime2 NOT NULL,
    [ArchivedBy] nvarchar(100) NOT NULL,
    [ValidFrom] datetime2 NOT NULL,
    [ValidTo] datetime2 NOT NULL,
    CONSTRAINT [PK_RawDataImportArchives] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Roles] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(255) NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [SalaryParameters] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Value] decimal(18,2) NOT NULL,
    [Note] nvarchar(max) NULL,
    CONSTRAINT [PK_SalaryParameters] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [TransactionAdjustmentFactors] (
    [Id] int NOT NULL IDENTITY,
    [LegacyKPIDefinitionId] int NULL,
    [Factor] decimal(18,4) NOT NULL,
    [Note] nvarchar(max) NULL,
    CONSTRAINT [PK_TransactionAdjustmentFactors] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Units] (
    [Id] int NOT NULL IDENTITY,
    [Code] nvarchar(50) NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    [Type] nvarchar(100) NULL,
    [ParentUnitId] int NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Units] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Units_Units_ParentUnitId] FOREIGN KEY ([ParentUnitId]) REFERENCES [Units] ([Id]) ON DELETE NO ACTION
);
GO

DECLARE @historyTableSchema sysname = SCHEMA_NAME()
EXEC(N'CREATE TABLE [ImportedDataItems] (
    [Id] int NOT NULL IDENTITY,
    [ImportedDataRecordId] int NOT NULL,
    [RawData] nvarchar(max) NOT NULL,
    [ProcessedDate] datetime2 NOT NULL,
    [ProcessingNotes] nvarchar(500) NULL,
    [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    CONSTRAINT [PK_ImportedDataItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ImportedDataItems_ImportedDataRecords_ImportedDataRecordId] FOREIGN KEY ([ImportedDataRecordId]) REFERENCES [ImportedDataRecords] ([Id]) ON DELETE CASCADE,
    PERIOD FOR SYSTEM_TIME([SysStartTime], [SysEndTime])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + N'].[ImportedDataItems_History]))');
GO

CREATE TABLE [MSIT72_TSBD] (
    [Id] bigint NOT NULL IDENTITY,
    [ImportedDataRecordId] int NOT NULL,
    [RawData] NVARCHAR(MAX) NULL,
    [ProcessedData] NVARCHAR(MAX) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [ModifiedAt] datetime2 NOT NULL,
    [SysStartTime] datetime2 NOT NULL,
    [SysEndTime] datetime2 NOT NULL,
    CONSTRAINT [PK_MSIT72_TSBD] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MSIT72_TSBD_ImportedDataRecords_ImportedDataRecordId] FOREIGN KEY ([ImportedDataRecordId]) REFERENCES [ImportedDataRecords] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [MSIT72_TSGH] (
    [Id] bigint NOT NULL IDENTITY,
    [ImportedDataRecordId] int NOT NULL,
    [RawData] NVARCHAR(MAX) NULL,
    [ProcessedData] NVARCHAR(MAX) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [ModifiedAt] datetime2 NOT NULL,
    [SysStartTime] datetime2 NOT NULL,
    [SysEndTime] datetime2 NOT NULL,
    CONSTRAINT [PK_MSIT72_TSGH] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MSIT72_TSGH_ImportedDataRecords_ImportedDataRecordId] FOREIGN KEY ([ImportedDataRecordId]) REFERENCES [ImportedDataRecords] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [ThuXLRR] (
    [Id] bigint NOT NULL IDENTITY,
    [ImportedDataRecordId] int NOT NULL,
    [RawData] NVARCHAR(MAX) NULL,
    [ProcessedData] NVARCHAR(MAX) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [ModifiedAt] datetime2 NOT NULL,
    [SysStartTime] datetime2 NOT NULL,
    [SysEndTime] datetime2 NOT NULL,
    CONSTRAINT [PK_ThuXLRR] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ThuXLRR_ImportedDataRecords_ImportedDataRecordId] FOREIGN KEY ([ImportedDataRecordId]) REFERENCES [ImportedDataRecords] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [KpiIndicators] (
    [Id] int NOT NULL IDENTITY,
    [TableId] int NOT NULL,
    [IndicatorName] nvarchar(300) NOT NULL,
    [MaxScore] decimal(18,2) NOT NULL,
    [Unit] nvarchar(50) NOT NULL,
    [OrderIndex] int NOT NULL,
    [ValueType] int NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_KpiIndicators] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_KpiIndicators_KpiAssignmentTables_TableId] FOREIGN KEY ([TableId]) REFERENCES [KpiAssignmentTables] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [RawDataRecords] (
    [Id] int NOT NULL IDENTITY,
    [RawDataImportId] int NOT NULL,
    [JsonData] nvarchar(max) NOT NULL,
    [ProcessedDate] datetime2 NOT NULL,
    [ProcessingNotes] nvarchar(500) NULL,
    CONSTRAINT [PK_RawDataRecords] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RawDataRecords_LegacyRawDataImports_RawDataImportId] FOREIGN KEY ([RawDataImportId]) REFERENCES [LegacyRawDataImports] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [BusinessPlanTargets] (
    [Id] int NOT NULL IDENTITY,
    [DashboardIndicatorId] int NOT NULL,
    [UnitId] int NOT NULL,
    [Year] int NOT NULL,
    [Quarter] int NULL,
    [Month] int NULL,
    [TargetValue] decimal(18,2) NOT NULL,
    [Notes] nvarchar(500) NULL,
    [Status] nvarchar(50) NULL DEFAULT N'Draft',
    [CreatedDate] datetime2 NOT NULL DEFAULT (GETDATE()),
    [CreatedBy] nvarchar(100) NULL,
    [ModifiedDate] datetime2 NULL,
    [ModifiedBy] nvarchar(100) NULL,
    [IsDeleted] bit NOT NULL,
    [ApprovedBy] nvarchar(100) NULL,
    [ApprovedAt] datetime2 NULL,
    CONSTRAINT [PK_BusinessPlanTargets] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BusinessPlanTargets_DashboardIndicators_DashboardIndicatorId] FOREIGN KEY ([DashboardIndicatorId]) REFERENCES [DashboardIndicators] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_BusinessPlanTargets_Units_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [Units] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [DashboardCalculations] (
    [Id] int NOT NULL IDENTITY,
    [DashboardIndicatorId] int NOT NULL,
    [UnitId] int NOT NULL,
    [Year] int NOT NULL,
    [Quarter] int NULL,
    [Month] int NULL,
    [CalculationDate] datetime2 NOT NULL,
    [ActualValue] decimal(18,2) NULL,
    [CalculationDetails] nvarchar(max) NULL,
    [DataSource] nvarchar(50) NULL,
    [DataDate] datetime2 NOT NULL,
    [CreatedDate] datetime2 NOT NULL DEFAULT (GETDATE()),
    [CreatedBy] nvarchar(100) NULL,
    [ModifiedDate] datetime2 NULL,
    [ModifiedBy] nvarchar(100) NULL,
    [Status] nvarchar(20) NOT NULL DEFAULT N'Success',
    [ErrorMessage] nvarchar(1000) NULL,
    [ExecutionTime] time NULL,
    [AppliedFilters] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_DashboardCalculations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DashboardCalculations_DashboardIndicators_DashboardIndicatorId] FOREIGN KEY ([DashboardIndicatorId]) REFERENCES [DashboardIndicators] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_DashboardCalculations_Units_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [Units] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Employees] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeCode] nvarchar(20) NOT NULL,
    [CBCode] nvarchar(9) NOT NULL,
    [FullName] nvarchar(255) NOT NULL,
    [Username] nvarchar(100) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [Email] nvarchar(255) NULL,
    [PhoneNumber] nvarchar(20) NULL,
    [IsActive] bit NOT NULL,
    [UnitId] int NOT NULL,
    [PositionId] int NOT NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Employees_Positions_PositionId] FOREIGN KEY ([PositionId]) REFERENCES [Positions] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Employees_Units_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [Units] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [UnitKhoanAssignments] (
    [Id] int NOT NULL IDENTITY,
    [UnitId] int NOT NULL,
    [KhoanPeriodId] int NOT NULL,
    [AssignedDate] datetime2 NOT NULL,
    [Note] nvarchar(max) NULL,
    CONSTRAINT [PK_UnitKhoanAssignments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UnitKhoanAssignments_KhoanPeriods_KhoanPeriodId] FOREIGN KEY ([KhoanPeriodId]) REFERENCES [KhoanPeriods] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UnitKhoanAssignments_Units_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [Units] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [EmployeeKhoanAssignments] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [KhoanPeriodId] int NOT NULL,
    [AssignedDate] datetime2 NOT NULL,
    [Note] nvarchar(max) NULL,
    CONSTRAINT [PK_EmployeeKhoanAssignments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EmployeeKhoanAssignments_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_EmployeeKhoanAssignments_KhoanPeriods_KhoanPeriodId] FOREIGN KEY ([KhoanPeriodId]) REFERENCES [KhoanPeriods] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [EmployeeKpiAssignments] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [KpiDefinitionId] int NOT NULL,
    [KhoanPeriodId] int NOT NULL,
    [TargetValue] decimal(18,2) NOT NULL,
    [ActualValue] decimal(18,2) NULL,
    [Score] decimal(18,2) NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NULL,
    [Notes] nvarchar(1000) NULL,
    CONSTRAINT [PK_EmployeeKpiAssignments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EmployeeKpiAssignments_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_EmployeeKpiAssignments_KPIDefinitions_KpiDefinitionId] FOREIGN KEY ([KpiDefinitionId]) REFERENCES [KPIDefinitions] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_EmployeeKpiAssignments_KhoanPeriods_KhoanPeriodId] FOREIGN KEY ([KhoanPeriodId]) REFERENCES [KhoanPeriods] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [EmployeeKpiTargets] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [IndicatorId] int NOT NULL,
    [KhoanPeriodId] int NOT NULL,
    [TargetValue] decimal(18,2) NULL,
    [ActualValue] decimal(18,2) NULL,
    [Score] decimal(18,2) NULL,
    [AssignedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NULL,
    [Notes] nvarchar(500) NULL,
    CONSTRAINT [PK_EmployeeKpiTargets] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EmployeeKpiTargets_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_EmployeeKpiTargets_KhoanPeriods_KhoanPeriodId] FOREIGN KEY ([KhoanPeriodId]) REFERENCES [KhoanPeriods] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_EmployeeKpiTargets_KpiIndicators_IndicatorId] FOREIGN KEY ([IndicatorId]) REFERENCES [KpiIndicators] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [EmployeeRoles] (
    [EmployeeId] int NOT NULL,
    [RoleId] int NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_EmployeeRoles] PRIMARY KEY ([EmployeeId], [RoleId]),
    CONSTRAINT [FK_EmployeeRoles_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_EmployeeRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [FinalPayouts] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [KhoanPeriodId] int NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [V1] decimal(18,2) NULL,
    [V2] decimal(18,2) NULL,
    [CompletionFactor] decimal(18,4) NULL,
    [Note] nvarchar(max) NULL,
    CONSTRAINT [PK_FinalPayouts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FinalPayouts_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_FinalPayouts_KhoanPeriods_KhoanPeriodId] FOREIGN KEY ([KhoanPeriodId]) REFERENCES [KhoanPeriods] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [UnitKhoanAssignmentDetails] (
    [Id] int NOT NULL IDENTITY,
    [UnitKhoanAssignmentId] int NOT NULL,
    [LegacyKPICode] nvarchar(max) NULL,
    [LegacyKPIName] nvarchar(max) NULL,
    [TargetValue] decimal(18,2) NOT NULL,
    [ActualValue] decimal(18,2) NULL,
    [Score] decimal(18,2) NULL,
    [Note] nvarchar(max) NULL,
    CONSTRAINT [PK_UnitKhoanAssignmentDetails] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UnitKhoanAssignmentDetails_UnitKhoanAssignments_UnitKhoanAssignmentId] FOREIGN KEY ([UnitKhoanAssignmentId]) REFERENCES [UnitKhoanAssignments] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [UnitKpiScorings] (
    [Id] int NOT NULL IDENTITY,
    [UnitKhoanAssignmentId] int NOT NULL,
    [KhoanPeriodId] int NOT NULL,
    [UnitId] int NOT NULL,
    [ScoringDate] datetime2 NOT NULL,
    [TotalScore] decimal(10,2) NOT NULL,
    [BaseScore] decimal(10,2) NOT NULL,
    [AdjustmentScore] decimal(10,2) NOT NULL,
    [Notes] nvarchar(1000) NULL,
    [ScoredBy] nvarchar(100) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_UnitKpiScorings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UnitKpiScorings_KhoanPeriods_KhoanPeriodId] FOREIGN KEY ([KhoanPeriodId]) REFERENCES [KhoanPeriods] ([Id]),
    CONSTRAINT [FK_UnitKpiScorings_UnitKhoanAssignments_UnitKhoanAssignmentId] FOREIGN KEY ([UnitKhoanAssignmentId]) REFERENCES [UnitKhoanAssignments] ([Id]),
    CONSTRAINT [FK_UnitKpiScorings_Units_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [Units] ([Id])
);
GO

CREATE TABLE [EmployeeKhoanAssignmentDetails] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeKhoanAssignmentId] int NOT NULL,
    [LegacyKPICode] nvarchar(max) NULL,
    [LegacyKPIName] nvarchar(max) NULL,
    [TargetValue] decimal(18,2) NOT NULL,
    [ActualValue] decimal(18,2) NULL,
    [Score] decimal(18,2) NULL,
    [Note] nvarchar(max) NULL,
    CONSTRAINT [PK_EmployeeKhoanAssignmentDetails] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EmployeeKhoanAssignmentDetails_EmployeeKhoanAssignments_EmployeeKhoanAssignmentId] FOREIGN KEY ([EmployeeKhoanAssignmentId]) REFERENCES [EmployeeKhoanAssignments] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [UnitKpiScoringCriterias] (
    [Id] int NOT NULL IDENTITY,
    [UnitKpiScoringId] int NOT NULL,
    [ViolationType] nvarchar(30) NOT NULL,
    [ViolationLevel] nvarchar(20) NOT NULL,
    [ViolationCount] int NOT NULL,
    [PenaltyScore] decimal(10,2) NOT NULL,
    [Description] nvarchar(500) NULL,
    [Notes] nvarchar(500) NULL,
    [ViolationDate] datetime2 NULL,
    CONSTRAINT [PK_UnitKpiScoringCriterias] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UnitKpiScoringCriterias_UnitKpiScorings_UnitKpiScoringId] FOREIGN KEY ([UnitKpiScoringId]) REFERENCES [UnitKpiScorings] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [UnitKpiScoringDetails] (
    [Id] int NOT NULL IDENTITY,
    [UnitKpiScoringId] int NOT NULL,
    [KpiDefinitionId] int NOT NULL,
    [KpiIndicatorId] int NULL,
    [IndicatorName] nvarchar(200) NOT NULL,
    [TargetValue] decimal(15,2) NOT NULL,
    [ActualValue] decimal(15,2) NULL,
    [CompletionRate] decimal(10,4) NULL,
    [BaseScore] decimal(10,2) NOT NULL,
    [AdjustmentScore] decimal(10,2) NOT NULL,
    [FinalScore] decimal(10,2) NOT NULL,
    [Score] decimal(10,2) NOT NULL,
    [IndicatorType] nvarchar(20) NOT NULL,
    [ScoringFormula] nvarchar(50) NULL,
    [Notes] nvarchar(500) NULL,
    CONSTRAINT [PK_UnitKpiScoringDetails] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UnitKpiScoringDetails_KPIDefinitions_KpiDefinitionId] FOREIGN KEY ([KpiDefinitionId]) REFERENCES [KPIDefinitions] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UnitKpiScoringDetails_KpiIndicators_KpiIndicatorId] FOREIGN KEY ([KpiIndicatorId]) REFERENCES [KpiIndicators] ([Id]),
    CONSTRAINT [FK_UnitKpiScoringDetails_UnitKpiScorings_UnitKpiScoringId] FOREIGN KEY ([UnitKpiScoringId]) REFERENCES [UnitKpiScorings] ([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_BusinessPlanTarget_Unique] ON [BusinessPlanTargets] ([DashboardIndicatorId], [UnitId], [Year], [Quarter], [Month]) WHERE [Quarter] IS NOT NULL AND [Month] IS NOT NULL;
GO

CREATE INDEX [IX_BusinessPlanTargets_UnitId] ON [BusinessPlanTargets] ([UnitId]);
GO

CREATE UNIQUE INDEX [IX_DashboardCalculation_Unique] ON [DashboardCalculations] ([DashboardIndicatorId], [UnitId], [CalculationDate]);
GO

CREATE INDEX [IX_DashboardCalculations_UnitId] ON [DashboardCalculations] ([UnitId]);
GO

CREATE UNIQUE INDEX [IX_DashboardIndicators_Code] ON [DashboardIndicators] ([Code]);
GO

CREATE INDEX [IX_DPDA_IsCurrent] ON [DPDA] ([IsCurrent]);
GO

CREATE INDEX [IX_DPDA_ProcessedDate] ON [DPDA] ([ProcessedDate]);
GO

CREATE INDEX [IX_DPDA_StatementDate] ON [DPDA] ([StatementDate]);
GO

CREATE INDEX [IX_EI01_IsCurrent] ON [EI01] ([IsCurrent]);
GO

CREATE INDEX [IX_EI01_ProcessedDate] ON [EI01] ([ProcessedDate]);
GO

CREATE INDEX [IX_EI01_StatementDate] ON [EI01] ([StatementDate]);
GO

CREATE INDEX [IX_EmployeeKhoanAssignmentDetails_EmployeeKhoanAssignmentId] ON [EmployeeKhoanAssignmentDetails] ([EmployeeKhoanAssignmentId]);
GO

CREATE INDEX [IX_EmployeeKhoanAssignments_EmployeeId] ON [EmployeeKhoanAssignments] ([EmployeeId]);
GO

CREATE INDEX [IX_EmployeeKhoanAssignments_KhoanPeriodId] ON [EmployeeKhoanAssignments] ([KhoanPeriodId]);
GO

CREATE INDEX [IX_EmployeeKpiAssignments_EmployeeId] ON [EmployeeKpiAssignments] ([EmployeeId]);
GO

CREATE INDEX [IX_EmployeeKpiAssignments_KhoanPeriodId] ON [EmployeeKpiAssignments] ([KhoanPeriodId]);
GO

CREATE INDEX [IX_EmployeeKpiAssignments_KpiDefinitionId] ON [EmployeeKpiAssignments] ([KpiDefinitionId]);
GO

CREATE INDEX [IX_EmployeeKpiTargets_EmployeeId] ON [EmployeeKpiTargets] ([EmployeeId]);
GO

CREATE INDEX [IX_EmployeeKpiTargets_IndicatorId] ON [EmployeeKpiTargets] ([IndicatorId]);
GO

CREATE INDEX [IX_EmployeeKpiTargets_KhoanPeriodId] ON [EmployeeKpiTargets] ([KhoanPeriodId]);
GO

CREATE INDEX [IX_EmployeeRoles_RoleId] ON [EmployeeRoles] ([RoleId]);
GO

CREATE INDEX [IX_Employees_PositionId] ON [Employees] ([PositionId]);
GO

CREATE INDEX [IX_Employees_UnitId] ON [Employees] ([UnitId]);
GO

CREATE INDEX [IX_FinalPayouts_EmployeeId] ON [FinalPayouts] ([EmployeeId]);
GO

CREATE INDEX [IX_FinalPayouts_KhoanPeriodId] ON [FinalPayouts] ([KhoanPeriodId]);
GO

CREATE INDEX [IX_ImportedDataItems_ProcessedDate] ON [ImportedDataItems] ([ProcessedDate]);
GO

CREATE INDEX [IX_ImportedDataItems_Record_Date] ON [ImportedDataItems] ([ImportedDataRecordId], [ProcessedDate]);
GO

CREATE INDEX [IX_ImportedDataItems_RecordId] ON [ImportedDataItems] ([ImportedDataRecordId]);
GO

CREATE INDEX [IX_ImportedDataRecords_Category_ImportDate] ON [ImportedDataRecords] ([Category], [ImportDate]);
GO

CREATE INDEX [IX_ImportedDataRecords_ImportDate] ON [ImportedDataRecords] ([ImportDate]);
GO

CREATE INDEX [IX_ImportedDataRecords_StatementDate] ON [ImportedDataRecords] ([StatementDate]);
GO

CREATE INDEX [IX_ImportedDataRecords_Status] ON [ImportedDataRecords] ([Status]);
GO

CREATE INDEX [IX_KH03_IsCurrent] ON [KH03] ([IsCurrent]);
GO

CREATE INDEX [IX_KH03_ProcessedDate] ON [KH03] ([ProcessedDate]);
GO

CREATE INDEX [IX_KH03_StatementDate] ON [KH03] ([StatementDate]);
GO

CREATE INDEX [IX_KpiIndicators_TableId] ON [KpiIndicators] ([TableId]);
GO

CREATE INDEX [IX_KpiScoringRules_IndicatorName] ON [KpiScoringRules] ([KpiIndicatorName]);
GO

CREATE INDEX [IX_MSIT72_TSBD_ImportedDataRecordId] ON [MSIT72_TSBD] ([ImportedDataRecordId]);
GO

CREATE INDEX [IX_MSIT72_TSGH_ImportedDataRecordId] ON [MSIT72_TSGH] ([ImportedDataRecordId]);
GO

CREATE INDEX [IX_RawDataRecords_RawDataImportId] ON [RawDataRecords] ([RawDataImportId]);
GO

CREATE INDEX [IX_ThuXLRR_ImportedDataRecordId] ON [ThuXLRR] ([ImportedDataRecordId]);
GO

CREATE INDEX [IX_UnitKhoanAssignmentDetails_UnitKhoanAssignmentId] ON [UnitKhoanAssignmentDetails] ([UnitKhoanAssignmentId]);
GO

CREATE INDEX [IX_UnitKhoanAssignments_KhoanPeriodId] ON [UnitKhoanAssignments] ([KhoanPeriodId]);
GO

CREATE INDEX [IX_UnitKhoanAssignments_UnitId] ON [UnitKhoanAssignments] ([UnitId]);
GO

CREATE INDEX [IX_UnitKpiScoringCriterias_UnitKpiScoringId] ON [UnitKpiScoringCriterias] ([UnitKpiScoringId]);
GO

CREATE INDEX [IX_UnitKpiScoringDetails_KpiDefinitionId] ON [UnitKpiScoringDetails] ([KpiDefinitionId]);
GO

CREATE INDEX [IX_UnitKpiScoringDetails_KpiIndicatorId] ON [UnitKpiScoringDetails] ([KpiIndicatorId]);
GO

CREATE INDEX [IX_UnitKpiScoringDetails_UnitKpiScoringId] ON [UnitKpiScoringDetails] ([UnitKpiScoringId]);
GO

CREATE INDEX [IX_UnitKpiScorings_KhoanPeriodId] ON [UnitKpiScorings] ([KhoanPeriodId]);
GO

CREATE INDEX [IX_UnitKpiScorings_UnitId] ON [UnitKpiScorings] ([UnitId]);
GO

CREATE INDEX [IX_UnitKpiScorings_UnitKhoanAssignmentId] ON [UnitKpiScorings] ([UnitKhoanAssignmentId]);
GO

CREATE INDEX [IX_Units_ParentUnitId] ON [Units] ([ParentUnitId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250701100846_InitialCreate', N'8.0.5');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250706111533_AddSortOrderToUnits', N'8.0.5');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [ImportedDataItems] SET (SYSTEM_VERSIONING = OFF)

GO

DROP TABLE [ImportedDataItems];
GO

DROP TABLE [ImportedDataItems_History];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250709153700_DropImportedDataItemsTable', N'8.0.5');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250712144236_SyncEmployeeIsActive', N'8.0.5');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Employees]') AND [c].[name] = N'IsActive');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Employees] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Employees] ADD DEFAULT CAST(1 AS bit) FOR [IsActive];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250712144738_AddIsActiveToEmployeeRoles', N'8.0.5');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250712154820_UpdateKpiIndicatorsSchema', N'8.0.5');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP INDEX [IX_RR01_MaCN] ON [RR01];
GO

DROP INDEX [IX_RR01_NGAY_DL_MaCN] ON [RR01];
GO

DROP INDEX [IX_GL01_MaCN] ON [GL01];
GO

DROP INDEX [IX_GL01_NGAY_DL_MaCN] ON [GL01];
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'CreatedAt');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [RR01] DROP COLUMN [CreatedAt];
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'MA_CN');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [RR01] DROP COLUMN [MA_CN];
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'StatementDate');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [RR01] DROP COLUMN [StatementDate];
GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'UpdatedAt');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [RR01] DROP COLUMN [UpdatedAt];
GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'CreatedAt');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [GL01] DROP COLUMN [CreatedAt];
GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'MA_CN');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [GL01] DROP COLUMN [MA_CN];
GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'StatementDate');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [GL01] DROP COLUMN [StatementDate];
GO

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'UpdatedAt');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [GL01] DROP COLUMN [UpdatedAt];
GO

DECLARE @var9 sysname;
SELECT @var9 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'CreatedAt');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var9 + '];');
ALTER TABLE [EI01] DROP COLUMN [CreatedAt];
GO

DECLARE @var10 sysname;
SELECT @var10 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'StatementDate');
IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var10 + '];');
ALTER TABLE [EI01] DROP COLUMN [StatementDate];
GO

DECLARE @var11 sysname;
SELECT @var11 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'UpdatedAt');
IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var11 + '];');
ALTER TABLE [EI01] DROP COLUMN [UpdatedAt];
GO

DECLARE @var12 sysname;
SELECT @var12 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'CreatedAt');
IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var12 + '];');
ALTER TABLE [DP01] DROP COLUMN [CreatedAt];
GO

DECLARE @var13 sysname;
SELECT @var13 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'StatementDate');
IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var13 + '];');
ALTER TABLE [DP01] DROP COLUMN [StatementDate];
GO

DECLARE @var14 sysname;
SELECT @var14 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'UpdatedAt');
IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var14 + '];');
ALTER TABLE [DP01] DROP COLUMN [UpdatedAt];
GO

EXEC sp_rename N'[RR01].[ImportedAt]', N'UPDATED_DATE', N'COLUMN';
GO

EXEC sp_rename N'[GL01].[ImportedAt]', N'UPDATED_DATE', N'COLUMN';
GO

EXEC sp_rename N'[EI01].[ImportedAt]', N'UPDATED_DATE', N'COLUMN';
GO

EXEC sp_rename N'[DP01].[ImportedAt]', N'UPDATED_DATE', N'COLUMN';
GO

DECLARE @var15 sysname;
SELECT @var15 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'VAMC_FLG');
IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var15 + '];');
UPDATE [RR01] SET [VAMC_FLG] = N'' WHERE [VAMC_FLG] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [VAMC_FLG] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [VAMC_FLG];
GO

DECLARE @var16 sysname;
SELECT @var16 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'TSK');
IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var16 + '];');
UPDATE [RR01] SET [TSK] = N'' WHERE [TSK] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [TSK] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [TSK];
GO

DECLARE @var17 sysname;
SELECT @var17 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'THU_LAI');
IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var17 + '];');
UPDATE [RR01] SET [THU_LAI] = N'' WHERE [THU_LAI] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [THU_LAI] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [THU_LAI];
GO

DECLARE @var18 sysname;
SELECT @var18 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'THU_GOC');
IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var18 + '];');
UPDATE [RR01] SET [THU_GOC] = N'' WHERE [THU_GOC] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [THU_GOC] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [THU_GOC];
GO

DECLARE @var19 sysname;
SELECT @var19 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'TEN_KH');
IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var19 + '];');
UPDATE [RR01] SET [TEN_KH] = N'' WHERE [TEN_KH] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [TEN_KH] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [TEN_KH];
GO

DECLARE @var20 sysname;
SELECT @var20 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'SO_LDS');
IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var20 + '];');
UPDATE [RR01] SET [SO_LDS] = N'' WHERE [SO_LDS] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [SO_LDS] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [SO_LDS];
GO

DECLARE @var21 sysname;
SELECT @var21 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'SO_LAV');
IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var21 + '];');
UPDATE [RR01] SET [SO_LAV] = N'' WHERE [SO_LAV] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [SO_LAV] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [SO_LAV];
GO

DECLARE @var22 sysname;
SELECT @var22 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'NGAY_XLRR');
IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var22 + '];');
UPDATE [RR01] SET [NGAY_XLRR] = N'' WHERE [NGAY_XLRR] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [NGAY_XLRR] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [NGAY_XLRR];
GO

DECLARE @var23 sysname;
SELECT @var23 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'NGAY_GIAI_NGAN');
IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var23 + '];');
UPDATE [RR01] SET [NGAY_GIAI_NGAN] = N'' WHERE [NGAY_GIAI_NGAN] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [NGAY_GIAI_NGAN] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [NGAY_GIAI_NGAN];
GO

DROP INDEX [IX_RR01_NGAY_DL] ON [RR01];
DECLARE @var24 sysname;
SELECT @var24 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'NGAY_DL');
IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var24 + '];');
UPDATE [RR01] SET [NGAY_DL] = N'' WHERE [NGAY_DL] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [NGAY_DL] nvarchar(10) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [NGAY_DL];
CREATE INDEX [IX_RR01_NGAY_DL] ON [RR01] ([NGAY_DL]);
GO

DECLARE @var25 sysname;
SELECT @var25 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'NGAY_DEN_HAN');
IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var25 + '];');
UPDATE [RR01] SET [NGAY_DEN_HAN] = N'' WHERE [NGAY_DEN_HAN] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [NGAY_DEN_HAN] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [NGAY_DEN_HAN];
GO

DECLARE @var26 sysname;
SELECT @var26 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'MA_KH');
IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var26 + '];');
UPDATE [RR01] SET [MA_KH] = N'' WHERE [MA_KH] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [MA_KH] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [MA_KH];
GO

DECLARE @var27 sysname;
SELECT @var27 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'LOAI_KH');
IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var27 + '];');
UPDATE [RR01] SET [LOAI_KH] = N'' WHERE [LOAI_KH] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [LOAI_KH] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [LOAI_KH];
GO

DECLARE @var28 sysname;
SELECT @var28 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'FILE_NAME');
IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var28 + '];');
UPDATE [RR01] SET [FILE_NAME] = N'' WHERE [FILE_NAME] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [FILE_NAME] nvarchar(255) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [FILE_NAME];
GO

DECLARE @var29 sysname;
SELECT @var29 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'DUNO_TRUNG_HAN');
IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var29 + '];');
UPDATE [RR01] SET [DUNO_TRUNG_HAN] = N'' WHERE [DUNO_TRUNG_HAN] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [DUNO_TRUNG_HAN] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [DUNO_TRUNG_HAN];
GO

DECLARE @var30 sysname;
SELECT @var30 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'DUNO_NGAN_HAN');
IF @var30 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var30 + '];');
UPDATE [RR01] SET [DUNO_NGAN_HAN] = N'' WHERE [DUNO_NGAN_HAN] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [DUNO_NGAN_HAN] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [DUNO_NGAN_HAN];
GO

DECLARE @var31 sysname;
SELECT @var31 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'DUNO_LAI_TICHLUY_BD');
IF @var31 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var31 + '];');
UPDATE [RR01] SET [DUNO_LAI_TICHLUY_BD] = N'' WHERE [DUNO_LAI_TICHLUY_BD] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [DUNO_LAI_TICHLUY_BD] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [DUNO_LAI_TICHLUY_BD];
GO

DECLARE @var32 sysname;
SELECT @var32 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'DUNO_LAI_HIENTAI');
IF @var32 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var32 + '];');
UPDATE [RR01] SET [DUNO_LAI_HIENTAI] = N'' WHERE [DUNO_LAI_HIENTAI] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [DUNO_LAI_HIENTAI] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [DUNO_LAI_HIENTAI];
GO

DECLARE @var33 sysname;
SELECT @var33 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'DUNO_GOC_HIENTAI');
IF @var33 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var33 + '];');
UPDATE [RR01] SET [DUNO_GOC_HIENTAI] = N'' WHERE [DUNO_GOC_HIENTAI] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [DUNO_GOC_HIENTAI] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [DUNO_GOC_HIENTAI];
GO

DECLARE @var34 sysname;
SELECT @var34 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'DUNO_GOC_BAN_DAU');
IF @var34 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var34 + '];');
UPDATE [RR01] SET [DUNO_GOC_BAN_DAU] = N'' WHERE [DUNO_GOC_BAN_DAU] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [DUNO_GOC_BAN_DAU] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [DUNO_GOC_BAN_DAU];
GO

DECLARE @var35 sysname;
SELECT @var35 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'DUNO_DAI_HAN');
IF @var35 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var35 + '];');
UPDATE [RR01] SET [DUNO_DAI_HAN] = N'' WHERE [DUNO_DAI_HAN] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [DUNO_DAI_HAN] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [DUNO_DAI_HAN];
GO

DECLARE @var36 sysname;
SELECT @var36 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'DS');
IF @var36 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var36 + '];');
UPDATE [RR01] SET [DS] = N'' WHERE [DS] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [DS] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [DS];
GO

DECLARE @var37 sysname;
SELECT @var37 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'DOC_DAUKY_DA_THU_HT');
IF @var37 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var37 + '];');
UPDATE [RR01] SET [DOC_DAUKY_DA_THU_HT] = N'' WHERE [DOC_DAUKY_DA_THU_HT] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [DOC_DAUKY_DA_THU_HT] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [DOC_DAUKY_DA_THU_HT];
GO

DECLARE @var38 sysname;
SELECT @var38 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'CN_LOAI_I');
IF @var38 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var38 + '];');
UPDATE [RR01] SET [CN_LOAI_I] = N'' WHERE [CN_LOAI_I] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [CN_LOAI_I] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [CN_LOAI_I];
GO

DECLARE @var39 sysname;
SELECT @var39 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'CCY');
IF @var39 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var39 + '];');
UPDATE [RR01] SET [CCY] = N'' WHERE [CCY] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [CCY] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [CCY];
GO

DECLARE @var40 sysname;
SELECT @var40 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'BRCD');
IF @var40 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var40 + '];');
UPDATE [RR01] SET [BRCD] = N'' WHERE [BRCD] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [BRCD] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [BRCD];
GO

DECLARE @var41 sysname;
SELECT @var41 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'BDS');
IF @var41 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var41 + '];');
UPDATE [RR01] SET [BDS] = N'' WHERE [BDS] IS NULL;
ALTER TABLE [RR01] ALTER COLUMN [BDS] nvarchar(50) NOT NULL;
ALTER TABLE [RR01] ADD DEFAULT N'' FOR [BDS];
GO

DECLARE @var42 sysname;
SELECT @var42 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'Id');
IF @var42 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var42 + '];');
ALTER TABLE [RR01] ALTER COLUMN [Id] int NOT NULL;
GO

DECLARE @var43 sysname;
SELECT @var43 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RR01]') AND [c].[name] = N'UPDATED_DATE');
IF @var43 IS NOT NULL EXEC(N'ALTER TABLE [RR01] DROP CONSTRAINT [' + @var43 + '];');
ALTER TABLE [RR01] ALTER COLUMN [UPDATED_DATE] datetime2 NOT NULL;
GO

ALTER TABLE [RR01] ADD [CREATED_DATE] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

DECLARE @var44 sysname;
SELECT @var44 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN03]') AND [c].[name] = N'UPDATED_DATE');
IF @var44 IS NOT NULL EXEC(N'ALTER TABLE [LN03] DROP CONSTRAINT [' + @var44 + '];');
ALTER TABLE [LN03] ALTER COLUMN [UPDATED_DATE] datetime2 NOT NULL;
GO

DECLARE @var45 sysname;
SELECT @var45 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN03]') AND [c].[name] = N'THUNOSAUXL');
IF @var45 IS NOT NULL EXEC(N'ALTER TABLE [LN03] DROP CONSTRAINT [' + @var45 + '];');
UPDATE [LN03] SET [THUNOSAUXL] = N'' WHERE [THUNOSAUXL] IS NULL;
ALTER TABLE [LN03] ALTER COLUMN [THUNOSAUXL] nvarchar(50) NOT NULL;
ALTER TABLE [LN03] ADD DEFAULT N'' FOR [THUNOSAUXL];
GO

DECLARE @var46 sysname;
SELECT @var46 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN03]') AND [c].[name] = N'TENKH');
IF @var46 IS NOT NULL EXEC(N'ALTER TABLE [LN03] DROP CONSTRAINT [' + @var46 + '];');
UPDATE [LN03] SET [TENKH] = N'' WHERE [TENKH] IS NULL;
ALTER TABLE [LN03] ALTER COLUMN [TENKH] nvarchar(50) NOT NULL;
ALTER TABLE [LN03] ADD DEFAULT N'' FOR [TENKH];
GO

DECLARE @var47 sysname;
SELECT @var47 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN03]') AND [c].[name] = N'TENCHINHANH');
IF @var47 IS NOT NULL EXEC(N'ALTER TABLE [LN03] DROP CONSTRAINT [' + @var47 + '];');
UPDATE [LN03] SET [TENCHINHANH] = N'' WHERE [TENCHINHANH] IS NULL;
ALTER TABLE [LN03] ALTER COLUMN [TENCHINHANH] nvarchar(50) NOT NULL;
ALTER TABLE [LN03] ADD DEFAULT N'' FOR [TENCHINHANH];
GO

DECLARE @var48 sysname;
SELECT @var48 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN03]') AND [c].[name] = N'TENCBTD');
IF @var48 IS NOT NULL EXEC(N'ALTER TABLE [LN03] DROP CONSTRAINT [' + @var48 + '];');
UPDATE [LN03] SET [TENCBTD] = N'' WHERE [TENCBTD] IS NULL;
ALTER TABLE [LN03] ALTER COLUMN [TENCBTD] nvarchar(50) NOT NULL;
ALTER TABLE [LN03] ADD DEFAULT N'' FOR [TENCBTD];
GO

DECLARE @var49 sysname;
SELECT @var49 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN03]') AND [c].[name] = N'TAIKHOANHACHTOAN');
IF @var49 IS NOT NULL EXEC(N'ALTER TABLE [LN03] DROP CONSTRAINT [' + @var49 + '];');
UPDATE [LN03] SET [TAIKHOANHACHTOAN] = N'' WHERE [TAIKHOANHACHTOAN] IS NULL;
ALTER TABLE [LN03] ALTER COLUMN [TAIKHOANHACHTOAN] nvarchar(50) NOT NULL;
ALTER TABLE [LN03] ADD DEFAULT N'' FOR [TAIKHOANHACHTOAN];
GO

DECLARE @var50 sysname;
SELECT @var50 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN03]') AND [c].[name] = N'SOTIENXLRR');
IF @var50 IS NOT NULL EXEC(N'ALTER TABLE [LN03] DROP CONSTRAINT [' + @var50 + '];');
UPDATE [LN03] SET [SOTIENXLRR] = N'' WHERE [SOTIENXLRR] IS NULL;
ALTER TABLE [LN03] ALTER COLUMN [SOTIENXLRR] nvarchar(50) NOT NULL;
ALTER TABLE [LN03] ADD DEFAULT N'' FOR [SOTIENXLRR];
GO

DECLARE @var51 sysname;
SELECT @var51 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN03]') AND [c].[name] = N'SOHOPDONG');
IF @var51 IS NOT NULL EXEC(N'ALTER TABLE [LN03] DROP CONSTRAINT [' + @var51 + '];');
UPDATE [LN03] SET [SOHOPDONG] = N'' WHERE [SOHOPDONG] IS NULL;
ALTER TABLE [LN03] ALTER COLUMN [SOHOPDONG] nvarchar(50) NOT NULL;
ALTER TABLE [LN03] ADD DEFAULT N'' FOR [SOHOPDONG];
GO

DECLARE @var52 sysname;
SELECT @var52 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN03]') AND [c].[name] = N'REFNO');
IF @var52 IS NOT NULL EXEC(N'ALTER TABLE [LN03] DROP CONSTRAINT [' + @var52 + '];');
UPDATE [LN03] SET [REFNO] = N'' WHERE [REFNO] IS NULL;
ALTER TABLE [LN03] ALTER COLUMN [REFNO] nvarchar(50) NOT NULL;
ALTER TABLE [LN03] ADD DEFAULT N'' FOR [REFNO];
GO

DECLARE @var53 sysname;
SELECT @var53 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN03]') AND [c].[name] = N'NHOMNO');
IF @var53 IS NOT NULL EXEC(N'ALTER TABLE [LN03] DROP CONSTRAINT [' + @var53 + '];');
UPDATE [LN03] SET [NHOMNO] = N'' WHERE [NHOMNO] IS NULL;
ALTER TABLE [LN03] ALTER COLUMN [NHOMNO] nvarchar(50) NOT NULL;
ALTER TABLE [LN03] ADD DEFAULT N'' FOR [NHOMNO];
GO

DROP INDEX [IX_LN03_NGAY_DL] ON [LN03];
DECLARE @var54 sysname;
SELECT @var54 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN03]') AND [c].[name] = N'NGAY_DL');
IF @var54 IS NOT NULL EXEC(N'ALTER TABLE [LN03] DROP CONSTRAINT [' + @var54 + '];');
ALTER TABLE [LN03] ALTER COLUMN [NGAY_DL] nvarchar(10) NOT NULL;
CREATE INDEX [IX_LN03_NGAY_DL] ON [LN03] ([NGAY_DL]);
GO

DECLARE @var55 sysname;
SELECT @var55 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN03]') AND [c].[name] = N'NGAYPHATSINHXL');
IF @var55 IS NOT NULL EXEC(N'ALTER TABLE [LN03] DROP CONSTRAINT [' + @var55 + '];');
UPDATE [LN03] SET [NGAYPHATSINHXL] = N'' WHERE [NGAYPHATSINHXL] IS NULL;
ALTER TABLE [LN03] ALTER COLUMN [NGAYPHATSINHXL] nvarchar(50) NOT NULL;
ALTER TABLE [LN03] ADD DEFAULT N'' FOR [NGAYPHATSINHXL];
GO

DECLARE @var56 sysname;
SELECT @var56 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN03]') AND [c].[name] = N'MAPGD');
IF @var56 IS NOT NULL EXEC(N'ALTER TABLE [LN03] DROP CONSTRAINT [' + @var56 + '];');
UPDATE [LN03] SET [MAPGD] = N'' WHERE [MAPGD] IS NULL;
ALTER TABLE [LN03] ALTER COLUMN [MAPGD] nvarchar(50) NOT NULL;
ALTER TABLE [LN03] ADD DEFAULT N'' FOR [MAPGD];
GO

DECLARE @var57 sysname;
SELECT @var57 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN03]') AND [c].[name] = N'MAKH');
IF @var57 IS NOT NULL EXEC(N'ALTER TABLE [LN03] DROP CONSTRAINT [' + @var57 + '];');
UPDATE [LN03] SET [MAKH] = N'' WHERE [MAKH] IS NULL;
ALTER TABLE [LN03] ALTER COLUMN [MAKH] nvarchar(50) NOT NULL;
ALTER TABLE [LN03] ADD DEFAULT N'' FOR [MAKH];
GO

DECLARE @var58 sysname;
SELECT @var58 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN03]') AND [c].[name] = N'MACHINHANH');
IF @var58 IS NOT NULL EXEC(N'ALTER TABLE [LN03] DROP CONSTRAINT [' + @var58 + '];');
UPDATE [LN03] SET [MACHINHANH] = N'' WHERE [MACHINHANH] IS NULL;
ALTER TABLE [LN03] ALTER COLUMN [MACHINHANH] nvarchar(50) NOT NULL;
ALTER TABLE [LN03] ADD DEFAULT N'' FOR [MACHINHANH];
GO

DECLARE @var59 sysname;
SELECT @var59 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN03]') AND [c].[name] = N'MACBTD');
IF @var59 IS NOT NULL EXEC(N'ALTER TABLE [LN03] DROP CONSTRAINT [' + @var59 + '];');
UPDATE [LN03] SET [MACBTD] = N'' WHERE [MACBTD] IS NULL;
ALTER TABLE [LN03] ALTER COLUMN [MACBTD] nvarchar(50) NOT NULL;
ALTER TABLE [LN03] ADD DEFAULT N'' FOR [MACBTD];
GO

DECLARE @var60 sysname;
SELECT @var60 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN03]') AND [c].[name] = N'LOAINGUONVON');
IF @var60 IS NOT NULL EXEC(N'ALTER TABLE [LN03] DROP CONSTRAINT [' + @var60 + '];');
UPDATE [LN03] SET [LOAINGUONVON] = N'' WHERE [LOAINGUONVON] IS NULL;
ALTER TABLE [LN03] ALTER COLUMN [LOAINGUONVON] nvarchar(50) NOT NULL;
ALTER TABLE [LN03] ADD DEFAULT N'' FOR [LOAINGUONVON];
GO

DECLARE @var61 sysname;
SELECT @var61 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN03]') AND [c].[name] = N'FILE_NAME');
IF @var61 IS NOT NULL EXEC(N'ALTER TABLE [LN03] DROP CONSTRAINT [' + @var61 + '];');
UPDATE [LN03] SET [FILE_NAME] = N'' WHERE [FILE_NAME] IS NULL;
ALTER TABLE [LN03] ALTER COLUMN [FILE_NAME] nvarchar(255) NOT NULL;
ALTER TABLE [LN03] ADD DEFAULT N'' FOR [FILE_NAME];
GO

DECLARE @var62 sysname;
SELECT @var62 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN03]') AND [c].[name] = N'DUNONOIBANG');
IF @var62 IS NOT NULL EXEC(N'ALTER TABLE [LN03] DROP CONSTRAINT [' + @var62 + '];');
UPDATE [LN03] SET [DUNONOIBANG] = N'' WHERE [DUNONOIBANG] IS NULL;
ALTER TABLE [LN03] ALTER COLUMN [DUNONOIBANG] nvarchar(50) NOT NULL;
ALTER TABLE [LN03] ADD DEFAULT N'' FOR [DUNONOIBANG];
GO

DECLARE @var63 sysname;
SELECT @var63 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN03]') AND [c].[name] = N'CREATED_DATE');
IF @var63 IS NOT NULL EXEC(N'ALTER TABLE [LN03] DROP CONSTRAINT [' + @var63 + '];');
ALTER TABLE [LN03] ALTER COLUMN [CREATED_DATE] datetime2 NOT NULL;
GO

DECLARE @var64 sysname;
SELECT @var64 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN03]') AND [c].[name] = N'CONLAINGOAIBANG');
IF @var64 IS NOT NULL EXEC(N'ALTER TABLE [LN03] DROP CONSTRAINT [' + @var64 + '];');
UPDATE [LN03] SET [CONLAINGOAIBANG] = N'' WHERE [CONLAINGOAIBANG] IS NULL;
ALTER TABLE [LN03] ALTER COLUMN [CONLAINGOAIBANG] nvarchar(50) NOT NULL;
ALTER TABLE [LN03] ADD DEFAULT N'' FOR [CONLAINGOAIBANG];
GO

DECLARE @var65 sysname;
SELECT @var65 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN03]') AND [c].[name] = N'Id');
IF @var65 IS NOT NULL EXEC(N'ALTER TABLE [LN03] DROP CONSTRAINT [' + @var65 + '];');
ALTER TABLE [LN03] ALTER COLUMN [Id] int NOT NULL;
GO

DECLARE @var66 sysname;
SELECT @var66 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'YRDAYS');
IF @var66 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var66 + '];');
UPDATE [LN01] SET [YRDAYS] = N'' WHERE [YRDAYS] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [YRDAYS] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [YRDAYS];
GO

DECLARE @var67 sysname;
SELECT @var67 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'USRIDOP');
IF @var67 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var67 + '];');
UPDATE [LN01] SET [USRIDOP] = N'' WHERE [USRIDOP] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [USRIDOP] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [USRIDOP];
GO

DECLARE @var68 sysname;
SELECT @var68 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'UPDATED_DATE');
IF @var68 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var68 + '];');
ALTER TABLE [LN01] ALTER COLUMN [UPDATED_DATE] datetime2 NOT NULL;
GO

DECLARE @var69 sysname;
SELECT @var69 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'TY_GIA');
IF @var69 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var69 + '];');
UPDATE [LN01] SET [TY_GIA] = N'' WHERE [TY_GIA] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [TY_GIA] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [TY_GIA];
GO

DECLARE @var70 sysname;
SELECT @var70 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'TRCTNM');
IF @var70 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var70 + '];');
UPDATE [LN01] SET [TRCTNM] = N'' WHERE [TRCTNM] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [TRCTNM] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [TRCTNM];
GO

DECLARE @var71 sysname;
SELECT @var71 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'TRCTCD');
IF @var71 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var71 + '];');
UPDATE [LN01] SET [TRCTCD] = N'' WHERE [TRCTCD] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [TRCTCD] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [TRCTCD];
GO

DECLARE @var72 sysname;
SELECT @var72 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'TRANSACTION_DATE');
IF @var72 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var72 + '];');
UPDATE [LN01] SET [TRANSACTION_DATE] = N'' WHERE [TRANSACTION_DATE] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [TRANSACTION_DATE] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [TRANSACTION_DATE];
GO

DECLARE @var73 sysname;
SELECT @var73 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'TOTAL_INTEREST_REPAY_AMOUNT');
IF @var73 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var73 + '];');
UPDATE [LN01] SET [TOTAL_INTEREST_REPAY_AMOUNT] = N'' WHERE [TOTAL_INTEREST_REPAY_AMOUNT] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [TOTAL_INTEREST_REPAY_AMOUNT] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [TOTAL_INTEREST_REPAY_AMOUNT];
GO

DECLARE @var74 sysname;
SELECT @var74 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'TAI_KHOAN_GIAI_NGAN_2');
IF @var74 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var74 + '];');
UPDATE [LN01] SET [TAI_KHOAN_GIAI_NGAN_2] = N'' WHERE [TAI_KHOAN_GIAI_NGAN_2] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [TAI_KHOAN_GIAI_NGAN_2] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [TAI_KHOAN_GIAI_NGAN_2];
GO

DECLARE @var75 sysname;
SELECT @var75 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'TAI_KHOAN_GIAI_NGAN_1');
IF @var75 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var75 + '];');
UPDATE [LN01] SET [TAI_KHOAN_GIAI_NGAN_1] = N'' WHERE [TAI_KHOAN_GIAI_NGAN_1] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [TAI_KHOAN_GIAI_NGAN_1] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [TAI_KHOAN_GIAI_NGAN_1];
GO

DECLARE @var76 sysname;
SELECT @var76 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'TAI_KHOAN');
IF @var76 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var76 + '];');
UPDATE [LN01] SET [TAI_KHOAN] = N'' WHERE [TAI_KHOAN] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [TAI_KHOAN] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [TAI_KHOAN];
GO

DECLARE @var77 sysname;
SELECT @var77 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'SO_TIEN_GIAI_NGAN_2');
IF @var77 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var77 + '];');
UPDATE [LN01] SET [SO_TIEN_GIAI_NGAN_2] = N'' WHERE [SO_TIEN_GIAI_NGAN_2] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [SO_TIEN_GIAI_NGAN_2] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [SO_TIEN_GIAI_NGAN_2];
GO

DECLARE @var78 sysname;
SELECT @var78 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'SO_TIEN_GIAI_NGAN_1');
IF @var78 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var78 + '];');
UPDATE [LN01] SET [SO_TIEN_GIAI_NGAN_1] = N'' WHERE [SO_TIEN_GIAI_NGAN_1] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [SO_TIEN_GIAI_NGAN_1] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [SO_TIEN_GIAI_NGAN_1];
GO

DECLARE @var79 sysname;
SELECT @var79 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'SECURED_PERCENT');
IF @var79 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var79 + '];');
UPDATE [LN01] SET [SECURED_PERCENT] = N'' WHERE [SECURED_PERCENT] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [SECURED_PERCENT] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [SECURED_PERCENT];
GO

DECLARE @var80 sysname;
SELECT @var80 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'REPAYMENT_AMOUNT');
IF @var80 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var80 + '];');
UPDATE [LN01] SET [REPAYMENT_AMOUNT] = N'' WHERE [REPAYMENT_AMOUNT] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [REPAYMENT_AMOUNT] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [REPAYMENT_AMOUNT];
GO

DECLARE @var81 sysname;
SELECT @var81 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'REMARK');
IF @var81 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var81 + '];');
UPDATE [LN01] SET [REMARK] = N'' WHERE [REMARK] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [REMARK] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [REMARK];
GO

DECLARE @var82 sysname;
SELECT @var82 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'PROVINCE');
IF @var82 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var82 + '];');
UPDATE [LN01] SET [PROVINCE] = N'' WHERE [PROVINCE] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [PROVINCE] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [PROVINCE];
GO

DECLARE @var83 sysname;
SELECT @var83 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'PHUONG_THUC_GIAI_NGAN_2');
IF @var83 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var83 + '];');
UPDATE [LN01] SET [PHUONG_THUC_GIAI_NGAN_2] = N'' WHERE [PHUONG_THUC_GIAI_NGAN_2] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [PHUONG_THUC_GIAI_NGAN_2] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [PHUONG_THUC_GIAI_NGAN_2];
GO

DECLARE @var84 sysname;
SELECT @var84 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'PHUONG_THUC_GIAI_NGAN_1');
IF @var84 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var84 + '];');
UPDATE [LN01] SET [PHUONG_THUC_GIAI_NGAN_1] = N'' WHERE [PHUONG_THUC_GIAI_NGAN_1] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [PHUONG_THUC_GIAI_NGAN_1] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [PHUONG_THUC_GIAI_NGAN_1];
GO

DECLARE @var85 sysname;
SELECT @var85 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'PASTDUE_INTEREST_AMOUNT');
IF @var85 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var85 + '];');
UPDATE [LN01] SET [PASTDUE_INTEREST_AMOUNT] = N'' WHERE [PASTDUE_INTEREST_AMOUNT] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [PASTDUE_INTEREST_AMOUNT] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [PASTDUE_INTEREST_AMOUNT];
GO

DECLARE @var86 sysname;
SELECT @var86 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'OFFICER_NAME');
IF @var86 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var86 + '];');
UPDATE [LN01] SET [OFFICER_NAME] = N'' WHERE [OFFICER_NAME] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [OFFICER_NAME] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [OFFICER_NAME];
GO

DECLARE @var87 sysname;
SELECT @var87 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'OFFICER_IPCAS');
IF @var87 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var87 + '];');
UPDATE [LN01] SET [OFFICER_IPCAS] = N'' WHERE [OFFICER_IPCAS] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [OFFICER_IPCAS] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [OFFICER_IPCAS];
GO

DECLARE @var88 sysname;
SELECT @var88 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'OFFICER_ID');
IF @var88 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var88 + '];');
UPDATE [LN01] SET [OFFICER_ID] = N'' WHERE [OFFICER_ID] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [OFFICER_ID] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [OFFICER_ID];
GO

DECLARE @var89 sysname;
SELECT @var89 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'NHOM_NO');
IF @var89 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var89 + '];');
UPDATE [LN01] SET [NHOM_NO] = N'' WHERE [NHOM_NO] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [NHOM_NO] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [NHOM_NO];
GO

DECLARE @var90 sysname;
SELECT @var90 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'NGAY_SINH');
IF @var90 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var90 + '];');
UPDATE [LN01] SET [NGAY_SINH] = N'' WHERE [NGAY_SINH] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [NGAY_SINH] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [NGAY_SINH];
GO

DROP INDEX [IX_LN01_NGAY_DL] ON [LN01];
DECLARE @var91 sysname;
SELECT @var91 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'NGAY_DL');
IF @var91 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var91 + '];');
ALTER TABLE [LN01] ALTER COLUMN [NGAY_DL] nvarchar(10) NOT NULL;
CREATE INDEX [IX_LN01_NGAY_DL] ON [LN01] ([NGAY_DL]);
GO

DECLARE @var92 sysname;
SELECT @var92 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'NEXT_REPAY_DATE');
IF @var92 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var92 + '];');
UPDATE [LN01] SET [NEXT_REPAY_DATE] = N'' WHERE [NEXT_REPAY_DATE] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [NEXT_REPAY_DATE] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [NEXT_REPAY_DATE];
GO

DECLARE @var93 sysname;
SELECT @var93 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'NEXT_REPAY_AMOUNT');
IF @var93 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var93 + '];');
UPDATE [LN01] SET [NEXT_REPAY_AMOUNT] = N'' WHERE [NEXT_REPAY_AMOUNT] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [NEXT_REPAY_AMOUNT] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [NEXT_REPAY_AMOUNT];
GO

DECLARE @var94 sysname;
SELECT @var94 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'NEXT_INT_REPAY_DATE');
IF @var94 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var94 + '];');
UPDATE [LN01] SET [NEXT_INT_REPAY_DATE] = N'' WHERE [NEXT_INT_REPAY_DATE] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [NEXT_INT_REPAY_DATE] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [NEXT_INT_REPAY_DATE];
GO

DECLARE @var95 sysname;
SELECT @var95 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'MA_NGANH_KT');
IF @var95 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var95 + '];');
UPDATE [LN01] SET [MA_NGANH_KT] = N'' WHERE [MA_NGANH_KT] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [MA_NGANH_KT] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [MA_NGANH_KT];
GO

DECLARE @var96 sysname;
SELECT @var96 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'MA_CB_AGRI');
IF @var96 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var96 + '];');
UPDATE [LN01] SET [MA_CB_AGRI] = N'' WHERE [MA_CB_AGRI] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [MA_CB_AGRI] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [MA_CB_AGRI];
GO

DECLARE @var97 sysname;
SELECT @var97 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'LOAN_TYPE');
IF @var97 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var97 + '];');
UPDATE [LN01] SET [LOAN_TYPE] = N'' WHERE [LOAN_TYPE] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [LOAN_TYPE] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [LOAN_TYPE];
GO

DECLARE @var98 sysname;
SELECT @var98 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'LCLWARDNM');
IF @var98 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var98 + '];');
UPDATE [LN01] SET [LCLWARDNM] = N'' WHERE [LCLWARDNM] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [LCLWARDNM] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [LCLWARDNM];
GO

DECLARE @var99 sysname;
SELECT @var99 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'LCLPROVINNM');
IF @var99 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var99 + '];');
UPDATE [LN01] SET [LCLPROVINNM] = N'' WHERE [LCLPROVINNM] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [LCLPROVINNM] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [LCLPROVINNM];
GO

DECLARE @var100 sysname;
SELECT @var100 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'LCLDISTNM');
IF @var100 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var100 + '];');
UPDATE [LN01] SET [LCLDISTNM] = N'' WHERE [LCLDISTNM] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [LCLDISTNM] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [LCLDISTNM];
GO

DECLARE @var101 sysname;
SELECT @var101 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'LAST_REPAY_DATE');
IF @var101 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var101 + '];');
UPDATE [LN01] SET [LAST_REPAY_DATE] = N'' WHERE [LAST_REPAY_DATE] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [LAST_REPAY_DATE] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [LAST_REPAY_DATE];
GO

DECLARE @var102 sysname;
SELECT @var102 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'LAST_INT_CHARGE_DATE');
IF @var102 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var102 + '];');
UPDATE [LN01] SET [LAST_INT_CHARGE_DATE] = N'' WHERE [LAST_INT_CHARGE_DATE] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [LAST_INT_CHARGE_DATE] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [LAST_INT_CHARGE_DATE];
GO

DECLARE @var103 sysname;
SELECT @var103 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'INT_PAYMENT_INTERVAL');
IF @var103 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var103 + '];');
UPDATE [LN01] SET [INT_PAYMENT_INTERVAL] = N'' WHERE [INT_PAYMENT_INTERVAL] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [INT_PAYMENT_INTERVAL] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [INT_PAYMENT_INTERVAL];
GO

DECLARE @var104 sysname;
SELECT @var104 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'INT_PARTIAL_PAYMENT_TYPE');
IF @var104 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var104 + '];');
UPDATE [LN01] SET [INT_PARTIAL_PAYMENT_TYPE] = N'' WHERE [INT_PARTIAL_PAYMENT_TYPE] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [INT_PARTIAL_PAYMENT_TYPE] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [INT_PARTIAL_PAYMENT_TYPE];
GO

DECLARE @var105 sysname;
SELECT @var105 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'INT_LUMPSUM_PARTIAL_TYPE');
IF @var105 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var105 + '];');
UPDATE [LN01] SET [INT_LUMPSUM_PARTIAL_TYPE] = N'' WHERE [INT_LUMPSUM_PARTIAL_TYPE] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [INT_LUMPSUM_PARTIAL_TYPE] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [INT_LUMPSUM_PARTIAL_TYPE];
GO

DECLARE @var106 sysname;
SELECT @var106 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'INTTRMMTH');
IF @var106 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var106 + '];');
UPDATE [LN01] SET [INTTRMMTH] = N'' WHERE [INTTRMMTH] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [INTTRMMTH] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [INTTRMMTH];
GO

DECLARE @var107 sysname;
SELECT @var107 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'INTRPYMTH');
IF @var107 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var107 + '];');
UPDATE [LN01] SET [INTRPYMTH] = N'' WHERE [INTRPYMTH] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [INTRPYMTH] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [INTRPYMTH];
GO

DECLARE @var108 sysname;
SELECT @var108 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'INTEREST_RATE');
IF @var108 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var108 + '];');
UPDATE [LN01] SET [INTEREST_RATE] = N'' WHERE [INTEREST_RATE] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [INTEREST_RATE] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [INTEREST_RATE];
GO

DECLARE @var109 sysname;
SELECT @var109 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'INTEREST_AMOUNT');
IF @var109 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var109 + '];');
UPDATE [LN01] SET [INTEREST_AMOUNT] = N'' WHERE [INTEREST_AMOUNT] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [INTEREST_AMOUNT] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [INTEREST_AMOUNT];
GO

DECLARE @var110 sysname;
SELECT @var110 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'INTCMTH');
IF @var110 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var110 + '];');
UPDATE [LN01] SET [INTCMTH] = N'' WHERE [INTCMTH] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [INTCMTH] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [INTCMTH];
GO

DECLARE @var111 sysname;
SELECT @var111 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'GRPNO');
IF @var111 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var111 + '];');
UPDATE [LN01] SET [GRPNO] = N'' WHERE [GRPNO] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [GRPNO] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [GRPNO];
GO

DECLARE @var112 sysname;
SELECT @var112 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'FUND_RESOURCE_CODE');
IF @var112 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var112 + '];');
UPDATE [LN01] SET [FUND_RESOURCE_CODE] = N'' WHERE [FUND_RESOURCE_CODE] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [FUND_RESOURCE_CODE] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [FUND_RESOURCE_CODE];
GO

DECLARE @var113 sysname;
SELECT @var113 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'FUND_PURPOSE_CODE');
IF @var113 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var113 + '];');
UPDATE [LN01] SET [FUND_PURPOSE_CODE] = N'' WHERE [FUND_PURPOSE_CODE] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [FUND_PURPOSE_CODE] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [FUND_PURPOSE_CODE];
GO

DECLARE @var114 sysname;
SELECT @var114 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'FILE_NAME');
IF @var114 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var114 + '];');
UPDATE [LN01] SET [FILE_NAME] = N'' WHERE [FILE_NAME] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [FILE_NAME] nvarchar(255) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [FILE_NAME];
GO

DECLARE @var115 sysname;
SELECT @var115 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'EXEMPTINTTYPE');
IF @var115 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var115 + '];');
UPDATE [LN01] SET [EXEMPTINTTYPE] = N'' WHERE [EXEMPTINTTYPE] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [EXEMPTINTTYPE] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [EXEMPTINTTYPE];
GO

DECLARE @var116 sysname;
SELECT @var116 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'EXEMPTINTAMT');
IF @var116 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var116 + '];');
UPDATE [LN01] SET [EXEMPTINTAMT] = N'' WHERE [EXEMPTINTAMT] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [EXEMPTINTAMT] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [EXEMPTINTAMT];
GO

DECLARE @var117 sysname;
SELECT @var117 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'EXEMPTINT');
IF @var117 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var117 + '];');
UPDATE [LN01] SET [EXEMPTINT] = N'' WHERE [EXEMPTINT] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [EXEMPTINT] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [EXEMPTINT];
GO

DECLARE @var118 sysname;
SELECT @var118 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'DU_NO');
IF @var118 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var118 + '];');
UPDATE [LN01] SET [DU_NO] = N'' WHERE [DU_NO] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [DU_NO] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [DU_NO];
GO

DECLARE @var119 sysname;
SELECT @var119 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'DSBSSEQ');
IF @var119 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var119 + '];');
UPDATE [LN01] SET [DSBSSEQ] = N'' WHERE [DSBSSEQ] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [DSBSSEQ] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [DSBSSEQ];
GO

DECLARE @var120 sysname;
SELECT @var120 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'DSBSMATDT');
IF @var120 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var120 + '];');
UPDATE [LN01] SET [DSBSMATDT] = N'' WHERE [DSBSMATDT] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [DSBSMATDT] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [DSBSMATDT];
GO

DECLARE @var121 sysname;
SELECT @var121 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'DSBSDT');
IF @var121 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var121 + '];');
UPDATE [LN01] SET [DSBSDT] = N'' WHERE [DSBSDT] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [DSBSDT] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [DSBSDT];
GO

DECLARE @var122 sysname;
SELECT @var122 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'DISTRICT');
IF @var122 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var122 + '];');
UPDATE [LN01] SET [DISTRICT] = N'' WHERE [DISTRICT] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [DISTRICT] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [DISTRICT];
GO

DECLARE @var123 sysname;
SELECT @var123 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'DISBUR_CCY');
IF @var123 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var123 + '];');
UPDATE [LN01] SET [DISBUR_CCY] = N'' WHERE [DISBUR_CCY] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [DISBUR_CCY] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [DISBUR_CCY];
GO

DECLARE @var124 sysname;
SELECT @var124 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'DISBURSEMENT_AMOUNT');
IF @var124 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var124 + '];');
UPDATE [LN01] SET [DISBURSEMENT_AMOUNT] = N'' WHERE [DISBURSEMENT_AMOUNT] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [DISBURSEMENT_AMOUNT] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [DISBURSEMENT_AMOUNT];
GO

DECLARE @var125 sysname;
SELECT @var125 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'CUSTSEQ');
IF @var125 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var125 + '];');
UPDATE [LN01] SET [CUSTSEQ] = N'' WHERE [CUSTSEQ] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [CUSTSEQ] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [CUSTSEQ];
GO

DECLARE @var126 sysname;
SELECT @var126 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'CUSTOMER_TYPE_CODE_DETAIL');
IF @var126 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var126 + '];');
UPDATE [LN01] SET [CUSTOMER_TYPE_CODE_DETAIL] = N'' WHERE [CUSTOMER_TYPE_CODE_DETAIL] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [CUSTOMER_TYPE_CODE_DETAIL] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [CUSTOMER_TYPE_CODE_DETAIL];
GO

DECLARE @var127 sysname;
SELECT @var127 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'CUSTOMER_TYPE_CODE');
IF @var127 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var127 + '];');
UPDATE [LN01] SET [CUSTOMER_TYPE_CODE] = N'' WHERE [CUSTOMER_TYPE_CODE] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [CUSTOMER_TYPE_CODE] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [CUSTOMER_TYPE_CODE];
GO

DECLARE @var128 sysname;
SELECT @var128 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'CUSTNM');
IF @var128 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var128 + '];');
UPDATE [LN01] SET [CUSTNM] = N'' WHERE [CUSTNM] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [CUSTNM] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [CUSTNM];
GO

DECLARE @var129 sysname;
SELECT @var129 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'CTCV');
IF @var129 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var129 + '];');
UPDATE [LN01] SET [CTCV] = N'' WHERE [CTCV] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [CTCV] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [CTCV];
GO

DECLARE @var130 sysname;
SELECT @var130 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'CREDIT_LINE_YPE');
IF @var130 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var130 + '];');
UPDATE [LN01] SET [CREDIT_LINE_YPE] = N'' WHERE [CREDIT_LINE_YPE] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [CREDIT_LINE_YPE] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [CREDIT_LINE_YPE];
GO

DECLARE @var131 sysname;
SELECT @var131 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'CREATED_DATE');
IF @var131 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var131 + '];');
ALTER TABLE [LN01] ALTER COLUMN [CREATED_DATE] datetime2 NOT NULL;
GO

DECLARE @var132 sysname;
SELECT @var132 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'COMMCD');
IF @var132 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var132 + '];');
UPDATE [LN01] SET [COMMCD] = N'' WHERE [COMMCD] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [COMMCD] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [COMMCD];
GO

DECLARE @var133 sysname;
SELECT @var133 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'CMT_HC');
IF @var133 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var133 + '];');
UPDATE [LN01] SET [CMT_HC] = N'' WHERE [CMT_HC] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [CMT_HC] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [CMT_HC];
GO

DECLARE @var134 sysname;
SELECT @var134 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'CHITIEU');
IF @var134 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var134 + '];');
UPDATE [LN01] SET [CHITIEU] = N'' WHERE [CHITIEU] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [CHITIEU] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [CHITIEU];
GO

DECLARE @var135 sysname;
SELECT @var135 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'CCY');
IF @var135 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var135 + '];');
UPDATE [LN01] SET [CCY] = N'' WHERE [CCY] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [CCY] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [CCY];
GO

DECLARE @var136 sysname;
SELECT @var136 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'BUSCD');
IF @var136 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var136 + '];');
UPDATE [LN01] SET [BUSCD] = N'' WHERE [BUSCD] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [BUSCD] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [BUSCD];
GO

DECLARE @var137 sysname;
SELECT @var137 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'BSRTCD');
IF @var137 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var137 + '];');
UPDATE [LN01] SET [BSRTCD] = N'' WHERE [BSRTCD] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [BSRTCD] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [BSRTCD];
GO

DECLARE @var138 sysname;
SELECT @var138 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'BSNSSCLTPCD');
IF @var138 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var138 + '];');
UPDATE [LN01] SET [BSNSSCLTPCD] = N'' WHERE [BSNSSCLTPCD] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [BSNSSCLTPCD] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [BSNSSCLTPCD];
GO

DECLARE @var139 sysname;
SELECT @var139 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'BRCD');
IF @var139 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var139 + '];');
UPDATE [LN01] SET [BRCD] = N'' WHERE [BRCD] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [BRCD] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [BRCD];
GO

DECLARE @var140 sysname;
SELECT @var140 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'APPR_CCY');
IF @var140 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var140 + '];');
UPDATE [LN01] SET [APPR_CCY] = N'' WHERE [APPR_CCY] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [APPR_CCY] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [APPR_CCY];
GO

DECLARE @var141 sysname;
SELECT @var141 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'APPRSEQ');
IF @var141 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var141 + '];');
UPDATE [LN01] SET [APPRSEQ] = N'' WHERE [APPRSEQ] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [APPRSEQ] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [APPRSEQ];
GO

DECLARE @var142 sysname;
SELECT @var142 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'APPRMATDT');
IF @var142 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var142 + '];');
UPDATE [LN01] SET [APPRMATDT] = N'' WHERE [APPRMATDT] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [APPRMATDT] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [APPRMATDT];
GO

DECLARE @var143 sysname;
SELECT @var143 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'APPRDT');
IF @var143 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var143 + '];');
UPDATE [LN01] SET [APPRDT] = N'' WHERE [APPRDT] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [APPRDT] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [APPRDT];
GO

DECLARE @var144 sysname;
SELECT @var144 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'APPRAMT');
IF @var144 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var144 + '];');
UPDATE [LN01] SET [APPRAMT] = N'' WHERE [APPRAMT] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [APPRAMT] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [APPRAMT];
GO

DECLARE @var145 sysname;
SELECT @var145 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'AN_HAN_LAI');
IF @var145 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var145 + '];');
UPDATE [LN01] SET [AN_HAN_LAI] = N'' WHERE [AN_HAN_LAI] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [AN_HAN_LAI] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [AN_HAN_LAI];
GO

DECLARE @var146 sysname;
SELECT @var146 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'ADDR1');
IF @var146 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var146 + '];');
UPDATE [LN01] SET [ADDR1] = N'' WHERE [ADDR1] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [ADDR1] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [ADDR1];
GO

DECLARE @var147 sysname;
SELECT @var147 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'ACCRUAL_AMOUNT_END_OF_MONTH');
IF @var147 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var147 + '];');
UPDATE [LN01] SET [ACCRUAL_AMOUNT_END_OF_MONTH] = N'' WHERE [ACCRUAL_AMOUNT_END_OF_MONTH] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [ACCRUAL_AMOUNT_END_OF_MONTH] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [ACCRUAL_AMOUNT_END_OF_MONTH];
GO

DECLARE @var148 sysname;
SELECT @var148 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'ACCRUAL_AMOUNT');
IF @var148 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var148 + '];');
UPDATE [LN01] SET [ACCRUAL_AMOUNT] = N'' WHERE [ACCRUAL_AMOUNT] IS NULL;
ALTER TABLE [LN01] ALTER COLUMN [ACCRUAL_AMOUNT] nvarchar(50) NOT NULL;
ALTER TABLE [LN01] ADD DEFAULT N'' FOR [ACCRUAL_AMOUNT];
GO

DECLARE @var149 sysname;
SELECT @var149 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LN01]') AND [c].[name] = N'Id');
IF @var149 IS NOT NULL EXEC(N'ALTER TABLE [LN01] DROP CONSTRAINT [' + @var149 + '];');
ALTER TABLE [LN01] ALTER COLUMN [Id] int NOT NULL;
GO

DECLARE @var150 sysname;
SELECT @var150 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL41]') AND [c].[name] = N'UPDATED_DATE');
IF @var150 IS NOT NULL EXEC(N'ALTER TABLE [GL41] DROP CONSTRAINT [' + @var150 + '];');
UPDATE [GL41] SET [UPDATED_DATE] = '0001-01-01T00:00:00.0000000' WHERE [UPDATED_DATE] IS NULL;
ALTER TABLE [GL41] ALTER COLUMN [UPDATED_DATE] datetime2 NOT NULL;
ALTER TABLE [GL41] ADD DEFAULT '0001-01-01T00:00:00.0000000' FOR [UPDATED_DATE];
GO

DECLARE @var151 sysname;
SELECT @var151 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL41]') AND [c].[name] = N'TEN_TK');
IF @var151 IS NOT NULL EXEC(N'ALTER TABLE [GL41] DROP CONSTRAINT [' + @var151 + '];');
UPDATE [GL41] SET [TEN_TK] = N'' WHERE [TEN_TK] IS NULL;
ALTER TABLE [GL41] ALTER COLUMN [TEN_TK] nvarchar(50) NOT NULL;
ALTER TABLE [GL41] ADD DEFAULT N'' FOR [TEN_TK];
GO

DECLARE @var152 sysname;
SELECT @var152 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL41]') AND [c].[name] = N'ST_GHINO');
IF @var152 IS NOT NULL EXEC(N'ALTER TABLE [GL41] DROP CONSTRAINT [' + @var152 + '];');
UPDATE [GL41] SET [ST_GHINO] = N'' WHERE [ST_GHINO] IS NULL;
ALTER TABLE [GL41] ALTER COLUMN [ST_GHINO] nvarchar(50) NOT NULL;
ALTER TABLE [GL41] ADD DEFAULT N'' FOR [ST_GHINO];
GO

DECLARE @var153 sysname;
SELECT @var153 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL41]') AND [c].[name] = N'ST_GHICO');
IF @var153 IS NOT NULL EXEC(N'ALTER TABLE [GL41] DROP CONSTRAINT [' + @var153 + '];');
UPDATE [GL41] SET [ST_GHICO] = N'' WHERE [ST_GHICO] IS NULL;
ALTER TABLE [GL41] ALTER COLUMN [ST_GHICO] nvarchar(50) NOT NULL;
ALTER TABLE [GL41] ADD DEFAULT N'' FOR [ST_GHICO];
GO

DECLARE @var154 sysname;
SELECT @var154 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL41]') AND [c].[name] = N'SBT_NO');
IF @var154 IS NOT NULL EXEC(N'ALTER TABLE [GL41] DROP CONSTRAINT [' + @var154 + '];');
UPDATE [GL41] SET [SBT_NO] = N'' WHERE [SBT_NO] IS NULL;
ALTER TABLE [GL41] ALTER COLUMN [SBT_NO] nvarchar(50) NOT NULL;
ALTER TABLE [GL41] ADD DEFAULT N'' FOR [SBT_NO];
GO

DECLARE @var155 sysname;
SELECT @var155 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL41]') AND [c].[name] = N'SBT_CO');
IF @var155 IS NOT NULL EXEC(N'ALTER TABLE [GL41] DROP CONSTRAINT [' + @var155 + '];');
UPDATE [GL41] SET [SBT_CO] = N'' WHERE [SBT_CO] IS NULL;
ALTER TABLE [GL41] ALTER COLUMN [SBT_CO] nvarchar(50) NOT NULL;
ALTER TABLE [GL41] ADD DEFAULT N'' FOR [SBT_CO];
GO

DECLARE @var156 sysname;
SELECT @var156 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL41]') AND [c].[name] = N'NGAY_DL');
IF @var156 IS NOT NULL EXEC(N'ALTER TABLE [GL41] DROP CONSTRAINT [' + @var156 + '];');
ALTER TABLE [GL41] ALTER COLUMN [NGAY_DL] nvarchar(10) NOT NULL;
GO

DECLARE @var157 sysname;
SELECT @var157 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL41]') AND [c].[name] = N'MA_TK');
IF @var157 IS NOT NULL EXEC(N'ALTER TABLE [GL41] DROP CONSTRAINT [' + @var157 + '];');
UPDATE [GL41] SET [MA_TK] = N'' WHERE [MA_TK] IS NULL;
ALTER TABLE [GL41] ALTER COLUMN [MA_TK] nvarchar(50) NOT NULL;
ALTER TABLE [GL41] ADD DEFAULT N'' FOR [MA_TK];
GO

DECLARE @var158 sysname;
SELECT @var158 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL41]') AND [c].[name] = N'MA_CN');
IF @var158 IS NOT NULL EXEC(N'ALTER TABLE [GL41] DROP CONSTRAINT [' + @var158 + '];');
UPDATE [GL41] SET [MA_CN] = N'' WHERE [MA_CN] IS NULL;
ALTER TABLE [GL41] ALTER COLUMN [MA_CN] nvarchar(50) NOT NULL;
ALTER TABLE [GL41] ADD DEFAULT N'' FOR [MA_CN];
GO

DECLARE @var159 sysname;
SELECT @var159 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL41]') AND [c].[name] = N'LOAI_TIEN');
IF @var159 IS NOT NULL EXEC(N'ALTER TABLE [GL41] DROP CONSTRAINT [' + @var159 + '];');
UPDATE [GL41] SET [LOAI_TIEN] = N'' WHERE [LOAI_TIEN] IS NULL;
ALTER TABLE [GL41] ALTER COLUMN [LOAI_TIEN] nvarchar(50) NOT NULL;
ALTER TABLE [GL41] ADD DEFAULT N'' FOR [LOAI_TIEN];
GO

DECLARE @var160 sysname;
SELECT @var160 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL41]') AND [c].[name] = N'LOAI_BT');
IF @var160 IS NOT NULL EXEC(N'ALTER TABLE [GL41] DROP CONSTRAINT [' + @var160 + '];');
UPDATE [GL41] SET [LOAI_BT] = N'' WHERE [LOAI_BT] IS NULL;
ALTER TABLE [GL41] ALTER COLUMN [LOAI_BT] nvarchar(50) NOT NULL;
ALTER TABLE [GL41] ADD DEFAULT N'' FOR [LOAI_BT];
GO

DECLARE @var161 sysname;
SELECT @var161 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL41]') AND [c].[name] = N'FILE_NAME');
IF @var161 IS NOT NULL EXEC(N'ALTER TABLE [GL41] DROP CONSTRAINT [' + @var161 + '];');
UPDATE [GL41] SET [FILE_NAME] = N'' WHERE [FILE_NAME] IS NULL;
ALTER TABLE [GL41] ALTER COLUMN [FILE_NAME] nvarchar(255) NOT NULL;
ALTER TABLE [GL41] ADD DEFAULT N'' FOR [FILE_NAME];
GO

DECLARE @var162 sysname;
SELECT @var162 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL41]') AND [c].[name] = N'DN_DAUKY');
IF @var162 IS NOT NULL EXEC(N'ALTER TABLE [GL41] DROP CONSTRAINT [' + @var162 + '];');
UPDATE [GL41] SET [DN_DAUKY] = N'' WHERE [DN_DAUKY] IS NULL;
ALTER TABLE [GL41] ALTER COLUMN [DN_DAUKY] nvarchar(50) NOT NULL;
ALTER TABLE [GL41] ADD DEFAULT N'' FOR [DN_DAUKY];
GO

DECLARE @var163 sysname;
SELECT @var163 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL41]') AND [c].[name] = N'DN_CUOIKY');
IF @var163 IS NOT NULL EXEC(N'ALTER TABLE [GL41] DROP CONSTRAINT [' + @var163 + '];');
UPDATE [GL41] SET [DN_CUOIKY] = N'' WHERE [DN_CUOIKY] IS NULL;
ALTER TABLE [GL41] ALTER COLUMN [DN_CUOIKY] nvarchar(50) NOT NULL;
ALTER TABLE [GL41] ADD DEFAULT N'' FOR [DN_CUOIKY];
GO

DECLARE @var164 sysname;
SELECT @var164 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL41]') AND [c].[name] = N'DC_DAUKY');
IF @var164 IS NOT NULL EXEC(N'ALTER TABLE [GL41] DROP CONSTRAINT [' + @var164 + '];');
UPDATE [GL41] SET [DC_DAUKY] = N'' WHERE [DC_DAUKY] IS NULL;
ALTER TABLE [GL41] ALTER COLUMN [DC_DAUKY] nvarchar(50) NOT NULL;
ALTER TABLE [GL41] ADD DEFAULT N'' FOR [DC_DAUKY];
GO

DECLARE @var165 sysname;
SELECT @var165 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL41]') AND [c].[name] = N'DC_CUOIKY');
IF @var165 IS NOT NULL EXEC(N'ALTER TABLE [GL41] DROP CONSTRAINT [' + @var165 + '];');
UPDATE [GL41] SET [DC_CUOIKY] = N'' WHERE [DC_CUOIKY] IS NULL;
ALTER TABLE [GL41] ALTER COLUMN [DC_CUOIKY] nvarchar(50) NOT NULL;
ALTER TABLE [GL41] ADD DEFAULT N'' FOR [DC_CUOIKY];
GO

DECLARE @var166 sysname;
SELECT @var166 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL41]') AND [c].[name] = N'CREATED_DATE');
IF @var166 IS NOT NULL EXEC(N'ALTER TABLE [GL41] DROP CONSTRAINT [' + @var166 + '];');
ALTER TABLE [GL41] ALTER COLUMN [CREATED_DATE] datetime2 NOT NULL;
GO

DECLARE @var167 sysname;
SELECT @var167 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL41]') AND [c].[name] = N'Id');
IF @var167 IS NOT NULL EXEC(N'ALTER TABLE [GL41] DROP CONSTRAINT [' + @var167 + '];');
ALTER TABLE [GL41] ALTER COLUMN [Id] int NOT NULL;
GO

DECLARE @var168 sysname;
SELECT @var168 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'VALUE_DATE');
IF @var168 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var168 + '];');
UPDATE [GL01] SET [VALUE_DATE] = N'' WHERE [VALUE_DATE] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [VALUE_DATE] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [VALUE_DATE];
GO

DECLARE @var169 sysname;
SELECT @var169 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'UNIT_BUS_CODE');
IF @var169 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var169 + '];');
UPDATE [GL01] SET [UNIT_BUS_CODE] = N'' WHERE [UNIT_BUS_CODE] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [UNIT_BUS_CODE] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [UNIT_BUS_CODE];
GO

DECLARE @var170 sysname;
SELECT @var170 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'TR_TYPE');
IF @var170 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var170 + '];');
UPDATE [GL01] SET [TR_TYPE] = N'' WHERE [TR_TYPE] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [TR_TYPE] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [TR_TYPE];
GO

DECLARE @var171 sysname;
SELECT @var171 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'TR_TIME');
IF @var171 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var171 + '];');
UPDATE [GL01] SET [TR_TIME] = N'' WHERE [TR_TIME] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [TR_TIME] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [TR_TIME];
GO

DECLARE @var172 sysname;
SELECT @var172 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'TR_NAME');
IF @var172 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var172 + '];');
UPDATE [GL01] SET [TR_NAME] = N'' WHERE [TR_NAME] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [TR_NAME] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [TR_NAME];
GO

DECLARE @var173 sysname;
SELECT @var173 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'TR_EX_RT');
IF @var173 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var173 + '];');
UPDATE [GL01] SET [TR_EX_RT] = N'' WHERE [TR_EX_RT] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [TR_EX_RT] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [TR_EX_RT];
GO

DECLARE @var174 sysname;
SELECT @var174 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'TR_CODE');
IF @var174 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var174 + '];');
UPDATE [GL01] SET [TR_CODE] = N'' WHERE [TR_CODE] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [TR_CODE] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [TR_CODE];
GO

DECLARE @var175 sysname;
SELECT @var175 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'TRDT_TIME');
IF @var175 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var175 + '];');
UPDATE [GL01] SET [TRDT_TIME] = N'' WHERE [TRDT_TIME] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [TRDT_TIME] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [TRDT_TIME];
GO

DECLARE @var176 sysname;
SELECT @var176 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'TEN_TK');
IF @var176 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var176 + '];');
UPDATE [GL01] SET [TEN_TK] = N'' WHERE [TEN_TK] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [TEN_TK] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [TEN_TK];
GO

DECLARE @var177 sysname;
SELECT @var177 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'TEN_KH');
IF @var177 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var177 + '];');
UPDATE [GL01] SET [TEN_KH] = N'' WHERE [TEN_KH] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [TEN_KH] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [TEN_KH];
GO

DECLARE @var178 sysname;
SELECT @var178 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'TAI_KHOAN');
IF @var178 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var178 + '];');
UPDATE [GL01] SET [TAI_KHOAN] = N'' WHERE [TAI_KHOAN] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [TAI_KHOAN] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [TAI_KHOAN];
GO

DECLARE @var179 sysname;
SELECT @var179 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'STS');
IF @var179 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var179 + '];');
UPDATE [GL01] SET [STS] = N'' WHERE [STS] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [STS] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [STS];
GO

DECLARE @var180 sysname;
SELECT @var180 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'SO_TIEN_GD');
IF @var180 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var180 + '];');
UPDATE [GL01] SET [SO_TIEN_GD] = N'' WHERE [SO_TIEN_GD] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [SO_TIEN_GD] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [SO_TIEN_GD];
GO

DECLARE @var181 sysname;
SELECT @var181 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'REMARK');
IF @var181 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var181 + '];');
UPDATE [GL01] SET [REMARK] = N'' WHERE [REMARK] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [REMARK] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [REMARK];
GO

DECLARE @var182 sysname;
SELECT @var182 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'REFERENCE');
IF @var182 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var182 + '];');
UPDATE [GL01] SET [REFERENCE] = N'' WHERE [REFERENCE] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [REFERENCE] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [REFERENCE];
GO

DECLARE @var183 sysname;
SELECT @var183 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'POST_BR');
IF @var183 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var183 + '];');
UPDATE [GL01] SET [POST_BR] = N'' WHERE [POST_BR] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [POST_BR] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [POST_BR];
GO

DECLARE @var184 sysname;
SELECT @var184 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'NGUOI_TAO');
IF @var184 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var184 + '];');
UPDATE [GL01] SET [NGUOI_TAO] = N'' WHERE [NGUOI_TAO] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [NGUOI_TAO] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [NGUOI_TAO];
GO

DECLARE @var185 sysname;
SELECT @var185 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'NGAY_GD');
IF @var185 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var185 + '];');
UPDATE [GL01] SET [NGAY_GD] = N'' WHERE [NGAY_GD] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [NGAY_GD] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [NGAY_GD];
GO

DROP INDEX [IX_GL01_NGAY_DL] ON [GL01];
DECLARE @var186 sysname;
SELECT @var186 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'NGAY_DL');
IF @var186 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var186 + '];');
UPDATE [GL01] SET [NGAY_DL] = N'' WHERE [NGAY_DL] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [NGAY_DL] nvarchar(10) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [NGAY_DL];
CREATE INDEX [IX_GL01_NGAY_DL] ON [GL01] ([NGAY_DL]);
GO

DECLARE @var187 sysname;
SELECT @var187 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'MA_KH');
IF @var187 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var187 + '];');
UPDATE [GL01] SET [MA_KH] = N'' WHERE [MA_KH] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [MA_KH] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [MA_KH];
GO

DECLARE @var188 sysname;
SELECT @var188 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'LOAI_TIEN');
IF @var188 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var188 + '];');
UPDATE [GL01] SET [LOAI_TIEN] = N'' WHERE [LOAI_TIEN] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [LOAI_TIEN] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [LOAI_TIEN];
GO

DECLARE @var189 sysname;
SELECT @var189 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'FILE_NAME');
IF @var189 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var189 + '];');
UPDATE [GL01] SET [FILE_NAME] = N'' WHERE [FILE_NAME] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [FILE_NAME] nvarchar(255) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [FILE_NAME];
GO

DECLARE @var190 sysname;
SELECT @var190 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'DYSEQ');
IF @var190 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var190 + '];');
UPDATE [GL01] SET [DYSEQ] = N'' WHERE [DYSEQ] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [DYSEQ] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [DYSEQ];
GO

DECLARE @var191 sysname;
SELECT @var191 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'DT_SEQ');
IF @var191 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var191 + '];');
UPDATE [GL01] SET [DT_SEQ] = N'' WHERE [DT_SEQ] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [DT_SEQ] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [DT_SEQ];
GO

DECLARE @var192 sysname;
SELECT @var192 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'DR_CR');
IF @var192 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var192 + '];');
UPDATE [GL01] SET [DR_CR] = N'' WHERE [DR_CR] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [DR_CR] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [DR_CR];
GO

DECLARE @var193 sysname;
SELECT @var193 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'DEPT_CODE');
IF @var193 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var193 + '];');
UPDATE [GL01] SET [DEPT_CODE] = N'' WHERE [DEPT_CODE] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [DEPT_CODE] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [DEPT_CODE];
GO

DECLARE @var194 sysname;
SELECT @var194 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'COMFIRM');
IF @var194 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var194 + '];');
UPDATE [GL01] SET [COMFIRM] = N'' WHERE [COMFIRM] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [COMFIRM] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [COMFIRM];
GO

DECLARE @var195 sysname;
SELECT @var195 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'CCA_USRID');
IF @var195 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var195 + '];');
UPDATE [GL01] SET [CCA_USRID] = N'' WHERE [CCA_USRID] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [CCA_USRID] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [CCA_USRID];
GO

DECLARE @var196 sysname;
SELECT @var196 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'BUS_CODE');
IF @var196 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var196 + '];');
UPDATE [GL01] SET [BUS_CODE] = N'' WHERE [BUS_CODE] IS NULL;
ALTER TABLE [GL01] ALTER COLUMN [BUS_CODE] nvarchar(50) NOT NULL;
ALTER TABLE [GL01] ADD DEFAULT N'' FOR [BUS_CODE];
GO

DECLARE @var197 sysname;
SELECT @var197 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'Id');
IF @var197 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var197 + '];');
ALTER TABLE [GL01] ALTER COLUMN [Id] int NOT NULL;
GO

DECLARE @var198 sysname;
SELECT @var198 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GL01]') AND [c].[name] = N'UPDATED_DATE');
IF @var198 IS NOT NULL EXEC(N'ALTER TABLE [GL01] DROP CONSTRAINT [' + @var198 + '];');
ALTER TABLE [GL01] ALTER COLUMN [UPDATED_DATE] datetime2 NOT NULL;
GO

ALTER TABLE [GL01] ADD [CREATED_DATE] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

DECLARE @var199 sysname;
SELECT @var199 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'USER_SMS');
IF @var199 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var199 + '];');
UPDATE [EI01] SET [USER_SMS] = N'' WHERE [USER_SMS] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [USER_SMS] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [USER_SMS];
GO

DECLARE @var200 sysname;
SELECT @var200 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'USER_SAV');
IF @var200 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var200 + '];');
UPDATE [EI01] SET [USER_SAV] = N'' WHERE [USER_SAV] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [USER_SAV] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [USER_SAV];
GO

DECLARE @var201 sysname;
SELECT @var201 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'USER_OTT');
IF @var201 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var201 + '];');
UPDATE [EI01] SET [USER_OTT] = N'' WHERE [USER_OTT] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [USER_OTT] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [USER_OTT];
GO

DECLARE @var202 sysname;
SELECT @var202 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'USER_LN');
IF @var202 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var202 + '];');
UPDATE [EI01] SET [USER_LN] = N'' WHERE [USER_LN] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [USER_LN] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [USER_LN];
GO

DECLARE @var203 sysname;
SELECT @var203 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'USER_EMB');
IF @var203 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var203 + '];');
UPDATE [EI01] SET [USER_EMB] = N'' WHERE [USER_EMB] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [USER_EMB] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [USER_EMB];
GO

DECLARE @var204 sysname;
SELECT @var204 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'TRANG_THAI_SMS');
IF @var204 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var204 + '];');
UPDATE [EI01] SET [TRANG_THAI_SMS] = N'' WHERE [TRANG_THAI_SMS] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [TRANG_THAI_SMS] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [TRANG_THAI_SMS];
GO

DECLARE @var205 sysname;
SELECT @var205 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'TRANG_THAI_SAV');
IF @var205 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var205 + '];');
UPDATE [EI01] SET [TRANG_THAI_SAV] = N'' WHERE [TRANG_THAI_SAV] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [TRANG_THAI_SAV] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [TRANG_THAI_SAV];
GO

DECLARE @var206 sysname;
SELECT @var206 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'TRANG_THAI_OTT');
IF @var206 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var206 + '];');
UPDATE [EI01] SET [TRANG_THAI_OTT] = N'' WHERE [TRANG_THAI_OTT] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [TRANG_THAI_OTT] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [TRANG_THAI_OTT];
GO

DECLARE @var207 sysname;
SELECT @var207 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'TRANG_THAI_LN');
IF @var207 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var207 + '];');
UPDATE [EI01] SET [TRANG_THAI_LN] = N'' WHERE [TRANG_THAI_LN] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [TRANG_THAI_LN] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [TRANG_THAI_LN];
GO

DECLARE @var208 sysname;
SELECT @var208 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'TRANG_THAI_EMB');
IF @var208 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var208 + '];');
UPDATE [EI01] SET [TRANG_THAI_EMB] = N'' WHERE [TRANG_THAI_EMB] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [TRANG_THAI_EMB] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [TRANG_THAI_EMB];
GO

DECLARE @var209 sysname;
SELECT @var209 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'TEN_KH');
IF @var209 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var209 + '];');
UPDATE [EI01] SET [TEN_KH] = N'' WHERE [TEN_KH] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [TEN_KH] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [TEN_KH];
GO

DECLARE @var210 sysname;
SELECT @var210 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'SDT_SMS');
IF @var210 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var210 + '];');
UPDATE [EI01] SET [SDT_SMS] = N'' WHERE [SDT_SMS] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [SDT_SMS] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [SDT_SMS];
GO

DECLARE @var211 sysname;
SELECT @var211 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'SDT_SAV');
IF @var211 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var211 + '];');
UPDATE [EI01] SET [SDT_SAV] = N'' WHERE [SDT_SAV] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [SDT_SAV] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [SDT_SAV];
GO

DECLARE @var212 sysname;
SELECT @var212 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'SDT_OTT');
IF @var212 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var212 + '];');
UPDATE [EI01] SET [SDT_OTT] = N'' WHERE [SDT_OTT] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [SDT_OTT] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [SDT_OTT];
GO

DECLARE @var213 sysname;
SELECT @var213 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'SDT_LN');
IF @var213 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var213 + '];');
UPDATE [EI01] SET [SDT_LN] = N'' WHERE [SDT_LN] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [SDT_LN] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [SDT_LN];
GO

DECLARE @var214 sysname;
SELECT @var214 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'SDT_EMB');
IF @var214 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var214 + '];');
UPDATE [EI01] SET [SDT_EMB] = N'' WHERE [SDT_EMB] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [SDT_EMB] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [SDT_EMB];
GO

DROP INDEX [IX_EI01_NGAY_DL] ON [EI01];
DROP INDEX [IX_EI01_NGAY_DL_MaCN] ON [EI01];
DECLARE @var215 sysname;
SELECT @var215 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'NGAY_DL');
IF @var215 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var215 + '];');
UPDATE [EI01] SET [NGAY_DL] = N'' WHERE [NGAY_DL] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [NGAY_DL] nvarchar(10) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [NGAY_DL];
CREATE INDEX [IX_EI01_NGAY_DL] ON [EI01] ([NGAY_DL]);
CREATE INDEX [IX_EI01_NGAY_DL_MaCN] ON [EI01] ([NGAY_DL], [MA_CN]);
GO

DECLARE @var216 sysname;
SELECT @var216 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'NGAY_DK_SMS');
IF @var216 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var216 + '];');
UPDATE [EI01] SET [NGAY_DK_SMS] = N'' WHERE [NGAY_DK_SMS] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [NGAY_DK_SMS] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [NGAY_DK_SMS];
GO

DECLARE @var217 sysname;
SELECT @var217 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'NGAY_DK_SAV');
IF @var217 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var217 + '];');
UPDATE [EI01] SET [NGAY_DK_SAV] = N'' WHERE [NGAY_DK_SAV] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [NGAY_DK_SAV] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [NGAY_DK_SAV];
GO

DECLARE @var218 sysname;
SELECT @var218 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'NGAY_DK_OTT');
IF @var218 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var218 + '];');
UPDATE [EI01] SET [NGAY_DK_OTT] = N'' WHERE [NGAY_DK_OTT] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [NGAY_DK_OTT] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [NGAY_DK_OTT];
GO

DECLARE @var219 sysname;
SELECT @var219 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'NGAY_DK_LN');
IF @var219 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var219 + '];');
UPDATE [EI01] SET [NGAY_DK_LN] = N'' WHERE [NGAY_DK_LN] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [NGAY_DK_LN] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [NGAY_DK_LN];
GO

DECLARE @var220 sysname;
SELECT @var220 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'NGAY_DK_EMB');
IF @var220 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var220 + '];');
UPDATE [EI01] SET [NGAY_DK_EMB] = N'' WHERE [NGAY_DK_EMB] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [NGAY_DK_EMB] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [NGAY_DK_EMB];
GO

DECLARE @var221 sysname;
SELECT @var221 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'MA_KH');
IF @var221 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var221 + '];');
UPDATE [EI01] SET [MA_KH] = N'' WHERE [MA_KH] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [MA_KH] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [MA_KH];
GO

DROP INDEX [IX_EI01_MaCN] ON [EI01];
DROP INDEX [IX_EI01_NGAY_DL_MaCN] ON [EI01];
DECLARE @var222 sysname;
SELECT @var222 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'MA_CN');
IF @var222 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var222 + '];');
UPDATE [EI01] SET [MA_CN] = N'' WHERE [MA_CN] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [MA_CN] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [MA_CN];
CREATE INDEX [IX_EI01_MaCN] ON [EI01] ([MA_CN]);
CREATE INDEX [IX_EI01_NGAY_DL_MaCN] ON [EI01] ([NGAY_DL], [MA_CN]);
GO

DECLARE @var223 sysname;
SELECT @var223 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'LOAI_KH');
IF @var223 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var223 + '];');
UPDATE [EI01] SET [LOAI_KH] = N'' WHERE [LOAI_KH] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [LOAI_KH] nvarchar(50) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [LOAI_KH];
GO

DECLARE @var224 sysname;
SELECT @var224 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'FILE_NAME');
IF @var224 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var224 + '];');
UPDATE [EI01] SET [FILE_NAME] = N'' WHERE [FILE_NAME] IS NULL;
ALTER TABLE [EI01] ALTER COLUMN [FILE_NAME] nvarchar(255) NOT NULL;
ALTER TABLE [EI01] ADD DEFAULT N'' FOR [FILE_NAME];
GO

DECLARE @var225 sysname;
SELECT @var225 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'Id');
IF @var225 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var225 + '];');
ALTER TABLE [EI01] ALTER COLUMN [Id] int NOT NULL;
GO

DECLARE @var226 sysname;
SELECT @var226 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EI01]') AND [c].[name] = N'UPDATED_DATE');
IF @var226 IS NOT NULL EXEC(N'ALTER TABLE [EI01] DROP CONSTRAINT [' + @var226 + '];');
ALTER TABLE [EI01] ALTER COLUMN [UPDATED_DATE] datetime2 NOT NULL;
GO

ALTER TABLE [EI01] ADD [CREATED_DATE] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

DECLARE @var227 sysname;
SELECT @var227 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DPDA]') AND [c].[name] = N'USER_PHAT_HANH');
IF @var227 IS NOT NULL EXEC(N'ALTER TABLE [DPDA] DROP CONSTRAINT [' + @var227 + '];');
UPDATE [DPDA] SET [USER_PHAT_HANH] = N'' WHERE [USER_PHAT_HANH] IS NULL;
ALTER TABLE [DPDA] ALTER COLUMN [USER_PHAT_HANH] nvarchar(50) NOT NULL;
ALTER TABLE [DPDA] ADD DEFAULT N'' FOR [USER_PHAT_HANH];
GO

DECLARE @var228 sysname;
SELECT @var228 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DPDA]') AND [c].[name] = N'UPDATED_DATE');
IF @var228 IS NOT NULL EXEC(N'ALTER TABLE [DPDA] DROP CONSTRAINT [' + @var228 + '];');
UPDATE [DPDA] SET [UPDATED_DATE] = '0001-01-01T00:00:00.0000000' WHERE [UPDATED_DATE] IS NULL;
ALTER TABLE [DPDA] ALTER COLUMN [UPDATED_DATE] datetime2 NOT NULL;
ALTER TABLE [DPDA] ADD DEFAULT '0001-01-01T00:00:00.0000000' FOR [UPDATED_DATE];
GO

DECLARE @var229 sysname;
SELECT @var229 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DPDA]') AND [c].[name] = N'TRANG_THAI');
IF @var229 IS NOT NULL EXEC(N'ALTER TABLE [DPDA] DROP CONSTRAINT [' + @var229 + '];');
UPDATE [DPDA] SET [TRANG_THAI] = N'' WHERE [TRANG_THAI] IS NULL;
ALTER TABLE [DPDA] ALTER COLUMN [TRANG_THAI] nvarchar(50) NOT NULL;
ALTER TABLE [DPDA] ADD DEFAULT N'' FOR [TRANG_THAI];
GO

DECLARE @var230 sysname;
SELECT @var230 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DPDA]') AND [c].[name] = N'TEN_KHACH_HANG');
IF @var230 IS NOT NULL EXEC(N'ALTER TABLE [DPDA] DROP CONSTRAINT [' + @var230 + '];');
UPDATE [DPDA] SET [TEN_KHACH_HANG] = N'' WHERE [TEN_KHACH_HANG] IS NULL;
ALTER TABLE [DPDA] ALTER COLUMN [TEN_KHACH_HANG] nvarchar(50) NOT NULL;
ALTER TABLE [DPDA] ADD DEFAULT N'' FOR [TEN_KHACH_HANG];
GO

DECLARE @var231 sysname;
SELECT @var231 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DPDA]') AND [c].[name] = N'SO_THE');
IF @var231 IS NOT NULL EXEC(N'ALTER TABLE [DPDA] DROP CONSTRAINT [' + @var231 + '];');
UPDATE [DPDA] SET [SO_THE] = N'' WHERE [SO_THE] IS NULL;
ALTER TABLE [DPDA] ALTER COLUMN [SO_THE] nvarchar(50) NOT NULL;
ALTER TABLE [DPDA] ADD DEFAULT N'' FOR [SO_THE];
GO

DECLARE @var232 sysname;
SELECT @var232 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DPDA]') AND [c].[name] = N'SO_TAI_KHOAN');
IF @var232 IS NOT NULL EXEC(N'ALTER TABLE [DPDA] DROP CONSTRAINT [' + @var232 + '];');
UPDATE [DPDA] SET [SO_TAI_KHOAN] = N'' WHERE [SO_TAI_KHOAN] IS NULL;
ALTER TABLE [DPDA] ALTER COLUMN [SO_TAI_KHOAN] nvarchar(50) NOT NULL;
ALTER TABLE [DPDA] ADD DEFAULT N'' FOR [SO_TAI_KHOAN];
GO

DECLARE @var233 sysname;
SELECT @var233 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DPDA]') AND [c].[name] = N'PHAN_LOAI');
IF @var233 IS NOT NULL EXEC(N'ALTER TABLE [DPDA] DROP CONSTRAINT [' + @var233 + '];');
UPDATE [DPDA] SET [PHAN_LOAI] = N'' WHERE [PHAN_LOAI] IS NULL;
ALTER TABLE [DPDA] ALTER COLUMN [PHAN_LOAI] nvarchar(50) NOT NULL;
ALTER TABLE [DPDA] ADD DEFAULT N'' FOR [PHAN_LOAI];
GO

DECLARE @var234 sysname;
SELECT @var234 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DPDA]') AND [c].[name] = N'NGAY_PHAT_HANH');
IF @var234 IS NOT NULL EXEC(N'ALTER TABLE [DPDA] DROP CONSTRAINT [' + @var234 + '];');
UPDATE [DPDA] SET [NGAY_PHAT_HANH] = N'' WHERE [NGAY_PHAT_HANH] IS NULL;
ALTER TABLE [DPDA] ALTER COLUMN [NGAY_PHAT_HANH] nvarchar(50) NOT NULL;
ALTER TABLE [DPDA] ADD DEFAULT N'' FOR [NGAY_PHAT_HANH];
GO

DECLARE @var235 sysname;
SELECT @var235 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DPDA]') AND [c].[name] = N'NGAY_NOP_DON');
IF @var235 IS NOT NULL EXEC(N'ALTER TABLE [DPDA] DROP CONSTRAINT [' + @var235 + '];');
UPDATE [DPDA] SET [NGAY_NOP_DON] = N'' WHERE [NGAY_NOP_DON] IS NULL;
ALTER TABLE [DPDA] ALTER COLUMN [NGAY_NOP_DON] nvarchar(50) NOT NULL;
ALTER TABLE [DPDA] ADD DEFAULT N'' FOR [NGAY_NOP_DON];
GO

DROP INDEX [IX_DPDA_NGAY_DL] ON [DPDA];
DECLARE @var236 sysname;
SELECT @var236 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DPDA]') AND [c].[name] = N'NGAY_DL');
IF @var236 IS NOT NULL EXEC(N'ALTER TABLE [DPDA] DROP CONSTRAINT [' + @var236 + '];');
ALTER TABLE [DPDA] ALTER COLUMN [NGAY_DL] nvarchar(10) NOT NULL;
CREATE INDEX [IX_DPDA_NGAY_DL] ON [DPDA] ([NGAY_DL]);
GO

DECLARE @var237 sysname;
SELECT @var237 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DPDA]') AND [c].[name] = N'MA_KHACH_HANG');
IF @var237 IS NOT NULL EXEC(N'ALTER TABLE [DPDA] DROP CONSTRAINT [' + @var237 + '];');
UPDATE [DPDA] SET [MA_KHACH_HANG] = N'' WHERE [MA_KHACH_HANG] IS NULL;
ALTER TABLE [DPDA] ALTER COLUMN [MA_KHACH_HANG] nvarchar(50) NOT NULL;
ALTER TABLE [DPDA] ADD DEFAULT N'' FOR [MA_KHACH_HANG];
GO

DECLARE @var238 sysname;
SELECT @var238 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DPDA]') AND [c].[name] = N'MA_CHI_NHANH');
IF @var238 IS NOT NULL EXEC(N'ALTER TABLE [DPDA] DROP CONSTRAINT [' + @var238 + '];');
UPDATE [DPDA] SET [MA_CHI_NHANH] = N'' WHERE [MA_CHI_NHANH] IS NULL;
ALTER TABLE [DPDA] ALTER COLUMN [MA_CHI_NHANH] nvarchar(50) NOT NULL;
ALTER TABLE [DPDA] ADD DEFAULT N'' FOR [MA_CHI_NHANH];
GO

DECLARE @var239 sysname;
SELECT @var239 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DPDA]') AND [c].[name] = N'LOAI_THE');
IF @var239 IS NOT NULL EXEC(N'ALTER TABLE [DPDA] DROP CONSTRAINT [' + @var239 + '];');
UPDATE [DPDA] SET [LOAI_THE] = N'' WHERE [LOAI_THE] IS NULL;
ALTER TABLE [DPDA] ALTER COLUMN [LOAI_THE] nvarchar(50) NOT NULL;
ALTER TABLE [DPDA] ADD DEFAULT N'' FOR [LOAI_THE];
GO

DECLARE @var240 sysname;
SELECT @var240 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DPDA]') AND [c].[name] = N'LOAI_PHAT_HANH');
IF @var240 IS NOT NULL EXEC(N'ALTER TABLE [DPDA] DROP CONSTRAINT [' + @var240 + '];');
UPDATE [DPDA] SET [LOAI_PHAT_HANH] = N'' WHERE [LOAI_PHAT_HANH] IS NULL;
ALTER TABLE [DPDA] ALTER COLUMN [LOAI_PHAT_HANH] nvarchar(50) NOT NULL;
ALTER TABLE [DPDA] ADD DEFAULT N'' FOR [LOAI_PHAT_HANH];
GO

DECLARE @var241 sysname;
SELECT @var241 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DPDA]') AND [c].[name] = N'GIAO_THE');
IF @var241 IS NOT NULL EXEC(N'ALTER TABLE [DPDA] DROP CONSTRAINT [' + @var241 + '];');
UPDATE [DPDA] SET [GIAO_THE] = N'' WHERE [GIAO_THE] IS NULL;
ALTER TABLE [DPDA] ALTER COLUMN [GIAO_THE] nvarchar(50) NOT NULL;
ALTER TABLE [DPDA] ADD DEFAULT N'' FOR [GIAO_THE];
GO

DECLARE @var242 sysname;
SELECT @var242 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DPDA]') AND [c].[name] = N'FILE_NAME');
IF @var242 IS NOT NULL EXEC(N'ALTER TABLE [DPDA] DROP CONSTRAINT [' + @var242 + '];');
UPDATE [DPDA] SET [FILE_NAME] = N'' WHERE [FILE_NAME] IS NULL;
ALTER TABLE [DPDA] ALTER COLUMN [FILE_NAME] nvarchar(255) NOT NULL;
ALTER TABLE [DPDA] ADD DEFAULT N'' FOR [FILE_NAME];
GO

DECLARE @var243 sysname;
SELECT @var243 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DPDA]') AND [c].[name] = N'CREATED_DATE');
IF @var243 IS NOT NULL EXEC(N'ALTER TABLE [DPDA] DROP CONSTRAINT [' + @var243 + '];');
ALTER TABLE [DPDA] ALTER COLUMN [CREATED_DATE] datetime2 NOT NULL;
GO

DECLARE @var244 sysname;
SELECT @var244 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DPDA]') AND [c].[name] = N'Id');
IF @var244 IS NOT NULL EXEC(N'ALTER TABLE [DPDA] DROP CONSTRAINT [' + @var244 + '];');
ALTER TABLE [DPDA] ALTER COLUMN [Id] int NOT NULL;
GO

DECLARE @var245 sysname;
SELECT @var245 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'ZIP_CODE');
IF @var245 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var245 + '];');
UPDATE [DP01] SET [ZIP_CODE] = N'' WHERE [ZIP_CODE] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [ZIP_CODE] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [ZIP_CODE];
GO

DECLARE @var246 sysname;
SELECT @var246 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'UNTBUSCD');
IF @var246 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var246 + '];');
UPDATE [DP01] SET [UNTBUSCD] = N'' WHERE [UNTBUSCD] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [UNTBUSCD] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [UNTBUSCD];
GO

DECLARE @var247 sysname;
SELECT @var247 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'TYGIA');
IF @var247 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var247 + '];');
UPDATE [DP01] SET [TYGIA] = N'' WHERE [TYGIA] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [TYGIA] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [TYGIA];
GO

DECLARE @var248 sysname;
SELECT @var248 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'TIME_DP_TYPE');
IF @var248 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var248 + '];');
UPDATE [DP01] SET [TIME_DP_TYPE] = N'' WHERE [TIME_DP_TYPE] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [TIME_DP_TYPE] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [TIME_DP_TYPE];
GO

DECLARE @var249 sysname;
SELECT @var249 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'TIME_DP_NAME');
IF @var249 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var249 + '];');
UPDATE [DP01] SET [TIME_DP_NAME] = N'' WHERE [TIME_DP_NAME] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [TIME_DP_NAME] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [TIME_DP_NAME];
GO

DECLARE @var250 sysname;
SELECT @var250 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'TERM_DP_TYPE');
IF @var250 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var250 + '];');
UPDATE [DP01] SET [TERM_DP_TYPE] = N'' WHERE [TERM_DP_TYPE] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [TERM_DP_TYPE] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [TERM_DP_TYPE];
GO

DECLARE @var251 sysname;
SELECT @var251 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'TERM_DP_NAME');
IF @var251 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var251 + '];');
UPDATE [DP01] SET [TERM_DP_NAME] = N'' WHERE [TERM_DP_NAME] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [TERM_DP_NAME] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [TERM_DP_NAME];
GO

DECLARE @var252 sysname;
SELECT @var252 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'TEN_PGD');
IF @var252 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var252 + '];');
UPDATE [DP01] SET [TEN_PGD] = N'' WHERE [TEN_PGD] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [TEN_PGD] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [TEN_PGD];
GO

DECLARE @var253 sysname;
SELECT @var253 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'TEN_NGUOI_GIOI_THIEU');
IF @var253 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var253 + '];');
UPDATE [DP01] SET [TEN_NGUOI_GIOI_THIEU] = N'' WHERE [TEN_NGUOI_GIOI_THIEU] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [TEN_NGUOI_GIOI_THIEU] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [TEN_NGUOI_GIOI_THIEU];
GO

DECLARE @var254 sysname;
SELECT @var254 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'TEN_KH');
IF @var254 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var254 + '];');
UPDATE [DP01] SET [TEN_KH] = N'' WHERE [TEN_KH] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [TEN_KH] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [TEN_KH];
GO

DECLARE @var255 sysname;
SELECT @var255 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'TEN_CAN_BO_PT');
IF @var255 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var255 + '];');
UPDATE [DP01] SET [TEN_CAN_BO_PT] = N'' WHERE [TEN_CAN_BO_PT] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [TEN_CAN_BO_PT] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [TEN_CAN_BO_PT];
GO

DECLARE @var256 sysname;
SELECT @var256 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'TELEPHONE');
IF @var256 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var256 + '];');
UPDATE [DP01] SET [TELEPHONE] = N'' WHERE [TELEPHONE] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [TELEPHONE] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [TELEPHONE];
GO

DECLARE @var257 sysname;
SELECT @var257 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'TAX_CODE_LOCATION');
IF @var257 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var257 + '];');
UPDATE [DP01] SET [TAX_CODE_LOCATION] = N'' WHERE [TAX_CODE_LOCATION] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [TAX_CODE_LOCATION] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [TAX_CODE_LOCATION];
GO

DECLARE @var258 sysname;
SELECT @var258 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'TAI_KHOAN_HACH_TOAN');
IF @var258 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var258 + '];');
UPDATE [DP01] SET [TAI_KHOAN_HACH_TOAN] = N'' WHERE [TAI_KHOAN_HACH_TOAN] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [TAI_KHOAN_HACH_TOAN] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [TAI_KHOAN_HACH_TOAN];
GO

DECLARE @var259 sysname;
SELECT @var259 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'STATES_CODE');
IF @var259 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var259 + '];');
UPDATE [DP01] SET [STATES_CODE] = N'' WHERE [STATES_CODE] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [STATES_CODE] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [STATES_CODE];
GO

DECLARE @var260 sysname;
SELECT @var260 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'SPECIAL_RATE');
IF @var260 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var260 + '];');
UPDATE [DP01] SET [SPECIAL_RATE] = N'' WHERE [SPECIAL_RATE] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [SPECIAL_RATE] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [SPECIAL_RATE];
GO

DECLARE @var261 sysname;
SELECT @var261 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'SO_TAI_KHOAN');
IF @var261 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var261 + '];');
UPDATE [DP01] SET [SO_TAI_KHOAN] = N'' WHERE [SO_TAI_KHOAN] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [SO_TAI_KHOAN] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [SO_TAI_KHOAN];
GO

DECLARE @var262 sysname;
SELECT @var262 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'SO_KY_AD_LSDB');
IF @var262 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var262 + '];');
UPDATE [DP01] SET [SO_KY_AD_LSDB] = N'' WHERE [SO_KY_AD_LSDB] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [SO_KY_AD_LSDB] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [SO_KY_AD_LSDB];
GO

DECLARE @var263 sysname;
SELECT @var263 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'SEX_TYPE');
IF @var263 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var263 + '];');
UPDATE [DP01] SET [SEX_TYPE] = N'' WHERE [SEX_TYPE] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [SEX_TYPE] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [SEX_TYPE];
GO

DECLARE @var264 sysname;
SELECT @var264 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'RENEW_DATE');
IF @var264 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var264 + '];');
UPDATE [DP01] SET [RENEW_DATE] = N'' WHERE [RENEW_DATE] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [RENEW_DATE] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [RENEW_DATE];
GO

DECLARE @var265 sysname;
SELECT @var265 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'RATE');
IF @var265 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var265 + '];');
UPDATE [DP01] SET [RATE] = N'' WHERE [RATE] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [RATE] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [RATE];
GO

DECLARE @var266 sysname;
SELECT @var266 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'QUOC_TICH');
IF @var266 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var266 + '];');
UPDATE [DP01] SET [QUOC_TICH] = N'' WHERE [QUOC_TICH] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [QUOC_TICH] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [QUOC_TICH];
GO

DECLARE @var267 sysname;
SELECT @var267 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'PREVIOUS_DP_CAP_DATE');
IF @var267 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var267 + '];');
UPDATE [DP01] SET [PREVIOUS_DP_CAP_DATE] = N'' WHERE [PREVIOUS_DP_CAP_DATE] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [PREVIOUS_DP_CAP_DATE] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [PREVIOUS_DP_CAP_DATE];
GO

DECLARE @var268 sysname;
SELECT @var268 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'PHONG_CAN_BO_PT');
IF @var268 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var268 + '];');
UPDATE [DP01] SET [PHONG_CAN_BO_PT] = N'' WHERE [PHONG_CAN_BO_PT] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [PHONG_CAN_BO_PT] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [PHONG_CAN_BO_PT];
GO

DECLARE @var269 sysname;
SELECT @var269 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'OPENING_DATE');
IF @var269 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var269 + '];');
UPDATE [DP01] SET [OPENING_DATE] = N'' WHERE [OPENING_DATE] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [OPENING_DATE] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [OPENING_DATE];
GO

DECLARE @var270 sysname;
SELECT @var270 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'NOTENO');
IF @var270 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var270 + '];');
UPDATE [DP01] SET [NOTENO] = N'' WHERE [NOTENO] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [NOTENO] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [NOTENO];
GO

DECLARE @var271 sysname;
SELECT @var271 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'NGUOI_NUOC_NGOAI');
IF @var271 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var271 + '];');
UPDATE [DP01] SET [NGUOI_NUOC_NGOAI] = N'' WHERE [NGUOI_NUOC_NGOAI] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [NGUOI_NUOC_NGOAI] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [NGUOI_NUOC_NGOAI];
GO

DECLARE @var272 sysname;
SELECT @var272 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'NGUOI_GIOI_THIEU');
IF @var272 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var272 + '];');
UPDATE [DP01] SET [NGUOI_GIOI_THIEU] = N'' WHERE [NGUOI_GIOI_THIEU] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [NGUOI_GIOI_THIEU] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [NGUOI_GIOI_THIEU];
GO

DROP INDEX [IX_DP01_NGAY_DL] ON [DP01];
DROP INDEX [IX_DP01_NGAY_DL_MaCN] ON [DP01];
DECLARE @var273 sysname;
SELECT @var273 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'NGAY_DL');
IF @var273 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var273 + '];');
UPDATE [DP01] SET [NGAY_DL] = N'' WHERE [NGAY_DL] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [NGAY_DL] nvarchar(10) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [NGAY_DL];
CREATE INDEX [IX_DP01_NGAY_DL] ON [DP01] ([NGAY_DL]);
CREATE INDEX [IX_DP01_NGAY_DL_MaCN] ON [DP01] ([NGAY_DL], [MA_CN]);
GO

DECLARE @var274 sysname;
SELECT @var274 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'NEXT_DP_CAP_DATE');
IF @var274 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var274 + '];');
UPDATE [DP01] SET [NEXT_DP_CAP_DATE] = N'' WHERE [NEXT_DP_CAP_DATE] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [NEXT_DP_CAP_DATE] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [NEXT_DP_CAP_DATE];
GO

DECLARE @var275 sysname;
SELECT @var275 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'MONTH_TERM');
IF @var275 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var275 + '];');
UPDATE [DP01] SET [MONTH_TERM] = N'' WHERE [MONTH_TERM] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [MONTH_TERM] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [MONTH_TERM];
GO

DROP INDEX [IX_DP01_MaPGD] ON [DP01];
DECLARE @var276 sysname;
SELECT @var276 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'MA_PGD');
IF @var276 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var276 + '];');
UPDATE [DP01] SET [MA_PGD] = N'' WHERE [MA_PGD] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [MA_PGD] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [MA_PGD];
CREATE INDEX [IX_DP01_MaPGD] ON [DP01] ([MA_PGD]);
GO

DECLARE @var277 sysname;
SELECT @var277 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'MA_KH');
IF @var277 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var277 + '];');
UPDATE [DP01] SET [MA_KH] = N'' WHERE [MA_KH] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [MA_KH] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [MA_KH];
GO

DROP INDEX [IX_DP01_MaCN] ON [DP01];
DROP INDEX [IX_DP01_NGAY_DL_MaCN] ON [DP01];
DECLARE @var278 sysname;
SELECT @var278 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'MA_CN');
IF @var278 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var278 + '];');
UPDATE [DP01] SET [MA_CN] = N'' WHERE [MA_CN] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [MA_CN] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [MA_CN];
CREATE INDEX [IX_DP01_MaCN] ON [DP01] ([MA_CN]);
CREATE INDEX [IX_DP01_NGAY_DL_MaCN] ON [DP01] ([NGAY_DL], [MA_CN]);
GO

DECLARE @var279 sysname;
SELECT @var279 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'MA_CAN_BO_PT');
IF @var279 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var279 + '];');
UPDATE [DP01] SET [MA_CAN_BO_PT] = N'' WHERE [MA_CAN_BO_PT] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [MA_CAN_BO_PT] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [MA_CAN_BO_PT];
GO

DECLARE @var280 sysname;
SELECT @var280 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'MA_CAN_BO_AGRIBANK');
IF @var280 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var280 + '];');
UPDATE [DP01] SET [MA_CAN_BO_AGRIBANK] = N'' WHERE [MA_CAN_BO_AGRIBANK] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [MA_CAN_BO_AGRIBANK] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [MA_CAN_BO_AGRIBANK];
GO

DECLARE @var281 sysname;
SELECT @var281 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'MATURITY_DATE');
IF @var281 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var281 + '];');
UPDATE [DP01] SET [MATURITY_DATE] = N'' WHERE [MATURITY_DATE] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [MATURITY_DATE] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [MATURITY_DATE];
GO

DECLARE @var282 sysname;
SELECT @var282 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'LOCAL_WARD_NAME');
IF @var282 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var282 + '];');
UPDATE [DP01] SET [LOCAL_WARD_NAME] = N'' WHERE [LOCAL_WARD_NAME] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [LOCAL_WARD_NAME] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [LOCAL_WARD_NAME];
GO

DECLARE @var283 sysname;
SELECT @var283 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'LOCAL_PROVIN_NAME');
IF @var283 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var283 + '];');
UPDATE [DP01] SET [LOCAL_PROVIN_NAME] = N'' WHERE [LOCAL_PROVIN_NAME] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [LOCAL_PROVIN_NAME] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [LOCAL_PROVIN_NAME];
GO

DECLARE @var284 sysname;
SELECT @var284 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'LOCAL_DISTRICT_NAME');
IF @var284 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var284 + '];');
UPDATE [DP01] SET [LOCAL_DISTRICT_NAME] = N'' WHERE [LOCAL_DISTRICT_NAME] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [LOCAL_DISTRICT_NAME] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [LOCAL_DISTRICT_NAME];
GO

DECLARE @var285 sysname;
SELECT @var285 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'ISSUE_DATE');
IF @var285 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var285 + '];');
UPDATE [DP01] SET [ISSUE_DATE] = N'' WHERE [ISSUE_DATE] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [ISSUE_DATE] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [ISSUE_DATE];
GO

DECLARE @var286 sysname;
SELECT @var286 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'ISSUED_BY');
IF @var286 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var286 + '];');
UPDATE [DP01] SET [ISSUED_BY] = N'' WHERE [ISSUED_BY] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [ISSUED_BY] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [ISSUED_BY];
GO

DECLARE @var287 sysname;
SELECT @var287 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'ID_NUMBER');
IF @var287 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var287 + '];');
UPDATE [DP01] SET [ID_NUMBER] = N'' WHERE [ID_NUMBER] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [ID_NUMBER] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [ID_NUMBER];
GO

DECLARE @var288 sysname;
SELECT @var288 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'FILE_NAME');
IF @var288 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var288 + '];');
UPDATE [DP01] SET [FILE_NAME] = N'' WHERE [FILE_NAME] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [FILE_NAME] nvarchar(255) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [FILE_NAME];
GO

DECLARE @var289 sysname;
SELECT @var289 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'EMPLOYEE_NUMBER');
IF @var289 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var289 + '];');
UPDATE [DP01] SET [EMPLOYEE_NUMBER] = N'' WHERE [EMPLOYEE_NUMBER] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [EMPLOYEE_NUMBER] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [EMPLOYEE_NUMBER];
GO

DECLARE @var290 sysname;
SELECT @var290 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'EMPLOYEE_NAME');
IF @var290 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var290 + '];');
UPDATE [DP01] SET [EMPLOYEE_NAME] = N'' WHERE [EMPLOYEE_NAME] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [EMPLOYEE_NAME] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [EMPLOYEE_NAME];
GO

DECLARE @var291 sysname;
SELECT @var291 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'DRAMT');
IF @var291 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var291 + '];');
UPDATE [DP01] SET [DRAMT] = N'' WHERE [DRAMT] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [DRAMT] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [DRAMT];
GO

DECLARE @var292 sysname;
SELECT @var292 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'DP_TYPE_NAME');
IF @var292 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var292 + '];');
UPDATE [DP01] SET [DP_TYPE_NAME] = N'' WHERE [DP_TYPE_NAME] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [DP_TYPE_NAME] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [DP_TYPE_NAME];
GO

DECLARE @var293 sysname;
SELECT @var293 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'DP_TYPE_CODE');
IF @var293 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var293 + '];');
UPDATE [DP01] SET [DP_TYPE_CODE] = N'' WHERE [DP_TYPE_CODE] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [DP_TYPE_CODE] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [DP_TYPE_CODE];
GO

DECLARE @var294 sysname;
SELECT @var294 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'CUST_TYPE_NAME');
IF @var294 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var294 + '];');
UPDATE [DP01] SET [CUST_TYPE_NAME] = N'' WHERE [CUST_TYPE_NAME] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [CUST_TYPE_NAME] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [CUST_TYPE_NAME];
GO

DECLARE @var295 sysname;
SELECT @var295 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'CUST_TYPE_DETAIL');
IF @var295 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var295 + '];');
UPDATE [DP01] SET [CUST_TYPE_DETAIL] = N'' WHERE [CUST_TYPE_DETAIL] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [CUST_TYPE_DETAIL] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [CUST_TYPE_DETAIL];
GO

DECLARE @var296 sysname;
SELECT @var296 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'CUST_TYPE');
IF @var296 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var296 + '];');
UPDATE [DP01] SET [CUST_TYPE] = N'' WHERE [CUST_TYPE] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [CUST_TYPE] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [CUST_TYPE];
GO

DECLARE @var297 sysname;
SELECT @var297 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'CUST_DETAIL_NAME');
IF @var297 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var297 + '];');
UPDATE [DP01] SET [CUST_DETAIL_NAME] = N'' WHERE [CUST_DETAIL_NAME] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [CUST_DETAIL_NAME] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [CUST_DETAIL_NAME];
GO

DECLARE @var298 sysname;
SELECT @var298 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'CURRENT_BALANCE');
IF @var298 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var298 + '];');
UPDATE [DP01] SET [CURRENT_BALANCE] = N'' WHERE [CURRENT_BALANCE] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [CURRENT_BALANCE] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [CURRENT_BALANCE];
GO

DECLARE @var299 sysname;
SELECT @var299 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'CRAMT');
IF @var299 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var299 + '];');
UPDATE [DP01] SET [CRAMT] = N'' WHERE [CRAMT] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [CRAMT] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [CRAMT];
GO

DECLARE @var300 sysname;
SELECT @var300 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'COUNTRY_CODE');
IF @var300 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var300 + '];');
UPDATE [DP01] SET [COUNTRY_CODE] = N'' WHERE [COUNTRY_CODE] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [COUNTRY_CODE] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [COUNTRY_CODE];
GO

DECLARE @var301 sysname;
SELECT @var301 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'CONTRACT_COUTS_DAY');
IF @var301 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var301 + '];');
UPDATE [DP01] SET [CONTRACT_COUTS_DAY] = N'' WHERE [CONTRACT_COUTS_DAY] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [CONTRACT_COUTS_DAY] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [CONTRACT_COUTS_DAY];
GO

DECLARE @var302 sysname;
SELECT @var302 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'CLOSE_DATE');
IF @var302 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var302 + '];');
UPDATE [DP01] SET [CLOSE_DATE] = N'' WHERE [CLOSE_DATE] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [CLOSE_DATE] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [CLOSE_DATE];
GO

DECLARE @var303 sysname;
SELECT @var303 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'CCY');
IF @var303 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var303 + '];');
UPDATE [DP01] SET [CCY] = N'' WHERE [CCY] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [CCY] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [CCY];
GO

DECLARE @var304 sysname;
SELECT @var304 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'BIRTH_DATE');
IF @var304 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var304 + '];');
UPDATE [DP01] SET [BIRTH_DATE] = N'' WHERE [BIRTH_DATE] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [BIRTH_DATE] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [BIRTH_DATE];
GO

DECLARE @var305 sysname;
SELECT @var305 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'AUTO_RENEWAL');
IF @var305 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var305 + '];');
UPDATE [DP01] SET [AUTO_RENEWAL] = N'' WHERE [AUTO_RENEWAL] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [AUTO_RENEWAL] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [AUTO_RENEWAL];
GO

DECLARE @var306 sysname;
SELECT @var306 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'ADDRESS');
IF @var306 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var306 + '];');
UPDATE [DP01] SET [ADDRESS] = N'' WHERE [ADDRESS] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [ADDRESS] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [ADDRESS];
GO

DECLARE @var307 sysname;
SELECT @var307 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'ACRUAL_AMOUNT_END');
IF @var307 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var307 + '];');
UPDATE [DP01] SET [ACRUAL_AMOUNT_END] = N'' WHERE [ACRUAL_AMOUNT_END] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [ACRUAL_AMOUNT_END] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [ACRUAL_AMOUNT_END];
GO

DECLARE @var308 sysname;
SELECT @var308 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'ACRUAL_AMOUNT');
IF @var308 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var308 + '];');
UPDATE [DP01] SET [ACRUAL_AMOUNT] = N'' WHERE [ACRUAL_AMOUNT] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [ACRUAL_AMOUNT] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [ACRUAL_AMOUNT];
GO

DECLARE @var309 sysname;
SELECT @var309 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'ACCOUNT_STATUS');
IF @var309 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var309 + '];');
UPDATE [DP01] SET [ACCOUNT_STATUS] = N'' WHERE [ACCOUNT_STATUS] IS NULL;
ALTER TABLE [DP01] ALTER COLUMN [ACCOUNT_STATUS] nvarchar(50) NOT NULL;
ALTER TABLE [DP01] ADD DEFAULT N'' FOR [ACCOUNT_STATUS];
GO

DECLARE @var310 sysname;
SELECT @var310 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'Id');
IF @var310 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var310 + '];');
ALTER TABLE [DP01] ALTER COLUMN [Id] int NOT NULL;
GO

DECLARE @var311 sysname;
SELECT @var311 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DP01]') AND [c].[name] = N'UPDATED_DATE');
IF @var311 IS NOT NULL EXEC(N'ALTER TABLE [DP01] DROP CONSTRAINT [' + @var311 + '];');
ALTER TABLE [DP01] ALTER COLUMN [UPDATED_DATE] datetime2 NOT NULL;
GO

ALTER TABLE [DP01] ADD [CREATED_DATE] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250723014337_CreateFresh8DataTables', N'8.0.5');
GO

COMMIT;
GO

