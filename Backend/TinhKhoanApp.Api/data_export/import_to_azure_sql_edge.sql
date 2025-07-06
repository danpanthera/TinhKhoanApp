-- =============================================================
-- IMPORT DỮ LIỆU VÀO AZURE SQL EDGE - COMPATIBLE VERSION
-- Source: SQL Server Docker (TinhKhoanDB_Restored)
-- Target: Azure SQL Edge (TinhKhoanDB)
-- =============================================================

USE TinhKhoanDB;
GO

PRINT '🚀 BẮT ĐẦU IMPORT DỮ LIỆU VÀO AZURE SQL EDGE';
PRINT '============================================';
PRINT '';

-- =============================================================
-- 1. IMPORT UNITS (46 đơn vị)
-- =============================================================
PRINT '📥 1. IMPORTING UNITS (46 đơn vị)...';

-- Xóa dữ liệu cũ nếu có
DELETE FROM Units WHERE Id BETWEEN 1 AND 46;

-- Import Units data (compatible với cấu trúc Azure SQL Edge)
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (1, 'CnLaiChau', 'Chi nhánh tỉnh Lai Châu', 'CNL1', NULL, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (2, 'HoiSoBgd', 'Ban Giám đốc', 'PNVL2', 46, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (3, 'HoiSoKhdn', 'Phòng Khách hàng Doanh nghiệp', 'PNVL1', 46, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (4, 'HoiSoKhcn', 'Phòng Khách hàng Cá nhân', 'PNVL1', 46, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (5, 'HoiSoKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL1', 46, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (6, 'HoiSoTongHop', 'Phòng Tổng hợp', 'PNVL1', 46, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (7, 'HoiSoKhqlrr', 'Phòng Kế hoạch & Quản lý rủi ro', 'PNVL1', 46, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (8, 'HoiSoKtgs', 'Phòng Kiểm tra giám sát', 'PNVL1', 46, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (9, 'CnBinhLu', 'Chi nhánh Bình Lư', 'CNL2', 1, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (10, 'CnBinhLuBgd', 'Ban Giám đốc', 'PNVL2', 9, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (11, 'CnBinhLuKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 9, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (12, 'CnBinhLuKh', 'Phòng Khách hàng', 'PNVL2', 9, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (13, 'CnPhongTho', 'Chi nhánh Phong Thổ', 'CNL2', 1, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (14, 'CnPhongThoBgd', 'Ban Giám đốc', 'PNVL2', 13, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (15, 'CnPhongThoKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 13, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (16, 'CnPhongThoKh', 'Phòng Khách hàng', 'PNVL2', 13, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (17, 'CnPhongThoPgdSo5', 'Phòng giao dịch Số 5', 'PGDL2', 13, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (18, 'CnSinHo', 'Chi nhánh Sìn Hồ', 'CNL2', 1, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (19, 'CnSinHoBgd', 'Ban Giám đốc', 'PNVL2', 18, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (20, 'CnSinHoKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 18, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (21, 'CnSinHoKh', 'Phòng Khách hàng', 'PNVL2', 18, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (22, 'CnBumTo', 'Chi nhánh Bum Tở', 'CNL2', 1, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (23, 'CnBumToBgd', 'Ban Giám đốc', 'PNVL2', 22, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (24, 'CnBumToKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 22, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (25, 'CnBumToKh', 'Phòng Khách hàng', 'PNVL2', 22, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (26, 'CnThanUyen', 'Chi nhánh Than Uyên', 'CNL2', 1, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (27, 'CnThanUyenBgd', 'Ban Giám đốc', 'PNVL2', 26, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (28, 'CnThanUyenKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 26, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (29, 'CnThanUyenKh', 'Phòng Khách hàng', 'PNVL2', 26, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (30, 'CnThanUyenPgdSo6', 'Phòng giao dịch Số 6', 'PGDL2', 26, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (31, 'CnDoanKet', 'Chi nhánh Đoàn Kết', 'CNL2', 1, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (32, 'CnDoanKetBgd', 'Ban Giám đốc', 'PNVL2', 31, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (33, 'CnDoanKetKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 31, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (34, 'CnDoanKetKh', 'Phòng Khách hàng', 'PNVL2', 31, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (35, 'CnDoanKetPgdSo1', 'Phòng giao dịch số 1', 'PGDL2', 31, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (36, 'CnDoanKetPgdSo2', 'Phòng giao dịch số 2', 'PGDL2', 31, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (37, 'CnTanUyen', 'Chi nhánh Tân Uyên', 'CNL2', 1, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (38, 'CnTanUyenBgd', 'Ban Giám đốc', 'PNVL2', 37, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (39, 'CnTanUyenKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 37, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (40, 'CnTanUyenKh', 'Phòng Khách hàng', 'PNVL2', 37, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (41, 'CnTanUyenPgdSo3', 'Phòng giao dịch số 3', 'PGDL2', 37, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (42, 'CnNamHang', 'Chi nhánh Nậm Hàng', 'CNL2', 1, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (43, 'CnNamHangBgd', 'Ban Giám đốc', 'PNVL2', 42, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (44, 'CnNamHangKtnq', 'Phòng Kế toán & Ngân quỹ', 'PNVL2', 42, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (45, 'CnNamHangKh', 'Phòng Khách hàng', 'PNVL2', 42, 0);
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES (46, 'HoiSo', 'Hội Sở', 'CNL2', 1, 0);

SELECT 'Successfully imported ' + CAST(COUNT(*) AS VARCHAR) + ' Units records.' FROM Units WHERE IsDeleted = 0;
PRINT '';

-- =============================================================
-- 2. IMPORT POSITIONS (7 chức vụ)
-- =============================================================
PRINT '📥 2. IMPORTING POSITIONS (7 chức vụ)...';

-- Xóa dữ liệu cũ nếu có
DELETE FROM Positions WHERE Id BETWEEN 1 AND 7;

-- Import Positions data
INSERT INTO Positions (Id, Name, Description) VALUES (1, 'Giám đốc', 'Giám đốc công ty');
INSERT INTO Positions (Id, Name, Description) VALUES (2, 'Phó Giám đốc', 'Phó Giám đốc công ty');
INSERT INTO Positions (Id, Name, Description) VALUES (3, 'Trưởng phòng', 'Trưởng phòng ban');
INSERT INTO Positions (Id, Name, Description) VALUES (4, 'Phó trưởng phòng', 'Phó trưởng phòng ban');
INSERT INTO Positions (Id, Name, Description) VALUES (5, 'Giám đốc Phòng giao dịch', 'Giám đốc Phòng giao dịch');
INSERT INTO Positions (Id, Name, Description) VALUES (6, 'Phó giám đốc Phòng giao dịch', 'Phó giám đốc Phòng giao dịch');
INSERT INTO Positions (Id, Name, Description) VALUES (7, 'Nhân viên', 'Nhân viên');

SELECT 'Successfully imported ' + CAST(COUNT(*) AS VARCHAR) + ' Positions records.' FROM Positions;
PRINT '';

PRINT '✅ IMPORT HOÀN THÀNH!';
PRINT 'Dữ liệu đã được import thành công vào Azure SQL Edge.';

GO
