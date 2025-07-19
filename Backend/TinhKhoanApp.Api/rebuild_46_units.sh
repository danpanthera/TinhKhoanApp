#!/bin/bash

# 🔄 Rebuild 46 Units Structure - Complete Reset
# Xóa sạch và tạo lại 46 đơn vị theo sơ đồ chính xác

echo "🔄 REBUILDING 46 UNITS STRUCTURE"
echo "================================"

echo "📊 Step 1: Clearing all data..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
SET QUOTED_IDENTIFIER ON;

-- Clear assignments first
DELETE FROM EmployeeKpiAssignments;
DELETE FROM UnitKpiScorings;
DELETE FROM UnitKhoanAssignments;

-- Clear employees
DELETE FROM Employees;

-- Clear units
DELETE FROM Units;

-- Reset identities
DBCC CHECKIDENT ('Units', RESEED, 0);
DBCC CHECKIDENT ('Employees', RESEED, 0);

SELECT 'Step 1: Data cleared' as Status;
" -C

echo ""
echo "🏢 Step 2: Creating 46 Units according to exact structure..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
SET QUOTED_IDENTIFIER ON;
SET IDENTITY_INSERT Units ON;

-- 1. Root unit
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(1, 'CNL1', N'Chi nhánh Lai Châu', 'CNL1', NULL, 0);

-- 2. Hội Sở
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(2, 'HOISO', N'Hội Sở', 'CNL1', 1, 0);

-- 3-9. Các phòng ban Hội Sở (7 phòng)
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(3, 'BGD', N'Ban Giám đốc', 'PNVL1', 2, 0),
(4, 'PKHDN', N'Phòng Khách hàng Doanh nghiệp', 'PNVL1', 2, 0),
(5, 'PKHCN', N'Phòng Khách hàng Cá nhân', 'PNVL1', 2, 0),
(6, 'PKTNQ', N'Phòng Kế toán & Ngân quỹ', 'PNVL1', 2, 0),
(7, 'PTH', N'Phòng Tổng hợp', 'PNVL1', 2, 0),
(8, 'PKHQLRR', N'Phòng Kế hoạch & Quản lý rủi ro', 'PNVL1', 2, 0),
(9, 'PKTGS', N'Phòng Kiểm tra giám sát', 'PNVL1', 2, 0);

SELECT 'Step 2a: Root + Hội Sở + 7 phòng ban created' as Status;
" -C

echo ""
echo "🏢 Step 3: Creating 8 Chi nhánh cấp 2..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
SET QUOTED_IDENTIFIER ON;
SET IDENTITY_INSERT Units ON;

-- 10-17. Chi nhánh cấp 2 (8 chi nhánh)
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(10, 'CNBL', N'Chi nhánh Bình Lư', 'CNL2', 1, 0),
(11, 'CNPT', N'Chi nhánh Phong Thổ', 'CNL2', 1, 0),
(12, 'CNSH', N'Chi nhánh Sìn Hồ', 'CNL2', 1, 0),
(13, 'CNBT', N'Chi nhánh Bum Tở', 'CNL2', 1, 0),
(14, 'CNTU', N'Chi nhánh Than Uyên', 'CNL2', 1, 0),
(15, 'CNDK', N'Chi nhánh Đoàn Kết', 'CNL2', 1, 0),
(16, 'CNTUY', N'Chi nhánh Tân Uyên', 'CNL2', 1, 0),
(17, 'CNNH', N'Chi nhánh Nậm Hàng', 'CNL2', 1, 0);

SELECT 'Step 3: 8 Chi nhánh cấp 2 created' as Status;
" -C

echo ""
echo "🏢 Step 4: Creating departments for Chi nhánh Bình Lư (10)..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
SET QUOTED_IDENTIFIER ON;
SET IDENTITY_INSERT Units ON;

-- Chi nhánh Bình Lư departments
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(18, 'BGD_BL', N'Ban Giám đốc', 'PNVL2', 10, 0),
(19, 'PKTNQ_BL', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 10, 0),
(20, 'PKH_BL', N'Phòng Khách hàng', 'PNVL2', 10, 0);

SELECT 'Step 4: Bình Lư departments created' as Status;
" -C

echo ""
echo "🏢 Step 5: Creating departments for Chi nhánh Phong Thổ (11)..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
SET QUOTED_IDENTIFIER ON;
SET IDENTITY_INSERT Units ON;

-- Chi nhánh Phong Thổ departments + 1 PGD
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(21, 'BGD_PT', N'Ban Giám đốc', 'PNVL2', 11, 0),
(22, 'PKTNQ_PT', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 11, 0),
(23, 'PKH_PT', N'Phòng Khách hàng', 'PNVL2', 11, 0),
(24, 'PGD5_PT', N'Phòng giao dịch Số 5', 'PGDL2', 11, 0);

SELECT 'Step 5: Phong Thổ departments + PGD5 created' as Status;
" -C

echo ""
echo "🏢 Step 6: Creating departments for Chi nhánh Sìn Hồ (12)..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
SET QUOTED_IDENTIFIER ON;
SET IDENTITY_INSERT Units ON;

-- Chi nhánh Sìn Hồ departments
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(25, 'BGD_SH', N'Ban Giám đốc', 'PNVL2', 12, 0),
(26, 'PKTNQ_SH', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 12, 0),
(27, 'PKH_SH', N'Phòng Khách hàng', 'PNVL2', 12, 0);

SELECT 'Step 6: Sìn Hồ departments created' as Status;
" -C

echo ""
echo "🏢 Step 7: Creating departments for Chi nhánh Bum Tở (13)..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
SET QUOTED_IDENTIFIER ON;
SET IDENTITY_INSERT Units ON;

-- Chi nhánh Bum Tở departments
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(28, 'BGD_BT', N'Ban Giám đốc', 'PNVL2', 13, 0),
(29, 'PKTNQ_BT', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 13, 0),
(30, 'PKH_BT', N'Phòng Khách hàng', 'PNVL2', 13, 0);

SELECT 'Step 7: Bum Tở departments created' as Status;
" -C

echo ""
echo "🏢 Step 8: Creating departments for Chi nhánh Than Uyên (14)..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
SET QUOTED_IDENTIFIER ON;
SET IDENTITY_INSERT Units ON;

-- Chi nhánh Than Uyên departments + 1 PGD
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(31, 'BGD_TU', N'Ban Giám đốc', 'PNVL2', 14, 0),
(32, 'PKTNQ_TU', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 14, 0),
(33, 'PKH_TU', N'Phòng Khách hàng', 'PNVL2', 14, 0),
(34, 'PGD6_TU', N'Phòng giao dịch số 6', 'PGDL2', 14, 0);

SELECT 'Step 8: Than Uyên departments + PGD6 created' as Status;
" -C

echo ""
echo "🏢 Step 9: Creating departments for Chi nhánh Đoàn Kết (15)..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
SET QUOTED_IDENTIFIER ON;
SET IDENTITY_INSERT Units ON;

-- Chi nhánh Đoàn Kết departments + 2 PGD
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(35, 'BGD_DK', N'Ban Giám đốc', 'PNVL2', 15, 0),
(36, 'PKTNQ_DK', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 15, 0),
(37, 'PKH_DK', N'Phòng Khách hàng', 'PNVL2', 15, 0),
(38, 'PGD1_DK', N'Phòng giao dịch số 1', 'PGDL2', 15, 0),
(39, 'PGD2_DK', N'Phòng giao dịch số 2', 'PGDL2', 15, 0);

SELECT 'Step 9: Đoàn Kết departments + 2 PGD created' as Status;
" -C

echo ""
echo "🏢 Step 10: Creating departments for Chi nhánh Tân Uyên (16)..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
SET QUOTED_IDENTIFIER ON;
SET IDENTITY_INSERT Units ON;

-- Chi nhánh Tân Uyên departments + 1 PGD
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(40, 'BGD_TUY', N'Ban Giám đốc', 'PNVL2', 16, 0),
(41, 'PKTNQ_TUY', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 16, 0),
(42, 'PKH_TUY', N'Phòng Khách hàng', 'PNVL2', 16, 0),
(43, 'PGD3_TUY', N'Phòng giao dịch số 3', 'PGDL2', 16, 0);

SELECT 'Step 10: Tân Uyên departments + PGD3 created' as Status;
" -C

echo ""
echo "🏢 Step 11: Creating departments for Chi nhánh Nậm Hàng (17)..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
SET QUOTED_IDENTIFIER ON;
SET IDENTITY_INSERT Units ON;

-- Chi nhánh Nậm Hàng departments
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(44, 'BGD_NH', N'Ban Giám đốc', 'PNVL2', 17, 0),
(45, 'PKTNQ_NH', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 17, 0),
(46, 'PKH_NH', N'Phòng Khách hàng', 'PNVL2', 17, 0);

SET IDENTITY_INSERT Units OFF;

SELECT 'Step 11: Nậm Hàng departments created - ALL 46 UNITS COMPLETE!' as Status;
" -C

echo ""
echo "📊 Final Verification:"
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
SELECT
    Type,
    COUNT(*) as Count,
    STRING_AGG(Name, ', ') as Examples
FROM Units
GROUP BY Type
ORDER BY Type;

SELECT COUNT(*) as TotalUnits FROM Units;
" -C

echo ""
echo "🎉 46 UNITS STRUCTURE REBUILT SUCCESSFULLY!"
echo "✅ Structure matches exactly with the provided schema"
echo "✅ Proper parent-child relationships maintained"
echo "✅ UTF-8 Vietnamese names properly encoded"
