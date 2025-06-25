-- This script fixes issues with temporal tables
-- It ensures that all tables have the correct temporal configuration

USE [TinhKhoanDB];
GO

-- Check if temporal tables are enabled correctly
DECLARE @ImportedDataRecordsIsTemporal BIT = 0;
DECLARE @ImportedDataItemsIsTemporal BIT = 0;

-- Check ImportedDataRecords
SELECT @ImportedDataRecordsIsTemporal = 1
FROM sys.tables t
WHERE t.name = 'ImportedDataRecords' AND t.temporal_type = 2;

-- Check ImportedDataItems
SELECT @ImportedDataItemsIsTemporal = 1
FROM sys.tables t
WHERE t.name = 'ImportedDataItems' AND t.temporal_type = 2;

-- Fix ImportedDataRecords if not temporal
IF @ImportedDataRecordsIsTemporal = 0 AND EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ImportedDataRecords')
BEGIN
    PRINT '⚠️ ImportedDataRecords is not a temporal table. Attempting to fix...';
    
    -- Check if the history table exists
    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ImportedDataRecords_History')
    BEGIN
        -- Add temporal columns if they don't exist
        IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'SysStartTime' AND object_id = OBJECT_ID('ImportedDataRecords'))
        BEGIN
            ALTER TABLE [ImportedDataRecords] ADD [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME();
        END
        
        IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'SysEndTime' AND object_id = OBJECT_ID('ImportedDataRecords'))
        BEGIN
            ALTER TABLE [ImportedDataRecords] ADD [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(datetime2, '9999-12-31 23:59:59.9999999');
        END
        
        -- Add CompressionRatio if it doesn't exist
        IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'CompressionRatio' AND object_id = OBJECT_ID('ImportedDataRecords'))
        BEGIN
            ALTER TABLE [ImportedDataRecords] ADD [CompressionRatio] float NOT NULL DEFAULT 0.0;
        END
        
        -- Enable system versioning
        ALTER TABLE [ImportedDataRecords] 
        ADD PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
        
        ALTER TABLE [ImportedDataRecords]
        SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[ImportedDataRecords_History]));
        
        PRINT '✅ ImportedDataRecords successfully converted to temporal table';
    END
    ELSE
    BEGIN
        PRINT '⚠️ ImportedDataRecords_History exists but system versioning is not enabled. Manual intervention required.';
    END
END
ELSE IF @ImportedDataRecordsIsTemporal = 1
BEGIN
    PRINT '✅ ImportedDataRecords is already a temporal table';
END

-- Fix ImportedDataItems if not temporal
IF @ImportedDataItemsIsTemporal = 0 AND EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ImportedDataItems')
BEGIN
    PRINT '⚠️ ImportedDataItems is not a temporal table. Attempting to fix...';
    
    -- Check if the history table exists
    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ImportedDataItems_History')
    BEGIN
        -- Add temporal columns if they don't exist
        IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'SysStartTime' AND object_id = OBJECT_ID('ImportedDataItems'))
        BEGIN
            ALTER TABLE [ImportedDataItems] ADD [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME();
        END
        
        IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'SysEndTime' AND object_id = OBJECT_ID('ImportedDataItems'))
        BEGIN
            ALTER TABLE [ImportedDataItems] ADD [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(datetime2, '9999-12-31 23:59:59.9999999');
        END
        
        -- Enable system versioning
        ALTER TABLE [ImportedDataItems] 
        ADD PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
        
        ALTER TABLE [ImportedDataItems]
        SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[ImportedDataItems_History]));
        
        PRINT '✅ ImportedDataItems successfully converted to temporal table';
    END
    ELSE
    BEGIN
        PRINT '⚠️ ImportedDataItems_History exists but system versioning is not enabled. Manual intervention required.';
    END
END
ELSE IF @ImportedDataItemsIsTemporal = 1
BEGIN
    PRINT '✅ ImportedDataItems is already a temporal table';
END

-- Create columnstore index if it doesn't exist
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ImportedDataItems_Columnstore' AND object_id = OBJECT_ID('ImportedDataItems'))
BEGIN
    PRINT '📊 Creating columnstore index for analytics performance...';
    
    CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_ImportedDataItems_Columnstore]
    ON [ImportedDataItems] ([ImportedDataRecordId], [ProcessedDate])
    WHERE [ProcessedDate] >= '2024-01-01';
    
    PRINT '✅ Columnstore index created successfully';
END
ELSE
BEGIN
    PRINT '✅ Columnstore index already exists';
END

-- Create missing indexes if they don't exist
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ImportedDataRecords_StatementDate' AND object_id = OBJECT_ID('ImportedDataRecords'))
BEGIN
    CREATE INDEX [IX_ImportedDataRecords_StatementDate] ON [ImportedDataRecords] ([StatementDate]);
    PRINT '✅ Created missing index IX_ImportedDataRecords_StatementDate';
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ImportedDataRecords_Status' AND object_id = OBJECT_ID('ImportedDataRecords'))
BEGIN
    CREATE INDEX [IX_ImportedDataRecords_Status] ON [ImportedDataRecords] ([Status]);
    PRINT '✅ Created missing index IX_ImportedDataRecords_Status';
END

-- Ensure EF Migrations History is updated correctly
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250624074404_AddMissingImportedDataColumns')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250624074404_AddMissingImportedDataColumns', '8.0.5');
    PRINT '✅ Updated migration history for AddMissingImportedDataColumns';
END

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250624161307_FixCompressionRatioDataType')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250624161307_FixCompressionRatioDataType', '8.0.5');
    PRINT '✅ Updated migration history for FixCompressionRatioDataType';
END

PRINT '✅ Database fix script completed successfully';
GO
