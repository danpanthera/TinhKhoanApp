-- Update admin user position from "Giamdoc" to "CB" (Cán bộ)
-- This script removes the "Giamdoc" position assignment from the admin user

-- First, let's check current admin user position
SELECT 
    e.Id,
    e.Username,
    e.FullName,
    e.PositionId,
    p.Name as CurrentPosition
FROM Employees e
INNER JOIN Positions p ON e.PositionId = p.Id
WHERE e.Username = 'admin';

-- Get the "CB" position ID
DECLARE @CBPositionId INT;
SELECT @CBPositionId = Id FROM Positions WHERE Name = 'CB';

-- Update admin user to use "CB" position instead of "Giamdoc"
UPDATE Employees 
SET PositionId = @CBPositionId
WHERE Username = 'admin' AND PositionId = (SELECT Id FROM Positions WHERE Name = 'Giamdoc');

-- Verify the update
SELECT 
    e.Id,
    e.Username,
    e.FullName,
    e.PositionId,
    p.Name as NewPosition
FROM Employees e
INNER JOIN Positions p ON e.PositionId = p.Id
WHERE e.Username = 'admin';

PRINT 'Admin user position updated successfully from Giamdoc to CB';
