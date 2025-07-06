-- ==================================================
-- SCRIPT CẤU HÌNH TEMPORAL TABLES + COLUMNSTORE INDEXES
-- CHO CÁC BẢNG DỮ LIỆU THÔ - TINHKHOAN APP
-- ==================================================

USE TinhKhoanDB;
GO

-- ==================================================
-- 1. BẢNG 7800_DT_KHKD1 (Excel Files)
-- ==================================================
PRINT 'Cấu hình bảng 7800_DT_KHKD1...';

-- Thêm system-time columns nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('7800_DT_KHKD1') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE [7800_DT_KHKD1]
    ADD SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN DEFAULT GETUTCDATE(),
        SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END

-- Bật system versioning
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '7800_DT_KHKD1' AND temporal_type = 2)
BEGIN
    ALTER TABLE [7800_DT_KHKD1] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.[7800_DT_KHKD1_History]));
END

-- Tạo columnstore index
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('7800_DT_KHKD1') AND type_desc = 'CLUSTERED COLUMNSTORE')
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_7800_DT_KHKD1 ON [7800_DT_KHKD1];
END

-- ==================================================
-- 2. BẢNG DB01 (CSV Files)
-- ==================================================
PRINT 'Cấu hình bảng DB01...';

-- Thêm system-time columns nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DB01') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE [DB01]
    ADD SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN DEFAULT GETUTCDATE(),
        SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END

-- Bật system versioning
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DB01' AND temporal_type = 2)
BEGIN
    ALTER TABLE [DB01] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.[DB01_History]));
END

-- Tạo columnstore index
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('DB01') AND type_desc = 'CLUSTERED COLUMNSTORE')
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_DB01 ON [DB01];
END

-- ==================================================
-- 3. BẢNG DP01_New (CSV Files)
-- ==================================================
PRINT 'Cấu hình bảng DP01_New...';

-- Thêm system-time columns nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DP01_New') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE [DP01_New]
    ADD SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN DEFAULT GETUTCDATE(),
        SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END

-- Bật system versioning
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DP01_New' AND temporal_type = 2)
BEGIN
    ALTER TABLE [DP01_New] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.[DP01_New_History]));
END

-- Tạo columnstore index
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('DP01_New') AND type_desc = 'CLUSTERED COLUMNSTORE')
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_DP01_New ON [DP01_New];
END

-- ==================================================
-- 4. BẢNG DPDA (CSV Files)
-- ==================================================
PRINT 'Cấu hình bảng DPDA...';

-- Thêm system-time columns nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DPDA') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE [DPDA]
    ADD SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN DEFAULT GETUTCDATE(),
        SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END

-- Bật system versioning
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DPDA' AND temporal_type = 2)
BEGIN
    ALTER TABLE [DPDA] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.[DPDA_History]));
END

-- Tạo columnstore index
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('DPDA') AND type_desc = 'CLUSTERED COLUMNSTORE')
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_DPDA ON [DPDA];
END

-- ==================================================
-- 5. BẢNG EI01 (CSV Files)
-- ==================================================
PRINT 'Cấu hình bảng EI01...';

-- Thêm system-time columns nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EI01') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE [EI01]
    ADD SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN DEFAULT GETUTCDATE(),
        SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END

-- Bật system versioning
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EI01' AND temporal_type = 2)
BEGIN
    ALTER TABLE [EI01] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.[EI01_History]));
END

-- Tạo columnstore index
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('EI01') AND type_desc = 'CLUSTERED COLUMNSTORE')
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_EI01 ON [EI01];
END

-- ==================================================
-- 6. BẢNG GL01 (CSV Files)
-- ==================================================
PRINT 'Cấu hình bảng GL01...';

-- Thêm system-time columns nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GL01') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE [GL01]
    ADD SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN DEFAULT GETUTCDATE(),
        SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END

-- Bật system versioning
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GL01' AND temporal_type = 2)
BEGIN
    ALTER TABLE [GL01] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.[GL01_History]));
END

-- Tạo columnstore index
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('GL01') AND type_desc = 'CLUSTERED COLUMNSTORE')
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_GL01 ON [GL01];
END

-- ==================================================
-- 7. BẢNG GL41 (CSV Files)
-- ==================================================
PRINT 'Cấu hình bảng GL41...';

-- Thêm system-time columns nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GL41') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE [GL41]
    ADD SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN DEFAULT GETUTCDATE(),
        SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END

-- Bật system versioning
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GL41' AND temporal_type = 2)
BEGIN
    ALTER TABLE [GL41] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.[GL41_History]));
END

-- Tạo columnstore index
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('GL41') AND type_desc = 'CLUSTERED COLUMNSTORE')
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_GL41 ON [GL41];
END

-- ==================================================
-- 8. BẢNG KH03 (CSV Files)
-- ==================================================
PRINT 'Cấu hình bảng KH03...';

-- Thêm system-time columns nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KH03') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE [KH03]
    ADD SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN DEFAULT GETUTCDATE(),
        SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END

-- Bật system versioning
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'KH03' AND temporal_type = 2)
BEGIN
    ALTER TABLE [KH03] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.[KH03_History]));
END

-- Tạo columnstore index
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('KH03') AND type_desc = 'CLUSTERED COLUMNSTORE')
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_KH03 ON [KH03];
END

-- ==================================================
-- 9. BẢNG LN01 (CSV Files)
-- ==================================================
PRINT 'Cấu hình bảng LN01...';

-- Thêm system-time columns nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('LN01') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE [LN01]
    ADD SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN DEFAULT GETUTCDATE(),
        SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END

-- Bật system versioning
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LN01' AND temporal_type = 2)
BEGIN
    ALTER TABLE [LN01] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.[LN01_History]));
END

-- Tạo columnstore index
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('LN01') AND type_desc = 'CLUSTERED COLUMNSTORE')
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_LN01 ON [LN01];
END

-- ==================================================
-- 10. BẢNG LN02 (CSV Files)
-- ==================================================
PRINT 'Cấu hình bảng LN02...';

-- Thêm system-time columns nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('LN02') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE [LN02]
    ADD SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN DEFAULT GETUTCDATE(),
        SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END

-- Bật system versioning
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LN02' AND temporal_type = 2)
BEGIN
    ALTER TABLE [LN02] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.[LN02_History]));
END

-- Tạo columnstore index
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('LN02') AND type_desc = 'CLUSTERED COLUMNSTORE')
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_LN02 ON [LN02];
END

-- ==================================================
-- 11. BẢNG LN03 (CSV Files)
-- ==================================================
PRINT 'Cấu hình bảng LN03...';

-- Thêm system-time columns nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('LN03') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE [LN03]
    ADD SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN DEFAULT GETUTCDATE(),
        SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END

-- Bật system versioning
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LN03' AND temporal_type = 2)
BEGIN
    ALTER TABLE [LN03] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.[LN03_History]));
END

-- Tạo columnstore index
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('LN03') AND type_desc = 'CLUSTERED COLUMNSTORE')
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_LN03 ON [LN03];
END

-- ==================================================
-- 12. BẢNG RR01 (CSV Files)
-- ==================================================
PRINT 'Cấu hình bảng RR01...';

-- Thêm system-time columns nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('RR01') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE [RR01]
    ADD SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN DEFAULT GETUTCDATE(),
        SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN DEFAULT '9999-12-31 23:59:59.9999999',
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
END

-- Bật system versioning
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RR01' AND temporal_type = 2)
BEGIN
    ALTER TABLE [RR01] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.[RR01_History]));
END

-- Tạo columnstore index
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('RR01') AND type_desc = 'CLUSTERED COLUMNSTORE')
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_RR01 ON [RR01];
END

-- ==================================================
-- KẾT THÚC CẤU HÌNH
-- ==================================================
PRINT '=== HOÀN THÀNH CẤU HÌNH TEMPORAL TABLES + COLUMNSTORE INDEXES ===';
PRINT 'Tất cả 12 bảng dữ liệu thô đã được cấu hình thành công!';

GO
