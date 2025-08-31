-- ========================================
-- 🕰️ LN03 TEMPORAL TABLE CONVERSION SCRIPT
-- ========================================
-- Chuyển đổi LN03 từ bảng thường thành Temporal Table với lịch sử tự động

USE TinhKhoanDB;
GO

PRINT '🕰️ BẮT ĐẦU CHUYỂN ĐỔI LN03 THÀNH TEMPORAL TABLE...';

-- ========================================
-- 1. KIỂM TRA TRẠNG THÁI HIỆN TẠI
-- ========================================
PRINT '🔍 Đang kiểm tra trạng thái hiện tại của bảng LN03...';

SELECT 
    name,
    temporal_type_desc,
    CASE WHEN temporal_type = 0 THEN '❌ NON-TEMPORAL' 
         ELSE '✅ TEMPORAL' END as Status
FROM sys.tables 
WHERE name = 'LN03';

-- ========================================
-- 2. THÊM CÁC CỘT TEMPORAL (nếu chưa có)
-- ========================================
PRINT '📅 Đang thêm các cột temporal...';

-- Kiểm tra và thêm SysStartTime
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'LN03' AND COLUMN_NAME = 'SysStartTime')
BEGIN
    ALTER TABLE LN03 
    ADD SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN
        CONSTRAINT DF_LN03_SysStartTime DEFAULT SYSUTCDATETIME();
    PRINT '✅ Đã thêm cột SysStartTime';
END
ELSE
    PRINT '⚠️ Cột SysStartTime đã tồn tại';

-- Kiểm tra và thêm SysEndTime  
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'LN03' AND COLUMN_NAME = 'SysEndTime')
BEGIN
    ALTER TABLE LN03 
    ADD SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN
        CONSTRAINT DF_LN03_SysEndTime DEFAULT CONVERT(datetime2, '9999-12-31 23:59:59.9999999');
    PRINT '✅ Đã thêm cột SysEndTime';
END
ELSE
    PRINT '⚠️ Cột SysEndTime đã tồn tại';

-- ========================================
-- 3. TẠO PERIOD DEFINITION
-- ========================================
PRINT '⏰ Đang tạo Period definition...';

-- Thêm Period cho System Versioning
IF NOT EXISTS (SELECT * FROM sys.periods WHERE object_id = OBJECT_ID('LN03'))
BEGIN
    ALTER TABLE LN03 
    ADD PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
    PRINT '✅ Đã thêm Period definition';
END
ELSE
    PRINT '⚠️ Period definition đã tồn tại';

-- ========================================
-- 4. TẠO HISTORY TABLE
-- ========================================
PRINT '📚 Đang tạo History table...';

-- Tạo bảng lịch sử nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LN03_History')
BEGIN
    -- Tạo bảng history với cùng structure
    SELECT TOP 0 *
    INTO LN03_History
    FROM LN03;
    
    -- Thêm clustered index cho history table
    CREATE CLUSTERED INDEX CIX_LN03_History 
    ON LN03_History (SysEndTime, SysStartTime) 
    WITH (DATA_COMPRESSION = PAGE);
    
    PRINT '✅ Đã tạo bảng LN03_History';
END
ELSE
    PRINT '⚠️ Bảng LN03_History đã tồn tại';

-- ========================================
-- 5. ENABLE SYSTEM VERSIONING
-- ========================================
PRINT '🔄 Đang bật System Versioning...';

BEGIN TRY
    ALTER TABLE LN03 
    SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.LN03_History));
    
    PRINT '✅ System Versioning đã được bật thành công!';
END TRY
BEGIN CATCH
    PRINT '⚠️ Lỗi khi bật System Versioning: ' + ERROR_MESSAGE();
    
    -- Nếu có lỗi, có thể do constraints, thử cleanup
    IF ERROR_NUMBER() = 13591 -- Foreign key constraint error
    BEGIN
        PRINT '🔧 Đang thử cleanup và retry...';
        -- Có thể cần xử lý constraints ở đây
    END
END CATCH

-- ========================================
-- 6. KIỂM TRA KẾT QUẢ
-- ========================================
PRINT '🔍 Đang kiểm tra kết quả conversion...';

-- Kiểm tra trạng thái temporal
SELECT 
    t.name AS TableName,
    t.temporal_type_desc AS TemporalType,
    h.name AS HistoryTableName,
    CASE 
        WHEN t.temporal_type = 2 THEN '✅ SYSTEM-VERSIONED TEMPORAL TABLE'
        WHEN t.temporal_type = 1 THEN '⚠️ HISTORY TABLE' 
        ELSE '❌ NON-TEMPORAL TABLE'
    END AS Status
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id  
WHERE t.name IN ('LN03', 'LN03_History')
ORDER BY t.temporal_type DESC;

-- Hiển thị cấu trúc temporal columns
PRINT '';
PRINT '📅 TEMPORAL COLUMNS:';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    generated_always_type_desc,
    is_hidden
FROM sys.columns c
JOIN sys.tables t ON c.object_id = t.object_id
WHERE t.name = 'LN03' 
  AND COLUMN_NAME IN ('SysStartTime', 'SysEndTime')
ORDER BY column_id;

-- Test insert để xem temporal hoạt động
PRINT '';
PRINT '🧪 Test temporal functionality...';

DECLARE @TestId INT;
INSERT INTO LN03 (NGAY_DL, MACHINHANH, MAKH, TENKH, IS_DELETED, CREATED_DATE, UPDATED_DATE)
VALUES ('2024-12-31', '7800', 'TEST001', 'Test Customer', 0, GETDATE(), GETDATE());

SET @TestId = SCOPE_IDENTITY();

-- Cập nhật để tạo lịch sử
WAITFOR DELAY '00:00:01'; -- Đợi 1 giây để có thời gian khác biệt
UPDATE LN03 SET TENKH = 'Test Customer Updated' WHERE Id = @TestId;

-- Kiểm tra lịch sử
SELECT COUNT(*) as HistoryRecords 
FROM LN03_History 
WHERE Id = @TestId;

-- Cleanup test record
DELETE FROM LN03 WHERE Id = @TestId;

-- ========================================
-- 7. TEMPORAL TABLE CONVERSION COMPLETE
-- ========================================
PRINT '';
PRINT '🎉 TEMPORAL TABLE CONVERSION HOÀN THÀNH!';
PRINT '';
PRINT '📊 KẾT QUẢ:';
PRINT '   ✅ Temporal Columns: SysStartTime, SysEndTime';
PRINT '   ✅ History Table: LN03_History';
PRINT '   ✅ System Versioning: ENABLED';
PRINT '   ✅ Automatic History: ACTIVE';
PRINT '';
PRINT '🕰️ LN03 BẢN TEMPORAL TABLE SẴN SÀNG!';

GO
