-- 🚀 SCRIPT CHUYỂN ĐỔI AN TOÀN TỪNG BẢNG THÀNH TEMPORAL TABLES
-- Author: AI Assistant - Ngày: 22/06/2025

USE TinhKhoanDB;
GO

PRINT '🚀 BẮT ĐẦU CHUYỂN ĐỔI AN TOÀN SANG TEMPORAL TABLES'
PRINT '=================================================='

-- BƯỚC 1: XỬ LÝ TRIGGER CẢN TRỞ
PRINT '🔧 Bước 1: Xử lý triggers cản trở...'

-- Backup và xóa trigger trên Units
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'PreventNewUnitCreation')
BEGIN
    PRINT '📝 Backup và xóa trigger PreventNewUnitCreation...'
    
    -- Lưu định nghĩa trigger để restore sau
    DECLARE @TriggerDef NVARCHAR(MAX)
    SELECT @TriggerDef = OBJECT_DEFINITION(OBJECT_ID('PreventNewUnitCreation'))
    
    -- Tạo bảng backup trigger definitions nếu chưa có
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TriggerBackup')
    BEGIN
        CREATE TABLE TriggerBackup (
            TriggerName NVARCHAR(128),
            TableName NVARCHAR(128),
            Definition NVARCHAR(MAX),
            BackupDate DATETIME2 DEFAULT GETDATE()
        )
    END
    
    -- Backup definition
    INSERT INTO TriggerBackup (TriggerName, TableName, Definition)
    VALUES ('PreventNewUnitCreation', 'Units', @TriggerDef)
    
    -- Xóa trigger
    DROP TRIGGER PreventNewUnitCreation
    PRINT '✅ Đã backup và xóa trigger PreventNewUnitCreation'
END

-- BƯỚC 2: CHUYỂN ĐỔI CÁC BẢNG CORE SYSTEM
PRINT ''
PRINT '🔄 Bước 2: Chuyển đổi các bảng Core System...'

-- Chuyển đổi Units
BEGIN TRY
    PRINT '⚙️ Chuyển đổi bảng Units...'
    
    -- Thêm period columns
    ALTER TABLE Units ADD 
        ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    
    -- Kích hoạt system versioning
    ALTER TABLE Units SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.UnitsHistory))
    
    -- Tạo columnstore index cho history table
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_UnitsHistory ON dbo.UnitsHistory
    
    PRINT '✅ Units chuyển đổi thành công!'
END TRY
BEGIN CATCH
    PRINT '❌ Lỗi khi chuyển đổi Units: ' + ERROR_MESSAGE()
END CATCH

-- Chuyển đổi Positions  
BEGIN TRY
    PRINT '⚙️ Chuyển đổi bảng Positions...'
    
    ALTER TABLE Positions ADD 
        ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    
    ALTER TABLE Positions SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.PositionsHistory))
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_PositionsHistory ON dbo.PositionsHistory
    
    PRINT '✅ Positions chuyển đổi thành công!'
END TRY
BEGIN CATCH
    PRINT '❌ Lỗi khi chuyển đổi Positions: ' + ERROR_MESSAGE()
END CATCH

-- Chuyển đổi Roles
BEGIN TRY
    PRINT '⚙️ Chuyển đổi bảng Roles...'
    
    ALTER TABLE Roles ADD 
        ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    
    ALTER TABLE Roles SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.RolesHistory))
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_RolesHistory ON dbo.RolesHistory
    
    PRINT '✅ Roles chuyển đổi thành công!'
END TRY
BEGIN CATCH
    PRINT '❌ Lỗi khi chuyển đổi Roles: ' + ERROR_MESSAGE()
END CATCH

-- BƯỚC 3: CHUYỂN ĐỔI CÁC BẢNG EMPLOYEES SYSTEM
PRINT ''
PRINT '👥 Bước 3: Chuyển đổi bảng Employees System...'

-- Chuyển đổi Employees
BEGIN TRY
    PRINT '⚙️ Chuyển đổi bảng Employees...'
    
    ALTER TABLE Employees ADD 
        ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    
    ALTER TABLE Employees SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.EmployeesHistory))
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_EmployeesHistory ON dbo.EmployeesHistory
    
    PRINT '✅ Employees chuyển đổi thành công!'
END TRY
BEGIN CATCH
    PRINT '❌ Lỗi khi chuyển đổi Employees: ' + ERROR_MESSAGE()
END CATCH

-- BƯỚC 4: CHUYỂN ĐỔI CÁC BẢNG KPI SYSTEM
PRINT ''
PRINT '📊 Bước 4: Chuyển đổi bảng KPI System...'

-- Chuyển đổi KPIDefinitions
BEGIN TRY
    PRINT '⚙️ Chuyển đổi bảng KPIDefinitions...'
    
    ALTER TABLE KPIDefinitions ADD 
        ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    
    ALTER TABLE KPIDefinitions SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.KPIDefinitionsHistory))
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_KPIDefinitionsHistory ON dbo.KPIDefinitionsHistory
    
    PRINT '✅ KPIDefinitions chuyển đổi thành công!'
END TRY
BEGIN CATCH
    PRINT '❌ Lỗi khi chuyển đổi KPIDefinitions: ' + ERROR_MESSAGE()
END CATCH

-- Chuyển đổi KpiScoringRules
BEGIN TRY
    PRINT '⚙️ Chuyển đổi bảng KpiScoringRules...'
    
    ALTER TABLE KpiScoringRules ADD 
        ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    
    ALTER TABLE KpiScoringRules SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.KpiScoringRulesHistory))
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_KpiScoringRulesHistory ON dbo.KpiScoringRulesHistory
    
    PRINT '✅ KpiScoringRules chuyển đổi thành công!'
END TRY
BEGIN CATCH
    PRINT '❌ Lỗi khi chuyển đổi KpiScoringRules: ' + ERROR_MESSAGE()
END CATCH

-- BƯỚC 5: CHUYỂN ĐỔI CÁC BẢNG FINANCIAL SYSTEM
PRINT ''
PRINT '💰 Bước 5: Chuyển đổi bảng Financial System...'

-- Chuyển đổi SalaryParameters
BEGIN TRY
    PRINT '⚙️ Chuyển đổi bảng SalaryParameters...'
    
    ALTER TABLE SalaryParameters ADD 
        ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    
    ALTER TABLE SalaryParameters SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.SalaryParametersHistory))
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_SalaryParametersHistory ON dbo.SalaryParametersHistory
    
    PRINT '✅ SalaryParameters chuyển đổi thành công!'
END TRY
BEGIN CATCH
    PRINT '❌ Lỗi khi chuyển đổi SalaryParameters: ' + ERROR_MESSAGE()
END CATCH

-- Chuyển đổi FinalPayouts
BEGIN TRY
    PRINT '⚙️ Chuyển đổi bảng FinalPayouts...'
    
    ALTER TABLE FinalPayouts ADD 
        ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    
    ALTER TABLE FinalPayouts SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.FinalPayoutsHistory))
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_FinalPayoutsHistory ON dbo.FinalPayoutsHistory
    
    PRINT '✅ FinalPayouts chuyển đổi thành công!'
END TRY
BEGIN CATCH
    PRINT '❌ Lỗi khi chuyển đổi FinalPayouts: ' + ERROR_MESSAGE()
END CATCH

-- BƯỚC 6: CHUYỂN ĐỔI CÁC BẢNG KHOAN PERIODS
PRINT ''
PRINT '📅 Bước 6: Chuyển đổi bảng Khoan Periods...'

-- Chuyển đổi KhoanPeriods
BEGIN TRY
    PRINT '⚙️ Chuyển đổi bảng KhoanPeriods...'
    
    ALTER TABLE KhoanPeriods ADD 
        ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    
    ALTER TABLE KhoanPeriods SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.KhoanPeriodsHistory))
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_KhoanPeriodsHistory ON dbo.KhoanPeriodsHistory
    
    PRINT '✅ KhoanPeriods chuyển đổi thành công!'
END TRY
BEGIN CATCH
    PRINT '❌ Lỗi khi chuyển đổi KhoanPeriods: ' + ERROR_MESSAGE()
END CATCH

-- BƯỚC 7: KIỂM TRA KẾT QUẢ
PRINT ''
PRINT '📊 BÁO CÁO KẾT QUẢ CHUYỂN ĐỔI'
PRINT '================================='

-- Đếm số Temporal Tables sau chuyển đổi
DECLARE @TotalTables INT, @TemporalTables INT, @NonTemporalTables INT
SELECT @TotalTables = COUNT(*) FROM sys.tables WHERE name NOT IN ('__EFMigrationsHistory', 'sysdiagrams', 'TriggerBackup')
SELECT @TemporalTables = COUNT(*) FROM sys.tables WHERE temporal_type = 2
SELECT @NonTemporalTables = @TotalTables - @TemporalTables

PRINT 'Tổng số tables: ' + CAST(@TotalTables AS VARCHAR(10))
PRINT 'Đã chuyển đổi thành Temporal Tables: ' + CAST(@TemporalTables AS VARCHAR(10))
PRINT 'Chưa chuyển đổi: ' + CAST(@NonTemporalTables AS VARCHAR(10))
PRINT 'Tỷ lệ hoàn thành: ' + CAST((@TemporalTables * 100 / @TotalTables) AS VARCHAR(10)) + '%'

-- Liệt kê các Temporal Tables đã tạo
PRINT ''
PRINT 'Danh sách Temporal Tables đã tạo:'
SELECT 
    t.name AS TableName,
    h.name AS HistoryTableName
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
WHERE t.temporal_type = 2
ORDER BY t.name

PRINT ''
PRINT '🎉 HOÀN THÀNH GIAI ĐOẠN 1 - CHUYỂN ĐỔI CÁC BẢNG CƠ BẢN!'
PRINT 'Thời gian: ' + CONVERT(VARCHAR, GETDATE(), 120)

GO
