-- ================================================================
-- 🚀 SETUP SQL SERVER TEMPORAL TABLES & COLUMNSTORE INDEXES
-- Script thiết lập hệ thống lưu trữ dữ liệu thô hiệu suất cao
-- Author: Agribank Lai Châu Dev Team  
-- Date: 2025-01-20
-- ================================================================

PRINT '🏦 AGRIBANK LAI CHÂU - THIẾT LẬP TEMPORAL TABLES & COLUMNSTORE';
PRINT '================================================================';
PRINT '';

-- 1. TẠO CÁC BẢNG DỮ LIỆU THÔ (NẾU CHƯA CÓ)
PRINT '1️⃣ TẠO CÁC BẢNG DỮ LIỆU THÔ';
PRINT '============================';

-- Bảng raw_data_imports (chính)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[raw_data_imports]') AND type in (N'U'))
BEGIN
    PRINT '📄 Tạo bảng raw_data_imports...';
    CREATE TABLE [dbo].[raw_data_imports] (
        [id] INT IDENTITY(1,1) PRIMARY KEY,
        [data_type] NVARCHAR(50) NOT NULL,
        [file_name] NVARCHAR(255) NOT NULL,
        [file_size] BIGINT NOT NULL,
        [statement_date] DATE NOT NULL,
        [total_records] INT NOT NULL DEFAULT 0,
        [successful_records] INT NOT NULL DEFAULT 0,
        [failed_records] INT NOT NULL DEFAULT 0,
        [import_status] NVARCHAR(20) NOT NULL DEFAULT 'Processing',
        [error_message] NVARCHAR(MAX) NULL,
        [created_date] DATETIME NOT NULL DEFAULT GETDATE(),
        [created_by] NVARCHAR(100) NULL,
        [is_compressed] BIT NOT NULL DEFAULT 0,
        [compression_ratio] DECIMAL(5,2) NULL,
        [processing_time_ms] INT NULL
    );
    PRINT '✅ Đã tạo bảng raw_data_imports';
END
ELSE
BEGIN
    PRINT '✅ Bảng raw_data_imports đã tồn tại';
END

-- Bảng raw_data_records (chi tiết)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[raw_data_records]') AND type in (N'U'))
BEGIN
    PRINT '📄 Tạo bảng raw_data_records...';
    CREATE TABLE [dbo].[raw_data_records] (
        [id] BIGINT IDENTITY(1,1) PRIMARY KEY,
        [raw_data_import_id] INT NOT NULL,
        [row_number] INT NOT NULL,
        [raw_data] NVARCHAR(MAX) NOT NULL,
        [parsed_data] NVARCHAR(MAX) NULL,
        [validation_status] NVARCHAR(20) NOT NULL DEFAULT 'Valid',
        [validation_errors] NVARCHAR(MAX) NULL,
        [processed_date] DATETIME NOT NULL DEFAULT GETDATE(),
        [hash_value] NVARCHAR(64) NULL,
        FOREIGN KEY ([raw_data_import_id]) REFERENCES [raw_data_imports]([id])
    );
    PRINT '✅ Đã tạo bảng raw_data_records';
END
ELSE
BEGIN
    PRINT '✅ Bảng raw_data_records đã tồn tại';
END

PRINT '';
PRINT '2️⃣ KÍCH HOẠT TEMPORAL TABLES';
PRINT '============================';

-- Kích hoạt temporal cho raw_data_imports
IF NOT EXISTS (
    SELECT * FROM sys.tables 
    WHERE name = 'raw_data_imports' AND temporal_type = 2
)
BEGIN
    PRINT '⏰ Kích hoạt temporal cho raw_data_imports...';
    
    -- Thêm cột temporal nếu chưa có
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('raw_data_imports') AND name = 'ValidFrom')
    BEGIN
        ALTER TABLE [raw_data_imports] ADD
            [ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
            [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo]);
    END
    
    -- Kích hoạt system versioning
    BEGIN TRY
        ALTER TABLE [raw_data_imports] 
        SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[raw_data_imports_history]));
        PRINT '✅ Đã kích hoạt temporal cho raw_data_imports';
    END TRY
    BEGIN CATCH
        PRINT '⚠️  Lỗi kích hoạt temporal cho raw_data_imports: ' + ERROR_MESSAGE();
    END CATCH
END
ELSE
BEGIN
    PRINT '✅ Temporal đã được kích hoạt cho raw_data_imports';
END

-- Kích hoạt temporal cho raw_data_records
IF NOT EXISTS (
    SELECT * FROM sys.tables 
    WHERE name = 'raw_data_records' AND temporal_type = 2
)
BEGIN
    PRINT '⏰ Kích hoạt temporal cho raw_data_records...';
    
    -- Thêm cột temporal nếu chưa có
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('raw_data_records') AND name = 'ValidFrom')
    BEGIN
        ALTER TABLE [raw_data_records] ADD
            [ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
            [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo]);
    END
    
    -- Kích hoạt system versioning
    BEGIN TRY
        ALTER TABLE [raw_data_records] 
        SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[raw_data_records_history]));
        PRINT '✅ Đã kích hoạt temporal cho raw_data_records';
    END TRY
    BEGIN CATCH
        PRINT '⚠️  Lỗi kích hoạt temporal cho raw_data_records: ' + ERROR_MESSAGE();
    END CATCH
END
ELSE
BEGIN
    PRINT '✅ Temporal đã được kích hoạt cho raw_data_records';
END

PRINT '';
PRINT '3️⃣ TẠO COLUMNSTORE INDEXES';
PRINT '==========================';

-- Columnstore index cho raw_data_imports
IF NOT EXISTS (
    SELECT * FROM sys.indexes 
    WHERE object_id = OBJECT_ID('raw_data_imports') 
    AND name = 'IX_raw_data_imports_columnstore'
)
BEGIN
    PRINT '📊 Tạo columnstore index cho raw_data_imports...';
    BEGIN TRY
        CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_raw_data_imports_columnstore]
        ON [dbo].[raw_data_imports] 
        (
            [data_type], 
            [statement_date], 
            [total_records], 
            [successful_records], 
            [failed_records], 
            [import_status],
            [created_date],
            [file_size],
            [processing_time_ms]
        );
        PRINT '✅ Đã tạo columnstore index cho raw_data_imports';
    END TRY
    BEGIN CATCH
        PRINT '⚠️  Lỗi tạo columnstore index cho raw_data_imports: ' + ERROR_MESSAGE();
    END CATCH
END
ELSE
BEGIN
    PRINT '✅ Columnstore index đã tồn tại cho raw_data_imports';
END

-- Columnstore index cho raw_data_records (bảng lớn)
IF NOT EXISTS (
    SELECT * FROM sys.indexes 
    WHERE object_id = OBJECT_ID('raw_data_records') 
    AND name = 'IX_raw_data_records_columnstore'
)
BEGIN
    PRINT '📊 Tạo columnstore index cho raw_data_records...';
    BEGIN TRY
        CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_raw_data_records_columnstore]
        ON [dbo].[raw_data_records] 
        (
            [raw_data_import_id], 
            [validation_status], 
            [processed_date],
            [row_number],
            [hash_value]
        );
        PRINT '✅ Đã tạo columnstore index cho raw_data_records';
    END TRY
    BEGIN CATCH
        PRINT '⚠️  Lỗi tạo columnstore index cho raw_data_records: ' + ERROR_MESSAGE();
    END CATCH
END
ELSE
BEGIN
    PRINT '✅ Columnstore index đã tồn tại cho raw_data_records';
END

PRINT '';
PRINT '4️⃣ TẠO INDEXES TỐI ƯU HIỆU SUẤT';
PRINT '==============================';

-- Index cho raw_data_imports
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('raw_data_imports') AND name = 'IX_raw_data_imports_datatype_date')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_raw_data_imports_datatype_date] 
    ON [raw_data_imports] ([data_type], [statement_date]);
    PRINT '✅ Đã tạo index IX_raw_data_imports_datatype_date';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('raw_data_imports') AND name = 'IX_raw_data_imports_status')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_raw_data_imports_status] 
    ON [raw_data_imports] ([import_status]) 
    INCLUDE ([total_records], [successful_records], [failed_records]);
    PRINT '✅ Đã tạo index IX_raw_data_imports_status';
END

-- Index cho raw_data_records
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('raw_data_records') AND name = 'IX_raw_data_records_import_status')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_raw_data_records_import_status] 
    ON [raw_data_records] ([raw_data_import_id], [validation_status]);
    PRINT '✅ Đã tạo index IX_raw_data_records_import_status';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('raw_data_records') AND name = 'IX_raw_data_records_hash')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_raw_data_records_hash] 
    ON [raw_data_records] ([hash_value]) 
    WHERE [hash_value] IS NOT NULL;
    PRINT '✅ Đã tạo index IX_raw_data_records_hash';
END

PRINT '';
PRINT '5️⃣ TẠO VIEWS TỐI ƯU TRUY VẤN';
PRINT '============================';

-- View tổng hợp thống kê import
IF OBJECT_ID('vw_import_statistics', 'V') IS NOT NULL
    DROP VIEW [vw_import_statistics];

PRINT '📊 Tạo view vw_import_statistics...';
EXEC('
CREATE VIEW [vw_import_statistics] AS
SELECT 
    [data_type],
    [statement_date],
    COUNT(*) as [total_imports],
    SUM([total_records]) as [total_records],
    SUM([successful_records]) as [successful_records], 
    SUM([failed_records]) as [failed_records],
    AVG([processing_time_ms]) as [avg_processing_time],
    SUM([file_size]) as [total_file_size],
    MAX([created_date]) as [last_import_date]
FROM [raw_data_imports]
WHERE [import_status] = ''Completed''
GROUP BY [data_type], [statement_date]
');
PRINT '✅ Đã tạo view vw_import_statistics';

-- View lịch sử temporal 
IF OBJECT_ID('vw_import_history', 'V') IS NOT NULL
    DROP VIEW [vw_import_history];

PRINT '📚 Tạo view vw_import_history...';
EXEC('
CREATE VIEW [vw_import_history] AS
SELECT 
    [id],
    [data_type],
    [file_name],
    [statement_date],
    [total_records],
    [import_status],
    [created_date],
    [ValidFrom],
    [ValidTo],
    ''Current'' as [Version_Type]
FROM [raw_data_imports]
UNION ALL
SELECT 
    [id],
    [data_type], 
    [file_name],
    [statement_date],
    [total_records],
    [import_status],
    [created_date],
    [ValidFrom],
    [ValidTo],
    ''History'' as [Version_Type]
FROM [raw_data_imports_history]
');
PRINT '✅ Đã tạo view vw_import_history';

PRINT '';
PRINT '6️⃣ CẬP NHẬT THỐNG KÊ VÀ TỐI ƯU';
PRINT '==============================';

-- Cập nhật statistics cho các bảng
PRINT '📈 Cập nhật statistics...';
UPDATE STATISTICS [raw_data_imports];
UPDATE STATISTICS [raw_data_records];

-- Thiết lập auto-stats
ALTER DATABASE CURRENT SET AUTO_UPDATE_STATISTICS ON;
ALTER DATABASE CURRENT SET AUTO_CREATE_STATISTICS ON;

PRINT '✅ Đã cập nhật statistics và tối ưu database';

PRINT '';
PRINT '7️⃣ KIỂM TRA KẾT QUẢ THIẾT LẬP';
PRINT '=============================';

-- Kiểm tra temporal tables
SELECT 
    t.name as [Table Name],
    CASE t.temporal_type 
        WHEN 2 THEN '✅ Temporal Enabled'
        ELSE '❌ Not Temporal'
    END as [Temporal Status],
    h.name as [History Table]
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
WHERE t.name IN ('raw_data_imports', 'raw_data_records');

-- Kiểm tra columnstore indexes
SELECT 
    t.name as [Table Name],
    i.name as [Index Name],
    i.type_desc as [Index Type]
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE t.name IN ('raw_data_imports', 'raw_data_records')
AND i.type IN (5, 6); -- COLUMNSTORE

-- Kiểm tra views
SELECT name as [View Name]
FROM sys.views 
WHERE name IN ('vw_import_statistics', 'vw_import_history');

PRINT '';
PRINT '================================================================';
PRINT '🎉 HOÀN THÀNH THIẾT LẬP TEMPORAL TABLES & COLUMNSTORE INDEXES!';
PRINT '';
PRINT '✅ Các bảng đã được cấu hình temporal tables';
PRINT '✅ Columnstore indexes đã được tạo để tối ưu hiệu suất';
PRINT '✅ Views và indexes đã được thiết lập';
PRINT '✅ Hệ thống sẵn sàng cho import dữ liệu thật!';
PRINT '';
PRINT '📋 CÁC TÍNH NĂNG ĐÃ CÓ:';
PRINT '   🕒 Lưu trữ lịch sử thay đổi tự động (temporal tables)';
PRINT '   📊 Truy vấn siêu nhanh với columnstore indexes';
PRINT '   🔍 Views tối ưu cho báo cáo và thống kê'; 
PRINT '   ⚡ Indexes được tối ưu cho các truy vấn thường dùng';
PRINT '';
PRINT '🚀 SẴN SÀNG IMPORT DỮ LIỆU THẬT!';
PRINT '================================================================';
