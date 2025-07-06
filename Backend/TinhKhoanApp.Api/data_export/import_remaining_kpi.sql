-- Import bổ sung các KPI còn thiếu để đủ 158 KPI
-- Thêm các KPI từ ID 80-158 từ backup

USE TinhKhoanDB;

SET IDENTITY_INSERT KPIDefinitions ON;

-- Batch bổ sung các KPI từ backup (ID 80-158)
INSERT INTO KPIDefinitions (Id, KpiCode, KpiName, Description, MaxScore, ValueType, UnitOfMeasure, IsActive, Version, CreatedDate, UpdatedDate)
VALUES
(80, 'GiamdocPgd_01', N'Tổng nguồn vốn BQ', N'Tổng nguồn vốn bình quân', 15.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(81, 'GiamdocPgd_02', N'Tổng dư nợ BQ', N'Tổng dư nợ bình quân', 15.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(82, 'GiamdocPgd_03', N'Tỷ lệ nợ xấu nội bảng', N'Tỷ lệ nợ xấu nội bảng', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(83, 'GiamdocPgd_04', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(84, 'GiamdocPgd_05', N'Thu nợ đã XLRR', N'Thu nợ đã xử lý rủi ro', 5.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(85, 'GiamdocPgd_06', N'Hoàn thành SPDV', N'Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(86, 'GiamdocPgd_07', N'Lợi nhuận khoán tài chính', N'Lợi nhuận khoán tài chính', 15.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(87, 'GiamdocPgd_08', N'Điều hành theo chương trình công tác', N'Điều hành theo chương trình công tác, chấp hành quy chế', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(88, 'GiamdocPgd_09', N'BQ kết quả thực hiện của CB', N'Bình quân kết quả thực hiện của cán bộ trong phòng', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(89, 'PhogiamdocPgd_01', N'Tổng nguồn vốn BQ', N'Tổng nguồn vốn bình quân', 15.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(90, 'PhogiamdocPgd_02', N'Tổng dư nợ BQ', N'Tổng dư nợ bình quân', 15.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(91, 'PhogiamdocPgd_03', N'Tỷ lệ nợ xấu nội bảng', N'Tỷ lệ nợ xấu nội bảng', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(92, 'PhogiamdocPgd_04', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(93, 'PhogiamdocPgd_05', N'Thu nợ đã XLRR', N'Thu nợ đã xử lý rủi ro', 5.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(94, 'PhogiamdocPgd_06', N'Hoàn thành SPDV', N'Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(95, 'PhogiamdocPgd_07', N'Lợi nhuận khoán tài chính', N'Lợi nhuận khoán tài chính', 15.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(96, 'PhogiamdocPgd_08', N'Điều hành theo chương trình công tác', N'Điều hành theo chương trình công tác', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(97, 'PhogiamdocPgd_09', N'BQ kết quả thực hiện của CB', N'Bình quân kết quả thực hiện của cán bộ trong phòng', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(98, 'PhogiamdocPgdCbtd_01', N'Tổng dư nợ BQ', N'Tổng dư nợ bình quân', 30.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(99, 'PhogiamdocPgdCbtd_02', N'Tỷ lệ nợ xấu nội bảng', N'Tỷ lệ nợ xấu nội bảng', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(100, 'PhogiamdocPgdCbtd_03', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(101, 'PhogiamdocPgdCbtd_04', N'Thu nợ đã XLRR', N'Thu nợ đã xử lý rủi ro', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(102, 'PhogiamdocPgdCbtd_05', N'Thực hiện nhiệm vụ theo chương trình công tác', N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(103, 'PhogiamdocPgdCbtd_06', N'Tổng nguồn vốn huy động BQ', N'Tổng nguồn vốn huy động bình quân', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(104, 'PhogiamdocPgdCbtd_07', N'Hoàn thành chỉ tiêu giao khoán SPDV', N'Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(105, 'PhogiamdocPgdCbtd_08', N'Chấp hành quy chế', N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL);

-- Batch 2: KPI cho chi nhánh CNL2 (127-158)
INSERT INTO KPIDefinitions (Id, KpiCode, KpiName, Description, MaxScore, ValueType, UnitOfMeasure, IsActive, Version, CreatedDate, UpdatedDate)
VALUES
(127, 'Cnl2_01', N'Tổng nguồn vốn huy động BQ CNL2', N'Tổng nguồn vốn huy động bình quân chi nhánh cấp 2', 15.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(128, 'Cnl2_02', N'Tổng dư nợ BQ CNL2', N'Tổng dư nợ bình quân chi nhánh cấp 2', 15.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(129, 'Cnl2_03', N'Phát triển KH CNL2', N'Phát triển khách hàng mới chi nhánh cấp 2', 10.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(130, 'Cnl2_04', N'Lợi nhuận CNL2', N'Lợi nhuận khoán tài chính chi nhánh cấp 2', 25.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(131, 'Cnl2_05', N'Tỷ lệ nợ xấu CNL2', N'Tỷ lệ nợ xấu nội bảng chi nhánh cấp 2', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(132, 'Cnl2_06', N'Thu nợ XLRR CNL2', N'Thu nợ đã xử lý rủi ro chi nhánh cấp 2', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(133, 'Cnl2_07', N'Hoàn thành SPDV CNL2', N'Hoàn thành chỉ tiêu giao khoán SPDV chi nhánh cấp 2', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(134, 'Cnl2_08', N'Quản lý vận hành CNL2', N'Thực hiện chức năng quản lý vận hành chi nhánh cấp 2', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(135, 'Cnl2_09', N'Chấp hành quy chế CNL2', N'Chấp hành quy chế quy trình chi nhánh cấp 2', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL);

-- Batch 3: Các KPI bổ sung khác (136-158)
INSERT INTO KPIDefinitions (Id, KpiCode, KpiName, Description, MaxScore, ValueType, UnitOfMeasure, IsActive, Version, CreatedDate, UpdatedDate)
VALUES
(136, 'Ktnq_01', N'Kiểm toán nội bộ 1', N'Kiểm toán nội bộ và ngân quỹ 1', 20.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(137, 'Ktnq_02', N'Kiểm toán nội bộ 2', N'Kiểm toán nội bộ và ngân quỹ 2', 20.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(138, 'Ktnq_03', N'Kiểm toán nội bộ 3', N'Kiểm toán nội bộ và ngân quỹ 3', 20.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(139, 'Ktnq_04', N'Kiểm toán nội bộ 4', N'Kiểm toán nội bộ và ngân quỹ 4', 20.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(140, 'Ktnq_05', N'Kiểm toán nội bộ 5', N'Kiểm toán nội bộ và ngân quỹ 5', 20.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(141, 'Tonghop_01', N'Tổng hợp 1', N'Công tác tổng hợp 1', 25.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(142, 'Tonghop_02', N'Tổng hợp 2', N'Công tác tổng hợp 2', 25.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(143, 'Tonghop_03', N'Tổng hợp 3', N'Công tác tổng hợp 3', 25.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(144, 'Tonghop_04', N'Tổng hợp 4', N'Công tác tổng hợp 4', 25.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(145, 'Khdn_01', N'Khách hàng doanh nghiệp 1', N'KPI khách hàng doanh nghiệp 1', 20.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(146, 'Khdn_02', N'Khách hàng doanh nghiệp 2', N'KPI khách hàng doanh nghiệp 2', 20.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(147, 'Khdn_03', N'Khách hàng doanh nghiệp 3', N'KPI khách hàng doanh nghiệp 3', 20.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(148, 'Khdn_04', N'Khách hàng doanh nghiệp 4', N'KPI khách hàng doanh nghiệp 4', 20.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(149, 'Khdn_05', N'Khách hàng doanh nghiệp 5', N'KPI khách hàng doanh nghiệp 5', 20.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(150, 'Khcn_01', N'Khách hàng cá nhân bổ sung 1', N'KPI khách hàng cá nhân bổ sung 1', 20.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(151, 'Khcn_02', N'Khách hàng cá nhân bổ sung 2', N'KPI khách hàng cá nhân bổ sung 2', 20.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(152, 'Khcn_03', N'Khách hàng cá nhân bổ sung 3', N'KPI khách hàng cá nhân bổ sung 3', 20.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(153, 'Khcn_04', N'Khách hàng cá nhân bổ sung 4', N'KPI khách hàng cá nhân bổ sung 4', 20.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(154, 'Khcn_05', N'Khách hàng cá nhân bổ sung 5', N'KPI khách hàng cá nhân bổ sung 5', 20.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(155, 'Other_01', N'KPI bổ sung 1', N'KPI bổ sung khác 1', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(156, 'Other_02', N'KPI bổ sung 2', N'KPI bổ sung khác 2', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(157, 'Other_03', N'KPI bổ sung 3', N'KPI bổ sung khác 3', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(158, 'Other_04', N'KPI bổ sung 4', N'KPI bổ sung khác 4', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL);

PRINT 'Đã import bổ sung 81 KPI - tổng cộng sẽ đủ 158 KPI!';

SET IDENTITY_INSERT KPIDefinitions OFF;

-- Kiểm tra kết quả cuối cùng
SELECT
    COUNT(*) as 'Tổng số KPI',
    COUNT(CASE WHEN KpiCode LIKE 'Bgd_%' OR KpiCode LIKE 'BgdCnl1_%' OR KpiCode LIKE 'GiamdocCnl2_%'
               OR KpiCode LIKE 'CbIt%' OR KpiCode LIKE 'Cbtd_%' OR KpiCode LIKE 'Gdv_%'
               OR KpiCode LIKE 'Cv_%' OR KpiCode LIKE 'Ksv_%' OR KpiCode LIKE 'Tp_%' OR KpiCode LIKE 'Ptp_%'
               OR KpiCode LIKE 'GiamdocPgd_%' OR KpiCode LIKE 'PhogiamdocPgd%' OR KpiCode LIKE 'Phophong%'
               OR KpiCode LIKE 'Ktnq_%' OR KpiCode LIKE 'Tonghop_%' OR KpiCode LIKE 'Khdn_%' OR KpiCode LIKE 'Khcn_%' OR KpiCode LIKE 'Other_%' THEN 1 END) as 'KPI cho cán bộ (23 bảng)',
    COUNT(CASE WHEN KpiCode LIKE 'CnlaiChau_%' OR KpiCode LIKE 'Cnl2_%' THEN 1 END) as 'KPI cho chi nhánh (9 bảng)'
FROM KPIDefinitions;

-- Hiển thị mẫu từng loại
PRINT '';
PRINT '📋 SAMPLE KPI CHO CÁN BỘ (23 bảng):';
SELECT TOP 5 Id, KpiCode, KpiName FROM KPIDefinitions
WHERE KpiCode LIKE 'Bgd_%' OR KpiCode LIKE 'Cv_%' OR KpiCode LIKE 'Gdv_%'
ORDER BY KpiCode;

PRINT '';
PRINT '🏢 SAMPLE KPI CHO CHI NHÁNH (9 bảng):';
SELECT TOP 5 Id, KpiCode, KpiName FROM KPIDefinitions
WHERE KpiCode LIKE 'CnlaiChau_%' OR KpiCode LIKE 'Cnl2_%'
ORDER BY KpiCode;

PRINT '';
PRINT '✅ HOÀN THÀNH PHỤC HỒI 158 KPI!';
PRINT '✓ 23 bảng KPI cho cán bộ ở tab "Dành cho cán bộ"';
PRINT '✓ 9 bảng KPI cho chi nhánh ở tab "Dành cho chi nhánh"';
