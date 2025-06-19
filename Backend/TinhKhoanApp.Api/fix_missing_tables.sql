-- Fix missing tables for SQLite database
-- This script creates the missing tables that are causing the import errors

-- Create ImportedDataRecords table
CREATE TABLE IF NOT EXISTS "ImportedDataRecords" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ImportedDataRecords" PRIMARY KEY AUTOINCREMENT,
    "FileName" TEXT NOT NULL,
    "ImportDate" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "ImportedBy" TEXT NOT NULL DEFAULT 'System',
    "Status" TEXT NOT NULL DEFAULT 'Completed',
    "RecordsCount" INTEGER NOT NULL DEFAULT 0,
    "FileType" TEXT,
    "Notes" TEXT
);

-- Create ImportedDataItems table
CREATE TABLE IF NOT EXISTS "ImportedDataItems" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ImportedDataItems" PRIMARY KEY AUTOINCREMENT,
    "ImportedDataRecordId" INTEGER NOT NULL,
    "RowIndex" INTEGER NOT NULL,
    "JsonData" TEXT NOT NULL,
    "ProcessedDate" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "FK_ImportedDataItems_ImportedDataRecords_ImportedDataRecordId" 
        FOREIGN KEY ("ImportedDataRecordId") REFERENCES "ImportedDataRecords" ("Id")
);

-- Create EmployeeRoles table (junction table for many-to-many relationship)
CREATE TABLE IF NOT EXISTS "EmployeeRoles" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_EmployeeRoles" PRIMARY KEY AUTOINCREMENT,
    "EmployeeId" INTEGER NOT NULL,
    "RoleId" INTEGER NOT NULL,
    "AssignedDate" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    CONSTRAINT "FK_EmployeeRoles_Employees_EmployeeId" 
        FOREIGN KEY ("EmployeeId") REFERENCES "Employees" ("Id"),
    CONSTRAINT "FK_EmployeeRoles_Roles_RoleId" 
        FOREIGN KEY ("RoleId") REFERENCES "Roles" ("Id")
);

-- Create unique index to prevent duplicate role assignments
CREATE UNIQUE INDEX IF NOT EXISTS "IX_EmployeeRoles_EmployeeId_RoleId" 
    ON "EmployeeRoles" ("EmployeeId", "RoleId");

-- Add missing columns to existing tables if they don't exist
-- Note: SQLite doesn't support adding columns with constraints easily, 
-- so we'll add them without constraints for now

-- Check if the tables were created successfully
SELECT 'ImportedDataRecords created/exists' as result 
WHERE EXISTS (SELECT 1 FROM sqlite_master WHERE type='table' AND name='ImportedDataRecords');

SELECT 'ImportedDataItems created/exists' as result 
WHERE EXISTS (SELECT 1 FROM sqlite_master WHERE type='table' AND name='ImportedDataItems');

SELECT 'EmployeeRoles created/exists' as result 
WHERE EXISTS (SELECT 1 FROM sqlite_master WHERE type='table' AND name='EmployeeRoles');
