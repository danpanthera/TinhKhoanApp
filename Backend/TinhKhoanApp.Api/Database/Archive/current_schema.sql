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


CREATE TABLE [ImportedDataRecords] (
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
    CONSTRAINT [PK_ImportedDataRecords] PRIMARY KEY ([Id])
);
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


CREATE TABLE [KH03_History] (
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
    CONSTRAINT [PK_KH03_History] PRIMARY KEY ([Id])
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
    [IsArchiveFile] bit NOT NULL,
    [ArchiveType] nvarchar(20) NULL,
    [RequiresPassword] bit NOT NULL,
    [ExtractedFilesCount] int NOT NULL,
    [ExtractedFilesList] nvarchar(2000) NULL,
    CONSTRAINT [PK_LegacyRawDataImports] PRIMARY KEY ([Id])
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
    [Unit] nvarchar(10) NOT NULL,
    [Target] decimal(18,4) NULL,
    [Achievement] decimal(18,4) NULL,
    [Score] decimal(5,2) NULL,
    [Note] nvarchar(500) NOT NULL,
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
    [Id] bigint NOT NULL IDENTITY,
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


CREATE INDEX [IX_ImportedDataItems_RecordId] ON [ImportedDataItems] ([ImportedDataRecordId]);
GO


CREATE INDEX [IX_ImportedDataRecords_Category_ImportDate] ON [ImportedDataRecords] ([Category], [ImportDate]);
GO


CREATE INDEX [IX_ImportedDataRecords_StatementDate] ON [ImportedDataRecords] ([StatementDate]);
GO


CREATE INDEX [IX_ImportedDataRecords_Status] ON [ImportedDataRecords] ([Status]);
GO


CREATE INDEX [IX_KpiIndicators_TableId] ON [KpiIndicators] ([TableId]);
GO


CREATE INDEX [IX_KpiScoringRules_IndicatorName] ON [KpiScoringRules] ([KpiIndicatorName]);
GO


CREATE INDEX [IX_RawDataRecords_RawDataImportId] ON [RawDataRecords] ([RawDataImportId]);
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


