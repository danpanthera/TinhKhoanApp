-- Script để tái tạo KPI Indicators theo logic trong KpiAssignmentTableSeeder.cs
-- Date: 2025-06-18

-- Bước 1: Xóa tất cả indicators hiện tại (đã làm rồi)
-- DELETE FROM KpiIndicators;

-- Bước 2: Tạo indicators cho 23 bảng KPI cán bộ
-- Mỗi vai trò sẽ có các chỉ tiêu khác nhau theo KpiAssignmentTableSeeder.cs

-- 1. TruongphongKhdn (Table ID = 1) - 8 chỉ tiêu KHDN
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(1, 'Tổng Dư nợ KHDN', 20, 'Tỷ VND', 1),
(1, 'Tỷ lệ nợ xấu KHDN', 10, '%', 2),
(1, 'Thu nợ đã XLRR KHDN', 10, 'Tỷ VND', 3),
(1, 'Lợi nhuận khoán tài chính', 10, 'Tỷ VND', 4),
(1, 'Phát triển khách hàng mới', 10, 'Khách hàng', 5),
(1, 'Điều hành theo chương trình công tác', 20, '%', 6),
(1, 'Chấp hành quy chế, quy trình nghiệp vụ', 10, '%', 7),
(1, 'BQ kết quả thực hiện CB trong phòng mình phụ trách', 10, '%', 8);

-- 2. TruongphongKhcn (Table ID = 2) - 8 chỉ tiêu KHCN
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(2, 'Tổng Dư nợ KHCN', 20, 'Tỷ VND', 1),
(2, 'Tỷ lệ nợ xấu KHCN', 10, '%', 2),
(2, 'Thu nợ đã XLRR KHCN', 10, 'Tỷ VND', 3),
(2, 'Lợi nhuận khoán tài chính', 10, 'Tỷ VND', 4),
(2, 'Phát triển khách hàng mới', 10, 'Khách hàng', 5),
(2, 'Điều hành theo chương trình công tác', 20, '%', 6),
(2, 'Chấp hành quy chế, quy trình nghiệp vụ', 10, '%', 7),
(2, 'BQ kết quả thực hiện CB trong phòng mình phụ trách', 10, '%', 8);

-- 3. PhophongKhdn (Table ID = 3) - 8 chỉ tiêu KHDN
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(3, 'Tổng Dư nợ KHDN', 20, 'Tỷ VND', 1),
(3, 'Tỷ lệ nợ xấu KHDN', 10, '%', 2),
(3, 'Thu nợ đã XLRR KHDN', 10, 'Tỷ VND', 3),
(3, 'Lợi nhuận khoán tài chính', 10, 'Tỷ VND', 4),
(3, 'Phát triển khách hàng mới', 10, 'Khách hàng', 5),
(3, 'Điều hành theo chương trình công tác', 20, '%', 6),
(3, 'Chấp hành quy chế, quy trình nghiệp vụ', 10, '%', 7),
(3, 'BQ kết quả thực hiện CB trong phòng mình phụ trách', 10, '%', 8);

-- 4. PhophongKhcn (Table ID = 4) - 8 chỉ tiêu KHCN
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(4, 'Tổng Dư nợ KHCN', 20, 'Tỷ VND', 1),
(4, 'Tỷ lệ nợ xấu KHCN', 10, '%', 2),
(4, 'Thu nợ đã XLRR KHCN', 10, 'Tỷ VND', 3),
(4, 'Lợi nhuận khoán tài chính', 10, 'Tỷ VND', 4),
(4, 'Phát triển khách hàng mới', 10, 'Khách hàng', 5),
(4, 'Điều hành theo chương trình công tác', 20, '%', 6),
(4, 'Chấp hành quy chế, quy trình nghiệp vụ', 10, '%', 7),
(4, 'BQ kết quả thực hiện CB trong phòng mình phụ trách', 10, '%', 8);

-- 5. TruongphongKhqlrr (Table ID = 5) - 6 chỉ tiêu KHQLRR
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(5, 'Tổng nguồn vốn', 10, 'Tỷ VND', 1),
(5, 'Tổng dư nợ', 10, 'Tỷ VND', 2),
(5, 'Lợi nhuận khoán tài chính', 10, 'Tỷ VND', 3),
(5, 'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 50, '%', 4),
(5, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 5),
(5, 'Kết quả thực hiện BQ của CB trong phòng', 10, '%', 6);

-- 6. PhophongKhqlrr (Table ID = 6) - 6 chỉ tiêu KHQLRR
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(6, 'Tổng nguồn vốn', 10, 'Tỷ VND', 1),
(6, 'Tổng dư nợ', 10, 'Tỷ VND', 2),
(6, 'Lợi nhuận khoán tài chính', 10, 'Tỷ VND', 3),
(6, 'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 50, '%', 4),
(6, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 5),
(6, 'Kết quả thực hiện BQ của CB trong phòng', 10, '%', 6);

-- 7. Cbtd (Table ID = 7) - 8 chỉ tiêu CBTD
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex) VALUES
(7, 'Tổng dư nợ BQ', 30, 'Tỷ VND', 1),
(7, 'Tỷ lệ nợ xấu', 15, '%', 2),
(7, 'Phát triển khách hàng mới', 10, 'Khách hàng', 3),
(7, 'Thu nợ đã XLRR (nếu không có nợ XLRR thì cộng vào chỉ tiêu Dư nợ)', 10, 'Tỷ VND', 4),
(7, 'Thực hiện nhiệm vụ theo chương trình công tác', 10, '%', 5),
(7, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 6),
(7, 'Tổng nguồn vốn huy động BQ', 10, 'Tỷ VND', 7),
(7, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 8);
