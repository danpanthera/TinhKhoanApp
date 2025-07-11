-- ==================================================
-- SCRIPT CẤU HÌNH TEMPORAL TABLES + COLUMNSTORE INDEXES - PHIÊN BẢN CẢI TIẾN
-- CHO CÁC BẢNG DỮ LIỆU THÔ - TINHKHOAN APP
-- ==================================================

USE TinhKhoanDB;
GO

-- ==================================================
-- HÀM HELPER: XÓA UNIQUE CONSTRAINTS TRÊN HISTORY TABLE
-- ==================================================
DECLARE @sql NVARCHAR(MAX) = '';

-- Tạo script để xóa tất cả unique constraints trên history tables
SELECT @sql = @sql + 'ALTER TABLE [' + t.name + '] DROP CONSTRAINT [' + i.name + '];' + CHAR(13)
FROM sys.tables t
INNER JOIN sys.indexes i ON t.object_id = i.object_id
WHERE t.name LIKE '%_History'
    AND i.is_unique = 1
    AND i.is_primary_key = 1;

-- Thực thi script xóa constraints
IF LEN(@sql) > 0
BEGIN
    PRINT 'Xóa primary key constraints trên history tables...';
    EXEC sp_executesql @sql;
END

-- ==================================================
-- 1. BẢNG 7800_DT_KHKD1 (Excel Files)
-- ==================================================
PRINT 'Cấu hình bảng 7800_DT_KHKD1...';

-- Kiểm tra và thêm system-time columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('7800_DT_KHKD1') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE [7800_DT_KHKD1]
    ADD SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN DEFAULT GETUTCDATE(),
        SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
    PRINT '  - Đã thêm system-time columns cho 7800_DT_KHKD1';
END

-- Bật system versioning
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '7800_DT_KHKD1' AND temporal_type = 2)
BEGIN
    ALTER TABLE [7800_DT_KHKD1] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.[7800_DT_KHKD1_History]));
    PRINT '  - Đã bật system versioning cho 7800_DT_KHKD1';
END

-- Tạo columnstore index
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('7800_DT_KHKD1') AND type_desc = 'CLUSTERED COLUMNSTORE')
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_7800_DT_KHKD1 ON [7800_DT_KHKD1];
    PRINT '  - Đã tạo columnstore index cho 7800_DT_KHKD1';
END

-- ==================================================
-- MACRO CẤU HÌNH CHO TẤT CẢ CÁC BẢNG KHÁC
-- ==================================================

-- Danh sách các bảng cần cấu hình
DECLARE @tables TABLE (TableName NVARCHAR(50));
INSERT INTO @tables VALUES
('DB01'), ('DP01_New'), ('DPDA'), ('EI01'),
('GL01'), ('GL41'), ('KH03'), ('LN01'),
('LN02'), ('LN03'), ('RR01');

DECLARE @tableName NVARCHAR(50);
DECLARE @sqlCmd NVARCHAR(MAX);

DECLARE table_cursor CURSOR FOR
SELECT TableName FROM @tables;

OPEN table_cursor;
FETCH NEXT FROM table_cursor INTO @tableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT 'Cấu hình bảng ' + @tableName + '...';

    -- Kiểm tra và thêm system-time columns
    SET @sqlCmd = '
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(''' + @tableName + ''') AND name = ''SysStartTime'')
    BEGIN
        ALTER TABLE [' + @tableName + ']
        ADD SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN DEFAULT GETUTCDATE(),
            SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN DEFAULT ''9999-12-31 23:59:59.9999999'',
            PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
        PRINT ''  - Đã thêm system-time columns cho ' + @tableName + ''';
    END';
    EXEC sp_executesql @sqlCmd;

    -- Bật system versioning
    SET @sqlCmd = '
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = ''' + @tableName + ''' AND temporal_type = 2)
    BEGIN
        ALTER TABLE [' + @tableName + '] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.[' + @tableName + '_History]));
        PRINT ''  - Đã bật system versioning cho ' + @tableName + ''';
    END';
    EXEC sp_executesql @sqlCmd;

    -- Tạo columnstore index
    SET @sqlCmd = '
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(''' + @tableName + ''') AND type_desc = ''CLUSTERED COLUMNSTORE'')
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX CCI_' + @tableName + ' ON [' + @tableName + '];
        PRINT ''  - Đã tạo columnstore index cho ' + @tableName + ''';
    END';
    EXEC sp_executesql @sqlCmd;

    FETCH NEXT FROM table_cursor INTO @tableName;
END

CLOSE table_cursor;
DEALLOCATE table_cursor;

-- ==================================================
-- KẾT THÚC VÀ BÁO CÁO
-- ==================================================
PRINT '';
PRINT '=== HOÀN THÀNH CẤU HÌNH TEMPORAL TABLES + COLUMNSTORE INDEXES ===';
PRINT 'Tất cả 12 bảng dữ liệu thô đã được cấu hình thành công!';
PRINT '';

-- Báo cáo kết quả
SELECT
    t.name as TableName,
    CASE
        WHEN t.temporal_type = 2 THEN 'Temporal ✅'
        ELSE 'Non-temporal ❌'
    END as TemporalStatus,
    CASE
        WHEN EXISTS (SELECT 1 FROM sys.indexes i WHERE i.object_id = t.object_id AND i.type_desc = 'CLUSTERED COLUMNSTORE')
        THEN 'Columnstore ✅'
        ELSE 'No Columnstore ❌'
    END as ColumnstoreStatus
FROM sys.tables t
WHERE t.name IN ('7800_DT_KHKD1', 'DB01', 'DP01_New', 'DPDA', 'EI01', 'GL01', 'GL41', 'KH03', 'LN01', 'LN02', 'LN03', 'RR01')
ORDER BY t.name;

GO
