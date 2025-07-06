-- =============================================
-- Backup script for Units and Roles data
-- Created: 2025-07-06
-- Purpose: Backup before deletion for recovery if needed
-- =============================================

-- Backup Units table
SELECT * INTO UnitsBackup_20250706 FROM Units;
PRINT 'Units data backed up to UnitsBackup_20250706';

-- Backup Roles table
SELECT * INTO RolesBackup_20250706 FROM Roles;
PRINT 'Roles data backed up to RolesBackup_20250706';

-- Backup EmployeeRoles table if exists
IF OBJECT_ID('EmployeeRoles', 'U') IS NOT NULL
BEGIN
    SELECT * INTO EmployeeRolesBackup_20250706 FROM EmployeeRoles;
    PRINT 'EmployeeRoles data backed up to EmployeeRolesBackup_20250706';
END

-- Backup Employees table (contains UnitId references)
IF OBJECT_ID('Employees', 'U') IS NOT NULL
BEGIN
    SELECT * INTO EmployeesBackup_20250706 FROM Employees;
    PRINT 'Employees data backed up to EmployeesBackup_20250706';
END

-- Backup any KPI assignment tables that reference Units or Roles
-- Employee KPI Assignment tables
IF OBJECT_ID('EmployeeKpiAssignments', 'U') IS NOT NULL
BEGIN
    SELECT * INTO EmployeeKpiAssignmentsBackup_20250706 FROM EmployeeKpiAssignments;
    PRINT 'EmployeeKpiAssignments data backed up';
END

-- Branch KPI Assignment tables
IF OBJECT_ID('BranchKpiAssignments', 'U') IS NOT NULL
BEGIN
    SELECT * INTO BranchKpiAssignmentsBackup_20250706 FROM BranchKpiAssignments;
    PRINT 'BranchKpiAssignments data backed up';
END

-- Check for other tables that might reference Units or Roles
SELECT
    t.name as TableName,
    c.name as ColumnName,
    'References Units' as RelationType
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.tables t ON fkc.parent_object_id = t.object_id
INNER JOIN sys.columns c ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
INNER JOIN sys.tables ref_t ON fkc.referenced_object_id = ref_t.object_id
WHERE ref_t.name = 'Units'

UNION ALL

SELECT
    t.name as TableName,
    c.name as ColumnName,
    'References Roles' as RelationType
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.tables t ON fkc.parent_object_id = t.object_id
INNER JOIN sys.columns c ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
INNER JOIN sys.tables ref_t ON fkc.referenced_object_id = ref_t.object_id
WHERE ref_t.name = 'Roles';

PRINT 'Backup completed successfully. Data can be restored if needed.';
