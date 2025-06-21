-- Update roles to match exactly what's defined in SeedKPIDefinitionMaxScore.cs
-- This script updates role names to align with the canonical definitions

-- Update role 7: CBTD -> Cán bộ Tín dụng
UPDATE Roles SET Name = 'Cán bộ Tín dụng' WHERE Id = 7;

-- Update role 10: GDV -> Giao dịch viên  
UPDATE Roles SET Name = 'Giao dịch viên' WHERE Id = 10;

-- Update role 11: TQ/HK/KTNB -> TQ/Hậu kiểm/Kế toán nội bộ
UPDATE Roles SET Name = 'TQ/Hậu kiểm/Kế toán nội bộ' WHERE Id = 11;

-- Update role 12: Trưởng phòng IT/TH/KTGS -> Trưởng phó phòng IT/TH/KTGS
UPDATE Roles SET Name = 'Trưởng phó phòng IT/TH/KTGS' WHERE Id = 12;

-- Update role 19: Phó giám đốc CNL2 Phụ trách Kế toán -> Phó giám đốc CNL2 Phụ trách Kế toán
UPDATE Roles SET Name = 'Phó giám đốc CNL2 phụ trách Kế toán' WHERE Id = 19;

-- Verify changes
SELECT 'UPDATED ROLES:' as status;
SELECT Id, Name FROM Roles WHERE Id IN (7, 10, 11, 12, 19) ORDER BY Id;

-- Verify total count (should still be 23)
SELECT COUNT(*) as 'Total Roles' FROM Roles;

-- Display all roles to confirm they match SeedKPIDefinitionMaxScore.cs exactly
SELECT 'ALL ROLES AFTER UPDATE:' as status;
SELECT Id, Name FROM Roles ORDER BY Id;
