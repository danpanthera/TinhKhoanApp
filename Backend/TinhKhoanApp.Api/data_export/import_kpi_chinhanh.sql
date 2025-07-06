-- Import KPI cho chi nhánh - 9 bảng KPI dành cho "Dành cho chi nhánh"
-- Bổ sung để có đủ 23 bảng KPI cán bộ + 9 bảng KPI chi nhánh

USE TinhKhoanDB;

SET IDENTITY_INSERT KPIDefinitions ON;

-- Batch 3: Thêm các KPI còn thiếu cho cán bộ (để đủ 23 bảng)
INSERT INTO KPIDefinitions (Id, KpiCode, KpiName, Description, MaxScore, ValueType, UnitOfMeasure, IsActive, Version, CreatedDate, UpdatedDate)
VALUES
-- Nhóm Chuyen vien
(21, 'Cv_01', N'Tổng dư nợ BQ', N'Tổng dư nợ bình quân', 25.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(22, 'Cv_02', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(23, 'Cv_03', N'Thu nợ đã XLRR', N'Thu nợ đã xử lý rủi ro', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(24, 'Cv_04', N'Thực hiện nhiệm vụ theo chương trình công tác', N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(25, 'Cv_05', N'Chấp hành quy chế, quy trình nghiệp vụ', N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(26, 'Cv_06', N'Tổng nguồn vốn huy động BQ', N'Tổng nguồn vốn huy động bình quân', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(27, 'Cv_07', N'Hoàn thành chỉ tiêu giao khoán SPDV', N'Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),

-- Nhóm Kiem soat vien
(28, 'Ksv_01', N'Thực hiện nhiệm vụ kiểm soát', N'Thực hiện nhiệm vụ kiểm soát theo kế hoạch', 50.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(29, 'Ksv_02', N'Phát hiện vi phạm', N'Phát hiện và báo cáo vi phạm', 15.00, 1, N'Trường hợp', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(30, 'Ksv_03', N'Chấp hành quy chế kiểm soát', N'Chấp hành quy chế, quy trình kiểm soát', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(31, 'Ksv_04', N'Hoàn thành báo cáo kiểm soát', N'Hoàn thành báo cáo kiểm soát đúng hạn', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),

-- Nhóm Truong phong
(32, 'Tp_01', N'Tổng dư nợ BQ phòng ban', N'Tổng dư nợ bình quân phòng ban', 25.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(33, 'Tp_02', N'Phát triển KH phòng ban', N'Phát triển khách hàng mới phòng ban', 15.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(34, 'Tp_03', N'Thu nợ XLRR phòng ban', N'Thu nợ đã xử lý rủi ro phòng ban', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(35, 'Tp_04', N'Quản lý điều hành phòng ban', N'Thực hiện chức năng quản lý điều hành phòng ban', 15.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(36, 'Tp_05', N'Chấp hành quy chế phòng ban', N'Chấp hành quy chế quy trình phòng ban', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),

-- Nhóm Pho truong phong
(37, 'Ptp_01', N'Hỗ trợ quản lý phòng ban', N'Hỗ trợ trưởng phòng quản lý điều hành', 30.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(38, 'Ptp_02', N'Thực hiện nhiệm vụ được giao', N'Thực hiện nhiệm vụ được trưởng phòng giao', 25.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(39, 'Ptp_03', N'Hỗ trợ nghiệp vụ chuyên môn', N'Hỗ trợ nghiệp vụ chuyên môn cho cán bộ', 20.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(40, 'Ptp_04', N'Chấp hành quy chế', N'Chấp hành quy chế quy trình nghiệp vụ', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL);

-- Batch 4: 9 bảng KPI CHO CHI NHÁNH
INSERT INTO KPIDefinitions (Id, KpiCode, KpiName, Description, MaxScore, ValueType, UnitOfMeasure, IsActive, Version, CreatedDate, UpdatedDate)
VALUES
-- CNL1 - Chi nhánh Lai Châu (tổng thể)
(118, 'CnlaiChau_01', N'Tổng nguồn vốn huy động BQ toàn chi nhánh', N'Tổng nguồn vốn huy động bình quân toàn chi nhánh', 15.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(119, 'CnlaiChau_02', N'Tổng dư nợ BQ toàn chi nhánh', N'Tổng dư nợ bình quân toàn chi nhánh', 15.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(120, 'CnlaiChau_03', N'Phát triển KH toàn chi nhánh', N'Phát triển khách hàng mới toàn chi nhánh', 10.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(121, 'CnlaiChau_04', N'Lợi nhuận toàn chi nhánh', N'Lợi nhuận khoán tài chính toàn chi nhánh', 25.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(122, 'CnlaiChau_05', N'Tỷ lệ nợ xấu toàn chi nhánh', N'Tỷ lệ nợ xấu nội bảng toàn chi nhánh', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(123, 'CnlaiChau_06', N'Thu nợ XLRR toàn chi nhánh', N'Thu nợ đã xử lý rủi ro toàn chi nhánh', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(124, 'CnlaiChau_07', N'Hoàn thành SPDV toàn chi nhánh', N'Hoàn thành chỉ tiêu giao khoán SPDV toàn chi nhánh', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(125, 'CnlaiChau_08', N'Quản lý vận hành toàn chi nhánh', N'Thực hiện chức năng quản lý vận hành toàn chi nhánh', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(126, 'CnlaiChau_09', N'Chấp hành quy chế toàn chi nhánh', N'Chấp hành quy chế quy trình toàn chi nhánh', 5.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL);

PRINT 'Đã import batch 3 và 4 - thêm 29 KPI (20 cho cán bộ + 9 cho chi nhánh)!';

SET IDENTITY_INSERT KPIDefinitions OFF;

-- Kiểm tra kết quả chi tiết
SELECT
    COUNT(*) as 'Tổng số KPI',
    COUNT(CASE WHEN KpiCode LIKE 'Bgd_%' OR KpiCode LIKE 'BgdCnl1_%' OR KpiCode LIKE 'GiamdocCnl2_%'
               OR KpiCode LIKE 'CbIt%' OR KpiCode LIKE 'Cbtd_%' OR KpiCode LIKE 'Gdv_%'
               OR KpiCode LIKE 'Cv_%' OR KpiCode LIKE 'Ksv_%' OR KpiCode LIKE 'Tp_%' OR KpiCode LIKE 'Ptp_%' THEN 1 END) as 'KPI cho cán bộ',
    COUNT(CASE WHEN KpiCode LIKE 'CnlaiChau_%' THEN 1 END) as 'KPI cho chi nhánh'
FROM KPIDefinitions;

PRINT '✅ HOÀN THÀNH! Đã có đủ KPI cho 23 bảng cán bộ và 9 bảng chi nhánh!';
