-- =============================================
-- Delete all data related to Units and Roles
-- Created: 2025-07-06
-- WARNING: This script will delete ALL data related to Units and Roles
-- Make sure to run backup_units_roles_data.sql first!
-- =============================================

-- Disable foreign key constraints temporarily
EXEC sp_MSforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT all";
PRINT 'Foreign key constraints disabled';

-- Delete in proper order to avoid constraint violations

-- 1. Delete Employee KPI Assignments (if they reference Units/Roles)
IF OBJECT_ID('EmployeeKpiAssignments', 'U') IS NOT NULL
BEGIN
    DELETE FROM EmployeeKpiAssignments;
    PRINT 'Deleted all EmployeeKpiAssignments';
END

-- 2. Delete Branch KPI Assignments (if they reference Units)
IF OBJECT_ID('BranchKpiAssignments', 'U') IS NOT NULL
BEGIN
    DELETE FROM BranchKpiAssignments;
    PRINT 'Deleted all BranchKpiAssignments';
END

-- 3. Delete EmployeeRoles junction table
IF OBJECT_ID('EmployeeRoles', 'U') IS NOT NULL
BEGIN
    DELETE FROM EmployeeRoles;
    PRINT 'Deleted all EmployeeRoles';
END

-- 4. Update Employees to remove Unit references (set to NULL instead of deleting employees)
IF OBJECT_ID('Employees', 'U') IS NOT NULL
BEGIN
    UPDATE Employees SET UnitId = NULL WHERE UnitId IS NOT NULL;
    PRINT 'Removed Unit references from Employees';
END

-- 5. Delete child Units first (those with ParentUnitId)
DELETE FROM Units WHERE ParentUnitId IS NOT NULL;
PRINT 'Deleted child Units';

-- 6. Delete parent Units
DELETE FROM Units WHERE ParentUnitId IS NULL;
PRINT 'Deleted parent Units';

-- 7. Delete all Roles
DELETE FROM Roles;
PRINT 'Deleted all Roles';

-- Re-enable foreign key constraints
EXEC sp_MSforeachtable "ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all";
PRINT 'Foreign key constraints re-enabled';

-- Verify deletion
SELECT COUNT(*) as RemainingUnits FROM Units;
SELECT COUNT(*) as RemainingRoles FROM Roles;

-- Reset identity columns if needed
IF EXISTS (SELECT * FROM sys.identity_columns WHERE OBJECT_NAME(object_id) = 'Units')
BEGIN
    DBCC CHECKIDENT ('Units', RESEED, 0);
    PRINT 'Units identity reset';
END

IF EXISTS (SELECT * FROM sys.identity_columns WHERE OBJECT_NAME(object_id) = 'Roles')
BEGIN
    DBCC CHECKIDENT ('Roles', RESEED, 0);
    PRINT 'Roles identity reset';
END

PRINT 'Deletion completed successfully. All Units and Roles data has been removed.';
