-- Script tạo 99 KPI Indicators cho 9 chi nhánh (11 chỉ tiêu × 9 chi nhánh)
-- Mỗi chi nhánh sẽ có 11 chỉ tiêu giống GiamdocCnl2

-- Tạo indicators cho 9 bảng KPI chi nhánh (TableId: 56-64)
-- Mỗi bảng có 11 chỉ tiêu giống GiamdocCnl2

-- BẢNG 1: KPI Hội Sở (ID: 56) - 11 chỉ tiêu
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(56, N'Tổng nguồn vốn cuối kỳ', 5, N'Triệu VND', 1, 4, 1),
(56, N'Tổng nguồn vốn huy động BQ trong kỳ', 10, N'Triệu VND', 2, 4, 1),
(56, N'Tổng dư nợ cuối kỳ', 5, N'Triệu VND', 3, 4, 1),
(56, N'Tổng dư nợ BQ trong kỳ', 10, N'Triệu VND', 4, 4, 1),
(56, N'Tổng dư nợ HSX&CN', 5, N'Triệu VND', 5, 4, 1),
(56, N'Tỷ lệ nợ xấu nội bảng', 10, N'%', 6, 2, 1),
(56, N'Thu nợ đã XLRR', 5, N'Triệu VND', 7, 4, 1),
(56, N'Phát triển Khách hàng', 10, N'Khách hàng', 8, 1, 1),
(56, N'Lợi nhuận khoán tài chính', 20, N'Triệu VND', 9, 4, 1),
(56, N'Thu dịch vụ', 10, N'Triệu VND', 10, 4, 1),
(56, N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10, N'%', 11, 2, 1),

-- BẢNG 2: KPI Chi nhánh Bình Lư (ID: 57) - 11 chỉ tiêu
(57, N'Tổng nguồn vốn cuối kỳ', 5, N'Triệu VND', 1, 4, 1),
(57, N'Tổng nguồn vốn huy động BQ trong kỳ', 10, N'Triệu VND', 2, 4, 1),
(57, N'Tổng dư nợ cuối kỳ', 5, N'Triệu VND', 3, 4, 1),
(57, N'Tổng dư nợ BQ trong kỳ', 10, N'Triệu VND', 4, 4, 1),
(57, N'Tổng dư nợ HSX&CN', 5, N'Triệu VND', 5, 4, 1),
(57, N'Tỷ lệ nợ xấu nội bảng', 10, N'%', 6, 2, 1),
(57, N'Thu nợ đã XLRR', 5, N'Triệu VND', 7, 4, 1),
(57, N'Phát triển Khách hàng', 10, N'Khách hàng', 8, 1, 1),
(57, N'Lợi nhuận khoán tài chính', 20, N'Triệu VND', 9, 4, 1),
(57, N'Thu dịch vụ', 10, N'Triệu VND', 10, 4, 1),
(57, N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10, N'%', 11, 2, 1),

-- BẢNG 3: KPI Chi nhánh Phong Thổ (ID: 58) - 11 chỉ tiêu
(58, N'Tổng nguồn vốn cuối kỳ', 5, N'Triệu VND', 1, 4, 1),
(58, N'Tổng nguồn vốn huy động BQ trong kỳ', 10, N'Triệu VND', 2, 4, 1),
(58, N'Tổng dư nợ cuối kỳ', 5, N'Triệu VND', 3, 4, 1),
(58, N'Tổng dư nợ BQ trong kỳ', 10, N'Triệu VND', 4, 4, 1),
(58, N'Tổng dư nợ HSX&CN', 5, N'Triệu VND', 5, 4, 1),
(58, N'Tỷ lệ nợ xấu nội bảng', 10, N'%', 6, 2, 1),
(58, N'Thu nợ đã XLRR', 5, N'Triệu VND', 7, 4, 1),
(58, N'Phát triển Khách hàng', 10, N'Khách hàng', 8, 1, 1),
(58, N'Lợi nhuận khoán tài chính', 20, N'Triệu VND', 9, 4, 1),
(58, N'Thu dịch vụ', 10, N'Triệu VND', 10, 4, 1),
(58, N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10, N'%', 11, 2, 1),

-- BẢNG 4: KPI Chi nhánh Sìn Hồ (ID: 59) - 11 chỉ tiêu
(59, N'Tổng nguồn vốn cuối kỳ', 5, N'Triệu VND', 1, 4, 1),
(59, N'Tổng nguồn vốn huy động BQ trong kỳ', 10, N'Triệu VND', 2, 4, 1),
(59, N'Tổng dư nợ cuối kỳ', 5, N'Triệu VND', 3, 4, 1),
(59, N'Tổng dư nợ BQ trong kỳ', 10, N'Triệu VND', 4, 4, 1),
(59, N'Tổng dư nợ HSX&CN', 5, N'Triệu VND', 5, 4, 1),
(59, N'Tỷ lệ nợ xấu nội bảng', 10, N'%', 6, 2, 1),
(59, N'Thu nợ đã XLRR', 5, N'Triệu VND', 7, 4, 1),
(59, N'Phát triển Khách hàng', 10, N'Khách hàng', 8, 1, 1),
(59, N'Lợi nhuận khoán tài chính', 20, N'Triệu VND', 9, 4, 1),
(59, N'Thu dịch vụ', 10, N'Triệu VND', 10, 4, 1),
(59, N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10, N'%', 11, 2, 1),

-- BẢNG 5: KPI Chi nhánh Bum Tở (ID: 60) - 11 chỉ tiêu
(60, N'Tổng nguồn vốn cuối kỳ', 5, N'Triệu VND', 1, 4, 1),
(60, N'Tổng nguồn vốn huy động BQ trong kỳ', 10, N'Triệu VND', 2, 4, 1),
(60, N'Tổng dư nợ cuối kỳ', 5, N'Triệu VND', 3, 4, 1),
(60, N'Tổng dư nợ BQ trong kỳ', 10, N'Triệu VND', 4, 4, 1),
(60, N'Tổng dư nợ HSX&CN', 5, N'Triệu VND', 5, 4, 1),
(60, N'Tỷ lệ nợ xấu nội bảng', 10, N'%', 6, 2, 1),
(60, N'Thu nợ đã XLRR', 5, N'Triệu VND', 7, 4, 1),
(60, N'Phát triển Khách hàng', 10, N'Khách hàng', 8, 1, 1),
(60, N'Lợi nhuận khoán tài chính', 20, N'Triệu VND', 9, 4, 1),
(60, N'Thu dịch vụ', 10, N'Triệu VND', 10, 4, 1),
(60, N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10, N'%', 11, 2, 1),

-- BẢNG 6: KPI Chi nhánh Than Uyên (ID: 61) - 11 chỉ tiêu
(61, N'Tổng nguồn vốn cuối kỳ', 5, N'Triệu VND', 1, 4, 1),
(61, N'Tổng nguồn vốn huy động BQ trong kỳ', 10, N'Triệu VND', 2, 4, 1),
(61, N'Tổng dư nợ cuối kỳ', 5, N'Triệu VND', 3, 4, 1),
(61, N'Tổng dư nợ BQ trong kỳ', 10, N'Triệu VND', 4, 4, 1),
(61, N'Tổng dư nợ HSX&CN', 5, N'Triệu VND', 5, 4, 1),
(61, N'Tỷ lệ nợ xấu nội bảng', 10, N'%', 6, 2, 1),
(61, N'Thu nợ đã XLRR', 5, N'Triệu VND', 7, 4, 1),
(61, N'Phát triển Khách hàng', 10, N'Khách hàng', 8, 1, 1),
(61, N'Lợi nhuận khoán tài chính', 20, N'Triệu VND', 9, 4, 1),
(61, N'Thu dịch vụ', 10, N'Triệu VND', 10, 4, 1),
(61, N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10, N'%', 11, 2, 1),

-- BẢNG 7: KPI Chi nhánh Đoàn Kết (ID: 62) - 11 chỉ tiêu
(62, N'Tổng nguồn vốn cuối kỳ', 5, N'Triệu VND', 1, 4, 1),
(62, N'Tổng nguồn vốn huy động BQ trong kỳ', 10, N'Triệu VND', 2, 4, 1),
(62, N'Tổng dư nợ cuối kỳ', 5, N'Triệu VND', 3, 4, 1),
(62, N'Tổng dư nợ BQ trong kỳ', 10, N'Triệu VND', 4, 4, 1),
(62, N'Tổng dư nợ HSX&CN', 5, N'Triệu VND', 5, 4, 1),
(62, N'Tỷ lệ nợ xấu nội bảng', 10, N'%', 6, 2, 1),
(62, N'Thu nợ đã XLRR', 5, N'Triệu VND', 7, 4, 1),
(62, N'Phát triển Khách hàng', 10, N'Khách hàng', 8, 1, 1),
(62, N'Lợi nhuận khoán tài chính', 20, N'Triệu VND', 9, 4, 1),
(62, N'Thu dịch vụ', 10, N'Triệu VND', 10, 4, 1),
(62, N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10, N'%', 11, 2, 1),

-- BẢNG 8: KPI Chi nhánh Tân Uyên (ID: 63) - 11 chỉ tiêu
(63, N'Tổng nguồn vốn cuối kỳ', 5, N'Triệu VND', 1, 4, 1),
(63, N'Tổng nguồn vốn huy động BQ trong kỳ', 10, N'Triệu VND', 2, 4, 1),
(63, N'Tổng dư nợ cuối kỳ', 5, N'Triệu VND', 3, 4, 1),
(63, N'Tổng dư nợ BQ trong kỳ', 10, N'Triệu VND', 4, 4, 1),
(63, N'Tổng dư nợ HSX&CN', 5, N'Triệu VND', 5, 4, 1),
(63, N'Tỷ lệ nợ xấu nội bảng', 10, N'%', 6, 2, 1),
(63, N'Thu nợ đã XLRR', 5, N'Triệu VND', 7, 4, 1),
(63, N'Phát triển Khách hàng', 10, N'Khách hàng', 8, 1, 1),
(63, N'Lợi nhuận khoán tài chính', 20, N'Triệu VND', 9, 4, 1),
(63, N'Thu dịch vụ', 10, N'Triệu VND', 10, 4, 1),
(63, N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10, N'%', 11, 2, 1),

-- BẢNG 9: KPI Chi nhánh Nậm Hàng (ID: 64) - 11 chỉ tiêu
(64, N'Tổng nguồn vốn cuối kỳ', 5, N'Triệu VND', 1, 4, 1),
(64, N'Tổng nguồn vốn huy động BQ trong kỳ', 10, N'Triệu VND', 2, 4, 1),
(64, N'Tổng dư nợ cuối kỳ', 5, N'Triệu VND', 3, 4, 1),
(64, N'Tổng dư nợ BQ trong kỳ', 10, N'Triệu VND', 4, 4, 1),
(64, N'Tổng dư nợ HSX&CN', 5, N'Triệu VND', 5, 4, 1),
(64, N'Tỷ lệ nợ xấu nội bảng', 10, N'%', 6, 2, 1),
(64, N'Thu nợ đã XLRR', 5, N'Triệu VND', 7, 4, 1),
(64, N'Phát triển Khách hàng', 10, N'Khách hàng', 8, 1, 1),
(64, N'Lợi nhuận khoán tài chính', 20, N'Triệu VND', 9, 4, 1),
(64, N'Thu dịch vụ', 10, N'Triệu VND', 10, 4, 1),
(64, N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10, N'%', 11, 2, 1);

-- Verification: Kiểm tra tổng số indicators sau khi thêm
SELECT 'TỔNG SỐ KPI INDICATORS SAU KHI THÊM CHI NHÁNH' AS Description, COUNT(*) AS Total FROM KpiIndicators;

-- Kiểm tra phân bố theo category
SELECT 'PHÂN BỔ THEO CATEGORY' AS Description;
SELECT
    kat.Category,
    COUNT(ki.Id) AS IndicatorCount,
    COUNT(DISTINCT ki.TableId) AS TableCount
FROM KpiIndicators ki
INNER JOIN KpiAssignmentTables kat ON ki.TableId = kat.Id
GROUP BY kat.Category
ORDER BY kat.Category;

-- Kiểm tra chi tiết cho từng bảng chi nhánh
SELECT 'CHI TIẾT BẢNG KPI CHI NHÁNH' AS Description;
SELECT
    kat.Id AS TableId,
    kat.TableName,
    COUNT(ki.Id) AS IndicatorCount
FROM KpiAssignmentTables kat
LEFT JOIN KpiIndicators ki ON kat.Id = ki.TableId
WHERE kat.Category = 'CHINHANH'
GROUP BY kat.Id, kat.TableName
ORDER BY kat.Id;
