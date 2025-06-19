-- Script tạo lại danh sách đơn vị theo cấu trúc mới
-- Xóa dữ liệu cũ và tạo lại

-- Chi nhánh tỉnh Lai Châu (gốc)
INSERT INTO Units (Id, UnitCode, UnitName, UnitType, ParentUnitId, IsActive, CreatedDate) VALUES
(10, 'CnLaiChau', 'Chi nhánh tỉnh Lai Châu', 'CNL1', NULL, 1, datetime('now'));

-- Chi nhánh huyện Tam Đường và các phòng thuộc
INSERT INTO Units (Id, UnitCode, UnitName, UnitType, ParentUnitId, IsActive, CreatedDate) VALUES
(20, 'CnTamDuong', 'Chi nhánh huyện Tam Đường', 'CNL2', 10, 1, datetime('now')),
(21, 'PhongKhachHang', 'Phòng Khách hàng', 'PNVL2', 20, 1, datetime('now')),
(22, 'PhongKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 20, 1, datetime('now'));

-- Chi nhánh huyện Phong Thổ và các phòng/PGD thuộc
INSERT INTO Units (Id, UnitCode, UnitName, UnitType, ParentUnitId, IsActive, CreatedDate) VALUES
(30, 'CnPhongTho', 'Chi nhánh huyện Phong Thổ', 'CNL2', 10, 1, datetime('now')),
(31, 'PgdMuongSo', 'Phòng giao dịch Mường So', 'PGDL2', 30, 1, datetime('now')),
(32, 'PhongKhachHang', 'Phòng Khách hàng', 'PNVL2', 30, 1, datetime('now')),
(33, 'PhongKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 30, 1, datetime('now'));

-- Chi nhánh huyện Sìn Hồ và các phòng thuộc
INSERT INTO Units (Id, UnitCode, UnitName, UnitType, ParentUnitId, IsActive, CreatedDate) VALUES
(40, 'CnSinHo', 'Chi nhánh huyện Sìn Hồ', 'CNL2', 10, 1, datetime('now')),
(41, 'PhongKhachHang', 'Phòng Khách hàng', 'PNVL2', 40, 1, datetime('now')),
(42, 'PhongKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 40, 1, datetime('now'));

-- Chi nhánh huyện Mường Tè và các phòng thuộc
INSERT INTO Units (Id, UnitCode, UnitName, UnitType, ParentUnitId, IsActive, CreatedDate) VALUES
(50, 'CnMuongTe', 'Chi nhánh huyện Mường Tè', 'CNL2', 10, 1, datetime('now')),
(51, 'PhongKhachHang', 'Phòng Khách hàng', 'PNVL2', 50, 1, datetime('now')),
(52, 'PhongKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 50, 1, datetime('now'));

-- Chi nhánh huyện Than Uyên và các phòng/PGD thuộc
INSERT INTO Units (Id, UnitCode, UnitName, UnitType, ParentUnitId, IsActive, CreatedDate) VALUES
(60, 'CnThanUyen', 'Chi nhánh huyện Than Uyên', 'CNL2', 10, 1, datetime('now')),
(61, 'PhongKhachHang', 'Phòng Khách hàng', 'PNVL2', 60, 1, datetime('now')),
(62, 'PhongKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 60, 1, datetime('now')),
(63, 'PgdMuongThan', 'Phòng giao dịch Mường Than', 'PGDL2', 60, 1, datetime('now'));

-- Chi nhánh Thành Phố và các phòng/PGD thuộc
INSERT INTO Units (Id, UnitCode, UnitName, UnitType, ParentUnitId, IsActive, CreatedDate) VALUES
(70, 'CnThanhPho', 'Chi nhánh Thành Phố', 'CNL2', 10, 1, datetime('now')),
(71, 'PhongKhachHang', 'Phòng Khách hàng', 'PNVL2', 70, 1, datetime('now')),
(72, 'PhongKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 70, 1, datetime('now')),
(73, 'PgdSo1', 'Phòng giao dịch số 1', 'PGDL2', 70, 1, datetime('now')),
(74, 'PgdSo2', 'Phòng giao dịch số 2', 'PGDL2', 70, 1, datetime('now'));

-- Chi nhánh huyện Tân Uyên và các phòng/PGD thuộc
INSERT INTO Units (Id, UnitCode, UnitName, UnitType, ParentUnitId, IsActive, CreatedDate) VALUES
(80, 'CnTanUyen', 'Chi nhánh huyện Tân Uyên', 'CNL2', 10, 1, datetime('now')),
(81, 'PhongKhachHang', 'Phòng Khách hàng', 'PNVL2', 80, 1, datetime('now')),
(82, 'PhongKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 80, 1, datetime('now')),
(83, 'PgdSo3', 'Phòng giao dịch số 3', 'PGDL2', 80, 1, datetime('now'));

-- Chi nhánh huyện Nậm Nhùn và các phòng thuộc
INSERT INTO Units (Id, UnitCode, UnitName, UnitType, ParentUnitId, IsActive, CreatedDate) VALUES
(90, 'CnNamNhun', 'Chi nhánh huyện Nậm Nhùn', 'CNL2', 10, 1, datetime('now')),
(91, 'PhongKhachHang', 'Phòng Khách hàng', 'PNVL2', 90, 1, datetime('now')),
(92, 'PhongKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 90, 1, datetime('now'));
