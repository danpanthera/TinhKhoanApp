#!/bin/bash

# 🌱 SEED FULL SYSTEM - Khoan App
# Script to seed all core data: Units, Positions, Roles, Employees

echo "🌱 Starting Full System Seeding..."
echo "=================================="

# Step 1: Seed Positions using SQL
echo "📋 Step 1: Seeding Positions..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d KhoanDB -Q "
DELETE FROM Positions;
INSERT INTO Positions (Name, Description) VALUES
('Giamdoc', 'Giám đốc'),
('Phogiamdoc', 'Phó Giám đốc'),
('Truongphong', 'Trưởng phòng'),
('Photruongphong', 'Phó trưởng phòng'),
('GiamdocPhonggiaodich', 'Giám đốc phòng giao dịch'),
('PhogiamdocPhonggiaodich', 'Phó giám đốc phòng giao dịch'),
('Nhanvien', 'Nhân viên');
SELECT COUNT(*) as PositionsCount FROM Positions;
" -C

# Step 2: Seed Roles using SQL
echo "👥 Step 2: Seeding Roles..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d KhoanDB -Q "
DELETE FROM Roles;
INSERT INTO Roles (Name, Description) VALUES
('TruongphongKhdn', 'Trưởng phòng KHDN'),
('TruongphongKhcn', 'Trưởng phòng KHCN'),
('PhophongKhdn', 'Phó phòng KHDN'),
('PhophongKhcn', 'Phó phòng KHCN'),
('TruongphongKhqlrr', 'Trưởng phòng Kế hoạch và Quản lý rủi ro'),
('PhophongKhqlrr', 'Phó phòng Kế hoạch và Quản lý rủi ro'),
('Cbtd', 'Cán bộ Tín dụng'),
('TruongphongKtnqCnl1', 'Trưởng phòng KTNQ CNL1'),
('PhophongKtnqCnl1', 'Phó phòng KTNQ CNL1'),
('Gdv', 'Giao dịch viên'),
('TqHkKtnb', 'TQ/Hậu kiểm/Kế toán nội bộ'),
('TruongphongItThKtgs', 'Trưởng phó phòng IT/TH/KTGS'),
('CbItThKtgsKhqlrr', 'CB IT/TH/KTGS/KHQLRR'),
('GiamdocPgd', 'Giám đốc PGD'),
('PhogiamdocPgd', 'Phó giám đốc PGD'),
('PhogiamdocPgdCbtd', 'Phó giám đốc PGD kiêm CBTD'),
('GiamdocCnl2', 'Giám đốc CNL2'),
('PhogiamdocCnl2Td', 'Phó giám đốc CNL2 phụ trách Tín dụng'),
('PhogiamdocCnl2Kt', 'Phó giám đốc CNL2 Phụ trách Kế toán'),
('TruongphongKhCnl2', 'Trưởng phòng KH CNL2'),
('PhophongKhCnl2', 'Phó phòng KH CNL2'),
('TruongphongKtnqCnl2', 'Trưởng phòng KTNQ CNL2'),
('PhophongKtnqCnl2', 'Phó phòng KTNQ CNL2');
SELECT COUNT(*) as RolesCount FROM Roles;
" -C

# Step 3: Seed Admin Employee using SQL
echo "👤 Step 3: Seeding Admin Employee..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d KhoanDB -Q "
DELETE FROM Employees;
INSERT INTO Employees (EmployeeCode, CBCode, FullName, Username, PasswordHash, Email, IsActive, UnitId, PositionId)
SELECT 'ADMIN', 'CB001', 'Quản trị viên', 'admin', '\$2a\$11\$N9qo8uLOickgx2ZMRZoMve8YfYqfDzO2zM3JgXSVKnMU8jt/g7i12', 'admin@tinhkhoan.local', 1,
       (SELECT TOP 1 Id FROM Units ORDER BY Id),
       (SELECT TOP 1 Id FROM Positions ORDER BY Id);
SELECT COUNT(*) as EmployeesCount FROM Employees;
" -C

# Step 4: Verify all core tables
echo "✅ Step 4: Verification Summary..."
echo "================================="
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d KhoanDB -Q "
SELECT 'Units' as TableName, COUNT(*) as Count FROM Units
UNION ALL
SELECT 'Positions' as TableName, COUNT(*) as Count FROM Positions
UNION ALL
SELECT 'Roles' as TableName, COUNT(*) as Count FROM Roles
UNION ALL
SELECT 'Employees' as TableName, COUNT(*) as Count FROM Employees;
" -C

echo ""
echo "🎉 Full System Seeding Completed!"
echo "✅ Ready for CRUD operations on frontend"
