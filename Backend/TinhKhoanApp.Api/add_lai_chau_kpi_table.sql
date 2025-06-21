-- Thêm bảng KPI cho Chi nhánh tỉnh Lai Châu sử dụng chỉ tiêu giống CNL2
-- Script này sẽ thêm bảng và các chỉ tiêu tương ứng

-- 1. Thêm bảng KPI assignment
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES ('CnTinhLaiChau', N'Chi nhánh tỉnh Lai Châu', N'Bảng giao khoán KPI cho Chi nhánh tỉnh Lai Châu sử dụng chỉ tiêu giống CNL2', N'Dành cho Chi nhánh', 1, GETDATE());

-- 2. Lấy ID của bảng vừa tạo
DECLARE @TableId INT;
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableType = 'CnTinhLaiChau';

-- 3. Thêm các chỉ tiêu KPI giống như GiamdocCnl2 (11 chỉ tiêu, tổng 100 điểm)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
VALUES 
    (@TableId, N'Tổng nguồn vốn cuối kỳ', 5, N'Tỷ VND', 1, 'NUMBER', 1),
    (@TableId, N'Tổng nguồn vốn huy động BQ trong kỳ', 10, N'Tỷ VND', 2, 'NUMBER', 1),
    (@TableId, N'Tổng dư nợ cuối kỳ', 5, N'Tỷ VND', 3, 'NUMBER', 1),
    (@TableId, N'Tổng dư nợ BQ trong kỳ', 10, N'Tỷ VND', 4, 'NUMBER', 1),
    (@TableId, N'Tổng dư nợ HSX&CN', 5, N'Tỷ VND', 5, 'NUMBER', 1),
    (@TableId, N'Tỷ lệ nợ xấu nội bảng', 10, N'%', 6, 'PERCENTAGE', 1),
    (@TableId, N'Thu nợ đã XLRR', 5, N'Tỷ VND', 7, 'NUMBER', 1),
    (@TableId, N'Phát triển khách hàng mới', 10, N'Khách hàng', 8, 'NUMBER', 1),
    (@TableId, N'Lợi nhuận khoán tài chính', 20, N'Tỷ VND', 9, 'NUMBER', 1),
    (@TableId, N'Thu dịch vụ', 10, N'Tỷ VND', 10, 'NUMBER', 1),
    (@TableId, N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành, văn hóa Agribank', 10, N'%', 11, 'PERCENTAGE', 1);

-- 4. Kiểm tra kết quả
SELECT 
    t.TableName,
    t.Description,
    t.Category,
    COUNT(i.Id) as IndicatorCount,
    SUM(i.MaxScore) as TotalMaxScore
FROM KpiAssignmentTables t
LEFT JOIN KpiIndicators i ON t.Id = i.TableId
WHERE t.TableType = 'CnTinhLaiChau'
GROUP BY t.Id, t.TableName, t.Description, t.Category;
