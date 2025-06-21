-- ======================================================
-- Script cập nhật terminology cuối cùng trong database
-- ======================================================
USE TinhKhoanApp;

-- 1. Thay thế "Kinh tế Nội vụ" → "Kế toán & Ngân quỹ"
UPDATE KpiAssignmentTables 
SET TableName = REPLACE(TableName, 'Kinh tế Nội vụ', 'Kế toán & Ngân quỹ'),
    Description = REPLACE(Description, 'Kinh tế Nội vụ', 'Kế toán & Ngân quỹ')
WHERE TableName LIKE '%Kinh tế Nội vụ%' OR Description LIKE '%Kinh tế Nội vụ%';

-- 2. Thay thế "Hạch kiểm" → "Hậu kiểm"  
UPDATE KpiAssignmentTables 
SET TableName = REPLACE(TableName, 'Hạch kiểm', 'Hậu kiểm'),
    Description = REPLACE(Description, 'Hạch kiểm', 'Hậu kiểm')
WHERE TableName LIKE '%Hạch kiểm%' OR Description LIKE '%Hạch kiểm%';

-- 3. Thay thế "phụ trách Kinh tế" → "Phụ trách Kế toán"
UPDATE KpiAssignmentTables 
SET TableName = REPLACE(TableName, 'phụ trách Kinh tế', 'Phụ trách Kế toán'),
    Description = REPLACE(Description, 'phụ trách Kinh tế', 'Phụ trách Kế toán')
WHERE TableName LIKE '%phụ trách Kinh tế%' OR Description LIKE '%phụ trách Kinh tế%';

-- 4. Thay thế "KTNV" → "KTNQ" (nếu còn sót lại)
UPDATE KpiAssignmentTables 
SET TableName = REPLACE(TableName, 'KTNV', 'KTNQ'),
    Description = REPLACE(Description, 'KTNV', 'KTNQ')
WHERE TableName LIKE '%KTNV%' OR Description LIKE '%KTNV%';

-- 5. Thay thế các cụm từ trong KPI Indicators
UPDATE KpiIndicators
SET IndicatorName = REPLACE(IndicatorName, 'Hạch kiểm', 'Hậu kiểm'),
    Description = REPLACE(Description, 'Hạch kiểm', 'Hậu kiểm')
WHERE IndicatorName LIKE '%Hạch kiểm%' OR Description LIKE '%Hạch kiểm%';

UPDATE KpiIndicators
SET IndicatorName = REPLACE(IndicatorName, 'Kinh tế Nội vụ', 'Kế toán & Ngân quỹ'),
    Description = REPLACE(Description, 'Kinh tế Nội vụ', 'Kế toán & Ngân quỹ')
WHERE IndicatorName LIKE '%Kinh tế Nội vụ%' OR Description LIKE '%Kinh tế Nội vụ%';

UPDATE KpiIndicators
SET IndicatorName = REPLACE(IndicatorName, 'phụ trách Kinh tế', 'Phụ trách Kế toán'),
    Description = REPLACE(Description, 'phụ trách Kinh tế', 'Phụ trách Kế toán')
WHERE IndicatorName LIKE '%phụ trách Kinh tế%' OR Description LIKE '%phụ trách Kinh tế%';

-- 6. Kiểm tra kết quả
SELECT 'KpiAssignmentTables' as TableName, COUNT(*) as AffectedRows
FROM KpiAssignmentTables 
WHERE TableName LIKE '%Kế toán & Ngân quỹ%' 
   OR TableName LIKE '%Hậu kiểm%' 
   OR TableName LIKE '%Phụ trách Kế toán%'
   OR TableName LIKE '%KTNQ%'

UNION ALL

SELECT 'KpiIndicators' as TableName, COUNT(*) as AffectedRows  
FROM KpiIndicators
WHERE IndicatorName LIKE '%Hậu kiểm%'
   OR IndicatorName LIKE '%Kế toán & Ngân quỹ%'
   OR IndicatorName LIKE '%Phụ trách Kế toán%';

-- 7. Hiển thị tất cả table names để kiểm tra
SELECT Id, TableName, Description 
FROM KpiAssignmentTables 
WHERE IsActive = 1
ORDER BY TableName;

PRINT '✅ Hoàn thành cập nhật terminology cuối cùng!';
