-- Script tạo 10 sample employees bằng SQL trực tiếp
-- Ngày tạo: 12/07/2025

USE TinhKhoanDB;
GO

-- Insert 10 sample employees
INSERT INTO Employees (EmployeeCode, CBCode, FullName, Username, Email, PhoneNumber, IsActive, UnitId, PositionId, CreatedDate, IsDeleted)
VALUES
-- 1. Giám đốc Lai Châu (Unit 1)
(N'GD001', N'123456001', N'Nguyễn Văn An - Giám đốc CN Lai Châu', N'giamdoc.laichau', N'giamdoc@agribank.com.vn', N'0987654321', 1, 1, 1, GETDATE(), 0),

-- 2. Trưởng phòng KHDN (Unit 4)
(N'TP001', N'123456002', N'Trần Thị Bình - TP KHDN', N'tp.khdn', N'tp.khdn@agribank.com.vn', N'0987654322', 1, 4, 2, GETDATE(), 0),

-- 3. Phó phòng KHDN (Unit 4)
(N'PP001', N'123456003', N'Lê Văn Cường - PP KHDN', N'pp.khdn', N'pp.khdn@agribank.com.vn', N'0987654323', 1, 4, 3, GETDATE(), 0),

-- 4. Trưởng phòng KHCN (Unit 5)
(N'TP002', N'123456004', N'Phạm Thị Dung - TP KHCN', N'tp.khcn', N'tp.khcn@agribank.com.vn', N'0987654324', 1, 5, 2, GETDATE(), 0),

-- 5. Cán bộ tín dụng (Unit 4)
(N'CB001', N'123456005', N'Hoàng Văn Minh - CBTD', N'cbtd.01', N'cbtd.01@agribank.com.vn', N'0987654325', 1, 4, 4, GETDATE(), 0),

-- 6. Trưởng phòng KTNQ (Unit 6)
(N'TP003', N'123456006', N'Vũ Thị Hoa - TP KTNQ', N'tp.ktnq', N'tp.ktnq@agribank.com.vn', N'0987654326', 1, 6, 2, GETDATE(), 0),

-- 7. Giao dịch viên (Unit 6)
(N'GDV001', N'123456007', N'Đỗ Văn Tùng - GDV', N'gdv.01', N'gdv.01@agribank.com.vn', N'0987654327', 1, 6, 5, GETDATE(), 0),

-- 8. Trưởng phòng KH&QLRR (Unit 8)
(N'TP004', N'123456008', N'Ngô Thị Lan - TP KH&QLRR', N'tp.khqlrr', N'tp.khqlrr@agribank.com.vn', N'0987654328', 1, 8, 2, GETDATE(), 0),

-- 9. Giám đốc CNL2 Bình Lư (Unit 10)
(N'GD002', N'123456009', N'Bùi Văn Đức - GĐ CNL2 Bình Lư', N'gd.binhlu', N'gd.binhlu@agribank.com.vn', N'0987654329', 1, 10, 1, GETDATE(), 0),

-- 10. Phó Giám đốc CNL2 Phong Thổ (Unit 11)
(N'PGD001', N'123456010', N'Đinh Thị Mai - PGĐ Phong Thổ', N'pgd.phongtho', N'pgd.phongtho@agribank.com.vn', N'0987654330', 1, 11, 3, GETDATE(), 0);

-- Gán roles cho employees
INSERT INTO EmployeeRoles (EmployeeId, RoleId)
VALUES
-- Employee 1: Role 17 (Giám đốc CNL2)
(1, 17),
-- Employee 2: Role 1 (Trưởng phòng KHDN)
(2, 1),
-- Employee 3: Role 3 (Phó phòng KHDN)
(3, 3),
-- Employee 4: Role 2 (Trưởng phòng KHCN)
(4, 2),
-- Employee 5: Role 7 (Cán bộ tín dụng)
(5, 7),
-- Employee 6: Role 8 (Trưởng phòng KTNQ CNL1)
(6, 8),
-- Employee 7: Role 10 (GDV)
(7, 10),
-- Employee 8: Role 5 (Trưởng phòng KH&QLRR)
(8, 5),
-- Employee 9: Role 17 (Giám đốc CNL2)
(9, 17),
-- Employee 10: Role 18 (Phó giám đốc CNL2 TD)
(10, 18);

-- Kiểm tra kết quả
SELECT COUNT(*) as TotalEmployees FROM Employees;
SELECT COUNT(*) as TotalEmployeeRoles FROM EmployeeRoles;

PRINT '✅ Đã tạo 10 sample employees với roles tương ứng';
