-- Script thêm đầy đủ 9 chi nhánh KPI (Hội sở + 8 chi nhánh loại II)
-- Date: 2025-06-18

-- Backup trước khi thêm
CREATE TABLE IF NOT EXISTS KpiAssignmentTables_Backup_FullBranch AS 
SELECT * FROM KpiAssignmentTables WHERE Category = 'Dành cho Chi nhánh';

-- Thêm các chi nhánh còn thiếu (7801-7805)

-- 1. Chi nhánh H. Tam Đường (7801) - ID = 25
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (25, 25, 'Chi nhánh H. Tam Đường (7801)', 'Bảng giao khoán KPI cho Chi nhánh H. Tam Đường', 'Dành cho Chi nhánh', 1, datetime('now'))
ON CONFLICT(Id) DO UPDATE SET
    TableName = 'Chi nhánh H. Tam Đường (7801)',
    Description = 'Bảng giao khoán KPI cho Chi nhánh H. Tam Đường';

-- 2. Chi nhánh H. Phong Thổ (7802) - ID = 26
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (26, 26, 'Chi nhánh H. Phong Thổ (7802)', 'Bảng giao khoán KPI cho Chi nhánh H. Phong Thổ', 'Dành cho Chi nhánh', 1, datetime('now'))
ON CONFLICT(Id) DO UPDATE SET
    TableName = 'Chi nhánh H. Phong Thổ (7802)',
    Description = 'Bảng giao khoán KPI cho Chi nhánh H. Phong Thổ';

-- 3. Chi nhánh H. Sìn Hồ (7803) - ID = 27
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (27, 27, 'Chi nhánh H. Sìn Hồ (7803)', 'Bảng giao khoán KPI cho Chi nhánh H. Sìn Hồ', 'Dành cho Chi nhánh', 1, datetime('now'))
ON CONFLICT(Id) DO UPDATE SET
    TableName = 'Chi nhánh H. Sìn Hồ (7803)',
    Description = 'Bảng giao khoán KPI cho Chi nhánh H. Sìn Hồ';

-- 4. Chi nhánh H. Mường Tè (7804) - ID = 28
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (28, 28, 'Chi nhánh H. Mường Tè (7804)', 'Bảng giao khoán KPI cho Chi nhánh H. Mường Tè', 'Dành cho Chi nhánh', 1, datetime('now'))
ON CONFLICT(Id) DO UPDATE SET
    TableName = 'Chi nhánh H. Mường Tè (7804)',
    Description = 'Bảng giao khoán KPI cho Chi nhánh H. Mường Tè';

-- 5. Chi nhánh H. Than Uyên (7805) - ID = 29
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (29, 29, 'Chi nhánh H. Than Uyên (7805)', 'Bảng giao khoán KPI cho Chi nhánh H. Than Uyên', 'Dành cho Chi nhánh', 1, datetime('now'))
ON CONFLICT(Id) DO UPDATE SET
    TableName = 'Chi nhánh H. Than Uyên (7805)',
    Description = 'Bảng giao khoán KPI cho Chi nhánh H. Than Uyên';

-- Thêm chỉ tiêu KPI cho 5 chi nhánh mới (mỗi chi nhánh 11 chỉ tiêu giống CNL2)

-- Chi nhánh H. Tam Đường (7801) - TableId = 25
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(25, 'Tổng nguồn vốn', 10, 'Tỷ VND', 1),
(25, 'Tổng dư nợ', 10, 'Tỷ VND', 2),
(25, 'Tỷ lệ nợ xấu', 10, '%', 3),
(25, 'Lợi nhuận khoán tài chính', 15, 'Tỷ VND', 4),
(25, 'Thu dịch vụ thanh toán trong nước', 10, 'Tỷ VND', 5),
(25, 'Tổng doanh thu phí dịch vụ', 10, 'Tỷ VND', 6),
(25, 'Số thẻ phát hành', 5, 'cái', 7),
(25, 'Điều hành theo chương trình công tác', 10, '%', 8),
(25, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 9),
(25, 'Kết quả thực hiện BQ của CB cấp dưới', 5, '%', 10),
(25, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 11);

-- Chi nhánh H. Phong Thổ (7802) - TableId = 26
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(26, 'Tổng nguồn vốn', 10, 'Tỷ VND', 1),
(26, 'Tổng dư nợ', 10, 'Tỷ VND', 2),
(26, 'Tỷ lệ nợ xấu', 10, '%', 3),
(26, 'Lợi nhuận khoán tài chính', 15, 'Tỷ VND', 4),
(26, 'Thu dịch vụ thanh toán trong nước', 10, 'Tỷ VND', 5),
(26, 'Tổng doanh thu phí dịch vụ', 10, 'Tỷ VND', 6),
(26, 'Số thẻ phát hành', 5, 'cái', 7),
(26, 'Điều hành theo chương trình công tác', 10, '%', 8),
(26, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 9),
(26, 'Kết quả thực hiện BQ của CB cấp dưới', 5, '%', 10),
(26, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 11);

-- Chi nhánh H. Sìn Hồ (7803) - TableId = 27
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(27, 'Tổng nguồn vốn', 10, 'Tỷ VND', 1),
(27, 'Tổng dư nợ', 10, 'Tỷ VND', 2),
(27, 'Tỷ lệ nợ xấu', 10, '%', 3),
(27, 'Lợi nhuận khoán tài chính', 15, 'Tỷ VND', 4),
(27, 'Thu dịch vụ thanh toán trong nước', 10, 'Tỷ VND', 5),
(27, 'Tổng doanh thu phí dịch vụ', 10, 'Tỷ VND', 6),
(27, 'Số thẻ phát hành', 5, 'cái', 7),
(27, 'Điều hành theo chương trình công tác', 10, '%', 8),
(27, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 9),
(27, 'Kết quả thực hiện BQ của CB cấp dưới', 5, '%', 10),
(27, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 11);

-- Chi nhánh H. Mường Tè (7804) - TableId = 28
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(28, 'Tổng nguồn vốn', 10, 'Tỷ VND', 1),
(28, 'Tổng dư nợ', 10, 'Tỷ VND', 2),
(28, 'Tỷ lệ nợ xấu', 10, '%', 3),
(28, 'Lợi nhuận khoán tài chính', 15, 'Tỷ VND', 4),
(28, 'Thu dịch vụ thanh toán trong nước', 10, 'Tỷ VND', 5),
(28, 'Tổng doanh thu phí dịch vụ', 10, 'Tỷ VND', 6),
(28, 'Số thẻ phát hành', 5, 'cái', 7),
(28, 'Điều hành theo chương trình công tác', 10, '%', 8),
(28, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 9),
(28, 'Kết quả thực hiện BQ của CB cấp dưới', 5, '%', 10),
(28, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 11);

-- Chi nhánh H. Than Uyên (7805) - TableId = 29
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(29, 'Tổng nguồn vốn', 10, 'Tỷ VND', 1),
(29, 'Tổng dư nợ', 10, 'Tỷ VND', 2),
(29, 'Tỷ lệ nợ xấu', 10, '%', 3),
(29, 'Lợi nhuận khoán tài chính', 15, 'Tỷ VND', 4),
(29, 'Thu dịch vụ thanh toán trong nước', 10, 'Tỷ VND', 5),
(29, 'Tổng doanh thu phí dịch vụ', 10, 'Tỷ VND', 6),
(29, 'Số thẻ phát hành', 5, 'cái', 7),
(29, 'Điều hành theo chương trình công tác', 10, '%', 8),
(29, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 9),
(29, 'Kết quả thực hiện BQ của CB cấp dưới', 5, '%', 10),
(29, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 11);

-- Kiểm tra kết quả
SELECT 'FINAL RESULT' as Status, Id, TableName 
FROM KpiAssignmentTables 
WHERE Category = 'Dành cho Chi nhánh' 
ORDER BY Id;

-- Thống kê cuối
SELECT COUNT(*) as 'Total Branch KPI Tables' 
FROM KpiAssignmentTables 
WHERE Category = 'Dành cho Chi nhánh';

SELECT COUNT(*) as 'Total Branch KPI Indicators' 
FROM KpiIndicators 
WHERE TableId IN (24, 25, 26, 27, 28, 29, 30, 31, 32);
