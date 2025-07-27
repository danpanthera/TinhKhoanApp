-- ================================================
-- Script phục hồi 23 vai trò chuẩn từ RoleSeeder.cs
-- Tổng số: 23 vai trò KPI theo chuẩn hệ thống
-- ================================================

USE TinhKhoanDB;
GO

-- Xóa toàn bộ dữ liệu hiện tại để tránh trung lặp
PRINT '🗑️ Xóa dữ liệu roles hiện tại...';
DELETE FROM Roles;
GO

-- Reset IDENTITY seed để bắt đầu từ ID = 1
DBCC CHECKIDENT ('Roles', RESEED, 0);
GO

PRINT '🚀 Bắt đầu phục hồi 23 vai trò chuẩn...';

-- Thêm 23 vai trò theo thứ tự trong RoleSeeder.cs
INSERT INTO Roles (Name, Description) VALUES
('TruongphongKhdn', N'Trưởng phòng KHDN'),
('TruongphongKhcn', N'Trưởng phòng KHCN'),
('PhophongKhdn', N'Phó phòng KHDN'),
('PhophongKhcn', N'Phó phòng KHCN'),
('TruongphongKhqlrr', N'Trưởng phòng Kế hoạch và Quản lý rủi ro'),
('PhophongKhqlrr', N'Phó phòng Kế hoạch và Quản lý rủi ro'),
('Cbtd', N'Cán bộ Tín dụng'),
('TruongphongKtnqCnl1', N'Trưởng phòng KTNQ CNL1'),
('PhophongKtnqCnl1', N'Phó phòng KTNQ CNL1'),
('Gdv', N'Giao dịch viên'),
('TqHkKtnb', N'TQ/Hậu kiểm/Kế toán nội bộ'),
('TruongphongItThKtgs', N'Trưởng phó phòng IT/TH/KTGS'),
('CbItThKtgsKhqlrr', N'CB IT/TH/KTGS/KHQLRR'),
('GiamdocPgd', N'Giám đốc PGD'),
('PhogiamdocPgd', N'Phó giám đốc PGD'),
('PhogiamdocPgdCbtd', N'Phó giám đốc PGD kiêm CBTD'),
('GiamdocCnl2', N'Giám đốc CNL2'),
('PhogiamdocCnl2Td', N'Phó giám đốc CNL2 phụ trách Tín dụng'),
('PhogiamdocCnl2Kt', N'Phó giám đốc CNL2 Phụ trách Kế toán'),
('TruongphongKhCnl2', N'Trưởng phòng KH CNL2'),
('PhophongKhCnl2', N'Phó phòng KH CNL2'),
('TruongphongKtnqCnl2', N'Trưởng phòng KTNQ CNL2'),
('PhophongKtnqCnl2', N'Phó phòng KTNQ CNL2');

-- Kiểm tra kết quả
PRINT '📊 Kết quả phục hồi:';
SELECT
    'Total Roles' as Category,
    COUNT(*) as Count
FROM Roles;

PRINT '✅ Hoàn thành phục hồi 23 vai trò chuẩn!';

-- Hiển thị một số vai trò mẫu để xác minh
PRINT '🔍 Mẫu các vai trò đã được tạo:';
SELECT TOP 5
    Id,
    Name,
    Description
FROM Roles
ORDER BY Id;

PRINT '📋 Tổng số vai trò theo từng loại:';
SELECT
    CASE
        WHEN Name LIKE '%Truongphong%' THEN 'Trưởng phòng'
        WHEN Name LIKE '%Phophong%' THEN 'Phó phòng'
        WHEN Name LIKE '%Giamdoc%' THEN 'Giám đốc'
        WHEN Name LIKE '%Phogiamdoc%' THEN 'Phó giám đốc'
        WHEN Name IN ('Cbtd', 'Gdv', 'TqHkKtnb', 'CbItThKtgsKhqlrr') THEN 'Cán bộ chuyên môn'
        ELSE 'Khác'
    END as RoleCategory,
    COUNT(*) as Count
FROM Roles
GROUP BY
    CASE
        WHEN Name LIKE '%Truongphong%' THEN 'Trưởng phòng'
        WHEN Name LIKE '%Phophong%' THEN 'Phó phòng'
        WHEN Name LIKE '%Giamdoc%' THEN 'Giám đốc'
        WHEN Name LIKE '%Phogiamdoc%' THEN 'Phó giám đốc'
        WHEN Name IN ('Cbtd', 'Gdv', 'TqHkKtnb', 'CbItThKtgsKhqlrr') THEN 'Cán bộ chuyên môn'
        ELSE 'Khác'
    END
ORDER BY Count DESC;
