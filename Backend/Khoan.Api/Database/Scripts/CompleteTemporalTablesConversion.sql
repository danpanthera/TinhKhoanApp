-- 🚀 SCRIPT CHUYỂN ĐỔI HOÀN CHỈNH 100% SANG TEMPORAL TABLES + COLUMNSTORE INDEXES
-- Author: AI Assistant - Ngày: 22/06/2025
-- Mục tiêu: Chuyển đổi tất cả các bảng còn lại thành Temporal Tables với Columnstore Indexes

USE TinhKhoanDB;
GO

-- ================================
-- BƯỚC 1: KHỞI TẠO VARIABLES
-- ================================
DECLARE @sql NVARCHAR(MAX) = ''
DECLARE @tableName NVARCHAR(128)
DECLARE @historyTableName NVARCHAR(128)

PRINT '🚀 BẮTĐẦU CHUYỂN ĐỔI 100% SANG TEMPORAL TABLES + COLUMNSTORE'
PRINT '================================================================='
PRINT CONVERT(VARCHAR, GETDATE(), 120) + ' - Khởi tạo script chuyển đổi'

-- ================================
-- BƯỚC 2: CHUYỂN ĐỔI CÁC BẢNG CORE SYSTEM
-- ================================

-- 📋 TABLE: Units (Đơn vị)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Units' AND temporal_type = 2)
BEGIN
    PRINT '🔄 Chuyển đổi bảng Units thành Temporal Table...'
    
    -- Thêm cột period nếu chưa có
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Units') AND name = 'ValidFrom')
    BEGIN
        ALTER TABLE Units ADD 
            ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
            ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    END
    
    -- Kích hoạt Temporal Table
    ALTER TABLE Units SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.UnitsHistory))
    
    -- Tạo Columnstore Index cho history table
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'UnitsHistory')
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX CCI_UnitsHistory ON dbo.UnitsHistory
    END
    
    PRINT '✅ Units chuyển đổi thành công!'
END
ELSE
    PRINT 'ℹ️ Units đã là Temporal Table'

-- 📋 TABLE: Positions (Chức vụ)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Positions' AND temporal_type = 2)
BEGIN
    PRINT '🔄 Chuyển đổi bảng Positions thành Temporal Table...'
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Positions') AND name = 'ValidFrom')
    BEGIN
        ALTER TABLE Positions ADD 
            ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
            ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    END
    
    ALTER TABLE Positions SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.PositionsHistory))
    
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'PositionsHistory')
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX CCI_PositionsHistory ON dbo.PositionsHistory
    END
    
    PRINT '✅ Positions chuyển đổi thành công!'
END
ELSE
    PRINT 'ℹ️ Positions đã là Temporal Table'

-- 📋 TABLE: Employees (Nhân viên)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Employees' AND temporal_type = 2)
BEGIN
    PRINT '🔄 Chuyển đổi bảng Employees thành Temporal Table...'
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Employees') AND name = 'ValidFrom')
    BEGIN
        ALTER TABLE Employees ADD 
            ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
            ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    END
    
    ALTER TABLE Employees SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.EmployeesHistory))
    
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EmployeesHistory')
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX CCI_EmployeesHistory ON dbo.EmployeesHistory
    END
    
    PRINT '✅ Employees chuyển đổi thành công!'
END
ELSE
    PRINT 'ℹ️ Employees đã là Temporal Table'

-- 📋 TABLE: Roles (Vai trò)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Roles' AND temporal_type = 2)
BEGIN
    PRINT '🔄 Chuyển đổi bảng Roles thành Temporal Table...'
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Roles') AND name = 'ValidFrom')
    BEGIN
        ALTER TABLE Roles ADD 
            ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
            ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    END
    
    ALTER TABLE Roles SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.RolesHistory))
    
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'RolesHistory')
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX CCI_RolesHistory ON dbo.RolesHistory
    END
    
    PRINT '✅ Roles chuyển đổi thành công!'
END
ELSE
    PRINT 'ℹ️ Roles đã là Temporal Table'

-- ================================
-- BƯỚC 3: CHUYỂN ĐỔI CÁC BẢNG KPI SYSTEM
-- ================================

-- 📋 TABLE: KPIDefinitions
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'KPIDefinitions' AND temporal_type = 2)
BEGIN
    PRINT '🔄 Chuyển đổi bảng KPIDefinitions thành Temporal Table...'
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KPIDefinitions') AND name = 'ValidFrom')
    BEGIN
        ALTER TABLE KPIDefinitions ADD 
            ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
            ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    END
    
    ALTER TABLE KPIDefinitions SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.KPIDefinitionsHistory))
    
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'KPIDefinitionsHistory')
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX CCI_KPIDefinitionsHistory ON dbo.KPIDefinitionsHistory
    END
    
    PRINT '✅ KPIDefinitions chuyển đổi thành công!'
END
ELSE
    PRINT 'ℹ️ KPIDefinitions đã là Temporal Table'

-- 📋 TABLE: EmployeeKpiAssignments
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EmployeeKpiAssignments' AND temporal_type = 2)
BEGIN
    PRINT '🔄 Chuyển đổi bảng EmployeeKpiAssignments thành Temporal Table...'
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmployeeKpiAssignments') AND name = 'ValidFrom')
    BEGIN
        ALTER TABLE EmployeeKpiAssignments ADD 
            ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
            ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    END
    
    ALTER TABLE EmployeeKpiAssignments SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.EmployeeKpiAssignmentsHistory))
    
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EmployeeKpiAssignmentsHistory')
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX CCI_EmployeeKpiAssignmentsHistory ON dbo.EmployeeKpiAssignmentsHistory
    END
    
    PRINT '✅ EmployeeKpiAssignments chuyển đổi thành công!'
END
ELSE
    PRINT 'ℹ️ EmployeeKpiAssignments đã là Temporal Table'

-- 📋 TABLE: KpiScoringRules
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'KpiScoringRules' AND temporal_type = 2)
BEGIN
    PRINT '🔄 Chuyển đổi bảng KpiScoringRules thành Temporal Table...'
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KpiScoringRules') AND name = 'ValidFrom')
    BEGIN
        ALTER TABLE KpiScoringRules ADD 
            ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
            ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    END
    
    ALTER TABLE KpiScoringRules SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.KpiScoringRulesHistory))
    
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'KpiScoringRulesHistory')
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX CCI_KpiScoringRulesHistory ON dbo.KpiScoringRulesHistory
    END
    
    PRINT '✅ KpiScoringRules chuyển đổi thành công!'
END
ELSE
    PRINT 'ℹ️ KpiScoringRules đã là Temporal Table'

-- ================================
-- BƯỚC 4: CHUYỂN ĐỔI CÁC BẢNG FINANCIAL SYSTEM
-- ================================

-- 📋 TABLE: SalaryParameters
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SalaryParameters' AND temporal_type = 2)
BEGIN
    PRINT '🔄 Chuyển đổi bảng SalaryParameters thành Temporal Table...'
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SalaryParameters') AND name = 'ValidFrom')
    BEGIN
        ALTER TABLE SalaryParameters ADD 
            ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
            ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    END
    
    ALTER TABLE SalaryParameters SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.SalaryParametersHistory))
    
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'SalaryParametersHistory')
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX CCI_SalaryParametersHistory ON dbo.SalaryParametersHistory
    END
    
    PRINT '✅ SalaryParameters chuyển đổi thành công!'
END
ELSE
    PRINT 'ℹ️ SalaryParameters đã là Temporal Table'

-- 📋 TABLE: FinalPayouts
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FinalPayouts' AND temporal_type = 2)
BEGIN
    PRINT '🔄 Chuyển đổi bảng FinalPayouts thành Temporal Table...'
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FinalPayouts') AND name = 'ValidFrom')
    BEGIN
        ALTER TABLE FinalPayouts ADD 
            ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
            ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    END
    
    ALTER TABLE FinalPayouts SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.FinalPayoutsHistory))
    
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'FinalPayoutsHistory')
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX CCI_FinalPayoutsHistory ON dbo.FinalPayoutsHistory
    END
    
    PRINT '✅ FinalPayouts chuyển đổi thành công!'
END
ELSE
    PRINT 'ℹ️ FinalPayouts đã là Temporal Table'

-- ================================
-- BƯỚC 5: CHUYỂN ĐỔI CÁC BẢNG ASSIGNMENT SYSTEM
-- ================================

-- 📋 TABLE: KhoanPeriods
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'KhoanPeriods' AND temporal_type = 2)
BEGIN
    PRINT '🔄 Chuyển đổi bảng KhoanPeriods thành Temporal Table...'
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KhoanPeriods') AND name = 'ValidFrom')
    BEGIN
        ALTER TABLE KhoanPeriods ADD 
            ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
            ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    END
    
    ALTER TABLE KhoanPeriods SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.KhoanPeriodsHistory))
    
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'KhoanPeriodsHistory')
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX CCI_KhoanPeriodsHistory ON dbo.KhoanPeriodsHistory
    END
    
    PRINT '✅ KhoanPeriods chuyển đổi thành công!'
END
ELSE
    PRINT 'ℹ️ KhoanPeriods đã là Temporal Table'

-- 📋 TABLE: EmployeeKhoanAssignments
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EmployeeKhoanAssignments' AND temporal_type = 2)
BEGIN
    PRINT '🔄 Chuyển đổi bảng EmployeeKhoanAssignments thành Temporal Table...'
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmployeeKhoanAssignments') AND name = 'ValidFrom')
    BEGIN
        ALTER TABLE EmployeeKhoanAssignments ADD 
            ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
            ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    END
    
    ALTER TABLE EmployeeKhoanAssignments SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.EmployeeKhoanAssignmentsHistory))
    
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EmployeeKhoanAssignmentsHistory')
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX CCI_EmployeeKhoanAssignmentsHistory ON dbo.EmployeeKhoanAssignmentsHistory
    END
    
    PRINT '✅ EmployeeKhoanAssignments chuyển đổi thành công!'
END
ELSE
    PRINT 'ℹ️ EmployeeKhoanAssignments đã là Temporal Table'

-- ================================
-- BƯỚC 6: CHUYỂN ĐỔI CÁC BẢNG DASHBOARD SYSTEM  
-- ================================

-- 📋 TABLE: DashboardIndicators
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DashboardIndicators' AND temporal_type = 2)
BEGIN
    PRINT '🔄 Chuyển đổi bảng DashboardIndicators thành Temporal Table...'
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DashboardIndicators') AND name = 'ValidFrom')
    BEGIN
        ALTER TABLE DashboardIndicators ADD 
            ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
            ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    END
    
    ALTER TABLE DashboardIndicators SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.DashboardIndicatorsHistory))
    
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DashboardIndicatorsHistory')
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX CCI_DashboardIndicatorsHistory ON dbo.DashboardIndicatorsHistory
    END
    
    PRINT '✅ DashboardIndicators chuyển đổi thành công!'
END
ELSE
    PRINT 'ℹ️ DashboardIndicators đã là Temporal Table'

-- 📋 TABLE: BusinessPlanTargets
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BusinessPlanTargets' AND temporal_type = 2)
BEGIN
    PRINT '🔄 Chuyển đổi bảng BusinessPlanTargets thành Temporal Table...'
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('BusinessPlanTargets') AND name = 'ValidFrom')
    BEGIN
        ALTER TABLE BusinessPlanTargets ADD 
            ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
            ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    END
    
    ALTER TABLE BusinessPlanTargets SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.BusinessPlanTargetsHistory))
    
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'BusinessPlanTargetsHistory')
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX CCI_BusinessPlanTargetsHistory ON dbo.BusinessPlanTargetsHistory
    END
    
    PRINT '✅ BusinessPlanTargets chuyển đổi thành công!'
END
ELSE
    PRINT 'ℹ️ BusinessPlanTargets đã là Temporal Table'

-- 📋 TABLE: DashboardCalculations
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DashboardCalculations' AND temporal_type = 2)
BEGIN
    PRINT '🔄 Chuyển đổi bảng DashboardCalculations thành Temporal Table...'
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DashboardCalculations') AND name = 'ValidFrom')
    BEGIN
        ALTER TABLE DashboardCalculations ADD 
            ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
            ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    END
    
    ALTER TABLE DashboardCalculations SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.DashboardCalculationsHistory))
    
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DashboardCalculationsHistory')
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX CCI_DashboardCalculationsHistory ON dbo.DashboardCalculationsHistory
    END
    
    PRINT '✅ DashboardCalculations chuyển đổi thành công!'
END
ELSE
    PRINT 'ℹ️ DashboardCalculations đã là Temporal Table'

-- ================================
-- BƯỚC 7: TẠO OPTIMIZED INDEXES CHO CÁC BẢNG CHÍNH
-- ================================

PRINT '🔧 Tạo các Optimized Indexes cho hiệu suất truy vấn...'

-- Index tối ưu cho Employees
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Employees_UnitId_Active' AND object_id = OBJECT_ID('Employees'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Employees_UnitId_Active ON Employees (UnitId, IsActive) 
    INCLUDE (Id, FullName, EmployeeCode, PositionId)
    PRINT '✅ Tạo index IX_Employees_UnitId_Active'
END

-- Index tối ưu cho KPIDefinitions
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_KPIDefinitions_Category_Active' AND object_id = OBJECT_ID('KPIDefinitions'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_KPIDefinitions_Category_Active ON KPIDefinitions (Category, IsActive) 
    INCLUDE (Id, Name, Formula, Weight)
    PRINT '✅ Tạo index IX_KPIDefinitions_Category_Active'
END

-- Index tối ưu cho FinalPayouts theo thời gian
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_FinalPayouts_Period_Employee' AND object_id = OBJECT_ID('FinalPayouts'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_FinalPayouts_Period_Employee ON FinalPayouts (PeriodId, EmployeeId) 
    INCLUDE (TotalAmount, BonusAmount, DeductionAmount, NetAmount)
    PRINT '✅ Tạo index IX_FinalPayouts_Period_Employee'
END

-- ================================
-- BƯỚC 8: TẠO VIEWS TỐI ƯU CHO TEMPORAL DATA
-- ================================

PRINT '📊 Tạo Views tối ưu cho Temporal Data...'

-- View tổng hợp thông tin nhân viên với lịch sử
IF NOT EXISTS (SELECT * FROM sys.views WHERE name = 'vw_EmployeeHistoricalData')
BEGIN
    EXEC('
    CREATE VIEW vw_EmployeeHistoricalData AS
    SELECT 
        e.Id,
        e.FullName,
        e.EmployeeCode,
        u.Name AS UnitName,
        p.Name AS PositionName,
        e.ValidFrom,
        e.ValidTo,
        CASE WHEN e.ValidTo = ''9999-12-31 23:59:59.9999999'' THEN 1 ELSE 0 END AS IsCurrent
    FROM Employees FOR SYSTEM_TIME ALL e
    LEFT JOIN Units u ON e.UnitId = u.Id
    LEFT JOIN Positions p ON e.PositionId = p.Id
    ')
    PRINT '✅ Tạo view vw_EmployeeHistoricalData'
END

-- View tổng hợp KPI với lịch sử thay đổi
IF NOT EXISTS (SELECT * FROM sys.views WHERE name = 'vw_KPIDefinitionHistory')
BEGIN
    EXEC('
    CREATE VIEW vw_KPIDefinitionHistory AS
    SELECT 
        kd.Id,
        kd.Name,
        kd.Category,
        kd.Formula,
        kd.Weight,
        kd.IsActive,
        kd.ValidFrom,
        kd.ValidTo,
        CASE WHEN kd.ValidTo = ''9999-12-31 23:59:59.9999999'' THEN 1 ELSE 0 END AS IsCurrent
    FROM KPIDefinitions FOR SYSTEM_TIME ALL kd
    ')
    PRINT '✅ Tạo view vw_KPIDefinitionHistory'
END

-- ================================
-- BƯỚC 9: TẠO STORED PROCEDURES TEMPORAL QUERIES
-- ================================

PRINT '⚙️ Tạo Stored Procedures cho Temporal Queries...'

-- SP để lấy lịch sử thay đổi của nhân viên
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetEmployeeHistory')
BEGIN
    EXEC('
    CREATE PROCEDURE sp_GetEmployeeHistory
        @EmployeeId INT,
        @FromDate DATETIME2 = NULL,
        @ToDate DATETIME2 = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        
        SELECT 
            e.Id,
            e.FullName,
            e.EmployeeCode,
            e.Email,
            u.Name AS UnitName,
            p.Name AS PositionName,
            e.ValidFrom,
            e.ValidTo,
            DATEDIFF(DAY, e.ValidFrom, ISNULL(e.ValidTo, GETDATE())) AS DaysInPosition
        FROM Employees FOR SYSTEM_TIME ALL e
        LEFT JOIN Units u ON e.UnitId = u.Id
        LEFT JOIN Positions p ON e.PositionId = p.Id
        WHERE e.Id = @EmployeeId
            AND (@FromDate IS NULL OR e.ValidFrom >= @FromDate)
            AND (@ToDate IS NULL OR e.ValidTo <= @ToDate)
        ORDER BY e.ValidFrom DESC
    END
    ')
    PRINT '✅ Tạo SP sp_GetEmployeeHistory'
END

-- SP để lấy thống kê thay đổi KPI
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetKPIChangeStatistics')
BEGIN
    EXEC('
    CREATE PROCEDURE sp_GetKPIChangeStatistics
        @FromDate DATETIME2,
        @ToDate DATETIME2
    AS
    BEGIN
        SET NOCOUNT ON;
        
        SELECT 
            k.Category,
            COUNT(*) AS TotalChanges,
            COUNT(DISTINCT k.Id) AS UniqueKPIs,
            AVG(CAST(k.Weight AS FLOAT)) AS AvgWeight,
            MIN(k.ValidFrom) AS FirstChange,
            MAX(k.ValidFrom) AS LastChange
        FROM KPIDefinitions FOR SYSTEM_TIME ALL k
        WHERE k.ValidFrom BETWEEN @FromDate AND @ToDate
        GROUP BY k.Category
        ORDER BY TotalChanges DESC
    END
    ')
    PRINT '✅ Tạo SP sp_GetKPIChangeStatistics'
END

-- ================================
-- BƯỚC 10: BÁO CÁO KẾT QUẢ
-- ================================

PRINT '📊 BÁO CÁO KẾT QUẢ CHUYỂN ĐỔI'
PRINT '================================='

-- Đếm số lượng Temporal Tables
DECLARE @TemporalTablesCount INT
SELECT @TemporalTablesCount = COUNT(*)
FROM sys.tables 
WHERE temporal_type = 2

PRINT 'Tổng số Temporal Tables: ' + CAST(@TemporalTablesCount AS VARCHAR(10))

-- Liệt kê tất cả Temporal Tables
PRINT 'Danh sách các Temporal Tables:'
DECLARE @TableList NVARCHAR(MAX) = ''
SELECT @TableList = @TableList + '✅ ' + t.name + ' (History: ' + ISNULL(h.name, 'N/A') + ')' + CHAR(13)
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
WHERE t.temporal_type = 2

PRINT @TableList

-- Kiểm tra Columnstore Indexes
DECLARE @ColumnstoreCount INT
SELECT @ColumnstoreCount = COUNT(*)
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE i.type IN (5, 6) -- Columnstore indexes
AND t.name LIKE '%History'

PRINT 'Tổng số Columnstore Indexes trên History Tables: ' + CAST(@ColumnstoreCount AS VARCHAR(10))

PRINT '🎉 HOÀN THÀNH CHUYỂN ĐỔI 100% SANG TEMPORAL TABLES!'
PRINT 'Thời gian hoàn thành: ' + CONVERT(VARCHAR, GETDATE(), 120)
PRINT '================================================================='
GO
