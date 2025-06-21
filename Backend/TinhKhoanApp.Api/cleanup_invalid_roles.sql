-- Clean up roles and KPI tables to ensure only valid 23 roles exist - SQL SERVER VERSION
-- This script removes all invalid roles and resets the KPI assignment tables

-- 1. CLEAN UP INVALID ROLES
-- Delete all existing roles (IDENTITY column will reset automatically)
DELETE FROM Roles;

-- 2. CLEAN UP KPI ASSIGNMENT TABLES AND INDICATORS
-- This will force the seeder to recreate them properly
DELETE FROM KpiIndicators;
DELETE FROM KpiAssignmentTables;

-- 3. RESET IDENTITY COLUMNS FOR SQL SERVER
-- Reset identity seeds if needed
DBCC CHECKIDENT ('Roles', RESEED, 0);
DBCC CHECKIDENT ('KpiIndicators', RESEED, 0);
DBCC CHECKIDENT ('KpiAssignmentTables', RESEED, 0);

-- 4. VERIFICATION
SELECT 'CLEANUP COMPLETED - Database is ready for fresh seeding' as Status;
SELECT COUNT(*) as 'Remaining Roles' FROM Roles;
SELECT COUNT(*) as 'Remaining KPI Tables' FROM KpiAssignmentTables;
SELECT COUNT(*) as 'Remaining KPI Indicators' FROM KpiIndicators;
