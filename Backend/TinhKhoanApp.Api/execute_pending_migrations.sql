-- This script handles migration issues by executing needed SQL directly
-- It combines the necessary table creations from pending migrations
-- and adds them manually to avoid conflicts with EF migrations

-- First check if the migration history table exists
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[__EFMigrationsHistory]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
    
    PRINT '✅ Created EF Migrations History table';
END

-- Check if DashboardIndicators already exists to avoid conflicts
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DashboardIndicators]') AND type in (N'U'))
BEGIN
    -- Create DashboardIndicators table (from DashboardTablesCreated migration)
    CREATE TABLE [dbo].[DashboardIndicators] (
        [Id] int NOT NULL IDENTITY(1,1),
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
    
    PRINT '✅ Created DashboardIndicators table';
END
ELSE
BEGIN
    PRINT '⚠️ DashboardIndicators table already exists';
END

-- Include the RawDataImports table creation script
-- 🏗️ Tạo bảng RawDataImports với Temporal Tables và Columnstore Indexes
-- Dành cho hiệu năng cao khi xử lý 100K-200K records/ngày

-- Tạo bảng chính RawDataImports với System Versioning
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OptimizedRawDataImports]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[OptimizedRawDataImports] (
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
        CONSTRAINT [PK_OptimizedRawDataImports] PRIMARY KEY CLUSTERED ([Id] ASC)
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[OptimizedRawDataImports_History]));
    
    PRINT '✅ Created OptimizedRawDataImports table with System Versioning';
END
ELSE
BEGIN
    PRINT '⚠️ OptimizedRawDataImports table already exists';
END

-- Tạo Columnstore Index cho hiệu năng cao
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OptimizedRawDataImports]') AND name = N'CCI_OptimizedRawDataImports')
BEGIN
    CREATE NONCLUSTERED COLUMNSTORE INDEX [CCI_OptimizedRawDataImports] 
    ON [dbo].[OptimizedRawDataImports] (
        [ImportDate], [BranchCode], [DepartmentCode], [EmployeeCode], 
        [KpiCode], [KpiValue], [Target], [Achievement], [Score], 
        [ImportBatchId], [CreatedDate]
    );
    
    PRINT '✅ Created Columnstore Index for OptimizedRawDataImports';
END
ELSE
BEGIN
    PRINT '⚠️ Columnstore Index for OptimizedRawDataImports already exists';
END

-- Tạo các Index thông thường để tối ưu truy vấn
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OptimizedRawDataImports]') AND name = N'IX_OptimizedRawDataImports_ImportDate')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_OptimizedRawDataImports_ImportDate] 
    ON [dbo].[OptimizedRawDataImports] ([ImportDate] DESC) 
    INCLUDE ([BranchCode], [EmployeeCode], [KpiCode]);
    
    PRINT '✅ Created ImportDate index for OptimizedRawDataImports';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OptimizedRawDataImports]') AND name = N'IX_OptimizedRawDataImports_EmployeeKpi')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_OptimizedRawDataImports_EmployeeKpi] 
    ON [dbo].[OptimizedRawDataImports] ([EmployeeCode], [KpiCode]) 
    INCLUDE ([ImportDate], [KpiValue], [Score]);
    
    PRINT '✅ Created EmployeeKpi index for OptimizedRawDataImports';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OptimizedRawDataImports]') AND name = N'IX_OptimizedRawDataImports_BatchId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_OptimizedRawDataImports_BatchId] 
    ON [dbo].[OptimizedRawDataImports] ([ImportBatchId]) 
    INCLUDE ([FileName], [CreatedDate]);
    
    PRINT '✅ Created BatchId index for OptimizedRawDataImports';
END

-- Remove columns from ImportedDataRecords (from CreateRawDataImportsTable migration)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ImportedDataRecords]') AND name = N'CompressedData')
BEGIN
    ALTER TABLE [dbo].[ImportedDataRecords] DROP COLUMN [CompressedData];
    PRINT '✅ Dropped CompressedData column from ImportedDataRecords';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ImportedDataRecords]') AND name = N'CompressionRatio')
BEGIN
    ALTER TABLE [dbo].[ImportedDataRecords] DROP COLUMN [CompressionRatio];
    PRINT '✅ Dropped CompressionRatio column from ImportedDataRecords';
END

-- Mark migrations as applied in EF Migrations History
-- Only insert if the migration isn't already there
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250621132903_DashboardTablesCreated')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250621132903_DashboardTablesCreated', N'8.0.1');
    PRINT '✅ Marked DashboardTablesCreated migration as applied';
END

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250623140553_CreateRawDataImportsTable')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250623140553_CreateRawDataImportsTable', N'8.0.1');
    PRINT '✅ Marked CreateRawDataImportsTable migration as applied';
END

PRINT '🎉 All pending migrations have been applied manually';
