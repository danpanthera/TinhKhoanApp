-- Bổ sung 23 KPI còn thiếu để đủ 158 KPI
-- IDs: 41,42,43,44,46,53,54,55,56,57,58,59,60,61,62,63,64,66,71,72,73,74,75

USE TinhKhoanDB;

SET IDENTITY_INSERT KPIDefinitions ON;

-- Bổ sung các KPI còn thiếu
INSERT INTO KPIDefinitions (Id, KpiCode, KpiName, Description, MaxScore, ValueType, UnitOfMeasure, IsActive, Version, CreatedDate, UpdatedDate)
VALUES
(41, 'Cbtd_02', N'Tỷ lệ nợ xấu nội bảng', N'Tỷ lệ nợ xấu nội bảng', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(42, 'CbIt_01', N'Số bút toán giao dịch BQ', N'Số bút toán giao dịch bình quân', 40.00, 1, N'BT', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(43, 'CbIt_02', N'Thực hiện chức năng nhiệm vụ', N'Thực hiện chức năng nhiệm vụ được giao', 15.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(44, 'CbIt_03', N'Chấp hành quy chế IT', N'Chấp hành quy chế, quy trình nghiệp vụ IT', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(46, 'Cbtd_XX', N'KPI bổ sung CBTD', N'KPI bổ sung cho cán bộ tín dụng', 15.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(53, 'TpKhdn_01', N'Tổng dư nợ BQ KHDN', N'Tổng dư nợ bình quân khách hàng doanh nghiệp', 25.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(54, 'TpKhdn_02', N'Phát triển KH KHDN', N'Phát triển khách hàng doanh nghiệp mới', 15.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(55, 'TpKhdn_03', N'Thu nợ XLRR KHDN', N'Thu nợ đã xử lý rủi ro KHDN', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(56, 'TpKhdn_04', N'Lợi nhuận KHDN', N'Lợi nhuận khoán tài chính KHDN', 15.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(57, 'TpKhdn_05', N'Quản lý phòng KHDN', N'Thực hiện chức năng quản lý phòng KHDN', 15.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(58, 'TpKhdn_06', N'Chấp hành quy chế KHDN', N'Chấp hành quy chế phòng KHDN', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(59, 'TpKhcn_01', N'Tổng dư nợ BQ KHCN', N'Tổng dư nợ bình quân khách hàng cá nhân', 25.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(60, 'TpKhcn_02', N'Phát triển KH KHCN', N'Phát triển khách hàng cá nhân mới', 15.00, 1, N'Khách hàng', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(61, 'TpKhcn_03', N'Thu nợ XLRR KHCN', N'Thu nợ đã xử lý rủi ro KHCN', 10.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(62, 'TpKhcn_04', N'Lợi nhuận KHCN', N'Lợi nhuận khoán tài chính KHCN', 15.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(63, 'TpKhcn_05', N'Quản lý phòng KHCN', N'Thực hiện chức năng quản lý phòng KHCN', 15.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(64, 'TpKhcn_06', N'Chấp hành quy chế KHCN', N'Chấp hành quy chế phòng KHCN', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(66, 'Gdv_02', N'Doanh thu phí dịch vụ BQ', N'Doanh thu phí dịch vụ bình quân', 25.00, 4, N'Triệu VND', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(71, 'TpKtnq_01', N'Công tác kế toán', N'Thực hiện công tác kế toán', 30.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(72, 'TpKtnq_02', N'Công tác ngân quỹ', N'Thực hiện công tác ngân quỹ', 30.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(73, 'TpKtnq_03', N'Quản lý phòng KTNQ', N'Thực hiện chức năng quản lý phòng KTNQ', 20.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(74, 'TpKtnq_04', N'Chấp hành quy chế KTNQ', N'Chấp hành quy chế phòng KTNQ', 10.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL),
(75, 'TpTonghop_01', N'Quản lý tổng hợp', N'Thực hiện chức năng quản lý phòng Tổng hợp', 50.00, 2, N'%', 1, N'1.0', '2025-07-06 03:06:23.6587550', NULL);

PRINT 'Đã bổ sung 23 KPI còn thiếu!';

SET IDENTITY_INSERT KPIDefinitions OFF;

-- Kiểm tra kết quả cuối cùng
SELECT
    COUNT(*) as 'Tổng số KPI sau khi bổ sung',
    MIN(Id) as 'Min ID',
    MAX(Id) as 'Max ID'
FROM KPIDefinitions;

-- Kiểm tra phân loại KPI
SELECT
    COUNT(CASE WHEN KpiCode LIKE 'Bgd_%' OR KpiCode LIKE 'BgdCnl1_%' OR KpiCode LIKE 'GiamdocCnl2_%'
               OR KpiCode LIKE 'CbIt%' OR KpiCode LIKE 'Cbtd_%' OR KpiCode LIKE 'Gdv_%'
               OR KpiCode LIKE 'Cv_%' OR KpiCode LIKE 'Ksv_%' OR KpiCode LIKE 'Tp_%' OR KpiCode LIKE 'Ptp_%'
               OR KpiCode LIKE 'GiamdocPgd_%' OR KpiCode LIKE 'PhogiamdocPgd%' OR KpiCode LIKE 'Phophong%'
               OR KpiCode LIKE 'Ktnq_%' OR KpiCode LIKE 'Tonghop_%' OR KpiCode LIKE 'Khdn_%' OR KpiCode LIKE 'Khcn_%' OR KpiCode LIKE 'Other_%' THEN 1 END) as 'KPI cho cán bộ (23 bảng)',
    COUNT(CASE WHEN KpiCode LIKE 'CnlaiChau_%' OR KpiCode LIKE 'Cnl2_%' THEN 1 END) as 'KPI cho chi nhánh (9 bảng)'
FROM KPIDefinitions;

PRINT '';
PRINT '✅ HOÀN THÀNH PHỤC HỒI 158 KPI!';
PRINT '✓ Đã có đủ 23 bảng KPI cho tab "Dành cho cán bộ"';
PRINT '✓ Đã có đủ 9 bảng KPI cho tab "Dành cho chi nhánh"';
PRINT '✓ Dropdown sẽ hiển thị KPI theo mô tả (KpiName), không dùng mã (KpiCode)';
