-- 🚀 SCRIPT CHUYỂN ĐỔI CÁC BẢNG CÒN LẠI
-- Tiếp tục chuyển đổi để đạt 100% Temporal Tables
-- Ngày: 22/06/2025

USE TinhKhoanDB;
GO

PRINT '🚀 CHUYỂN ĐỔI CÁC BẢNG CÒN LẠI ĐỂ ĐẠT 100%'
PRINT '=========================================='

-- Danh sách các bảng còn lại cần chuyển đổi
DECLARE @tableName NVARCHAR(128)
DECLARE @sql NVARCHAR(MAX)

DECLARE remaining_tables CURSOR FOR
SELECT name FROM sys.tables 
WHERE temporal_type = 0  -- Chỉ bảng chưa là temporal
    AND name NOT IN ('__EFMigrationsHistory', 'sysdiagrams', 'TriggerBackup')
    AND name NOT LIKE '%History'  -- Loại trừ history tables
ORDER BY name

OPEN remaining_tables
FETCH NEXT FROM remaining_tables INTO @tableName

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT ''
    PRINT '🔄 Chuyển đổi bảng: ' + @tableName
    
    BEGIN TRY
        -- Thêm cột temporal
        PRINT '📝 Thêm cột temporal cho ' + @tableName + '...'
        SET @sql = 'ALTER TABLE ' + @tableName + ' ADD ' +
                  'ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(), ' +
                  'ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, ''9999-12-31 23:59:59.9999999''), ' +
                  'PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)'
        EXEC sp_executesql @sql
        
        -- Kích hoạt System Versioning
        PRINT '🔧 Kích hoạt System Versioning cho ' + @tableName + '...'
        SET @sql = 'ALTER TABLE ' + @tableName + ' SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.' + @tableName + 'History))'
        EXEC sp_executesql @sql
        
        PRINT '✅ ' + @tableName + ' đã chuyển đổi thành công!'
        
    END TRY
    BEGIN CATCH
        PRINT '❌ Lỗi khi chuyển đổi ' + @tableName + ': ' + ERROR_MESSAGE()
    END CATCH
    
    FETCH NEXT FROM remaining_tables INTO @tableName
END

CLOSE remaining_tables
DEALLOCATE remaining_tables

-- BÁO CÁO KẾT QUẢ CUỐI CÙNG
PRINT ''
PRINT '📊 BÁO CÁO KẾT QUẢ HOÀN CHỈNH'
PRINT '============================='

DECLARE @TotalTables INT, @TemporalTables INT, @CompletionRate INT

-- Đếm tổng số bảng (loại trừ system tables và history tables)
SELECT @TotalTables = COUNT(*) 
FROM sys.tables 
WHERE name NOT IN ('__EFMigrationsHistory', 'sysdiagrams', 'TriggerBackup')
    AND name NOT LIKE '%History'

-- Đếm Temporal Tables
SELECT @TemporalTables = COUNT(*) 
FROM sys.tables 
WHERE temporal_type = 2

SET @CompletionRate = (@TemporalTables * 100) / @TotalTables

PRINT 'Tổng số bảng cần chuyển đổi: ' + CAST(@TotalTables AS VARCHAR(10))
PRINT 'Đã chuyển thành Temporal Tables: ' + CAST(@TemporalTables AS VARCHAR(10))
PRINT 'Tỷ lệ hoàn thành: ' + CAST(@CompletionRate AS VARCHAR(10)) + '%'

IF @CompletionRate = 100
    PRINT '🎉 HOÀN THÀNH 100% CHUYỂN ĐỔI TEMPORAL TABLES!'
ELSE IF @CompletionRate >= 80
    PRINT '🌟 ĐẠT MỨC ĐỘ CAO: ' + CAST(@CompletionRate AS VARCHAR(10)) + '% hoàn thành!'
ELSE
    PRINT '⚠️ Cần tiếp tục chuyển đổi: ' + CAST(100 - @CompletionRate AS VARCHAR(10)) + '% còn lại'

-- Liệt kê TẤT CẢ Temporal Tables đã tạo
PRINT ''
PRINT '✅ DANH SÁCH TẤT CẢ TEMPORAL TABLES:'
DECLARE @counter INT = 1
DECLARE @tableName_display NVARCHAR(128)
DECLARE @historyName NVARCHAR(128)

DECLARE display_cursor CURSOR FOR
SELECT t.name, h.name
FROM sys.tables t
INNER JOIN sys.tables h ON t.history_table_id = h.object_id
WHERE t.temporal_type = 2
ORDER BY t.name

OPEN display_cursor
FETCH NEXT FROM display_cursor INTO @tableName_display, @historyName

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT CAST(@counter AS VARCHAR(3)) + '. ' + @tableName_display + ' → ' + @historyName
    SET @counter = @counter + 1
    FETCH NEXT FROM display_cursor INTO @tableName_display, @historyName
END

CLOSE display_cursor
DEALLOCATE display_cursor

-- Liệt kê bảng CHƯA chuyển đổi (nếu có)
DECLARE @remainingCount INT
SELECT @remainingCount = COUNT(*)
FROM sys.tables 
WHERE temporal_type = 0 
    AND name NOT IN ('__EFMigrationsHistory', 'sysdiagrams', 'TriggerBackup')
    AND name NOT LIKE '%History'

IF @remainingCount > 0
BEGIN
    PRINT ''
    PRINT '❌ BẢNG CHƯA CHUYỂN ĐỔI (' + CAST(@remainingCount AS VARCHAR(10)) + ' bảng):'
    
    DECLARE @counter2 INT = 1
    DECLARE remaining_display CURSOR FOR
    SELECT name
    FROM sys.tables 
    WHERE temporal_type = 0 
        AND name NOT IN ('__EFMigrationsHistory', 'sysdiagrams', 'TriggerBackup')
        AND name NOT LIKE '%History'
    ORDER BY name
    
    OPEN remaining_display
    FETCH NEXT FROM remaining_display INTO @tableName_display
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        PRINT CAST(@counter2 AS VARCHAR(3)) + '. ' + @tableName_display
        SET @counter2 = @counter2 + 1
        FETCH NEXT FROM remaining_display INTO @tableName_display
    END
    
    CLOSE remaining_display
    DEALLOCATE remaining_display
END

PRINT ''
PRINT '🚀 HOÀN THÀNH MIGRATION TEMPORAL TABLES!'
PRINT 'Thời gian hoàn thành: ' + CONVERT(VARCHAR, GETDATE(), 120)
PRINT '================================================'

GO
