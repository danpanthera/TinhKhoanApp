-- ========================================
-- 🕰️ LN03 TEMPORAL TABLE CONVERSION SCRIPT (CORRECTED)
-- ========================================
-- Chuyển đổi LN03 từ bảng thường thành Temporal Table

USE TinhKhoanDB;
GO

PRINT '🕰️ BẮT ĐẦU CHUYỂN ĐỔI LN03 THÀNH TEMPORAL TABLE...';

-- ========================================
-- 1. KIỂM TRA TRẠNG THÁI HIỆN TẠI
-- ========================================
SELECT 
    name,
    temporal_type_desc,
    CASE WHEN temporal_type = 0 THEN 'NON-TEMPORAL' 
         ELSE 'TEMPORAL' END as Status
FROM sys.tables 
WHERE name = 'LN03';

-- ========================================
-- 2. THÊM CÁC CỘT TEMPORAL BƯỚC TỪNG BƯỚC
-- ========================================
PRINT 'Đang thêm cột SysStartTime...';

-- Bước 1: Thêm cột SysStartTime với default value
ALTER TABLE LN03 
ADD SysStartTime datetime2 NOT NULL DEFAULT SYSUTCDATETIME();

PRINT 'Đã thêm SysStartTime.';

PRINT 'Đang thêm cột SysEndTime...';

-- Bước 2: Thêm cột SysEndTime với max value  
ALTER TABLE LN03 
ADD SysEndTime datetime2 NOT NULL DEFAULT CONVERT(datetime2, '9999-12-31 23:59:59.9999999');

PRINT 'Đã thêm SysEndTime.';

-- ========================================
-- 3. TẠO PERIOD DEFINITION
-- ========================================
PRINT 'Đang tạo Period definition...';

ALTER TABLE LN03 
ADD PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);

PRINT 'Đã tạo Period definition.';

-- ========================================  
-- 4. SỬA ĐỔI CÁC CỘT THÀNH GENERATED ALWAYS
-- ========================================
PRINT 'Đang chuyển đổi các cột thành Generated Always...';

-- Sửa SysStartTime thành Generated Always
ALTER TABLE LN03 
ALTER COLUMN SysStartTime ADD GENERATED ALWAYS AS ROW START;

-- Sửa SysEndTime thành Generated Always  
ALTER TABLE LN03 
ALTER COLUMN SysEndTime ADD GENERATED ALWAYS AS ROW END;

PRINT 'Đã chuyển đổi các cột temporal.';

-- ========================================
-- 5. TẠO HISTORY TABLE
-- ========================================
PRINT 'Đang tạo History table...';

-- Tạo bảng history với cùng structure (trừ temporal columns sẽ được thêm tự động)
CREATE TABLE LN03_History (
    Id int,
    NGAY_DL datetime2,
    MACHINHANH nvarchar(50),
    TENCHINHANH nvarchar(200),
    MAKH nvarchar(200),
    TENKH nvarchar(500),
    MACBTD nvarchar(50),
    TENCBTD nvarchar(200),
    TAIKHOANHACHTOAN nvarchar(20),
    SOHOPDONG nvarchar(100),
    REFNO nvarchar(100),
    NHOMNO nvarchar(50),
    NGAYPHATSINHXL datetime2,
    MAPGD nvarchar(50),
    LOAINGUONVON nvarchar(100),
    DUNONOIBANG decimal(18,2),
    CONLAINGOAIBANG decimal(18,2),
    SOTIENXLRR decimal(18,2),
    COLUMN_18 nvarchar(max),
    COLUMN_19 nvarchar(max),
    COLUMN_20 nvarchar(max),
    THUNOSAUXL decimal(18,2),
    IS_DELETED bit,
    CREATED_DATE datetime2,
    UPDATED_DATE datetime2,
    SysStartTime datetime2 NOT NULL,
    SysEndTime datetime2 NOT NULL
);

-- Tạo clustered index cho history table
CREATE CLUSTERED INDEX CIX_LN03_History 
ON LN03_History (SysEndTime, SysStartTime);

PRINT 'Đã tạo History table.';

-- ========================================
-- 6. ENABLE SYSTEM VERSIONING
-- ========================================
PRINT 'Đang bật System Versioning...';

ALTER TABLE LN03 
SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.LN03_History));

PRINT 'System Versioning đã được bật!';

-- ========================================
-- 7. KIỂM TRA KẾT QUẢ
-- ========================================
PRINT '';
PRINT 'Kiểm tra trạng thái temporal:';

SELECT 
    t.name AS TableName,
    t.temporal_type_desc AS TemporalType,
    h.name AS HistoryTableName
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id  
WHERE t.name = 'LN03';

PRINT '';
PRINT '🎉 TEMPORAL TABLE CONVERSION HOÀN THÀNH!';
PRINT 'LN03 bây giờ là System-Versioned Temporal Table với lịch sử tự động.';

GO
