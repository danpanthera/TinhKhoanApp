-- Script to restore all 46 units from codebase
-- Clearing existing data and resetting identity
USE TinhKhoanDB;
GO

-- Clear existing units
DELETE FROM Units;
DBCC CHECKIDENT ('Units', RESEED, 0);
GO

-- Insert all units from SeedController.cs
-- Root unit
INSERT INTO Units (Code, Name, Type, ParentUnitId, IsDeleted) VALUES
('CNL1', N'Chi nhánh Lai Châu', 'CNL1', NULL, 0);

-- Get the root unit ID for foreign key references
DECLARE @RootId INT = SCOPE_IDENTITY();

-- Hội Sở
INSERT INTO Units (Code, Name, Type, ParentUnitId, IsDeleted) VALUES
('HOISO', N'Hội Sở', 'CNL1', @RootId, 0);

DECLARE @HoiSoId INT = SCOPE_IDENTITY();

-- Departments under Hội Sở
INSERT INTO Units (Code, Name, Type, ParentUnitId, IsDeleted) VALUES
('BGD', N'Ban Giám đốc', 'PNVL1', @HoiSoId, 0),
('PKHDN', N'Phòng Khách hàng Doanh nghiệp', 'PNVL1', @HoiSoId, 0),
('PKHCN', N'Phòng Khách hàng Cá nhân', 'PNVL1', @HoiSoId, 0),
('PKTNQ', N'Phòng Kế toán & Ngân quỹ', 'PNVL1', @HoiSoId, 0),
('PTH', N'Phòng Tổng hợp', 'PNVL1', @HoiSoId, 0),
('PKHQLRR', N'Phòng Kế hoạch & Quản lý rủi ro', 'PNVL1', @HoiSoId, 0),
('PKTGS', N'Phòng Kiểm tra giám sát', 'PNVL1', @HoiSoId, 0);

-- Chi nhánh Bình Lư
INSERT INTO Units (Code, Name, Type, ParentUnitId, IsDeleted) VALUES
('CNBL', N'Chi nhánh Bình Lư', 'CNL2', @RootId, 0);

DECLARE @CNBLId INT = SCOPE_IDENTITY();

INSERT INTO Units (Code, Name, Type, ParentUnitId, IsDeleted) VALUES
('BGDCNBL', N'Ban Giám đốc', 'PNVL2', @CNBLId, 0),
('PKTNQCNBL', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', @CNBLId, 0),
('PKHCNBL', N'Phòng Khách hàng', 'PNVL2', @CNBLId, 0);

-- Chi nhánh Phong Thổ
INSERT INTO Units (Code, Name, Type, ParentUnitId, IsDeleted) VALUES
('CNPT', N'Chi nhánh Phong Thổ', 'CNL2', @RootId, 0);

DECLARE @CNPTId INT = SCOPE_IDENTITY();

INSERT INTO Units (Code, Name, Type, ParentUnitId, IsDeleted) VALUES
('BGDCNPT', N'Ban Giám đốc', 'PNVL2', @CNPTId, 0),
('PKTNQCNPT', N'Phòng KT&NQ', 'PNVL2', @CNPTId, 0),
('PKHCNPT', N'Phòng KH', 'PNVL2', @CNPTId, 0),
('PGD5CNPT', N'Phòng giao dịch Số 5', 'PGDL2', @CNPTId, 0);

-- Chi nhánh Sìn Hồ
INSERT INTO Units (Code, Name, Type, ParentUnitId, IsDeleted) VALUES
('CNSH', N'Chi nhánh Sìn Hồ', 'CNL2', @RootId, 0);

DECLARE @CNSHId INT = SCOPE_IDENTITY();

INSERT INTO Units (Code, Name, Type, ParentUnitId, IsDeleted) VALUES
('BGDCNSH', N'Ban Giám đốc', 'PNVL2', @CNSHId, 0),
('PKTNQCNSH', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', @CNSHId, 0),
('PKHCNSH', N'Phòng Khách hàng', 'PNVL2', @CNSHId, 0);

-- Chi nhánh Bum Tở
INSERT INTO Units (Code, Name, Type, ParentUnitId, IsDeleted) VALUES
('CNBT', N'Chi nhánh Bum Tở', 'CNL2', @RootId, 0);

DECLARE @CNBTId INT = SCOPE_IDENTITY();

INSERT INTO Units (Code, Name, Type, ParentUnitId, IsDeleted) VALUES
('BGDCNBT', N'Ban Giám đốc', 'PNVL2', @CNBTId, 0),
('PKTNQCNBT', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', @CNBTId, 0),
('PKHCNBT', N'Phòng Khách hàng', 'PNVL2', @CNBTId, 0);

-- Chi nhánh Than Uyên
INSERT INTO Units (Code, Name, Type, ParentUnitId, IsDeleted) VALUES
('CNTU', N'Chi nhánh Than Uyên', 'CNL2', @RootId, 0);

DECLARE @CNTUId INT = SCOPE_IDENTITY();

INSERT INTO Units (Code, Name, Type, ParentUnitId, IsDeleted) VALUES
('BGDCNTU', N'Ban Giám đốc', 'PNVL2', @CNTUId, 0),
('PKTNQCNTU', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', @CNTUId, 0),
('PKHCNTU', N'Phòng Khách hàng', 'PNVL2', @CNTUId, 0),
('PGD6CNTU', N'Phòng giao dịch số 6', 'PGDL2', @CNTUId, 0);

-- Chi nhánh Đoàn Kết
INSERT INTO Units (Code, Name, Type, ParentUnitId, IsDeleted) VALUES
('CNDK', N'Chi nhánh Đoàn Kết', 'CNL2', @RootId, 0);

DECLARE @CNDKId INT = SCOPE_IDENTITY();

INSERT INTO Units (Code, Name, Type, ParentUnitId, IsDeleted) VALUES
('BGDCNDK', N'Ban Giám đốc', 'PNVL2', @CNDKId, 0),
('PKTNQCNDK', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', @CNDKId, 0),
('PKHCNDK', N'Phòng Khách hàng', 'PNVL2', @CNDKId, 0),
('PGD1CNDK', N'Phòng giao dịch số 1', 'PGDL2', @CNDKId, 0),
('PGD2CNDK', N'Phòng giao dịch số 2', 'PGDL2', @CNDKId, 0);

-- Chi nhánh Tân Uyên
INSERT INTO Units (Code, Name, Type, ParentUnitId, IsDeleted) VALUES
('CNTUY', N'Chi nhánh Tân Uyên', 'CNL2', @RootId, 0);

DECLARE @CNTUYId INT = SCOPE_IDENTITY();

INSERT INTO Units (Code, Name, Type, ParentUnitId, IsDeleted) VALUES
('BGDCNTUY', N'Ban Giám đốc', 'PNVL2', @CNTUYId, 0),
('PKTNQCNTUY', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', @CNTUYId, 0),
('PKHCNTUY', N'Phòng Khách hàng', 'PNVL2', @CNTUYId, 0),
('PGD3CNTUY', N'Phòng giao dịch số 3', 'PGDL2', @CNTUYId, 0);

-- Chi nhánh Nậm Hàng
INSERT INTO Units (Code, Name, Type, ParentUnitId, IsDeleted) VALUES
('CNNH', N'Chi nhánh Nậm Hàng', 'CNL2', @RootId, 0);

DECLARE @CNNHId INT = SCOPE_IDENTITY();

INSERT INTO Units (Code, Name, Type, ParentUnitId, IsDeleted) VALUES
('BGDCNNH', N'Ban Giám đốc', 'PNVL2', @CNNHId, 0),
('PKTNQCNNH', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', @CNNHId, 0),
('PKHCNNH', N'Phòng Khách hàng', 'PNVL2', @CNNHId, 0);

-- Verify results
SELECT
    Type,
    COUNT(*) as Count
FROM Units
GROUP BY Type
ORDER BY Type;

SELECT COUNT(*) as TotalUnits FROM Units;

PRINT '✅ Successfully restored all 46+ units from codebase!';
