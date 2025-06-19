-- Script tạo KPI Indicators phần 3 (bảng 16-23 cán bộ + 24-33 chi nhánh)
-- Date: 2025-06-18

-- 16. PhogiamdocPgdCbtd (Table ID = 16) - 8 chỉ tiêu PGD CBTD
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(16, 'Tổng dư nợ BQ', 30, 'Tỷ VND', 1),
(16, 'Tỷ lệ nợ xấu', 15, '%', 2),
(16, 'Tỷ lệ thực thu lãi', 10, '%', 3),
(16, 'Thu nợ đã XLRR', 10, 'Tỷ VND', 4),
(16, 'Thực hiện nhiệm vụ theo chương trình công tác', 10, '%', 5),
(16, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 6),
(16, 'Tổng nguồn vốn huy động BQ', 10, 'Tỷ VND', 7),
(16, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 8);

-- 17. GiamdocCnl2 (Table ID = 17) - 11 chỉ tiêu CNL2
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(17, 'Tổng nguồn vốn', 10, 'Tỷ VND', 1),
(17, 'Tổng dư nợ', 10, 'Tỷ VND', 2),
(17, 'Tỷ lệ nợ xấu', 10, '%', 3),
(17, 'Lợi nhuận khoán tài chính', 15, 'Tỷ VND', 4),
(17, 'Thu dịch vụ thanh toán trong nước', 10, 'Tỷ VND', 5),
(17, 'Tổng doanh thu phí dịch vụ', 10, 'Tỷ VND', 6),
(17, 'Số thẻ phát hành', 5, 'cái', 7),
(17, 'Điều hành theo chương trình công tác', 10, '%', 8),
(17, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 9),
(17, 'Kết quả thực hiện BQ của CB cấp dưới', 5, '%', 10),
(17, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 11);

-- 18. PhogiamdocCnl2Td (Table ID = 18) - 8 chỉ tiêu CNL2 Tín dụng
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(18, 'Tổng dư nợ BQ', 30, 'Tỷ VND', 1),
(18, 'Tỷ lệ nợ xấu', 15, '%', 2),
(18, 'Tỷ lệ thực thu lãi', 10, '%', 3),
(18, 'Thu nợ đã XLRR', 10, 'Tỷ VND', 4),
(18, 'Thực hiện nhiệm vụ theo chương trình công tác', 10, '%', 5),
(18, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 6),
(18, 'Tổng nguồn vốn huy động BQ', 10, 'Tỷ VND', 7),
(18, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 8);

-- 19. PhogiamdocCnl2Kt (Table ID = 19) - 6 chỉ tiêu CNL2 Kế toán
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(19, 'Tổng nguồn vốn', 20, 'Tỷ VND', 1),
(19, 'Lợi nhuận khoán tài chính', 30, 'Tỷ VND', 2),
(19, 'Tổng doanh thu phí dịch vụ', 20, 'Tỷ VND', 3),
(19, 'Số thẻ phát hành', 10, 'cái', 4),
(19, 'Điều hành theo chương trình công tác, nhiệm vụ được giao', 10, '%', 5),
(19, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 6);

-- 20. TruongphongKhCnl2 (Table ID = 20) - 9 chỉ tiêu KH CNL2 Trưởng phòng
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(20, 'Tổng dư nợ', 20, 'Tỷ VND', 1),
(20, 'Tỷ lệ nợ xấu', 15, '%', 2),
(20, 'Tỷ lệ thực thu lãi', 10, '%', 3),
(20, 'Thu nợ đã XLRR', 10, 'Tỷ VND', 4),
(20, 'Điều hành theo chương trình công tác', 10, '%', 5),
(20, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 6),
(20, 'Tổng nguồn vốn huy động BQ', 10, 'Tỷ VND', 7),
(20, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 8),
(20, 'Kết quả thực hiện BQ của CB trong phòng', 10, '%', 9);

-- 21. PhophongKhCnl2 (Table ID = 21) - 8 chỉ tiêu KH CNL2 Phó phòng
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(21, 'Tổng dư nợ BQ', 30, 'Tỷ VND', 1),
(21, 'Tỷ lệ nợ xấu', 15, '%', 2),
(21, 'Tỷ lệ thực thu lãi', 10, '%', 3),
(21, 'Thu nợ đã XLRR', 10, 'Tỷ VND', 4),
(21, 'Thực hiện nhiệm vụ theo chương trình công tác', 10, '%', 5),
(21, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 6),
(21, 'Tổng nguồn vốn huy động BQ', 10, 'Tỷ VND', 7),
(21, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 8);

-- 22. TruongphongKtnqCnl2 (Table ID = 22) - 6 chỉ tiêu KTNQ CNL2 Trưởng phòng
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(22, 'Tổng nguồn vốn', 10, 'Tỷ VND', 1),
(22, 'Lợi nhuận khoán tài chính', 20, 'Tỷ VND', 2),
(22, 'Thu dịch vụ thanh toán trong nước', 10, 'Tỷ VND', 3),
(22, 'Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng', 40, '%', 4),
(22, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 5),
(22, 'Kết quả thực hiện BQ của CB trong phòng', 10, '%', 6);

-- 23. PhophongKtnqCnl2 (Table ID = 23) - 5 chỉ tiêu KTNQ CNL2 Phó phòng
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(23, 'Số bút toán giao dịch BQ', 40, 'BT', 1),
(23, 'Số bút toán hủy', 20, 'BT', 2),
(23, 'Thực hiện nhiệm vụ theo chương trình công tác', 25, '%', 3),
(23, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 4),
(23, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 5);

-- CHI NHÁNH: Tất cả các chi nhánh sẽ có chỉ tiêu giống CNL2 (11 chỉ tiêu)
-- 24. HoiSo (Table ID = 24) - 11 chỉ tiêu
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(24, 'Tổng nguồn vốn', 10, 'Tỷ VND', 1),
(24, 'Tổng dư nợ', 10, 'Tỷ VND', 2),
(24, 'Tỷ lệ nợ xấu', 10, '%', 3),
(24, 'Lợi nhuận khoán tài chính', 15, 'Tỷ VND', 4),
(24, 'Thu dịch vụ thanh toán trong nước', 10, 'Tỷ VND', 5),
(24, 'Tổng doanh thu phí dịch vụ', 10, 'Tỷ VND', 6),
(24, 'Số thẻ phát hành', 5, 'cái', 7),
(24, 'Điều hành theo chương trình công tác', 10, '%', 8),
(24, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 9),
(24, 'Kết quả thực hiện BQ của CB cấp dưới', 5, '%', 10),
(24, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 11);

-- 25. CnTamDuong (Table ID = 25) - 11 chỉ tiêu
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

-- 26. CnPhongTho (Table ID = 26) - 11 chỉ tiêu
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

-- 27. CnSinHo (Table ID = 27) - 11 chỉ tiêu
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

-- 28. CnMuongTe (Table ID = 28) - 11 chỉ tiêu
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

-- 29. CnThanUyen (Table ID = 29) - 11 chỉ tiêu
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

-- 30. CnThanhPho (Table ID = 30) - 11 chỉ tiêu
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(30, 'Tổng nguồn vốn', 10, 'Tỷ VND', 1),
(30, 'Tổng dư nợ', 10, 'Tỷ VND', 2),
(30, 'Tỷ lệ nợ xấu', 10, '%', 3),
(30, 'Lợi nhuận khoán tài chính', 15, 'Tỷ VND', 4),
(30, 'Thu dịch vụ thanh toán trong nước', 10, 'Tỷ VND', 5),
(30, 'Tổng doanh thu phí dịch vụ', 10, 'Tỷ VND', 6),
(30, 'Số thẻ phát hành', 5, 'cái', 7),
(30, 'Điều hành theo chương trình công tác', 10, '%', 8),
(30, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 9),
(30, 'Kết quả thực hiện BQ của CB cấp dưới', 5, '%', 10),
(30, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 11);

-- 31. CnTanUyen (Table ID = 31) - 11 chỉ tiêu
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(31, 'Tổng nguồn vốn', 10, 'Tỷ VND', 1),
(31, 'Tổng dư nợ', 10, 'Tỷ VND', 2),
(31, 'Tỷ lệ nợ xấu', 10, '%', 3),
(31, 'Lợi nhuận khoán tài chính', 15, 'Tỷ VND', 4),
(31, 'Thu dịch vụ thanh toán trong nước', 10, 'Tỷ VND', 5),
(31, 'Tổng doanh thu phí dịch vụ', 10, 'Tỷ VND', 6),
(31, 'Số thẻ phát hành', 5, 'cái', 7),
(31, 'Điều hành theo chương trình công tác', 10, '%', 8),
(31, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 9),
(31, 'Kết quả thực hiện BQ của CB cấp dưới', 5, '%', 10),
(31, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 11);

-- 32. CnNamNhun (Table ID = 32) - 11 chỉ tiêu
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(32, 'Tổng nguồn vốn', 10, 'Tỷ VND', 1),
(32, 'Tổng dư nợ', 10, 'Tỷ VND', 2),
(32, 'Tỷ lệ nợ xấu', 10, '%', 3),
(32, 'Lợi nhuận khoán tài chính', 15, 'Tỷ VND', 4),
(32, 'Thu dịch vụ thanh toán trong nước', 10, 'Tỷ VND', 5),
(32, 'Tổng doanh thu phí dịch vụ', 10, 'Tỷ VND', 6),
(32, 'Số thẻ phát hành', 5, 'cái', 7),
(32, 'Điều hành theo chương trình công tác', 10, '%', 8),
(32, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 9),
(32, 'Kết quả thực hiện BQ của CB cấp dưới', 5, '%', 10),
(32, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 11);

-- 33. CnTinhLaiChau (Table ID = 33) - 11 chỉ tiêu
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(33, 'Tổng nguồn vốn', 10, 'Tỷ VND', 1),
(33, 'Tổng dư nợ', 10, 'Tỷ VND', 2),
(33, 'Tỷ lệ nợ xấu', 10, '%', 3),
(33, 'Lợi nhuận khoán tài chính', 15, 'Tỷ VND', 4),
(33, 'Thu dịch vụ thanh toán trong nước', 10, 'Tỷ VND', 5),
(33, 'Tổng doanh thu phí dịch vụ', 10, 'Tỷ VND', 6),
(33, 'Số thẻ phát hành', 5, 'cái', 7),
(33, 'Điều hành theo chương trình công tác', 10, '%', 8),
(33, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 9),
(33, 'Kết quả thực hiện BQ của CB cấp dưới', 5, '%', 10),
(33, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 11);
