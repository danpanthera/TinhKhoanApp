-- =============================================
-- Tạo cấu trúc đơn vị mới cho TinhKhoanApp
-- Created: 2025-07-06
-- Purpose: Import 46 đơn vị theo cấu trúc hierarchical
-- =============================================

-- Reset identity nếu cần
DBCC CHECKIDENT ('Units', RESEED, 0);
PRINT 'Reset Units identity to 0';

-- Insert Units theo thứ tự hierarchical
-- 1. Chi nhánh Lai Châu (root)
INSERT INTO Units (Id, Name, Code, Type, ParentUnitId) VALUES
(1, N'Chi nhánh Lai Châu', 'CnLaiChau', 'CNL1', NULL);

-- 2. Hội Sở và các phòng ban trực thuộc
INSERT INTO Units (Id, Name, Code, Type, ParentUnitId) VALUES
(2, N'Hội Sở', 'HoiSo', 'CNL1', 1),
(3, N'Ban Giám đốc', 'HoiSoBgd', 'PNVL1', 2),
(4, N'Phòng Khách hàng Doanh nghiệp', 'HoiSoKhdn', 'PNVL1', 2),
(5, N'Phòng Khách hàng Cá nhân', 'HoiSoKhcn', 'PNVL1', 2),
(6, N'Phòng Kế toán & Ngân quỹ', 'HoiSoKtnq', 'PNVL1', 2),
(7, N'Phòng Tổng hợp', 'HoiSoTonghop', 'PNVL1', 2),
(8, N'Phòng Kế hoạch & Quản lý rủi ro', 'HoiSoKhqlrr', 'PNVL1', 2),
(9, N'Phòng Kiểm tra giám sát', 'HoiSoKtgs', 'PNVL1', 2);

-- 3. Chi nhánh Bình Lư và các phòng ban
INSERT INTO Units (Id, Name, Code, Type, ParentUnitId) VALUES
(10, N'Chi nhánh Bình Lư', 'CnBinhLu', 'CNL2', 1),
(11, N'Ban Giám đốc', 'CnBinhLuBgd', 'PNVL2', 10),
(12, N'Phòng Kế toán & Ngân quỹ', 'CnBinhLuKtnq', 'PNVL2', 10),
(13, N'Phòng Khách hàng', 'CnBinhLuKh', 'PNVL2', 10);

-- 4. Chi nhánh Phong Thổ và các phòng ban
INSERT INTO Units (Id, Name, Code, Type, ParentUnitId) VALUES
(14, N'Chi nhánh Phong Thổ', 'CnPhongTho', 'CNL2', 1),
(15, N'Ban Giám đốc', 'CnPhongThoBgd', 'PNVL2', 14),
(16, N'Phòng Kế toán & Ngân quỹ', 'CnPhongThoKtnq', 'PNVL2', 14),
(17, N'Phòng Khách hàng', 'CnPhongThoKh', 'PNVL2', 14),
(18, N'Phòng giao dịch Số 5', 'CnPhongThoPgdSo5', 'PGDL2', 14);

-- 5. Chi nhánh Sìn Hồ và các phòng ban
INSERT INTO Units (Id, Name, Code, Type, ParentUnitId) VALUES
(19, N'Chi nhánh Sìn Hồ', 'CnSinHo', 'CNL2', 1),
(20, N'Ban Giám đốc', 'CnSinHoBgd', 'PNVL2', 19),
(21, N'Phòng Kế toán & Ngân quỹ', 'CnSinHoKtnq', 'PNVL2', 19),
(22, N'Phòng Khách hàng', 'CnSinHoKh', 'PNVL2', 19);

-- 6. Chi nhánh Bum Tở và các phòng ban
INSERT INTO Units (Id, Name, Code, Type, ParentUnitId) VALUES
(23, N'Chi nhánh Bum Tở', 'CnBumTo', 'CNL2', 1),
(24, N'Ban Giám đốc', 'CnBumToBgd', 'PNVL2', 23),
(25, N'Phòng Kế toán & Ngân quỹ', 'CnBumToKtnq', 'PNVL2', 23),
(26, N'Phòng Khách hàng', 'CnBumToKh', 'PNVL2', 23);

-- 7. Chi nhánh Than Uyên và các phòng ban
INSERT INTO Units (Id, Name, Code, Type, ParentUnitId) VALUES
(27, N'Chi nhánh Than Uyên', 'CnThanUyen', 'CNL2', 1),
(28, N'Ban Giám đốc', 'CnThanUyenBgd', 'PNVL2', 27),
(29, N'Phòng Kế toán & Ngân quỹ', 'CnThanUyenKtnq', 'PNVL2', 27),
(30, N'Phòng Khách hàng', 'CnThanUyenKh', 'PNVL2', 27),
(31, N'Phòng giao dịch số 6', 'CnThanUyenPgdSo6', 'PGDL2', 27);

-- 8. Chi nhánh Đoàn Kết và các phòng ban
INSERT INTO Units (Id, Name, Code, Type, ParentUnitId) VALUES
(32, N'Chi nhánh Đoàn Kết', 'CnDoanKet', 'CNL2', 1),
(33, N'Ban Giám đốc', 'CnDoanKetBgd', 'PNVL2', 32),
(34, N'Phòng Kế toán & Ngân quỹ', 'CnDoanKetKtnq', 'PNVL2', 32),
(35, N'Phòng Khách hàng', 'CnDoanKetKh', 'PNVL2', 32),
(36, N'Phòng giao dịch số 1', 'CnDoanKetPgdso1', 'PGDL2', 32),
(37, N'Phòng giao dịch số 2', 'CnDoanKetPgdso2', 'PGDL2', 32);

-- 9. Chi nhánh Tân Uyên và các phòng ban
INSERT INTO Units (Id, Name, Code, Type, ParentUnitId) VALUES
(38, N'Chi nhánh Tân Uyên', 'CnTanUyen', 'CNL2', 1),
(39, N'Ban Giám đốc', 'CnTanUyenBgd', 'PNVL2', 38),
(40, N'Phòng Kế toán & Ngân quỹ', 'CnTanUyenKtnq', 'PNVL2', 38),
(41, N'Phòng Khách hàng', 'CnTanUyenKh', 'PNVL2', 38),
(42, N'Phòng giao dịch số 3', 'CnTanUyenPgdso3', 'PGDL2', 38);

-- 10. Chi nhánh Nậm Hàng và các phòng ban
INSERT INTO Units (Id, Name, Code, Type, ParentUnitId) VALUES
(43, N'Chi nhánh Nậm Hàng', 'CnNamHang', 'CNL2', 1),
(44, N'Ban Giám đốc', 'CnNamHangBgd', 'PNVL2', 43),
(45, N'Phòng Kế toán & Ngân quỹ', 'CnNamHangKtnq', 'PNVL2', 43),
(46, N'Phòng Khách hàng', 'CnNamHangKh', 'PNVL2', 43);

-- Kiểm tra kết quả
SELECT COUNT(*) as TotalUnits FROM Units;
PRINT 'Total Units created: 46';

-- Hiển thị cấu trúc hierarchical
SELECT
    u.Id,
    u.Name,
    u.Code,
    u.Type,
    u.ParentUnitId,
    p.Name as ParentUnitName
FROM Units u
LEFT JOIN Units p ON u.ParentUnitId = p.Id
ORDER BY u.Id;

PRINT 'Units structure created successfully!';
