-- Script tạo lại KPI Indicators cho 33 bảng KPI
-- Xóa indicators cũ
DELETE FROM KpiIndicators;

-- Tạo indicators cho từng bảng (dựa trên KpiAssignmentTableSeeder.cs)

-- 1. TruongphongKhdn (TableType = 1)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) 
SELECT 1, 'Tổng nguồn vốn huy động BQ trong kỳ', 10.0, 'Tỷ VND', 1, 1, 1
WHERE EXISTS (SELECT 1 FROM KpiAssignmentTables WHERE TableType = 1);

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) 
SELECT 1, 'Tổng dư nợ BQ trong kỳ', 10.0, 'Tỷ VND', 2, 1, 1
WHERE EXISTS (SELECT 1 FROM KpiAssignmentTables WHERE TableType = 1);

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) 
SELECT 1, 'Tỷ lệ nợ xấu nội bảng', 10.0, '%', 3, 2, 1
WHERE EXISTS (SELECT 1 FROM KpiAssignmentTables WHERE TableType = 1);

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) 
SELECT 1, 'Thu nợ đã XLRR', 5.0, 'Tỷ VND', 4, 1, 1
WHERE EXISTS (SELECT 1 FROM KpiAssignmentTables WHERE TableType = 1);

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) 
SELECT 1, 'Phát triển khách hàng mới', 10.0, 'Khách hàng', 5, 2, 1
WHERE EXISTS (SELECT 1 FROM KpiAssignmentTables WHERE TableType = 1);

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) 
SELECT 1, 'Lợi nhuận khoán tài chính', 20.0, 'Tỷ VND', 6, 1, 1
WHERE EXISTS (SELECT 1 FROM KpiAssignmentTables WHERE TableType = 1);

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) 
SELECT 1, 'Thu dịch vụ', 10.0, 'Tỷ VND', 7, 1, 1
WHERE EXISTS (SELECT 1 FROM KpiAssignmentTables WHERE TableType = 1);

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) 
SELECT 1, 'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành, văn hóa Agribank', 10.0, '%', 8, 2, 1
WHERE EXISTS (SELECT 1 FROM KpiAssignmentTables WHERE TableType = 1);

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) 
SELECT 1, 'Phối hợp thực hiện các nhiệm vụ được giao', 5.0, '%', 9, 2, 1
WHERE EXISTS (SELECT 1 FROM KpiAssignmentTables WHERE TableType = 1);

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) 
SELECT 1, 'Sáng kiến, cải tiến quy trình nghiệp vụ', 5.0, '%', 10, 2, 1
WHERE EXISTS (SELECT 1 FROM KpiAssignmentTables WHERE TableType = 1);

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) 
SELECT 1, 'Công tác an toàn, bảo mật', 5.0, '%', 11, 2, 1
WHERE EXISTS (SELECT 1 FROM KpiAssignmentTables WHERE TableType = 1);

-- 2. TruongphongKhcn (TableType = 2)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) 
SELECT 2, 'Tổng nguồn vốn huy động BQ trong kỳ', 15.0, 'Tỷ VND', 1, 1, 1
WHERE EXISTS (SELECT 1 FROM KpiAssignmentTables WHERE TableType = 2);

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) 
SELECT 2, 'Tổng dư nợ BQ trong kỳ', 10.0, 'Tỷ VND', 2, 1, 1
WHERE EXISTS (SELECT 1 FROM KpiAssignmentTables WHERE TableType = 2);

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) 
SELECT 2, 'Tỷ lệ nợ xấu nội bảng', 10.0, '%', 3, 2, 1
WHERE EXISTS (SELECT 1 FROM KpiAssignmentTables WHERE TableType = 2);

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) 
SELECT 2, 'Thu nợ đã XLRR', 5.0, 'Tỷ VND', 4, 1, 1
WHERE EXISTS (SELECT 1 FROM KpiAssignmentTables WHERE TableType = 2);

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) 
SELECT 2, 'Phát triển khách hàng mới', 10.0, 'Khách hàng', 5, 2, 1
WHERE EXISTS (SELECT 1 FROM KpiAssignmentTables WHERE TableType = 2);

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) 
SELECT 2, 'Lợi nhuận khoán tài chính', 15.0, 'Tỷ VND', 6, 1, 1
WHERE EXISTS (SELECT 1 FROM KpiAssignmentTables WHERE TableType = 2);

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) 
SELECT 2, 'Thu dịch vụ', 10.0, 'Tỷ VND', 7, 1, 1
WHERE EXISTS (SELECT 1 FROM KpiAssignmentTables WHERE TableType = 2);

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) 
SELECT 2, 'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành, văn hóa Agribank', 10.0, '%', 8, 2, 1
WHERE EXISTS (SELECT 1 FROM KpiAssignmentTables WHERE TableType = 2);

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) 
SELECT 2, 'Phối hợp thực hiện các nhiệm vụ được giao', 5.0, '%', 9, 2, 1
WHERE EXISTS (SELECT 1 FROM KpiAssignmentTables WHERE TableType = 2);

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) 
SELECT 2, 'Sáng kiến, cải tiến quy trình nghiệp vụ', 5.0, '%', 10, 2, 1
WHERE EXISTS (SELECT 1 FROM KpiAssignmentTables WHERE TableType = 2);

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) 
SELECT 2, 'Công tác an toàn, bảo mật', 5.0, '%', 11, 2, 1
WHERE EXISTS (SELECT 1 FROM KpiAssignmentTables WHERE TableType = 2);

-- Kiểm tra kết quả
SELECT 
    t.TableName,
    t.TableType,
    COUNT(i.Id) as IndicatorCount,
    SUM(i.MaxScore) as TotalMaxScore
FROM KpiAssignmentTables t
LEFT JOIN KpiIndicators i ON t.Id = i.TableId
GROUP BY t.Id, t.TableName, t.TableType
ORDER BY t.TableType;

SELECT 'Total KPI Indicators created:', COUNT(*) FROM KpiIndicators;
