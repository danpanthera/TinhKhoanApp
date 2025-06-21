-- Fix Position table schema to match EF models (SQL Server)
-- Current schema has: PositionName, PositionCode
-- EF Model expects: Name, Description

-- First, backup the current data
SELECT * INTO Positions_backup FROM Positions;

-- Drop constraints first
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Positions_Units')
    ALTER TABLE Positions DROP CONSTRAINT FK_Positions_Units;

-- Drop the old table
DROP TABLE IF EXISTS Positions;

-- Create new table with correct schema to match EF models
CREATE TABLE [Positions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    [Description] nvarchar(500),
    CONSTRAINT [PK_Positions] PRIMARY KEY ([Id])
);

-- Migrate data from backup (map PositionName to Name, PositionCode to Description)
SET IDENTITY_INSERT Positions ON;
INSERT INTO Positions (Id, Name, Description) 
SELECT Id, PositionName, PositionCode FROM Positions_backup;
SET IDENTITY_INSERT Positions OFF;

-- Recreate foreign key if needed
-- ALTER TABLE Positions ADD CONSTRAINT FK_Positions_Units FOREIGN KEY (UnitId) REFERENCES Units(Id);

-- Clean up backup table
DROP TABLE Positions_backup;

-- Verify the new schema and show migrated data
SELECT TOP 5 * FROM Positions;
