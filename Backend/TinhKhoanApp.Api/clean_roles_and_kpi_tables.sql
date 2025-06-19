-- Clean script to retain only the 23 roles from SeedKPIDefinitionMaxScore.cs
-- and reference the correct KPI tables

-- 1. Delete all existing data
DELETE FROM KpiIndicators;
DELETE FROM KpiAssignmentTables;
DELETE FROM Roles;

-- Reset auto-increment counters
DELETE FROM sqlite_sequence WHERE name='KpiIndicators';
DELETE FROM sqlite_sequence WHERE name='KpiAssignmentTables';
DELETE FROM sqlite_sequence WHERE name='Roles';

-- 2. Insert the 23 canonical roles from SeedKPIDefinitionMaxScore.cs
INSERT INTO Roles (Name, Description, IsActive, CreatedDate) VALUES
('TruongphongKhdn', 'Trưởng phòng KHDN', 1, datetime('now')),
('TruongphongKhcn', 'Trưởng phòng KHCN', 1, datetime('now')),
('PhophongKhdn', 'Phó phòng KHDN', 1, datetime('now')),
('PhophongKhcn', 'Phó phòng KHCN', 1, datetime('now')),
('TruongphongKhqlrr', 'Trưởng phòng Kế hoạch và Quản lý rủi ro', 1, datetime('now')),
('PhophongKhqlrr', 'Phó phòng Kế hoạch và Quản lý rủi ro', 1, datetime('now')),
('Cbtd', 'Cán bộ Tín dụng', 1, datetime('now')),
('TruongphongKtnqCnl1', 'Trưởng phòng KTNQ CNL1', 1, datetime('now')),
('PhophongKtnqCnl1', 'Phó phòng KTNQ CNL1', 1, datetime('now')),
('Gdv', 'Giao dịch viên', 1, datetime('now')),
('TqHkKtnb', 'TQ/Hậu kiểm/Kế toán nội bộ', 1, datetime('now')),
('TruongphongItThKtgs', 'Trưởng phó phòng IT/TH/KTGS', 1, datetime('now')),
('CbItThKtgsKhqlrr', 'CB IT/TH/KTGS/KHQLRR', 1, datetime('now')),
('GiamdocPgd', 'Giám đốc PGD', 1, datetime('now')),
('PhogiamdocPgd', 'Phó giám đốc PGD', 1, datetime('now')),
('PhogiamdocPgdCbtd', 'Phó giám đốc PGD kiêm CBTD', 1, datetime('now')),
('GiamdocCnl2', 'Giám đốc CNL2', 1, datetime('now')),
('PhogiamdocCnl2Td', 'Phó giám đốc CNL2 phụ trách Tín dụng', 1, datetime('now')),
('PhogiamdocCnl2Kt', 'Phó giám đốc CNL2 phụ trách Kế toán', 1, datetime('now')),
('TruongphongKhCnl2', 'Trưởng phòng KH CNL2', 1, datetime('now')),
('PhophongKhCnl2', 'Phó phòng KH CNL2', 1, datetime('now')),
('TruongphongKtnqCnl2', 'Trưởng phòng KTNQ CNL2', 1, datetime('now')),
('PhophongKtnqCnl2', 'Phó phòng KTNQ CNL2', 1, datetime('now'));

-- 3. Create KPI Assignment Tables for the 23 roles
INSERT INTO KpiAssignmentTables (TableName, Description, TableType, Category, IsActive, CreatedDate) VALUES
-- Roles 1-23 - Using the roleCode as part of table name
('KPI_TruongphongKhdn', 'Bảng KPI cho Trưởng phòng KHDN', 1, 'Dành cho Cán bộ', 1, datetime('now')),
('KPI_TruongphongKhcn', 'Bảng KPI cho Trưởng phòng KHCN', 2, 'Dành cho Cán bộ', 1, datetime('now')),
('KPI_PhophongKhdn', 'Bảng KPI cho Phó phòng KHDN', 3, 'Dành cho Cán bộ', 1, datetime('now')),
('KPI_PhophongKhcn', 'Bảng KPI cho Phó phòng KHCN', 4, 'Dành cho Cán bộ', 1, datetime('now')),
('KPI_TruongphongKhqlrr', 'Bảng KPI cho Trưởng phòng Kế hoạch và Quản lý rủi ro', 5, 'Dành cho Cán bộ', 1, datetime('now')),
('KPI_PhophongKhqlrr', 'Bảng KPI cho Phó phòng Kế hoạch và Quản lý rủi ro', 6, 'Dành cho Cán bộ', 1, datetime('now')),
('KPI_Cbtd', 'Bảng KPI cho Cán bộ Tín dụng', 7, 'Dành cho Cán bộ', 1, datetime('now')),
('KPI_TruongphongKtnqCnl1', 'Bảng KPI cho Trưởng phòng KTNQ CNL1', 8, 'Dành cho Cán bộ', 1, datetime('now')),
('KPI_PhophongKtnqCnl1', 'Bảng KPI cho Phó phòng KTNQ CNL1', 9, 'Dành cho Cán bộ', 1, datetime('now')),
('KPI_Gdv', 'Bảng KPI cho Giao dịch viên', 10, 'Dành cho Cán bộ', 1, datetime('now')),
('KPI_TqHkKtnb', 'Bảng KPI cho TQ/Hậu kiểm/Kế toán nội bộ', 11, 'Dành cho Cán bộ', 1, datetime('now')),
('KPI_TruongphongItThKtgs', 'Bảng KPI cho Trưởng phó phòng IT/TH/KTGS', 12, 'Dành cho Cán bộ', 1, datetime('now')),
('KPI_CbItThKtgsKhqlrr', 'Bảng KPI cho CB IT/TH/KTGS/KHQLRR', 13, 'Dành cho Cán bộ', 1, datetime('now')),
('KPI_GiamdocPgd', 'Bảng KPI cho Giám đốc PGD', 14, 'Dành cho Cán bộ', 1, datetime('now')),
('KPI_PhogiamdocPgd', 'Bảng KPI cho Phó giám đốc PGD', 15, 'Dành cho Cán bộ', 1, datetime('now')),
('KPI_PhogiamdocPgdCbtd', 'Bảng KPI cho Phó giám đốc PGD kiêm CBTD', 16, 'Dành cho Cán bộ', 1, datetime('now')),
('KPI_GiamdocCnl2', 'Bảng KPI cho Giám đốc CNL2', 17, 'Dành cho Cán bộ', 1, datetime('now')),
('KPI_PhogiamdocCnl2Td', 'Bảng KPI cho Phó giám đốc CNL2 phụ trách Tín dụng', 18, 'Dành cho Cán bộ', 1, datetime('now')),
('KPI_PhogiamdocCnl2Kt', 'Bảng KPI cho Phó giám đốc CNL2 phụ trách Kế toán', 19, 'Dành cho Cán bộ', 1, datetime('now')),
('KPI_TruongphongKhCnl2', 'Bảng KPI cho Trưởng phòng KH CNL2', 20, 'Dành cho Cán bộ', 1, datetime('now')),
('KPI_PhophongKhCnl2', 'Bảng KPI cho Phó phòng KH CNL2', 21, 'Dành cho Cán bộ', 1, datetime('now')),
('KPI_TruongphongKtnqCnl2', 'Bảng KPI cho Trưởng phòng KTNQ CNL2', 22, 'Dành cho Cán bộ', 1, datetime('now')),
('KPI_PhophongKtnqCnl2', 'Bảng KPI cho Phó phòng KTNQ CNL2', 23, 'Dành cho Cán bộ', 1, datetime('now'));

-- 4. Verify results
SELECT 'Tổng số vai trò:' as info, COUNT(*) as count FROM Roles;
SELECT 'Tổng số bảng KPI:' as info, COUNT(*) as count FROM KpiAssignmentTables;

SELECT 'Danh sách 23 vai trò với roleCode chuẩn:' as info;
SELECT ROW_NUMBER() OVER (ORDER BY Id) as '#', Id, Name as RoleCode, Description FROM Roles ORDER BY Id;

SELECT 'Danh sách 23 bảng KPI tương ứng:' as info;
SELECT ROW_NUMBER() OVER (ORDER BY Id) as '#', Id, TableName, Description FROM KpiAssignmentTables ORDER BY Id;
