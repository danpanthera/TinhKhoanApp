-- Clean up roles and KPI tables to ensure only valid 23 roles exist
-- This script removes all invalid roles and resets the KPI assignment tables

-- 1. CLEAN UP INVALID ROLES
-- Delete all existing roles
DELETE FROM Roles;
DELETE FROM sqlite_sequence WHERE name='Roles';

-- 2. CLEAN UP KPI ASSIGNMENT TABLES AND INDICATORS
-- This will force the seeder to recreate them properly
DELETE FROM KpiIndicators;
DELETE FROM KpiAssignmentTables;
DELETE FROM sqlite_sequence WHERE name='KpiIndicators';
DELETE FROM sqlite_sequence WHERE name='KpiAssignmentTables';

-- 3. VERIFICATION
SELECT 'CLEANUP COMPLETED - Database is ready for fresh seeding' as Status;
SELECT COUNT(*) as 'Remaining Roles' FROM Roles;
SELECT COUNT(*) as 'Remaining KPI Tables' FROM KpiAssignmentTables;
SELECT COUNT(*) as 'Remaining KPI Indicators' FROM KpiIndicators;
