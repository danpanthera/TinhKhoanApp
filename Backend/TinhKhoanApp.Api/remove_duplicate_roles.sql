-- Remove duplicate roles and keep only standardized versions
-- Date: $(date '+%Y-%m-%d %H:%M:%S')
-- Purpose: Remove CBTD, GDV, TQ/HK/KTNB in favor of Cbtd, Gdv, TqHkKtnb

-- 1. VERIFY DUPLICATE ROLES BEFORE DELETION
SELECT 'DUPLICATE ROLES TO BE REMOVED:' as Status;
SELECT Id, Name, Description 
FROM Roles 
WHERE Name IN ('CBTD', 'GDV', 'TQ/HK/KTNB')
ORDER BY Id;

-- 2. VERIFY STANDARD ROLES TO BE KEPT
SELECT 'STANDARD ROLES TO BE KEPT:' as Status;
SELECT Id, Name, Description 
FROM Roles 
WHERE Name IN ('Cbtd', 'Gdv', 'TqHkKtnb', 'PhogiamdocPgdCbtd')
ORDER BY Id;

-- 3. CHECK FOR ANY EXISTING REFERENCES (KPI assignments, Employee roles, etc.)
SELECT 'CHECKING REFERENCES FOR DUPLICATE ROLES:' as Status;

-- Check if duplicates have any KPI assignments 
SELECT 'KPI Assignment References:' as CheckType, 
       r.Name as RoleName, 
       COUNT(*) as ReferenceCount
FROM Roles r
LEFT JOIN EmployeeRoles er ON r.Id = er.RoleId
WHERE r.Name IN ('CBTD', 'GDV', 'TQ/HK/KTNB')
GROUP BY r.Id, r.Name;

-- 4. SAFE DELETION - Remove duplicate roles
DELETE FROM Roles 
WHERE Name IN ('CBTD', 'GDV', 'TQ/HK/KTNB');

-- 5. VERIFICATION AFTER DELETION
SELECT 'VERIFICATION AFTER DELETION:' as Status;
SELECT COUNT(*) as 'Total Roles Remaining' FROM Roles;

-- 6. CONFIRM NO DUPLICATES EXIST
SELECT 'FINAL CHECK - NO DUPLICATES SHOULD EXIST:' as Status;
SELECT Id, Name 
FROM Roles 
WHERE Name IN ('CBTD', 'GDV', 'TQ/HK/KTNB', 'Cbtd', 'Gdv', 'TqHkKtnb')
ORDER BY Name, Id;

-- 7. SUCCESS MESSAGE
SELECT '✅ DUPLICATE ROLES REMOVED SUCCESSFULLY' as Result;
