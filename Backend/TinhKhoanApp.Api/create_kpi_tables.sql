-- Tạo các bảng KPI còn thiếu trong database
-- Dựa trên các models trong ApplicationDbContext
-- SQL Server compatible version

-- 1. Bảng KpiAssignmentTables
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='KpiAssignmentTables' AND xtype='U')
CREATE TABLE KpiAssignmentTables (
    Id INT NOT NULL CONSTRAINT PK_KpiAssignmentTables PRIMARY KEY IDENTITY(1,1),
    TableName NVARCHAR(255) NOT NULL,
    DatabaseName NVARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdated DATETIME
);

-- 2. Bảng KPIDefinitions
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='KPIDefinitions' AND xtype='U')
CREATE TABLE KPIDefinitions (
    Id INT NOT NULL CONSTRAINT PK_KPIDefinitions PRIMARY KEY IDENTITY(1,1),
    KpiCode NVARCHAR(50) NOT NULL,
    KpiName NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    ValueType INT NOT NULL DEFAULT 1,
    Unit NVARCHAR(50),
    MaxScore FLOAT NOT NULL DEFAULT 100.0,
    IsActive BIT NOT NULL DEFAULT 1,
    Category NVARCHAR(100),
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedDate DATETIME
);

-- 3. Bảng KpiIndicators
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='KpiIndicators' AND xtype='U')
CREATE TABLE KpiIndicators (
    Id INT NOT NULL CONSTRAINT PK_KpiIndicators PRIMARY KEY IDENTITY(1,1),
    IndicatorName NVARCHAR(255) NOT NULL,
    IndicatorCode NVARCHAR(50) NOT NULL,
    Description NVARCHAR(MAX),
    ValueType INT NOT NULL DEFAULT 1,
    Unit NVARCHAR(50),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
);

-- 4. Bảng EmployeeKpiAssignments
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EmployeeKpiAssignments' AND xtype='U')
CREATE TABLE EmployeeKpiAssignments (
    Id INT NOT NULL CONSTRAINT PK_EmployeeKpiAssignments PRIMARY KEY IDENTITY(1,1),
    EmployeeId INT NOT NULL,
    KpiDefinitionId INT NOT NULL,
    AssignedDate DATETIME NOT NULL DEFAULT GETDATE(),
    DueDate DATETIME,
    TargetValue FLOAT,
    ActualValue FLOAT,
    CompletionRate FLOAT,
    Score FLOAT,
    Notes NVARCHAR(MAX),
    Status NVARCHAR(50) NOT NULL DEFAULT 'ASSIGNED',
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_EmployeeKpiAssignments_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES Employees (Id),
    CONSTRAINT FK_EmployeeKpiAssignments_KPIDefinitions_KpiDefinitionId FOREIGN KEY (KpiDefinitionId) REFERENCES KPIDefinitions (Id)
);

-- 5. Bảng EmployeeKpiTargets
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EmployeeKpiTargets' AND xtype='U')
CREATE TABLE EmployeeKpiTargets (
    Id INT NOT NULL CONSTRAINT PK_EmployeeKpiTargets PRIMARY KEY IDENTITY(1,1),
    EmployeeId INT NOT NULL,
    KpiIndicatorId INT NOT NULL,
    TargetValue FLOAT NOT NULL,
    Period NVARCHAR(50) NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_EmployeeKpiTargets_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES Employees (Id),
    CONSTRAINT FK_EmployeeKpiTargets_KpiIndicators_KpiIndicatorId FOREIGN KEY (KpiIndicatorId) REFERENCES KpiIndicators (Id)
);

-- 6. Bảng UnitKpiScorings (KPI của Chi nhánh)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UnitKpiScorings' AND xtype='U')
CREATE TABLE UnitKpiScorings (
    Id INT NOT NULL CONSTRAINT PK_UnitKpiScorings PRIMARY KEY IDENTITY(1,1),
    UnitId INT NOT NULL,
    ScoringPeriod NVARCHAR(50) NOT NULL,
    TotalScore FLOAT NOT NULL DEFAULT 0,
    MaxPossibleScore FLOAT NOT NULL DEFAULT 100,
    CompletionRate FLOAT NOT NULL DEFAULT 0,
    Grade NVARCHAR(10),
    Notes NVARCHAR(MAX),
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedDate DATETIME,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_UnitKpiScorings_Units_UnitId FOREIGN KEY (UnitId) REFERENCES Units (Id)
);

-- 7. Bảng UnitKpiScoringDetails
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UnitKpiScoringDetails' AND xtype='U')
CREATE TABLE UnitKpiScoringDetails (
    Id INT NOT NULL CONSTRAINT PK_UnitKpiScoringDetails PRIMARY KEY IDENTITY(1,1),
    UnitKpiScoringId INT NOT NULL,
    IndicatorName NVARCHAR(255) NOT NULL,
    TargetValue FLOAT,
    ActualValue FLOAT,
    CompletionRate FLOAT NOT NULL DEFAULT 0,
    Score FLOAT NOT NULL DEFAULT 0,
    MaxScore FLOAT NOT NULL DEFAULT 10,
    Weight FLOAT NOT NULL DEFAULT 1.0,
    IndicatorType NVARCHAR(50) NOT NULL,
    Notes NVARCHAR(MAX),
    CONSTRAINT FK_UnitKpiScoringDetails_UnitKpiScorings_UnitKpiScoringId FOREIGN KEY (UnitKpiScoringId) REFERENCES UnitKpiScorings (Id)
);

-- 8. Bảng UnitKpiScoringCriterias
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UnitKpiScoringCriterias' AND xtype='U')
CREATE TABLE UnitKpiScoringCriterias (
    Id INT NOT NULL CONSTRAINT PK_UnitKpiScoringCriterias PRIMARY KEY IDENTITY(1,1),
    UnitKpiScoringId INT NOT NULL,
    ViolationType NVARCHAR(100) NOT NULL,
    ViolationLevel NVARCHAR(50) NOT NULL,
    DeductionPoints FLOAT NOT NULL DEFAULT 0,
    MaxDeductions FLOAT,
    ActualDeductions FLOAT NOT NULL DEFAULT 0,
    ViolationCount INT NOT NULL DEFAULT 0,
    Notes NVARCHAR(MAX),
    CONSTRAINT FK_UnitKpiScoringCriterias_UnitKpiScorings_UnitKpiScoringId FOREIGN KEY (UnitKpiScoringId) REFERENCES UnitKpiScorings (Id)
);

-- 9. Bảng KpiScoringRules
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='KpiScoringRules' AND xtype='U')
CREATE TABLE KpiScoringRules (
    Id INT NOT NULL CONSTRAINT PK_KpiScoringRules PRIMARY KEY IDENTITY(1,1),
    KpiIndicatorName NVARCHAR(255) NOT NULL,
    RuleType NVARCHAR(50) NOT NULL DEFAULT 'COMPLETION_RATE',
    MinValue FLOAT,
    MaxValue FLOAT,
    ScoreFormula NVARCHAR(MAX),
    BonusPoints FLOAT NOT NULL DEFAULT 0,
    PenaltyPoints FLOAT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedDate DATETIME
);

-- 10. Bảng EmployeeRoles (nếu chưa có)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EmployeeRoles' AND xtype='U')
CREATE TABLE EmployeeRoles (
    EmployeeId INT NOT NULL,
    RoleId INT NOT NULL,
    AssignedDate DATETIME NOT NULL DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT PK_EmployeeRoles PRIMARY KEY (EmployeeId, RoleId),
    CONSTRAINT FK_EmployeeRoles_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES Employees (Id),
    CONSTRAINT FK_EmployeeRoles_Roles_RoleId FOREIGN KEY (RoleId) REFERENCES Roles (Id)
);

-- Check if we need to modify Positions table (commented out - depends on existing schema)
/*
-- Fix bảng Positions để có đúng schema như EF model expects
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Positions') AND name = 'PositionName')
BEGIN
    -- Backup existing data
    SELECT * INTO Positions_backup FROM Positions;
    
    -- Drop and recreate with correct schema
    DROP TABLE Positions;
    
    CREATE TABLE Positions (
        Id INT NOT NULL CONSTRAINT PK_Positions PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(255) NOT NULL,
        Description NVARCHAR(MAX)
    );
    
    -- Migrate data từ schema cũ sang mới
    INSERT INTO Positions (Name, Description) 
    SELECT PositionName, PositionCode FROM Positions_backup;
    
    DROP TABLE Positions_backup;
END
*/

-- Add missing columns to Employees table (check if they exist first)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Employees') AND name = 'Username')
    ALTER TABLE Employees ADD Username NVARCHAR(100);

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Employees') AND name = 'PasswordHash')
    ALTER TABLE Employees ADD PasswordHash NVARCHAR(MAX);

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Employees') AND name = 'CBCode')
    ALTER TABLE Employees ADD CBCode NVARCHAR(50);

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Employees') AND name = 'Email')
    ALTER TABLE Employees ADD Email NVARCHAR(255);

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Employees') AND name = 'PhoneNumber')
    ALTER TABLE Employees ADD PhoneNumber NVARCHAR(20);

-- Test data (use MERGE to avoid duplicates)
MERGE Units AS target
USING (VALUES 
    (1, 'ROOT', 'Agribank Lai Châu', 'ROOT'),
    (2, 'CNH', 'Chi nhánh chính', 'BRANCH'), 
    (3, 'PGD01', 'Phòng giao dịch 01', 'DEPARTMENT')
) AS source (Id, Code, Name, Type)
ON target.Id = source.Id
WHEN NOT MATCHED THEN 
    INSERT (Id, Code, Name, Type) VALUES (source.Id, source.Code, source.Name, source.Type);

-- KPI test data
IF NOT EXISTS (SELECT * FROM KPIDefinitions WHERE KpiCode = 'KPI001')
INSERT INTO KPIDefinitions (KpiCode, KpiName, Description, ValueType, MaxScore) VALUES
('KPI001', 'Doanh số tín dụng', 'Chỉ tiêu doanh số cho vay', 4, 20.0);

IF NOT EXISTS (SELECT * FROM KPIDefinitions WHERE KpiCode = 'KPI002')
INSERT INTO KPIDefinitions (KpiCode, KpiName, Description, ValueType, MaxScore) VALUES
('KPI002', 'Tỷ lệ nợ xấu', 'Kiểm soát tỷ lệ nợ xấu dưới 2%', 2, 15.0);

IF NOT EXISTS (SELECT * FROM KPIDefinitions WHERE KpiCode = 'KPI003')
INSERT INTO KPIDefinitions (KpiCode, KpiName, Description, ValueType, MaxScore) VALUES
('KPI003', 'Huy động vốn', 'Chỉ tiêu huy động tiền gửi', 4, 20.0);

-- Verify tables created
PRINT 'Tables created successfully!';
SELECT name as TableName FROM sys.tables WHERE name LIKE '%Kpi%' OR name LIKE '%KPI%' ORDER BY name;
