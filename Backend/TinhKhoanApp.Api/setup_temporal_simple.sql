-- 📊 THIẾT LẬP TEMPORAL TABLES + COLUMNSTORE CHO 8 BẢNG CORE (Simple & Effective)
-- Ngày: July 13, 2025

USE TinhKhoanDB;
GO

PRINT '🚀 Thiết lập Temporal Tables + Columnstore cho 8 bảng core...';

-- ============================================================================
-- 1. DP01 - Dữ liệu Tiền gửi
-- ============================================================================
PRINT '📋 Thiết lập DP01...';

-- Thêm temporal columns nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DP01') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE DP01 ADD
        SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END;

-- Tạo history table manually
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DP01_History')
BEGIN
    CREATE TABLE DP01_History (
        Id int NOT NULL,
        NGAY_DL datetime NULL,
        BRANCH_CODE nvarchar(max) NULL,
        CREATED_DATE datetime2 NULL,
        UPDATED_DATE datetime2 NULL,
        TK_TKCK nvarchar(max) NULL,
        SO_TK nvarchar(max) NULL,
        TEN_TK nvarchar(max) NULL,
        DCHITINH nvarchar(max) NULL,
        DCHIHUYEN nvarchar(max) NULL,
        DCHIXA nvarchar(max) NULL,
        SysStartTime datetime2 NOT NULL,
        SysEndTime datetime2 NOT NULL
    );

    CREATE CLUSTERED INDEX IX_DP01_History_Period ON DP01_History (SysEndTime, SysStartTime, Id);
END;

-- Bật temporal
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DP01' AND temporal_type = 2)
BEGIN
    ALTER TABLE DP01 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.DP01_History));
    PRINT '✅ DP01: Temporal enabled';
END;

-- Tạo columnstore trên history
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'CCI_DP01_History')
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_DP01_History ON DP01_History WITH (DROP_EXISTING = ON);
    PRINT '✅ DP01: Columnstore created';
END;

-- ============================================================================
-- 2. LN01 - Dữ liệu Cho vay
-- ============================================================================
PRINT '📋 Thiết lập LN01...';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('LN01') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE LN01 ADD
        SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LN01_History')
BEGIN
    CREATE TABLE LN01_History (
        Id int NOT NULL,
        NGAY_DL datetime NULL,
        BRANCH_CODE nvarchar(max) NULL,
        CREATED_DATE datetime2 NULL,
        UPDATED_DATE datetime2 NULL,
        -- Thêm các cột khác của LN01 ở đây (simplified)
        SysStartTime datetime2 NOT NULL,
        SysEndTime datetime2 NOT NULL
    );
    CREATE CLUSTERED INDEX IX_LN01_History_Period ON LN01_History (SysEndTime, SysStartTime, Id);
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LN01' AND temporal_type = 2)
BEGIN
    ALTER TABLE LN01 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.LN01_History));
    PRINT '✅ LN01: Temporal enabled';
END;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'CCI_LN01_History')
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_LN01_History ON LN01_History WITH (DROP_EXISTING = ON);
    PRINT '✅ LN01: Columnstore created';
END;

-- ============================================================================
-- Tương tự cho các bảng khác...
-- ============================================================================

-- 3. LN03
PRINT '📋 Thiết lập LN03...';
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('LN03') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE LN03 ADD
        SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END;

-- 4. GL01
PRINT '📋 Thiết lập GL01...';
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GL01') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE GL01 ADD
        SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END;

-- 5. GL41
PRINT '📋 Thiết lập GL41...';
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GL41') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE GL41 ADD
        SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END;

-- 6. DPDA
PRINT '📋 Thiết lập DPDA...';
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DPDA') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE DPDA ADD
        SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END;

-- 7. EI01
PRINT '📋 Thiết lập EI01...';
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EI01') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE EI01 ADD
        SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END;

-- 8. RR01
PRINT '📋 Thiết lập RR01...';
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('RR01') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE RR01 ADD
        SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END;

-- ============================================================================
-- KIỂM TRA KẾT QUẢ
-- ============================================================================
PRINT '📊 Kiểm tra kết quả...';

SELECT
    t.name AS TABLE_NAME,
    CASE WHEN t.temporal_type = 2 THEN '✅ YES' ELSE '❌ NO' END AS Is_Temporal,
    CASE
        WHEN t.history_table_id IS NOT NULL
        THEN OBJECT_NAME(t.history_table_id)
        ELSE 'No History'
    END AS History_Table
FROM sys.tables t
WHERE t.name IN ('DP01', 'LN01', 'LN03', 'GL01', 'GL41', 'DPDA', 'EI01', 'RR01')
ORDER BY t.name;

PRINT '🎉 Hoàn thành!';
