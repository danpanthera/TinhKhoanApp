-- 🏢 Script SQL tạo chỉ tiêu KPI cho 9 bảng chi nhánh
-- Dựa trên bảng "Giám đốc Chi nhánh cấp 2" (11 chỉ tiêu)
-- Ngày tạo: 22:23:10 7/7/2025

-- Xóa chỉ tiêu cũ của 9 bảng chi nhánh (ID 24-32)
DELETE FROM KpiIndicators WHERE TableId BETWEEN 24 AND 32;

-- Bảng 24
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (24, N'Tổng nguồn vốn cuối kỳ', 5, N'Triệu VND', 1, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (24, N'Tổng nguồn vốn huy động BQ trong kỳ', 10, N'Triệu VND', 2, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (24, N'Tổng dư nợ cuối kỳ', 5, N'Triệu VND', 3, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (24, N'Tổng dư nợ BQ trong kỳ', 10, N'Triệu VND', 4, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (24, N'Tổng dư nợ HSX&CN', 5, N'Triệu VND', 5, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (24, N'Tỷ lệ nợ xấu nội bảng', 10, N'%', 6, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (24, N'Thu nợ đã XLRR', 5, N'Triệu VND', 7, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (24, N'Phát triển Khách hàng', 10, N'Khách hàng', 8, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (24, N'Lợi nhuận khoán tài chính', 20, N'Triệu VND', 9, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (24, N'Thu dịch vụ', 10, N'Triệu VND', 10, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (24, N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10, N'%', 11, NUMBER, 1);

-- Bảng 25
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (25, N'Tổng nguồn vốn cuối kỳ', 5, N'Triệu VND', 1, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (25, N'Tổng nguồn vốn huy động BQ trong kỳ', 10, N'Triệu VND', 2, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (25, N'Tổng dư nợ cuối kỳ', 5, N'Triệu VND', 3, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (25, N'Tổng dư nợ BQ trong kỳ', 10, N'Triệu VND', 4, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (25, N'Tổng dư nợ HSX&CN', 5, N'Triệu VND', 5, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (25, N'Tỷ lệ nợ xấu nội bảng', 10, N'%', 6, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (25, N'Thu nợ đã XLRR', 5, N'Triệu VND', 7, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (25, N'Phát triển Khách hàng', 10, N'Khách hàng', 8, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (25, N'Lợi nhuận khoán tài chính', 20, N'Triệu VND', 9, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (25, N'Thu dịch vụ', 10, N'Triệu VND', 10, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (25, N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10, N'%', 11, NUMBER, 1);

-- Bảng 26
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (26, N'Tổng nguồn vốn cuối kỳ', 5, N'Triệu VND', 1, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (26, N'Tổng nguồn vốn huy động BQ trong kỳ', 10, N'Triệu VND', 2, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (26, N'Tổng dư nợ cuối kỳ', 5, N'Triệu VND', 3, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (26, N'Tổng dư nợ BQ trong kỳ', 10, N'Triệu VND', 4, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (26, N'Tổng dư nợ HSX&CN', 5, N'Triệu VND', 5, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (26, N'Tỷ lệ nợ xấu nội bảng', 10, N'%', 6, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (26, N'Thu nợ đã XLRR', 5, N'Triệu VND', 7, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (26, N'Phát triển Khách hàng', 10, N'Khách hàng', 8, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (26, N'Lợi nhuận khoán tài chính', 20, N'Triệu VND', 9, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (26, N'Thu dịch vụ', 10, N'Triệu VND', 10, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (26, N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10, N'%', 11, NUMBER, 1);

-- Bảng 27
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (27, N'Tổng nguồn vốn cuối kỳ', 5, N'Triệu VND', 1, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (27, N'Tổng nguồn vốn huy động BQ trong kỳ', 10, N'Triệu VND', 2, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (27, N'Tổng dư nợ cuối kỳ', 5, N'Triệu VND', 3, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (27, N'Tổng dư nợ BQ trong kỳ', 10, N'Triệu VND', 4, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (27, N'Tổng dư nợ HSX&CN', 5, N'Triệu VND', 5, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (27, N'Tỷ lệ nợ xấu nội bảng', 10, N'%', 6, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (27, N'Thu nợ đã XLRR', 5, N'Triệu VND', 7, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (27, N'Phát triển Khách hàng', 10, N'Khách hàng', 8, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (27, N'Lợi nhuận khoán tài chính', 20, N'Triệu VND', 9, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (27, N'Thu dịch vụ', 10, N'Triệu VND', 10, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (27, N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10, N'%', 11, NUMBER, 1);

-- Bảng 28
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (28, N'Tổng nguồn vốn cuối kỳ', 5, N'Triệu VND', 1, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (28, N'Tổng nguồn vốn huy động BQ trong kỳ', 10, N'Triệu VND', 2, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (28, N'Tổng dư nợ cuối kỳ', 5, N'Triệu VND', 3, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (28, N'Tổng dư nợ BQ trong kỳ', 10, N'Triệu VND', 4, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (28, N'Tổng dư nợ HSX&CN', 5, N'Triệu VND', 5, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (28, N'Tỷ lệ nợ xấu nội bảng', 10, N'%', 6, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (28, N'Thu nợ đã XLRR', 5, N'Triệu VND', 7, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (28, N'Phát triển Khách hàng', 10, N'Khách hàng', 8, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (28, N'Lợi nhuận khoán tài chính', 20, N'Triệu VND', 9, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (28, N'Thu dịch vụ', 10, N'Triệu VND', 10, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (28, N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10, N'%', 11, NUMBER, 1);

-- Bảng 29
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (29, N'Tổng nguồn vốn cuối kỳ', 5, N'Triệu VND', 1, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (29, N'Tổng nguồn vốn huy động BQ trong kỳ', 10, N'Triệu VND', 2, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (29, N'Tổng dư nợ cuối kỳ', 5, N'Triệu VND', 3, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (29, N'Tổng dư nợ BQ trong kỳ', 10, N'Triệu VND', 4, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (29, N'Tổng dư nợ HSX&CN', 5, N'Triệu VND', 5, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (29, N'Tỷ lệ nợ xấu nội bảng', 10, N'%', 6, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (29, N'Thu nợ đã XLRR', 5, N'Triệu VND', 7, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (29, N'Phát triển Khách hàng', 10, N'Khách hàng', 8, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (29, N'Lợi nhuận khoán tài chính', 20, N'Triệu VND', 9, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (29, N'Thu dịch vụ', 10, N'Triệu VND', 10, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (29, N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10, N'%', 11, NUMBER, 1);

-- Bảng 30
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (30, N'Tổng nguồn vốn cuối kỳ', 5, N'Triệu VND', 1, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (30, N'Tổng nguồn vốn huy động BQ trong kỳ', 10, N'Triệu VND', 2, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (30, N'Tổng dư nợ cuối kỳ', 5, N'Triệu VND', 3, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (30, N'Tổng dư nợ BQ trong kỳ', 10, N'Triệu VND', 4, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (30, N'Tổng dư nợ HSX&CN', 5, N'Triệu VND', 5, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (30, N'Tỷ lệ nợ xấu nội bảng', 10, N'%', 6, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (30, N'Thu nợ đã XLRR', 5, N'Triệu VND', 7, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (30, N'Phát triển Khách hàng', 10, N'Khách hàng', 8, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (30, N'Lợi nhuận khoán tài chính', 20, N'Triệu VND', 9, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (30, N'Thu dịch vụ', 10, N'Triệu VND', 10, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (30, N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10, N'%', 11, NUMBER, 1);

-- Bảng 31
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (31, N'Tổng nguồn vốn cuối kỳ', 5, N'Triệu VND', 1, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (31, N'Tổng nguồn vốn huy động BQ trong kỳ', 10, N'Triệu VND', 2, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (31, N'Tổng dư nợ cuối kỳ', 5, N'Triệu VND', 3, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (31, N'Tổng dư nợ BQ trong kỳ', 10, N'Triệu VND', 4, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (31, N'Tổng dư nợ HSX&CN', 5, N'Triệu VND', 5, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (31, N'Tỷ lệ nợ xấu nội bảng', 10, N'%', 6, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (31, N'Thu nợ đã XLRR', 5, N'Triệu VND', 7, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (31, N'Phát triển Khách hàng', 10, N'Khách hàng', 8, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (31, N'Lợi nhuận khoán tài chính', 20, N'Triệu VND', 9, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (31, N'Thu dịch vụ', 10, N'Triệu VND', 10, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (31, N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10, N'%', 11, NUMBER, 1);

-- Bảng 32
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (32, N'Tổng nguồn vốn cuối kỳ', 5, N'Triệu VND', 1, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (32, N'Tổng nguồn vốn huy động BQ trong kỳ', 10, N'Triệu VND', 2, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (32, N'Tổng dư nợ cuối kỳ', 5, N'Triệu VND', 3, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (32, N'Tổng dư nợ BQ trong kỳ', 10, N'Triệu VND', 4, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (32, N'Tổng dư nợ HSX&CN', 5, N'Triệu VND', 5, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (32, N'Tỷ lệ nợ xấu nội bảng', 10, N'%', 6, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (32, N'Thu nợ đã XLRR', 5, N'Triệu VND', 7, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (32, N'Phát triển Khách hàng', 10, N'Khách hàng', 8, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (32, N'Lợi nhuận khoán tài chính', 20, N'Triệu VND', 9, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (32, N'Thu dịch vụ', 10, N'Triệu VND', 10, NUMBER, 1);
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (32, N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10, N'%', 11, NUMBER, 1);

-- ✅ Hoàn tất populate 11 chỉ tiêu cho 9 bảng chi nhánh (ID 24-32)
-- Total: 99 records inserted
