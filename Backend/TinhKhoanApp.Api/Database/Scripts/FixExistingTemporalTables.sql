-- =============================================
-- Script: FixExistingTemporalTables.sql
-- Mục đích: Kiểm tra và sửa chữa các temporal tables hiện có
-- Tác giả: Em (siêu lập trình viên Fullstack)
-- Ngày: 26/06/2025
-- Chú thích: Đảm bảo tất cả tables đều có temporal + columnstore
-- =============================================

USE TinhKhoanDB;
GO

PRINT '🔧 KIỂM TRA VÀ SỬA CHỮA TEMPORAL TABLES HIỆN CÓ';
PRINT '===============================================================================';

-- =====================================================
-- BƯỚC 1: KIỂM TRA TRẠNG THÁI CÁC BẢNG HIỆN TẠI
-- =====================================================

PRINT '📊 Kiểm tra trạng thái các bảng hiện tại...';

-- Tạo bảng tạm để lưu kết quả kiểm tra
IF OBJECT_ID('tempdb..#TableStatus') IS NOT NULL DROP TABLE #TableStatus;

CREATE TABLE #TableStatus (
    TableName NVARCHAR(128),
    IsTemporal BIT,
    HasColumnstore BIT,
    HistoryTableName NVARCHAR(128),
    RowCount BIGINT,
    Issues NVARCHAR(500),
    Priority INT
);

-- Kiểm tra các bảng chính
INSERT INTO #TableStatus (TableName, IsTemporal, HasColumnstore, HistoryTableName, RowCount, Issues, Priority)
SELECT 
    t.name AS TableName,
    CASE WHEN t.temporal_type = 2 THEN 1 ELSE 0 END AS IsTemporal,
    CASE WHEN EXISTS (
        SELECT 1 FROM sys.indexes i 
        WHERE i.object_id = t.object_id AND i.type IN (5,6)
    ) THEN 1 ELSE 0 END AS HasColumnstore,
    h.name AS HistoryTableName,
    ISNULL(p.rows, 0) AS RowCount,
    '' AS Issues,
    CASE 
        WHEN t.name LIKE 'ImportedData%' THEN 1  -- Ưu tiên cao nhất
        WHEN t.name LIKE '%RawData%' THEN 2      -- Ưu tiên cao
        WHEN t.name LIKE '%History' THEN 3      -- Ưu tiên thấp (history tables)
        ELSE 4                                   -- Các bảng khác
    END AS Priority
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
LEFT JOIN sys.partitions p ON t.object_id = p.object_id AND p.index_id IN (0,1)
WHERE t.name NOT LIKE '%_History%'  -- Loại trừ history tables
ORDER BY Priority, t.name;

-- Cập nhật thông tin issues
UPDATE #TableStatus 
SET Issues = CASE 
    WHEN IsTemporal = 0 AND HasColumnstore = 0 THEN 'Missing temporal + columnstore'
    WHEN IsTemporal = 0 THEN 'Missing temporal'
    WHEN HasColumnstore = 0 THEN 'Missing columnstore'
    ELSE 'OK'
END;

-- Hiển thị báo cáo trạng thái
SELECT 
    TableName,
    CASE WHEN IsTemporal = 1 THEN '✅' ELSE '❌' END AS Temporal,
    CASE WHEN HasColumnstore = 1 THEN '✅' ELSE '❌' END AS Columnstore,
    HistoryTableName,
    RowCount,
    Issues,
    Priority
FROM #TableStatus 
ORDER BY Priority, TableName;

-- =====================================================
-- BƯỚC 2: SỬA CHỮA IMPORTEDDATARECORDS VÀ IMPORTEDDATAITEMS
-- =====================================================

PRINT '';
PRINT '🔧 Sửa chữa ImportedDataRecords và ImportedDataItems...';

-- Fix ImportedDataRecords
IF EXISTS (SELECT 1 FROM #TableStatus WHERE TableName = 'ImportedDataRecords' AND IsTemporal = 0)
BEGIN
    PRINT '🔧 Chuyển đổi ImportedDataRecords sang Temporal Table...';
    
    BEGIN TRY
        -- Disable system versioning if exists
        IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ImportedDataRecords' AND temporal_type = 2)
        BEGIN
            ALTER TABLE [ImportedDataRecords] SET (SYSTEM_VERSIONING = OFF);
        END
        
        -- Add temporal columns if not exists
        IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'SysStartTime' AND object_id = OBJECT_ID('ImportedDataRecords'))
        BEGIN
            ALTER TABLE [ImportedDataRecords] ADD 
                [SysStartTime] DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
                [SysEndTime] DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999');
        END
        
        -- Add period if not exists
        IF NOT EXISTS (SELECT 1 FROM sys.periods WHERE object_id = OBJECT_ID('ImportedDataRecords'))
        BEGIN
            ALTER TABLE [ImportedDataRecords] 
            ADD PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
        END
        
        -- Enable system versioning
        ALTER TABLE [ImportedDataRecords] 
        SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[ImportedDataRecords_History]));
        
        PRINT '   ✅ ImportedDataRecords đã được chuyển đổi sang Temporal Table';
        
    END TRY
    BEGIN CATCH
        PRINT '   ❌ Lỗi chuyển đổi ImportedDataRecords: ' + ERROR_MESSAGE();
    END CATCH
END

-- Fix ImportedDataItems
IF EXISTS (SELECT 1 FROM #TableStatus WHERE TableName = 'ImportedDataItems' AND IsTemporal = 0)
BEGIN
    PRINT '🔧 Chuyển đổi ImportedDataItems sang Temporal Table...';
    
    BEGIN TRY
        -- Disable system versioning if exists
        IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ImportedDataItems' AND temporal_type = 2)
        BEGIN
            ALTER TABLE [ImportedDataItems] SET (SYSTEM_VERSIONING = OFF);
        END
        
        -- Add temporal columns if not exists
        IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'SysStartTime' AND object_id = OBJECT_ID('ImportedDataItems'))
        BEGIN
            ALTER TABLE [ImportedDataItems] ADD 
                [SysStartTime] DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
                [SysEndTime] DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999');
        END
        
        -- Add period if not exists
        IF NOT EXISTS (SELECT 1 FROM sys.periods WHERE object_id = OBJECT_ID('ImportedDataItems'))
        BEGIN
            ALTER TABLE [ImportedDataItems] 
            ADD PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
        END
        
        -- Enable system versioning
        ALTER TABLE [ImportedDataItems] 
        SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[ImportedDataItems_History]));
        
        PRINT '   ✅ ImportedDataItems đã được chuyển đổi sang Temporal Table';
        
    END TRY
    BEGIN CATCH
        PRINT '   ❌ Lỗi chuyển đổi ImportedDataItems: ' + ERROR_MESSAGE();
    END CATCH
END

-- =====================================================
-- BƯỚC 3: TẠO COLUMNSTORE INDEXES CHO TẤT CẢ BẢNG CẦN THIẾT
-- =====================================================

PRINT '';
PRINT '📊 Tạo Columnstore Indexes cho các bảng cần thiết...';

DECLARE @TableToFix NVARCHAR(128), @HasCS BIT;
DECLARE fix_cursor CURSOR FOR
SELECT TableName, HasColumnstore 
FROM #TableStatus 
WHERE Issues != 'OK' AND TableName NOT LIKE '%_History';

OPEN fix_cursor;
FETCH NEXT FROM fix_cursor INTO @TableToFix, @HasCS;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT '🔧 Xử lý bảng: ' + @TableToFix;
    
    -- Tạo columnstore index nếu chưa có
    IF @HasCS = 0
    BEGIN
        BEGIN TRY
            DECLARE @CSIndexName NVARCHAR(128) = 'NCCI_' + @TableToFix + '_Analytics';
            DECLARE @CSIndexSQL NVARCHAR(MAX);
            
            -- Kiểm tra size của bảng để quyết định loại columnstore index
            DECLARE @RowCount BIGINT;
            SELECT @RowCount = RowCount FROM #TableStatus WHERE TableName = @TableToFix;
            
            IF @RowCount > 100000 OR @TableToFix LIKE '%RawData%' -- Bảng lớn dùng clustered columnstore
            BEGIN
                SET @CSIndexSQL = 'CREATE CLUSTERED COLUMNSTORE INDEX [CCI_' + @TableToFix + '] ON [dbo].[' + @TableToFix + ']
                                  WITH (COMPRESSION_DELAY = 0, MAXDOP = 4);';
            END
            ELSE -- Bảng nhỏ dùng nonclustered columnstore
            BEGIN
                SET @CSIndexSQL = 'CREATE NONCLUSTERED COLUMNSTORE INDEX [' + @CSIndexName + '] 
                                  ON [dbo].[' + @TableToFix + '] ([Id])
                                  WITH (COMPRESSION_DELAY = 0);';
            END
            
            EXEC sp_executesql @CSIndexSQL;
            PRINT '   ✅ Đã tạo Columnstore Index cho: ' + @TableToFix;
            
        END TRY
        BEGIN CATCH
            PRINT '   ⚠️ Không thể tạo Columnstore Index cho ' + @TableToFix + ': ' + ERROR_MESSAGE();
        END CATCH
    END
    
    -- Tạo columnstore cho history table nếu có
    DECLARE @HistoryTable NVARCHAR(128);
    SELECT @HistoryTable = HistoryTableName FROM #TableStatus WHERE TableName = @TableToFix;
    
    IF @HistoryTable IS NOT NULL
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name LIKE 'CCI_%' AND object_id = OBJECT_ID(@HistoryTable))
            BEGIN
                DECLARE @HistoryCSSQL NVARCHAR(MAX) = 'CREATE CLUSTERED COLUMNSTORE INDEX [CCI_' + @HistoryTable + '] 
                                                      ON [dbo].[' + @HistoryTable + ']
                                                      WITH (COMPRESSION_DELAY = 0, MAXDOP = 4);';
                EXEC sp_executesql @HistoryCSSQL;
                PRINT '   ✅ Đã tạo Columnstore Index cho: ' + @HistoryTable;
            END
            
        END TRY
        BEGIN CATCH
            PRINT '   ⚠️ Không thể tạo Columnstore Index cho ' + @HistoryTable + ': ' + ERROR_MESSAGE();
        END CATCH
    END
    
    FETCH NEXT FROM fix_cursor INTO @TableToFix, @HasCS;
END

CLOSE fix_cursor;
DEALLOCATE fix_cursor;

-- =====================================================
-- BƯỚC 4: KIỂM TRA LẠI VÀ BÁO CÁO KẾT QUẢ
-- =====================================================

PRINT '';
PRINT '🔍 Kiểm tra lại trạng thái sau khi sửa chữa...';

-- Xóa và tạo lại bảng tạm
DROP TABLE #TableStatus;

CREATE TABLE #TableStatus (
    TableName NVARCHAR(128),
    IsTemporal BIT,
    HasColumnstore BIT,
    HistoryTableName NVARCHAR(128),
    RowCount BIGINT,
    Issues NVARCHAR(500),
    Priority INT
);

-- Kiểm tra lại
INSERT INTO #TableStatus (TableName, IsTemporal, HasColumnstore, HistoryTableName, RowCount, Issues, Priority)
SELECT 
    t.name AS TableName,
    CASE WHEN t.temporal_type = 2 THEN 1 ELSE 0 END AS IsTemporal,
    CASE WHEN EXISTS (
        SELECT 1 FROM sys.indexes i 
        WHERE i.object_id = t.object_id AND i.type IN (5,6)
    ) THEN 1 ELSE 0 END AS HasColumnstore,
    h.name AS HistoryTableName,
    ISNULL(p.rows, 0) AS RowCount,
    '' AS Issues,
    CASE 
        WHEN t.name LIKE 'ImportedData%' THEN 1
        WHEN t.name LIKE '%RawData%' THEN 2
        WHEN t.name LIKE '%History' THEN 3
        ELSE 4
    END AS Priority
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
LEFT JOIN sys.partitions p ON t.object_id = p.object_id AND p.index_id IN (0,1)
WHERE t.name NOT LIKE '%_History%'
ORDER BY Priority, t.name;

-- Cập nhật issues
UPDATE #TableStatus 
SET Issues = CASE 
    WHEN IsTemporal = 0 AND HasColumnstore = 0 THEN '❌ Missing temporal + columnstore'
    WHEN IsTemporal = 0 THEN '⚠️ Missing temporal'
    WHEN HasColumnstore = 0 THEN '⚠️ Missing columnstore'
    ELSE '✅ OK'
END;

-- Báo cáo cuối cùng
PRINT '';
PRINT '📋 BÁO CÁO TRẠNG THÁI CUỐI CÙNG:';
PRINT '===============================================================================';

SELECT 
    TableName,
    CASE WHEN IsTemporal = 1 THEN '✅ Yes' ELSE '❌ No' END AS [Temporal Table],
    CASE WHEN HasColumnstore = 1 THEN '✅ Yes' ELSE '❌ No' END AS [Columnstore Index],
    HistoryTableName AS [History Table],
    FORMAT(RowCount, 'N0') AS [Row Count],
    Issues AS [Status]
FROM #TableStatus 
ORDER BY Priority, TableName;

-- Thống kê tổng quan
DECLARE @TotalTables INT, @TemporalTables INT, @ColumnstoreTables INT, @CompleteTables INT;

SELECT 
    @TotalTables = COUNT(*),
    @TemporalTables = SUM(CASE WHEN IsTemporal = 1 THEN 1 ELSE 0 END),
    @ColumnstoreTables = SUM(CASE WHEN HasColumnstore = 1 THEN 1 ELSE 0 END),
    @CompleteTables = SUM(CASE WHEN IsTemporal = 1 AND HasColumnstore = 1 THEN 1 ELSE 0 END)
FROM #TableStatus;

PRINT '';
PRINT '📊 THỐNG KÊ TỔNG QUAN:';
PRINT '• Tổng số bảng: ' + CAST(@TotalTables AS NVARCHAR);
PRINT '• Bảng có Temporal: ' + CAST(@TemporalTables AS NVARCHAR) + '/' + CAST(@TotalTables AS NVARCHAR);
PRINT '• Bảng có Columnstore: ' + CAST(@ColumnstoreTables AS NVARCHAR) + '/' + CAST(@TotalTables AS NVARCHAR);
PRINT '• Bảng hoàn chỉnh (Temporal + Columnstore): ' + CAST(@CompleteTables AS NVARCHAR) + '/' + CAST(@TotalTables AS NVARCHAR);
PRINT '';

IF @CompleteTables = @TotalTables
BEGIN
    PRINT '🎉 TẤT CẢ BẢNG ĐÃ ĐƯỢC CẤU HÌNH TEMPORAL + COLUMNSTORE HOÀN CHỈNH!';
END
ELSE
BEGIN
    PRINT '⚠️ Vẫn còn ' + CAST(@TotalTables - @CompleteTables AS NVARCHAR) + ' bảng cần được hoàn thiện.';
    PRINT '💡 Chạy lại script CompleteTemporalTablesSetup.sql để tạo các bảng dữ liệu thô còn thiếu.';
END

-- Dọn dẹp
DROP TABLE #TableStatus;

PRINT '';
PRINT '✅ HOÀN THÀNH KIỂM TRA VÀ SỬA CHỮA TEMPORAL TABLES';

GO
