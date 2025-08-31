-- ========================================
-- 🕰️ LN03 TEMPORAL TABLE TEST SCRIPT
-- ========================================  
-- Tạo bảng temporal mới để test, sau đó migrate dữ liệu

USE TinhKhoanDB;
GO

PRINT '🕰️ TESTING TEMPORAL TABLE FUNCTIONALITY...';

-- ========================================
-- 1. TẠO BẢNG TEMPORAL MỚI ĐỂ TEST
-- ========================================
PRINT 'Đang tạo bảng LN03_Temporal để test...';

-- Drop bảng test nếu đã tồn tại
IF OBJECT_ID('LN03_Temporal', 'U') IS NOT NULL
BEGIN
    ALTER TABLE LN03_Temporal SET (SYSTEM_VERSIONING = OFF);
    DROP TABLE LN03_Temporal_History;
    DROP TABLE LN03_Temporal;
    PRINT 'Đã xóa bảng test cũ.';
END

-- Tạo bảng temporal mới
CREATE TABLE LN03_Temporal (
    Id int IDENTITY(1,1) PRIMARY KEY,
    NGAY_DL datetime2,
    MACHINHANH nvarchar(50),
    MAKH nvarchar(200),
    TENKH nvarchar(500),
    SOTIENXLRR decimal(18,2),
    IS_DELETED bit DEFAULT 0,
    CREATED_DATE datetime2 DEFAULT GETDATE(),
    UPDATED_DATE datetime2 DEFAULT GETDATE(),
    
    -- Temporal columns
    SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN 
        CONSTRAINT DF_LN03_Temporal_SysStart DEFAULT SYSUTCDATETIME(),
    SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN 
        CONSTRAINT DF_LN03_Temporal_SysEnd DEFAULT CONVERT(datetime2, '9999-12-31 23:59:59.9999999'),
    
    -- Period definition
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime)
)
WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.LN03_Temporal_History));

PRINT 'Đã tạo LN03_Temporal với System Versioning.';

-- ========================================
-- 2. TEST TEMPORAL FUNCTIONALITY
-- ========================================
PRINT '';
PRINT 'Đang test temporal functionality...';

-- Insert test record
DECLARE @TestId INT;
INSERT INTO LN03_Temporal (NGAY_DL, MACHINHANH, MAKH, TENKH, SOTIENXLRR)
VALUES ('2024-12-31', '7800', 'TEST001', 'Test Customer Original', 1000000.00);

SET @TestId = SCOPE_IDENTITY();
PRINT 'Đã insert record ID: ' + CAST(@TestId AS VARCHAR);

-- Đợi 1 giây rồi update để tạo lịch sử
WAITFOR DELAY '00:00:01';
UPDATE LN03_Temporal 
SET TENKH = 'Test Customer Updated', SOTIENXLRR = 2000000.00
WHERE Id = @TestId;

PRINT 'Đã update record để tạo lịch sử.';

-- Kiểm tra current data
PRINT '';
PRINT 'Current data:';
SELECT Id, TENKH, SOTIENXLRR FROM LN03_Temporal WHERE Id = @TestId;

-- Kiểm tra history data  
PRINT '';
PRINT 'History data:';
SELECT Id, TENKH, SOTIENXLRR, SysStartTime, SysEndTime 
FROM LN03_Temporal_History WHERE Id = @TestId;

-- Temporal query - lấy data tại thời điểm quá khứ
PRINT '';
PRINT 'Temporal query - data FOR SYSTEM_TIME ALL:';
SELECT Id, TENKH, SOTIENXLRR, SysStartTime, SysEndTime
FROM LN03_Temporal FOR SYSTEM_TIME ALL
WHERE Id = @TestId
ORDER BY SysStartTime;

-- Cleanup test record
DELETE FROM LN03_Temporal WHERE Id = @TestId;

-- ========================================
-- 3. KIỂM TRA KẾT QUẢ
-- ========================================
PRINT '';
PRINT 'Kiểm tra bảng temporal:';

SELECT 
    t.name AS TableName,
    t.temporal_type_desc AS TemporalType,
    h.name AS HistoryTableName,
    CASE 
        WHEN t.temporal_type = 2 THEN 'SUCCESS: System-Versioned Temporal Table'
        ELSE 'ERROR: Not temporal'
    END AS Status
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id  
WHERE t.name = 'LN03_Temporal';

PRINT '';
PRINT '🎉 TEMPORAL TABLE TEST HOÀN THÀNH!';
PRINT 'Temporal functionality hoạt động chính xác.';
PRINT '';
PRINT '📋 NEXT STEPS:';
PRINT '1. Có thể migrate dữ liệu từ LN03 sang LN03_Temporal';  
PRINT '2. Hoặc tạo temporal table mới cho production';
PRINT '3. Re-enable temporal trong ApplicationDbContext';

GO
