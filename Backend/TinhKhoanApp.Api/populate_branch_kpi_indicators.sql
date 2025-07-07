-- 🏢 Script SQL tạo chỉ tiêu KPI cho 9 bảng chi nhánh
-- Dựa trên mẫu bảng "Giám đốc CNL2" (ID=17)

USE TinhKhoanDB;
GO

-- Xóa chỉ tiêu cũ của 9 bảng chi nhánh (ID 24-32)
DELETE FROM KpiIndicators WHERE KpiAssignmentTableId BETWEEN 24 AND 32;

-- Template chỉ tiêu dựa trên "Giám đốc CNL2"
DECLARE @indicators TABLE (
    TableId INT,
    IndicatorName NVARCHAR(255),
    MaxScore DECIMAL(5,2),
    Unit NVARCHAR(50),
    OrderIndex INT,
    ValueType NVARCHAR(20),
    IsActive BIT
);

-- Thêm chỉ tiêu cho từng bảng chi nhánh
INSERT INTO @indicators VALUES
-- HoiSo (24)
(24, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
(24, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
(24, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
(24, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
(24, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
(24, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
(24, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
(24, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1),

-- BinhLu (25)
(25, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
(25, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
(25, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
(25, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
(25, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
(25, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
(25, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
(25, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1),

-- PhongTho (26)
(26, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
(26, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
(26, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
(26, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
(26, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
(26, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
(26, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
(26, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1),

-- SinHo (27)
(27, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
(27, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
(27, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
(27, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
(27, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
(27, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
(27, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
(27, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1),

-- BumTo (28)
(28, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
(28, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
(28, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
(28, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
(28, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
(28, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
(28, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
(28, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1),

-- ThanUyen (29)
(29, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
(29, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
(29, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
(29, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
(29, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
(29, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
(29, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
(29, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1),

-- DoanKet (30)
(30, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
(30, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
(30, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
(30, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
(30, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
(30, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
(30, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
(30, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1),

-- TanUyen (31)
(31, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
(31, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
(31, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
(31, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
(31, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
(31, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
(31, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
(31, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1),

-- NamHang (32)
(32, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
(32, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
(32, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
(32, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
(32, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
(32, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
(32, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
(32, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1);

-- Insert vào bảng KpiIndicators
INSERT INTO KpiIndicators (
    KpiAssignmentTableId,
    IndicatorName,
    MaxScore,
    Unit,
    OrderIndex,
    ValueType,
    IsActive,
    CreatedAt,
    UpdatedAt
)
SELECT
    TableId,
    IndicatorName,
    MaxScore,
    Unit,
    OrderIndex,
    ValueType,
    IsActive,
    GETUTCDATE(),
    GETUTCDATE()
FROM @indicators;

-- Báo cáo kết quả
SELECT
    kat.TableName,
    kat.Description,
    COUNT(ki.Id) as IndicatorCount,
    SUM(ki.MaxScore) as TotalMaxScore
FROM KpiAssignmentTables kat
LEFT JOIN KpiIndicators ki ON kat.Id = ki.KpiAssignmentTableId
WHERE kat.Id BETWEEN 24 AND 32
GROUP BY kat.Id, kat.TableName, kat.Description
ORDER BY kat.Id;

PRINT '✅ Đã tạo thành công chỉ tiêu KPI cho 9 bảng chi nhánh!';
PRINT '📊 Mỗi bảng có 8 chỉ tiêu với tổng điểm 100';
