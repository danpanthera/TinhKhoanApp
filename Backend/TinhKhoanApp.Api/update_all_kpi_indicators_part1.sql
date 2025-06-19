-- Script cập nhật chỉ tiêu KPI theo yêu cầu mới
-- Xóa tất cả chỉ tiêu hiện tại và tạo lại theo đúng yêu cầu

-- Backup dữ liệu chỉ tiêu cũ
CREATE TABLE IF NOT EXISTS KpiIndicators_Backup AS SELECT * FROM KpiIndicators WHERE TableId <= 23;

-- Xóa chỉ tiêu cũ của 23 bảng cán bộ
DELETE FROM KpiIndicators WHERE TableId <= 23;

-- 1. TruongphongKhdn (TableId = 1)
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate) VALUES
(1, 'Tổng Dư nợ KHDN', 'Tổng dư nợ khách hàng doanh nghiệp', 20.0, 'Tỷ VND', 1, 'NUMBER', 1, datetime('now')),
(1, 'Tỷ lệ nợ xấu KHDN', 'Tỷ lệ nợ xấu khách hàng doanh nghiệp', 10.0, '%', 2, 'PERCENTAGE', 1, datetime('now')),
(1, 'Thu nợ đã XLRR KHDN', 'Thu nợ đã xử lý rủi ro khách hàng doanh nghiệp', 10.0, 'Tỷ VND', 3, 'NUMBER', 1, datetime('now')),
(1, 'Lợi nhuận khoán tài chính', 'Lợi nhuận khoán tài chính', 10.0, 'Tỷ VND', 4, 'NUMBER', 1, datetime('now')),
(1, 'Phát triển Khách hàng Doanh nghiệp', 'Phát triển khách hàng doanh nghiệp', 10.0, 'Khách hàng', 5, 'NUMBER', 1, datetime('now')),
(1, 'Điều hành theo chương trình công tác', 'Điều hành theo chương trình công tác', 20.0, '%', 6, 'PERCENTAGE', 1, datetime('now')),
(1, 'Chấp hành quy chế, quy trình nghiệp vụ', 'Chấp hành quy chế, quy trình nghiệp vụ', 10.0, '%', 7, 'PERCENTAGE', 1, datetime('now')),
(1, 'BQ kết quả thực hiện CB trong phòng mình phụ trách', 'Bình quân kết quả thực hiện cán bộ trong phòng mình phụ trách', 10.0, '%', 8, 'PERCENTAGE', 1, datetime('now'));

-- 2. TruongphongKhcn (TableId = 2)
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate) VALUES
(2, 'Tổng Dư nợ KHCN', 'Tổng dư nợ khách hàng cá nhân', 20.0, 'Tỷ VND', 1, 'NUMBER', 1, datetime('now')),
(2, 'Tỷ lệ nợ xấu KHCN', 'Tỷ lệ nợ xấu khách hàng cá nhân', 10.0, '%', 2, 'PERCENTAGE', 1, datetime('now')),
(2, 'Thu nợ đã XLRR KHCN', 'Thu nợ đã xử lý rủi ro khách hàng cá nhân', 10.0, 'Tỷ VND', 3, 'NUMBER', 1, datetime('now')),
(2, 'Lợi nhuận khoán tài chính', 'Lợi nhuận khoán tài chính', 10.0, 'Tỷ VND', 4, 'NUMBER', 1, datetime('now')),
(2, 'Phát triển Khách hàng Cá nhân', 'Phát triển khách hàng cá nhân', 10.0, 'Khách hàng', 5, 'NUMBER', 1, datetime('now')),
(2, 'Điều hành theo chương trình công tác', 'Điều hành theo chương trình công tác', 20.0, '%', 6, 'PERCENTAGE', 1, datetime('now')),
(2, 'Chấp hành quy chế, quy trình nghiệp vụ', 'Chấp hành quy chế, quy trình nghiệp vụ', 10.0, '%', 7, 'PERCENTAGE', 1, datetime('now')),
(2, 'BQ kết quả thực hiện CB trong phòng mình phụ trách', 'Bình quân kết quả thực hiện cán bộ trong phòng mình phụ trách', 10.0, '%', 8, 'PERCENTAGE', 1, datetime('now'));

-- 3. PhophongKhdn (TableId = 3)
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate) VALUES
(3, 'Tổng Dư nợ KHDN', 'Tổng dư nợ khách hàng doanh nghiệp', 20.0, 'Tỷ VND', 1, 'NUMBER', 1, datetime('now')),
(3, 'Tỷ lệ nợ xấu KHDN', 'Tỷ lệ nợ xấu khách hàng doanh nghiệp', 10.0, '%', 2, 'PERCENTAGE', 1, datetime('now')),
(3, 'Thu nợ đã XLRR KHDN', 'Thu nợ đã xử lý rủi ro khách hàng doanh nghiệp', 10.0, 'Tỷ VND', 3, 'NUMBER', 1, datetime('now')),
(3, 'Lợi nhuận khoán tài chính', 'Lợi nhuận khoán tài chính', 10.0, 'Tỷ VND', 4, 'NUMBER', 1, datetime('now')),
(3, 'Phát triển Khách hàng Doanh nghiệp', 'Phát triển khách hàng doanh nghiệp', 10.0, 'Khách hàng', 5, 'NUMBER', 1, datetime('now')),
(3, 'Điều hành theo chương trình công tác', 'Điều hành theo chương trình công tác', 20.0, '%', 6, 'PERCENTAGE', 1, datetime('now')),
(3, 'Chấp hành quy chế, quy trình nghiệp vụ', 'Chấp hành quy chế, quy trình nghiệp vụ', 10.0, '%', 7, 'PERCENTAGE', 1, datetime('now')),
(3, 'BQ kết quả thực hiện CB trong phòng mình phụ trách', 'Bình quân kết quả thực hiện cán bộ trong phòng mình phụ trách', 10.0, '%', 8, 'PERCENTAGE', 1, datetime('now'));

-- 4. PhophongKhcn (TableId = 4)
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate) VALUES
(4, 'Tổng Dư nợ KHCN', 'Tổng dư nợ khách hàng cá nhân', 20.0, 'Tỷ VND', 1, 'NUMBER', 1, datetime('now')),
(4, 'Tỷ lệ nợ xấu KHCN', 'Tỷ lệ nợ xấu khách hàng cá nhân', 10.0, '%', 2, 'PERCENTAGE', 1, datetime('now')),
(4, 'Thu nợ đã XLRR KHCN', 'Thu nợ đã xử lý rủi ro khách hàng cá nhân', 10.0, 'Tỷ VND', 3, 'NUMBER', 1, datetime('now')),
(4, 'Lợi nhuận khoán tài chính', 'Lợi nhuận khoán tài chính', 10.0, 'Tỷ VND', 4, 'NUMBER', 1, datetime('now')),
(4, 'Phát triển Khách hàng Cá nhân', 'Phát triển khách hàng cá nhân', 10.0, 'Khách hàng', 5, 'NUMBER', 1, datetime('now')),
(4, 'Điều hành theo chương trình công tác', 'Điều hành theo chương trình công tác', 20.0, '%', 6, 'PERCENTAGE', 1, datetime('now')),
(4, 'Chấp hành quy chế, quy trình nghiệp vụ', 'Chấp hành quy chế, quy trình nghiệp vụ', 10.0, '%', 7, 'PERCENTAGE', 1, datetime('now')),
(4, 'BQ kết quả thực hiện CB trong phòng mình phụ trách', 'Bình quân kết quả thực hiện cán bộ trong phòng mình phụ trách', 10.0, '%', 8, 'PERCENTAGE', 1, datetime('now'));

-- 5. TruongphongKhqlrr (TableId = 5)
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate) VALUES
(5, 'Tổng nguồn vốn', 'Tổng nguồn vốn', 10.0, 'Tỷ VND', 1, 'NUMBER', 1, datetime('now')),
(5, 'Tổng dư nợ', 'Tổng dư nợ', 10.0, 'Tỷ VND', 2, 'NUMBER', 1, datetime('now')),
(5, 'Lợi nhuận khoán tài chính', 'Lợi nhuận khoán tài chính', 10.0, 'Tỷ VND', 3, 'NUMBER', 1, datetime('now')),
(5, 'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 50.0, '%', 4, 'PERCENTAGE', 1, datetime('now')),
(5, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.0, '%', 5, 'PERCENTAGE', 1, datetime('now')),
(5, 'Kết quả thực hiện BQ của CB trong phòng', 'Kết quả thực hiện bình quân của cán bộ trong phòng', 10.0, '%', 6, 'PERCENTAGE', 1, datetime('now'));

-- 6. PhophongKhqlrr (TableId = 6)
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate) VALUES
(6, 'Tổng nguồn vốn', 'Tổng nguồn vốn', 10.0, 'Tỷ VND', 1, 'NUMBER', 1, datetime('now')),
(6, 'Tổng dư nợ', 'Tổng dư nợ', 10.0, 'Tỷ VND', 2, 'NUMBER', 1, datetime('now')),
(6, 'Lợi nhuận khoán tài chính', 'Lợi nhuận khoán tài chính', 10.0, 'Tỷ VND', 3, 'NUMBER', 1, datetime('now')),
(6, 'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 50.0, '%', 4, 'PERCENTAGE', 1, datetime('now')),
(6, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.0, '%', 5, 'PERCENTAGE', 1, datetime('now')),
(6, 'Kết quả thực hiện BQ của CB trong phòng mình phụ trách', 'Kết quả thực hiện bình quân của cán bộ trong phòng mình phụ trách', 10.0, '%', 6, 'PERCENTAGE', 1, datetime('now'));

-- 7. Cbtd (TableId = 7)
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate) VALUES
(7, 'Tổng dư nợ BQ', 'Tổng dư nợ bình quân', 30.0, 'Tỷ VND', 1, 'NUMBER', 1, datetime('now')),
(7, 'Tỷ lệ nợ xấu', 'Tỷ lệ nợ xấu', 15.0, '%', 2, 'PERCENTAGE', 1, datetime('now')),
(7, 'Phát triển Khách hàng', 'Phát triển khách hàng', 10.0, 'Khách hàng', 3, 'NUMBER', 1, datetime('now')),
(7, 'Thu nợ đã XLRR (nếu không có nợ XLRR thì cộng vào chỉ tiêu Dư nợ)', 'Thu nợ đã xử lý rủi ro', 10.0, 'Tỷ VND', 4, 'NUMBER', 1, datetime('now')),
(7, 'Thực hiện nhiệm vụ theo chương trình công tác', 'Thực hiện nhiệm vụ theo chương trình công tác', 10.0, '%', 5, 'PERCENTAGE', 1, datetime('now')),
(7, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.0, '%', 6, 'PERCENTAGE', 1, datetime('now')),
(7, 'Tổng nguồn vốn huy động BQ', 'Tổng nguồn vốn huy động bình quân', 10.0, 'Tỷ VND', 7, 'NUMBER', 1, datetime('now')),
(7, 'Hoàn thành chỉ tiêu giao khoán SPDV', 'Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ', 5.0, '%', 8, 'PERCENTAGE', 1, datetime('now'));
