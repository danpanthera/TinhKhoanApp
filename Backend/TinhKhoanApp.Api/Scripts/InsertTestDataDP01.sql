-- Script thêm dữ liệu mẫu cho bảng DP01
-- Để test chức năng tính toán nguồn vốn

USE [TinhKhoanAppDB] -- Thay thế bằng tên database thực tế
GO

-- Xóa dữ liệu cũ nếu có (chỉ dành cho test)
DELETE FROM [DP01] WHERE CREATED_DATE IS NOT NULL;

-- Thêm dữ liệu mẫu cho ngày 31/12/2024 (cuối năm)
INSERT INTO [dbo].[DP01] ([DATA_DATE], [MA_CN], [MA_PGD], [TAI_KHOAN_HACH_TOAN], [CURRENT_BALANCE], [CREATED_DATE])
VALUES 
    -- Hội sở (7800)
    ('2024-12-31', '7800', NULL, '111101', 1500000000.00, GETDATE()),  -- 1.5 tỷ
    ('2024-12-31', '7800', NULL, '111102', 800000000.00, GETDATE()),   -- 800 triệu
    ('2024-12-31', '7800', NULL, '111103', 650000000.00, GETDATE()),   -- 650 triệu
    
    -- Chi nhánh Bình Lư (7801)
    ('2024-12-31', '7801', NULL, '121101', 500000000.00, GETDATE()),   -- 500 triệu
    ('2024-12-31', '7801', NULL, '121102', 350000000.00, GETDATE()),   -- 350 triệu
    ('2024-12-31', '7801', NULL, '121103', 200000000.00, GETDATE()),   -- 200 triệu
    
    -- Chi nhánh Phong Thổ (7802)
    ('2024-12-31', '7802', NULL, '131101', 400000000.00, GETDATE()),   -- 400 triệu
    ('2024-12-31', '7802', NULL, '131102', 300000000.00, GETDATE()),   -- 300 triệu
    
    -- Chi nhánh Phong Thổ - PGD Số 5 (7802 + 01)
    ('2024-12-31', '7802', '01', '131201', 150000000.00, GETDATE()),   -- 150 triệu
    ('2024-12-31', '7802', '01', '131202', 100000000.00, GETDATE()),   -- 100 triệu
    
    -- Chi nhánh Sìn Hồ (7803)
    ('2024-12-31', '7803', NULL, '141101', 280000000.00, GETDATE()),   -- 280 triệu
    ('2024-12-31', '7803', NULL, '141102', 220000000.00, GETDATE()),   -- 220 triệu
    
    -- Chi nhánh Bum Tở (7804)
    ('2024-12-31', '7804', NULL, '151101', 320000000.00, GETDATE()),   -- 320 triệu
    ('2024-12-31', '7804', NULL, '151102', 180000000.00, GETDATE()),   -- 180 triệu
    
    -- Chi nhánh Than Uyên (7805)
    ('2024-12-31', '7805', NULL, '161101', 450000000.00, GETDATE()),   -- 450 triệu
    ('2024-12-31', '7805', NULL, '161102', 380000000.00, GETDATE()),   -- 380 triệu
    
    -- Chi nhánh Than Uyên - PGD Số 6 (7805 + 01)
    ('2024-12-31', '7805', '01', '161201', 120000000.00, GETDATE()),   -- 120 triệu
    ('2024-12-31', '7805', '01', '161202', 80000000.00, GETDATE()),    -- 80 triệu
    
    -- Chi nhánh Đoàn Kết (7806)
    ('2024-12-31', '7806', NULL, '171101', 600000000.00, GETDATE()),   -- 600 triệu
    ('2024-12-31', '7806', NULL, '171102', 420000000.00, GETDATE()),   -- 420 triệu
    
    -- Chi nhánh Đoàn Kết - PGD Số 1 (7806 + 01)
    ('2024-12-31', '7806', '01', '171201', 200000000.00, GETDATE()),   -- 200 triệu
    ('2024-12-31', '7806', '01', '171202', 150000000.00, GETDATE()),   -- 150 triệu
    
    -- Chi nhánh Đoàn Kết - PGD Số 2 (7806 + 02)
    ('2024-12-31', '7806', '02', '171301', 180000000.00, GETDATE()),   -- 180 triệu
    ('2024-12-31', '7806', '02', '171302', 120000000.00, GETDATE()),   -- 120 triệu
    
    -- Chi nhánh Tân Uyên (7807)
    ('2024-12-31', '7807', NULL, '181101', 380000000.00, GETDATE()),   -- 380 triệu
    ('2024-12-31', '7807', NULL, '181102', 290000000.00, GETDATE()),   -- 290 triệu
    
    -- Chi nhánh Tân Uyên - PGD Số 3 (7807 + 01)
    ('2024-12-31', '7807', '01', '181201', 130000000.00, GETDATE()),   -- 130 triệu
    ('2024-12-31', '7807', '01', '181202', 90000000.00, GETDATE()),    -- 90 triệu
    
    -- Chi nhánh Nậm Hàng (7808)
    ('2024-12-31', '7808', NULL, '191101', 250000000.00, GETDATE()),   -- 250 triệu
    ('2024-12-31', '7808', NULL, '191102', 180000000.00, GETDATE()),   -- 180 triệu
    
    -- Các tài khoản loại trừ (không tính vào nguồn vốn)
    ('2024-12-31', '7800', NULL, '40101', 999999999.99, GETDATE()),    -- TK 40* (loại trừ)
    ('2024-12-31', '7800', NULL, '41101', 888888888.88, GETDATE()),    -- TK 41* (loại trừ)
    ('2024-12-31', '7800', NULL, '427101', 777777777.77, GETDATE()),   -- TK 427* (loại trừ)
    ('2024-12-31', '7800', NULL, '211108', 666666666.66, GETDATE());   -- TK 211108 (loại trừ)

-- Thêm dữ liệu cho tháng 11/2024 (để test tháng)
INSERT INTO [dbo].[DP01] ([DATA_DATE], [MA_CN], [MA_PGD], [TAI_KHOAN_HACH_TOAN], [CURRENT_BALANCE], [CREATED_DATE])
VALUES 
    -- Hội sở tháng 11
    ('2024-11-30', '7800', NULL, '111101', 1400000000.00, GETDATE()),  -- 1.4 tỷ
    ('2024-11-30', '7800', NULL, '111102', 750000000.00, GETDATE()),   -- 750 triệu
    
    -- Chi nhánh Bình Lư tháng 11
    ('2024-11-30', '7801', NULL, '121101', 480000000.00, GETDATE()),   -- 480 triệu
    ('2024-11-30', '7801', NULL, '121102', 320000000.00, GETDATE()),   -- 320 triệu
    
    -- Chi nhánh Phong Thổ - PGD Số 5 tháng 11
    ('2024-11-30', '7802', '01', '131201', 140000000.00, GETDATE()),   -- 140 triệu
    ('2024-11-30', '7802', '01', '131202', 95000000.00, GETDATE());    -- 95 triệu

-- Thêm dữ liệu cho ngày 15/11/2024 (để test ngày cụ thể)
INSERT INTO [dbo].[DP01] ([DATA_DATE], [MA_CN], [MA_PGD], [TAI_KHOAN_HACH_TOAN], [CURRENT_BALANCE], [CREATED_DATE])
VALUES 
    -- Hội sở ngày 15/11
    ('2024-11-15', '7800', NULL, '111101', 1350000000.00, GETDATE()),  -- 1.35 tỷ
    ('2024-11-15', '7800', NULL, '111102', 720000000.00, GETDATE()),   -- 720 triệu
    
    -- Chi nhánh Đoàn Kết - PGD Số 1 ngày 15/11
    ('2024-11-15', '7806', '01', '171201', 190000000.00, GETDATE()),   -- 190 triệu
    ('2024-11-15', '7806', '01', '171202', 145000000.00, GETDATE());   -- 145 triệu

PRINT N'✅ Đã thêm dữ liệu mẫu DP01 thành công!';
PRINT N'📊 Tổng số bản ghi đã thêm: ' + CAST(@@ROWCOUNT AS NVARCHAR);

-- Hiển thị thống kê
SELECT 
    DATA_DATE,
    COUNT(*) as SoLuongBanGhi,
    SUM(CURRENT_BALANCE) as TongSoDu,
    COUNT(DISTINCT MA_CN) as SoChiNhanh,
    COUNT(DISTINCT CASE WHEN MA_PGD IS NOT NULL THEN CONCAT(MA_CN, '_', MA_PGD) END) as SoPGD
FROM [DP01] 
WHERE CREATED_DATE IS NOT NULL
GROUP BY DATA_DATE
ORDER BY DATA_DATE DESC;

PRINT N'📋 Dữ liệu test đã sẵn sàng cho API /api/NguonVon/calculate';
