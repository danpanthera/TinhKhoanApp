-- Fix Position table schema to match EF models
-- Current schema has: PositionName, PositionCode
-- EF Model expects: Name, Description

-- First, backup the current data
CREATE TABLE Positions_backup AS SELECT * FROM Positions;

-- Drop the old table
DROP TABLE Positions;

-- Create new table with correct schema to match EF models
CREATE TABLE "Positions" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Positions" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT,
    CONSTRAINT "FK_Positions_Units" FOREIGN KEY ("Id") REFERENCES "Units" ("Id") ON DELETE RESTRICT
);

-- Migrate data from backup (map PositionName to Name, PositionCode to Description)
INSERT INTO Positions (Id, Name, Description) 
SELECT Id, PositionName, PositionCode FROM Positions_backup;

-- Clean up backup table
DROP TABLE Positions_backup;

-- Verify the new schema
.schema Positions

-- Show migrated data
SELECT * FROM Positions LIMIT 5;
