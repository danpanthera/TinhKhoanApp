-- ===============================================
-- DATABASE CLEANUP SCRIPT
-- Purpose: Clean up redundant data as requested
-- ===============================================

USE TinhKhoanApp;
GO

-- 1. DELETE DEPARTMENTS WITH ID >= 1026
PRINT 'Deleting departments with ID >= 1026...';
DELETE FROM Departments WHERE Id >= 1026;
PRINT 'Departments deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
GO

-- 2. DELETE ALL EXISTING POSITIONS AND RECREATE STANDARD ONES
PRINT 'Deleting all existing positions...';
DELETE FROM Positions;
PRINT 'Positions deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
GO

-- Reset identity seed for Positions
DBCC CHECKIDENT ('Positions', RESEED, 0);
GO

-- 3. INSERT STANDARD POSITIONS
PRINT 'Creating standard positions...';
INSERT INTO Positions (Id, Name, Description, CreatedDate, LastModifiedDate, IsDeleted) VALUES
(1, N'Giám đốc', N'Giám đốc công ty', GETDATE(), GETDATE(), 0),
(2, N'Phó Giám đốc', N'Phó Giám đốc công ty', GETDATE(), GETDATE(), 0),
(3, N'Trưởng phòng', N'Trưởng phòng ban', GETDATE(), GETDATE(), 0),
(4, N'Phó trưởng phòng', N'Phó trưởng phòng ban', GETDATE(), GETDATE(), 0),
(5, N'Giám đốc Phòng giao dịch', N'Giám đốc Phòng giao dịch', GETDATE(), GETDATE(), 0),
(6, N'Phó giám đốc Phòng giao dịch', N'Phó giám đốc Phòng giao dịch', GETDATE(), GETDATE(), 0),
(7, N'Nhân viên', N'Nhân viên', GETDATE(), GETDATE(), 0);
PRINT 'Standard positions created: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
GO

-- 4. DELETE ROLES WITH SPACES IN CODE
PRINT 'Deleting roles with spaces in code...';
DELETE FROM Roles WHERE Code LIKE '% %';
PRINT 'Roles with spaces deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
GO

-- 5. VERIFY CLEANUP RESULTS
PRINT 'Verification results:';
PRINT 'Total departments remaining: ' + CAST((SELECT COUNT(*) FROM Departments) AS VARCHAR(10));
PRINT 'Total positions: ' + CAST((SELECT COUNT(*) FROM Positions) AS VARCHAR(10));
PRINT 'Total roles: ' + CAST((SELECT COUNT(*) FROM Roles) AS VARCHAR(10));
PRINT 'Roles without spaces: ' + CAST((SELECT COUNT(*) FROM Roles WHERE Code NOT LIKE '% %') AS VARCHAR(10));
GO

-- 6. LIST CURRENT POSITIONS
PRINT 'Current positions:';
SELECT Id, Name, Description FROM Positions ORDER BY Id;
GO

-- 7. LIST REMAINING ROLES
PRINT 'Remaining roles (sample):';
SELECT TOP 10 Id, Code, Name FROM Roles ORDER BY Code;
GO

PRINT 'Database cleanup completed successfully!';
GO
