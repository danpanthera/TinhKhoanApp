-- =====================================================
-- ADVANCED PERFORMANCE OPTIMIZATION INDEXES
-- Tối ưu hóa hiệu suất truy xuất dữ liệu cho hệ thống TinhKhoanApp
-- Updated: 2025-06-17 - Enhanced with SCD Type 2 optimization
-- =====================================================

-- 1. ADVANCED SCD TYPE 2 INDEXES
-- Tối ưu cho SCD Type 2 với deleted records handling
DROP INDEX IF EXISTS IX_RawDataRecords_SCD_Composite;
CREATE INDEX IX_RawDataRecords_SCD_Composite 
ON RawDataRecords_SCD (DataSource, BranchCode, StatementDate, IsCurrent)
WHERE IsDeleted = 0;

DROP INDEX IF EXISTS IX_RawDataRecords_SCD_SourceId_Hash;
CREATE INDEX IX_RawDataRecords_SCD_SourceId_Hash 
ON RawDataRecords_SCD (SourceId, RecordHash, IsCurrent, VersionNumber);

DROP INDEX IF EXISTS IX_RawDataRecords_SCD_Current_Active;
CREATE INDEX IX_RawDataRecords_SCD_Current_Active 
ON RawDataRecords_SCD (IsCurrent, IsDeleted, DataType, BranchCode);

DROP INDEX IF EXISTS IX_RawDataRecords_SCD_ValidFromTo;
CREATE INDEX IX_RawDataRecords_SCD_ValidFromTo 
ON RawDataRecords_SCD (ValidFrom, ValidTo, IsCurrent);

-- 2. EMPLOYEES TABLE OPTIMIZATION
DROP INDEX IF EXISTS IX_Employees_Active_Unit;
CREATE INDEX IX_Employees_Active_Unit 
ON Employees (UnitId, IsActive, EmployeeCode);

DROP INDEX IF EXISTS IX_Employees_Search_Optimized;
CREATE INDEX IX_Employees_Search_Optimized 
ON Employees (EmployeeCode, FullName, CBCode, IsActive);

-- 3. UNITS TABLE OPTIMIZATION  
DROP INDEX IF EXISTS IX_Units_Hierarchy;
CREATE INDEX IX_Units_Hierarchy 
ON Units (ParentUnitId, UnitCode, UnitType);

-- 4. RAW DATA IMPORT OPTIMIZATION
CREATE INDEX IF NOT EXISTS IX_RawDataImports_ImportDate ON RawDataImports (ImportDate DESC);
CREATE INDEX IF NOT EXISTS IX_RawDataImports_DataType ON RawDataImports (DataType);
CREATE INDEX IF NOT EXISTS IX_RawDataImports_StatementDate ON RawDataImports (StatementDate DESC);
CREATE INDEX IF NOT EXISTS IX_RawDataImports_Status ON RawDataImports (Status);
CREATE INDEX IF NOT EXISTS IX_RawDataImports_DataType_ImportDate ON RawDataImports (DataType, ImportDate DESC);
CREATE INDEX IF NOT EXISTS IX_RawDataImports_ImportedBy ON RawDataImports (ImportedBy);

-- 2. INDEXES CHO BẢNG RawDataRecords  
-- Tăng tốc join với RawDataImports và tìm kiếm theo JSON
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_ImportId ON RawDataRecords (RawDataImportId);
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_ProcessedDate ON RawDataRecords (ProcessedDate DESC);
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_ImportId_ProcessedDate ON RawDataRecords (RawDataImportId, ProcessedDate DESC);

-- 3. INDEXES CHO CÁC BẢNG SCD HISTORY
-- Tối ưu hóa truy vấn lịch sử và phiên bản

-- LN01_History Performance Indexes
CREATE INDEX IF NOT EXISTS IX_LN01_History_Performance_1 ON LN01_History (IsCurrent, ValidFrom DESC) WHERE IsCurrent = 1;
CREATE INDEX IF NOT EXISTS IX_LN01_History_Performance_2 ON LN01_History (SourceID, VersionNumber DESC);
CREATE INDEX IF NOT EXISTS IX_LN01_History_Performance_3 ON LN01_History (ValidFrom, ValidTo, IsCurrent);
CREATE INDEX IF NOT EXISTS IX_LN01_History_CreatedDate ON LN01_History (CreatedDate DESC);
CREATE INDEX IF NOT EXISTS IX_LN01_History_ModifiedDate ON LN01_History (ModifiedDate DESC);

-- Indexes cho các trường business quan trọng của LN01
CREATE INDEX IF NOT EXISTS IX_LN01_History_BUKRS ON LN01_History (BUKRS) WHERE IsCurrent = 1;
CREATE INDEX IF NOT EXISTS IX_LN01_History_MANDT ON LN01_History (MANDT) WHERE IsCurrent = 1;

-- GL01_History Performance Indexes  
CREATE INDEX IF NOT EXISTS IX_GL01_History_Performance_1 ON GL01_History (IsCurrent, ValidFrom DESC) WHERE IsCurrent = 1;
CREATE INDEX IF NOT EXISTS IX_GL01_History_Performance_2 ON GL01_History (SourceID, VersionNumber DESC);
CREATE INDEX IF NOT EXISTS IX_GL01_History_Performance_3 ON GL01_History (ValidFrom, ValidTo, IsCurrent);
CREATE INDEX IF NOT EXISTS IX_GL01_History_CreatedDate ON GL01_History (CreatedDate DESC);
CREATE INDEX IF NOT EXISTS IX_GL01_History_ModifiedDate ON GL01_History (ModifiedDate DESC);

-- Indexes cho các trường business quan trọng của GL01
CREATE INDEX IF NOT EXISTS IX_GL01_History_BUKRS ON GL01_History (BUKRS) WHERE IsCurrent = 1;
CREATE INDEX IF NOT EXISTS IX_GL01_History_BELNR ON GL01_History (BELNR) WHERE IsCurrent = 1;
CREATE INDEX IF NOT EXISTS IX_GL01_History_GJAHR ON GL01_History (GJAHR) WHERE IsCurrent = 1;
CREATE INDEX IF NOT EXISTS IX_GL01_History_HKONT ON GL01_History (HKONT) WHERE IsCurrent = 1;
CREATE INDEX IF NOT EXISTS IX_GL01_History_DMBTR ON GL01_History (DMBTR DESC) WHERE IsCurrent = 1;

-- DP01_History Performance Indexes
CREATE INDEX IF NOT EXISTS IX_DP01_History_Performance_1 ON DP01_History (IsCurrent, ValidFrom DESC) WHERE IsCurrent = 1;
CREATE INDEX IF NOT EXISTS IX_DP01_History_Performance_2 ON DP01_History (SourceID, VersionNumber DESC);
CREATE INDEX IF NOT EXISTS IX_DP01_History_Performance_3 ON DP01_History (ValidFrom, ValidTo, IsCurrent);
CREATE INDEX IF NOT EXISTS IX_DP01_History_CreatedDate ON DP01_History (CreatedDate DESC);
CREATE INDEX IF NOT EXISTS IX_DP01_History_ModifiedDate ON DP01_History (ModifiedDate DESC);

-- 4. INDEXES CHO BẢNG SCD GENERIC (nếu có)
-- Indexes cho RawDataRecords_SCD (bảng SCD chung)
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_Performance_1 ON RawDataRecords_SCD (IsCurrent, ValidFrom DESC) WHERE IsCurrent = 1;
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_Performance_2 ON RawDataRecords_SCD (SourceId, VersionNumber DESC);
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_Performance_3 ON RawDataRecords_SCD (ValidFrom, ValidTo, IsCurrent);
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_DataType ON RawDataRecords_SCD (DataType) WHERE IsCurrent = 1;
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_BranchCode ON RawDataRecords_SCD (BranchCode) WHERE IsCurrent = 1;
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_StatementDate ON RawDataRecords_SCD (StatementDate DESC) WHERE IsCurrent = 1;
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_CreatedAt ON RawDataRecords_SCD (CreatedAt DESC);
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_ImportDate ON RawDataRecords_SCD (ImportDate DESC);

-- 5. INDEXES CHO BẢNG ImportLog
CREATE INDEX IF NOT EXISTS IX_ImportLog_Performance_1 ON ImportLog (TableName, ImportDate DESC);
CREATE INDEX IF NOT EXISTS IX_ImportLog_Performance_2 ON ImportLog (Status, ImportDate DESC);
CREATE INDEX IF NOT EXISTS IX_ImportLog_Performance_3 ON ImportLog (BatchId, ImportDate DESC);
CREATE INDEX IF NOT EXISTS IX_ImportLog_StartTime ON ImportLog (StartTime DESC);
CREATE INDEX IF NOT EXISTS IX_ImportLog_EndTime ON ImportLog (EndTime DESC);
CREATE INDEX IF NOT EXISTS IX_ImportLog_Duration ON ImportLog (Duration DESC);

-- 6. COMPOSITE INDEXES ĐỂ TỐI ƯU TRUY VẤN PHỨC TẠP
-- Composite index cho truy vấn thường dùng
CREATE INDEX IF NOT EXISTS IX_RawDataImports_Composite_1 ON RawDataImports (DataType, Status, ImportDate DESC);
CREATE INDEX IF NOT EXISTS IX_RawDataImports_Composite_2 ON RawDataImports (ImportedBy, DataType, ImportDate DESC);
CREATE INDEX IF NOT EXISTS IX_RawDataImports_Composite_3 ON RawDataImports (StatementDate DESC, DataType, Status);

-- Composite indexes cho SCD queries
CREATE INDEX IF NOT EXISTS IX_LN01_History_Composite_1 ON LN01_History (BUKRS, IsCurrent, ValidFrom DESC);
CREATE INDEX IF NOT EXISTS IX_GL01_History_Composite_1 ON GL01_History (BUKRS, GJAHR, IsCurrent);
CREATE INDEX IF NOT EXISTS IX_GL01_History_Composite_2 ON GL01_History (HKONT, BUKRS, IsCurrent);

-- 7. COMPOSITE INDEXES ĐỂ TỐI ƯU TRUY VẤN PHỨC TẠP
-- Composite index cho truy vấn thường dùng
CREATE INDEX IF NOT EXISTS IX_RawDataImports_Composite_1 ON RawDataImports (DataType, Status, ImportDate DESC);
CREATE INDEX IF NOT EXISTS IX_RawDataImports_Composite_2 ON RawDataImports (ImportedBy, DataType, ImportDate DESC);
CREATE INDEX IF NOT EXISTS IX_RawDataImports_Composite_3 ON RawDataImports (StatementDate DESC, DataType, Status);

-- Composite indexes cho SCD queries
CREATE INDEX IF NOT EXISTS IX_LN01_History_Composite_1 ON LN01_History (BUKRS, IsCurrent, ValidFrom DESC);
CREATE INDEX IF NOT EXISTS IX_GL01_History_Composite_1 ON GL01_History (BUKRS, GJAHR, IsCurrent);
CREATE INDEX IF NOT EXISTS IX_GL01_History_Composite_2 ON GL01_History (HKONT, BUKRS, IsCurrent);
-- 8. ANALYZE DATABASE STATISTICS
-- Cập nhật thống kê để query optimizer hoạt động tối ưu
ANALYZE;

-- 9. VACUUM DATABASE (chạy định kỳ để tối ưu storage)
-- VACUUM; -- Uncomment để chạy vacuum

-- 10. PRAGMA SETTINGS ĐỂ TỐI ƯU HIỆU SUẤT
PRAGMA journal_mode = WAL;
PRAGMA synchronous = NORMAL;
PRAGMA cache_size = 10000;
PRAGMA temp_store = MEMORY;
PRAGMA mmap_size = 268435456; -- 256MB

-- =====================================================
-- KẾT THÚC SCRIPT TỐI ƯU HÓA INDEX
-- =====================================================

-- Ghi chú:
-- 1. Chạy ANALYZE sau khi tạo indexes để cập nhật statistics
-- 2. Chạy VACUUM định kỳ để tối ưu database size
-- 3. Monitor query performance sau khi apply indexes
-- 4. Có thể cần adjust cache_size và mmap_size tùy theo RAM available
