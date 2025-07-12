-- Create KPI Schema for TinhKhoanApp
-- Tạo schema KPI cho hệ thống giao khoán

-- 1. Tạo bảng KpiAssignmentTables (Template cho các bảng KPI)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'KpiAssignmentTables')
BEGIN
    CREATE TABLE KpiAssignmentTables (
        Id int IDENTITY(1,1) PRIMARY KEY,
        TableType nvarchar(50) NOT NULL,
        TableName nvarchar(100) NOT NULL,
        Description nvarchar(max),
        Category nvarchar(50) NOT NULL,
        CreatedAt datetime2 DEFAULT GETDATE(),
        UpdatedAt datetime2 DEFAULT GETDATE()
    );
    PRINT 'Created KpiAssignmentTables table';
END
ELSE
BEGIN
    PRINT 'KpiAssignmentTables table already exists';
END

-- 2. Tạo bảng KpiIndicators (Chỉ tiêu KPI cho từng bảng)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'KpiIndicators')
BEGIN
    CREATE TABLE KpiIndicators (
        Id int IDENTITY(1,1) PRIMARY KEY,
        TableId int NOT NULL,
        IndicatorCode nvarchar(50) NOT NULL,
        IndicatorName nvarchar(200) NOT NULL,
        Description nvarchar(max),
        Unit nvarchar(50),
        IsActive bit DEFAULT 1,
        CreatedAt datetime2 DEFAULT GETDATE(),
        UpdatedAt datetime2 DEFAULT GETDATE(),
        CONSTRAINT FK_KpiIndicators_KpiAssignmentTables
        FOREIGN KEY (TableId) REFERENCES KpiAssignmentTables(Id)
    );
    PRINT 'Created KpiIndicators table';
END
ELSE
BEGIN
    PRINT 'KpiIndicators table already exists';
END

-- 3. Tạo bảng EmployeeKpiAssignments (Giao khoán KPI cho cán bộ)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'EmployeeKpiAssignments')
BEGIN
    CREATE TABLE EmployeeKpiAssignments (
        Id int IDENTITY(1,1) PRIMARY KEY,
        EmployeeId int NOT NULL,
        KpiIndicatorId int NOT NULL,
        KhoanPeriodId int NOT NULL,
        TargetValue decimal(18,2),
        ActualValue decimal(18,2) NULL,
        Status nvarchar(20) DEFAULT 'ACTIVE',
        AssignedAt datetime2 DEFAULT GETDATE(),
        UpdatedAt datetime2 DEFAULT GETDATE(),
        CONSTRAINT FK_EmployeeKpiAssignments_Employees
        FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
        CONSTRAINT FK_EmployeeKpiAssignments_KpiIndicators
        FOREIGN KEY (KpiIndicatorId) REFERENCES KpiIndicators(Id),
        CONSTRAINT FK_EmployeeKpiAssignments_KhoanPeriods
        FOREIGN KEY (KhoanPeriodId) REFERENCES KhoanPeriods(Id)
    );
    PRINT 'Created EmployeeKpiAssignments table';
END
ELSE
BEGIN
    PRINT 'EmployeeKpiAssignments table already exists';
END

-- 4. Tạo bảng UnitKpiScorings (Điểm KPI cho đơn vị)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UnitKpiScorings')
BEGIN
    CREATE TABLE UnitKpiScorings (
        Id int IDENTITY(1,1) PRIMARY KEY,
        UnitId int NOT NULL,
        KhoanPeriodId int NOT NULL,
        TotalScore decimal(18,2) DEFAULT 0,
        MaxScore decimal(18,2) DEFAULT 100,
        Percentage decimal(5,2) DEFAULT 0,
        Ranking int NULL,
        Status nvarchar(20) DEFAULT 'DRAFT',
        CreatedAt datetime2 DEFAULT GETDATE(),
        UpdatedAt datetime2 DEFAULT GETDATE(),
        CONSTRAINT FK_UnitKpiScorings_Units
        FOREIGN KEY (UnitId) REFERENCES Units(Id),
        CONSTRAINT FK_UnitKpiScorings_KhoanPeriods
        FOREIGN KEY (KhoanPeriodId) REFERENCES KhoanPeriods(Id)
    );
    PRINT 'Created UnitKpiScorings table';
END
ELSE
BEGIN
    PRINT 'UnitKpiScorings table already exists';
END

-- 5. Tạo indexes để tối ưu performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_KpiIndicators_TableId')
BEGIN
    CREATE INDEX IX_KpiIndicators_TableId ON KpiIndicators(TableId);
    PRINT 'Created index IX_KpiIndicators_TableId';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmployeeKpiAssignments_EmployeeId')
BEGIN
    CREATE INDEX IX_EmployeeKpiAssignments_EmployeeId ON EmployeeKpiAssignments(EmployeeId);
    PRINT 'Created index IX_EmployeeKpiAssignments_EmployeeId';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmployeeKpiAssignments_KhoanPeriodId')
BEGIN
    CREATE INDEX IX_EmployeeKpiAssignments_KhoanPeriodId ON EmployeeKpiAssignments(KhoanPeriodId);
    PRINT 'Created index IX_EmployeeKpiAssignments_KhoanPeriodId';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UnitKpiScorings_UnitId')
BEGIN
    CREATE INDEX IX_UnitKpiScorings_UnitId ON UnitKpiScorings(UnitId);
    PRINT 'Created index IX_UnitKpiScorings_UnitId';
END

-- 6. Verification
SELECT
    'KPI Schema Creation Summary' as [Status],
    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME IN ('KpiAssignmentTables', 'KpiIndicators', 'EmployeeKpiAssignments', 'UnitKpiScorings')) as [Tables Created]

PRINT '✅ KPI Schema creation completed successfully!';
PRINT '📋 Tables: KpiAssignmentTables, KpiIndicators, EmployeeKpiAssignments, UnitKpiScorings';
PRINT '🔍 Indexes: Optimized for performance';
PRINT '🔗 Foreign Keys: All relationships established';
