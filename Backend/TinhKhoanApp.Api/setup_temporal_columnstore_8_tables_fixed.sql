-- 📊 THIẾT LẬP TEMPORAL TABLES + COLUMNSTORE INDEXES CHO 8 BẢNG CORE (Fixed)
-- Ngày: July 13, 2025
-- Fix: Xử lý IDENTITY columns trong history tables

USE TinhKhoanDB;
GO

PRINT '🚀 Bắt đầu thiết lập Temporal Tables + Columnstore cho 8 bảng core (Fixed)...';

-- ============================================================================
-- HELPER PROCEDURE: Tạo Temporal Table cho bảng bất kỳ
-- ============================================================================
IF OBJECT_ID('sp_CreateTemporalTable', 'P') IS NOT NULL
    DROP PROCEDURE sp_CreateTemporalTable;
GO

CREATE PROCEDURE sp_CreateTemporalTable
    @TableName NVARCHAR(128)
AS
BEGIN
    DECLARE @sql NVARCHAR(MAX);
    DECLARE @historyTableName NVARCHAR(128) = @TableName + '_History';

    PRINT '📋 Thiết lập ' + @TableName + '...';

    -- 1. Thêm các cột Temporal nếu chưa có
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(@TableName) AND name = 'SysStartTime')
    BEGIN
        SET @sql = 'ALTER TABLE ' + @TableName + ' ADD
            SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
            SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT ''9999-12-31 23:59:59.9999999'',
            PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);';
        EXEC sp_executesql @sql;
    END;

    -- 2. Tạo History Table với cấu trúc tương tự nhưng không có IDENTITY
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = @historyTableName)
    BEGIN
        -- Lấy danh sách columns (trừ IDENTITY)
        DECLARE @columnList NVARCHAR(MAX) = '';
        SELECT @columnList = @columnList +
            CASE
                WHEN is_identity = 1 THEN '[' + name + '] ' +
                    CASE
                        WHEN system_type_id = 56 THEN 'int'  -- int
                        WHEN system_type_id = 127 THEN 'bigint'  -- bigint
                        ELSE 'int'
                    END
                ELSE '[' + name + '] ' +
                    CASE
                        WHEN system_type_id = 56 THEN 'int'
                        WHEN system_type_id = 127 THEN 'bigint'
                        WHEN system_type_id = 231 THEN 'nvarchar(' + CAST(max_length/2 AS varchar) + ')'
                        WHEN system_type_id = 167 THEN 'varchar(' + CAST(max_length AS varchar) + ')'
                        WHEN system_type_id = 61 THEN 'datetime'
                        WHEN system_type_id = 42 THEN 'datetime2'
                        WHEN system_type_id = 104 THEN 'bit'
                        WHEN system_type_id = 106 THEN 'decimal(18,2)'
                        WHEN system_type_id = 108 THEN 'decimal(18,2)'
                        ELSE 'nvarchar(max)'
                    END
            END +
            CASE WHEN is_nullable = 1 THEN ' NULL' ELSE ' NOT NULL' END + ', '
        FROM sys.columns
        WHERE object_id = OBJECT_ID(@TableName)
          AND name NOT IN ('SysStartTime', 'SysEndTime')
        ORDER BY column_id;

        -- Thêm temporal columns
        SET @columnList = @columnList + 'SysStartTime datetime2 NOT NULL, SysEndTime datetime2 NOT NULL';

        -- Tạo history table
        SET @sql = 'CREATE TABLE ' + @historyTableName + ' (' + @columnList + ');';
        EXEC sp_executesql @sql;

        -- Tạo clustered index cho history table
        SET @sql = 'CREATE CLUSTERED INDEX IX_' + @TableName + '_History_Period ON ' + @historyTableName + ' (SysEndTime, SysStartTime, Id);';
        EXEC sp_executesql @sql;
    END;

    -- 3. Bật System Versioning
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = @TableName AND temporal_type = 2)
    BEGIN
        SET @sql = 'ALTER TABLE ' + @TableName + ' SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.' + @historyTableName + '));';
        EXEC sp_executesql @sql;
        PRINT '✅ ' + @TableName + ': Temporal Table enabled';
    END;

    -- 4. Tạo Columnstore Index trên history table
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'CCI_' + @TableName AND object_id = OBJECT_ID(@historyTableName))
    BEGIN
        SET @sql = 'CREATE CLUSTERED COLUMNSTORE INDEX CCI_' + @TableName + ' ON ' + @historyTableName + ';';
        EXEC sp_executesql @sql;
        PRINT '✅ ' + @TableName + ': Columnstore Index created';
    END;
END;
GO

-- ============================================================================
-- THIẾT LẬP CHO TẤT CẢ 8 BẢNG CORE
-- ============================================================================

-- 1. DP01 - Dữ liệu Tiền gửi
EXEC sp_CreateTemporalTable 'DP01';

-- 2. LN01 - Dữ liệu Cho vay
EXEC sp_CreateTemporalTable 'LN01';

-- 3. LN03 - Dữ liệu Nợ XLRR
EXEC sp_CreateTemporalTable 'LN03';

-- 4. GL01 - Dữ liệu Bút toán GDV
EXEC sp_CreateTemporalTable 'GL01';

-- 5. GL41 - Bảng cân đối kế toán
EXEC sp_CreateTemporalTable 'GL41';

-- 6. DPDA - Dữ liệu sao kê phát hành thẻ
EXEC sp_CreateTemporalTable 'DPDA';

-- 7. EI01 - Dữ liệu Mobile Banking
EXEC sp_CreateTemporalTable 'EI01';

-- 8. RR01 - Sao kê dư nợ gốc, lãi XLRR
EXEC sp_CreateTemporalTable 'RR01';

-- ============================================================================
-- TỔNG KẾT KIỂM TRA
-- ============================================================================
PRINT '📊 Kiểm tra kết quả thiết lập...';

SELECT
    t.name AS TABLE_NAME,
    CASE WHEN t.temporal_type = 2 THEN '✅ YES' ELSE '❌ NO' END AS Is_Temporal,
    CASE
        WHEN t.history_table_id IS NOT NULL
        THEN OBJECT_SCHEMA_NAME(t.history_table_id) + '.' + OBJECT_NAME(t.history_table_id)
        ELSE '❌ NO HISTORY'
    END AS History_Table,
    ISNULL((SELECT COUNT(*) FROM sys.indexes i WHERE i.object_id = t.history_table_id AND i.type_desc LIKE '%COLUMNSTORE%'), 0) AS Columnstore_Count
FROM sys.tables t
WHERE t.name IN ('DP01', 'LN01', 'LN03', 'GL01', 'GL41', 'DPDA', 'EI01', 'RR01')
ORDER BY t.name;

-- Clean up
DROP PROCEDURE sp_CreateTemporalTable;

PRINT '🎉 Hoàn thành thiết lập Temporal Tables + Columnstore cho 8 bảng core!';
