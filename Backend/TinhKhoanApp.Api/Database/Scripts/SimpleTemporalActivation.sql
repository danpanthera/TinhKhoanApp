-- 🎯 SCRIPT ĐƠN GIẢN: CHỈ KÍCH HOẠT TEMPORAL TABLES
-- Không tạo Columnstore Index để tránh xung đột
-- Ngày: 22/06/2025

USE TinhKhoanDB;
GO

PRINT '🎯 KÍCH HOẠT TEMPORAL TABLES ĐơN GIẢN'
PRINT '=================================='

-- Hàm helper để chuyển đổi từng bảng
DECLARE @tableName NVARCHAR(128)
DECLARE @sql NVARCHAR(MAX)

-- Danh sách bảng cần chuyển đổi ưu tiên
DECLARE table_cursor CURSOR FOR
SELECT name FROM sys.tables 
WHERE name IN (
    'Units', 'Positions', 'Roles', 'Employees',           -- Core tables
    'KPIDefinitions', 'KpiScoringRules',                  -- KPI system
    'SalaryParameters', 'FinalPayouts',                   -- Financial system
    'KhoanPeriods', 'EmployeeKhoanAssignments',           -- Assignment system
    'ImportedDataRecords', 'DashboardIndicators'         -- Data & Dashboard
)
    AND temporal_type = 0  -- Chỉ bảng chưa là temporal
ORDER BY 
    CASE name
        WHEN 'Units' THEN 1
        WHEN 'Positions' THEN 2
        WHEN 'Roles' THEN 3
        WHEN 'Employees' THEN 4
        ELSE 10
    END

OPEN table_cursor
FETCH NEXT FROM table_cursor INTO @tableName

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT ''
    PRINT '🔄 Xử lý bảng: ' + @tableName
    
    BEGIN TRY
        -- Bước 1: Thêm cột temporal nếu chưa có
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(@tableName) AND name = 'ValidFrom')
        BEGIN
            PRINT '📝 Thêm cột temporal...'
            SET @sql = 'ALTER TABLE ' + @tableName + ' ADD ' +
                      'ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(), ' +
                      'ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, ''9999-12-31 23:59:59.9999999''), ' +
                      'PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)'
            EXEC sp_executesql @sql
            PRINT '✅ Đã thêm cột temporal'
        END
        ELSE
        BEGIN
            PRINT 'ℹ️ Cột temporal đã tồn tại'
            -- Kiểm tra xem có period không
            IF NOT EXISTS (SELECT * FROM sys.periods WHERE object_id = OBJECT_ID(@tableName))
            BEGIN
                PRINT '📝 Thêm period definition...'
                SET @sql = 'ALTER TABLE ' + @tableName + ' ADD PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)'
                EXEC sp_executesql @sql
                PRINT '✅ Đã thêm period definition'
            END
        END
        
        -- Bước 2: Kích hoạt System Versioning (KHÔNG tạo Columnstore Index)
        PRINT '🔧 Kích hoạt System Versioning...'
        SET @sql = 'ALTER TABLE ' + @tableName + ' SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.' + @tableName + 'History))'
        EXEC sp_executesql @sql
        
        PRINT '🎉 ' + @tableName + ' đã trở thành Temporal Table!'
        
    END TRY
    BEGIN CATCH
        PRINT '❌ Lỗi: ' + ERROR_MESSAGE()
    END CATCH
    
    FETCH NEXT FROM table_cursor INTO @tableName
END

CLOSE table_cursor
DEALLOCATE table_cursor

-- Kiểm tra kết quả
PRINT ''
PRINT '📊 KẾT QUẢ CHUYỂN ĐỔI'
PRINT '==================='

DECLARE @TotalTables INT, @TemporalTables INT
SELECT @TotalTables = COUNT(*) FROM sys.tables WHERE name NOT IN ('__EFMigrationsHistory', 'sysdiagrams', 'TriggerBackup')
SELECT @TemporalTables = COUNT(*) FROM sys.tables WHERE temporal_type = 2

PRINT 'Tổng số tables: ' + CAST(@TotalTables AS VARCHAR(10))
PRINT 'Temporal Tables: ' + CAST(@TemporalTables AS VARCHAR(10))
PRINT 'Tỷ lệ hoàn thành: ' + CAST((@TemporalTables * 100 / @TotalTables) AS VARCHAR(10)) + '%'

-- Liệt kê các Temporal Tables
IF @TemporalTables > 0
BEGIN
    PRINT ''
    PRINT 'TEMPORAL TABLES ĐÃ TẠO:'
    SELECT 
        '✅ ' + t.name + ' → ' + h.name AS TemporalInfo
    FROM sys.tables t
    INNER JOIN sys.tables h ON t.history_table_id = h.object_id
    WHERE t.temporal_type = 2
    ORDER BY t.name
END

PRINT ''
PRINT '🚀 HOÀN THÀNH!'
PRINT 'Thời gian: ' + CONVERT(VARCHAR, GETDATE(), 120)

GO
