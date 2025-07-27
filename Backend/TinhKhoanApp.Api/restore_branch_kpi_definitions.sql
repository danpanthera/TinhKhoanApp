-- ================================================
-- Script phục hồi chỉ tiêu KPI cho 9 chi nhánh
-- Mỗi chi nhánh có 11 chỉ tiêu giống GiamdocCnl2
-- Tổng: 9 × 11 = 99 chỉ tiêu cho chi nhánh
-- ================================================

USE TinhKhoanDB;
GO

PRINT '🚀 Bắt đầu phục hồi 99 chỉ tiêu KPI cho 9 chi nhánh...';
PRINT '📋 Template: GiamdocCnl2 (11 chỉ tiêu) × 9 chi nhánh';

-- Tạo chỉ tiêu cho từng chi nhánh dựa trên template GiamdocCnl2

-- 1. HoiSo (200)
INSERT INTO KPIDefinitions (KpiCode, KpiName, Description, MaxScore, ValueType, UnitOfMeasure, IsActive, Version, CreatedDate) VALUES
('HoiSo_01', N'Tổng nguồn vốn cuối kỳ', N'Tổng nguồn vốn cuối kỳ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('HoiSo_02', N'Tổng nguồn vốn huy động BQ trong kỳ', N'Tổng nguồn vốn huy động bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('HoiSo_03', N'Tổng dư nợ cuối kỳ', N'Tổng dư nợ cuối kỳ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('HoiSo_04', N'Tổng dư nợ BQ trong kỳ', N'Tổng dư nợ bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('HoiSo_05', N'Tổng dư nợ HSX&CN', N'Tổng dư nợ hợp tác xã và chủ nợ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('HoiSo_06', N'Tỷ lệ nợ xấu nội bảng', N'Tỷ lệ nợ xấu nội bảng', 10.00, 2, N'%', 1, '1.0', GETUTCDATE()),
('HoiSo_07', N'Thu nợ đã XLRR', N'Thu nợ đã xử lý rủi ro', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('HoiSo_08', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, '1.0', GETUTCDATE()),
('HoiSo_09', N'Lợi nhuận khoán tài chính', N'Lợi nhuận khoán tài chính', 20.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('HoiSo_10', N'Thu dịch vụ', N'Thu dịch vụ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('HoiSo_11', N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10.00, 2, N'%', 1, '1.0', GETUTCDATE());

-- 2. CnBinhLu (201)
INSERT INTO KPIDefinitions (KpiCode, KpiName, Description, MaxScore, ValueType, UnitOfMeasure, IsActive, Version, CreatedDate) VALUES
('CnBinhLu_01', N'Tổng nguồn vốn cuối kỳ', N'Tổng nguồn vốn cuối kỳ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnBinhLu_02', N'Tổng nguồn vốn huy động BQ trong kỳ', N'Tổng nguồn vốn huy động bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnBinhLu_03', N'Tổng dư nợ cuối kỳ', N'Tổng dư nợ cuối kỳ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnBinhLu_04', N'Tổng dư nợ BQ trong kỳ', N'Tổng dư nợ bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnBinhLu_05', N'Tổng dư nợ HSX&CN', N'Tổng dư nợ hợp tác xã và chủ nợ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnBinhLu_06', N'Tỷ lệ nợ xấu nội bảng', N'Tỷ lệ nợ xấu nội bảng', 10.00, 2, N'%', 1, '1.0', GETUTCDATE()),
('CnBinhLu_07', N'Thu nợ đã XLRR', N'Thu nợ đã xử lý rủi ro', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnBinhLu_08', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, '1.0', GETUTCDATE()),
('CnBinhLu_09', N'Lợi nhuận khoán tài chính', N'Lợi nhuận khoán tài chính', 20.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnBinhLu_10', N'Thu dịch vụ', N'Thu dịch vụ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnBinhLu_11', N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10.00, 2, N'%', 1, '1.0', GETUTCDATE());

-- 3. CnPhongTho (202)
INSERT INTO KPIDefinitions (KpiCode, KpiName, Description, MaxScore, ValueType, UnitOfMeasure, IsActive, Version, CreatedDate) VALUES
('CnPhongTho_01', N'Tổng nguồn vốn cuối kỳ', N'Tổng nguồn vốn cuối kỳ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnPhongTho_02', N'Tổng nguồn vốn huy động BQ trong kỳ', N'Tổng nguồn vốn huy động bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnPhongTho_03', N'Tổng dư nợ cuối kỳ', N'Tổng dư nợ cuối kỳ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnPhongTho_04', N'Tổng dư nợ BQ trong kỳ', N'Tổng dư nợ bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnPhongTho_05', N'Tổng dư nợ HSX&CN', N'Tổng dư nợ hợp tác xã và chủ nợ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnPhongTho_06', N'Tỷ lệ nợ xấu nội bảng', N'Tỷ lệ nợ xấu nội bảng', 10.00, 2, N'%', 1, '1.0', GETUTCDATE()),
('CnPhongTho_07', N'Thu nợ đã XLRR', N'Thu nợ đã xử lý rủi ro', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnPhongTho_08', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, '1.0', GETUTCDATE()),
('CnPhongTho_09', N'Lợi nhuận khoán tài chính', N'Lợi nhuận khoán tài chính', 20.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnPhongTho_10', N'Thu dịch vụ', N'Thu dịch vụ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnPhongTho_11', N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10.00, 2, N'%', 1, '1.0', GETUTCDATE());

-- 4. CnSinHo (203)
INSERT INTO KPIDefinitions (KpiCode, KpiName, Description, MaxScore, ValueType, UnitOfMeasure, IsActive, Version, CreatedDate) VALUES
('CnSinHo_01', N'Tổng nguồn vốn cuối kỳ', N'Tổng nguồn vốn cuối kỳ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnSinHo_02', N'Tổng nguồn vốn huy động BQ trong kỳ', N'Tổng nguồn vốn huy động bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnSinHo_03', N'Tổng dư nợ cuối kỳ', N'Tổng dư nợ cuối kỳ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnSinHo_04', N'Tổng dư nợ BQ trong kỳ', N'Tổng dư nợ bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnSinHo_05', N'Tổng dư nợ HSX&CN', N'Tổng dư nợ hợp tác xã và chủ nợ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnSinHo_06', N'Tỷ lệ nợ xấu nội bảng', N'Tỷ lệ nợ xấu nội bảng', 10.00, 2, N'%', 1, '1.0', GETUTCDATE()),
('CnSinHo_07', N'Thu nợ đã XLRR', N'Thu nợ đã xử lý rủi ro', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnSinHo_08', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, '1.0', GETUTCDATE()),
('CnSinHo_09', N'Lợi nhuận khoán tài chính', N'Lợi nhuận khoán tài chính', 20.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnSinHo_10', N'Thu dịch vụ', N'Thu dịch vụ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnSinHo_11', N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10.00, 2, N'%', 1, '1.0', GETUTCDATE());

-- 5. CnBumTo (204)
INSERT INTO KPIDefinitions (KpiCode, KpiName, Description, MaxScore, ValueType, UnitOfMeasure, IsActive, Version, CreatedDate) VALUES
('CnBumTo_01', N'Tổng nguồn vốn cuối kỳ', N'Tổng nguồn vốn cuối kỳ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnBumTo_02', N'Tổng nguồn vốn huy động BQ trong kỳ', N'Tổng nguồn vốn huy động bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnBumTo_03', N'Tổng dư nợ cuối kỳ', N'Tổng dư nợ cuối kỳ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnBumTo_04', N'Tổng dư nợ BQ trong kỳ', N'Tổng dư nợ bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnBumTo_05', N'Tổng dư nợ HSX&CN', N'Tổng dư nợ hợp tác xã và chủ nợ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnBumTo_06', N'Tỷ lệ nợ xấu nội bảng', N'Tỷ lệ nợ xấu nội bảng', 10.00, 2, N'%', 1, '1.0', GETUTCDATE()),
('CnBumTo_07', N'Thu nợ đã XLRR', N'Thu nợ đã xử lý rủi ro', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnBumTo_08', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, '1.0', GETUTCDATE()),
('CnBumTo_09', N'Lợi nhuận khoán tài chính', N'Lợi nhuận khoán tài chính', 20.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnBumTo_10', N'Thu dịch vụ', N'Thu dịch vụ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnBumTo_11', N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10.00, 2, N'%', 1, '1.0', GETUTCDATE());

-- 6. CnThanUyen (205)
INSERT INTO KPIDefinitions (KpiCode, KpiName, Description, MaxScore, ValueType, UnitOfMeasure, IsActive, Version, CreatedDate) VALUES
('CnThanUyen_01', N'Tổng nguồn vốn cuối kỳ', N'Tổng nguồn vốn cuối kỳ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnThanUyen_02', N'Tổng nguồn vốn huy động BQ trong kỳ', N'Tổng nguồn vốn huy động bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnThanUyen_03', N'Tổng dư nợ cuối kỳ', N'Tổng dư nợ cuối kỳ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnThanUyen_04', N'Tổng dư nợ BQ trong kỳ', N'Tổng dư nợ bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnThanUyen_05', N'Tổng dư nợ HSX&CN', N'Tổng dư nợ hợp tác xã và chủ nợ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnThanUyen_06', N'Tỷ lệ nợ xấu nội bảng', N'Tỷ lệ nợ xấu nội bảng', 10.00, 2, N'%', 1, '1.0', GETUTCDATE()),
('CnThanUyen_07', N'Thu nợ đã XLRR', N'Thu nợ đã xử lý rủi ro', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnThanUyen_08', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, '1.0', GETUTCDATE()),
('CnThanUyen_09', N'Lợi nhuận khoán tài chính', N'Lợi nhuận khoán tài chính', 20.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnThanUyen_10', N'Thu dịch vụ', N'Thu dịch vụ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnThanUyen_11', N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10.00, 2, N'%', 1, '1.0', GETUTCDATE());

-- 7. CnDoanKet (206)
INSERT INTO KPIDefinitions (KpiCode, KpiName, Description, MaxScore, ValueType, UnitOfMeasure, IsActive, Version, CreatedDate) VALUES
('CnDoanKet_01', N'Tổng nguồn vốn cuối kỳ', N'Tổng nguồn vốn cuối kỳ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnDoanKet_02', N'Tổng nguồn vốn huy động BQ trong kỳ', N'Tổng nguồn vốn huy động bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnDoanKet_03', N'Tổng dư nợ cuối kỳ', N'Tổng dư nợ cuối kỳ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnDoanKet_04', N'Tổng dư nợ BQ trong kỳ', N'Tổng dư nợ bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnDoanKet_05', N'Tổng dư nợ HSX&CN', N'Tổng dư nợ hợp tác xã và chủ nợ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnDoanKet_06', N'Tỷ lệ nợ xấu nội bảng', N'Tỷ lệ nợ xấu nội bảng', 10.00, 2, N'%', 1, '1.0', GETUTCDATE()),
('CnDoanKet_07', N'Thu nợ đã XLRR', N'Thu nợ đã xử lý rủi ro', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnDoanKet_08', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, '1.0', GETUTCDATE()),
('CnDoanKet_09', N'Lợi nhuận khoán tài chính', N'Lợi nhuận khoán tài chính', 20.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnDoanKet_10', N'Thu dịch vụ', N'Thu dịch vụ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnDoanKet_11', N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10.00, 2, N'%', 1, '1.0', GETUTCDATE());

-- 8. CnTanUyen (207)
INSERT INTO KPIDefinitions (KpiCode, KpiName, Description, MaxScore, ValueType, UnitOfMeasure, IsActive, Version, CreatedDate) VALUES
('CnTanUyen_01', N'Tổng nguồn vốn cuối kỳ', N'Tổng nguồn vốn cuối kỳ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnTanUyen_02', N'Tổng nguồn vốn huy động BQ trong kỳ', N'Tổng nguồn vốn huy động bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnTanUyen_03', N'Tổng dư nợ cuối kỳ', N'Tổng dư nợ cuối kỳ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnTanUyen_04', N'Tổng dư nợ BQ trong kỳ', N'Tổng dư nợ bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnTanUyen_05', N'Tổng dư nợ HSX&CN', N'Tổng dư nợ hợp tác xã và chủ nợ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnTanUyen_06', N'Tỷ lệ nợ xấu nội bảng', N'Tỷ lệ nợ xấu nội bảng', 10.00, 2, N'%', 1, '1.0', GETUTCDATE()),
('CnTanUyen_07', N'Thu nợ đã XLRR', N'Thu nợ đã xử lý rủi ro', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnTanUyen_08', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, '1.0', GETUTCDATE()),
('CnTanUyen_09', N'Lợi nhuận khoán tài chính', N'Lợi nhuận khoán tài chính', 20.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnTanUyen_10', N'Thu dịch vụ', N'Thu dịch vụ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnTanUyen_11', N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10.00, 2, N'%', 1, '1.0', GETUTCDATE());

-- 9. CnNamHang (208)
INSERT INTO KPIDefinitions (KpiCode, KpiName, Description, MaxScore, ValueType, UnitOfMeasure, IsActive, Version, CreatedDate) VALUES
('CnNamHang_01', N'Tổng nguồn vốn cuối kỳ', N'Tổng nguồn vốn cuối kỳ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnNamHang_02', N'Tổng nguồn vốn huy động BQ trong kỳ', N'Tổng nguồn vốn huy động bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnNamHang_03', N'Tổng dư nợ cuối kỳ', N'Tổng dư nợ cuối kỳ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnNamHang_04', N'Tổng dư nợ BQ trong kỳ', N'Tổng dư nợ bình quân trong kỳ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnNamHang_05', N'Tổng dư nợ HSX&CN', N'Tổng dư nợ hợp tác xã và chủ nợ', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnNamHang_06', N'Tỷ lệ nợ xấu nội bảng', N'Tỷ lệ nợ xấu nội bảng', 10.00, 2, N'%', 1, '1.0', GETUTCDATE()),
('CnNamHang_07', N'Thu nợ đã XLRR', N'Thu nợ đã xử lý rủi ro', 5.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnNamHang_08', N'Phát triển Khách hàng', N'Phát triển khách hàng mới', 10.00, 1, N'Khách hàng', 1, '1.0', GETUTCDATE()),
('CnNamHang_09', N'Lợi nhuận khoán tài chính', N'Lợi nhuận khoán tài chính', 20.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnNamHang_10', N'Thu dịch vụ', N'Thu dịch vụ', 10.00, 4, N'Triệu VND', 1, '1.0', GETUTCDATE()),
('CnNamHang_11', N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10.00, 2, N'%', 1, '1.0', GETUTCDATE());

-- Kiểm tra kết quả
PRINT '✅ Hoàn thành phục hồi chỉ tiêu cho 9 chi nhánh!';

PRINT '📊 Tổng số chỉ tiêu hiện tại:';
SELECT COUNT(*) as TotalKPIDefinitions FROM KPIDefinitions;

PRINT '📋 Tổng số chỉ tiêu theo loại:';
SELECT
    CASE
        WHEN KpiCode LIKE 'HoiSo_%' OR KpiCode LIKE 'Cn%_%' THEN 'Chi nhánh'
        ELSE 'Cán bộ'
    END as Category,
    COUNT(*) as Count
FROM KPIDefinitions
GROUP BY
    CASE
        WHEN KpiCode LIKE 'HoiSo_%' OR KpiCode LIKE 'Cn%_%' THEN 'Chi nhánh'
        ELSE 'Cán bộ'
    END
ORDER BY Count DESC;

PRINT '📋 Chi tiết chỉ tiêu chi nhánh:';
SELECT
    CASE
        WHEN KpiCode LIKE 'HoiSo_%' THEN 'Hội sở'
        WHEN KpiCode LIKE 'CnBinhLu_%' THEN 'Chi nhánh Bình Lư'
        WHEN KpiCode LIKE 'CnPhongTho_%' THEN 'Chi nhánh Phong Thổ'
        WHEN KpiCode LIKE 'CnSinHo_%' THEN 'Chi nhánh Sin Hồ'
        WHEN KpiCode LIKE 'CnBumTo_%' THEN 'Chi nhánh Bum Tở'
        WHEN KpiCode LIKE 'CnThanUyen_%' THEN 'Chi nhánh Than Uyên'
        WHEN KpiCode LIKE 'CnDoanKet_%' THEN 'Chi nhánh Đoàn Kết'
        WHEN KpiCode LIKE 'CnTanUyen_%' THEN 'Chi nhánh Tân Uyên'
        WHEN KpiCode LIKE 'CnNamHang_%' THEN 'Chi nhánh Nậm Hàng'
        ELSE 'Khác'
    END as BranchName,
    COUNT(*) as KPICount
FROM KPIDefinitions
WHERE KpiCode LIKE 'HoiSo_%' OR KpiCode LIKE 'Cn%_%'
GROUP BY
    CASE
        WHEN KpiCode LIKE 'HoiSo_%' THEN 'Hội sở'
        WHEN KpiCode LIKE 'CnBinhLu_%' THEN 'Chi nhánh Bình Lư'
        WHEN KpiCode LIKE 'CnPhongTho_%' THEN 'Chi nhánh Phong Thổ'
        WHEN KpiCode LIKE 'CnSinHo_%' THEN 'Chi nhánh Sin Hồ'
        WHEN KpiCode LIKE 'CnBumTo_%' THEN 'Chi nhánh Bum Tở'
        WHEN KpiCode LIKE 'CnThanUyen_%' THEN 'Chi nhánh Than Uyên'
        WHEN KpiCode LIKE 'CnDoanKet_%' THEN 'Chi nhánh Đoàn Kết'
        WHEN KpiCode LIKE 'CnTanUyen_%' THEN 'Chi nhánh Tân Uyên'
        WHEN KpiCode LIKE 'CnNamHang_%' THEN 'Chi nhánh Nậm Hàng'
        ELSE 'Khác'
    END
ORDER BY KPICount DESC;
