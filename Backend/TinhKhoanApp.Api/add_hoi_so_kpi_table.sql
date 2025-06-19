-- Script để tạo bảng KPI mới: "Hội sở và Các Chi nhánh loại II"
-- Tương tự như bảng GiamdocCnl2 nhưng dành cho tổng hợp

-- 1. Thêm bảng KPI mới
INSERT INTO KpiAssignmentTables (
    tableType, 
    tableName, 
    description, 
    isActive, 
    createdDate
) VALUES (
    'HoisoVaChinhanh2', 
    'Hội sở và Các Chi nhánh loại II', 
    'Bảng giao khoán KPI tổng hợp cho Hội sở và Các Chi nhánh loại II', 
    1, 
    DATETIME('now')
);

-- 2. Lấy ID của bảng vừa tạo để thêm indicators
-- Giả sử ID mới là 1025 (sẽ được tự động generate)

-- 3. Copy tất cả indicators từ bảng GiamdocCnl2 (ID: 1018) sang bảng mới
INSERT INTO KpiIndicators (
    tableId,
    indicatorName,
    maxScore,
    unit,
    orderIndex,
    valueType,
    isActive
)
SELECT 
    1025, -- ID của bảng mới (cần update sau khi tạo)
    indicatorName,
    maxScore,
    unit,
    orderIndex,
    valueType,
    isActive
FROM KpiIndicators 
WHERE tableId = 1018; -- ID của bảng GiamdocCnl2

-- 4. Verify kết quả
SELECT 
    kt.id,
    kt.tableName,
    kt.tableType,
    COUNT(ki.id) as indicator_count,
    SUM(ki.maxScore) as total_max_score
FROM KpiAssignmentTables kt
LEFT JOIN KpiIndicators ki ON kt.id = ki.tableId
WHERE kt.tableType = 'HoisoVaChinhanh2'
GROUP BY kt.id, kt.tableName, kt.tableType;
