-- 📊 THIẾT LẬP TEMPORAL TABLES + COLUMNSTORE INDEXES CHO 8 BẢNG CORE
-- Ngày: July 13, 2025
-- Mục tiêu: Cấu hình đúng chuẩn theo yêu cầu anh

USE TinhKhoanDB;
GO

PRINT '🚀 Bắt đầu thiết lập Temporal Tables + Columnstore cho 8 bảng core...';

-- ============================================================================
-- 1. DP01 - DỮ LIỆU TIỀN GỬI (63 cột theo header CSV)
-- ============================================================================
PRINT '📋 Thiết lập DP01 - Dữ liệu Tiền gửi...';

-- Thêm các cột Temporal required
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DP01') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE DP01 ADD
        SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END;

-- Tạo History Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DP01_History')
BEGIN
    -- Tạo bảng history với cùng cấu trúc
    SELECT TOP 0 * INTO DP01_History FROM DP01;

    -- Thêm các cột temporal cho history table
    ALTER TABLE DP01_History ADD
        SysStartTime datetime2 NOT NULL,
        SysEndTime datetime2 NOT NULL;

    -- Tạo clustered index cho history table
    CREATE CLUSTERED INDEX IX_DP01_History_Period
    ON DP01_History (SysEndTime, SysStartTime, Id);
END;

-- Bật System Versioning
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DP01' AND temporal_type = 2)
BEGIN
    ALTER TABLE DP01 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.DP01_History));
    PRINT '✅ DP01: Temporal Table enabled';
END;

-- Tạo Columnstore Index
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'CCI_DP01' AND object_id = OBJECT_ID('DP01'))
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_DP01 ON DP01_History;
    PRINT '✅ DP01: Columnstore Index created';
END;

-- ============================================================================
-- 2. LN01 - DỮ LIỆU CHO VAY (79 cột theo header CSV)
-- ============================================================================
PRINT '📋 Thiết lập LN01 - Dữ liệu Cho vay...';

-- Thêm các cột Temporal required
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('LN01') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE LN01 ADD
        SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END;

-- Tạo History Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LN01_History')
BEGIN
    SELECT TOP 0 * INTO LN01_History FROM LN01;
    ALTER TABLE LN01_History ADD
        SysStartTime datetime2 NOT NULL,
        SysEndTime datetime2 NOT NULL;
    CREATE CLUSTERED INDEX IX_LN01_History_Period
    ON LN01_History (SysEndTime, SysStartTime, Id);
END;

-- Bật System Versioning
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LN01' AND temporal_type = 2)
BEGIN
    ALTER TABLE LN01 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.LN01_History));
    PRINT '✅ LN01: Temporal Table enabled';
END;

-- Tạo Columnstore Index
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'CCI_LN01' AND object_id = OBJECT_ID('LN01_History'))
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_LN01 ON LN01_History;
    PRINT '✅ LN01: Columnstore Index created';
END;

-- ============================================================================
-- 3. LN03 - DỮ LIỆU NỢ XLRR (17 cột)
-- ============================================================================
PRINT '📋 Thiết lập LN03 - Dữ liệu Nợ XLRR...';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('LN03') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE LN03 ADD
        SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LN03_History')
BEGIN
    SELECT TOP 0 * INTO LN03_History FROM LN03;
    ALTER TABLE LN03_History ADD
        SysStartTime datetime2 NOT NULL,
        SysEndTime datetime2 NOT NULL;
    CREATE CLUSTERED INDEX IX_LN03_History_Period
    ON LN03_History (SysEndTime, SysStartTime, Id);
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LN03' AND temporal_type = 2)
BEGIN
    ALTER TABLE LN03 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.LN03_History));
    PRINT '✅ LN03: Temporal Table enabled';
END;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'CCI_LN03' AND object_id = OBJECT_ID('LN03_History'))
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_LN03 ON LN03_History;
    PRINT '✅ LN03: Columnstore Index created';
END;

-- ============================================================================
-- 4. GL01 - DỮ LIỆU BÚT TOÁN GDV (27 cột)
-- ============================================================================
PRINT '📋 Thiết lập GL01 - Dữ liệu Bút toán GDV...';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GL01') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE GL01 ADD
        SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GL01_History')
BEGIN
    SELECT TOP 0 * INTO GL01_History FROM GL01;
    ALTER TABLE GL01_History ADD
        SysStartTime datetime2 NOT NULL,
        SysEndTime datetime2 NOT NULL;
    CREATE CLUSTERED INDEX IX_GL01_History_Period
    ON GL01_History (SysEndTime, SysStartTime, Id);
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GL01' AND temporal_type = 2)
BEGIN
    ALTER TABLE GL01 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.GL01_History));
    PRINT '✅ GL01: Temporal Table enabled';
END;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'CCI_GL01' AND object_id = OBJECT_ID('GL01_History'))
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_GL01 ON GL01_History;
    PRINT '✅ GL01: Columnstore Index created';
END;

-- ============================================================================
-- 5. GL41 - BẢNG CÂN ĐỐI KẾ TOÁN (13 cột)
-- ============================================================================
PRINT '📋 Thiết lập GL41 - Bảng cân đối kế toán...';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GL41') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE GL41 ADD
        SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GL41_History')
BEGIN
    SELECT TOP 0 * INTO GL41_History FROM GL41;
    ALTER TABLE GL41_History ADD
        SysStartTime datetime2 NOT NULL,
        SysEndTime datetime2 NOT NULL;
    CREATE CLUSTERED INDEX IX_GL41_History_Period
    ON GL41_History (SysEndTime, SysStartTime, Id);
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GL41' AND temporal_type = 2)
BEGIN
    ALTER TABLE GL41 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.GL41_History));
    PRINT '✅ GL41: Temporal Table enabled';
END;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'CCI_GL41' AND object_id = OBJECT_ID('GL41_History'))
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_GL41 ON GL41_History;
    PRINT '✅ GL41: Columnstore Index created';
END;

-- ============================================================================
-- 6. DPDA - DỮ LIỆU SAO KÊ PHÁT HÀNH THẺ (13 cột)
-- ============================================================================
PRINT '📋 Thiết lập DPDA - Dữ liệu sao kê phát hành thẻ...';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DPDA') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE DPDA ADD
        SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DPDA_History')
BEGIN
    SELECT TOP 0 * INTO DPDA_History FROM DPDA;
    ALTER TABLE DPDA_History ADD
        SysStartTime datetime2 NOT NULL,
        SysEndTime datetime2 NOT NULL;
    CREATE CLUSTERED INDEX IX_DPDA_History_Period
    ON DPDA_History (SysEndTime, SysStartTime, Id);
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DPDA' AND temporal_type = 2)
BEGIN
    ALTER TABLE DPDA SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.DPDA_History));
    PRINT '✅ DPDA: Temporal Table enabled';
END;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'CCI_DPDA' AND object_id = OBJECT_ID('DPDA_History'))
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_DPDA ON DPDA_History;
    PRINT '✅ DPDA: Columnstore Index created';
END;

-- ============================================================================
-- 7. EI01 - DỮ LIỆU MOBILE BANKING (24 cột)
-- ============================================================================
PRINT '📋 Thiết lập EI01 - Dữ liệu Mobile Banking...';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EI01') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE EI01 ADD
        SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EI01_History')
BEGIN
    SELECT TOP 0 * INTO EI01_History FROM EI01;
    ALTER TABLE EI01_History ADD
        SysStartTime datetime2 NOT NULL,
        SysEndTime datetime2 NOT NULL;
    CREATE CLUSTERED INDEX IX_EI01_History_Period
    ON EI01_History (SysEndTime, SysStartTime, Id);
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EI01' AND temporal_type = 2)
BEGIN
    ALTER TABLE EI01 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.EI01_History));
    PRINT '✅ EI01: Temporal Table enabled';
END;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'CCI_EI01' AND object_id = OBJECT_ID('EI01_History'))
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_EI01 ON EI01_History;
    PRINT '✅ EI01: Columnstore Index created';
END;

-- ============================================================================
-- 8. RR01 - SAO KÊ DƯ NỢ GỐC, LÃI XLRR (25 cột)
-- ============================================================================
PRINT '📋 Thiết lập RR01 - Sao kê dư nợ gốc, lãi XLRR...';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('RR01') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE RR01 ADD
        SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
        SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RR01_History')
BEGIN
    SELECT TOP 0 * INTO RR01_History FROM RR01;
    ALTER TABLE RR01_History ADD
        SysStartTime datetime2 NOT NULL,
        SysEndTime datetime2 NOT NULL;
    CREATE CLUSTERED INDEX IX_RR01_History_Period
    ON RR01_History (SysEndTime, SysStartTime, Id);
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RR01' AND temporal_type = 2)
BEGIN
    ALTER TABLE RR01 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.RR01_History));
    PRINT '✅ RR01: Temporal Table enabled';
END;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'CCI_RR01' AND object_id = OBJECT_ID('RR01_History'))
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_RR01 ON RR01_History;
    PRINT '✅ RR01: Columnstore Index created';
END;

-- ============================================================================
-- TỔNG KẾT KIỂM TRA
-- ============================================================================
PRINT '📊 Kiểm tra kết quả thiết lập...';

SELECT
    t.name AS TABLE_NAME,
    CASE WHEN t.temporal_type = 2 THEN '✅ YES' ELSE '❌ NO' END AS Is_Temporal,
    OBJECT_SCHEMA_NAME(t.history_table_id) + '.' + OBJECT_NAME(t.history_table_id) AS History_Table,
    (SELECT COUNT(*) FROM sys.indexes i WHERE i.object_id = t.history_table_id AND i.type_desc LIKE '%COLUMNSTORE%') AS Columnstore_Count
FROM sys.tables t
WHERE t.name IN ('DP01', 'LN01', 'LN03', 'GL01', 'GL41', 'DPDA', 'EI01', 'RR01')
ORDER BY t.name;

PRINT '🎉 Hoàn thành thiết lập Temporal Tables + Columnstore cho 8 bảng core!';
