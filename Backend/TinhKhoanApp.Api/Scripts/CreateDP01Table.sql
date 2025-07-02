-- Script tạo bảng DP01 cho tính toán nguồn vốn
-- Chạy script này trong SQL Server Management Studio hoặc Azure Data Studio

USE [TinhKhoanAppDB] -- Tên database mặc định của project
GO

-- Kiểm tra và tạo bảng DP01 nếu chưa tồn tại
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DP01' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[DP01] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [DATA_DATE] datetime2 NOT NULL,
        [MA_CN] nvarchar(10) NULL,
        [MA_PGD] nvarchar(10) NULL,
        [TAI_KHOAN_HACH_TOAN] nvarchar(20) NULL,
        [CURRENT_BALANCE] decimal(18,2) NULL,
        [CREATED_DATE] datetime2 NULL,
        [UPDATED_DATE] datetime2 NULL,

        CONSTRAINT [PK_DP01] PRIMARY KEY ([Id])
    );

    -- Tạo các index để tối ưu hiệu suất
    CREATE NONCLUSTERED INDEX [IX_DP01_DateBranchPGD_Clustered]
    ON [dbo].[DP01] ([DATA_DATE], [MA_CN], [MA_PGD]);

    CREATE NONCLUSTERED INDEX [IX_DP01_Account]
    ON [dbo].[DP01] ([TAI_KHOAN_HACH_TOAN]);

    CREATE NONCLUSTERED INDEX [IX_DP01_Branch]
    ON [dbo].[DP01] ([MA_CN]);

    PRINT N'✅ Đã tạo bảng DP01 và các index thành công!';
END
ELSE
BEGIN
    PRINT N'⚠️ Bảng DP01 đã tồn tại!';
END
GO

-- Thêm một vài dữ liệu mẫu để test (optional)
-- Uncomment các dòng sau nếu muốn thêm dữ liệu test

/*
INSERT INTO [dbo].[DP01] ([DATA_DATE], [MA_CN], [MA_PGD], [TAI_KHOAN_HACH_TOAN], [CURRENT_BALANCE], [CREATED_DATE])
VALUES
    ('2024-12-31', '7800', NULL, '111101', 1000000000.00, GETDATE()),  -- Hội sở
    ('2024-12-31', '7801', NULL, '111102', 500000000.00, GETDATE()),   -- CN Bình Lư
    ('2024-12-31', '7802', NULL, '111103', 300000000.00, GETDATE()),   -- CN Phong Thổ
    ('2024-12-31', '7802', '01', '111104', 100000000.00, GETDATE()),   -- CN Phong Thổ - PGD Số 5
    ('2024-12-31', '7805', '01', '111105', 80000000.00, GETDATE()),    -- CN Than Uyên - PGD Số 6
    ('2024-12-31', '7806', '01', '111106', 150000000.00, GETDATE()),   -- CN Đoàn Kết - PGD Số 1
    ('2024-12-31', '7806', '02', '111107', 120000000.00, GETDATE()),   -- CN Đoàn Kết - PGD Số 2
    ('2024-12-31', '7800', NULL, '40101', 999999999.99, GETDATE()),    -- Tài khoản loại trừ (40*)
    ('2024-12-31', '7800', NULL, '41101', 888888888.88, GETDATE()),    -- Tài khoản loại trừ (41*)
    ('2024-12-31', '7800', NULL, '427101', 777777777.77, GETDATE()),   -- Tài khoản loại trừ (427*)
    ('2024-12-31', '7800', NULL, '211108', 666666666.66, GETDATE());   -- Tài khoản loại trừ (211108)

PRINT N'✅ Đã thêm dữ liệu mẫu để test!';
*/
