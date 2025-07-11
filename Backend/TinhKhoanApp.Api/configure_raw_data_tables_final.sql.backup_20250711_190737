-- ==================================================
-- SCRIPT CẤU HÌNH TEMPORAL TABLES + COLUMNSTORE INDEXES - PHIÊN BẢN CUỐI CÙNG
-- XỬ LÝ HOÀN TOÀN IDENTITY VÀ CONSTRAINT ISSUES
-- ==================================================

USE TinhKhoanDB;
GO

-- ==================================================
-- PHẦN 1: DANH SÁCH CÁC BẢNG CẦN CẤU HÌNH
-- ==================================================
DECLARE @RawDataTables TABLE (
    TableName NVARCHAR(50),
    FileType NVARCHAR(10)
);

INSERT INTO @RawDataTables VALUES
('7800_DT_KHKD1', 'Excel'),
('DB01', 'CSV'),
('DP01_New', 'CSV'),
('DPDA', 'CSV'),
('EI01', 'CSV'),
('GL01', 'CSV'),
('GL41', 'CSV'),
('KH03', 'CSV'),
('LN01', 'CSV'),
('LN02', 'CSV'),
('LN03', 'CSV'),
('RR01', 'CSV');

-- ==================================================
-- PHẦN 2: XỬ LÝ TỪNG BẢNG
-- ==================================================
DECLARE @tableName NVARCHAR(50);
DECLARE @fileType NVARCHAR(10);
DECLARE @sql NVARCHAR(MAX);
DECLARE @historyTableName NVARCHAR(60);

DECLARE table_cursor CURSOR FOR
SELECT TableName, FileType FROM @RawDataTables;

OPEN table_cursor;
FETCH NEXT FROM table_cursor INTO @tableName, @fileType;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @historyTableName = @tableName + '_History';

    PRINT '=== Cấu hình bảng ' + @tableName + ' (' + @fileType + ' files) ===';

    -- Bước 1: Tắt system versioning nếu đang bật
    IF EXISTS (SELECT * FROM sys.tables WHERE name = @tableName AND temporal_type = 2)
    BEGIN
        SET @sql = 'ALTER TABLE [' + @tableName + '] SET (SYSTEM_VERSIONING = OFF);';
        EXEC sp_executesql @sql;
        PRINT '  ✓ Đã tắt system versioning';
    END

    -- Bước 2: Xóa history table cũ nếu có
    IF EXISTS (SELECT * FROM sys.tables WHERE name = @historyTableName)
    BEGIN
        SET @sql = 'DROP TABLE [' + @historyTableName + '];';
        EXEC sp_executesql @sql;
        PRINT '  ✓ Đã xóa history table cũ';
    END

    -- Bước 3: Xóa system-time columns cũ nếu có
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(@tableName) AND name = 'SysStartTime')
    BEGIN
        SET @sql = 'ALTER TABLE [' + @tableName + '] DROP PERIOD FOR SYSTEM_TIME;';
        EXEC sp_executesql @sql;

        SET @sql = 'ALTER TABLE [' + @tableName + '] DROP COLUMN SysStartTime, SysEndTime;';
        EXEC sp_executesql @sql;
        PRINT '  ✓ Đã xóa system-time columns cũ';
    END

    -- Bước 4: Thêm system-time columns mới
    SET @sql = 'ALTER TABLE [' + @tableName + ']
                ADD SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN DEFAULT GETUTCDATE(),
                    SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN DEFAULT ''9999-12-31 23:59:59.9999999'',
                    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);';
    EXEC sp_executesql @sql;
    PRINT '  ✓ Đã thêm system-time columns mới';

    -- Bước 5: Bật system versioning với history table mới
    SET @sql = 'ALTER TABLE [' + @tableName + ']
                SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.[' + @historyTableName + ']));';
    EXEC sp_executesql @sql;
    PRINT '  ✓ Đã bật system versioning với history table mới';

    -- Bước 6: Xóa clustered index cũ trước khi tạo columnstore (nếu có)
    DECLARE @indexName NVARCHAR(128);
    SELECT @indexName = i.name
    FROM sys.indexes i
    WHERE i.object_id = OBJECT_ID(@tableName)
        AND i.type_desc = 'CLUSTERED'
        AND i.name IS NOT NULL;

    IF @indexName IS NOT NULL
    BEGIN
        SET @sql = 'DROP INDEX [' + @indexName + '] ON [' + @tableName + '];';
        EXEC sp_executesql @sql;
        PRINT '  ✓ Đã xóa clustered index cũ: ' + @indexName;
    END

    -- Bước 7: Tạo clustered columnstore index
    SET @sql = 'CREATE CLUSTERED COLUMNSTORE INDEX CCI_' + @tableName + ' ON [' + @tableName + '];';
    EXEC sp_executesql @sql;
    PRINT '  ✓ Đã tạo clustered columnstore index';

    PRINT '  🎉 Hoàn thành cấu hình bảng ' + @tableName;
    PRINT '';

    FETCH NEXT FROM table_cursor INTO @tableName, @fileType;
END

CLOSE table_cursor;
DEALLOCATE table_cursor;

-- ==================================================
-- PHẦN 3: BÁO CÁO KẾT QUẢ
-- ==================================================
PRINT '========================================================';
PRINT '🎉 HOÀN THÀNH CẤU HÌNH TEMPORAL TABLES + COLUMNSTORE';
PRINT '========================================================';
PRINT '';

-- Báo cáo chi tiết
SELECT
    t.name as [Tên Bảng],
    CASE
        WHEN t.temporal_type = 2 THEN '✅ Temporal'
        ELSE '❌ Non-temporal'
    END as [Trạng thái Temporal],
    CASE
        WHEN EXISTS (SELECT 1 FROM sys.indexes i WHERE i.object_id = t.object_id AND i.type_desc = 'CLUSTERED COLUMNSTORE')
        THEN '✅ Columnstore'
        ELSE '❌ No Columnstore'
    END as [Trạng thái Columnstore],
    ht.name as [History Table]
FROM sys.tables t
LEFT JOIN sys.tables ht ON t.history_table_id = ht.object_id
WHERE t.name IN ('7800_DT_KHKD1', 'DB01', 'DP01_New', 'DPDA', 'EI01', 'GL01', 'GL41', 'KH03', 'LN01', 'LN02', 'LN03', 'RR01')
ORDER BY t.name;

-- Đếm số lượng
SELECT
    COUNT(*) as [Tổng số bảng],
    SUM(CASE WHEN temporal_type = 2 THEN 1 ELSE 0 END) as [Số bảng Temporal],
    SUM(CASE WHEN EXISTS (SELECT 1 FROM sys.indexes i WHERE i.object_id = t.object_id AND i.type_desc = 'CLUSTERED COLUMNSTORE') THEN 1 ELSE 0 END) as [Số bảng Columnstore]
FROM sys.tables t
WHERE t.name IN ('7800_DT_KHKD1', 'DB01', 'DP01_New', 'DPDA', 'EI01', 'GL01', 'GL41', 'KH03', 'LN01', 'LN02', 'LN03', 'RR01');

PRINT '';
PRINT '🚀 Tất cả bảng dữ liệu thô đã sẵn sàng cho hiệu năng tối ưu!';
GO
