-- Script tạo KPI Indicators phần 2 (bảng 8-15)
-- Date: 2025-06-18

-- 8. TruongphongKtnqCnl1 (Table ID = 8) - 6 chỉ tiêu KTNQ CNL1
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(8, 'Tổng nguồn vốn', 10, 'Tỷ VND', 1),
(8, 'Lợi nhuận khoán tài chính', 20, 'Tỷ VND', 2),
(8, 'Thu dịch vụ thanh toán trong nước', 10, 'Tỷ VND', 3),
(8, 'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 40, '%', 4),
(8, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 5),
(8, 'Kết quả thực hiện BQ của CB trong phòng', 10, '%', 6);

-- 9. PhophongKtnqCnl1 (Table ID = 9) - 6 chỉ tiêu KTNQ CNL1
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(9, 'Tổng nguồn vốn', 10, 'Tỷ VND', 1),
(9, 'Lợi nhuận khoán tài chính', 20, 'Tỷ VND', 2),
(9, 'Thu dịch vụ thanh toán trong nước', 10, 'Tỷ VND', 3),
(9, 'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 40, '%', 4),
(9, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 5),
(9, 'Kết quả thực hiện BQ của CB trong phòng', 10, '%', 6);

-- 10. Gdv (Table ID = 10) - 5 chỉ tiêu GDV
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(10, 'Số bút toán giao dịch BQ', 50, 'BT', 1),
(10, 'Số bút toán hủy', 15, 'BT', 2),
(10, 'Thực hiện chức năng, nhiệm vụ được giao', 10, '%', 3),
(10, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 4),
(10, 'Hoàn thành chỉ tiêu giao khoán SPDV', 15, '%', 5);

-- 11. TqHkKtnb (Table ID = 11) - 5 chỉ tiêu TQ/HK/KTNB  
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(11, 'Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ', 60, '%', 1),
(11, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 15, '%', 2),
(11, 'Không để phát sinh sai sót trong công việc', 10, '%', 3),
(11, 'Hoàn thành chỉ tiêu giao khoán SPDV', 15, '%', 4);

-- 12. TruongphoItThKtgs (Table ID = 12) - 4 chỉ tiêu IT/TH/KTGS
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(12, 'Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng', 60, '%', 1),
(12, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 15, '%', 2),
(12, 'Kết quả thực hiện BQ của CB trong phòng', 15, '%', 3),
(12, 'Hoàn thành chỉ tiêu giao khoán SPDV', 10, '%', 4);

-- 13. CBItThKtgsKhqlrr (Table ID = 13) - 4 chỉ tiêu CB IT/TH/KTGS/KHQLRR
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(13, 'Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ', 60, '%', 1),
(13, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 15, '%', 2),
(13, 'Không để phát sinh sai sót trong công việc', 15, '%', 3),
(13, 'Hoàn thành chỉ tiêu giao khoán SPDV', 10, '%', 4);

-- 14. GiamdocPgd (Table ID = 14) - 11 chỉ tiêu PGD (Giống CNL2)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(14, 'Tổng nguồn vốn', 10, 'Tỷ VND', 1),
(14, 'Tổng dư nợ', 10, 'Tỷ VND', 2),
(14, 'Tỷ lệ nợ xấu', 10, '%', 3),
(14, 'Lợi nhuận khoán tài chính', 15, 'Tỷ VND', 4),
(14, 'Thu dịch vụ thanh toán trong nước', 10, 'Tỷ VND', 5),
(14, 'Tổng doanh thu phí dịch vụ', 10, 'Tỷ VND', 6),
(14, 'Số thẻ phát hành', 5, 'cái', 7),
(14, 'Điều hành theo chương trình công tác', 10, '%', 8),
(14, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 9),
(14, 'Kết quả thực hiện BQ của CB cấp dưới', 5, '%', 10),
(14, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 11);

-- 15. PhogiamdocPgd (Table ID = 15) - 11 chỉ tiêu PGD (Giống CNL2)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(15, 'Tổng nguồn vốn', 10, 'Tỷ VND', 1),
(15, 'Tổng dư nợ', 10, 'Tỷ VND', 2),
(15, 'Tỷ lệ nợ xấu', 10, '%', 3),
(15, 'Lợi nhuận khoán tài chính', 15, 'Tỷ VND', 4),
(15, 'Thu dịch vụ thanh toán trong nước', 10, 'Tỷ VND', 5),
(15, 'Tổng doanh thu phí dịch vụ', 10, 'Tỷ VND', 6),
(15, 'Số thẻ phát hành', 5, 'cái', 7),
(15, 'Điều hành theo chương trình công tác', 10, '%', 8),
(15, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 9),
(15, 'Kết quả thực hiện BQ của CB cấp dưới', 5, '%', 10),
(15, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 11);
