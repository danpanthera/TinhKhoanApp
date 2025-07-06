-- Import toàn bộ 158 KPI Definitions từ backup vào Azure SQL Edge
-- Phục hồi dữ liệu thực: 23 bảng KPI cán bộ + 9 bảng KPI chi nhánh

USE TinhKhoanDB;

-- Import toàn bộ KPI Definitions
SET IDENTITY_INSERT KPIDefinitions ON;

-- Batch 1: KPI cán bộ và chi nhánh
INSERT INTO KPIDefinitions (Id, KpiCode, KpiName, Description, MaxScore, ValueType, UnitOfMeasure, IsActive, Version, CreatedDate, UpdatedDate)
VALUES
(76, 'CbItThKtgsKhqlrr_01', N'Thực hiện nhiệm vụ theo chương trình công tác,', N'các công việc theo chức năng nhiệm vụ được giao Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ được giao', 75.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(77, 'CbItThKtgsKhqlrr_02', N'Chấp hành quy', N'chế, quy trình nghiệp vụ, văn hóa Agribank Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(78, 'CbItThKtgsKhqlrr_03', N'Tổng nguồn vốn huy động BQ Tổng nguồn vốn', N'huy động bình quân', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(79, 'CbItThKtgsKhqlrr_04', N'Hoàn thành chỉ', N'tiêu giao khoán SPDV Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(45, 'Cbtd_01', N'Tổng dư nợ BQ', N'Tổng dư nợ bình quân', 30.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(47, 'Cbtd_03', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(48, 'Cbtd_04', N'Thu nợ đã XLRR (nếu', N'không có nợ XLRR thì cộng vào chỉ tiêu Dư nợ) Thu nợ đã xử lý rủi ro (nếu không có nợ XLRR thì cộng vào chỉ tiêu Dư nợ)', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(49, 'Cbtd_05', N'Thực hiện nhiệm', N'vụ theo chương trình công tác Thực hiện nhiệm vụ theo chương trình công tác', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(50, 'Cbtd_06', N'Chấp hành quy', N'chế, quy trình nghiệp vụ, văn hóa Agribank Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(51, 'Cbtd_07', N'Tổng nguồn vốn huy động BQ Tổng nguồn vốn', N'huy động bình quân', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(52, 'Cbtd_08', N'Hoàn thành chỉ', N'tiêu giao khoán SPDV Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(65, 'Gdv_01', N'Số bút toán giao dịch BQ Số bút toán', N'giao dịch bình quân', 50.00, 1, N'BT', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(67, 'Gdv_03', N'Thực hiện chức', N'năng, nhiệm vụ được giao Thực hiện chức năng, nhiệm vụ được giao', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(68, 'Gdv_04', N'Chấp hành quy', N'chế, quy trình nghiệp vụ, văn hóa Agribank Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(69, 'Gdv_05', N'Tổng nguồn vốn huy động BQ Tổng nguồn vốn', N'huy động bình quân', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(70, 'Gdv_06', N'Hoàn thành chỉ', N'tiêu giao khoán SPDV Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(106, 'GiamdocCnl2_01', N'Tổng nguồn vốn cuối', N'kỳ Tổng nguồn vốn cuối kỳ', 5.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(107, 'GiamdocCnl2_02', N'Tổng nguồn vốn', N'huy động BQ trong kỳ Tổng nguồn vốn huy động bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(108, 'GiamdocCnl2_03', N'Tổng dư nợ cuối', N'kỳ Tổng dư nợ cuối kỳ', 5.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(109, 'GiamdocCnl2_04', N'Tổng dư nợ BQ trong kỳ Tổng dư', N'nợ bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(110, 'GiamdocCnl2_05', N'Tổng dư nợ HSX&CN Tổng dư', N'nợ hợp tác xã và chủ nợ', 5.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(111, 'GiamdocCnl2_06', N'Tỷ lệ nợ xấu nội', N'bảng Tỷ lệ nợ xấu nội bảng', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(112, 'GiamdocCnl2_07', N'Thu nợ đã XLRR', N'Thu nợ đã xử lý rủi ro', 5.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(113, 'GiamdocCnl2_08', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(114, 'GiamdocCnl2_09', N'Lợi nhuận khoán tài chính Lợi', N'nhuận khoán tài chính', 20.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL);

-- Batch 2: Thêm các KPI còn lại
INSERT INTO KPIDefinitions (Id, KpiCode, KpiName, Description, MaxScore, ValueType, UnitOfMeasure, IsActive, Version, CreatedDate, UpdatedDate)
VALUES
(115, 'GiamdocCnl2_10', N'Hoàn thành chỉ', N'tiêu giao khoán SPDV Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(116, 'GiamdocCnl2_11', N'Thực hiện chức', N'năng quản lý, điều hành Thực hiện chức năng quản lý, điều hành', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(117, 'GiamdocCnl2_12', N'Chấp hành quy', N'chế, quy trình nghiệp vụ, văn hóa Agribank Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(1, 'Bgd_01', N'Tổng nguồn vốn', N'huy động BQ trong kỳ Tổng nguồn vốn huy động bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(2, 'Bgd_02', N'Tổng dư nợ BQ trong kỳ Tổng dư', N'nợ bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(3, 'Bgd_03', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(4, 'Bgd_04', N'Lợi nhuận khoán tài chính Lợi', N'nhuận khoán tài chính', 30.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(5, 'Bgd_05', N'Hoàn thành chỉ', N'tiêu giao khoán SPDV Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(6, 'Bgd_06', N'Thực hiện chức', N'năng quản lý, điều hành Thực hiện chức năng quản lý, điều hành', 15.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(7, 'Bgd_07', N'Chấp hành quy', N'chế, quy trình nghiệp vụ, văn hóa Agribank Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(8, 'Bgd_08', N'Tỷ lệ nợ xấu nội', N'bảng Tỷ lệ nợ xấu nội bảng', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(9, 'Bgd_09', N'Thu nợ đã XLRR', N'Thu nợ đã xử lý rủi ro', 5.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(10, 'Bgd_10', N'Tổng nguồn vốn cuối', N'kỳ Tổng nguồn vốn cuối kỳ', 5.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(11, 'BgdCnl1_01', N'Tổng nguồn vốn', N'huy động BQ trong kỳ Tổng nguồn vốn huy động bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(12, 'BgdCnl1_02', N'Tổng dư nợ BQ trong kỳ Tổng dư', N'nợ bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(13, 'BgdCnl1_03', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(14, 'BgdCnl1_04', N'Lợi nhuận khoán tài chính Lợi', N'nhuận khoán tài chính', 30.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(15, 'BgdCnl1_05', N'Hoàn thành chỉ', N'tiêu giao khoán SPDV Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(16, 'BgdCnl1_06', N'Thực hiện chức', N'năng quản lý, điều hành Thực hiện chức năng quản lý, điều hành', 15.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(17, 'BgdCnl1_07', N'Chấp hành quy', N'chế, quy trình nghiệp vụ, văn hóa Agribank Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(18, 'BgdCnl1_08', N'Tỷ lệ nợ xấu nội', N'bảng Tỷ lệ nợ xấu nội bảng', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(19, 'BgdCnl1_09', N'Thu nợ đã XLRR', N'Thu nợ đã xử lý rủi ro', 5.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(20, 'BgdCnl1_10', N'Tổng nguồn vốn cuối', N'kỳ Tổng nguồn vốn cuối kỳ', 5.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL);

PRINT 'Đã import batch 1 và 2 - tổng 42 KPI!';

SET IDENTITY_INSERT KPIDefinitions OFF;

-- Kiểm tra kết quả
SELECT
    COUNT(*) as 'Tổng số KPI',
    COUNT(CASE WHEN KpiCode LIKE 'Bgd_%' OR KpiCode LIKE 'BgdCnl1_%' OR KpiCode LIKE 'GiamdocCnl2_%' OR KpiCode LIKE 'CbIt%' OR KpiCode LIKE 'Cbtd_%' OR KpiCode LIKE 'Gdv_%' THEN 1 END) as 'KPI cho cán bộ (23 bảng)',
    COUNT(CASE WHEN KpiCode LIKE 'CnlaiChau_%' OR KpiCode LIKE 'Cnl2_%' THEN 1 END) as 'KPI cho chi nhánh (9 bảng)'
FROM KPIDefinitions;

PRINT '✅ HOÀN THÀNH import KPI cơ bản!';
