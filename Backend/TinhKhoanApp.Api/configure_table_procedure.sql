-- ==================================================
-- SCRIPT TỐI ƯU: CẤU HÌNH TỪNG BẢNG MỘT CÁCH AN TOÀN
-- ==================================================

USE TinhKhoanDB;
GO

-- Hàm helper để cấu hình một bảng
CREATE OR ALTER PROCEDURE ConfigureRawDataTable
    @TableName NVARCHAR(50)
AS
BEGIN
    DECLARE @sql NVARCHAR(MAX);
    DECLARE @historyTableName NVARCHAR(60) = @TableName + '_History';

    PRINT '=== Cấu hình bảng ' + @TableName + ' ===';

    -- Bước 1: Tắt system versioning nếu có
    IF EXISTS (SELECT * FROM sys.tables WHERE name = @TableName AND temporal_type = 2)
    BEGIN
        SET @sql = 'ALTER TABLE [' + @TableName + '] SET (SYSTEM_VERSIONING = OFF);';
        EXEC sp_executesql @sql;
        PRINT '  ✓ Đã tắt system versioning';
    END

    -- Bước 2: Xóa history table cũ
    IF EXISTS (SELECT * FROM sys.tables WHERE name = @historyTableName)
    BEGIN
        SET @sql = 'DROP TABLE [' + @historyTableName + '];';
        EXEC sp_executesql @sql;
        PRINT '  ✓ Đã xóa history table cũ';
    END

    -- Bước 3: Xóa default constraints cho system-time columns
    SET @sql = '';
    SELECT @sql = @sql + 'ALTER TABLE [' + @TableName + '] DROP CONSTRAINT [' + dc.name + '];' + CHAR(13)
    FROM sys.default_constraints dc
    INNER JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
    WHERE dc.parent_object_id = OBJECT_ID(@TableName)
        AND c.name IN ('SysStartTime', 'SysEndTime');

    IF LEN(@sql) > 0
    BEGIN
        EXEC sp_executesql @sql;
        PRINT '  ✓ Đã xóa default constraints';
    END

    -- Bước 4: Xóa PERIOD FOR SYSTEM_TIME
    IF EXISTS (SELECT * FROM sys.periods WHERE object_id = OBJECT_ID(@TableName))
    BEGIN
        SET @sql = 'ALTER TABLE [' + @TableName + '] DROP PERIOD FOR SYSTEM_TIME;';
        EXEC sp_executesql @sql;
        PRINT '  ✓ Đã xóa PERIOD FOR SYSTEM_TIME';
    END

    -- Bước 5: Xóa system-time columns
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(@TableName) AND name = 'SysStartTime')
    BEGIN
        SET @sql = 'ALTER TABLE [' + @TableName + '] DROP COLUMN SysStartTime, SysEndTime;';
        EXEC sp_executesql @sql;
        PRINT '  ✓ Đã xóa system-time columns cũ';
    END

    -- Bước 6: Xóa clustered index cũ (trừ primary key)
    DECLARE @indexName NVARCHAR(128);
    SELECT @indexName = i.name
    FROM sys.indexes i
    WHERE i.object_id = OBJECT_ID(@TableName)
        AND i.type_desc = 'CLUSTERED'
        AND i.is_primary_key = 0
        AND i.name IS NOT NULL;

    IF @indexName IS NOT NULL
    BEGIN
        SET @sql = 'DROP INDEX [' + @indexName + '] ON [' + @TableName + '];';
        EXEC sp_executesql @sql;
        PRINT '  ✓ Đã xóa clustered index cũ: ' + @indexName;
    END

    -- Bước 7: Thêm system-time columns mới
    SET @sql = 'ALTER TABLE [' + @TableName + ']
                ADD SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN DEFAULT GETUTCDATE(),
                    SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN DEFAULT ''9999-12-31 23:59:59.9999999'',
                    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);';
    EXEC sp_executesql @sql;
    PRINT '  ✓ Đã thêm system-time columns mới';

    -- Bước 8: Bật system versioning
    SET @sql = 'ALTER TABLE [' + @TableName + ']
                SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.[' + @historyTableName + ']));';
    EXEC sp_executesql @sql;
    PRINT '  ✓ Đã bật system versioning';

    -- Bước 9: Tạo columnstore index (nếu chưa có primary key clustered)
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(@TableName) AND is_primary_key = 1 AND type_desc = 'CLUSTERED')
    BEGIN
        SET @sql = 'CREATE CLUSTERED COLUMNSTORE INDEX CCI_' + @TableName + ' ON [' + @TableName + '];';
        EXEC sp_executesql @sql;
        PRINT '  ✓ Đã tạo clustered columnstore index';
    END
    ELSE
    BEGIN
        -- Tạo nonclustered columnstore index thay thế
        IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(@TableName) AND type_desc = 'NONCLUSTERED COLUMNSTORE')
        BEGIN
            SET @sql = 'CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_' + @TableName + ' ON [' + @TableName + '] ();';
            EXEC sp_executesql @sql;
            PRINT '  ✓ Đã tạo nonclustered columnstore index';
        END
    END

    PRINT '  🎉 Hoàn thành cấu hình bảng ' + @TableName;
    PRINT '';
END
GO
