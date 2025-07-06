DELETE FROM KPIDefinitions;

-- Reset Identity  
DBCC CHECKIDENT ('KPIDefinitions', RESEED, 0);

-- Import toàn bộ KPI Definitions
SET IDENTITY_INSERT KPIDefinitions ON;


-- Batch 1: KPI 1 - 20
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
(109, 'GiamdocCnl2_04', N'Tổng dư nợ BQ trong kỳ Tổng dư', N'nợ bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL);

-- Batch 2: KPI 21 - 40
INSERT INTO KPIDefinitions (Id, KpiCode, KpiName, Description, MaxScore, ValueType, UnitOfMeasure, IsActive, Version, CreatedDate, UpdatedDate)
VALUES
(110, 'GiamdocCnl2_05', N'Tổng dư nợ HSX&CN Tổng dư', N'nợ hợp tác xã và chủ nợ', 5.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(111, 'GiamdocCnl2_06', N'Tỷ lệ nợ xấu nội', N'bảng Tỷ lệ nợ xấu nội bảng', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(112, 'GiamdocCnl2_07', N'Thu nợ đã XLRR', N'Thu nợ đã xử lý rủi ro', 5.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(113, 'GiamdocCnl2_08', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(114, 'GiamdocCnl2_09', N'Lợi nhuận khoán tài chính Lợi', N'nhuận khoán tài chính', 20.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(116, 'GiamdocCnl2_11', N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung', N'chỉ đạo, điều hành của CNL1, văn hóa Agribank Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(80, 'GiamdocPgd_01', N'Tổng nguồn vốn', N'BQ Tổng nguồn vốn bình quân', 15.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(81, 'GiamdocPgd_02', N'Tổng dư nợ BQ', N'Tổng dư nợ bình quân', 15.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(83, 'GiamdocPgd_04', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(84, 'GiamdocPgd_05', N'Thu nợ đã XLRR', N'(nếu không có thì cộng vào chỉ tiêu dư nợ) Thu nợ đã xử lý rủi ro (nếu không có thì cộng vào chỉ tiêu dư nợ)', 5.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(86, 'GiamdocPgd_07', N'Lợi nhuận khoán tài chính Lợi', N'nhuận khoán tài chính', 15.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(87, 'GiamdocPgd_08', N'Điều hành theo chương trình công tác, chấp hành', N'quy chế, quy trình nghiệp vụ, văn hóa Agribank Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(88, 'GiamdocPgd_09', N'BQ kết quả thực', N'hiện của CB trong phòng Bình quân kết quả thực hiện của cán bộ trong phòng', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(126, 'PhogiamdocCnl2Kt_02', N'Lợi nhuận khoán tài chính Lợi', N'nhuận khoán tài chính', 30.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(127, 'PhogiamdocCnl2Kt_03', N'Tổng doanh thu phí dịch vụ Tổng', N'doanh thu phí dịch vụ', 20.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(129, 'PhogiamdocCnl2Kt_05', N'Điều hành theo', N'chương trình công tác, nhiệm vụ được giao Điều hành theo chương trình công tác, nhiệm vụ được giao', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(130, 'PhogiamdocCnl2Kt_06', N'Chấp hành quy', N'chế, quy trình nghiệp vụ, văn hóa Agribank Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(117, 'PhogiamdocCnl2Td_01', N'Tổng dư nợ cho', N'vay Tổng dư nợ cho vay', 20.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(118, 'PhogiamdocCnl2Td_02', N'Tổng dư nợ cho vay HSX&CN Tổng dư nợ cho vay hợp', N'tác xã và chủ nợ', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(120, 'PhogiamdocCnl2Td_04', N'Lợi nhuận khoán tài chính Lợi', N'nhuận khoán tài chính', 20.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL);

-- Batch 3: KPI 41 - 60
INSERT INTO KPIDefinitions (Id, KpiCode, KpiName, Description, MaxScore, ValueType, UnitOfMeasure, IsActive, Version, CreatedDate, UpdatedDate)
VALUES
(122, 'PhogiamdocCnl2Td_06', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(123, 'PhogiamdocCnl2Td_07', N'Điều hành theo', N'chương trình công tác, nhiệm vụ được giao Điều hành theo chương trình công tác, nhiệm vụ được giao', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(124, 'PhogiamdocCnl2Td_08', N'Chấp hành quy', N'chế, quy trình nghiệp vụ, văn hóa Agribank Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(89, 'PhogiamdocPgd_01', N'Tổng nguồn vốn', N'BQ Tổng nguồn vốn bình quân', 15.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(90, 'PhogiamdocPgd_02', N'Tổng dư nợ BQ', N'Tổng dư nợ bình quân', 15.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(92, 'PhogiamdocPgd_04', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(93, 'PhogiamdocPgd_05', N'Thu nợ đã XLRR', N'(nếu không có thì cộng vào chỉ tiêu dư nợ) Thu nợ đã xử lý rủi ro (nếu không có thì cộng vào chỉ tiêu dư nợ)', 5.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(95, 'PhogiamdocPgd_07', N'Lợi nhuận khoán tài chính Lợi', N'nhuận khoán tài chính', 15.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(96, 'PhogiamdocPgd_08', N'Điều hành theo chương trình công tác, chấp hành', N'quy chế, quy trình nghiệp vụ, văn hóa Agribank Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(97, 'PhogiamdocPgd_09', N'BQ kết quả thực', N'hiện của CB trong phòng Bình quân kết quả thực hiện của cán bộ trong phòng', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(98, 'PhogiamdocPgdCbtd_01', N'Tổng dư nợ BQ', N'Tổng dư nợ bình quân', 30.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(100, 'PhogiamdocPgdCbtd_03', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(101, 'PhogiamdocPgdCbtd_04', N'Thu nợ đã XLRR', N'(nếu không có thì cộng vào chỉ tiêu dư nợ) Thu nợ đã xử lý rủi ro (nếu không có thì cộng vào chỉ tiêu dư nợ)', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(102, 'PhogiamdocPgdCbtd_05', N'Thực hiện nhiệm', N'vụ theo chương trình công tác Thực hiện nhiệm vụ theo chương trình công tác', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(103, 'PhogiamdocPgdCbtd_06', N'Tổng nguồn vốn huy động BQ Tổng nguồn vốn', N'huy động bình quân', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(104, 'PhogiamdocPgdCbtd_07', N'Hoàn thành chỉ', N'tiêu giao khoán SPDV Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(105, 'PhogiamdocPgdCbtd_08', N'Chấp hành quy', N'chế, quy trình nghiệp vụ, văn hóa Agribank Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(25, 'PhophongKhcn_01', N'Tổng Dư nợ KHCN Tổng', N'dư nợ khách hàng cá nhân', 20.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(26, 'PhophongKhcn_02', N'Tỷ lệ nợ xấu KHCN Tỷ lệ', N'nợ xấu khách hàng cá nhân', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(27, 'PhophongKhcn_03', N'Thu nợ đã XLRR KHCN Thu nợ đã xử lý rủi ro', N'khách hàng cá nhân', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL);

-- Batch 4: KPI 61 - 80
INSERT INTO KPIDefinitions (Id, KpiCode, KpiName, Description, MaxScore, ValueType, UnitOfMeasure, IsActive, Version, CreatedDate, UpdatedDate)
VALUES
(28, 'PhophongKhcn_04', N'Lợi nhuận khoán tài chính Lợi', N'nhuận khoán tài chính', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(29, 'PhophongKhcn_05', N'Phát triển Khách hàng Cá nhân Phát triển khách', N'hàng cá nhân mới', 10.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(30, 'PhophongKhcn_06', N'Điều hành theo', N'chương trình công tác Điều hành theo chương trình công tác', 20.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(31, 'PhophongKhcn_07', N'Chấp hành quy', N'chế, quy trình nghiệp vụ Chấp hành quy chế, quy trình nghiệp vụ', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(32, 'PhophongKhcn_08', N'BQ kết quả thực', N'hiện CB trong phòng mình phụ trách Bình quân kết quả thực hiện cán bộ trong phòng mình phụ trách', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(140, 'PhophongKhCnl2_01', N'Tổng dư nợ BQ', N'Tổng dư nợ bình quân', 30.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(142, 'PhophongKhCnl2_03', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(143, 'PhophongKhCnl2_04', N'Thu nợ đã XLRR', N'Thu nợ đã xử lý rủi ro', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(144, 'PhophongKhCnl2_05', N'Thực hiện nhiệm', N'vụ theo chương trình công tác Thực hiện nhiệm vụ theo chương trình công tác', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(145, 'PhophongKhCnl2_06', N'Chấp hành quy', N'chế, quy trình nghiệp vụ, văn hóa Agribank Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(146, 'PhophongKhCnl2_07', N'Tổng nguồn vốn huy động BQ Tổng nguồn vốn', N'huy động bình quân', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(147, 'PhophongKhCnl2_08', N'Hoàn thành chỉ', N'tiêu giao khoán SPDV Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(17, 'PhophongKhdn_01', N'Tổng Dư nợ KHDN Tổng dư nợ', N'khách hàng doanh nghiệp', 20.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(18, 'PhophongKhdn_02', N'Tỷ lệ nợ xấu KHDN Tỷ lệ nợ xấu khách', N'hàng doanh nghiệp', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(19, 'PhophongKhdn_03', N'Thu nợ đã XLRR KHDN Thu nợ đã xử lý rủi ro khách hàng', N'doanh nghiệp', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(20, 'PhophongKhdn_04', N'Lợi nhuận khoán tài chính Lợi', N'nhuận khoán tài chính', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(21, 'PhophongKhdn_05', N'Phát triển Khách', N'hàng Doanh nghiệp Phát triển khách hàng doanh nghiệp mới', 10.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(22, 'PhophongKhdn_06', N'Điều hành theo', N'chương trình công tác Điều hành theo chương trình công tác', 20.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(23, 'PhophongKhdn_07', N'Chấp hành quy', N'chế, quy trình nghiệp vụ Chấp hành quy chế, quy trình nghiệp vụ', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(24, 'PhophongKhdn_08', N'BQ kết quả thực', N'hiện CB trong phòng mình phụ trách Bình quân kết quả thực hiện cán bộ trong phòng mình phụ trách', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL);

-- Batch 5: KPI 81 - 100
INSERT INTO KPIDefinitions (Id, KpiCode, KpiName, Description, MaxScore, ValueType, UnitOfMeasure, IsActive, Version, CreatedDate, UpdatedDate)
VALUES
(41, 'PhophongKhqlrr_03', N'Lợi nhuận khoán tài chính Lợi', N'nhuận khoán tài chính', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(42, 'PhophongKhqlrr_04', N'Thực hiện nhiệm vụ theo chương', N'trình công tác, chức năng nhiệm vụ của phòng Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 50.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(43, 'PhophongKhqlrr_05', N'Chấp hành quy', N'chế, quy trình nghiệp vụ, văn hóa Agribank Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(44, 'PhophongKhqlrr_06', N'Kết quả thực', N'hiện BQ của CB trong phòng mình phụ trách Kết quả thực hiện bình quân của cán bộ trong phòng mình phụ trách', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(60, 'PhophongKtnqCnl1_02', N'Lợi nhuận khoán tài chính Lợi', N'nhuận khoán tài chính', 20.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(61, 'PhophongKtnqCnl1_03', N'Thu dịch vụ thanh toán trong nước Thu dịch vụ thanh', N'toán trong nước', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(62, 'PhophongKtnqCnl1_04', N'Thực hiện nhiệm vụ theo chương', N'trình công tác, chức năng nhiệm vụ của phòng Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 40.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(63, 'PhophongKtnqCnl1_05', N'Chấp hành quy', N'chế, quy trình nghiệp vụ, văn hóa Agribank Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(64, 'PhophongKtnqCnl1_06', N'Kết quả thực', N'hiện BQ của CB thuộc mình phụ trách Kết quả thực hiện bình quân của cán bộ thuộc mình phụ trách', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(154, 'PhophongKtnqCnl2_01', N'Số bút toán giao dịch BQ Số bút toán', N'giao dịch bình quân', 40.00, 1, N'BT', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(156, 'PhophongKtnqCnl2_03', N'Thực hiện nhiệm', N'vụ theo chương trình công tác Thực hiện nhiệm vụ theo chương trình công tác', 25.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(157, 'PhophongKtnqCnl2_04', N'Chấp hành quy', N'chế, quy trình nghiệp vụ, văn hóa Agribank Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(158, 'PhophongKtnqCnl2_05', N'Hoàn thành chỉ', N'tiêu giao khoán SPDV Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(71, 'TruongphongItThKtgs_01', N'Thực hiện nhiệm vụ theo chương trình công tác,', N'các công việc theo chức năng nhiệm vụ của phòng Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng', 65.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(72, 'TruongphongItThKtgs_02', N'Chấp hành quy', N'chế, quy trình nghiệp vụ, văn hóa Agribank Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(73, 'TruongphongItThKtgs_03', N'Tổng nguồn vốn huy động BQ Tổng nguồn vốn', N'huy động bình quân', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(74, 'TruongphongItThKtgs_04', N'Hoàn thành chỉ', N'tiêu giao khoán SPDV Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(75, 'TruongphongItThKtgs_05', N'Kết quả thực', N'hiện BQ của cán bộ trong phòng Kết quả thực hiện bình quân của cán bộ trong phòng', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(9, 'TruongphongKhcn_01', N'Tổng Dư nợ KHCN Tổng', N'dư nợ khách hàng cá nhân', 20.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(10, 'TruongphongKhcn_02', N'Tỷ lệ nợ xấu KHCN Tỷ lệ', N'nợ xấu khách hàng cá nhân', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL);

-- Batch 6: KPI 101 - 120
INSERT INTO KPIDefinitions (Id, KpiCode, KpiName, Description, MaxScore, ValueType, UnitOfMeasure, IsActive, Version, CreatedDate, UpdatedDate)
VALUES
(11, 'TruongphongKhcn_03', N'Thu nợ đã XLRR KHCN Thu nợ đã xử lý rủi ro', N'khách hàng cá nhân', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(12, 'TruongphongKhcn_04', N'Lợi nhuận khoán tài chính Lợi', N'nhuận khoán tài chính', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(13, 'TruongphongKhcn_05', N'Phát triển Khách hàng Cá nhân Phát triển khách', N'hàng cá nhân mới', 10.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(14, 'TruongphongKhcn_06', N'Điều hành theo', N'chương trình công tác Điều hành theo chương trình công tác', 20.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(15, 'TruongphongKhcn_07', N'Chấp hành quy', N'chế, quy trình nghiệp vụ Chấp hành quy chế, quy trình nghiệp vụ', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(16, 'TruongphongKhcn_08', N'BQ kết quả thực', N'hiện CB trong phòng mình phụ trách Bình quân kết quả thực hiện cán bộ trong phòng mình phụ trách', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(133, 'TruongphongKhCnl2_03', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(134, 'TruongphongKhCnl2_04', N'Thu nợ đã XLRR', N'Thu nợ đã xử lý rủi ro', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(135, 'TruongphongKhCnl2_05', N'Điều hành theo', N'chương trình công tác Điều hành theo chương trình công tác', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(136, 'TruongphongKhCnl2_06', N'Chấp hành quy', N'chế, quy trình nghiệp vụ, văn hóa Agribank Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(137, 'TruongphongKhCnl2_07', N'Tổng nguồn vốn huy động BQ Tổng nguồn vốn', N'huy động bình quân', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(138, 'TruongphongKhCnl2_08', N'Hoàn thành chỉ', N'tiêu giao khoán SPDV Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(139, 'TruongphongKhCnl2_09', N'Kết quả thực', N'hiện BQ của CB trong phòng Kết quả thực hiện bình quân của cán bộ trong phòng', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(1, 'TruongphongKhdn_01', N'Tổng Dư nợ KHDN Tổng dư nợ', N'khách hàng doanh nghiệp', 20.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(2, 'TruongphongKhdn_02', N'Tỷ lệ nợ xấu KHDN Tỷ lệ nợ xấu khách', N'hàng doanh nghiệp', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(3, 'TruongphongKhdn_03', N'Thu nợ đã XLRR KHDN Thu nợ đã xử lý rủi ro khách hàng', N'doanh nghiệp', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(4, 'TruongphongKhdn_04', N'Lợi nhuận khoán tài chính Lợi', N'nhuận khoán tài chính', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(5, 'TruongphongKhdn_05', N'Phát triển Khách', N'hàng Doanh nghiệp Phát triển khách hàng doanh nghiệp mới', 10.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(6, 'TruongphongKhdn_06', N'Điều hành theo', N'chương trình công tác Điều hành theo chương trình công tác', 20.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(7, 'TruongphongKhdn_07', N'Chấp hành quy', N'chế, quy trình nghiệp vụ Chấp hành quy chế, quy trình nghiệp vụ', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL);

-- Batch 7: KPI 121 - 135
INSERT INTO KPIDefinitions (Id, KpiCode, KpiName, Description, MaxScore, ValueType, UnitOfMeasure, IsActive, Version, CreatedDate, UpdatedDate)
VALUES
(8, 'TruongphongKhdn_08', N'BQ kết quả thực', N'hiện CB trong phòng mình phụ trách Bình quân kết quả thực hiện cán bộ trong phòng mình phụ trách', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(35, 'TruongphongKhqlrr_03', N'Lợi nhuận khoán tài chính Lợi', N'nhuận khoán tài chính', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(36, 'TruongphongKhqlrr_04', N'Thực hiện nhiệm vụ theo chương', N'trình công tác, chức năng nhiệm vụ của phòng Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 50.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(37, 'TruongphongKhqlrr_05', N'Chấp hành quy', N'chế, quy trình nghiệp vụ, văn hóa Agribank Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(38, 'TruongphongKhqlrr_06', N'Kết quả thực', N'hiện BQ của CB trong phòng Kết quả thực hiện bình quân của cán bộ trong phòng', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(54, 'TruongphongKtnqCnl1_02', N'Lợi nhuận khoán tài chính Lợi', N'nhuận khoán tài chính', 20.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(55, 'TruongphongKtnqCnl1_03', N'Thu dịch vụ thanh toán trong nước Thu dịch vụ thanh', N'toán trong nước', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(56, 'TruongphongKtnqCnl1_04', N'Thực hiện nhiệm vụ theo chương', N'trình công tác, chức năng nhiệm vụ của phòng Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 40.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(57, 'TruongphongKtnqCnl1_05', N'Chấp hành quy', N'chế, quy trình nghiệp vụ, văn hóa Agribank Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(58, 'TruongphongKtnqCnl1_06', N'Kết quả thực', N'hiện BQ của CB trong phòng Kết quả thực hiện bình quân của cán bộ trong phòng', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(149, 'TruongphongKtnqCnl2_02', N'Lợi nhuận khoán tài chính Lợi', N'nhuận khoán tài chính', 20.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(150, 'TruongphongKtnqCnl2_03', N'Thu dịch vụ thanh toán trong nước Thu dịch vụ thanh', N'toán trong nước', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(151, 'TruongphongKtnqCnl2_04', N'Thực hiện nhiệm vụ theo chương trình công tác,', N'các công việc theo chức năng nhiệm vụ của phòng Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng', 40.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(152, 'TruongphongKtnqCnl2_05', N'Chấp hành quy', N'chế, quy trình nghiệp vụ, văn hóa Agribank Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(153, 'TruongphongKtnqCnl2_06', N'Kết quả thực', N'hiện BQ của CB trong phòng Kết quả thực hiện bình quân của cán bộ trong phòng', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL);

SET IDENTITY_INSERT KPIDefinitions OFF;

-- Kiểm tra kết quả import
SELECT COUNT(*) as TotalImported FROM KPIDefinitions;

-- Thống kê theo loại KPI
SELECT 
    CASE 
        WHEN KpiCode LIKE 'GiamdocCnl2%' OR KpiCode LIKE 'PhogiamdocCnl2%' THEN 'Chi nhánh'
        ELSE 'Cán bộ'
    END as Type,
    COUNT(*) as Count
FROM KPIDefinitions 
WHERE IsActive = 1
GROUP BY CASE 
    WHEN KpiCode LIKE 'GiamdocCnl2%' OR KpiCode LIKE 'PhogiamdocCnl2%' THEN 'Chi nhánh'
    ELSE 'Cán bộ'
END;

-- Danh sách các bảng KPI theo prefix
SELECT 
    LEFT(KpiCode, CHARINDEX('_', KpiCode) - 1) as RolePrefix,
    COUNT(*) as Count
FROM KPIDefinitions 
WHERE IsActive = 1 AND CHARINDEX('_', KpiCode) > 0
GROUP BY LEFT(KpiCode, CHARINDEX('_', KpiCode) - 1)
ORDER BY Count DESC;
