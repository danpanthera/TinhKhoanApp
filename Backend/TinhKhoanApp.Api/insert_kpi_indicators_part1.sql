-- ===============================================
-- INSERT CHÍNH XÁC CHỈ TIÊU KPI THEO YÊU CẦU
-- Date: August 8, 2025
-- Total: 181 chỉ tiêu (158 CANBO + 99 CHINHANH sẽ thêm sau)
-- ===============================================

-- Lấy danh sách TableId cho việc mapping
DECLARE @TableIds TABLE (TableId INT, TableName VARCHAR(255))
INSERT INTO @TableIds
SELECT Id, TableName FROM KpiAssignmentTables;

-- ============= 23 BẢNG CANBO - 158 CHỈ TIÊU =============

-- 1. TruongphongKhdn (8 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.TableId, 'Tổng Dư nợ KHDN', 20.00, 'Triệu VND', 1, 2, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhdn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tỷ lệ nợ xấu KHDN', 10.00, '%', 2, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhdn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thu nợ đã XLRR KHDN', 10.00, 'Triệu VND', 3, 2, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhdn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Lợi nhuận khoán tài chính', 10.00, 'Triệu VND', 4, 2, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhdn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Phát triển Khách hàng Doanh nghiệp', 10.00, 'Khách hàng', 5, 3, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhdn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Điều hành theo chương trình công tác', 20.00, '%', 6, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhdn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Chấp hành quy chế, quy trình nghiệp vụ', 10.00, '%', 7, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhdn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'BQ kết quả thực hiện CB trong phòng mình phụ trách', 10.00, '%', 8, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhdn_KPI_Assignment'

-- 2. TruongphongKhcn (8 chỉ tiêu)
UNION ALL
SELECT t.TableId, 'Tổng Dư nợ KHCN', 20.00, 'Triệu VND', 1, 2, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhcn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tỷ lệ nợ xấu KHCN', 10.00, '%', 2, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhcn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thu nợ đã XLRR KHCN', 10.00, 'Triệu VND', 3, 2, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhcn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Lợi nhuận khoán tài chính', 10.00, 'Triệu VND', 4, 2, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhcn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Phát triển Khách hàng Cá nhân', 10.00, 'Khách hàng', 5, 3, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhcn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Điều hành theo chương trình công tác', 20.00, '%', 6, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhcn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Chấp hành quy chế, quy trình nghiệp vụ', 10.00, '%', 7, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhcn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'BQ kết quả thực hiện CB trong phòng mình phụ trách', 10.00, '%', 8, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhcn_KPI_Assignment'

-- 3. PhophongKhdn (8 chỉ tiêu)
UNION ALL
SELECT t.TableId, 'Tổng Dư nợ KHDN', 20.00, 'Triệu VND', 1, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhdn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tỷ lệ nợ xấu KHDN', 10.00, '%', 2, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhdn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thu nợ đã XLRR KHDN', 10.00, 'Triệu VND', 3, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhdn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Lợi nhuận khoán tài chính', 10.00, 'Triệu VND', 4, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhdn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Phát triển Khách hàng Doanh nghiệp', 10.00, 'Khách hàng', 5, 3, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhdn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Điều hành theo chương trình công tác', 20.00, '%', 6, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhdn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Chấp hành quy chế, quy trình nghiệp vụ', 10.00, '%', 7, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhdn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'BQ kết quả thực hiện CB trong phòng mình phụ trách', 10.00, '%', 8, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhdn_KPI_Assignment'

-- 4. PhophongKhcn (8 chỉ tiêu)
UNION ALL
SELECT t.TableId, 'Tổng Dư nợ KHCN', 20.00, 'Triệu VND', 1, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhcn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tỷ lệ nợ xấu KHCN', 10.00, '%', 2, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhcn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thu nợ đã XLRR KHCN', 10.00, 'Triệu VND', 3, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhcn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Lợi nhuận khoán tài chính', 10.00, 'Triệu VND', 4, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhcn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Phát triển Khách hàng Cá nhân', 10.00, 'Khách hàng', 5, 3, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhcn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Điều hành theo chương trình công tác', 20.00, '%', 6, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhcn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Chấp hành quy chế, quy trình nghiệp vụ', 10.00, '%', 7, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhcn_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'BQ kết quả thực hiện CB trong phòng mình phụ trách', 10.00, '%', 8, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhcn_KPI_Assignment'

-- 5. TruongphongKhqlrr (6 chỉ tiêu)
UNION ALL
SELECT t.TableId, 'Tổng nguồn vốn', 10.00, 'Triệu VND', 1, 2, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tổng dư nợ', 10.00, 'Triệu VND', 2, 2, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Lợi nhuận khoán tài chính', 10.00, 'Triệu VND', 3, 2, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 50.00, '%', 4, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, '%', 5, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Kết quả thực hiện BQ của CB trong phòng', 10.00, '%', 6, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKhqlrr_KPI_Assignment'

-- 6. PhophongKhqlrr (6 chỉ tiêu)
UNION ALL
SELECT t.TableId, 'Tổng nguồn vốn', 10.00, 'Triệu VND', 1, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tổng dư nợ', 10.00, 'Triệu VND', 2, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Lợi nhuận khoán tài chính', 10.00, 'Triệu VND', 3, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 50.00, '%', 4, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, '%', 5, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Kết quả thực hiện BQ của CB trong phòng mình phụ trách', 10.00, '%', 6, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKhqlrr_KPI_Assignment'

-- 7. Cbtd (8 chỉ tiêu)
UNION ALL
SELECT t.TableId, 'Tổng dư nợ BQ', 30.00, 'Triệu VND', 1, 2, 1
FROM @TableIds t WHERE t.TableName = 'Cbtd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tỷ lệ nợ xấu', 15.00, '%', 2, 1, 1
FROM @TableIds t WHERE t.TableName = 'Cbtd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Phát triển Khách hàng', 10.00, 'Khách hàng', 3, 3, 1
FROM @TableIds t WHERE t.TableName = 'Cbtd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thu nợ đã XLRR (nếu không có nợ XLRR thì cộng vào chỉ tiêu Dư nợ)', 10.00, 'Triệu VND', 4, 2, 1
FROM @TableIds t WHERE t.TableName = 'Cbtd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, '%', 5, 1, 1
FROM @TableIds t WHERE t.TableName = 'Cbtd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, '%', 6, 1, 1
FROM @TableIds t WHERE t.TableName = 'Cbtd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tổng nguồn vốn huy động BQ', 10.00, 'Triệu VND', 7, 2, 1
FROM @TableIds t WHERE t.TableName = 'Cbtd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, '%', 8, 1, 1
FROM @TableIds t WHERE t.TableName = 'Cbtd_KPI_Assignment';

PRINT 'Đã thêm 52 chỉ tiêu cho 7 bảng đầu tiên. Tiếp tục với các bảng còn lại...'
