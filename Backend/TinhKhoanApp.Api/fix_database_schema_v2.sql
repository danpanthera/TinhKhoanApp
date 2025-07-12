-- Fix database schema issues carefully
-- 1. Add PasswordHash column to Employees table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Employees') AND name = 'PasswordHash')
BEGIN
    ALTER TABLE Employees ADD PasswordHash NVARCHAR(4000) NULL;
    PRINT 'Added PasswordHash column to Employees table';
END
ELSE
BEGIN
    PRINT 'PasswordHash column already exists in Employees table';
END

-- 2. Fix KhoanPeriods table by recreating it with correct data types
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'KhoanPeriods')
BEGIN
    -- Check current data types
    DECLARE @StatusDataType NVARCHAR(128), @TypeDataType NVARCHAR(128)

    SELECT @StatusDataType = DATA_TYPE
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'KhoanPeriods' AND COLUMN_NAME = 'Status'

    SELECT @TypeDataType = DATA_TYPE
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'KhoanPeriods' AND COLUMN_NAME = 'Type'

    PRINT 'Current Status data type: ' + ISNULL(@StatusDataType, 'NULL')
    PRINT 'Current Type data type: ' + ISNULL(@TypeDataType, 'NULL')

    -- If Status or Type are string types, we need to fix them
    IF @StatusDataType IN ('nvarchar', 'varchar', 'char', 'nchar') OR @TypeDataType IN ('nvarchar', 'varchar', 'char', 'nchar')
    BEGIN
        PRINT 'KhoanPeriods table has incorrect column types. Will backup and recreate.'

        -- Backup existing data
        SELECT * INTO KhoanPeriods_backup FROM KhoanPeriods;

        -- Drop the existing table
        DROP TABLE KhoanPeriods;

        -- Recreate with correct types
        CREATE TABLE KhoanPeriods (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            Name NVARCHAR(255) NOT NULL,
            StartDate DATETIME2(7) NOT NULL,
            EndDate DATETIME2(7) NOT NULL,
            Status INT NOT NULL DEFAULT 0,
            Type INT NOT NULL DEFAULT 0
        );

        -- Restore data with proper type conversion
        INSERT INTO KhoanPeriods (Name, StartDate, EndDate, Status, Type)
        SELECT
            Name,
            StartDate,
            EndDate,
            CASE
                WHEN ISNUMERIC(Status) = 1 THEN CAST(Status AS INT)
                ELSE 0
            END AS Status,
            CASE
                WHEN ISNUMERIC(Type) = 1 THEN CAST(Type AS INT)
                ELSE 0
            END AS Type
        FROM KhoanPeriods_backup;

        -- Drop backup table
        DROP TABLE KhoanPeriods_backup;

        PRINT 'KhoanPeriods table recreated with correct data types';
    END
    ELSE
    BEGIN
        PRINT 'KhoanPeriods table already has correct data types';
    END
END

PRINT 'Database schema fixes completed successfully!';
