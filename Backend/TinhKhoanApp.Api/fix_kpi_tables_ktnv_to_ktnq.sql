-- ===================================================================
-- Script cập nhật 4 bảng KPI từ KTNV sang KTNQ theo yêu cầu
-- ===================================================================
USE TinhKhoanApp;
GO

-- Tìm và hiển thị các bảng KPI hiện tại có chứa KTNV
SELECT Id, TableName, Description, TableType 
FROM KpiAssignmentTables 
WHERE TableName LIKE '%KTNV%' OR Description LIKE '%KTNV%'
ORDER BY TableName;

-- ===================================================================
-- CẬP NHẬT 4 BẢNG KPI THEO YÊU CẦU
-- ===================================================================

-- 1. Bảng KPI "Trưởng phòng KTNV CNL1" -> "Trưởng phòng KTNQ CNL1"
UPDATE KpiAssignmentTables 
SET TableName = 'Trưởng phòng KTNQ CNL1',
    Description = REPLACE(Description, 'KTNV', 'KTNQ'),
    TableType = 'TruongphongKtnqCnl1'
WHERE TableName = 'Trưởng phòng KTNV CNL1' OR TableName LIKE '%Trưởng phòng KTNV CNL1%';

-- 2. Bảng KPI "Phó phòng KTNV CNL1" -> "Phó phòng KTNQ CNL1"  
UPDATE KpiAssignmentTables 
SET TableName = 'Phó phòng KTNQ CNL1',
    Description = REPLACE(Description, 'KTNV', 'KTNQ'),
    TableType = 'PhophongKtnqCnl1'
WHERE TableName = 'Phó phòng KTNV CNL1' OR TableName LIKE '%Phó phòng KTNV CNL1%';

-- 3. Bảng KPI "Trưởng phòng KTNV CNL2" -> "Trưởng phòng KTNQ CNL2"
UPDATE KpiAssignmentTables 
SET TableName = 'Trưởng phòng KTNQ CNL2',
    Description = REPLACE(Description, 'KTNV', 'KTNQ'),
    TableType = 'TruongphongKtnqCnl2'
WHERE TableName = 'Trưởng phòng KTNV CNL2' OR TableName LIKE '%Trưởng phòng KTNV CNL2%';

-- 4. Bảng KPI "Phó phòng KTNV CNL2" -> "Phó phòng KTNQ CNL2"
UPDATE KpiAssignmentTables 
SET TableName = 'Phó phòng KTNQ CNL2',
    Description = REPLACE(Description, 'KTNV', 'KTNQ'),
    TableType = 'PhophongKtnqCnl2'
WHERE TableName = 'Phó phòng KTNV CNL2' OR TableName LIKE '%Phó phòng KTNV CNL2%';

-- ===================================================================
-- CẬP NHẬT CÁC CHỈ TIÊU KPI LIÊN QUAN
-- ===================================================================

-- Cập nhật các KPI Indicators cho các bảng vừa sửa
UPDATE KpiIndicators 
SET IndicatorName = REPLACE(IndicatorName, 'KTNV', 'KTNQ'),
    Description = REPLACE(ISNULL(Description, ''), 'KTNV', 'KTNQ')
WHERE TableId IN (
    SELECT Id FROM KpiAssignmentTables 
    WHERE TableType IN ('TruongphongKtnqCnl1', 'PhophongKtnqCnl1', 'TruongphongKtnqCnl2', 'PhophongKtnqCnl2')
);

-- ===================================================================
-- KIỂM TRA KẾT QUẢ SAU KHI CẬP NHẬT
-- ===================================================================

-- Hiển thị các bảng KPI đã được cập nhật
SELECT Id, TableName, Description, TableType 
FROM KpiAssignmentTables 
WHERE TableType IN ('TruongphongKtnqCnl1', 'PhophongKtnqCnl1', 'TruongphongKtnqCnl2', 'PhophongKtnqCnl2')
ORDER BY TableName;

-- Hiển thị số lượng chỉ tiêu KPI của từng bảng
SELECT kat.TableName, kat.TableType, COUNT(ki.Id) as SoLuongChiTieu
FROM KpiAssignmentTables kat
LEFT JOIN KpiIndicators ki ON kat.Id = ki.TableId AND ki.IsActive = 1
WHERE kat.TableType IN ('TruongphongKtnqCnl1', 'PhophongKtnqCnl1', 'TruongphongKtnqCnl2', 'PhophongKtnqCnl2')
GROUP BY kat.Id, kat.TableName, kat.TableType
ORDER BY kat.TableName;

PRINT 'Hoàn thành cập nhật 4 bảng KPI từ KTNV sang KTNQ!';
