-- Script khôi phục KPI Indicators bằng cách xóa và tạo lại
-- Xóa tất cả indicators hiện tại
DELETE FROM KpiIndicators;

-- Lấy TableId từ database và tạo indicators mẫu cho từng bảng
-- Script này sẽ tạo indicators cơ bản cho tất cả các bảng KPI

-- Helper: Tạo function để insert indicators cho một bảng
-- Vì SQLite không có stored procedures, chúng ta sẽ tạo manual

-- Lấy danh sách các bảng KPI hiện có
.mode table
.headers on
SELECT 'Danh sách bảng KPI hiện có:' as info;
SELECT Id, TableType, TableName FROM KpiAssignmentTables ORDER BY TableType;

-- Tạo indicators cơ bản cho tất cả các bảng (mẫu đơn giản)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT 
    t.Id as TableId,
    'Tổng nguồn vốn huy động BQ trong kỳ' as IndicatorName,
    10.0 as MaxScore,
    'Tỷ VND' as Unit,
    1 as OrderIndex,
    1 as ValueType,
    1 as IsActive
FROM KpiAssignmentTables t;

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT 
    t.Id as TableId,
    'Tổng dư nợ BQ trong kỳ' as IndicatorName,
    10.0 as MaxScore,
    'Tỷ VND' as Unit,
    2 as OrderIndex,
    1 as ValueType,
    1 as IsActive
FROM KpiAssignmentTables t;

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT 
    t.Id as TableId,
    'Tỷ lệ nợ xấu nội bảng' as IndicatorName,
    10.0 as MaxScore,
    '%' as Unit,
    3 as OrderIndex,
    2 as ValueType,
    1 as IsActive
FROM KpiAssignmentTables t;

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT 
    t.Id as TableId,
    'Thu nợ đã XLRR' as IndicatorName,
    5.0 as MaxScore,
    'Tỷ VND' as Unit,
    4 as OrderIndex,
    1 as ValueType,
    1 as IsActive
FROM KpiAssignmentTables t;

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT 
    t.Id as TableId,
    'Tỷ lệ thực thu lãi' as IndicatorName,
    10.0 as MaxScore,
    '%' as Unit,
    5 as OrderIndex,
    2 as ValueType,
    1 as IsActive
FROM KpiAssignmentTables t;

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT 
    t.Id as TableId,
    'Lợi nhuận khoán tài chính' as IndicatorName,
    20.0 as MaxScore,
    'Tỷ VND' as Unit,
    6 as OrderIndex,
    1 as ValueType,
    1 as IsActive
FROM KpiAssignmentTables t;

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT 
    t.Id as TableId,
    'Thu dịch vụ' as IndicatorName,
    10.0 as MaxScore,
    'Tỷ VND' as Unit,
    7 as OrderIndex,
    1 as ValueType,
    1 as IsActive
FROM KpiAssignmentTables t;

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT 
    t.Id as TableId,
    'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành, văn hóa Agribank' as IndicatorName,
    10.0 as MaxScore,
    '%' as Unit,
    8 as OrderIndex,
    2 as ValueType,
    1 as IsActive
FROM KpiAssignmentTables t;

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT 
    t.Id as TableId,
    'Phối hợp thực hiện các nhiệm vụ được giao' as IndicatorName,
    5.0 as MaxScore,
    '%' as Unit,
    9 as OrderIndex,
    2 as ValueType,
    1 as IsActive
FROM KpiAssignmentTables t;

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT 
    t.Id as TableId,
    'Sáng kiến, cải tiến quy trình nghiệp vụ' as IndicatorName,
    5.0 as MaxScore,
    '%' as Unit,
    10 as OrderIndex,
    2 as ValueType,
    1 as IsActive
FROM KpiAssignmentTables t;

INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT 
    t.Id as TableId,
    'Công tác an toàn, bảo mật' as IndicatorName,
    5.0 as MaxScore,
    '%' as Unit,
    11 as OrderIndex,
    2 as ValueType,
    1 as IsActive
FROM KpiAssignmentTables t;

-- Kiểm tra kết quả
SELECT 'Kết quả sau khi tạo KPI Indicators:' as info;
SELECT 
    t.TableName,
    t.Category,
    COUNT(i.Id) as IndicatorCount,
    SUM(i.MaxScore) as TotalMaxScore
FROM KpiAssignmentTables t
LEFT JOIN KpiIndicators i ON t.Id = i.TableId
GROUP BY t.Id, t.TableName, t.Category
ORDER BY t.TableType;

SELECT 'Tổng số KPI Indicators:', COUNT(*) as TotalIndicators FROM KpiIndicators;
