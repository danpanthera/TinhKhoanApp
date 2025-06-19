-- Tạo các bảng KPI còn thiếu trong database
-- Dựa trên các models trong ApplicationDbContext

-- 1. Bảng KpiAssignmentTables
CREATE TABLE IF NOT EXISTS "KpiAssignmentTables" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_KpiAssignmentTables" PRIMARY KEY AUTOINCREMENT,
    "TableName" TEXT NOT NULL,
    "DatabaseName" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "CreatedDate" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "LastUpdated" TEXT
);

-- 2. Bảng KPIDefinitions
CREATE TABLE IF NOT EXISTS "KPIDefinitions" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_KPIDefinitions" PRIMARY KEY AUTOINCREMENT,
    "KpiCode" TEXT NOT NULL,
    "KpiName" TEXT NOT NULL,
    "Description" TEXT,
    "ValueType" INTEGER NOT NULL DEFAULT 1,
    "Unit" TEXT,
    "MaxScore" REAL NOT NULL DEFAULT 100.0,
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "Category" TEXT,
    "CreatedDate" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedDate" TEXT
);

-- 3. Bảng KpiIndicators
CREATE TABLE IF NOT EXISTS "KpiIndicators" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_KpiIndicators" PRIMARY KEY AUTOINCREMENT,
    "IndicatorName" TEXT NOT NULL,
    "IndicatorCode" TEXT NOT NULL,
    "Description" TEXT,
    "ValueType" INTEGER NOT NULL DEFAULT 1,
    "Unit" TEXT,
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "CreatedDate" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 4. Bảng EmployeeKpiAssignments
CREATE TABLE IF NOT EXISTS "EmployeeKpiAssignments" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_EmployeeKpiAssignments" PRIMARY KEY AUTOINCREMENT,
    "EmployeeId" INTEGER NOT NULL,
    "KpiDefinitionId" INTEGER NOT NULL,
    "AssignedDate" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "DueDate" TEXT,
    "TargetValue" REAL,
    "ActualValue" REAL,
    "CompletionRate" REAL,
    "Score" REAL,
    "Notes" TEXT,
    "Status" TEXT NOT NULL DEFAULT 'ASSIGNED',
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    CONSTRAINT "FK_EmployeeKpiAssignments_Employees_EmployeeId" FOREIGN KEY ("EmployeeId") REFERENCES "Employees" ("Id"),
    CONSTRAINT "FK_EmployeeKpiAssignments_KPIDefinitions_KpiDefinitionId" FOREIGN KEY ("KpiDefinitionId") REFERENCES "KPIDefinitions" ("Id")
);

-- 5. Bảng EmployeeKpiTargets
CREATE TABLE IF NOT EXISTS "EmployeeKpiTargets" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_EmployeeKpiTargets" PRIMARY KEY AUTOINCREMENT,
    "EmployeeId" INTEGER NOT NULL,
    "KpiIndicatorId" INTEGER NOT NULL,
    "TargetValue" REAL NOT NULL,
    "Period" TEXT NOT NULL,
    "CreatedDate" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "FK_EmployeeKpiTargets_Employees_EmployeeId" FOREIGN KEY ("EmployeeId") REFERENCES "Employees" ("Id"),
    CONSTRAINT "FK_EmployeeKpiTargets_KpiIndicators_KpiIndicatorId" FOREIGN KEY ("KpiIndicatorId") REFERENCES "KpiIndicators" ("Id")
);

-- 6. Bảng UnitKpiScorings (KPI của Chi nhánh)
CREATE TABLE IF NOT EXISTS "UnitKpiScorings" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_UnitKpiScorings" PRIMARY KEY AUTOINCREMENT,
    "UnitId" INTEGER NOT NULL,
    "ScoringPeriod" TEXT NOT NULL,
    "TotalScore" REAL NOT NULL DEFAULT 0,
    "MaxPossibleScore" REAL NOT NULL DEFAULT 100,
    "CompletionRate" REAL NOT NULL DEFAULT 0,
    "Grade" TEXT,
    "Notes" TEXT,
    "CreatedDate" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedDate" TEXT,
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    CONSTRAINT "FK_UnitKpiScorings_Units_UnitId" FOREIGN KEY ("UnitId") REFERENCES "Units" ("Id")
);

-- 7. Bảng UnitKpiScoringDetails
CREATE TABLE IF NOT EXISTS "UnitKpiScoringDetails" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_UnitKpiScoringDetails" PRIMARY KEY AUTOINCREMENT,
    "UnitKpiScoringId" INTEGER NOT NULL,
    "IndicatorName" TEXT NOT NULL,
    "TargetValue" REAL,
    "ActualValue" REAL,
    "CompletionRate" REAL NOT NULL DEFAULT 0,
    "Score" REAL NOT NULL DEFAULT 0,
    "MaxScore" REAL NOT NULL DEFAULT 10,
    "Weight" REAL NOT NULL DEFAULT 1.0,
    "IndicatorType" TEXT NOT NULL,
    "Notes" TEXT,
    CONSTRAINT "FK_UnitKpiScoringDetails_UnitKpiScorings_UnitKpiScoringId" FOREIGN KEY ("UnitKpiScoringId") REFERENCES "UnitKpiScorings" ("Id")
);

-- 8. Bảng UnitKpiScoringCriterias
CREATE TABLE IF NOT EXISTS "UnitKpiScoringCriterias" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_UnitKpiScoringCriterias" PRIMARY KEY AUTOINCREMENT,
    "UnitKpiScoringId" INTEGER NOT NULL,
    "ViolationType" TEXT NOT NULL,
    "ViolationLevel" TEXT NOT NULL,
    "DeductionPoints" REAL NOT NULL DEFAULT 0,
    "MaxDeductions" REAL,
    "ActualDeductions" REAL NOT NULL DEFAULT 0,
    "ViolationCount" INTEGER NOT NULL DEFAULT 0,
    "Notes" TEXT,
    CONSTRAINT "FK_UnitKpiScoringCriterias_UnitKpiScorings_UnitKpiScoringId" FOREIGN KEY ("UnitKpiScoringId") REFERENCES "UnitKpiScorings" ("Id")
);

-- 9. Bảng KpiScoringRules
CREATE TABLE IF NOT EXISTS "KpiScoringRules" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_KpiScoringRules" PRIMARY KEY AUTOINCREMENT,
    "KpiIndicatorName" TEXT NOT NULL,
    "RuleType" TEXT NOT NULL DEFAULT 'COMPLETION_RATE',
    "MinValue" REAL,
    "MaxValue" REAL,
    "ScoreFormula" TEXT,
    "BonusPoints" REAL NOT NULL DEFAULT 0,
    "PenaltyPoints" REAL NOT NULL DEFAULT 0,
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "CreatedDate" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedDate" TEXT
);

-- 10. Bảng EmployeeRoles (nếu chưa có)
CREATE TABLE IF NOT EXISTS "EmployeeRoles" (
    "EmployeeId" INTEGER NOT NULL,
    "RoleId" INTEGER NOT NULL,
    "AssignedDate" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    CONSTRAINT "PK_EmployeeRoles" PRIMARY KEY ("EmployeeId", "RoleId"),
    CONSTRAINT "FK_EmployeeRoles_Employees_EmployeeId" FOREIGN KEY ("EmployeeId") REFERENCES "Employees" ("Id"),
    CONSTRAINT "FK_EmployeeRoles_Roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "Roles" ("Id")
);

-- Fix bảng Positions để có đúng schema như EF model expects
ALTER TABLE Positions RENAME TO Positions_old;

CREATE TABLE "Positions" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Positions" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT
);

-- Migrate data từ schema cũ sang mới
INSERT INTO Positions (Id, Name, Description) 
SELECT Id, PositionName, PositionCode FROM Positions_old;

DROP TABLE Positions_old;

-- Thêm các columns thiếu vào bảng Employees
ALTER TABLE Employees ADD COLUMN Username TEXT;
ALTER TABLE Employees ADD COLUMN PasswordHash TEXT;
ALTER TABLE Employees ADD COLUMN CBCode TEXT;
ALTER TABLE Employees ADD COLUMN Email TEXT;
ALTER TABLE Employees ADD COLUMN PhoneNumber TEXT;

-- Thêm test data cơ bản
INSERT OR IGNORE INTO Units (Id, UnitCode, UnitName, UnitType) VALUES 
(1, 'ROOT', 'Agribank Lai Châu', 'ROOT'),
(2, 'CNH', 'Chi nhánh chính', 'BRANCH'), 
(3, 'PGD01', 'Phòng giao dịch 01', 'DEPARTMENT');

INSERT OR IGNORE INTO Positions (Id, Name, Description) VALUES
(1, 'Giám đốc', 'GD'),
(2, 'Phó giám đốc', 'PGD'),
(3, 'Trưởng phòng', 'TP'),
(4, 'Nhân viên', 'NV');

INSERT OR IGNORE INTO Employees (Id, EmployeeCode, FullName, UnitId, PositionId, Username, PasswordHash, CBCode) VALUES
(1, 'ADMIN', 'Administrator', 1, 1, 'admin', '$2a$11$2G.3I4HWHwwTGF3Ey3JME.9iYbRvYJGvY6KYgJ6UZFfJ/qR7a8aBq', '000000000'),
(2, 'USER01', 'Test User', 2, 4, 'user01', NULL, NULL);

-- Test KPI data
INSERT OR IGNORE INTO KPIDefinitions (Id, KpiCode, KpiName, Description, ValueType, MaxScore) VALUES
(1, 'KPI001', 'Doanh số tín dụng', 'Chỉ tiêu doanh số cho vay', 4, 20.0),
(2, 'KPI002', 'Tỷ lệ nợ xấu', 'Kiểm soát tỷ lệ nợ xấu dưới 2%', 2, 15.0),
(3, 'KPI003', 'Huy động vốn', 'Chỉ tiêu huy động tiền gửi', 4, 20.0);

INSERT OR IGNORE INTO EmployeeKpiAssignments (EmployeeId, KpiDefinitionId, TargetValue, Status) VALUES
(1, 1, 500000000, 'ASSIGNED'),
(1, 2, 2.0, 'ASSIGNED'),
(2, 1, 300000000, 'ASSIGNED');

INSERT OR IGNORE INTO UnitKpiScorings (Id, UnitId, ScoringPeriod, TotalScore, MaxPossibleScore) VALUES
(1, 2, '2025-06', 85.5, 100.0),
(2, 3, '2025-06', 78.2, 100.0);

-- Verify tables created
SELECT 'Tables created successfully!' as Status;
.tables
