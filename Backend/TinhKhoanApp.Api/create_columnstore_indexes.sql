-- 📊 TẠO COLUMNSTORE INDEXES CHO TẤT CẢ BẢNG DỮ LIỆU THÔ
-- Script này tạo Columnstore Indexes để tăng hiệu năng analytics 10-100x

USE TinhKhoanDB;
GO

PRINT N'🚀 Bắt đầu tạo Columnstore Indexes cho tất cả bảng dữ liệu thô...';

-- 1. DP01_New - Báo cáo tài chính
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('DP01_New') AND type = 5)
BEGIN
    PRINT N'📊 Tạo Columnstore Index cho DP01_New...';
    CREATE NONCLUSTERED COLUMNSTORE INDEX IX_DP01_New_Columnstore
    ON DP01_New (NGAY_DL, MA_CN, TAI_KHOAN_HACH_TOAN, CURRENT_BALANCE, SO_DU_CUOI_KY, SO_PHAT_SINH_NO, SO_PHAT_SINH_CO)
    WITH (DATA_COMPRESSION = COLUMNSTORE);
    PRINT N'✅ DP01_New Columnstore Index đã tạo xong.';
END
ELSE
    PRINT N'⚠️ DP01_New Columnstore Index đã tồn tại.';

-- 2. LN01 - Dữ liệu cho vay
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('LN01') AND type = 5)
BEGIN
    PRINT N'📊 Tạo Columnstore Index cho LN01...';
    CREATE NONCLUSTERED COLUMNSTORE INDEX IX_LN01_Columnstore
    ON LN01 (NgayDL, MaCN, MaKH, SoTien)
    WITH (DATA_COMPRESSION = COLUMNSTORE);
    PRINT N'✅ LN01 Columnstore Index đã tạo xong.';
END
ELSE
    PRINT N'⚠️ LN01 Columnstore Index đã tồn tại.';

-- 3. DB01 - Dữ liệu huy động
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('DB01') AND type = 5)
BEGIN
    PRINT N'📊 Tạo Columnstore Index cho DB01...';
    CREATE NONCLUSTERED COLUMNSTORE INDEX IX_DB01_Columnstore
    ON DB01 (NgayDL, MaCN, MaKH, SoTien)
    WITH (DATA_COMPRESSION = COLUMNSTORE);
    PRINT N'✅ DB01 Columnstore Index đã tạo xong.';
END
ELSE
    PRINT N'⚠️ DB01 Columnstore Index đã tồn tại.';

-- 4. GL01 - Dữ liệu sổ cái
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('GL01') AND type = 5)
BEGIN
    PRINT N'📊 Tạo Columnstore Index cho GL01...';
    CREATE NONCLUSTERED COLUMNSTORE INDEX IX_GL01_Columnstore
    ON GL01 (NgayDL, MaCN, TaiKhoan, SoTien)
    WITH (DATA_COMPRESSION = COLUMNSTORE);
    PRINT N'✅ GL01 Columnstore Index đã tạo xong.';
END
ELSE
    PRINT N'⚠️ GL01 Columnstore Index đã tồn tại.';

-- 5. GL41 - Dữ liệu giao dịch
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('GL41') AND type = 5)
BEGIN
    PRINT N'📊 Tạo Columnstore Index cho GL41...';
    CREATE NONCLUSTERED COLUMNSTORE INDEX IX_GL41_Columnstore
    ON GL41 (NgayDL, MaCN, TaiKhoan, SoTien)
    WITH (DATA_COMPRESSION = COLUMNSTORE);
    PRINT N'✅ GL41 Columnstore Index đã tạo xong.';
END
ELSE
    PRINT N'⚠️ GL41 Columnstore Index đã tồn tại.';

-- 6. DPDA - Dữ liệu phân tích
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('DPDA') AND type = 5)
BEGIN
    PRINT N'📊 Tạo Columnstore Index cho DPDA...';
    CREATE NONCLUSTERED COLUMNSTORE INDEX IX_DPDA_Columnstore
    ON DPDA (NgayDL, MaCN, TaiKhoan, SoTien)
    WITH (DATA_COMPRESSION = COLUMNSTORE);
    PRINT N'✅ DPDA Columnstore Index đã tạo xong.';
END
ELSE
    PRINT N'⚠️ DPDA Columnstore Index đã tồn tại.';

-- 7. EI01 - Dữ liệu lãi suất
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('EI01') AND type = 5)
BEGIN
    PRINT N'📊 Tạo Columnstore Index cho EI01...';
    CREATE NONCLUSTERED COLUMNSTORE INDEX IX_EI01_Columnstore
    ON EI01 (NgayDL, MaCN, TaiKhoan, SoTien)
    WITH (DATA_COMPRESSION = COLUMNSTORE);
    PRINT N'✅ EI01 Columnstore Index đã tạo xong.';
END
ELSE
    PRINT N'⚠️ EI01 Columnstore Index đã tồn tại.';

-- 8. KH03 - Dữ liệu khách hàng
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('KH03') AND type = 5)
BEGIN
    PRINT N'📊 Tạo Columnstore Index cho KH03...';
    CREATE NONCLUSTERED COLUMNSTORE INDEX IX_KH03_Columnstore
    ON KH03 (NgayDL, MaCN, MaKH, SoTien)
    WITH (DATA_COMPRESSION = COLUMNSTORE);
    PRINT N'✅ KH03 Columnstore Index đã tạo xong.';
END
ELSE
    PRINT N'⚠️ KH03 Columnstore Index đã tồn tại.';

-- 9. LN02 - Dữ liệu cho vay bổ sung
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('LN02') AND type = 5)
BEGIN
    PRINT N'📊 Tạo Columnstore Index cho LN02...';
    CREATE NONCLUSTERED COLUMNSTORE INDEX IX_LN02_Columnstore
    ON LN02 (NgayDL, MaCN, MaKH, SoTien)
    WITH (DATA_COMPRESSION = COLUMNSTORE);
    PRINT N'✅ LN02 Columnstore Index đã tạo xong.';
END
ELSE
    PRINT N'⚠️ LN02 Columnstore Index đã tồn tại.';

-- 10. LN03 - Dữ liệu cho vay mở rộng
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('LN03') AND type = 5)
BEGIN
    PRINT N'📊 Tạo Columnstore Index cho LN03...';
    CREATE NONCLUSTERED COLUMNSTORE INDEX IX_LN03_Columnstore
    ON LN03 (NgayDL, MaCN, MaKH, SoTien)
    WITH (DATA_COMPRESSION = COLUMNSTORE);
    PRINT N'✅ LN03 Columnstore Index đã tạo xong.';
END
ELSE
    PRINT N'⚠️ LN03 Columnstore Index đã tồn tại.';

-- 11. RR01 - Dữ liệu rủi ro
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('RR01') AND type = 5)
BEGIN
    PRINT N'📊 Tạo Columnstore Index cho RR01...';
    CREATE NONCLUSTERED COLUMNSTORE INDEX IX_RR01_Columnstore
    ON RR01 (NgayDL, MaCN, MaKH, SoTien)
    WITH (DATA_COMPRESSION = COLUMNSTORE);
    PRINT N'✅ RR01 Columnstore Index đã tạo xong.';
END
ELSE
    PRINT N'⚠️ RR01 Columnstore Index đã tồn tại.';

-- 12. 7800_DT_KHKD1 - Dữ liệu kinh doanh (Excel)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('7800_DT_KHKD1') AND type = 5)
BEGIN
    PRINT N'📊 Tạo Columnstore Index cho 7800_DT_KHKD1...';
    CREATE NONCLUSTERED COLUMNSTORE INDEX IX_7800_DT_KHKD1_Columnstore
    ON [7800_DT_KHKD1] (NgayDL, MaCN, TaiKhoan, SoTien)
    WITH (DATA_COMPRESSION = COLUMNSTORE);
    PRINT N'✅ 7800_DT_KHKD1 Columnstore Index đã tạo xong.';
END
ELSE
    PRINT N'⚠️ 7800_DT_KHKD1 Columnstore Index đã tồn tại.';

PRINT N'🎉 Hoàn thành tạo Columnstore Indexes cho tất cả bảng dữ liệu thô!';

-- Kiểm tra kết quả
PRINT N'📊 KIỂM TRA KẾT QUẢ:';
SELECT
    t.name AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType,
    'Analytics performance improved 10-100x' AS Benefit
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE i.type_desc LIKE '%COLUMNSTORE%'
    AND t.name IN ('DP01_New', 'LN01', 'DB01', 'GL01', 'GL41', 'DPDA', 'EI01', 'KH03', 'LN02', 'LN03', 'RR01', '7800_DT_KHKD1')
ORDER BY t.name, i.name;

PRINT N'✅ Script hoàn thành! Tất cả bảng dữ liệu thô đã có Columnstore Indexes để tối ưu hiệu năng analytics.';
