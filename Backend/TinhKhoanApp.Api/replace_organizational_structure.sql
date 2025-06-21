-- =====================================================
-- ORGANIZATIONAL STRUCTURE REPLACEMENT SCRIPT
-- This script completely replaces all units/departments
-- with the new standardized structure
-- =====================================================

-- Disable foreign key constraints temporarily
EXEC sp_msforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT ALL"

-- Delete all existing units (cascading delete will handle related records)
DELETE FROM Units;

-- Reset identity seed to start from 1
DBCC CHECKIDENT ('Units', RESEED, 0);

-- Insert new organizational structure
-- ====================================================

-- Chi nhánh tỉnh Lai Châu (ID: 1)
INSERT INTO Units (Name, Code, Type, ParentUnitId) VALUES 
('Chi nhánh tỉnh Lai Châu', 'CnLaiChau', 'CNL1', NULL);

-- Units under Chi nhánh tỉnh Lai Châu
INSERT INTO Units (Name, Code, Type, ParentUnitId) VALUES 
('Ban Giám đốc', 'CnLaiChauBgd', 'PNVL1', 1),
('Phòng Khách hàng Doanh nghiệp', 'CnLaiChauKhdn', 'PNVL1', 1),
('Phòng Khách hàng Cá nhân', 'CnLaiChauKhcn', 'PNVL1', 1),
('Phòng Kế toán & Ngân quỹ', 'CnLaiChauKtnq', 'PNVL1', 1),
('Phòng Tổng hợp', 'CnLaiChauTonghop', 'PNVL1', 1),
('Phòng Kế hoạch & Quản lý rủi ro', 'CnLaiChauKhqlrr', 'PNVL1', 1),
('Phòng Kiểm tra giám sát', 'CnLaiChauKtgs', 'PNVL1', 1);

-- Chi nhánh Tam Đường
INSERT INTO Units (Name, Code, Type, ParentUnitId) VALUES 
('Chi nhánh Tam Đường', 'CnTamDuong', 'CNL2', 1),
('Ban Giám đốc', 'CnTamDuongBgd', 'PNVL2', 9),
('Phòng Kế toán & Ngân quỹ', 'CnTamDuongKtnq', 'PNVL2', 9),
('Phòng Khách hàng', 'CnTamDuongKh', 'PNVL2', 9);

-- Chi nhánh Phong Thổ
INSERT INTO Units (Name, Code, Type, ParentUnitId) VALUES 
('Chi nhánh Phong Thổ', 'CnPhongTho', 'CNL2', 1),
('Ban Giám đốc', 'CnPhongThoBgd', 'PNVL2', 13),
('Phòng Kế toán & Ngân quỹ', 'CnPhongThoKtnq', 'PNVL2', 13),
('Phòng Khách hàng', 'CnPhongThoKh', 'PNVL2', 13),
('Phòng giao dịch Mường So', 'CnPhongThoPgdMuongSo', 'PGDL2', 13);

-- Chi nhánh Sìn Hồ
INSERT INTO Units (Name, Code, Type, ParentUnitId) VALUES 
('Chi nhánh Sìn Hồ', 'CnSinHo', 'CNL2', 1),
('Ban Giám đốc', 'CnSinHoBgd', 'PNVL2', 18),
('Phòng Kế toán & Ngân quỹ', 'CnSinHoKtnq', 'PNVL2', 18),
('Phòng Khách hàng', 'CnSinHoKh', 'PNVL2', 18);

-- Chi nhánh Mường Tè
INSERT INTO Units (Name, Code, Type, ParentUnitId) VALUES 
('Chi nhánh Mường Tè', 'CnMuongTe', 'CNL2', 1),
('Ban Giám đốc', 'CnMuongTeBgd', 'PNVL2', 22),
('Phòng Kế toán & Ngân quỹ', 'CnMuongTeKtnq', 'PNVL2', 22),
('Phòng Khách hàng', 'CnMuongTeKh', 'PNVL2', 22);

-- Chi nhánh Than Uyên
INSERT INTO Units (Name, Code, Type, ParentUnitId) VALUES 
('Chi nhánh Than Uyên', 'CnThanUyen', 'CNL2', 1),
('Ban Giám đốc', 'CnThanUyenBgd', 'PNVL2', 26),
('Phòng Kế toán & Ngân quỹ', 'CnThanUyenKtnq', 'PNVL2', 26),
('Phòng Khách hàng', 'CnThanUyenKh', 'PNVL2', 26),
('Phòng giao dịch Mường Than', 'CnThanUyenPgdMuongThan', 'PGDL2', 26);

-- Chi nhánh Thành Phố
INSERT INTO Units (Name, Code, Type, ParentUnitId) VALUES 
('Chi nhánh Thành Phố', 'CnThanhPho', 'CNL2', 1),
('Ban Giám đốc', 'CnThanhPhoBgd', 'PNVL2', 31),
('Phòng Kế toán & Ngân quỹ', 'CnThanhPhoKtnq', 'PNVL2', 31),
('Phòng Khách hàng', 'CnThanhPhoKh', 'PNVL2', 31),
('Phòng giao dịch số 1', 'CnThanhPhoPgdso1', 'PGDL2', 31),
('Phòng giao dịch số 2', 'CnThanhPhoPgdso2', 'PGDL2', 31);

-- Chi nhánh Tân Uyên
INSERT INTO Units (Name, Code, Type, ParentUnitId) VALUES 
('Chi nhánh Tân Uyên', 'CnTanUyen', 'CNL2', 1),
('Ban Giám đốc', 'CnTanUyenBgd', 'PNVL2', 37),
('Phòng Kế toán & Ngân quỹ', 'CnTanUyenKtnq', 'PNVL2', 37),
('Phòng Khách hàng', 'CnTanUyenKh', 'PNVL2', 37),
('Phòng giao dịch số 3', 'CnTanUyenPgdso3', 'PGDL2', 37);

-- Chi nhánh Nậm Nhùn
INSERT INTO Units (Name, Code, Type, ParentUnitId) VALUES 
('Chi nhánh Nậm Nhùn', 'CnNamNhun', 'CNL2', 1),
('Ban Giám đốc', 'CnNamNhunBgd', 'PNVL2', 42),
('Phòng Kế toán & Ngân quỹ', 'CnNamNhunKtnq', 'PNVL2', 42),
('Phòng Khách hàng', 'CnNamNhunKh', 'PNVL2', 42);

-- Re-enable foreign key constraints
EXEC sp_msforeachtable "ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL"

-- Verify the new structure
SELECT 
    u.Id,
    u.Name,
    u.Code,
    u.Type,
    u.ParentUnitId,
    p.Name as ParentName
FROM Units u
LEFT JOIN Units p ON u.ParentUnitId = p.Id
ORDER BY u.Id;

-- Count by type
SELECT Type, COUNT(*) as Count
FROM Units
GROUP BY Type
ORDER BY Type;

PRINT 'Organizational structure replacement completed successfully!';
PRINT 'Total units created: 45';
PRINT 'Types: CNL1 (1), CNL2 (8), PNVL1 (7), PNVL2 (24), PGDL2 (5)';
