-- ===============================================
-- INSERT CHÍNH XÁC CHỈ TIÊU KPI - PART 3
-- Date: August 8, 2025
-- Tiếp tục từ bảng 17-23 (7 bảng CANBO cuối) + 9 bảng CHINHANH
-- ===============================================

-- Lấy danh sách TableId cho việc mapping
DECLARE @TableIds TABLE (TableId INT, TableName VARCHAR(255))
INSERT INTO @TableIds
SELECT Id, TableName FROM KpiAssignmentTables;

-- ============= 7 BẢNG CANBO CUỐI =============

-- 17. GiamdocCnl2 (11 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.TableId, 'Tổng nguồn vốn cuối kỳ', 5.00, 'Triệu VND', 1, 2, 1
FROM @TableIds t WHERE t.TableName = 'GiamdocCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tổng nguồn vốn huy động BQ trong kỳ', 10.00, 'Triệu VND', 2, 2, 1
FROM @TableIds t WHERE t.TableName = 'GiamdocCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tổng dư nợ cuối kỳ', 5.00, 'Triệu VND', 3, 2, 1
FROM @TableIds t WHERE t.TableName = 'GiamdocCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tổng dư nợ BQ trong kỳ', 10.00, 'Triệu VND', 4, 2, 1
FROM @TableIds t WHERE t.TableName = 'GiamdocCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tổng dư nợ HSX&CN', 5.00, 'Triệu VND', 5, 2, 1
FROM @TableIds t WHERE t.TableName = 'GiamdocCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tỷ lệ nợ xấu nội bảng', 10.00, '%', 6, 1, 1
FROM @TableIds t WHERE t.TableName = 'GiamdocCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thu nợ đã XLRR', 5.00, 'Triệu VND', 7, 2, 1
FROM @TableIds t WHERE t.TableName = 'GiamdocCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Phát triển Khách hàng', 10.00, 'Khách hàng', 8, 3, 1
FROM @TableIds t WHERE t.TableName = 'GiamdocCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Lợi nhuận khoán tài chính', 20.00, 'Triệu VND', 9, 2, 1
FROM @TableIds t WHERE t.TableName = 'GiamdocCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thu dịch vụ', 10.00, 'Triệu VND', 10, 2, 1
FROM @TableIds t WHERE t.TableName = 'GiamdocCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10.00, '%', 11, 1, 1
FROM @TableIds t WHERE t.TableName = 'GiamdocCnl2_KPI_Assignment'

-- 18. PhogiamdocCnl2Td (8 chỉ tiêu)
UNION ALL
SELECT t.TableId, 'Tổng dư nợ cho vay', 20.00, 'Triệu VND', 1, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocCnl2Td_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tổng dư nợ cho vay HSX&CN', 10.00, 'Triệu VND', 2, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocCnl2Td_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thu nợ đã xử lý', 10.00, 'Triệu VND', 3, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocCnl2Td_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Lợi nhuận khoán tài chính', 20.00, 'Triệu VND', 4, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocCnl2Td_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tỷ lệ nợ xấu', 10.00, '%', 5, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocCnl2Td_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Phát triển Khách hàng', 10.00, 'Khách hàng', 6, 3, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocCnl2Td_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Điều hành theo chương trình công tác, nhiệm vụ được giao', 10.00, '%', 7, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocCnl2Td_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, '%', 8, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocCnl2Td_KPI_Assignment'

-- 19. PhogiamdocCnl2Kt (6 chỉ tiêu) - Sửa lại Unit cho chỉ tiêu 6
UNION ALL
SELECT t.TableId, 'Tổng nguồn vốn', 20.00, 'Triệu VND', 1, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocCnl2Kt_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Lợi nhuận khoán tài chính', 30.00, 'Triệu VND', 2, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocCnl2Kt_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tổng doanh thu phí dịch vụ', 20.00, 'Triệu VND', 3, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocCnl2Kt_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Số thẻ phát hành', 10.00, 'cái', 4, 3, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocCnl2Kt_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Điều hành theo chương trình công tác, nhiệm vụ được giao', 10.00, '%', 5, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocCnl2Kt_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, '%', 6, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocCnl2Kt_KPI_Assignment'

-- 20. TruongphongKhCnl2 (9 chỉ tiêu)
UNION ALL
SELECT t.TableId, 'Tổng dư nợ', 20.00, 'Triệu VND', 1, 2, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tỷ lệ nợ xấu', 15.00, '%', 2, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Phát triển Khách hàng', 10.00, 'Khách hàng', 3, 3, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thu nợ đã XLRR', 10.00, 'Triệu VND', 4, 2, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Điều hành theo chương trình công tác', 10.00, '%', 5, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, '%', 6, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tổng nguồn vốn huy động BQ', 10.00, 'Triệu VND', 7, 2, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, '%', 8, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Kết quả thực hiện BQ của CB trong phòng', 10.00, '%', 9, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhCnl2_KPI_Assignment'

-- 21. PhophongKhCnl2 (8 chỉ tiêu)
UNION ALL
SELECT t.TableId, 'Tổng dư nợ BQ', 30.00, 'Triệu VND', 1, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tỷ lệ nợ xấu', 15.00, '%', 2, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Phát triển Khách hàng', 10.00, 'Khách hàng', 3, 3, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thu nợ đã XLRR', 10.00, 'Triệu VND', 4, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, '%', 5, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, '%', 6, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tổng nguồn vốn huy động BQ', 10.00, 'Triệu VND', 7, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, '%', 8, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhCnl2_KPI_Assignment'

-- 22. TruongphongKtnqCnl2 (6 chỉ tiêu)
UNION ALL
SELECT t.TableId, 'Tổng nguồn vốn', 10.00, 'Triệu VND', 1, 2, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKtnqCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Lợi nhuận khoán tài chính', 20.00, 'Triệu VND', 2, 2, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKtnqCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thu dịch vụ thanh toán trong nước', 10.00, 'Triệu VND', 3, 2, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKtnqCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng', 40.00, '%', 4, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKtnqCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, '%', 5, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKtnqCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Kết quả thực hiện BQ của CB trong phòng', 10.00, '%', 6, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKtnqCnl2_KPI_Assignment'

-- 23. PhophongKtnqCnl2 (5 chỉ tiêu)
UNION ALL
SELECT t.TableId, 'Số bút toán giao dịch BQ', 40.00, 'BT', 1, 3, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKtnqCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Số bút toán hủy', 20.00, 'BT', 2, 3, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKtnqCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thực hiện nhiệm vụ theo chương trình công tác', 25.00, '%', 3, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKtnqCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, '%', 4, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKtnqCnl2_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, '%', 5, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKtnqCnl2_KPI_Assignment';

-- ============= 9 BẢNG CHINHANH (MỖI BẢNG 11 CHỈ TIÊU GIỐNG GiamdocCnl2) =============

-- 9 bảng CHINHANH với 11 chỉ tiêu mỗi bảng (giống GiamdocCnl2)
DECLARE @ChinhanhTables TABLE (TableName VARCHAR(255))
INSERT INTO @ChinhanhTables VALUES
('HoiSo_KPI_Assignment'),
('CnBinhLu_KPI_Assignment'),
('CnPhongTho_KPI_Assignment'),
('CnSinHo_KPI_Assignment'),
('CnBumTo_KPI_Assignment'),
('CnThanUyen_KPI_Assignment'),
('CnDoanKet_KPI_Assignment'),
('CnTanUyen_KPI_Assignment'),
('CnNamHang_KPI_Assignment');

-- Thêm 11 chỉ tiêu cho mỗi bảng CHINHANH
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.TableId, 'Tổng nguồn vốn cuối kỳ', 5.00, 'Triệu VND', 1, 2, 1
FROM @TableIds t INNER JOIN @ChinhanhTables ct ON t.TableName = ct.TableName
UNION ALL
SELECT t.TableId, 'Tổng nguồn vốn huy động BQ trong kỳ', 10.00, 'Triệu VND', 2, 2, 1
FROM @TableIds t INNER JOIN @ChinhanhTables ct ON t.TableName = ct.TableName
UNION ALL
SELECT t.TableId, 'Tổng dư nợ cuối kỳ', 5.00, 'Triệu VND', 3, 2, 1
FROM @TableIds t INNER JOIN @ChinhanhTables ct ON t.TableName = ct.TableName
UNION ALL
SELECT t.TableId, 'Tổng dư nợ BQ trong kỳ', 10.00, 'Triệu VND', 4, 2, 1
FROM @TableIds t INNER JOIN @ChinhanhTables ct ON t.TableName = ct.TableName
UNION ALL
SELECT t.TableId, 'Tổng dư nợ HSX&CN', 5.00, 'Triệu VND', 5, 2, 1
FROM @TableIds t INNER JOIN @ChinhanhTables ct ON t.TableName = ct.TableName
UNION ALL
SELECT t.TableId, 'Tỷ lệ nợ xấu nội bảng', 10.00, '%', 6, 1, 1
FROM @TableIds t INNER JOIN @ChinhanhTables ct ON t.TableName = ct.TableName
UNION ALL
SELECT t.TableId, 'Thu nợ đã XLRR', 5.00, 'Triệu VND', 7, 2, 1
FROM @TableIds t INNER JOIN @ChinhanhTables ct ON t.TableName = ct.TableName
UNION ALL
SELECT t.TableId, 'Phát triển Khách hàng', 10.00, 'Khách hàng', 8, 3, 1
FROM @TableIds t INNER JOIN @ChinhanhTables ct ON t.TableName = ct.TableName
UNION ALL
SELECT t.TableId, 'Lợi nhuận khoán tài chính', 20.00, 'Triệu VND', 9, 2, 1
FROM @TableIds t INNER JOIN @ChinhanhTables ct ON t.TableName = ct.TableName
UNION ALL
SELECT t.TableId, 'Thu dịch vụ', 10.00, 'Triệu VND', 10, 2, 1
FROM @TableIds t INNER JOIN @ChinhanhTables ct ON t.TableName = ct.TableName
UNION ALL
SELECT t.TableId, 'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10.00, '%', 11, 1, 1
FROM @TableIds t INNER JOIN @ChinhanhTables ct ON t.TableName = ct.TableName;

PRINT 'HOÀN THÀNH: Đã thêm tất cả chỉ tiêu cho 23 bảng CANBO và 9 bảng CHINHANH'
PRINT 'TỔNG: 158 chỉ tiêu CANBO + 99 chỉ tiêu CHINHANH = 257 chỉ tiêu'
