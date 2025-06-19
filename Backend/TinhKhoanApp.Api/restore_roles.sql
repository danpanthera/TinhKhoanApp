-- Script phục hồi vai trò chuẩn
-- Xóa tất cả vai trò hiện tại và tạo lại 23 vai trò chuẩn

-- Bước 1: Xóa tất cả vai trò hiện tại
DELETE FROM Roles;

-- Bước 2: Reset AUTO_INCREMENT về 1
DELETE FROM sqlite_sequence WHERE name='Roles';

-- Bước 3: Thêm 23 vai trò chuẩn theo danh sách yêu cầu
INSERT INTO Roles (Name, Description, IsActive, CreatedDate) VALUES
-- ID 1: TruongphongKhdn
('Trưởng phòng KHDN', 'Trưởng phòng Khách hàng Doanh nghiệp', 1, datetime('now')),

-- ID 2: TruongphongKhcn  
('Trưởng phòng KHCN', 'Trưởng phòng Khách hàng Cá nhân', 1, datetime('now')),

-- ID 3: PhophongKhdn
('Phó phòng KHDN', 'Phó phòng Khách hàng Doanh nghiệp', 1, datetime('now')),

-- ID 4: PhophongKhcn
('Phó phòng KHCN', 'Phó phòng Khách hàng Cá nhân', 1, datetime('now')),

-- ID 5: TruongphongKhqlrr
('Trưởng phòng KH&QLRR', 'Trưởng phòng Kế hoạch và Quản lý rủi ro', 1, datetime('now')),

-- ID 6: PhophongKhqlrr
('Phó phòng KH&QLRR', 'Phó phòng Kế hoạch và Quản lý rủi ro', 1, datetime('now')),

-- ID 7: Cbtd
('Cán bộ tín dụng', 'Cán bộ Tín dụng', 1, datetime('now')),

-- ID 8: TruongphongKtnqCnl1
('Trưởng phòng KTNQ CNL1', 'Trưởng phòng Kế toán & Ngân quỹ Chi nhánh loại 1', 1, datetime('now')),

-- ID 9: PhophongKtnqCnl1
('Phó phòng KTNQ CNL1', 'Phó phòng Kế toán & Ngân quỹ Chi nhánh loại 1', 1, datetime('now')),

-- ID 10: Gdv
('GDV', 'Giao dịch viên', 1, datetime('now')),

-- ID 11: TqHkKtnb
('Thủ quỹ | Hậu kiểm | KTNB', 'Thủ quỹ/Hậu kiểm/Kế toán Nội bộ', 1, datetime('now')),

-- ID 12: TruongphoItThKtgs
('Trưởng phó IT | Tổng hợp | KTGS', 'Trưởng phó các bộ phận IT/Tổng hợp/Kiểm tra Giám sát', 1, datetime('now')),

-- ID 13: CBItThKtgsKhqlrr
('Cán bộ IT | Tổng hợp | KTGS | KH&QLRR', 'Cán bộ IT/TH/KTGS/KHQLRR', 1, datetime('now')),

-- ID 14: GiamdocPgd
('Giám đốc Phòng giao dịch', 'Giám đốc Phòng giao dịch', 1, datetime('now')),

-- ID 15: PhogiamdocPgd
('Phó giám đốc Phòng giao dịch', 'Phó giám đốc Phòng giao dịch', 1, datetime('now')),

-- ID 16: PhogiamdocPgdCbtd
('Phó giám đốc Phòng giao dịch kiêm CBTD', 'Phó giám đốc PGD kiêm nhiệm Cán bộ Tín dụng', 1, datetime('now')),

-- ID 17: GiamdocCnl2
('Giám đốc CNL2', 'Giám đốc Chi nhánh loại 2', 1, datetime('now')),

-- ID 18: PhogiamdocCnl2Td
('Phó giám đốc CNL2 phụ trách Tín dụng', 'Phó giám đốc Chi nhánh loại 2 phụ trách Tín dụng', 1, datetime('now')),

-- ID 19: PhogiamdocCnl2Kt
('Phó giám đốc CNL2 phụ trách Kế toán', 'Phó giám đốc Chi nhánh loại 2 phụ trách Kế toán', 1, datetime('now')),

-- ID 20: TruongphongKhCnl2
('Trưởng phòng Khách hàng CNL2', 'Trưởng phòng Khách hàng Chi nhánh loại 2', 1, datetime('now')),

-- ID 21: PhophongKhCnl2
('Phó phòng Khách hàng CNL2', 'Phó phòng Khách hàng Chi nhánh loại 2', 1, datetime('now')),

-- ID 22: TruongphongKtnqCnl2
('Trưởng phòng KTNQ CNL2', 'Trưởng phòng Kế toán & Ngân quỹ Chi nhánh loại 2', 1, datetime('now')),

-- ID 23: PhophongKtnqCnl2
('Phó phòng KTNQ CNL2', 'Phó phòng Kế toán & Ngân quỹ Chi nhánh loại 2', 1, datetime('now'));

-- Bước 4: Kiểm tra kết quả
SELECT 'Danh sách vai trò sau khi phục hồi:' as info;
SELECT Id, Name, Description 
FROM Roles 
ORDER BY Id;

SELECT 'Tổng số vai trò:', COUNT(*) as total_roles FROM Roles;

-- Bước 5: Kiểm tra tương ứng với KpiAssignmentTables
SELECT 'Kiểm tra mapping với KPI Tables:' as info;
SELECT 
    r.Id as RoleId,
    r.Name as RoleName,
    k.Id as TableId,
    k.TableName as KpiTableName,
    CASE 
        WHEN r.Id = k.Id THEN '✅ Match'
        ELSE '❌ Mismatch'
    END as Status
FROM Roles r
LEFT JOIN KpiAssignmentTables k ON r.Id = k.Id AND k.Category = 'Dành cho Cán bộ'
ORDER BY r.Id;
