-- Tiếp tục cập nhật các chỉ tiêu KPI (Part 2)

-- 8. TruongphongKtnqCnl1 (TableId = 8)
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate) VALUES
(8, 'Tổng nguồn vốn', 'Tổng nguồn vốn', 10.0, 'Tỷ VND', 1, 'NUMBER', 1, datetime('now')),
(8, 'Lợi nhuận khoán tài chính', 'Lợi nhuận khoán tài chính', 20.0, 'Tỷ VND', 2, 'NUMBER', 1, datetime('now')),
(8, 'Thu dịch vụ thanh toán trong nước', 'Thu dịch vụ thanh toán trong nước', 10.0, 'Tỷ VND', 3, 'NUMBER', 1, datetime('now')),
(8, 'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 40.0, '%', 4, 'PERCENTAGE', 1, datetime('now')),
(8, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.0, '%', 5, 'PERCENTAGE', 1, datetime('now')),
(8, 'Kết quả thực hiện BQ của CB trong phòng', 'Kết quả thực hiện bình quân của cán bộ trong phòng', 10.0, '%', 6, 'PERCENTAGE', 1, datetime('now'));

-- 9. PhophongKtnqCnl1 (TableId = 9)
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate) VALUES
(9, 'Tổng nguồn vốn', 'Tổng nguồn vốn', 10.0, 'Tỷ VND', 1, 'NUMBER', 1, datetime('now')),
(9, 'Lợi nhuận khoán tài chính', 'Lợi nhuận khoán tài chính', 20.0, 'Tỷ VND', 2, 'NUMBER', 1, datetime('now')),
(9, 'Thu dịch vụ thanh toán trong nước', 'Thu dịch vụ thanh toán trong nước', 10.0, 'Tỷ VND', 3, 'NUMBER', 1, datetime('now')),
(9, 'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 40.0, '%', 4, 'PERCENTAGE', 1, datetime('now')),
(9, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.0, '%', 5, 'PERCENTAGE', 1, datetime('now')),
(9, 'Kết quả thực hiện BQ của CB thuộc mình phụ trách', 'Kết quả thực hiện bình quân của cán bộ thuộc mình phụ trách', 10.0, '%', 6, 'PERCENTAGE', 1, datetime('now'));

-- 10. Gdv (TableId = 10)
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate) VALUES
(10, 'Số bút toán giao dịch BQ', 'Số bút toán giao dịch bình quân', 50.0, 'BT', 1, 'NUMBER', 1, datetime('now')),
(10, 'Số bút toán hủy', 'Số bút toán hủy', 15.0, 'BT', 2, 'NUMBER', 1, datetime('now')),
(10, 'Thực hiện chức năng, nhiệm vụ được giao', 'Thực hiện chức năng, nhiệm vụ được giao', 10.0, '%', 3, 'PERCENTAGE', 1, datetime('now')),
(10, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.0, '%', 4, 'PERCENTAGE', 1, datetime('now')),
(10, 'Tổng nguồn vốn huy động BQ', 'Tổng nguồn vốn huy động bình quân', 10.0, 'Tỷ VND', 5, 'NUMBER', 1, datetime('now')),
(10, 'Hoàn thành chỉ tiêu giao khoán SPDV', 'Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ', 5.0, '%', 6, 'PERCENTAGE', 1, datetime('now'));

-- 11. TqHkKtnb (TableId = 11) - Chưa có thông tin cụ thể, giữ nguyên tạm thời
-- Sẽ được cập nhật sau khi có thông tin từ TP KTNQ/Giám đốc CN loại 2

-- 12. TruongphoItThKtgs (TableId = 12)
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate) VALUES
(12, 'Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng', 'Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng', 65.0, '%', 1, 'PERCENTAGE', 1, datetime('now')),
(12, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.0, '%', 2, 'PERCENTAGE', 1, datetime('now')),
(12, 'Tổng nguồn vốn huy động BQ', 'Tổng nguồn vốn huy động bình quân', 10.0, 'Tỷ VND', 3, 'NUMBER', 1, datetime('now')),
(12, 'Hoàn thành chỉ tiêu giao khoán SPDV', 'Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ', 5.0, '%', 4, 'PERCENTAGE', 1, datetime('now')),
(12, 'Kết quả thực hiện BQ của cán bộ trong phòng', 'Kết quả thực hiện bình quân của cán bộ trong phòng', 10.0, '%', 5, 'PERCENTAGE', 1, datetime('now'));

-- 13. CBItThKtgsKhqlrr (TableId = 13)
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate) VALUES
(13, 'Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ được giao', 'Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ được giao', 75.0, '%', 1, 'PERCENTAGE', 1, datetime('now')),
(13, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.0, '%', 2, 'PERCENTAGE', 1, datetime('now')),
(13, 'Tổng nguồn vốn huy động BQ', 'Tổng nguồn vốn huy động bình quân', 10.0, 'Tỷ VND', 3, 'NUMBER', 1, datetime('now')),
(13, 'Hoàn thành chỉ tiêu giao khoán SPDV', 'Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ', 5.0, '%', 4, 'PERCENTAGE', 1, datetime('now'));

-- 14. GiamdocPgd (TableId = 14)
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate) VALUES
(14, 'Tổng nguồn vốn BQ', 'Tổng nguồn vốn bình quân', 15.0, 'Tỷ VND', 1, 'NUMBER', 1, datetime('now')),
(14, 'Tổng dư nợ BQ', 'Tổng dư nợ bình quân', 15.0, 'Tỷ VND', 2, 'NUMBER', 1, datetime('now')),
(14, 'Tỷ lệ nợ xấu', 'Tỷ lệ nợ xấu', 10.0, '%', 3, 'PERCENTAGE', 1, datetime('now')),
(14, 'Phát triển Khách hàng', 'Phát triển khách hàng', 10.0, 'Khách hàng', 4, 'NUMBER', 1, datetime('now')),
(14, 'Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)', 'Thu nợ đã xử lý rủi ro', 5.0, 'Tỷ VND', 5, 'NUMBER', 1, datetime('now')),
(14, 'Thu dịch vụ', 'Thu dịch vụ', 10.0, 'Tỷ VND', 6, 'NUMBER', 1, datetime('now')),
(14, 'Lợi nhuận khoán tài chính', 'Lợi nhuận khoán tài chính', 15.0, 'Tỷ VND', 7, 'NUMBER', 1, datetime('now')),
(14, 'Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 'Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.0, '%', 8, 'PERCENTAGE', 1, datetime('now')),
(14, 'BQ kết quả thực hiện của CB trong phòng', 'Bình quân kết quả thực hiện của cán bộ trong phòng', 10.0, '%', 9, 'PERCENTAGE', 1, datetime('now'));

-- 15. PhogiamdocPgd (TableId = 15)
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate) VALUES
(15, 'Tổng nguồn vốn BQ', 'Tổng nguồn vốn bình quân', 15.0, 'Tỷ VND', 1, 'NUMBER', 1, datetime('now')),
(15, 'Tổng dư nợ BQ', 'Tổng dư nợ bình quân', 15.0, 'Tỷ VND', 2, 'NUMBER', 1, datetime('now')),
(15, 'Tỷ lệ nợ xấu', 'Tỷ lệ nợ xấu', 10.0, '%', 3, 'PERCENTAGE', 1, datetime('now')),
(15, 'Phát triển Khách hàng', 'Phát triển khách hàng', 10.0, 'Khách hàng', 4, 'NUMBER', 1, datetime('now')),
(15, 'Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)', 'Thu nợ đã xử lý rủi ro', 5.0, 'Tỷ VND', 5, 'NUMBER', 1, datetime('now')),
(15, 'Thu dịch vụ', 'Thu dịch vụ', 10.0, 'Tỷ VND', 6, 'NUMBER', 1, datetime('now')),
(15, 'Lợi nhuận khoán tài chính', 'Lợi nhuận khoán tài chính', 15.0, 'Tỷ VND', 7, 'NUMBER', 1, datetime('now')),
(15, 'Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 'Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.0, '%', 8, 'PERCENTAGE', 1, datetime('now')),
(15, 'BQ kết quả thực hiện của CB trong phòng', 'Bình quân kết quả thực hiện của cán bộ trong phòng', 10.0, '%', 9, 'PERCENTAGE', 1, datetime('now'));

-- 16. PhogiamdocPgdCbtd (TableId = 16)
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate) VALUES
(16, 'Tổng dư nợ BQ', 'Tổng dư nợ bình quân', 30.0, 'Tỷ VND', 1, 'NUMBER', 1, datetime('now')),
(16, 'Tỷ lệ nợ xấu', 'Tỷ lệ nợ xấu', 15.0, '%', 2, 'PERCENTAGE', 1, datetime('now')),
(16, 'Phát triển Khách hàng', 'Phát triển khách hàng', 10.0, 'Khách hàng', 3, 'NUMBER', 1, datetime('now')),
(16, 'Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)', 'Thu nợ đã xử lý rủi ro', 10.0, 'Tỷ VND', 4, 'NUMBER', 1, datetime('now')),
(16, 'Thực hiện nhiệm vụ theo chương trình công tác', 'Thực hiện nhiệm vụ theo chương trình công tác', 10.0, '%', 5, 'PERCENTAGE', 1, datetime('now')),
(16, 'Tổng nguồn vốn huy động BQ', 'Tổng nguồn vốn huy động bình quân', 10.0, 'Tỷ VND', 6, 'NUMBER', 1, datetime('now')),
(16, 'Hoàn thành chỉ tiêu giao khoán SPDV', 'Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ', 5.0, '%', 7, 'PERCENTAGE', 1, datetime('now')),
(16, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.0, '%', 8, 'PERCENTAGE', 1, datetime('now'));
