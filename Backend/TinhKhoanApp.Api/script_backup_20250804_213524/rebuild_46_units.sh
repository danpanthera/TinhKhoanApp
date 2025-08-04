#!/bin/bash

# 🔄 Rebuild 46 Units Structure - New Hierarchical Structure with ParentID
# Xóa sạch và tạo lại 46 đơn vị theo sơ đồ phân cấp mới

echo "🔄 REBUILDING 46 UNITS STRUCTURE - NEW HIERARCHY"
echo "================================================"

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
echo "🏢 Step 2: Creating 46 Units with new hierarchical structure..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
SET QUOTED_IDENTIFIER ON;
SET IDENTITY_INSERT Units ON;

-- 1. ROOT: Chi nhánh Lai Châu (ID=1, CNL1) [ROOT]
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(1, 'CNLC', N'Chi nhánh Lai Châu', 'CNL1', NULL, 0);

-- 2. Hội Sở (ID=2, CNL1, Parent ID=1)
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(2, 'HOISO', N'Hội Sở', 'CNL1', 1, 0);

-- 3-9. Các phòng ban Hội Sở (PNVL1, parent ID=2)
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(3, 'BGD_HS', N'Ban Giám đốc', 'PNVL1', 2, 0),
(4, 'PKHDN', N'Phòng Khách hàng Doanh nghiệp', 'PNVL1', 2, 0),
(5, 'PKHCN', N'Phòng Khách hàng Cá nhân', 'PNVL1', 2, 0),
(6, 'PKTNQ_HS', N'Phòng Kế toán & Ngân quỹ', 'PNVL1', 2, 0),
(7, 'PTH', N'Phòng Tổng hợp', 'PNVL1', 2, 0),
(8, 'PKHQLRR', N'Phòng Kế hoạch & Quản lý rủi ro', 'PNVL1', 2, 0),
(9, 'PKTGS', N'Phòng Kiểm tra giám sát', 'PNVL1', 2, 0);

SELECT 'Step 2a: Root + Hội Sở + 7 phòng ban created' as Status;
" -C

echo ""
echo "🏢 Step 3: Creating 8 Chi nhánh cấp 2 (CNL2, parent ID=1)..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
SET QUOTED_IDENTIFIER ON;
SET IDENTITY_INSERT Units ON;

-- 10-17. Chi nhánh cấp 2 (8 chi nhánh, tất cả parent ID=1)
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
echo "🏢 Step 4: Creating departments for Chi nhánh Bình Lư (ID=10)..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
SET QUOTED_IDENTIFIER ON;
SET IDENTITY_INSERT Units ON;

-- Chi nhánh Bình Lư (ID=10) - 3 phòng ban (PNVL2, parent ID=10)
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(18, 'BGD_BL', N'Ban Giám đốc', 'PNVL2', 10, 0),
(19, 'PKTNQ_BL', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 10, 0),
(20, 'PKH_BL', N'Phòng Khách hàng', 'PNVL2', 10, 0);

SELECT 'Step 4: Bình Lư departments created' as Status;
" -C

echo ""
echo "🏢 Step 5: Creating departments for Chi nhánh Phong Thổ (ID=11)..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
SET QUOTED_IDENTIFIER ON;
SET IDENTITY_INSERT Units ON;

-- Chi nhánh Phong Thổ (ID=11) - 3 phòng ban + 1 PGD (parent ID=11)
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(21, 'BGD_PT', N'Ban Giám đốc', 'PNVL2', 11, 0),
(22, 'PKTNQ_PT', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 11, 0),
(23, 'PKH_PT', N'Phòng Khách hàng', 'PNVL2', 11, 0),
(24, 'PGD5_PT', N'Phòng giao dịch Số 5', 'PGDL2', 11, 0);

SELECT 'Step 5: Phong Thổ departments + PGD5 created' as Status;
" -C

echo ""
echo "🏢 Step 6: Creating departments for Chi nhánh Sìn Hồ (ID=12)..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
SET QUOTED_IDENTIFIER ON;
SET IDENTITY_INSERT Units ON;

-- Chi nhánh Sìn Hồ (ID=12) - 3 phòng ban (PNVL2, parent ID=12)
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(25, 'BGD_SH', N'Ban Giám đốc', 'PNVL2', 12, 0),
(26, 'PKTNQ_SH', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 12, 0),
(27, 'PKH_SH', N'Phòng Khách hàng', 'PNVL2', 12, 0);

SELECT 'Step 6: Sìn Hồ departments created' as Status;
" -C

echo ""
echo "🏢 Step 7: Creating departments for Chi nhánh Bum Tở (ID=13)..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
SET QUOTED_IDENTIFIER ON;
SET IDENTITY_INSERT Units ON;

-- Chi nhánh Bum Tở (ID=13) - 3 phòng ban (PNVL2, parent ID=13)
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(28, 'BGD_BT', N'Ban Giám đốc', 'PNVL2', 13, 0),
(29, 'PKTNQ_BT', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 13, 0),
(30, 'PKH_BT', N'Phòng Khách hàng', 'PNVL2', 13, 0);

SELECT 'Step 7: Bum Tở departments created' as Status;
" -C

echo ""
echo "🏢 Step 8: Creating departments for Chi nhánh Than Uyên (ID=14)..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
SET QUOTED_IDENTIFIER ON;
SET IDENTITY_INSERT Units ON;

-- Chi nhánh Than Uyên (ID=14) - 3 phòng ban + 1 PGD (parent ID=14)
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(31, 'BGD_TU', N'Ban Giám đốc', 'PNVL2', 14, 0),
(32, 'PKTNQ_TU', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 14, 0),
(33, 'PKH_TU', N'Phòng Khách hàng', 'PNVL2', 14, 0),
(34, 'PGD6_TU', N'Phòng giao dịch số 6', 'PGDL2', 14, 0);

SELECT 'Step 8: Than Uyên departments + PGD6 created' as Status;
" -C

echo ""
echo "🏢 Step 9: Creating departments for Chi nhánh Đoàn Kết (ID=15)..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
SET QUOTED_IDENTIFIER ON;
SET IDENTITY_INSERT Units ON;

-- Chi nhánh Đoàn Kết (ID=15) - 3 phòng ban + 2 PGD (parent ID=15)
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(35, 'BGD_DK', N'Ban Giám đốc', 'PNVL2', 15, 0),
(36, 'PKTNQ_DK', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 15, 0),
(37, 'PKH_DK', N'Phòng Khách hàng', 'PNVL2', 15, 0),
(38, 'PGD1_DK', N'Phòng giao dịch số 1', 'PGDL2', 15, 0),
(39, 'PGD2_DK', N'Phòng giao dịch số 2', 'PGDL2', 15, 0);

SELECT 'Step 9: Đoàn Kết departments + 2 PGDs created' as Status;
" -C

echo ""
echo "🏢 Step 10: Creating departments for Chi nhánh Tân Uyên (ID=16)..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
SET QUOTED_IDENTIFIER ON;
SET IDENTITY_INSERT Units ON;

-- Chi nhánh Tân Uyên (ID=16) - 3 phòng ban + 1 PGD (parent ID=16)
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(40, 'BGD_TUY', N'Ban Giám đốc', 'PNVL2', 16, 0),
(41, 'PKTNQ_TUY', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 16, 0),
(42, 'PKH_TUY', N'Phòng Khách hàng', 'PNVL2', 16, 0),
(43, 'PGD3_TUY', N'Phòng giao dịch số 3', 'PGDL2', 16, 0);

SELECT 'Step 10: Tân Uyên departments + PGD3 created' as Status;
" -C

echo ""
echo "🏢 Step 11: Creating departments for Chi nhánh Nậm Hàng (ID=17)..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
SET QUOTED_IDENTIFIER ON;
SET IDENTITY_INSERT Units ON;

-- Chi nhánh Nậm Hàng (ID=17) - 3 phòng ban (PNVL2, parent ID=17)
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(44, 'BGD_NH', N'Ban Giám đốc', 'PNVL2', 17, 0),
(45, 'PKTNQ_NH', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 17, 0),
(46, 'PKH_NH', N'Phòng Khách hàng', 'PNVL2', 17, 0);

SELECT 'Step 11: Nậm Hàng departments created' as Status;
" -C

echo ""
echo "🔄 Step 12: Disabling IDENTITY_INSERT and final verification..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
SET QUOTED_IDENTIFIER ON;
SET IDENTITY_INSERT Units OFF;

-- Final verification: Count units by type and hierarchy
SELECT
    'SUMMARY' as Section,
    Type,
    COUNT(*) as Count,
    STRING_AGG(CONCAT(Id, ': ', Name), ', ') as Units
FROM Units
GROUP BY Type
ORDER BY Type;

-- Hierarchical structure verification
SELECT
    'HIERARCHY' as Section,
    u1.Id as UnitId,
    u1.Name as UnitName,
    u1.Type as UnitType,
    u1.ParentUnitId,
    u2.Name as ParentName,
    u2.Type as ParentType
FROM Units u1
LEFT JOIN Units u2 ON u1.ParentUnitId = u2.Id
ORDER BY u1.Id;

SELECT 'NEW 46-UNIT STRUCTURE CREATED SUCCESSFULLY!' as FinalStatus;
" -C

echo ""
echo "✅ REBUILD COMPLETED!"
echo "📊 Summary of new structure:"
echo "  - CNL1: 2 units (Chi nhánh Lai Châu + Hội Sở)"
echo "  - CNL2: 8 chi nhánh (all under Chi nhánh Lai Châu)"
echo "  - PNVL1: 7 phòng ban Hội Sở"
echo "  - PNVL2: 24 phòng ban chi nhánh"
echo "  - PGDL2: 5 phòng giao dịch"
echo "  - TOTAL: 46 units with proper hierarchical structure"
echo ""
