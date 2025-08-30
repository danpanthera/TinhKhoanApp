-- 🚀 SCRIPT CHUYỂN ĐỔI THÔNG MINH TEMPORAL TABLES
-- Tự động phát hiện và xử lý trạng thái hiện tại
-- Ngày: 22/06/2025

USE TinhKhoanDB;
GO

PRINT '🚀 CHUYỂN ĐỔI THÔNG MINH SANG TEMPORAL TABLES'
PRINT '=============================================='

-- Hàm helper để chuyển đổi một bảng thành Temporal Table
DECLARE @sql NVARCHAR(MAX)
DECLARE @tableName NVARCHAR(128)
DECLARE @hasValidFrom BIT
DECLARE @hasValidTo BIT
DECLARE @hasPeriod BIT
DECLARE @isAlreadyTemporal BIT

-- Danh sách các bảng cần chuyển đổi
DECLARE table_cursor CURSOR FOR
SELECT name FROM sys.tables 
WHERE name IN ('Units', 'Positions', 'Roles', 'Employees', 'KPIDefinitions', 'KpiScoringRules', 'SalaryParameters', 'FinalPayouts', 'KhoanPeriods')
ORDER BY name

OPEN table_cursor
FETCH NEXT FROM table_cursor INTO @tableName

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT ''
    PRINT '🔄 Xử lý bảng: ' + @tableName
    
    -- Kiểm tra trạng thái hiện tại
    SELECT @isAlreadyTemporal = CASE WHEN temporal_type = 2 THEN 1 ELSE 0 END
    FROM sys.tables WHERE name = @tableName
    
    -- Kiểm tra có columns temporal không
    SELECT @hasValidFrom = CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(@tableName) AND name = 'ValidFrom'
    
    SELECT @hasValidTo = CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(@tableName) AND name = 'ValidTo'
    
    -- Kiểm tra có period không
    SELECT @hasPeriod = CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END
    FROM sys.periods 
    WHERE object_id = OBJECT_ID(@tableName)
    
    IF @isAlreadyTemporal = 1
    BEGIN
        PRINT '✅ ' + @tableName + ' đã là Temporal Table'
    END
    ELSE
    BEGIN
        -- Bắt đầu transaction để đảm bảo tính toàn vẹn
        BEGIN TRY
            BEGIN TRANSACTION
            
            -- Nếu chưa có cột ValidFrom/ValidTo
            IF @hasValidFrom = 0 OR @hasValidTo = 0
            BEGIN
                PRINT '📝 Thêm cột temporal cho ' + @tableName
                
                SET @sql = 'ALTER TABLE ' + @tableName + ' ADD ' +
                          'ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(), ' +
                          'ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, ''9999-12-31 23:59:59.9999999''), ' +
                          'PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)'
                
                EXEC sp_executesql @sql
                PRINT '✅ Đã thêm cột temporal'
            END
            ELSE IF @hasPeriod = 0
            BEGIN
                PRINT '📝 Thêm period definition cho ' + @tableName
                SET @sql = 'ALTER TABLE ' + @tableName + ' ADD PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)'
                EXEC sp_executesql @sql
                PRINT '✅ Đã thêm period definition'
            END
            
            -- Kích hoạt System Versioning
            PRINT '🔧 Kích hoạt System Versioning cho ' + @tableName
            SET @sql = 'ALTER TABLE ' + @tableName + ' SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.' + @tableName + 'History))'
            EXEC sp_executesql @sql
            
            -- Tạo Columnstore Index cho History Table
            PRINT '📊 Tạo Columnstore Index cho ' + @tableName + 'History'
            SET @sql = 'CREATE CLUSTERED COLUMNSTORE INDEX CCI_' + @tableName + 'History ON dbo.' + @tableName + 'History'
            EXEC sp_executesql @sql
            
            COMMIT TRANSACTION
            PRINT '🎉 ' + @tableName + ' chuyển đổi thành công!'
            
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION
            PRINT '❌ Lỗi khi chuyển đổi ' + @tableName + ': ' + ERROR_MESSAGE()
        END CATCH
    END
    
    FETCH NEXT FROM table_cursor INTO @tableName
END

CLOSE table_cursor
DEALLOCATE table_cursor

-- Bước 2: Chuyển đổi các bảng còn lại quan trọng
PRINT ''
PRINT '📊 CHUYỂN ĐỔI CÁC BẢNG QUAN TRỌNG KHÁC'
PRINT '====================================='

-- Danh sách bảng quan trọng khác
DECLARE table_cursor2 CURSOR FOR
SELECT name FROM sys.tables 
WHERE name IN ('EmployeeKhoanAssignments', 'EmployeeKpiAssignments', 'DashboardIndicators', 'BusinessPlanTargets', 'DashboardCalculations', 'ImportedDataRecords')
ORDER BY name

OPEN table_cursor2
FETCH NEXT FROM table_cursor2 INTO @tableName

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT ''
    PRINT '🔄 Xử lý bảng: ' + @tableName
    
    SELECT @isAlreadyTemporal = CASE WHEN temporal_type = 2 THEN 1 ELSE 0 END
    FROM sys.tables WHERE name = @tableName
    
    IF @isAlreadyTemporal = 1
    BEGIN
        PRINT '✅ ' + @tableName + ' đã là Temporal Table'
    END
    ELSE
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION
            
            PRINT '📝 Thêm cột temporal cho ' + @tableName
            SET @sql = 'ALTER TABLE ' + @tableName + ' ADD ' +
                      'ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(), ' +
                      'ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, ''9999-12-31 23:59:59.9999999''), ' +
                      'PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)'
            
            EXEC sp_executesql @sql
            
            PRINT '🔧 Kích hoạt System Versioning cho ' + @tableName
            SET @sql = 'ALTER TABLE ' + @tableName + ' SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.' + @tableName + 'History))'
            EXEC sp_executesql @sql
            
            PRINT '📊 Tạo Columnstore Index cho ' + @tableName + 'History'
            SET @sql = 'CREATE CLUSTERED COLUMNSTORE INDEX CCI_' + @tableName + 'History ON dbo.' + @tableName + 'History'
            EXEC sp_executesql @sql
            
            COMMIT TRANSACTION
            PRINT '🎉 ' + @tableName + ' chuyển đổi thành công!'
            
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION
            PRINT '❌ Lỗi khi chuyển đổi ' + @tableName + ': ' + ERROR_MESSAGE()
        END CATCH
    END
    
    FETCH NEXT FROM table_cursor2 INTO @tableName
END

CLOSE table_cursor2
DEALLOCATE table_cursor2

-- BÁO CÁO KẾT QUẢ CUỐI CÙNG
PRINT ''
PRINT '📊 BÁO CÁO KẾT QUẢ CHUYỂN ĐỔI CUỐI CÙNG'
PRINT '======================================='

DECLARE @TotalTables INT, @TemporalTables INT, @NonTemporalTables INT
SELECT @TotalTables = COUNT(*) FROM sys.tables WHERE name NOT IN ('__EFMigrationsHistory', 'sysdiagrams', 'TriggerBackup')
SELECT @TemporalTables = COUNT(*) FROM sys.tables WHERE temporal_type = 2
SELECT @NonTemporalTables = @TotalTables - @TemporalTables

PRINT 'Tổng số tables: ' + CAST(@TotalTables AS VARCHAR(10))
PRINT 'Đã chuyển đổi thành Temporal Tables: ' + CAST(@TemporalTables AS VARCHAR(10))
PRINT 'Chưa chuyển đổi: ' + CAST(@NonTemporalTables AS VARCHAR(10))

DECLARE @CompletionPercentage INT = (@TemporalTables * 100 / @TotalTables)
PRINT 'Tỷ lệ hoàn thành: ' + CAST(@CompletionPercentage AS VARCHAR(10)) + '%'

IF @CompletionPercentage >= 80
    PRINT '🎉 CHUYỂN ĐỔI THÀNH CÔNG! Đạt ' + CAST(@CompletionPercentage AS VARCHAR(10)) + '% mục tiêu!'
ELSE
    PRINT '⚠️ Cần tiếp tục chuyển đổi các bảng còn lại'

-- Liệt kê danh sách Temporal Tables
PRINT ''
PRINT 'DANH SÁCH TEMPORAL TABLES ĐÃ TẠO:'
SELECT 
    '✅ ' + t.name + ' (History: ' + h.name + ')' AS TemporalTableInfo
FROM sys.tables t
INNER JOIN sys.tables h ON t.history_table_id = h.object_id
WHERE t.temporal_type = 2
ORDER BY t.name

-- Liệt kê bảng chưa chuyển đổi
PRINT ''
PRINT 'BẢNG CHƯA CHUYỂN ĐỔI:'
SELECT 
    '❌ ' + name AS NonTemporalTable
FROM sys.tables 
WHERE temporal_type = 0 
    AND name NOT IN ('__EFMigrationsHistory', 'sysdiagrams', 'TriggerBackup')
ORDER BY name

PRINT ''
PRINT '🚀 HOÀN THÀNH CHUYỂN ĐỔI THÔNG MINH!'
PRINT 'Thời gian: ' + CONVERT(VARCHAR, GETDATE(), 120)

GO
