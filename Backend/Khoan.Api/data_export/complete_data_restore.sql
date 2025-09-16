-- =============================================================
-- PHỤC HỒI HOÀN CHỈNH DỮ LIỆU THỰC TỪ BACKUP
-- 46 Units + 23 Roles + KPI Definitions
-- =============================================================

USE TinhKhoanDB;
GO

PRINT '🚀 BẮT ĐẦU PHỤC HỒI DỮ LIỆU THỰC TỪ BACKUP';
PRINT '==========================================';
PRINT '';

-- =============================================================
-- 1. XÓA DỮ LIỆU CŨ VÀ PHỤC HỒI 46 UNITS HOÀN CHỈNH
-- =============================================================
PRINT '📥 1. PHỤC HỒI 46 UNITS THEO PHÂN CẤP...';

-- Tạm thời disable foreign key constraints
SET QUOTED_IDENTIFIER ON;
EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';

-- Xóa tất cả Units hiện tại
DELETE FROM Units;

-- Reset identity
DBCC CHECKIDENT ('Units', RESEED, 0);

SET IDENTITY_INSERT Units ON;

-- Import toàn bộ 46 Units theo đúng thứ tự phân cấp
-- Root level
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (1, 'CnLaiChau', 'Chi nhánh tỉnh Lai Châu', 'CNL1', NULL, 0);

-- Level 1 - Chi nhánh cấp 2 và Hội Sở
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (46, 'HoiSo', 'Hội Sở', 'CNL2', 1, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (9, 'CnBinhLu', 'Chi nhánh Bình Lư', 'CNL2', 1, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (13, 'CnPhongTho', 'Chi nhánh Phong Thổ', 'CNL2', 1, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (18, 'CnSinHo', 'Chi nhánh Sìn Hồ', 'CNL2', 1, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (22, 'CnBumTo', 'Chi nhánh Bum Tở', 'CNL2', 1, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (26, 'CnThanUyen', 'Chi nhánh Than Uyên', 'CNL2', 1, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (31, 'CnDoanKet', 'Chi nhánh Đoàn Kết', 'CNL2', 1, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (37, 'CnTanUyen', 'Chi nhánh Tân Uyên', 'CNL2', 1, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (42, 'CnNamHang', 'Chi nhánh Nậm Hàng', 'CNL2', 1, 0);

-- Level 2 - Phòng ban Hội Sở
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (2, 'HoiSoBgd', 'Ban Giám đốc', 'PNVL2', 46, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (3, 'HoiSoKhdn', 'Phòng Khách hàng Doanh nghiệp', 'PNVL1', 46, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (4, 'HoiSoKhcn', 'Phòng Khách hàng Cá nhân', 'PNVL1', 46, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (5, 'HoiSoKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL1', 46, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (6, 'HoiSoTongHop', 'Phòng Tổng hợp', 'PNVL1', 46, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (7, 'HoiSoKhqlrr', 'Phòng Kế hoạch & Quản lý rủi ro', 'PNVL1', 46, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (8, 'HoiSoKtgs', 'Phòng Kiểm tra giám sát', 'PNVL1', 46, 0);

-- Level 2 - Phòng ban chi nhánh Bình Lư
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (10, 'CnBinhLuBgd', 'Ban Giám đốc', 'PNVL2', 9, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (11, 'CnBinhLuKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 9, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (12, 'CnBinhLuKh', 'Phòng Khách hàng', 'PNVL2', 9, 0);

-- Level 2 - Phòng ban chi nhánh Phong Thổ
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (14, 'CnPhongThoBgd', 'Ban Giám đốc', 'PNVL2', 13, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (15, 'CnPhongThoKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 13, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (16, 'CnPhongThoKh', 'Phòng Khách hàng', 'PNVL2', 13, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (17, 'CnPhongThoPgdSo5', 'Phòng giao dịch Số 5', 'PGDL2', 13, 0);

-- Level 2 - Phòng ban chi nhánh Sìn Hồ
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (19, 'CnSinHoBgd', 'Ban Giám đốc', 'PNVL2', 18, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (20, 'CnSinHoKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 18, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (21, 'CnSinHoKh', 'Phòng Khách hàng', 'PNVL2', 18, 0);

-- Level 2 - Phòng ban chi nhánh Bum Tở
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (23, 'CnBumToBgd', 'Ban Giám đốc', 'PNVL2', 22, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (24, 'CnBumToKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 22, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (25, 'CnBumToKh', 'Phòng Khách hàng', 'PNVL2', 22, 0);

-- Level 2 - Phòng ban chi nhánh Than Uyên
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (27, 'CnThanUyenBgd', 'Ban Giám đốc', 'PNVL2', 26, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (28, 'CnThanUyenKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 26, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (29, 'CnThanUyenKh', 'Phòng Khách hàng', 'PNVL2', 26, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (30, 'CnThanUyenPgdSo6', 'Phòng giao dịch Số 6', 'PGDL2', 26, 0);

-- Level 2 - Phòng ban chi nhánh Đoàn Kết
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (32, 'CnDoanKetBgd', 'Ban Giám đốc', 'PNVL2', 31, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (33, 'CnDoanKetKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 31, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (34, 'CnDoanKetKh', 'Phòng Khách hàng', 'PNVL2', 31, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (35, 'CnDoanKetPgdSo1', 'Phòng giao dịch số 1', 'PGDL2', 31, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (36, 'CnDoanKetPgdSo2', 'Phòng giao dịch số 2', 'PGDL2', 31, 0);

-- Level 2 - Phòng ban chi nhánh Tân Uyên
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (38, 'CnTanUyenBgd', 'Ban Giám đốc', 'PNVL2', 37, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (39, 'CnTanUyenKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 37, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (40, 'CnTanUyenKh', 'Phòng Khách hàng', 'PNVL2', 37, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (41, 'CnTanUyenPgdSo3', 'Phòng giao dịch số 3', 'PGDL2', 37, 0);

-- Level 2 - Phòng ban chi nhánh Nậm Hàng
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (43, 'CnNamHangBgd', 'Ban Giám đốc', 'PNVL2', 42, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (44, 'CnNamHangKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 42, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (45, 'CnNamHangKh', 'Phòng Khách hàng', 'PNVL2', 42, 0);

SET IDENTITY_INSERT Units OFF;

-- Enable lại foreign key constraints
EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';

SELECT 'Successfully imported ' + CAST(COUNT(*) AS VARCHAR) + ' Units!' as Result FROM Units WHERE IsDeleted = 0;
PRINT '';

GO
