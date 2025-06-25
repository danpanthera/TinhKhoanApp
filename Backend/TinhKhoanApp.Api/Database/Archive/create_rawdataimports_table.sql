-- 🏗️ Tạo bảng RawDataImports với Temporal Tables và Columnstore Indexes
-- Dành cho hiệu năng cao khi xử lý 100K-200K records/ngày

-- Tạo bảng chính RawDataImports với System Versioning
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RawDataImports]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RawDataImports] (
        [Id] bigint NOT NULL IDENTITY(1,1),
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
        [CreatedDate] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [CreatedBy] nvarchar(100) NOT NULL DEFAULT ('SYSTEM'),
        [LastModifiedDate] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [LastModifiedBy] nvarchar(100) NOT NULL DEFAULT ('SYSTEM'),
        [IsDeleted] bit NOT NULL DEFAULT (0),
        [ValidFrom] datetime2 GENERATED ALWAYS AS ROW START NOT NULL,
        [ValidTo] datetime2 GENERATED ALWAYS AS ROW END NOT NULL,
        -- Additional compatibility fields
        [KpiName] nvarchar(200) NOT NULL DEFAULT (''),
        [DataType] nvarchar(50) NOT NULL DEFAULT (''),
        [FileName] nvarchar(500) NOT NULL DEFAULT (''),
        PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo]),
        CONSTRAINT [PK_RawDataImports] PRIMARY KEY CLUSTERED ([Id] ASC)
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[RawDataImports_History]));
    
    PRINT '✅ Tạo bảng RawDataImports với System Versioning thành công!';
END
ELSE
BEGIN
    PRINT '⚠️ Bảng RawDataImports đã tồn tại!';
END

-- Tạo Columnstore Index cho hiệu năng cao
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[RawDataImports]') AND name = N'CCI_RawDataImports')
BEGIN
    CREATE NONCLUSTERED COLUMNSTORE INDEX [CCI_RawDataImports] 
    ON [dbo].[RawDataImports] (
        [ImportDate], [BranchCode], [DepartmentCode], [EmployeeCode], 
        [KpiCode], [KpiValue], [Target], [Achievement], [Score], 
        [ImportBatchId], [CreatedDate]
    );
    
    PRINT '✅ Tạo Columnstore Index cho RawDataImports thành công!';
END
ELSE
BEGIN
    PRINT '⚠️ Columnstore Index cho RawDataImports đã tồn tại!';
END

-- Tạo các Index thông thường để tối ưu truy vấn
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[RawDataImports]') AND name = N'IX_RawDataImports_ImportDate')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_RawDataImports_ImportDate] 
    ON [dbo].[RawDataImports] ([ImportDate] DESC) 
    INCLUDE ([BranchCode], [EmployeeCode], [KpiCode]);
    
    PRINT '✅ Tạo Index theo ImportDate thành công!';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[RawDataImports]') AND name = N'IX_RawDataImports_EmployeeKpi')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_RawDataImports_EmployeeKpi] 
    ON [dbo].[RawDataImports] ([EmployeeCode], [KpiCode]) 
    INCLUDE ([ImportDate], [KpiValue], [Score]);
    
    PRINT '✅ Tạo Index theo EmployeeCode + KpiCode thành công!';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[RawDataImports]') AND name = N'IX_RawDataImports_BatchId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_RawDataImports_BatchId] 
    ON [dbo].[RawDataImports] ([ImportBatchId]) 
    INCLUDE ([FileName], [CreatedDate]);
    
    PRINT '✅ Tạo Index theo ImportBatchId thành công!';
END

PRINT '🎉 Hoàn thành thiết lập bảng RawDataImports với Temporal Tables + Columnstore Indexes!';
