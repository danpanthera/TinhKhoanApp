-- Fix database schema issues
-- 1. Add PasswordHash column to Employees table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Employees') AND name = 'PasswordHash')
BEGIN
    ALTER TABLE Employees ADD PasswordHash NVARCHAR(4000) NULL;
END

-- 2. Fix KhoanPeriods table column types
-- Check if Status column is string instead of int
DECLARE @StatusDataType NVARCHAR(128)
SELECT @StatusDataType = DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'KhoanPeriods' AND COLUMN_NAME = 'Status'

IF @StatusDataType = 'nvarchar' OR @StatusDataType = 'varchar'
BEGIN
    -- Drop and recreate Status column as int
    ALTER TABLE KhoanPeriods DROP COLUMN Status;
    ALTER TABLE KhoanPeriods ADD Status INT NOT NULL DEFAULT 0;
END

-- Check if Type column is string instead of int
DECLARE @TypeDataType NVARCHAR(128)
SELECT @TypeDataType = DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'KhoanPeriods' AND COLUMN_NAME = 'Type'

IF @TypeDataType = 'nvarchar' OR @TypeDataType = 'varchar'
BEGIN
    -- Drop and recreate Type column as int
    ALTER TABLE KhoanPeriods DROP COLUMN Type;
    ALTER TABLE KhoanPeriods ADD Type INT NOT NULL DEFAULT 0;
END

-- 3. Ensure all required tables exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Employees')
BEGIN
    CREATE TABLE Employees (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        EmployeeCode NVARCHAR(20) NOT NULL,
        CBCode NVARCHAR(9) NULL,
        FullName NVARCHAR(255) NOT NULL,
        Username NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NULL,
        PhoneNumber NVARCHAR(20) NULL,
        PasswordHash NVARCHAR(4000) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        UnitId INT NOT NULL,
        PositionId INT NOT NULL,
        CONSTRAINT FK_Employees_Units FOREIGN KEY (UnitId) REFERENCES Units(Id),
        CONSTRAINT FK_Employees_Positions FOREIGN KEY (PositionId) REFERENCES Positions(Id)
    );
END

PRINT 'Database schema fixes completed successfully!';
