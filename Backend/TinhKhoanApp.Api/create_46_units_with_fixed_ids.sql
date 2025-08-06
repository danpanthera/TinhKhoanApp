-- Script tạo 46 đơn vị theo cấu trúc 3 cấp với ID cố định
-- Set proper options for temporal tables
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
USE TinhKhoanDB;
GO

-- Xóa hết dữ liệu cũ trước
-- Xóa hết employees trước (foreign key)
DELETE FROM Employees;

-- Xóa hết units
DELETE FROM Units;

-- Reset IDENTITY để bắt đầu từ 1
DBCC CHECKIDENT ('Units', RESEED, 0);

-- Bật IDENTITY_INSERT để có thể set ID cố định
SET IDENTITY_INSERT Units ON;

-- Tạo 46 đơn vị theo cấu trúc 3 cấp
INSERT INTO Units (Id, Name, Code, Type, ParentUnitId, IsDeleted) VALUES
-- Cấp 1: Chi nhánh Lai Châu (Root)
(1, N'Chi nhánh Lai Châu', 'CnLaiChau', 'CNL1', NULL, 0),

-- Cấp 2: Hội Sở và 8 Chi nhánh
(2, N'Hội Sở', 'HoiSo', 'CNL2', 1, 0),
(10, N'Chi nhánh Bình Lư', 'CnBinhLu', 'CNL2', 1, 0),
(14, N'Chi nhánh Phong Thổ', 'CnPhongTho', 'CNL2', 1, 0),
(19, N'Chi nhánh Sìn Hồ', 'CnSinHo', 'CNL2', 1, 0),
(23, N'Chi nhánh Bum Tở', 'CnBumTo', 'CNL2', 1, 0),
(27, N'Chi nhánh Than Uyên', 'CnThanUyen', 'CNL2', 1, 0),
(32, N'Chi nhánh Đoàn Kết', 'CnDoanKet', 'CNL2', 1, 0),
(38, N'Chi nhánh Tân Uyên', 'CnTanUyen', 'CNL2', 1, 0),
(43, N'Chi nhánh Nậm Hàng', 'CnNamHang', 'CNL2', 1, 0),

-- Cấp 3: Các phòng ban thuộc Hội Sở (PNVL1)
(3, N'Ban Giám đốc', 'HoiSoBgd', 'PNVL1', 2, 0),
(4, N'Phòng Khách hàng Doanh nghiệp', 'HoiSoKhdn', 'PNVL1', 2, 0),
(5, N'Phòng Khách hàng Cá nhân', 'HoiSoKhcn', 'PNVL1', 2, 0),
(6, N'Phòng Kế toán & Ngân quỹ', 'HoiSoKtnq', 'PNVL1', 2, 0),
(7, N'Phòng Tổng hợp', 'HoiSoTonghop', 'PNVL1', 2, 0),
(8, N'Phòng Kế hoạch & Quản lý rủi ro', 'HoiSoKhqlrr', 'PNVL1', 2, 0),
(9, N'Phòng Kiểm tra giám sát', 'HoiSoKtgs', 'PNVL1', 2, 0),

-- Cấp 3: Các phòng thuộc CN Bình Lư (PNVL2)
(11, N'Ban Giám đốc', 'CnBinhLuBgd', 'PNVL2', 10, 0),
(12, N'Phòng Kế toán & Ngân quỹ', 'CnBinhLuKtnq', 'PNVL2', 10, 0),
(13, N'Phòng Khách hàng', 'CnBinhLuKh', 'PNVL2', 10, 0),

-- Cấp 3: Các phòng thuộc CN Phong Thổ (PNVL2 + PGDL2)
(15, N'Ban Giám đốc', 'CnPhongThoBgd', 'PNVL2', 14, 0),
(16, N'Phòng Kế toán & Ngân quỹ', 'CnPhongThoKtnq', 'PNVL2', 14, 0),
(17, N'Phòng Khách hàng', 'CnPhongThoKh', 'PNVL2', 14, 0),
(18, N'Phòng giao dịch Số 5', 'CnPhongThoPgdSo5', 'PGDL2', 14, 0),

-- Cấp 3: Các phòng thuộc CN Sìn Hồ (PNVL2)
(20, N'Ban Giám đốc', 'CnSinHoBgd', 'PNVL2', 19, 0),
(21, N'Phòng Kế toán & Ngân quỹ', 'CnSinHoKtnq', 'PNVL2', 19, 0),
(22, N'Phòng Khách hàng', 'CnSinHoKh', 'PNVL2', 19, 0),

-- Cấp 3: Các phòng thuộc CN Bum Tở (PNVL2)
(24, N'Ban Giám đốc', 'CnBumToBgd', 'PNVL2', 23, 0),
(25, N'Phòng Kế toán & Ngân quỹ', 'CnBumToKtnq', 'PNVL2', 23, 0),
(26, N'Phòng Khách hàng', 'CnBumToKh', 'PNVL2', 23, 0),

-- Cấp 3: Các phòng thuộc CN Than Uyên (PNVL2 + PGDL2)
(28, N'Ban Giám đốc', 'CnThanUyenBgd', 'PNVL2', 27, 0),
(29, N'Phòng Kế toán & Ngân quỹ', 'CnThanUyenKtnq', 'PNVL2', 27, 0),
(30, N'Phòng Khách hàng', 'CnThanUyenKh', 'PNVL2', 27, 0),
(31, N'Phòng giao dịch số 6', 'CnThanUyenPgdSo6', 'PGDL2', 27, 0),

-- Cấp 3: Các phòng thuộc CN Đoàn Kết (PNVL2 + 2 PGDL2)
(33, N'Ban Giám đốc', 'CnDoanKetBgd', 'PNVL2', 32, 0),
(34, N'Phòng Kế toán & Ngân quỹ', 'CnDoanKetKtnq', 'PNVL2', 32, 0),
(35, N'Phòng Khách hàng', 'CnDoanKetKh', 'PNVL2', 32, 0),
(36, N'Phòng giao dịch số 1', 'CnDoanKetPgdso1', 'PGDL2', 32, 0),
(37, N'Phòng giao dịch số 2', 'CnDoanKetPgdso2', 'PGDL2', 32, 0),

-- Cấp 3: Các phòng thuộc CN Tân Uyên (PNVL2 + PGDL2)
(39, N'Ban Giám đốc', 'CnTanUyenBgd', 'PNVL2', 38, 0),
(40, N'Phòng Kế toán & Ngân quỹ', 'CnTanUyenKtnq', 'PNVL2', 38, 0),
(41, N'Phòng Khách hàng', 'CnTanUyenKh', 'PNVL2', 38, 0),
(42, N'Phòng giao dịch số 3', 'CnTanUyenPgdso3', 'PGDL2', 38, 0),

-- Cấp 3: Các phòng thuộc CN Nậm Hàng (PNVL2)
(44, N'Ban Giám đốc', 'CnNamHangBgd', 'PNVL2', 43, 0),
(45, N'Phòng Kế toán & Ngân quỹ', 'CnNamHangKtnq', 'PNVL2', 43, 0),
(46, N'Phòng Khách hàng', 'CnNamHangKh', 'PNVL2', 43, 0);

-- Tắt IDENTITY_INSERT
SET IDENTITY_INSERT Units OFF;

-- Kiểm tra kết quả
SELECT
    Type,
    COUNT(*) as [Count],
    CASE Type
        WHEN 'CNL1' THEN 'Chi nhánh cấp 1 (Root)'
        WHEN 'CNL2' THEN 'Chi nhánh cấp 2 (Hội sở + CN)'
        WHEN 'PNVL1' THEN 'Phòng Hội sở'
        WHEN 'PNVL2' THEN 'Phòng Chi nhánh'
        WHEN 'PGDL2' THEN 'Phòng giao dịch'
        ELSE 'Khác'
    END as [Description]
FROM Units
WHERE IsDeleted = 0
GROUP BY Type
ORDER BY Type;

-- Kiểm tra tổng số
SELECT COUNT(*) as [Total_Units] FROM Units WHERE IsDeleted = 0;

-- Hiển thị cấu trúc theo hierarchy
SELECT
    CASE
        WHEN ParentUnitId IS NULL THEN '🏢 ' + Name + ' (ROOT)'
        WHEN Type = 'CNL2' THEN '├── 🏛️ ' + Name + ' (CNL2)'
        WHEN Type = 'PNVL1' THEN '    ├── 📁 ' + Name + ' (PNVL1)'
        WHEN Type = 'PNVL2' THEN '    ├── 📋 ' + Name + ' (PNVL2)'
        WHEN Type = 'PGDL2' THEN '    └── 🏪 ' + Name + ' (PGDL2)'
        ELSE '    └── ' + Name
    END as [Hierarchy],
    Id, Code, Type, ParentUnitId
FROM Units
WHERE IsDeleted = 0
ORDER BY Id;

PRINT '✅ Đã tạo thành công 46 đơn vị với ID cố định theo cấu trúc 3 cấp!';
