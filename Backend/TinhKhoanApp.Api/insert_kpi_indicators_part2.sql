-- ===============================================
-- INSERT CHÍNH XÁC CHỈ TIÊU KPI - PART 2
-- Date: August 8, 2025
-- Tiếp tục từ bảng 8-16 (9 bảng CANBO)
-- ===============================================

-- Lấy danh sách TableId cho việc mapping
DECLARE @TableIds TABLE (TableId INT, TableName VARCHAR(255))
INSERT INTO @TableIds
SELECT Id, TableName FROM KpiAssignmentTables;

-- 8. TruongphongKtnqCnl1 (6 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.TableId, 'Tổng nguồn vốn', 10.00, 'Triệu VND', 1, 2, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKtnqCnl1_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Lợi nhuận khoán tài chính', 20.00, 'Triệu VND', 2, 2, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKtnqCnl1_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thu dịch vụ thanh toán trong nước', 10.00, 'Triệu VND', 3, 2, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKtnqCnl1_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 40.00, '%', 4, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKtnqCnl1_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, '%', 5, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKtnqCnl1_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Kết quả thực hiện BQ của CB trong phòng', 10.00, '%', 6, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongKtnqCnl1_KPI_Assignment'

-- 9. PhophongKtnqCnl1 (6 chỉ tiêu)
UNION ALL
SELECT t.TableId, 'Tổng nguồn vốn', 10.00, 'Triệu VND', 1, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKtnqCnl1_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Lợi nhuận khoán tài chính', 20.00, 'Triệu VND', 2, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKtnqCnl1_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thu dịch vụ thanh toán trong nước', 10.00, 'Triệu VND', 3, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKtnqCnl1_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 40.00, '%', 4, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKtnqCnl1_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, '%', 5, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKtnqCnl1_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Kết quả thực hiện BQ của CB thuộc mình phụ trách', 10.00, '%', 6, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhophongKtnqCnl1_KPI_Assignment'

-- 10. Gdv (6 chỉ tiêu)
UNION ALL
SELECT t.TableId, 'Số bút toán giao dịch BQ', 50.00, 'BT', 1, 3, 1
FROM @TableIds t WHERE t.TableName = 'Gdv_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Số bút toán hủy', 15.00, 'BT', 2, 3, 1
FROM @TableIds t WHERE t.TableName = 'Gdv_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thực hiện chức năng, nhiệm vụ được giao', 10.00, '%', 3, 1, 1
FROM @TableIds t WHERE t.TableName = 'Gdv_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, '%', 4, 1, 1
FROM @TableIds t WHERE t.TableName = 'Gdv_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tổng nguồn vốn huy động BQ', 10.00, 'Triệu VND', 5, 2, 1
FROM @TableIds t WHERE t.TableName = 'Gdv_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, '%', 6, 1, 1
FROM @TableIds t WHERE t.TableName = 'Gdv_KPI_Assignment'

-- 11. TqHkKtnb (0 chỉ tiêu - sẽ được giao sau)
-- Không có chỉ tiêu - đợi TP KTNQ/Giám đốc CN loại 2 trực tiếp giao sau

-- 12. TruongphongItThKtgs (5 chỉ tiêu)
UNION ALL
SELECT t.TableId, 'Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng', 65.00, '%', 1, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongItThKtgs_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, '%', 2, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongItThKtgs_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tổng nguồn vốn huy động BQ', 10.00, 'Triệu VND', 3, 2, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongItThKtgs_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, '%', 4, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongItThKtgs_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Kết quả thực hiện BQ của cán bộ trong phòng', 10.00, '%', 5, 1, 1
FROM @TableIds t WHERE t.TableName = 'TruongphongItThKtgs_KPI_Assignment'

-- 13. CbItThKtgsKhqlrr (4 chỉ tiêu)
UNION ALL
SELECT t.TableId, 'Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ được giao', 75.00, '%', 1, 1, 1
FROM @TableIds t WHERE t.TableName = 'CbItThKtgsKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, '%', 2, 1, 1
FROM @TableIds t WHERE t.TableName = 'CbItThKtgsKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tổng nguồn vốn huy động BQ', 10.00, 'Triệu VND', 3, 2, 1
FROM @TableIds t WHERE t.TableName = 'CbItThKtgsKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, '%', 4, 1, 1
FROM @TableIds t WHERE t.TableName = 'CbItThKtgsKhqlrr_KPI_Assignment'

-- 14. GiamdocPgd (9 chỉ tiêu)
UNION ALL
SELECT t.TableId, 'Tổng nguồn vốn BQ', 15.00, 'Triệu VND', 1, 2, 1
FROM @TableIds t WHERE t.TableName = 'GiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tổng dư nợ BQ', 15.00, 'Triệu VND', 2, 2, 1
FROM @TableIds t WHERE t.TableName = 'GiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tỷ lệ nợ xấu', 10.00, '%', 3, 1, 1
FROM @TableIds t WHERE t.TableName = 'GiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Phát triển Khách hàng', 10.00, 'Khách hàng', 4, 3, 1
FROM @TableIds t WHERE t.TableName = 'GiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)', 5.00, 'Triệu VND', 5, 2, 1
FROM @TableIds t WHERE t.TableName = 'GiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thu dịch vụ', 10.00, 'Triệu VND', 6, 2, 1
FROM @TableIds t WHERE t.TableName = 'GiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Lợi nhuận khoán tài chính', 15.00, 'Triệu VND', 7, 2, 1
FROM @TableIds t WHERE t.TableName = 'GiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, '%', 8, 1, 1
FROM @TableIds t WHERE t.TableName = 'GiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'BQ kết quả thực hiện của CB trong phòng', 10.00, '%', 9, 1, 1
FROM @TableIds t WHERE t.TableName = 'GiamdocPgd_KPI_Assignment'

-- 15. PhogiamdocPgd (9 chỉ tiêu)
UNION ALL
SELECT t.TableId, 'Tổng nguồn vốn BQ', 15.00, 'Triệu VND', 1, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tổng dư nợ BQ', 15.00, 'Triệu VND', 2, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tỷ lệ nợ xấu', 10.00, '%', 3, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Phát triển Khách hàng', 10.00, 'Khách hàng', 4, 3, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)', 5.00, 'Triệu VND', 5, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thu dịch vụ', 10.00, 'Triệu VND', 6, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Lợi nhuận khoán tài chính', 15.00, 'Triệu VND', 7, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, '%', 8, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'BQ kết quả thực hiện của CB trong phòng', 10.00, '%', 9, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocPgd_KPI_Assignment'

-- 16. PhogiamdocPgdCbtd (8 chỉ tiêu)
UNION ALL
SELECT t.TableId, 'Tổng dư nợ BQ', 30.00, 'Triệu VND', 1, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocPgdCbtd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tỷ lệ nợ xấu', 15.00, '%', 2, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocPgdCbtd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Phát triển Khách hàng', 10.00, 'Khách hàng', 3, 3, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocPgdCbtd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)', 10.00, 'Triệu VND', 4, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocPgdCbtd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, '%', 5, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocPgdCbtd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Tổng nguồn vốn huy động BQ', 10.00, 'Triệu VND', 6, 2, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocPgdCbtd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, '%', 7, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocPgdCbtd_KPI_Assignment'
UNION ALL
SELECT t.TableId, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, '%', 8, 1, 1
FROM @TableIds t WHERE t.TableName = 'PhogiamdocPgdCbtd_KPI_Assignment';

PRINT 'Đã thêm 68 chỉ tiêu cho 9 bảng (8-16). Tổng: 120 chỉ tiêu.'
