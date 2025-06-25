-- Script sửa chữa temporal tables và tối ưu hiệu năng
-- Đảm bảo tất cả bảng có cấu hình temporal đúng cách

USE [TinhKhoanDB];
GO

PRINT '🔧 Bắt đầu sửa chữa temporal tables...';

-- Sửa chữa bảng ImportedDataRecords
PRINT '📊 Kiểm tra và sửa chữa ImportedDataRecords...';

IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ImportedDataRecords')
BEGIN
    -- Tắt temporal nếu đang có vấn đề
    IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ImportedDataRecords_History')
    BEGIN
        BEGIN TRY
            ALTER TABLE [ImportedDataRecords] SET (SYSTEM_VERSIONING = OFF);
            DROP TABLE [ImportedDataRecords_History];
            PRINT '✅ Đã tắt temporal cũ có vấn đề';
        END TRY
        BEGIN CATCH
            PRINT '⚠️ Không thể tắt temporal: ' + ERROR_MESSAGE();
        END CATCH
    END
    
    -- Thêm cột temporal nếu chưa có
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'SysStartTime' AND object_id = OBJECT_ID('ImportedDataRecords'))
    BEGIN
        ALTER TABLE [ImportedDataRecords] ADD 
            [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME(),
            [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(datetime2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
        PRINT '✅ Đã thêm cột temporal cho ImportedDataRecords';
    END
    
    -- Thêm cột CompressionRatio nếu chưa có
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'CompressionRatio' AND object_id = OBJECT_ID('ImportedDataRecords'))
    BEGIN
        ALTER TABLE [ImportedDataRecords] ADD [CompressionRatio] float NOT NULL DEFAULT 0.0;
        PRINT '✅ Đã thêm cột CompressionRatio';
    END
    
    -- Bật temporal versioning
    BEGIN TRY
        ALTER TABLE [ImportedDataRecords] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[ImportedDataRecords_History]));
        PRINT '✅ Đã bật temporal versioning cho ImportedDataRecords';
    END TRY
    BEGIN CATCH
        PRINT '⚠️ Không thể bật temporal versioning: ' + ERROR_MESSAGE();
    END CATCH
END

-- Sửa chữa bảng ImportedDataItems  
PRINT '📊 Kiểm tra và sửa chữa ImportedDataItems...';

IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ImportedDataItems')
BEGIN
    -- Tắt temporal nếu đang có vấn đề
    IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ImportedDataItems_History')
    BEGIN
        BEGIN TRY
            ALTER TABLE [ImportedDataItems] SET (SYSTEM_VERSIONING = OFF);
            DROP TABLE [ImportedDataItems_History];
            PRINT '✅ Đã tắt temporal cũ có vấn đề';
        END TRY
        BEGIN CATCH
            PRINT '⚠️ Không thể tắt temporal: ' + ERROR_MESSAGE();
        END CATCH
    END
    
    -- Thêm cột temporal nếu chưa có
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'SysStartTime' AND object_id = OBJECT_ID('ImportedDataItems'))
    BEGIN
        ALTER TABLE [ImportedDataItems] ADD 
            [SysStartTime] datetime2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME(),
            [SysEndTime] datetime2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(datetime2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
        PRINT '✅ Đã thêm cột temporal cho ImportedDataItems';
    END
    
    -- Bật temporal versioning
    BEGIN TRY
        ALTER TABLE [ImportedDataItems] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[ImportedDataItems_History]));
        PRINT '✅ Đã bật temporal versioning cho ImportedDataItems';
    END TRY
    BEGIN CATCH
        PRINT '⚠️ Không thể bật temporal versioning: ' + ERROR_MESSAGE();
    END CATCH
END

-- Tạo indexes cho hiệu năng
PRINT '📈 Tạo indexes tối ưu hiệu năng...';

-- Index cho ImportedDataRecords
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ImportedDataRecords_StatementDate' AND object_id = OBJECT_ID('ImportedDataRecords'))
BEGIN
    CREATE INDEX [IX_ImportedDataRecords_StatementDate] ON [ImportedDataRecords] ([StatementDate]);
    PRINT '✅ Đã tạo index StatementDate cho ImportedDataRecords';
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ImportedDataRecords_Category_ImportDate' AND object_id = OBJECT_ID('ImportedDataRecords'))
BEGIN
    CREATE INDEX [IX_ImportedDataRecords_Category_ImportDate] ON [ImportedDataRecords] ([Category], [ImportDate]);
    PRINT '✅ Đã tạo index Category_ImportDate cho ImportedDataRecords';
END

-- Index cho ImportedDataItems
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ImportedDataItems_ProcessedDate' AND object_id = OBJECT_ID('ImportedDataItems'))
BEGIN
    CREATE INDEX [IX_ImportedDataItems_ProcessedDate] ON [ImportedDataItems] ([ProcessedDate]);
    PRINT '✅ Đã tạo index ProcessedDate cho ImportedDataItems';
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ImportedDataItems_RecordId' AND object_id = OBJECT_ID('ImportedDataItems'))
BEGIN
    CREATE INDEX [IX_ImportedDataItems_RecordId] ON [ImportedDataItems] ([ImportedDataRecordId]);
    PRINT '✅ Đã tạo index RecordId cho ImportedDataItems';
END

-- Tạo Columnstore Index cho analytics (chỉ nếu có dữ liệu)
IF EXISTS (SELECT 1 FROM [ImportedDataItems]) AND 
   NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ImportedDataItems_Columnstore' AND object_id = OBJECT_ID('ImportedDataItems'))
BEGIN
    BEGIN TRY
        CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_ImportedDataItems_Columnstore]
        ON [ImportedDataItems] ([ImportedDataRecordId], [ProcessedDate])
        WHERE [ProcessedDate] >= '2024-01-01';
        PRINT '✅ Đã tạo Columnstore Index cho analytics';
    END TRY
    BEGIN CATCH
        PRINT '⚠️ Không thể tạo Columnstore Index: ' + ERROR_MESSAGE();
    END CATCH
END

-- Kiểm tra kết quả cuối cùng
PRINT '🔍 Kiểm tra trạng thái temporal tables...';

SELECT 
    t.name AS TableName,
    CASE t.temporal_type 
        WHEN 0 THEN 'Non-temporal'
        WHEN 1 THEN 'History table'
        WHEN 2 THEN 'System-versioned temporal'
    END AS TemporalType,
    h.name AS HistoryTableName
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
WHERE t.name IN ('ImportedDataRecords', 'ImportedDataItems')
ORDER BY t.name;

PRINT '✅ Hoàn thành sửa chữa temporal tables và tối ưu hiệu năng!';
