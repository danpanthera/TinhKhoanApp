-- Tạo 23 vai trò chuẩn theo yêu cầu - SQL SERVER VERSION
-- Date: 2025-06-18

-- Xóa hết vai trò hiện có (IDENTITY column will reset automatically)
DELETE FROM Roles;

-- Reset identity seed for SQL Server
DBCC CHECKIDENT ('Roles', RESEED, 0);

-- Tạo 23 vai trò theo thứ tự yêu cầu
INSERT INTO Roles (name, description) VALUES
('TruongphongKhdn', 'Trưởng phòng Khách hàng Doanh nghiệp'),
('TruongphongKhcn', 'Trưởng phòng Khách hàng Cá nhân'), 
('PhophongKhdn', 'Phó phòng Khách hàng Doanh nghiệp'),
('PhophongKhcn', 'Phó phòng Khách hàng Cá nhân'),
('TruongphongKhqlrr', 'Trưởng phòng Kế hoạch và Quản lý rủi ro'),
('PhophongKhqlrr', 'Phó phòng Kế hoạch và Quản lý rủi ro'),
('Cbtd', 'Cán bộ Tín dụng'),
('TruongphongKtnqCnl1', 'Trưởng phòng Kế toán và Ngân quỹ Chi nhánh loại 1'),
('PhophongKtnqCnl1', 'Phó phòng Kế toán và Ngân quỹ Chi nhánh loại 1'),
('Gdv', 'Giao dịch viên'),
('TqHkKtnb', 'Thủ quỹ/Hậu kiểm/Kế toán Nội bộ'),
('TruongphoItThKtgs', 'Trưởng phó IT/Tổng hợp/Kiểm tra Giám sát'),
('CBItThKtgsKhqlrr', 'Cán bộ IT/Tổng hợp/KTGS/KH&QLRR'),
('GiamdocPgd', 'Giám đốc Phòng giao dịch'),
('PhogiamdocPgd', 'Phó giám đốc Phòng giao dịch'),
('PhogiamdocPgdCbtd', 'Phó giám đốc Phòng giao dịch kiêm Cán bộ Tín dụng'),
('GiamdocCnl2', 'Giám đốc Chi nhánh loại 2'),
('PhogiamdocCnl2Td', 'Phó giám đốc Chi nhánh loại 2 phụ trách Tín dụng'),
('PhogiamdocCnl2Kt', 'Phó giám đốc Chi nhánh loại 2 phụ trách Kế toán'),
('TruongphongKhCnl2', 'Trưởng phòng Khách hàng Chi nhánh loại 2'),
('PhophongKhCnl2', 'Phó phòng Khách hàng Chi nhánh loại 2'),
('TruongphongKtnqCnl2', 'Trưởng phòng Kế toán và Ngân quỹ Chi nhánh loại 2'),
('PhophongKtnqCnl2', 'Phó phòng Kế toán và Ngân quỹ Chi nhánh loại 2');

-- Kiểm tra kết quả
SELECT COUNT(*) as 'Total Roles Created' FROM Roles;
SELECT id, name, description FROM Roles ORDER BY id;
