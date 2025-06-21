-- Verification script to confirm all roles match SeedKPIDefinitionMaxScore.cs exactly
-- This script checks that we have exactly the 23 roles defined in SeedKPIDefinitionMaxScore.cs

SELECT 'VERIFICATION: Roles matching SeedKPIDefinitionMaxScore.cs' as verification_status;
SELECT '=========================================' as separator;

-- Check exact count
SELECT COUNT(*) as 'Total Roles (Should be 23)' FROM Roles;

-- List all roles to manually verify against SeedKPIDefinitionMaxScore.cs
SELECT 'Current roles in database:' as info;
SELECT ROW_NUMBER() OVER (ORDER BY Id) as '#', Id, Name FROM Roles ORDER BY Id;

-- Check for any roles NOT in the expected list from SeedKPIDefinitionMaxScore.cs
SELECT 'Any unexpected roles (should be empty):' as check_status;
SELECT Id, Name FROM Roles 
WHERE Name NOT IN (
    'Trưởng phòng KHDN',
    'Trưởng phòng KHCN', 
    'Phó phòng KHDN',
    'Phó phòng KHCN',
    'Trưởng phòng Kế hoạch và Quản lý rủi ro',
    'Phó phòng Kế hoạch và Quản lý rủi ro',
    'Cán bộ Tín dụng',
    'Trưởng phòng KTNQ CNL1',
    'Phó phòng KTNQ CNL1',
    'Giao dịch viên',
    'TQ/Hậu kiểm/Kế toán nội bộ',
    'Trưởng phó phòng IT/TH/KTGS',
    'CB IT/TH/KTGS/KHQLRR',
    'Giám đốc PGD',
    'Phó giám đốc PGD',
    'Phó giám đốc PGD kiêm CBTD',
    'Giám đốc CNL2',
    'Phó giám đốc CNL2 phụ trách Tín dụng',
    'Phó giám đốc CNL2 Phụ trách Kế toán',
    'Trưởng phòng KH CNL2',
    'Phó phòng KH CNL2',
    'Trưởng phòng KTNQ CNL2',
    'Phó phòng KTNQ CNL2'
);

SELECT 'VERIFICATION COMPLETE ✅' as final_status;
