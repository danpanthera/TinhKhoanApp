-- =====================================================
-- PERFORMANCE OPTIMIZATION INDEXES
-- SQL Server version - comprehensive index strategy
-- =====================================================

-- 1. MAIN SCD INDEXES (RawDataRecords_SCD)
-- Core operational indexes for SCD Type 2 operations

-- Primary composite index for SCD queries
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RawDataRecords_SCD_Composite')
CREATE INDEX IX_RawDataRecords_SCD_Composite 
ON RawDataRecords_SCD (SourceID, IsCurrent, ValidFrom DESC, ValidTo ASC) 
INCLUDE (JsonData, CreatedDate, ModifiedDate);

-- Source ID + RecordHash for duplicate detection
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RawDataRecords_SCD_SourceId_Hash')
CREATE INDEX IX_RawDataRecords_SCD_SourceId_Hash 
ON RawDataRecords_SCD (SourceID, RecordHash) 
WHERE IsCurrent = 1;

-- 2. JSON FIELD INDEXES (If SQL Server supports computed columns for JSON)
-- Account number extraction index
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RawDataRecords_SCD_JsonData_AccountNo')
CREATE INDEX IX_RawDataRecords_SCD_JsonData_AccountNo 
ON RawDataRecords_SCD (JSON_VALUE(JsonData, '$.AccountNo'));

-- Balance extraction index for financial queries
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RawDataRecords_SCD_JsonData_Balance')
CREATE INDEX IX_RawDataRecords_SCD_JsonData_Balance 
ON RawDataRecords_SCD (JSON_VALUE(JsonData, '$.Balance'));

-- 3. RAWDATAIMPORTS TABLE INDEXES
-- Import date performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RawDataImports_ImportDate')
CREATE INDEX IX_RawDataImports_ImportDate ON RawDataImports (ImportDate DESC);

-- Data type filtering
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RawDataImports_DataType')
CREATE INDEX IX_RawDataImports_DataType ON RawDataImports (DataType);

-- Statement date for reporting
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RawDataImports_StatementDate')
CREATE INDEX IX_RawDataImports_StatementDate ON RawDataImports (StatementDate DESC);

-- Status filtering
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RawDataImports_Status')
CREATE INDEX IX_RawDataImports_Status ON RawDataImports (Status);

-- Composite index for common queries
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RawDataImports_DataType_ImportDate')
CREATE INDEX IX_RawDataImports_DataType_ImportDate ON RawDataImports (DataType, ImportDate DESC);

-- User tracking
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RawDataImports_ImportedBy')
CREATE INDEX IX_RawDataImports_ImportedBy ON RawDataImports (ImportedBy);

-- 4. RAWDATARECORDS TABLE INDEXES  
-- Import relationship
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RawDataRecords_ImportId')
CREATE INDEX IX_RawDataRecords_ImportId ON RawDataRecords (RawDataImportId);

-- Processing date for monitoring
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RawDataRecords_ProcessedDate')
CREATE INDEX IX_RawDataRecords_ProcessedDate ON RawDataRecords (ProcessedDate DESC);

-- Composite for import analysis
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RawDataRecords_ImportId_ProcessedDate')
CREATE INDEX IX_RawDataRecords_ImportId_ProcessedDate ON RawDataRecords (RawDataImportId, ProcessedDate DESC);

-- 5. HISTORY TABLE PERFORMANCE INDEXES

-- LN01_History optimizations
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LN01_History_Performance_1')
CREATE INDEX IX_LN01_History_Performance_1 ON LN01_History (IsCurrent, ValidFrom DESC) WHERE IsCurrent = 1;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LN01_History_Performance_2')
CREATE INDEX IX_LN01_History_Performance_2 ON LN01_History (SourceID, VersionNumber DESC);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LN01_History_Performance_3')
CREATE INDEX IX_LN01_History_Performance_3 ON LN01_History (ValidFrom, ValidTo, IsCurrent);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LN01_History_CreatedDate')
CREATE INDEX IX_LN01_History_CreatedDate ON LN01_History (CreatedDate DESC);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LN01_History_ModifiedDate')
CREATE INDEX IX_LN01_History_ModifiedDate ON LN01_History (ModifiedDate DESC);

-- Business key indexes for LN01
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LN01_History_BUKRS')
CREATE INDEX IX_LN01_History_BUKRS ON LN01_History (BUKRS) WHERE IsCurrent = 1;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LN01_History_MANDT')
CREATE INDEX IX_LN01_History_MANDT ON LN01_History (MANDT) WHERE IsCurrent = 1;

-- GL01_History optimizations
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_GL01_History_Performance_1')
CREATE INDEX IX_GL01_History_Performance_1 ON GL01_History (IsCurrent, ValidFrom DESC) WHERE IsCurrent = 1;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_GL01_History_Performance_2')
CREATE INDEX IX_GL01_History_Performance_2 ON GL01_History (SourceID, VersionNumber DESC);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_GL01_History_Performance_3')
CREATE INDEX IX_GL01_History_Performance_3 ON GL01_History (ValidFrom, ValidTo, IsCurrent);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_GL01_History_CreatedDate')
CREATE INDEX IX_GL01_History_CreatedDate ON GL01_History (CreatedDate DESC);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_GL01_History_ModifiedDate')
CREATE INDEX IX_GL01_History_ModifiedDate ON GL01_History (ModifiedDate DESC);

-- Business key indexes for GL01
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_GL01_History_BUKRS_GJAHR')
CREATE INDEX IX_GL01_History_BUKRS_GJAHR ON GL01_History (BUKRS, GJAHR) WHERE IsCurrent = 1;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_GL01_History_BELNR_BUZEI')
CREATE INDEX IX_GL01_History_BELNR_BUZEI ON GL01_History (BELNR, BUZEI) WHERE IsCurrent = 1;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_GL01_History_HKONT')
CREATE INDEX IX_GL01_History_HKONT ON GL01_History (HKONT) WHERE IsCurrent = 1;

-- DP01_History optimizations
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_DP01_History_Performance_1')
CREATE INDEX IX_DP01_History_Performance_1 ON DP01_History (IsCurrent, ValidFrom DESC) WHERE IsCurrent = 1;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_DP01_History_Performance_2')
CREATE INDEX IX_DP01_History_Performance_2 ON DP01_History (SourceID, VersionNumber DESC);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_DP01_History_KUNNR')
CREATE INDEX IX_DP01_History_KUNNR ON DP01_History (KUNNR) WHERE IsCurrent = 1;

-- 6. IMPORT LOG AND MONITORING INDEXES
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ImportLog_ImportDate')
CREATE INDEX IX_ImportLog_ImportDate ON ImportLog (ImportDate DESC);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ImportLog_TableName_Status')
CREATE INDEX IX_ImportLog_TableName_Status ON ImportLog (TableName, Status);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ImportLog_BatchId')
CREATE INDEX IX_ImportLog_BatchId ON ImportLog (BatchId);

-- 7. COMPOSITE INDEXES FOR COMPLEX QUERIES
-- SCD composite indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LN01_History_Composite_1')
CREATE INDEX IX_LN01_History_Composite_1 ON LN01_History (BUKRS, IsCurrent, ValidFrom DESC);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_GL01_History_Composite_1')
CREATE INDEX IX_GL01_History_Composite_1 ON GL01_History (BUKRS, GJAHR, IsCurrent);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_GL01_History_Composite_2')
CREATE INDEX IX_GL01_History_Composite_2 ON GL01_History (HKONT, BUKRS, IsCurrent);

-- 8. UPDATE DATABASE STATISTICS (SQL Server)
-- Cập nhật thống kê để query optimizer hoạt động tối ưu
UPDATE STATISTICS LN01_History;
UPDATE STATISTICS GL01_History;
UPDATE STATISTICS DP01_History;
UPDATE STATISTICS RawDataRecords_SCD;
UPDATE STATISTICS ImportLog;

-- 9. MAINTENANCE TASKS (SQL Server)
-- Có thể chạy định kỳ để tối ưu performance
-- ALTER INDEX ALL ON LN01_History REORGANIZE;
-- ALTER INDEX ALL ON GL01_History REORGANIZE;

-- 10. SQL SERVER SETTINGS ĐỂ TỐI ƯU HIỆU SUẤT
-- Các setting này cần được cấu hình ở mức database hoặc server
-- Ví dụ các thiết lập có thể cân nhắc:
-- - Recovery Model: SIMPLE hoặc FULL tùy yêu cầu backup
-- - Auto Update Statistics: ON
-- - Auto Create Statistics: ON
-- - Page Verify: CHECKSUM
-- - Query Store: ON (SQL Server 2016+)

-- =====================================================
-- KẾT THÚC SCRIPT TỐI ƯU HÓA INDEX
-- =====================================================

-- Ghi chú (SQL Server):
-- 1. Chạy UPDATE STATISTICS sau khi tạo indexes để cập nhật statistics
-- 2. Chạy ALTER INDEX...REORGANIZE hoặc REBUILD định kỳ để tối ưu index fragmentation
-- 3. Monitor query performance và index usage với sys.dm_db_index_usage_stats
-- 4. Sử dụng SQL Server Profiler hoặc Extended Events để monitor performance
-- 5. Cân nhắc index compression cho các bảng lớn (Enterprise Edition)
-- 6. Sử dụng filtered indexes khi có điều kiện WHERE để giảm kích thước index
