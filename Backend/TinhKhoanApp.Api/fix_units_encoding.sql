-- Fix Units table Vietnamese encoding
USE TinhKhoanDB;
GO

UPDATE Units SET Name = N'Chi nhánh tỉnh Lai Châu' WHERE Code = 'CnLaiChau';
UPDATE Units SET Name = N'Hội Sở' WHERE Code = 'HoiSo';
UPDATE Units SET Name = N'Chi nhánh Bình Lu' WHERE Code = 'CnBinhLu';
UPDATE Units SET Name = N'Chi nhánh Phong Thổ' WHERE Code = 'CnPhongTho';
UPDATE Units SET Name = N'Chi nhánh Sìn Hồ' WHERE Code = 'CnSinHo';
UPDATE Units SET Name = N'Chi nhánh Bum Tở' WHERE Code = 'CnBumTo';
UPDATE Units SET Name = N'Chi nhánh Than Uyên' WHERE Code = 'CnThanUyen';
UPDATE Units SET Name = N'Chi nhánh Đoàn Kết' WHERE Code = 'CnDoanKet';
UPDATE Units SET Name = N'Chi nhánh Tân Uyên' WHERE Code = 'CnTanUyen';
UPDATE Units SET Name = N'Chi nhánh Nậm Hàng' WHERE Code = 'CnNamHang';
UPDATE Units SET Name = N'Ban Giám đốc' WHERE Code = 'HoiSoBgd';
UPDATE Units SET Name = N'Phòng Khách hàng Doanh nghiệp' WHERE Code = 'HoiSoKhdn';
UPDATE Units SET Name = N'Phòng Khách hàng Cá nhân' WHERE Code = 'HoiSoKhcn';
UPDATE Units SET Name = N'Phòng Kế toán & Ngân quỹ' WHERE Code = 'HoiSoKtnq';
UPDATE Units SET Name = N'Phòng Tổng hợp' WHERE Code = 'HoiSoTongHop';
UPDATE Units SET Name = N'Phòng Kế hoạch & Quản lý rủi ro' WHERE Code = 'HoiSoKhqlrr';
UPDATE Units SET Name = N'Phòng Kiểm tra giám sát' WHERE Code = 'HoiSoKtgs';

-- Fix Phòng Khách hàng
UPDATE Units SET Name = N'Phòng Khách hàng' WHERE Code LIKE '%Kh' AND Code NOT LIKE '%Khdn' AND Code NOT LIKE '%Khcn' AND Code NOT LIKE '%Khqlrr';

-- Fix Ban Giám đốc
UPDATE Units SET Name = N'Ban Giám đốc' WHERE Code LIKE '%Bgd';

-- Fix Phòng Kế toán & Ngân quỹ
UPDATE Units SET Name = N'Phòng Kế toán & Ngân quỹ' WHERE Code LIKE '%Ktnq';

-- Fix Phòng giao dịch
UPDATE Units SET Name = N'Phòng giao dịch Số 5' WHERE Code = 'CnPhongThoPgdSo5';
UPDATE Units SET Name = N'Phòng giao dịch Số 6' WHERE Code = 'CnThanUyenPgdSo6';
UPDATE Units SET Name = N'Phòng giao dịch số 1' WHERE Code = 'CnDoanKetPgdSo1';
UPDATE Units SET Name = N'Phòng giao dịch số 2' WHERE Code = 'CnDoanKetPgdSo2';
UPDATE Units SET Name = N'Phòng giao dịch số 3' WHERE Code = 'CnTanUyenPgdSo3';

PRINT 'Units table fixed successfully!';
GO
