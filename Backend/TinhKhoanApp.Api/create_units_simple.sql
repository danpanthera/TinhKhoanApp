-- =============================================
-- Tạo cấu trúc đơn vị đơn giản - auto increment ID
-- Created: 2025-07-06
-- =============================================

-- 1. Chi nhánh Lai Châu (root)
INSERT INTO Units (Name, Code, Type, ParentUnitId) VALUES (N'Chi nhánh Lai Châu', 'CnLaiChau', 'CNL1', NULL);

-- 2. Hội Sở (child của Lai Châu)
INSERT INTO Units (Name, Code, Type, ParentUnitId) VALUES (N'Hội Sở', 'HoiSo', 'CNL1', 1);

-- 3. Các phòng ban Hội Sở (child của Hội Sở - ID 2)
INSERT INTO Units (Name, Code, Type, ParentUnitId) VALUES
(N'Ban Giám đốc', 'HoiSoBgd', 'PNVL1', 2),
(N'Phòng Khách hàng Doanh nghiệp', 'HoiSoKhdn', 'PNVL1', 2),
(N'Phòng Khách hàng Cá nhân', 'HoiSoKhcn', 'PNVL1', 2),
(N'Phòng Kế toán & Ngân quỹ', 'HoiSoKtnq', 'PNVL1', 2),
(N'Phòng Tổng hợp', 'HoiSoTonghop', 'PNVL1', 2),
(N'Phòng Kế hoạch & Quản lý rủi ro', 'HoiSoKhqlrr', 'PNVL1', 2),
(N'Phòng Kiểm tra giám sát', 'HoiSoKtgs', 'PNVL1', 2);

-- 4. Các chi nhánh cấp 2 (child của Lai Châu - ID 1)
INSERT INTO Units (Name, Code, Type, ParentUnitId) VALUES
(N'Chi nhánh Bình Lư', 'CnBinhLu', 'CNL2', 1),
(N'Chi nhánh Phong Thổ', 'CnPhongTho', 'CNL2', 1),
(N'Chi nhánh Sìn Hồ', 'CnSinHo', 'CNL2', 1),
(N'Chi nhánh Bum Tở', 'CnBumTo', 'CNL2', 1),
(N'Chi nhánh Than Uyên', 'CnThanUyen', 'CNL2', 1),
(N'Chi nhánh Đoàn Kết', 'CnDoanKet', 'CNL2', 1),
(N'Chi nhánh Tân Uyên', 'CnTanUyen', 'CNL2', 1),
(N'Chi nhánh Nậm Hàng', 'CnNamHang', 'CNL2', 1);

-- 5. Phòng ban Chi nhánh Bình Lư (ID sẽ là 11)
DECLARE @BinhLuId INT = (SELECT Id FROM Units WHERE Code = 'CnBinhLu');
INSERT INTO Units (Name, Code, Type, ParentUnitId) VALUES
(N'Ban Giám đốc', 'CnBinhLuBgd', 'PNVL2', @BinhLuId),
(N'Phòng Kế toán & Ngân quỹ', 'CnBinhLuKtnq', 'PNVL2', @BinhLuId),
(N'Phòng Khách hàng', 'CnBinhLuKh', 'PNVL2', @BinhLuId);

-- 6. Phòng ban Chi nhánh Phong Thổ (ID sẽ là 12)
DECLARE @PhongThoId INT = (SELECT Id FROM Units WHERE Code = 'CnPhongTho');
INSERT INTO Units (Name, Code, Type, ParentUnitId) VALUES
(N'Ban Giám đốc', 'CnPhongThoBgd', 'PNVL2', @PhongThoId),
(N'Phòng Kế toán & Ngân quỹ', 'CnPhongThoKtnq', 'PNVL2', @PhongThoId),
(N'Phòng Khách hàng', 'CnPhongThoKh', 'PNVL2', @PhongThoId),
(N'Phòng giao dịch Số 5', 'CnPhongThoPgdSo5', 'PGDL2', @PhongThoId);

-- 7. Phòng ban Chi nhánh Sìn Hồ (ID sẽ là 13)
DECLARE @SinHoId INT = (SELECT Id FROM Units WHERE Code = 'CnSinHo');
INSERT INTO Units (Name, Code, Type, ParentUnitId) VALUES
(N'Ban Giám đốc', 'CnSinHoBgd', 'PNVL2', @SinHoId),
(N'Phòng Kế toán & Ngân quỹ', 'CnSinHoKtnq', 'PNVL2', @SinHoId),
(N'Phòng Khách hàng', 'CnSinHoKh', 'PNVL2', @SinHoId);

-- 8. Phòng ban Chi nhánh Bum Tở (ID sẽ là 14)
DECLARE @BumToId INT = (SELECT Id FROM Units WHERE Code = 'CnBumTo');
INSERT INTO Units (Name, Code, Type, ParentUnitId) VALUES
(N'Ban Giám đốc', 'CnBumToBgd', 'PNVL2', @BumToId),
(N'Phòng Kế toán & Ngân quỹ', 'CnBumToKtnq', 'PNVL2', @BumToId),
(N'Phòng Khách hàng', 'CnBumToKh', 'PNVL2', @BumToId);

-- 9. Phòng ban Chi nhánh Than Uyên (ID sẽ là 15)
DECLARE @ThanUyenId INT = (SELECT Id FROM Units WHERE Code = 'CnThanUyen');
INSERT INTO Units (Name, Code, Type, ParentUnitId) VALUES
(N'Ban Giám đốc', 'CnThanUyenBgd', 'PNVL2', @ThanUyenId),
(N'Phòng Kế toán & Ngân quỹ', 'CnThanUyenKtnq', 'PNVL2', @ThanUyenId),
(N'Phòng Khách hàng', 'CnThanUyenKh', 'PNVL2', @ThanUyenId),
(N'Phòng giao dịch số 6', 'CnThanUyenPgdSo6', 'PGDL2', @ThanUyenId);

-- 10. Phòng ban Chi nhánh Đoàn Kết (ID sẽ là 16)
DECLARE @DoanKetId INT = (SELECT Id FROM Units WHERE Code = 'CnDoanKet');
INSERT INTO Units (Name, Code, Type, ParentUnitId) VALUES
(N'Ban Giám đốc', 'CnDoanKetBgd', 'PNVL2', @DoanKetId),
(N'Phòng Kế toán & Ngân quỹ', 'CnDoanKetKtnq', 'PNVL2', @DoanKetId),
(N'Phòng Khách hàng', 'CnDoanKetKh', 'PNVL2', @DoanKetId),
(N'Phòng giao dịch số 1', 'CnDoanKetPgdso1', 'PGDL2', @DoanKetId),
(N'Phòng giao dịch số 2', 'CnDoanKetPgdso2', 'PGDL2', @DoanKetId);

-- 11. Phòng ban Chi nhánh Tân Uyên (ID sẽ là 17)
DECLARE @TanUyenId INT = (SELECT Id FROM Units WHERE Code = 'CnTanUyen');
INSERT INTO Units (Name, Code, Type, ParentUnitId) VALUES
(N'Ban Giám đốc', 'CnTanUyenBgd', 'PNVL2', @TanUyenId),
(N'Phòng Kế toán & Ngân quỹ', 'CnTanUyenKtnq', 'PNVL2', @TanUyenId),
(N'Phòng Khách hàng', 'CnTanUyenKh', 'PNVL2', @TanUyenId),
(N'Phòng giao dịch số 3', 'CnTanUyenPgdso3', 'PGDL2', @TanUyenId);

-- 12. Phòng ban Chi nhánh Nậm Hàng (ID sẽ là 18)
DECLARE @NamHangId INT = (SELECT Id FROM Units WHERE Code = 'CnNamHang');
INSERT INTO Units (Name, Code, Type, ParentUnitId) VALUES
(N'Ban Giám đốc', 'CnNamHangBgd', 'PNVL2', @NamHangId),
(N'Phòng Kế toán & Ngân quỹ', 'CnNamHangKtnq', 'PNVL2', @NamHangId),
(N'Phòng Khách hàng', 'CnNamHangKh', 'PNVL2', @NamHangId);

-- Kiểm tra kết quả
SELECT COUNT(*) as TotalUnits FROM Units;
SELECT * FROM Units ORDER BY Id;
