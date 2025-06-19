-- CẬP NHẬT THỨ TỰ CHUẨN CHO TẤT CẢ CHI NHÁNH TRONG HỆ THỐNG
-- Thứ tự chuẩn: Hội sở, Tam Đường, Phong Thổ, Sìn Hồ, Mường Tè, Than Uyên, Thành Phố, Tân Uyên, Nậm Nhùn

-- 1. Thêm cột SortOrder vào bảng Units để quản lý thứ tự hiển thị
ALTER TABLE Units ADD COLUMN SortOrder INTEGER DEFAULT 999;

-- 2. Cập nhật SortOrder cho các chi nhánh theo thứ tự chuẩn
-- Hội sở = 1
UPDATE Units SET SortOrder = 1 WHERE UnitName LIKE '%Hội sở%' OR UnitName LIKE '%HOI SO%' OR UnitCode = 'HOI_SO';

-- Tam Đường = 2
UPDATE Units SET SortOrder = 2 WHERE UnitName LIKE '%Tam Đường%' OR UnitCode = 'CnTamDuong';

-- Phong Thổ = 3  
UPDATE Units SET SortOrder = 3 WHERE UnitName LIKE '%Phong Thổ%' OR UnitCode = 'CnPhongTho';

-- Sìn Hồ = 4
UPDATE Units SET SortOrder = 4 WHERE UnitName LIKE '%Sìn Hồ%' OR UnitCode = 'CnSinHo';

-- Mường Tè = 5
UPDATE Units SET SortOrder = 5 WHERE UnitName LIKE '%Mường Tè%' OR UnitCode = 'CnMuongTe';

-- Than Uyên = 6
UPDATE Units SET SortOrder = 6 WHERE UnitName LIKE '%Than Uyên%' OR UnitCode = 'CnThanUyen';

-- Thành Phố = 7
UPDATE Units SET SortOrder = 7 WHERE UnitName LIKE '%Thành Phố%' OR UnitCode = 'CnThanhPho';

-- Tân Uyên = 8
UPDATE Units SET SortOrder = 8 WHERE UnitName LIKE '%Tân Uyên%' OR UnitCode = 'CnTanUyen';

-- Nậm Nhùn = 9
UPDATE Units SET SortOrder = 9 WHERE UnitName LIKE '%Nậm Nhùn%' OR UnitCode = 'CnNamNhun';

-- 3. Thêm cột SortOrder vào bảng KpiAssignmentTables
ALTER TABLE KpiAssignmentTables ADD COLUMN SortOrder INTEGER DEFAULT 999;

-- 4. Cập nhật SortOrder cho bảng KpiAssignmentTables theo thứ tự chuẩn
UPDATE KpiAssignmentTables SET SortOrder = 1 WHERE TableName = 'KPI_HOI_SO';
UPDATE KpiAssignmentTables SET SortOrder = 2 WHERE TableName = 'KPI_CN_TAM_DUONG';
UPDATE KpiAssignmentTables SET SortOrder = 3 WHERE TableName = 'KPI_CN_PHONG_THO';
UPDATE KpiAssignmentTables SET SortOrder = 4 WHERE TableName = 'KPI_CN_SIN_HO';
UPDATE KpiAssignmentTables SET SortOrder = 5 WHERE TableName = 'KPI_CN_MUONG_TE';
UPDATE KpiAssignmentTables SET SortOrder = 6 WHERE TableName = 'KPI_CN_THAN_UYEN';
UPDATE KpiAssignmentTables SET SortOrder = 7 WHERE TableName = 'KPI_CN_THANH_PHO';
UPDATE KpiAssignmentTables SET SortOrder = 8 WHERE TableName = 'KPI_CN_TAN_UYEN';
UPDATE KpiAssignmentTables SET SortOrder = 9 WHERE TableName = 'KPI_CN_NAM_NHUN';

-- 5. Kiểm tra kết quả sau khi cập nhật
SELECT 'KIỂM TRA THỨ TỰ UNITS SAU KHI CẬP NHẬT' as Status;
SELECT SortOrder, UnitCode, UnitName 
FROM Units 
WHERE SortOrder <= 9 
ORDER BY SortOrder;

SELECT 'KIỂM TRA THỨ TỰ KPI ASSIGNMENT TABLES SAU KHI CẬP NHẬT' as Status;
SELECT SortOrder, TableName, DisplayName 
FROM KpiAssignmentTables 
WHERE Category = 'Dành cho Chi nhánh' 
ORDER BY SortOrder;

-- 6. Tạo view để luôn hiển thị chi nhánh theo thứ tự chuẩn
CREATE VIEW IF NOT EXISTS BranchesInOrder AS
SELECT 
    UnitCode,
    UnitName,
    SortOrder,
    CASE SortOrder
        WHEN 1 THEN 'Hội sở'
        WHEN 2 THEN 'Tam Đường'
        WHEN 3 THEN 'Phong Thổ'
        WHEN 4 THEN 'Sìn Hồ'
        WHEN 5 THEN 'Mường Tè'
        WHEN 6 THEN 'Than Uyên'
        WHEN 7 THEN 'Thành Phố'
        WHEN 8 THEN 'Tân Uyên'
        WHEN 9 THEN 'Nậm Nhùn'
        ELSE 'Khác'
    END as StandardName
FROM Units 
WHERE SortOrder <= 9
ORDER BY SortOrder;

-- 7. Kiểm tra view mới tạo
SELECT 'KIỂM TRA VIEW BranchesInOrder' as Status;
SELECT * FROM BranchesInOrder;
