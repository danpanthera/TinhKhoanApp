-- Fix Employees table Vietnamese encoding
USE TinhKhoanDB;
GO

UPDATE Employees SET FullName = N'Quản Trị Viên Hệ Thống' WHERE EmployeeCode = 'ADMIN001';
UPDATE Employees SET FullName = N'Nguyễn Văn An' WHERE EmployeeCode = 'LC001';
UPDATE Employees SET FullName = N'Trần Thị Bình' WHERE EmployeeCode = 'LC002';

-- Fix Positions table
UPDATE Positions SET Name = N'Quản Trị Viên' WHERE Id = 1;
UPDATE Positions SET Name = N'Phó Giám đốc' WHERE Id = 2;
UPDATE Positions SET Name = N'Trưởng phòng' WHERE Id = 3;
UPDATE Positions SET Name = N'Phó Trưởng phòng' WHERE Id = 4;
UPDATE Positions SET Name = N'Nhân viên' WHERE Id = 5;
UPDATE Positions SET Name = N'Giám đốc PGD' WHERE Id = 6;
UPDATE Positions SET Name = N'Phó Giám đốc PGD' WHERE Id = 7;
UPDATE Positions SET Name = N'Giao dịch viên' WHERE Id = 8;

-- Fix Roles table
UPDATE Roles SET Name = N'Quản trị viên hệ thống' WHERE Code = 'Admin';
UPDATE Roles SET Name = N'Giám đốc' WHERE Code = 'GiamDoc';
UPDATE Roles SET Name = N'Phó Giám đốc' WHERE Code = 'PhoGiamDoc';
UPDATE Roles SET Name = N'Trưởng phòng' WHERE Code = 'TruongPhong';
UPDATE Roles SET Name = N'Phó Trưởng phòng' WHERE Code = 'PhoTruongPhong';
UPDATE Roles SET Name = N'Nhân viên' WHERE Code = 'NhanVien';
UPDATE Roles SET Name = N'Kế toán' WHERE Code = 'KeToan';
UPDATE Roles SET Name = N'Thủ quỹ' WHERE Code = 'ThuQuy';
UPDATE Roles SET Name = N'Chuyên viên Khách hàng' WHERE Code = 'KhachHang';
UPDATE Roles SET Name = N'Kiểm tra viên' WHERE Code = 'KiemTra';
UPDATE Roles SET Name = N'Kế hoạch' WHERE Code = 'KeHoach';
UPDATE Roles SET Name = N'Quản lý rủi ro' WHERE Code = 'QuanLyRuiRo';
UPDATE Roles SET Name = N'Tổng hợp' WHERE Code = 'TongHop';
UPDATE Roles SET Name = N'Bảo vệ' WHERE Code = 'BaoVe';
UPDATE Roles SET Name = N'Lao công' WHERE Code = 'LaoCong';
UPDATE Roles SET Name = N'Tài xế' WHERE Code = 'TaiXe';
UPDATE Roles SET Name = N'Bảo trì' WHERE Code = 'BaoTri';
UPDATE Roles SET Name = N'Công nghệ thông tin' WHERE Code = 'IT';
UPDATE Roles SET Name = N'Thanh toán' WHERE Code = 'ThanhToan';
UPDATE Roles SET Name = N'Tín dụng' WHERE Code = 'TinDung';
UPDATE Roles SET Name = N'Huy động' WHERE Code = 'HuyDong';
UPDATE Roles SET Name = N'Dịch vụ' WHERE Code = 'DichVu';
UPDATE Roles SET Name = N'Ngoại hối' WHERE Code = 'NgoaiHoi';

PRINT 'Employees, Positions, and Roles tables fixed successfully!';
GO
