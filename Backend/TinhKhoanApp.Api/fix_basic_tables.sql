-- Fix Employees, Positions tables Vietnamese encoding
USE TinhKhoanDB;
GO

-- Fix Employees table
UPDATE Employees SET FullName = N'Quản Trị Viên Hệ Thống' WHERE EmployeeCode = 'ADMIN001';
UPDATE Employees SET FullName = N'Nguyễn Văn An' WHERE EmployeeCode = 'LC001';
UPDATE Employees SET FullName = N'Trần Thị Bình' WHERE EmployeeCode = 'LC002';

-- Fix Positions table by ID
UPDATE Positions SET Name = N'Quản Trị Viên' WHERE Id = 1;
UPDATE Positions SET Name = N'Phó Giám đốc' WHERE Id = 2;
UPDATE Positions SET Name = N'Trưởng phòng' WHERE Id = 3;
UPDATE Positions SET Name = N'Phó Trưởng phòng' WHERE Id = 4;
UPDATE Positions SET Name = N'Nhân viên' WHERE Id = 5;
UPDATE Positions SET Name = N'Giám đốc PGD' WHERE Id = 6;
UPDATE Positions SET Name = N'Phó Giám đốc PGD' WHERE Id = 7;
UPDATE Positions SET Name = N'Giao dịch viên' WHERE Id = 8;

-- Fix Roles table by name pattern
UPDATE Roles SET Name = N'Trưởng phòng Khách hàng Doanh nghiệp' WHERE Name = 'TruongphongKhdn';
UPDATE Roles SET Name = N'Trưởng phòng Khách hàng Cá nhân' WHERE Name = 'TruongphongKhcn';
UPDATE Roles SET Name = N'Phó phòng Khách hàng Doanh nghiệp' WHERE Name = 'PhophongKhdn';
UPDATE Roles SET Name = N'Phó phòng Khách hàng Cá nhân' WHERE Name = 'PhophongKhcn';
UPDATE Roles SET Name = N'Trưởng phòng Kế hoạch & Quản lý rủi ro' WHERE Name = 'TruongphongKhqlrr';

PRINT 'Basic tables fixed successfully!';
GO
