-- Kiểm tra các chỉ tiêu "Tỷ lệ..." và đơn vị của chúng
SELECT 
    IndicatorName,
    Unit,
    TableId,
    OrderIndex
FROM KpiIndicators 
WHERE IndicatorName LIKE N'Tỷ lệ%'
ORDER BY TableId, OrderIndex;

-- Đếm số chỉ tiêu "Tỷ lệ..." theo đơn vị
SELECT 
    Unit,
    COUNT(*) as Count
FROM KpiIndicators 
WHERE IndicatorName LIKE N'Tỷ lệ%'
GROUP BY Unit;

-- Kiểm tra tất cả các đơn vị trong hệ thống
SELECT DISTINCT Unit, COUNT(*) as Count
FROM KpiIndicators
GROUP BY Unit
ORDER BY Unit;
