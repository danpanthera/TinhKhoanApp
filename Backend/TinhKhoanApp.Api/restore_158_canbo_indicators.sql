-- =====================================================================
-- KHÔI PHỤC 158 CHỈ TIÊU CHUẨN CHO 22 BẢNG KPI CANBO (KHÔNG MOCK DATA)
-- Dựa trên dữ liệu thật từ yêu cầu nghiệp vụ
-- =====================================================================

USE TinhKhoanDB;
GO

-- 1. XÓA TẤT CẢ CHỈ TIÊU CANBO HIỆN TẠI
DELETE i FROM KpiIndicators i
INNER JOIN KpiAssignmentTables t ON i.TableId = t.Id
WHERE t.Category = 'CANBO';

PRINT '🧹 Đã xóa tất cả chỉ tiêu CANBO cũ';

-- 2. KHÔI PHỤC 158 CHỈ TIÊU THẬT CHO 22 BẢNG (TrừBảng TqHkKtnb - tạm thời chưa có)

-- TruongphongKhdn_KPI_Assignment (8 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.Id, 'Tổng Dư nợ KHDN', 20, 'Triệu VND', 1, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhdn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tỷ lệ nợ xấu KHDN', 10, '%', 2, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhdn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thu nợ đã XLRR KHDN', 10, 'Triệu VND', 3, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhdn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Lợi nhuận khoán tài chính', 10, 'Triệu VND', 4, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhdn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Phát triển Khách hàng Doanh nghiệp', 10, 'Khách hàng', 5, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhdn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Điều hành theo chương trình công tác', 20, '%', 6, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhdn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Chấp hành quy chế, quy trình nghiệp vụ', 10, '%', 7, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhdn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'BQ kết quả thực hiện CB trong phòng mình phụ trách', 10, '%', 8, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhdn_KPI_Assignment';

-- TruongphongKhcn_KPI_Assignment (8 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.Id, 'Tổng Dư nợ KHCN', 20, 'Triệu VND', 1, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhcn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tỷ lệ nợ xấu KHCN', 10, '%', 2, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhcn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thu nợ đã XLRR KHCN', 10, 'Triệu VND', 3, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhcn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Lợi nhuận khoán tài chính', 10, 'Triệu VND', 4, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhcn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Phát triển Khách hàng Cá nhân', 10, 'Khách hàng', 5, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhcn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Điều hành theo chương trình công tác', 20, '%', 6, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhcn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Chấp hành quy chế, quy trình nghiệp vụ', 10, '%', 7, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhcn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'BQ kết quả thực hiện CB trong phòng mình phụ trách', 10, '%', 8, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhcn_KPI_Assignment';

-- PhophongKhdn_KPI_Assignment (8 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.Id, 'Tổng Dư nợ KHDN', 20, 'Triệu VND', 1, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhdn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tỷ lệ nợ xấu KHDN', 10, '%', 2, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhdn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thu nợ đã XLRR KHDN', 10, 'Triệu VND', 3, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhdn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Lợi nhuận khoán tài chính', 10, 'Triệu VND', 4, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhdn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Phát triển Khách hàng Doanh nghiệp', 10, 'Khách hàng', 5, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhdn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Điều hành theo chương trình công tác', 20, '%', 6, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhdn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Chấp hành quy chế, quy trình nghiệp vụ', 10, '%', 7, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhdn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'BQ kết quả thực hiện CB trong phòng mình phụ trách', 10, '%', 8, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhdn_KPI_Assignment';

-- PhophongKhcn_KPI_Assignment (8 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.Id, 'Tổng Dư nợ KHCN', 20, 'Triệu VND', 1, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhcn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tỷ lệ nợ xấu KHCN', 10, '%', 2, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhcn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thu nợ đã XLRR KHCN', 10, 'Triệu VND', 3, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhcn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Lợi nhuận khoán tài chính', 10, 'Triệu VND', 4, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhcn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Phát triển Khách hàng Cá nhân', 10, 'Khách hàng', 5, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhcn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Điều hành theo chương trình công tác', 20, '%', 6, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhcn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Chấp hành quy chế, quy trình nghiệp vụ', 10, '%', 7, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhcn_KPI_Assignment'
UNION ALL
SELECT t.Id, 'BQ kết quả thực hiện CB trong phòng mình phụ trách', 10, '%', 8, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhcn_KPI_Assignment';

-- TruongphongKhqlrr_KPI_Assignment (6 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.Id, 'Tổng nguồn vốn', 10, 'Triệu VND', 1, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tổng dư nợ', 10, 'Triệu VND', 2, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Lợi nhuận khoán tài chính', 10, 'Triệu VND', 3, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 50, '%', 4, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 5, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Kết quả thực hiện BQ của CB trong phòng', 10, '%', 6, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhqlrr_KPI_Assignment';

-- PhophongKhqlrr_KPI_Assignment (6 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.Id, 'Tổng nguồn vốn', 10, 'Triệu VND', 1, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tổng dư nợ', 10, 'Triệu VND', 2, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Lợi nhuận khoán tài chính', 10, 'Triệu VND', 3, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 50, '%', 4, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 5, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Kết quả thực hiện BQ của CB trong phòng mình phụ trách', 10, '%', 6, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhqlrr_KPI_Assignment';

-- Cbtd_KPI_Assignment (8 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.Id, 'Tổng dư nợ BQ', 30, 'Triệu VND', 1, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'Cbtd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tỷ lệ nợ xấu', 15, '%', 2, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'Cbtd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Phát triển Khách hàng', 10, 'Khách hàng', 3, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'Cbtd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thu nợ đã XLRR (nếu không có nợ XLRR thì cộng vào chỉ tiêu Dư nợ)', 10, 'Triệu VND', 4, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'Cbtd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thực hiện nhiệm vụ theo chương trình công tác', 10, '%', 5, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'Cbtd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 6, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'Cbtd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tổng nguồn vốn huy động BQ', 10, 'Triệu VND', 7, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'Cbtd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 8, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'Cbtd_KPI_Assignment';

-- TruongphongKtnqCnl1_KPI_Assignment (6 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.Id, 'Tổng nguồn vốn', 10, 'Triệu VND', 1, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKtnqCnl1_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Lợi nhuận khoán tài chính', 20, 'Triệu VND', 2, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKtnqCnl1_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thu dịch vụ thanh toán trong nước', 10, 'Triệu VND', 3, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKtnqCnl1_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 40, '%', 4, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKtnqCnl1_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 5, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKtnqCnl1_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Kết quả thực hiện BQ của CB trong phòng', 10, '%', 6, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKtnqCnl1_KPI_Assignment';

-- PhophongKtnqCnl1_KPI_Assignment (6 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.Id, 'Tổng nguồn vốn', 10, 'Triệu VND', 1, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKtnqCnl1_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Lợi nhuận khoán tài chính', 20, 'Triệu VND', 2, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKtnqCnl1_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thu dịch vụ thanh toán trong nước', 10, 'Triệu VND', 3, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKtnqCnl1_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 40, '%', 4, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKtnqCnl1_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 5, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKtnqCnl1_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Kết quả thực hiện BQ của CB thuộc mình phụ trách', 10, '%', 6, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKtnqCnl1_KPI_Assignment';

-- Gdv_KPI_Assignment (6 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.Id, 'Số bút toán giao dịch BQ', 50, 'BT', 1, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'Gdv_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Số bút toán hủy', 15, 'BT', 2, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'Gdv_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thực hiện chức năng, nhiệm vụ được giao', 10, '%', 3, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'Gdv_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 4, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'Gdv_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tổng nguồn vốn huy động BQ', 10, 'Triệu VND', 5, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'Gdv_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 6, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'Gdv_KPI_Assignment';

-- TruongphoItThKtgs_KPI_Assignment (5 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.Id, 'Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng', 65, '%', 1, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphoItThKtgs_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 2, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphoItThKtgs_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tổng nguồn vốn huy động BQ', 10, 'Triệu VND', 3, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphoItThKtgs_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 4, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphoItThKtgs_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Kết quả thực hiện BQ của cán bộ trong phòng', 10, '%', 5, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphoItThKtgs_KPI_Assignment';

-- CbItThKtgsKhqlrr_KPI_Assignment (4 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.Id, 'Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ được giao', 75, '%', 1, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'CbItThKtgsKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 2, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'CbItThKtgsKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tổng nguồn vốn huy động BQ', 10, 'Triệu VND', 3, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'CbItThKtgsKhqlrr_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 4, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'CbItThKtgsKhqlrr_KPI_Assignment';

-- GiamdocPgd_KPI_Assignment (9 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.Id, 'Tổng nguồn vốn BQ', 15, 'Triệu VND', 1, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'GiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tổng dư nợ BQ', 15, 'Triệu VND', 2, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'GiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tỷ lệ nợ xấu', 10, '%', 3, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'GiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Phát triển Khách hàng', 10, 'Khách hàng', 4, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'GiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)', 5, 'Triệu VND', 5, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'GiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thu dịch vụ', 10, 'Triệu VND', 6, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'GiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Lợi nhuận khoán tài chính', 15, 'Triệu VND', 7, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'GiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 8, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'GiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'BQ kết quả thực hiện của CB trong phòng', 10, '%', 9, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'GiamdocPgd_KPI_Assignment';

-- PhogiamdocPgd_KPI_Assignment (9 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.Id, 'Tổng nguồn vốn BQ', 15, 'Triệu VND', 1, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tổng dư nợ BQ', 15, 'Triệu VND', 2, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tỷ lệ nợ xấu', 10, '%', 3, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Phát triển Khách hàng', 10, 'Khách hàng', 4, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)', 5, 'Triệu VND', 5, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thu dịch vụ', 10, 'Triệu VND', 6, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Lợi nhuận khoán tài chính', 15, 'Triệu VND', 7, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 8, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocPgd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'BQ kết quả thực hiện của CB trong phòng', 10, '%', 9, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocPgd_KPI_Assignment';

-- PhogiamdocPgdCbtd_KPI_Assignment (8 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.Id, 'Tổng dư nợ BQ', 30, 'Triệu VND', 1, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocPgdCbtd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tỷ lệ nợ xấu', 15, '%', 2, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocPgdCbtd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Phát triển Khách hàng', 10, 'Khách hàng', 3, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocPgdCbtd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)', 10, 'Triệu VND', 4, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocPgdCbtd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thực hiện nhiệm vụ theo chương trình công tác', 10, '%', 5, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocPgdCbtd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tổng nguồn vốn huy động BQ', 10, 'Triệu VND', 6, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocPgdCbtd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 7, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocPgdCbtd_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 8, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocPgdCbtd_KPI_Assignment';

-- GiamdocCnl2_KPI_Assignment (11 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.Id, 'Tổng nguồn vốn cuối kỳ', 5, 'Triệu VND', 1, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'GiamdocCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tổng nguồn vốn huy động BQ trong kỳ', 10, 'Triệu VND', 2, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'GiamdocCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tổng dư nợ cuối kỳ', 5, 'Triệu VND', 3, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'GiamdocCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tổng dư nợ BQ trong kỳ', 10, 'Triệu VND', 4, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'GiamdocCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tổng dư nợ HSX&CN', 5, 'Triệu VND', 5, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'GiamdocCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tỷ lệ nợ xấu nội bảng', 10, '%', 6, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'GiamdocCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thu nợ đã XLRR', 5, 'Triệu VND', 7, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'GiamdocCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Phát triển Khách hàng', 10, 'Khách hàng', 8, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'GiamdocCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Lợi nhuận khoán tài chính', 20, 'Triệu VND', 9, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'GiamdocCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thu dịch vụ', 10, 'Triệu VND', 10, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'GiamdocCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10, '%', 11, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'GiamdocCnl2_KPI_Assignment';

-- PhogiamdocCnl2Td_KPI_Assignment (8 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.Id, 'Tổng dư nợ cho vay', 20, 'Triệu VND', 1, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocCnl2Td_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tổng dư nợ cho vay HSX&CN', 10, 'Triệu VND', 2, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocCnl2Td_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thu nợ đã xử lý', 10, 'Triệu VND', 3, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocCnl2Td_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Lợi nhuận khoán tài chính', 20, 'Triệu VND', 4, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocCnl2Td_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tỷ lệ nợ xấu', 10, '%', 5, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocCnl2Td_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Phát triển Khách hàng', 10, 'Khách hàng', 6, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocCnl2Td_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Điều hành theo chương trình công tác, nhiệm vụ được giao', 10, '%', 7, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocCnl2Td_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 8, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocCnl2Td_KPI_Assignment';

-- PhogiamdocCnl2Kt_KPI_Assignment (6 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.Id, 'Tổng nguồn vốn', 20, 'Triệu VND', 1, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocCnl2Kt_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Lợi nhuận khoán tài chính', 30, 'Triệu VND', 2, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocCnl2Kt_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tổng doanh thu phí dịch vụ', 20, 'Triệu VND', 3, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocCnl2Kt_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Số thẻ phát hành', 10, 'cái', 4, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocCnl2Kt_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Điều hành theo chương trình công tác, nhiệm vụ được giao', 10, '%', 5, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocCnl2Kt_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 6, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhogiamdocCnl2Kt_KPI_Assignment';

-- TruongphongKhCnl2_KPI_Assignment (9 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.Id, 'Tổng dư nợ', 20, 'Triệu VND', 1, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tỷ lệ nợ xấu', 15, '%', 2, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Phát triển Khách hàng', 10, 'Khách hàng', 3, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thu nợ đã XLRR', 10, 'Triệu VND', 4, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Điều hành theo chương trình công tác', 10, '%', 5, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 6, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tổng nguồn vốn huy động BQ', 10, 'Triệu VND', 7, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 8, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Kết quả thực hiện BQ của CB trong phòng', 10, '%', 9, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKhCnl2_KPI_Assignment';

-- PhophongKhCnl2_KPI_Assignment (8 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.Id, 'Tổng dư nợ BQ', 30, 'Triệu VND', 1, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tỷ lệ nợ xấu', 15, '%', 2, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Phát triển Khách hàng', 10, 'Khách hàng', 3, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thu nợ đã XLRR', 10, 'Triệu VND', 4, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thực hiện nhiệm vụ theo chương trình công tác', 10, '%', 5, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 6, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Tổng nguồn vốn huy động BQ', 10, 'Triệu VND', 7, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 8, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKhCnl2_KPI_Assignment';

-- TruongphongKtnqCnl2_KPI_Assignment (6 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.Id, 'Tổng nguồn vốn', 10, 'Triệu VND', 1, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKtnqCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Lợi nhuận khoán tài chính', 20, 'Triệu VND', 2, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKtnqCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thu dịch vụ thanh toán trong nước', 10, 'Triệu VND', 3, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKtnqCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng', 40, '%', 4, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKtnqCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 5, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKtnqCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Kết quả thực hiện BQ của CB trong phòng', 10, '%', 6, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'TruongphongKtnqCnl2_KPI_Assignment';

-- PhophongKtnqCnl2_KPI_Assignment (5 chỉ tiêu)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
SELECT t.Id, 'Số bút toán giao dịch BQ', 40, 'BT', 1, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKtnqCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Số bút toán hủy', 20, 'BT', 2, 1, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKtnqCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Thực hiện nhiệm vụ theo chương trình công tác', 25, '%', 3, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKtnqCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, '%', 4, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKtnqCnl2_KPI_Assignment'
UNION ALL
SELECT t.Id, 'Hoàn thành chỉ tiêu giao khoán SPDV', 5, '%', 5, 2, 1 FROM KpiAssignmentTables t WHERE t.TableName = 'PhophongKtnqCnl2_KPI_Assignment';

-- 3. KIỂM TRA KẾT QUẢ
SELECT
    Category,
    COUNT(*) as TotalIndicators
FROM KpiIndicators i
INNER JOIN KpiAssignmentTables t ON i.TableId = t.Id
GROUP BY Category
ORDER BY Category;

SELECT
    t.TableName,
    t.Description,
    COUNT(i.Id) as IndicatorCount
FROM KpiAssignmentTables t
LEFT JOIN KpiIndicators i ON t.Id = i.TableId
WHERE t.Category = 'CANBO'
GROUP BY t.Id, t.TableName, t.Description
ORDER BY t.TableName;

PRINT '✅ HOÀN THÀNH: Đã khôi phục 158 chỉ tiêu chuẩn cho 22 bảng KPI CANBO (không bao gồm TqHkKtnb)';
