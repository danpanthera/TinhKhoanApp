-- FIX DATA CORRUPTION SCRIPT FOR TINH KHOAN APP
-- Sửa các ký tự tiếng Việt bị lỗi trong database

USE TinhKhoanDB;

-- Backup trước khi sửa
PRINT 'Starting data corruption fix...';

-- Fix Description field in KpiAssignmentTables
PRINT 'Fixing KpiAssignmentTables.Description...';

-- Chi nhánh names fixes
UPDATE KpiAssignmentTables SET Description = N'Chi nhánh Bum Tở' WHERE Id = 32;
UPDATE KpiAssignmentTables SET Description = N'Chi nhánh Đoàn Kết' WHERE Id = 33;
UPDATE KpiAssignmentTables SET Description = N'Chi nhánh Nậm Hàng' WHERE Id = 34;
UPDATE KpiAssignmentTables SET Description = N'Chi nhánh Phong Thổ' WHERE Id = 27;
UPDATE KpiAssignmentTables SET Description = N'Chi nhánh Sìn Hồ' WHERE Id = 28;
UPDATE KpiAssignmentTables SET Description = N'Hội Sở' WHERE Id = 26;

-- Job position fixes
UPDATE KpiAssignmentTables SET Description = N'Cán bộ tín dụng' WHERE Id = 9;
UPDATE KpiAssignmentTables SET Description = N'Giao dịch viên' WHERE Id = 12;
UPDATE KpiAssignmentTables SET Description = N'Giám đốc CNL2' WHERE Id = 19;
UPDATE KpiAssignmentTables SET Description = N'Giám đốc PGD' WHERE Id = 16;
UPDATE KpiAssignmentTables SET Description = N'Phó giám đốc CNL2/KT' WHERE Id = 21;
UPDATE KpiAssignmentTables SET Description = N'Phó giám đốc CNL2/TD' WHERE Id = 20;
UPDATE KpiAssignmentTables SET Description = N'Phó giám đốc PGD' WHERE Id = 17;
UPDATE KpiAssignmentTables SET Description = N'Phó giám đốc PGD/CBTD' WHERE Id = 18;
UPDATE KpiAssignmentTables SET Description = N'Tổ HK Kiểm tra nội bộ' WHERE Id = 13;
UPDATE KpiAssignmentTables SET Description = N'Trưởng phòng IT/TH/KTGS' WHERE Id = 14;
UPDATE KpiAssignmentTables SET Description = N'Trưởng phòng KHCN' WHERE Id = 4;
UPDATE KpiAssignmentTables SET Description = N'Trưởng phòng KH CNL2' WHERE Id = 22;
UPDATE KpiAssignmentTables SET Description = N'Trưởng phòng KHDN' WHERE Id = 3;
UPDATE KpiAssignmentTables SET Description = N'Trưởng phòng KH&QLRR' WHERE Id = 7;
UPDATE KpiAssignmentTables SET Description = N'Trưởng phòng KTNQ CNL1' WHERE Id = 10;
UPDATE KpiAssignmentTables SET Description = N'Trưởng phòng KTNQ CNL2' WHERE Id = 24;

PRINT 'Data corruption fix completed!';

-- Verify the fixes
SELECT 'After Fix - Records with UTF-8 issues:' as Status;
SELECT Id, Description
FROM KpiAssignmentTables
WHERE Description LIKE '%?%' OR Description LIKE '%Ð%' OR Description LIKE '%T?%'
ORDER BY Id;

PRINT 'Fix verification completed.';
