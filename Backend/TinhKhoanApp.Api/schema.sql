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


DECLARE @historyTableSchema sysname = SCHEMA_NAME()
EXEC(N'CREATE TABLE [DP01] (
    [Id] int NOT NULL IDENTITY,
    [NGAY_DL] nvarchar(10) NOT NULL,
    [CREATED_DATE] datetime2 NOT NULL,
    [UPDATED_DATE] datetime2 NULL,
    [FILE_NAME] nvarchar(255) NULL,
    [DATA_DATE] datetime2 NULL,
    [MA_CN] nvarchar(50) NULL,
    [TAI_KHOAN_HACH_TOAN] nvarchar(50) NULL,
    [MA_KH] nvarchar(50) NULL,
    [TEN_KH] nvarchar(255) NULL,
    [DP_TYPE_NAME] nvarchar(255) NULL,
    [CCY] nvarchar(10) NULL,
    [CURRENT_BALANCE] decimal(18,2) NULL,
    [RATE] decimal(18,6) NULL,
    [SO_TAI_KHOAN] nvarchar(50) NULL,
    [OPENING_DATE] nvarchar(20) NULL,
    [MATURITY_DATE] nvarchar(20) NULL,
    [ADDRESS] nvarchar(500) NULL,
    [NOTENO] nvarchar(50) NULL,
    [MONTH_TERM] int NULL,
    [TERM_DP_NAME] nvarchar(255) NULL,
    [TIME_DP_NAME] nvarchar(255) NULL,
    [MA_PGD] nvarchar(50) NULL,
    [TEN_PGD] nvarchar(255) NULL,
    [DP_TYPE_CODE] nvarchar(50) NULL,
    [RENEW_DATE] nvarchar(20) NULL,
    [CUST_TYPE] nvarchar(50) NULL,
    [CUST_TYPE_NAME] nvarchar(255) NULL,
    [CUST_TYPE_DETAIL] nvarchar(50) NULL,
    [CUST_DETAIL_NAME] nvarchar(255) NULL,
    [PREVIOUS_DP_CAP_DATE] nvarchar(20) NULL,
    [NEXT_DP_CAP_DATE] nvarchar(20) NULL,
    [ID_NUMBER] nvarchar(50) NULL,
    [ISSUED_BY] nvarchar(255) NULL,
    [ISSUE_DATE] nvarchar(20) NULL,
    [SEX_TYPE] nvarchar(20) NULL,
    [BIRTH_DATE] nvarchar(20) NULL,
    [TELEPHONE] nvarchar(50) NULL,
    [ACRUAL_AMOUNT] decimal(18,2) NULL,
    [ACRUAL_AMOUNT_END] decimal(18,2) NULL,
    [ACCOUNT_STATUS] nvarchar(50) NULL,
    [DRAMT] decimal(18,2) NULL,
    [CRAMT] decimal(18,2) NULL,
    [EMPLOYEE_NUMBER] nvarchar(50) NULL,
    [EMPLOYEE_NAME] nvarchar(255) NULL,
    [SPECIAL_RATE] decimal(18,6) NULL,
    [AUTO_RENEWAL] int NULL,
    [CLOSE_DATE] nvarchar(20) NULL,
    [LOCAL_PROVIN_NAME] nvarchar(255) NULL,
    [LOCAL_DISTRICT_NAME] nvarchar(255) NULL,
    [LOCAL_WARD_NAME] nvarchar(255) NULL,
    [TERM_DP_TYPE] nvarchar(50) NULL,
    [TIME_DP_TYPE] nvarchar(50) NULL,
    [STATES_CODE] nvarchar(50) NULL,
    [ZIP_CODE] nvarchar(20) NULL,
    [COUNTRY_CODE] nvarchar(10) NULL,
    [TAX_CODE_LOCATION] nvarchar(255) NULL,
    [MA_CAN_BO_PT] nvarchar(50) NULL,
    [TEN_CAN_BO_PT] nvarchar(255) NULL,
    [PHONG_CAN_BO_PT] nvarchar(255) NULL,
    [NGUOI_NUOC_NGOAI] nvarchar(10) NULL,
    [QUOC_TICH] nvarchar(100) NULL,
    [MA_CAN_BO_AGRIBANK] nvarchar(50) NULL,
    [NGUOI_GIOI_THIEU] nvarchar(50) NULL,
    [TEN_NGUOI_GIOI_THIEU] nvarchar(255) NULL,
    [CONTRACT_COUTS_DAY] int NULL,
    [SO_KY_AD_LSDB] int NULL,
    [UNTBUSCD] nvarchar(50) NULL,
    [TYGIA] decimal(18,6) NULL,
    [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    CONSTRAINT [PK_DP01] PRIMARY KEY ([Id]),
    PERIOD FOR SYSTEM_TIME([SysStartTime], [SysEndTime])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + N'].[DP01_History]))');
GO


DECLARE @historyTableSchema sysname = SCHEMA_NAME()
EXEC(N'CREATE TABLE [DPDA] (
    [Id] int NOT NULL IDENTITY,
    [NGAY_DL] nvarchar(10) NOT NULL,
    [MA_CHI_NHANH] nvarchar(50) NULL,
    [MA_KHACH_HANG] nvarchar(50) NULL,
    [TEN_KHACH_HANG] nvarchar(255) NULL,
    [SO_TAI_KHOAN] nvarchar(50) NULL,
    [LOAI_THE] nvarchar(50) NULL,
    [SO_THE] nvarchar(50) NULL,
    [NGAY_NOP_DON] nvarchar(20) NULL,
    [NGAY_PHAT_HANH] nvarchar(20) NULL,
    [USER_PHAT_HANH] nvarchar(100) NULL,
    [TRANG_THAI] nvarchar(50) NULL,
    [PHAN_LOAI] nvarchar(50) NULL,
    [GIAO_THE] nvarchar(50) NULL,
    [LOAI_PHAT_HANH] nvarchar(50) NULL,
    [CREATED_DATE] datetime2 NOT NULL,
    [UPDATED_DATE] datetime2 NULL,
    [FILE_NAME] nvarchar(255) NULL,
    [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    CONSTRAINT [PK_DPDA] PRIMARY KEY ([Id]),
    PERIOD FOR SYSTEM_TIME([SysStartTime], [SysEndTime])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + N'].[DPDA_History]))');
GO


CREATE TABLE [DPDA_History] (
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
    CONSTRAINT [PK_DPDA_History] PRIMARY KEY ([Id])
);
GO


DECLARE @historyTableSchema sysname = SCHEMA_NAME()
EXEC(N'CREATE TABLE [EI01] (
    [Id] int NOT NULL IDENTITY,
    [NGAY_DL] nvarchar(10) NOT NULL,
    [MA_CN] nvarchar(50) NULL,
    [MA_KH] nvarchar(50) NULL,
    [TEN_KH] nvarchar(255) NULL,
    [LOAI_KH] nvarchar(50) NULL,
    [SDT_EMB] nvarchar(20) NULL,
    [TRANG_THAI_EMB] nvarchar(50) NULL,
    [NGAY_DK_EMB] nvarchar(20) NULL,
    [SDT_OTT] nvarchar(20) NULL,
    [TRANG_THAI_OTT] nvarchar(50) NULL,
    [NGAY_DK_OTT] nvarchar(20) NULL,
    [SDT_SMS] nvarchar(20) NULL,
    [TRANG_THAI_SMS] nvarchar(50) NULL,
    [NGAY_DK_SMS] nvarchar(20) NULL,
    [SDT_SAV] nvarchar(20) NULL,
    [TRANG_THAI_SAV] nvarchar(50) NULL,
    [NGAY_DK_SAV] nvarchar(20) NULL,
    [SDT_LN] nvarchar(20) NULL,
    [TRANG_THAI_LN] nvarchar(50) NULL,
    [NGAY_DK_LN] nvarchar(20) NULL,
    [USER_EMB] nvarchar(100) NULL,
    [USER_OTT] nvarchar(100) NULL,
    [USER_SMS] nvarchar(100) NULL,
    [USER_SAV] nvarchar(100) NULL,
    [USER_LN] nvarchar(100) NULL,
    [CREATED_DATE] datetime2 NOT NULL,
    [UPDATED_DATE] datetime2 NULL,
    [FILE_NAME] nvarchar(255) NULL,
    [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    CONSTRAINT [PK_EI01] PRIMARY KEY ([Id]),
    PERIOD FOR SYSTEM_TIME([SysStartTime], [SysEndTime])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + N'].[EI01_History]))');
GO


CREATE TABLE [EI01_History] (
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
    CONSTRAINT [PK_EI01_History] PRIMARY KEY ([Id])
);
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


DECLARE @historyTableSchema sysname = SCHEMA_NAME()
EXEC(N'CREATE TABLE [GL01] (
    [Id] int NOT NULL IDENTITY,
    [NGAY_DL] nvarchar(10) NOT NULL,
    [STS] nvarchar(20) NULL,
    [NGAY_GD] nvarchar(20) NULL,
    [NGUOI_TAO] nvarchar(100) NULL,
    [DYSEQ] nvarchar(50) NULL,
    [TR_TYPE] nvarchar(20) NULL,
    [DT_SEQ] nvarchar(50) NULL,
    [TAI_KHOAN] nvarchar(50) NULL,
    [TEN_TK] nvarchar(255) NULL,
    [SO_TIEN_GD] decimal(18,2) NULL,
    [POST_BR] nvarchar(20) NULL,
    [LOAI_TIEN] nvarchar(10) NULL,
    [DR_CR] nvarchar(10) NULL,
    [MA_KH] nvarchar(50) NULL,
    [TEN_KH] nvarchar(255) NULL,
    [CCA_USRID] nvarchar(50) NULL,
    [TR_EX_RT] decimal(18,6) NULL,
    [REMARK] nvarchar(500) NULL,
    [BUS_CODE] nvarchar(20) NULL,
    [UNIT_BUS_CODE] nvarchar(20) NULL,
    [TR_CODE] nvarchar(20) NULL,
    [TR_NAME] nvarchar(255) NULL,
    [REFERENCE] nvarchar(100) NULL,
    [VALUE_DATE] nvarchar(20) NULL,
    [DEPT_CODE] nvarchar(20) NULL,
    [TR_TIME] nvarchar(20) NULL,
    [COMFIRM] nvarchar(20) NULL,
    [TRDT_TIME] nvarchar(20) NULL,
    [CREATED_DATE] datetime2 NOT NULL,
    [UPDATED_DATE] datetime2 NULL,
    [FILE_NAME] nvarchar(255) NULL,
    [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    CONSTRAINT [PK_GL01] PRIMARY KEY ([Id]),
    PERIOD FOR SYSTEM_TIME([SysStartTime], [SysEndTime])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + N'].[GL01_History]))');
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


CREATE TABLE [GL41] (
    [Id] int NOT NULL IDENTITY,
    [NGAY_DL] nvarchar(10) NOT NULL,
    [CREATED_DATE] datetime2 NOT NULL,
    [UPDATED_DATE] datetime2 NULL,
    [FILE_NAME] nvarchar(255) NULL,
    [MA_CN] nvarchar(50) NULL,
    [LOAI_TIEN] nvarchar(10) NULL,
    [MA_TK] nvarchar(50) NULL,
    [TEN_TK] nvarchar(255) NULL,
    [LOAI_BT] nvarchar(10) NULL,
    [DN_DAUKY] decimal(18,2) NULL,
    [DC_DAUKY] decimal(18,2) NULL,
    [SBT_NO] decimal(18,2) NULL,
    [ST_GHINO] decimal(18,2) NULL,
    [SBT_CO] decimal(18,2) NULL,
    [ST_GHICO] decimal(18,2) NULL,
    [DN_CUOIKY] decimal(18,2) NULL,
    [DC_CUOIKY] decimal(18,2) NULL,
    CONSTRAINT [PK_GL41] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [GL41_History] (
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
    CONSTRAINT [PK_GL41_History] PRIMARY KEY ([Id])
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


DECLARE @historyTableSchema sysname = SCHEMA_NAME()
EXEC(N'CREATE TABLE [LN01] (
    [Id] int NOT NULL IDENTITY,
    [NGAY_DL] nvarchar(10) NOT NULL,
    [BRCD] nvarchar(50) NULL,
    [CUSTSEQ] nvarchar(50) NULL,
    [CUSTNM] nvarchar(255) NULL,
    [TAI_KHOAN] nvarchar(50) NULL,
    [CCY] nvarchar(10) NULL,
    [DU_NO] decimal(18,2) NULL,
    [DSBSSEQ] nvarchar(50) NULL,
    [TRANSACTION_DATE] nvarchar(20) NULL,
    [DSBSDT] nvarchar(20) NULL,
    [DISBUR_CCY] nvarchar(10) NULL,
    [DISBURSEMENT_AMOUNT] decimal(18,2) NULL,
    [DSBSMATDT] nvarchar(20) NULL,
    [BSRTCD] nvarchar(50) NULL,
    [INTEREST_RATE] decimal(18,2) NULL,
    [APPRSEQ] nvarchar(50) NULL,
    [APPRDT] nvarchar(20) NULL,
    [APPR_CCY] nvarchar(10) NULL,
    [APPRAMT] decimal(18,2) NULL,
    [APPRMATDT] nvarchar(20) NULL,
    [LOAN_TYPE] nvarchar(50) NULL,
    [FUND_RESOURCE_CODE] nvarchar(50) NULL,
    [FUND_PURPOSE_CODE] nvarchar(50) NULL,
    [REPAYMENT_AMOUNT] decimal(18,2) NULL,
    [NEXT_REPAY_DATE] nvarchar(20) NULL,
    [NEXT_REPAY_AMOUNT] decimal(18,2) NULL,
    [NEXT_INT_REPAY_DATE] nvarchar(20) NULL,
    [OFFICER_ID] nvarchar(50) NULL,
    [OFFICER_NAME] nvarchar(255) NULL,
    [INTEREST_AMOUNT] decimal(18,2) NULL,
    [PASTDUE_INTEREST_AMOUNT] decimal(18,2) NULL,
    [TOTAL_INTEREST_REPAY_AMOUNT] decimal(18,2) NULL,
    [CUSTOMER_TYPE_CODE] nvarchar(50) NULL,
    [CUSTOMER_TYPE_CODE_DETAIL] nvarchar(50) NULL,
    [TRCTCD] nvarchar(50) NULL,
    [TRCTNM] nvarchar(255) NULL,
    [ADDR1] nvarchar(500) NULL,
    [PROVINCE] nvarchar(255) NULL,
    [LCLPROVINNM] nvarchar(255) NULL,
    [DISTRICT] nvarchar(255) NULL,
    [LCLDISTNM] nvarchar(255) NULL,
    [COMMCD] nvarchar(50) NULL,
    [LCLWARDNM] nvarchar(255) NULL,
    [LAST_REPAY_DATE] nvarchar(20) NULL,
    [SECURED_PERCENT] decimal(18,2) NULL,
    [NHOM_NO] nvarchar(20) NULL,
    [LAST_INT_CHARGE_DATE] nvarchar(20) NULL,
    [EXEMPTINT] decimal(18,2) NULL,
    [EXEMPTINTTYPE] nvarchar(50) NULL,
    [EXEMPTINTAMT] decimal(18,2) NULL,
    [GRPNO] nvarchar(50) NULL,
    [BUSCD] nvarchar(50) NULL,
    [BSNSSCLTPCD] nvarchar(50) NULL,
    [USRIDOP] nvarchar(50) NULL,
    [ACCRUAL_AMOUNT] decimal(18,2) NULL,
    [ACCRUAL_AMOUNT_END_OF_MONTH] decimal(18,2) NULL,
    [INTCMTH] decimal(18,2) NULL,
    [INTRPYMTH] decimal(18,2) NULL,
    [INTTRMMTH] int NULL,
    [YRDAYS] int NULL,
    [REMARK] nvarchar(500) NULL,
    [CHITIEU] nvarchar(255) NULL,
    [CTCV] nvarchar(255) NULL,
    [CREDIT_LINE_YPE] nvarchar(50) NULL,
    [INT_LUMPSUM_PARTIAL_TYPE] nvarchar(50) NULL,
    [INT_PARTIAL_PAYMENT_TYPE] nvarchar(50) NULL,
    [INT_PAYMENT_INTERVAL] int NULL,
    [AN_HAN_LAI] nvarchar(50) NULL,
    [PHUONG_THUC_GIAI_NGAN_1] nvarchar(255) NULL,
    [TAI_KHOAN_GIAI_NGAN_1] nvarchar(50) NULL,
    [SO_TIEN_GIAI_NGAN_1] decimal(18,2) NULL,
    [PHUONG_THUC_GIAI_NGAN_2] nvarchar(255) NULL,
    [TAI_KHOAN_GIAI_NGAN_2] nvarchar(50) NULL,
    [SO_TIEN_GIAI_NGAN_2] decimal(18,2) NULL,
    [CMT_HC] nvarchar(50) NULL,
    [NGAY_SINH] nvarchar(20) NULL,
    [MA_CB_AGRI] nvarchar(50) NULL,
    [MA_NGANH_KT] nvarchar(50) NULL,
    [TY_GIA] decimal(18,2) NULL,
    [OFFICER_IPCAS] nvarchar(50) NULL,
    [CREATED_DATE] datetime2 NOT NULL,
    [UPDATED_DATE] datetime2 NULL,
    [FILE_NAME] nvarchar(255) NULL,
    [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    CONSTRAINT [PK_LN01] PRIMARY KEY ([Id]),
    PERIOD FOR SYSTEM_TIME([SysStartTime], [SysEndTime])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + N'].[LN01_History]))');
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


DECLARE @historyTableSchema sysname = SCHEMA_NAME()
EXEC(N'CREATE TABLE [LN03] (
    [Id] int NOT NULL IDENTITY,
    [NGAY_DL] nvarchar(10) NOT NULL,
    [MACHINHANH] nvarchar(50) NULL,
    [TENCHINHANH] nvarchar(255) NULL,
    [MAKH] nvarchar(50) NULL,
    [TENKH] nvarchar(255) NULL,
    [SOHOPDONG] nvarchar(50) NULL,
    [SOTIENXLRR] decimal(18,2) NULL,
    [NGAYPHATSINHXL] nvarchar(20) NULL,
    [THUNOSAUXL] decimal(18,2) NULL,
    [CONLAINGOAIBANG] decimal(18,2) NULL,
    [DUNONOIBANG] decimal(18,2) NULL,
    [NHOMNO] nvarchar(20) NULL,
    [MACBTD] nvarchar(50) NULL,
    [TENCBTD] nvarchar(255) NULL,
    [MAPGD] nvarchar(50) NULL,
    [TAIKHOANHACHTOAN] nvarchar(50) NULL,
    [REFNO] nvarchar(50) NULL,
    [LOAINGUONVON] nvarchar(50) NULL,
    [R] nvarchar(50) NULL,
    [S] nvarchar(50) NULL,
    [T] decimal(18,2) NULL,
    [CREATED_DATE] datetime2 NOT NULL,
    [UPDATED_DATE] datetime2 NULL,
    [FILE_NAME] nvarchar(255) NULL,
    [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    CONSTRAINT [PK_LN03] PRIMARY KEY ([Id]),
    PERIOD FOR SYSTEM_TIME([SysStartTime], [SysEndTime])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + N'].[LN03_History]))');
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


DECLARE @historyTableSchema sysname = SCHEMA_NAME()
EXEC(N'CREATE TABLE [RR01] (
    [Id] int NOT NULL IDENTITY,
    [NGAY_DL] nvarchar(10) NOT NULL,
    [CN_LOAI_I] nvarchar(50) NULL,
    [BRCD] nvarchar(50) NULL,
    [MA_KH] nvarchar(50) NULL,
    [TEN_KH] nvarchar(255) NULL,
    [SO_LDS] nvarchar(50) NULL,
    [CCY] nvarchar(10) NULL,
    [SO_LAV] nvarchar(50) NULL,
    [LOAI_KH] nvarchar(50) NULL,
    [NGAY_GIAI_NGAN] nvarchar(20) NULL,
    [NGAY_DEN_HAN] nvarchar(20) NULL,
    [VAMC_FLG] nvarchar(10) NULL,
    [NGAY_XLRR] nvarchar(20) NULL,
    [DUNO_GOC_BAN_DAU] decimal(18,2) NULL,
    [DUNO_LAI_TICHLUY_BD] decimal(18,2) NULL,
    [DOC_DAUKY_DA_THU_HT] decimal(18,2) NULL,
    [DUNO_GOC_HIENTAI] decimal(18,2) NULL,
    [DUNO_LAI_HIENTAI] decimal(18,2) NULL,
    [DUNO_NGAN_HAN] decimal(18,2) NULL,
    [DUNO_TRUNG_HAN] decimal(18,2) NULL,
    [DUNO_DAI_HAN] decimal(18,2) NULL,
    [THU_GOC] decimal(18,2) NULL,
    [THU_LAI] decimal(18,2) NULL,
    [BDS] decimal(18,2) NULL,
    [DS] decimal(18,2) NULL,
    [TSK] decimal(18,2) NULL,
    [CREATED_DATE] datetime2 NOT NULL,
    [UPDATED_DATE] datetime2 NULL,
    [FILE_NAME] nvarchar(255) NULL,
    [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    CONSTRAINT [PK_RR01] PRIMARY KEY ([Id]),
    PERIOD FOR SYSTEM_TIME([SysStartTime], [SysEndTime])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + N'].[RR01_History]))');
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
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
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


CREATE INDEX [IX_DP01_MaCN] ON [DP01] ([MA_CN]);
GO


CREATE INDEX [IX_DP01_MaPGD] ON [DP01] ([MA_PGD]);
GO


CREATE INDEX [IX_DP01_NgayDL] ON [DP01] ([NGAY_DL]);
GO


CREATE INDEX [IX_DP01_NgayDL_MaCN] ON [DP01] ([NGAY_DL], [MA_CN]);
GO


CREATE INDEX [IX_DPDA_NgayDL] ON [DPDA] ([NGAY_DL]);
GO


CREATE INDEX [IX_EI01_MaCN] ON [EI01] ([MA_CN]);
GO


CREATE INDEX [IX_EI01_NgayDL] ON [EI01] ([NGAY_DL]);
GO


CREATE INDEX [IX_EI01_NgayDL_MaCN] ON [EI01] ([NGAY_DL], [MA_CN]);
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


CREATE INDEX [IX_GL01_NgayDL] ON [GL01] ([NGAY_DL]);
GO


CREATE INDEX [IX_ImportedDataRecords_Category_ImportDate] ON [ImportedDataRecords] ([Category], [ImportDate]);
GO


CREATE INDEX [IX_ImportedDataRecords_ImportDate] ON [ImportedDataRecords] ([ImportDate]);
GO


CREATE INDEX [IX_ImportedDataRecords_StatementDate] ON [ImportedDataRecords] ([StatementDate]);
GO


CREATE INDEX [IX_ImportedDataRecords_Status] ON [ImportedDataRecords] ([Status]);
GO


CREATE INDEX [IX_KpiIndicators_TableId] ON [KpiIndicators] ([TableId]);
GO


CREATE INDEX [IX_KpiScoringRules_IndicatorName] ON [KpiScoringRules] ([KpiIndicatorName]);
GO


CREATE INDEX [IX_LN01_NgayDL] ON [LN01] ([NGAY_DL]);
GO


CREATE INDEX [IX_LN03_NgayDL] ON [LN03] ([NGAY_DL]);
GO


CREATE INDEX [IX_MSIT72_TSBD_ImportedDataRecordId] ON [MSIT72_TSBD] ([ImportedDataRecordId]);
GO


CREATE INDEX [IX_MSIT72_TSGH_ImportedDataRecordId] ON [MSIT72_TSGH] ([ImportedDataRecordId]);
GO


CREATE INDEX [IX_RawDataRecords_RawDataImportId] ON [RawDataRecords] ([RawDataImportId]);
GO


CREATE INDEX [IX_RR01_NgayDL] ON [RR01] ([NGAY_DL]);
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


