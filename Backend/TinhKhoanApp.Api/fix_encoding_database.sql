-- 🇻🇳 FIX VIETNAMESE ENCODING IN DATABASE
-- Khắc phục lỗi encoding tiếng Việt trong Azure SQL Edge

USE master;
GO

-- Drop existing database if needed (CAREFUL!)
-- DROP DATABASE IF EXISTS TinhKhoanDB;
-- GO

-- Create database with UTF-8 collation
-- CREATE DATABASE TinhKhoanDB COLLATE SQL_Latin1_General_CP1_CI_AS;
-- GO

USE TinhKhoanDB;
GO

-- Thay vì recreate database, ta sẽ fix dữ liệu trực tiếp
-- Fix Units table
UPDATE Units
SET Name = CASE
    WHEN Code = 'CnLaiChau' THEN N'Chi nhánh tỉnh Lai Châu'
    WHEN Code = 'HoiSo' THEN N'Hội Sở'
    WHEN Code = 'CnBinhLu' THEN N'Chi nhánh Bình Lu'
    WHEN Code = 'CnPhongTho' THEN N'Chi nhánh Phong Thổ'
    WHEN Code = 'CnSinHo' THEN N'Chi nhánh Sìn Hồ'
    WHEN Code = 'CnBumTo' THEN N'Chi nhánh Bum Tở'
    WHEN Code = 'CnThanUyen' THEN N'Chi nhánh Than Uyên'
    WHEN Code = 'CnDoanKet' THEN N'Chi nhánh Đoàn Kết'
    WHEN Code = 'CnTanUyen' THEN N'Chi nhánh Tân Uyên'
    WHEN Code = 'CnNamHang' THEN N'Chi nhánh Nậm Hàng'
    WHEN Code = 'HoiSoBgd' THEN N'Ban Giám đốc'
    WHEN Code = 'HoiSoKhdn' THEN N'Phòng Khách hàng Doanh nghiệp'
    WHEN Code = 'HoiSoKhcn' THEN N'Phòng Khách hàng Cá nhân'
    WHEN Code = 'HoiSoKtnq' THEN N'Phòng Kế toán & Ngân quỹ'
    WHEN Code = 'HoiSoTongHop' THEN N'Phòng Tổng hợp'
    WHEN Code = 'HoiSoKhqlrr' THEN N'Phòng Kế hoạch & Quản lý rủi ro'
    WHEN Code = 'HoiSoKtgs' THEN N'Phòng Kiểm tra giám sát'
    WHEN Code LIKE '%Bgd' THEN N'Ban Giám đốc'
    WHEN Code LIKE '%Ktnq' THEN N'Phòng Kế toán & Ngân quỹ'
    WHEN Code LIKE '%Kh' THEN N'Phòng Khách hàng'
    WHEN Code LIKE '%PgdSo%' THEN N'Phòng giao dịch số ' + RIGHT(Code, 1)
    ELSE Name
END
WHERE Name LIKE '%?%';

-- Fix Employees table
UPDATE Employees
SET FullName = CASE
    WHEN EmployeeCode = 'ADMIN001' THEN N'Quản Trị Viên Hệ Thống'
    WHEN EmployeeCode = 'LC001' THEN N'Nguyễn Văn An'
    WHEN EmployeeCode = 'LC002' THEN N'Trần Thị Bình'
    ELSE FullName
END
WHERE FullName LIKE '%?%';

-- Fix Positions table
UPDATE Positions
SET Name = CASE
    WHEN Code = 'QuanTriVien' THEN N'Quản Trị Viên'
    WHEN Code = 'PhoGiamDoc' THEN N'Phó Giám đốc'
    WHEN Code = 'TruongPhong' THEN N'Trưởng phòng'
    WHEN Code = 'PhoTruongPhong' THEN N'Phó Trưởng phòng'
    WHEN Code = 'NhanVien' THEN N'Nhân viên'
    WHEN Code = 'GiamdocPgd' THEN N'Giám đốc PGD'
    WHEN Code = 'PhogiamdocPgd' THEN N'Phó Giám đốc PGD'
    WHEN Code = 'Gdv' THEN N'Giao dịch viên'
    ELSE Name
END
WHERE Name LIKE '%?%';

-- Fix Roles table
UPDATE Roles
SET Name = CASE
    WHEN Code = 'Admin' THEN N'Quản trị viên hệ thống'
    WHEN Code = 'GiamDoc' THEN N'Giám đốc'
    WHEN Code = 'PhoGiamDoc' THEN N'Phó Giám đốc'
    WHEN Code = 'TruongPhong' THEN N'Trưởng phòng'
    WHEN Code = 'PhoTruongPhong' THEN N'Phó Trưởng phòng'
    WHEN Code = 'NhanVien' THEN N'Nhân viên'
    WHEN Code = 'KeToan' THEN N'Kế toán'
    WHEN Code = 'ThuQuy' THEN N'Thủ quỹ'
    WHEN Code = 'KhachHang' THEN N'Chuyên viên Khách hàng'
    WHEN Code = 'KiemTra' THEN N'Kiểm tra viên'
    WHEN Code = 'KeHoach' THEN N'Kế hoạch'
    WHEN Code = 'QuanLyRuiRo' THEN N'Quản lý rủi ro'
    WHEN Code = 'TongHop' THEN N'Tổng hợp'
    WHEN Code = 'BaoVe' THEN N'Bảo vệ'
    WHEN Code = 'LaoCong' THEN N'Lao công'
    WHEN Code = 'TaiXe' THEN N'Tài xế'
    WHEN Code = 'BaoTri' THEN N'Bảo trì'
    WHEN Code = 'IT' THEN N'Công nghệ thông tin'
    WHEN Code = 'ThanhToan' THEN N'Thanh toán'
    WHEN Code = 'TinDung' THEN N'Tín dụng'
    WHEN Code = 'HuyDong' THEN N'Huy động'
    WHEN Code = 'DichVu' THEN N'Dịch vụ'
    WHEN Code = 'NgoaiHoi' THEN N'Ngoại hối'
    ELSE Name
END
WHERE Name LIKE '%?%';

PRINT 'Vietnamese encoding fixed successfully!';
GO
