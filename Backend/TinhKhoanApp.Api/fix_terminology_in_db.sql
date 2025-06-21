-- Script chuẩn hóa terminology: KTNV -> KTNQ, "Kinh tế Nội vụ" -> "Kế toán & Ngân quỹ"
-- Cập nhật KTNV thành KTNQ trong TableName
UPDATE KpiAssignmentTables SET TableName = REPLACE(TableName, 'KTNV', 'KTNQ') WHERE TableName LIKE '%KTNV%';

-- Cập nhật "Kinh tế Nội vụ" thành "Kế toán & Ngân quỹ" trong Description
UPDATE KpiAssignmentTables SET Description = REPLACE(Description, 'Kinh tế Nội vụ', 'Kế toán & Ngân quỹ') WHERE Description LIKE '%Kinh tế Nội vụ%';

-- Cập nhật "Hạch kiểm" thành "Hậu kiểm" trong Description
UPDATE KpiAssignmentTables SET Description = REPLACE(Description, 'Hạch kiểm', 'Hậu kiểm') WHERE Description LIKE '%Hạch kiểm%';

-- Cập nhật "phụ trách Kinh tế" thành "Phụ trách Kế toán" trong TableName
UPDATE KpiAssignmentTables SET TableName = REPLACE(TableName, 'phụ trách Kinh tế', 'Phụ trách Kế toán') WHERE TableName LIKE '%phụ trách Kinh tế%';
UPDATE KpiAssignmentTables SET TableName = REPLACE(TableName, 'Phó giám đốc CNL2 phụ trách Kinh tế', 'Phó giám đốc CNL2 Phụ trách Kế toán') WHERE TableName LIKE '%phụ trách Kinh tế%';

-- Cập nhật "phụ trách Kinh tế" thành "Phụ trách Kế toán" trong Description
UPDATE KpiAssignmentTables SET Description = REPLACE(Description, 'phụ trách Kinh tế', 'Phụ trách Kế toán') WHERE Description LIKE '%phụ trách Kinh tế%';
UPDATE KpiAssignmentTables SET Description = REPLACE(Description, 'Phó giám đốc Chi nhánh loại 2 phụ trách Kinh tế', 'Phó giám đốc Chi nhánh loại 2 Phụ trách Kế toán') WHERE Description LIKE '%phụ trách Kinh tế%';

-- Sửa "TQ/HK/KTNB" và description của nó
UPDATE KpiAssignmentTables SET TableName = 'TQ/HK/KTNQ', Description = REPLACE(Description, 'Thủ quỹ/Hạch kiểm/Kinh tế Nội bộ', 'Thủ quỹ/Hậu kiểm/Kế toán Nội bộ') WHERE TableName = 'TQ/HK/KTNB';

-- In kết quả kiểm tra
SELECT TableName, Description FROM KpiAssignmentTables 
WHERE TableName LIKE '%KTNV%' OR Description LIKE '%Kinh tế Nội vụ%' 
   OR Description LIKE '%Hạch kiểm%' OR TableName LIKE '%phụ trách Kinh tế%' 
   OR Description LIKE '%phụ trách Kinh tế%' OR TableName = 'TQ/HK/KTNB';
