-- ===============================================
-- CLEANUP KPI SYSTEM: XÓA BẢNG THỪA VÀ CHỈ TIÊU CŨ
-- Date: August 8, 2025
-- ===============================================

-- Bước 1: Xóa tất cả chỉ tiêu hiện tại (12 chỉ tiêu sample)
DELETE FROM KpiIndicators;

-- Bước 2: Xác định các bảng CANBO thừa cần xóa (28 -> 23 = xóa 5 bảng)
-- Danh sách 23 bảng CANBO cần GIỮ LẠI
DECLARE @ValidCanboTables TABLE (TableName VARCHAR(255))
INSERT INTO @ValidCanboTables VALUES
('TruongphongKhdn_KPI_Assignment'),
('TruongphongKhcn_KPI_Assignment'),
('PhophongKhdn_KPI_Assignment'),
('PhophongKhcn_KPI_Assignment'),
('TruongphongKhqlrr_KPI_Assignment'),
('PhophongKhqlrr_KPI_Assignment'),
('Cbtd_KPI_Assignment'),
('TruongphongKtnqCnl1_KPI_Assignment'),
('PhophongKtnqCnl1_KPI_Assignment'),
('Gdv_KPI_Assignment'),
('TqHkKtnb_KPI_Assignment'),
('TruongphongItThKtgs_KPI_Assignment'),
('CbItThKtgsKhqlrr_KPI_Assignment'),
('GiamdocPgd_KPI_Assignment'),
('PhogiamdocPgd_KPI_Assignment'),
('PhogiamdocPgdCbtd_KPI_Assignment'),
('GiamdocCnl2_KPI_Assignment'),
('PhogiamdocCnl2Td_KPI_Assignment'),
('PhogiamdocCnl2Kt_KPI_Assignment'),
('TruongphongKhCnl2_KPI_Assignment'),
('PhophongKhCnl2_KPI_Assignment'),
('TruongphongKtnqCnl2_KPI_Assignment'),
('PhophongKtnqCnl2_KPI_Assignment');

-- Bước 3: Danh sách 9 bảng CHINHANH cần GIỮ LẠI
DECLARE @ValidChinhanhTables TABLE (TableName VARCHAR(255))
INSERT INTO @ValidChinhanhTables VALUES
('HoiSo_KPI_Assignment'),
('CnBinhLu_KPI_Assignment'),
('CnPhongTho_KPI_Assignment'),
('CnSinHo_KPI_Assignment'),
('CnBumTo_KPI_Assignment'),
('CnThanUyen_KPI_Assignment'),
('CnDoanKet_KPI_Assignment'),
('CnTanUyen_KPI_Assignment'),
('CnNamHang_KPI_Assignment');

-- Bước 4: XÓA các bảng CANBO thừa
DELETE FROM KpiAssignmentTables
WHERE Category = 'CANBO'
AND TableName NOT IN (SELECT TableName FROM @ValidCanboTables);

-- Bước 5: XÓA các bảng CHINHANH thừa
DELETE FROM KpiAssignmentTables
WHERE Category = 'CHINHANH'
AND TableName NOT IN (SELECT TableName FROM @ValidChinhanhTables);

-- Bước 6: Kiểm tra kết quả
SELECT 'CANBO Tables Count' as ResultType, COUNT(*) as Count
FROM KpiAssignmentTables WHERE Category = 'CANBO'
UNION ALL
SELECT 'CHINHANH Tables Count' as ResultType, COUNT(*) as Count
FROM KpiAssignmentTables WHERE Category = 'CHINHANH'
UNION ALL
SELECT 'TOTAL KPI Tables' as ResultType, COUNT(*) as Count
FROM KpiAssignmentTables
UNION ALL
SELECT 'TOTAL KPI Indicators' as ResultType, COUNT(*) as Count
FROM KpiIndicators;

PRINT 'CLEANUP COMPLETED: 32 bảng chuẩn (23 CANBO + 9 CHINHANH), 0 chỉ tiêu (sẵn sàng cho việc thêm chỉ tiêu thực tế)'
