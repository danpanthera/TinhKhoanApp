-- Add 6 business departments under CNL1 (Chi nhánh tỉnh Lai Châu)
-- These are PNVL1 type units with ParentUnitId = 10

INSERT INTO Units (Id, UnitCode, UnitName, UnitType, ParentUnitId, IsActive, CreatedDate) VALUES
(11, 'PhongKhdn', 'Phòng Khách hàng Doanh nghiệp', 'PNVL1', 10, 1, datetime('now')),
(12, 'PhongKhcn', 'Phòng Khách hàng Cá nhân', 'PNVL1', 10, 1, datetime('now')),
(13, 'PhongKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL1', 10, 1, datetime('now')),
(14, 'PhongKtgs', 'Phòng Kiểm tra giám sát', 'PNVL1', 10, 1, datetime('now')),
(15, 'PhongTh', 'Phòng Tổng hợp', 'PNVL1', 10, 1, datetime('now')),
(16, 'PhongKhqlrr', 'Phòng Kế hoạch & QLRR', 'PNVL1', 10, 1, datetime('now'));
