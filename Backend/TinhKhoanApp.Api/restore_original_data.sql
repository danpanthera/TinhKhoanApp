-- ============================================
-- 🔄 SCRIPT PHỤC HỒI DỮ LIỆU GỐC HỆ THỐNG
-- Phục hồi Đơn vị, Chức vụ và Nhân viên gốc
-- ============================================

-- 1. PHỤC HỒI ĐƠN VỊ AGRIBANK LAI CHÂU (45 đơn vị)
BEGIN TRANSACTION;

-- Xóa dữ liệu cũ
DELETE FROM Units;
DBCC CHECKIDENT('Units', RESEED, 0);

-- Thêm dữ liệu đơn vị gốc
SET IDENTITY_INSERT Units ON;

INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(1, 'CnLaiChau', N'Chi nhánh tỉnh Lai Châu', 'CNL1', NULL, 0),
(2, 'CnLaiChauBgd', N'Ban Giám đốc', 'PNVL1', 1, 0),
(3, 'CnLaiChauKhdn', N'Phòng Khách hàng Doanh nghiệp', 'PNVL1', 1, 0),
(4, 'CnLaiChauKhcn', N'Phòng Khách hàng Cá nhân', 'PNVL1', 1, 0),
(5, 'CnLaiChauKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL1', 1, 0),
(6, 'CnLaiChauTonghop', N'Phòng Tổng hợp', 'PNVL1', 1, 0),
(7, 'CnLaiChauKhqlrr', N'Phòng Kế hoạch & Quản lý rủi ro', 'PNVL1', 1, 0),
(8, 'CnLaiChauKtgs', N'Phòng Kiểm tra giám sát', 'PNVL1', 1, 0),
(9, 'CnBinhLu', N'Chi nhánh Bình Lư', 'CNL2', 1, 0),
(10, 'CnBinhLuBgd', N'Ban Giám đốc', 'PNVL2', 9, 0),
(11, 'CnBinhLuKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 9, 0),
(12, 'CnBinhLuKh', N'Phòng Khách hàng', 'PNVL2', 9, 0),
(13, 'CnPhongTho', N'Chi nhánh Phong Thổ', 'CNL2', 1, 0),
(14, 'CnPhongThoBgd', N'Ban Giám đốc', 'PNVL2', 13, 0),
(15, 'CnPhongThoKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 13, 0),
(16, 'CnPhongThoKh', N'Phòng Khách hàng', 'PNVL2', 13, 0),
(17, 'CnPhongThoPgdSo5', N'Phòng giao dịch Số 5', 'PGDL2', 13, 0),
(18, 'CnSinHo', N'Chi nhánh Sìn Hồ', 'CNL2', 1, 0),
(19, 'CnSinHoBgd', N'Ban Giám đốc', 'PNVL2', 18, 0),
(20, 'CnSinHoKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 18, 0),
(21, 'CnSinHoKh', N'Phòng Khách hàng', 'PNVL2', 18, 0),
(22, 'CnBumTo', N'Chi nhánh Bum Tở', 'CNL2', 1, 0),
(23, 'CnBumToBgd', N'Ban Giám đốc', 'PNVL2', 22, 0),
(24, 'CnBumToKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 22, 0),
(25, 'CnBumToKh', N'Phòng Khách hàng', 'PNVL2', 22, 0),
(26, 'CnThanUyen', N'Chi nhánh Than Uyên', 'CNL2', 1, 0),
(27, 'CnThanUyenBgd', N'Ban Giám đốc', 'PNVL2', 26, 0),
(28, 'CnThanUyenKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 26, 0),
(29, 'CnThanUyenKh', N'Phòng Khách hàng', 'PNVL2', 26, 0),
(30, 'CnThanUyenPgdSo6', N'Phòng giao dịch Số 6', 'PGDL2', 26, 0),
(31, 'CnDoanKet', N'Chi nhánh Đoàn Kết', 'CNL2', 1, 0),
(32, 'CnDoanKetBgd', N'Ban Giám đốc', 'PNVL2', 31, 0),
(33, 'CnDoanKetKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 31, 0),
(34, 'CnDoanKetKh', N'Phòng Khách hàng', 'PNVL2', 31, 0),
(35, 'CnDoanKetPgdSo1', N'Phòng giao dịch số 1', 'PGDL2', 31, 0),
(36, 'CnDoanKetPgdSo2', N'Phòng giao dịch số 2', 'PGDL2', 31, 0),
(37, 'CnTanUyen', N'Chi nhánh Tân Uyên', 'CNL2', 1, 0),
(38, 'CnTanUyenBgd', N'Ban Giám đốc', 'PNVL2', 37, 0),
(39, 'CnTanUyenKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 37, 0),
(40, 'CnTanUyenKh', N'Phòng Khách hàng', 'PNVL2', 37, 0),
(41, 'CnTanUyenPgdSo3', N'Phòng giao dịch số 3', 'PGDL2', 37, 0),
(42, 'CnNamHang', N'Chi nhánh Nậm Hàng', 'CNL2', 1, 0),
(43, 'CnNamHangBgd', N'Ban Giám đốc', 'PNVL2', 42, 0),
(44, 'CnNamHangKtnq', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 42, 0),
(45, 'CnNamHangKh', N'Phòng Khách hàng', 'PNVL2', 42, 0);

SET IDENTITY_INSERT Units OFF;

PRINT '✅ Đã phục hồi 45 đơn vị Agribank Lai Châu';

-- 2. PHỤC HỒI CHỨC VỤ (7 chức vụ chuẩn)
DELETE FROM Positions;
DBCC CHECKIDENT('Positions', RESEED, 0);

INSERT INTO Positions (Name, Description) VALUES
(N'Giám đốc', N'Giám đốc công ty'),
(N'Phó Giám đốc', N'Phó Giám đốc công ty'),
(N'Trưởng phòng', N'Trưởng phòng ban'),
(N'Phó trưởng phòng', N'Phó trưởng phòng ban'),
(N'Giám đốc Phòng giao dịch', N'Giám đốc Phòng giao dịch'),
(N'Phó giám đốc Phòng giao dịch', N'Phó giám đốc Phòng giao dịch'),
(N'Nhân viên', N'Nhân viên');

PRINT '✅ Đã phục hồi 7 chức vụ chuẩn';

-- 3. TẠO ADMIN USER MẪU
DELETE FROM Employees WHERE Username = 'admin';

INSERT INTO Employees (
    EmployeeCode,
    CBCode,
    FullName,
    Username,
    PasswordHash,
    Email,
    PhoneNumber,
    IsActive,
    UnitId,
    PositionId,
    CreatedAt,
    UpdatedAt
) VALUES (
    'ADMIN001',
    '999999999',
    N'Quản trị viên hệ thống',
    'admin',
    '$2a$11$8Z7QZ9Z7QZ9Z7QZ9Z7QZ9OeKQFQFQFQFQFQFQFQFQFQFQFQFQFQFQF',
    'admin@agribank.com.vn',
    '0999999999',
    1,
    1, -- Chi nhánh tỉnh Lai Châu
    1, -- Giám đốc
    GETDATE(),
    GETDATE()
);

-- Tạo thêm vài nhân viên mẫu
INSERT INTO Employees (
    EmployeeCode,
    CBCode,
    FullName,
    Username,
    PasswordHash,
    Email,
    PhoneNumber,
    IsActive,
    UnitId,
    PositionId,
    CreatedAt,
    UpdatedAt
) VALUES
('LC001', '78001001', N'Nguyễn Văn An', 'nvan', 'password_hash', 'nvan@agribank.com.vn', '0987654321', 1, 2, 2, GETDATE(), GETDATE()),
('LC002', '78001002', N'Trần Thị Bình', 'ttbinh', 'password_hash', 'ttbinh@agribank.com.vn', '0987654322', 1, 3, 3, GETDATE(), GETDATE()),
('LC003', '78001003', N'Lê Văn Cường', 'lvcuong', 'password_hash', 'lvcuong@agribank.com.vn', '0987654323', 1, 4, 3, GETDATE(), GETDATE()),
('LC004', '78001004', N'Phạm Thị Dung', 'ptdung', 'password_hash', 'ptdung@agribank.com.vn', '0987654324', 1, 5, 4, GETDATE(), GETDATE()),
('LC005', '78001005', N'Hoàng Văn Em', 'hvem', 'password_hash', 'hvem@agribank.com.vn', '0987654325', 1, 6, 7, GETDATE(), GETDATE());

PRINT '✅ Đã tạo admin user và 5 nhân viên mẫu';

COMMIT TRANSACTION;

-- Kiểm tra kết quả
SELECT 'Units' as TableName, COUNT(*) as RecordCount FROM Units WHERE IsDeleted = 0
UNION ALL
SELECT 'Positions' as TableName, COUNT(*) as RecordCount FROM Positions
UNION ALL
SELECT 'Employees' as TableName, COUNT(*) as RecordCount FROM Employees WHERE IsActive = 1;

PRINT '🎉 HOÀN THÀNH PHỤC HỒI DỮ LIỆU GỐC!';
