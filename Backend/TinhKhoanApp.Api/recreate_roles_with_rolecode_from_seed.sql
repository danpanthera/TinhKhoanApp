-- Script xóa hết vai trò hiện tại và tái tạo lại 23 vai trò từ SeedKPIDefinitionMaxScore.cs
-- Ngày: 19/06/2025
-- Mục đích: Sử dụng roleCode từ SeedKPIDefinitionMaxScore.cs để tạo lại vai trò chuẩn

-- 1. XÓA HẾT DỮ LIỆU CŨ
SELECT 'Bắt đầu xóa dữ liệu cũ...' as status;

-- Xóa tất cả KPI Indicators
DELETE FROM KpiIndicators;
DELETE FROM sqlite_sequence WHERE name='KpiIndicators';

-- Xóa tất cả KPI Assignment Tables
DELETE FROM KpiAssignmentTables;
DELETE FROM sqlite_sequence WHERE name='KpiAssignmentTables';

-- Xóa tất cả Roles
DELETE FROM Roles;
DELETE FROM sqlite_sequence WHERE name='Roles';

SELECT 'Đã xóa hết dữ liệu cũ' as status;

-- 2. TẠI LẠI 23 VAI TRÒ VỚI ROLECODE TỪ SeedKPIDefinitionMaxScore.cs
SELECT 'Bắt đầu tạo lại 23 vai trò từ SeedKPIDefinitionMaxScore.cs...' as status;

INSERT INTO Roles (Name, Description, IsActive, CreatedDate) VALUES
-- 1. TruongphongKhdn
('TruongphongKhdn', 'Trưởng phòng KHDN', 1, datetime('now')),

-- 2. TruongphongKhcn
('TruongphongKhcn', 'Trưởng phòng KHCN', 1, datetime('now')),

-- 3. PhophongKhdn
('PhophongKhdn', 'Phó phòng KHDN', 1, datetime('now')),

-- 4. PhophongKhcn
('PhophongKhcn', 'Phó phòng KHCN', 1, datetime('now')),

-- 5. TruongphongKhqlrr
('TruongphongKhqlrr', 'Trưởng phòng Kế hoạch và Quản lý rủi ro', 1, datetime('now')),

-- 6. PhophongKhqlrr
('PhophongKhqlrr', 'Phó phòng Kế hoạch và Quản lý rủi ro', 1, datetime('now')),

-- 7. Cbtd
('Cbtd', 'Cán bộ Tín dụng', 1, datetime('now')),

-- 8. TruongphongKtnqCnl1
('TruongphongKtnqCnl1', 'Trưởng phòng KTNQ CNL1', 1, datetime('now')),

-- 9. PhophongKtnqCnl1
('PhophongKtnqCnl1', 'Phó phòng KTNQ CNL1', 1, datetime('now')),

-- 10. Gdv
('Gdv', 'Giao dịch viên', 1, datetime('now')),

-- 11. TqHkKtnb (tạm thời sử dụng roleCode này vì trong SeedKPIDefinitionMaxScore.cs chưa có roleCode cụ thể)
('TqHkKtnb', 'TQ/Hậu kiểm/Kế toán nội bộ', 1, datetime('now')),

-- 12. TruongphongItThKtgs
('TruongphongItThKtgs', 'Trưởng phó phòng IT/TH/KTGS', 1, datetime('now')),

-- 13. CbItThKtgsKhqlrr
('CbItThKtgsKhqlrr', 'CB IT/TH/KTGS/KHQLRR', 1, datetime('now')),

-- 14. GiamdocPgd
('GiamdocPgd', 'Giám đốc PGD', 1, datetime('now')),

-- 15. PhogiamdocPgd
('PhogiamdocPgd', 'Phó giám đốc PGD', 1, datetime('now')),

-- 16. PhogiamdocPgdCbtd
('PhogiamdocPgdCbtd', 'Phó giám đốc PGD kiêm CBTD', 1, datetime('now')),

-- 17. GiamdocCnl2
('GiamdocCnl2', 'Giám đốc CNL2', 1, datetime('now')),

-- 18. PhogiamdocCnl2Td
('PhogiamdocCnl2Td', 'Phó giám đốc CNL2 phụ trách Tín dụng', 1, datetime('now')),

-- 19. PhogiamdocCnl2Kt
('PhogiamdocCnl2Kt', 'Phó giám đốc CNL2 phụ trách Kế toán', 1, datetime('now')),

-- 20. TruongphongKhCnl2
('TruongphongKhCnl2', 'Trưởng phòng KH CNL2', 1, datetime('now')),

-- 21. PhophongKhCnl2
('PhophongKhCnl2', 'Phó phòng KH CNL2', 1, datetime('now')),

-- 22. TruongphongKtnqCnl2
('TruongphongKtnqCnl2', 'Trưởng phòng KTNQ CNL2', 1, datetime('now')),

-- 23. PhophongKtnqCnl2
('PhophongKtnqCnl2', 'Phó phòng KTNQ CNL2', 1, datetime('now'));

-- 3. KIỂM TRA KẾT QUẢ
SELECT 'Hoàn thành tạo vai trò mới!' as status;
SELECT COUNT(*) as 'Tổng số vai trò (phải là 23)' FROM Roles;

SELECT 'Danh sách 23 vai trò mới (sử dụng roleCode từ SeedKPIDefinitionMaxScore.cs):' as info;
SELECT ROW_NUMBER() OVER (ORDER BY Id) as '#', Id, Name as RoleCode, Description FROM Roles ORDER BY Id;

SELECT 'Sẵn sàng để tạo lại KPI Assignment Tables và Indicators!' as final_status;
