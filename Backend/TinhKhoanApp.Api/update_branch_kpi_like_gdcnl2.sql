-- Xóa hết chỉ tiêu cũ của 9 chi nhánh
DELETE FROM KpiIndicators WHERE TableId IN (
    SELECT Id FROM KpiAssignmentTables WHERE TableType >= 200 AND TableType <= 208
);

-- Thêm chỉ tiêu mới cho từng chi nhánh theo mẫu GiamdocCnl2 (TableId = 17)

-- Hội sở (7800) - TableId = 24
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate)
SELECT 24, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, datetime('now')
FROM KpiIndicators WHERE TableId = 17;

-- Chi nhánh H. Tam Dương (7801) - TableId = 34
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate)
SELECT 34, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, datetime('now')
FROM KpiIndicators WHERE TableId = 17;

-- Chi nhánh H. Phong Thổ (7802) - TableId = 35
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate)
SELECT 35, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, datetime('now')
FROM KpiIndicators WHERE TableId = 17;

-- Chi nhánh H. Sin Hồ (7803) - TableId = 36
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate)
SELECT 36, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, datetime('now')
FROM KpiIndicators WHERE TableId = 17;

-- Chi nhánh H. Mường Tè (7804) - TableId = 37
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate)
SELECT 37, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, datetime('now')
FROM KpiIndicators WHERE TableId = 17;

-- Chi nhánh H. Than Uyên (7805) - TableId = 38
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate)
SELECT 38, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, datetime('now')
FROM KpiIndicators WHERE TableId = 17;

-- Chi nhánh Thành Phố (7806) - TableId = 30
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate)
SELECT 30, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, datetime('now')
FROM KpiIndicators WHERE TableId = 17;

-- Chi nhánh H. Tân Uyên (7807) - TableId = 31
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate)
SELECT 31, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, datetime('now')
FROM KpiIndicators WHERE TableId = 17;

-- Chi nhánh H. Nậm Nhùn (7808) - TableId = 32
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate)
SELECT 32, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, datetime('now')
FROM KpiIndicators WHERE TableId = 17;

-- Kiểm tra kết quả
SELECT 'Số lượng chỉ tiêu của từng chi nhánh (theo mẫu GiamdocCnl2):' as message;
SELECT kat.TableName, COUNT(ki.Id) as TotalIndicators
FROM KpiAssignmentTables kat
LEFT JOIN KpiIndicators ki ON kat.Id = ki.TableId
WHERE kat.TableType >= 200 AND kat.TableType <= 208
GROUP BY kat.Id, kat.TableName
ORDER BY kat.TableType;

-- Kiểm tra chỉ tiêu mẫu
SELECT 'Chỉ tiêu mẫu từ GiamdocCnl2:' as message;
SELECT COUNT(*) as TotalFromGiamdocCnl2 FROM KpiIndicators WHERE TableId = 17;

-- Hiển thị ví dụ chỉ tiêu của chi nhánh đầu tiên
SELECT 'Ví dụ chỉ tiêu của Hội sở (7800):' as message;
SELECT IndicatorName, MaxScore, Unit FROM KpiIndicators WHERE TableId = 24 ORDER BY OrderIndex;
