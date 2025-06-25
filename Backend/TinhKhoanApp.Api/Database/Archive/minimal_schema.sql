-- Create database
CREATE DATABASE [TinhKhoanDB];
GO

USE [TinhKhoanDB];
GO

-- Create ImportedDataRecords table with System Versioning (Temporal Table)
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
    [CompressedData] varbinary(max) NULL,
    [CompressionRatio] float NOT NULL DEFAULT 0.0,
    [Notes] nvarchar(1000) NULL,
    [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME(),
    [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(datetime2, '9999-12-31 23:59:59.9999999'),
    CONSTRAINT [PK_ImportedDataRecords] PRIMARY KEY ([Id]),
    PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[ImportedDataRecords_History]));
GO

-- Create ImportedDataItems table with System Versioning (Temporal Table)
CREATE TABLE [ImportedDataItems] (
    [Id] bigint NOT NULL IDENTITY,
    [ImportedDataRecordId] int NOT NULL,
    [RawData] nvarchar(max) NOT NULL,
    [ProcessedDate] datetime2 NOT NULL,
    [ProcessingNotes] nvarchar(1000) NULL,
    [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME(),
    [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(datetime2, '9999-12-31 23:59:59.9999999'),
    CONSTRAINT [PK_ImportedDataItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ImportedDataItems_ImportedDataRecords_ImportedDataRecordId] FOREIGN KEY ([ImportedDataRecordId]) REFERENCES [ImportedDataRecords] ([Id]) ON DELETE CASCADE,
    PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[ImportedDataItems_History]));
GO

-- Create indexes for performance
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

-- Create entry in __EFMigrationsHistory for completed migrations
CREATE TABLE [__EFMigrationsHistory] (
    [MigrationId] nvarchar(150) NOT NULL,
    [ProductVersion] nvarchar(32) NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
);
GO

-- Insert completed migrations to avoid EF Core trying to apply them again
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES 
('20250621132903_DashboardTablesCreated', '8.0.5'),
('20250623140553_CreateRawDataImportsTable', '8.0.5'),
('20250624074404_AddMissingImportedDataColumns', '8.0.5'),
('20250624161307_FixCompressionRatioDataType', '8.0.5');
GO

-- Create a columnstore index for analytics performance
CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_ImportedDataItems_Columnstore]
ON [ImportedDataItems] ([ImportedDataRecordId], [ProcessedDate])
WHERE [ProcessedDate] >= '2024-01-01';
GO

PRINT 'Database created successfully with Temporal Tables and all migrations applied';
