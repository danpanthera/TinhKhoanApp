-- Create basic tables for TinhKhoan app
-- This is a minimal schema to get the import functionality working

PRAGMA foreign_keys = ON;

-- Roles table (referenced in RoleSeeder)
CREATE TABLE IF NOT EXISTS "Roles" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Roles" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT,
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "CreatedDate" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Units table (referenced in Program.cs)
CREATE TABLE IF NOT EXISTS "Units" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Units" PRIMARY KEY AUTOINCREMENT,
    "UnitCode" TEXT NOT NULL,
    "UnitName" TEXT NOT NULL,
    "ParentUnitId" INTEGER,
    "UnitType" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "CreatedDate" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "FK_Units_Units_ParentUnitId" FOREIGN KEY ("ParentUnitId") REFERENCES "Units" ("Id")
);

-- Employees table (basic)
CREATE TABLE IF NOT EXISTS "Employees" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Employees" PRIMARY KEY AUTOINCREMENT,
    "EmployeeCode" TEXT NOT NULL,
    "FullName" TEXT NOT NULL,
    "UnitId" INTEGER NOT NULL,
    "PositionId" INTEGER,
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "CreatedDate" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "FK_Employees_Units_UnitId" FOREIGN KEY ("UnitId") REFERENCES "Units" ("Id")
);

-- Positions table (basic)
CREATE TABLE IF NOT EXISTS "Positions" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Positions" PRIMARY KEY AUTOINCREMENT,
    "PositionName" TEXT NOT NULL,
    "PositionCode" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "CreatedDate" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- RawDataImports table (for import functionality)
CREATE TABLE IF NOT EXISTS "RawDataImports" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_RawDataImports" PRIMARY KEY AUTOINCREMENT,
    "FileName" TEXT NOT NULL,
    "DataType" TEXT NOT NULL,
    "ImportDate" TEXT NOT NULL,
    "StatementDate" TEXT NOT NULL,
    "ImportedBy" TEXT NOT NULL,
    "Status" TEXT,
    "RecordsCount" INTEGER NOT NULL DEFAULT 0,
    "Notes" TEXT,
    "OriginalFileData" BLOB,
    "IsArchiveFile" INTEGER NOT NULL DEFAULT 0,
    "ArchiveType" TEXT,
    "RequiresPassword" INTEGER NOT NULL DEFAULT 0,
    "ExtractedFilesCount" INTEGER NOT NULL DEFAULT 0,
    "ExtractedFilesList" TEXT
);

-- RawDataRecords table (for import functionality)
CREATE TABLE IF NOT EXISTS "RawDataRecords" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_RawDataRecords" PRIMARY KEY AUTOINCREMENT,
    "RawDataImportId" INTEGER NOT NULL,
    "JsonData" TEXT NOT NULL,
    "ProcessedDate" TEXT NOT NULL,
    CONSTRAINT "FK_RawDataRecords_RawDataImports_RawDataImportId" FOREIGN KEY ("RawDataImportId") REFERENCES "RawDataImports" ("Id") ON DELETE CASCADE
);

-- KhoanPeriods table (for periods management)
CREATE TABLE IF NOT EXISTS "KhoanPeriods" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_KhoanPeriods" PRIMARY KEY AUTOINCREMENT,
    "PeriodName" TEXT NOT NULL,
    "FromDate" TEXT NOT NULL,
    "ToDate" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "CreatedDate" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Insert basic data
INSERT OR IGNORE INTO "Units" ("Id", "UnitCode", "UnitName", "UnitType") VALUES
(1, 'ROOT', 'Agribank Lai Châu', 'ROOT'),
(2, 'CNH', 'Chi nhánh chính', 'BRANCH'),
(3, 'PGD01', 'Phòng giao dịch 01', 'DEPARTMENT');

INSERT OR IGNORE INTO "Positions" ("Id", "PositionName", "PositionCode") VALUES
(1, 'Giám đốc', 'GD'),
(2, 'Phó giám đốc', 'PGD'),
(3, 'Trưởng phòng', 'TP'),
(4, 'Nhân viên', 'NV');

INSERT OR IGNORE INTO "Employees" ("Id", "EmployeeCode", "FullName", "UnitId", "PositionId") VALUES
(1, 'ADMIN', 'Administrator', 1, 1),
(2, 'USER01', 'Test User', 2, 4);

INSERT OR IGNORE INTO "KhoanPeriods" ("Id", "PeriodName", "FromDate", "ToDate") VALUES
(1, 'Tháng 1/2024', '2024-01-01', '2024-01-31'),
(2, 'Tháng 2/2024', '2024-02-01', '2024-02-29');

-- Create indexes for performance
CREATE INDEX IF NOT EXISTS "IX_Employees_UnitId" ON "Employees" ("UnitId");
CREATE INDEX IF NOT EXISTS "IX_RawDataRecords_RawDataImportId" ON "RawDataRecords" ("RawDataImportId");
CREATE INDEX IF NOT EXISTS "IX_Units_ParentUnitId" ON "Units" ("ParentUnitId");
