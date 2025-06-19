-- SQLite-compatible Performance Optimization Indexes for TinhKhoanApp
-- Created: 2025-06-17
-- Purpose: Optimize query performance for large datasets on SQLite

-- 1. Composite indexes for RawDataRecords_SCD (most critical table)
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_Optimized_Composite 
ON RawDataRecords_SCD (DataSource, BranchCode, StatementDate, IsCurrent)
WHERE IsDeleted = 0;

CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_SourceId_Hash_Optimized 
ON RawDataRecords_SCD (SourceId, RecordHash, IsCurrent, VersionNumber);

-- 2. Indexes for JSON data queries
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_JsonData_AccountNo_Optimized 
ON RawDataRecords_SCD (json_extract(JsonData, '$.AccountNumber'));

CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_JsonData_Balance_Optimized 
ON RawDataRecords_SCD (json_extract(JsonData, '$.Balance'));

-- 3. Covering indexes for Employees table
CREATE INDEX IF NOT EXISTS IX_Employees_Active_Optimized 
ON Employees (UnitId, IsActive, EmployeeCode, FullName)
WHERE IsActive = 1;

CREATE INDEX IF NOT EXISTS IX_Employees_Search_Optimized 
ON Employees (EmployeeCode, FullName, CBCode, IsActive);

-- 4. Indexes for Units table
CREATE INDEX IF NOT EXISTS IX_Units_ParentUnit_Optimized 
ON Units (ParentUnitId, UnitCode, UnitName);

CREATE INDEX IF NOT EXISTS IX_Units_Code_Name_Optimized 
ON Units (UnitCode, UnitName);

-- 5. Indexes for import/export operations
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_ImportBatch_Optimized 
ON RawDataRecords_SCD (CreatedAt, DataSource, BranchCode);

CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_Export_Optimized 
ON RawDataRecords_SCD (StatementDate, DataSource, IsCurrent)
WHERE IsDeleted = 0;

-- 6. Performance indexes for reporting
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_DateRange_Optimized 
ON RawDataRecords_SCD (StatementDate, DataSource, IsCurrent)
WHERE IsCurrent = 1 AND IsDeleted = 0;

-- 7. Foreign key indexes (create only if not exists)
CREATE INDEX IF NOT EXISTS IX_Employees_UnitId_Optimized 
ON Employees (UnitId, IsActive);

CREATE INDEX IF NOT EXISTS IX_Employees_PositionId_Optimized 
ON Employees (PositionId, IsActive);

-- 8. Additional performance indexes for common queries
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_Current_Records
ON RawDataRecords_SCD (IsCurrent, IsDeleted, DataSource, StatementDate);

CREATE INDEX IF NOT EXISTS IX_Employees_Search_FullText
ON Employees (EmployeeCode, FullName, CBCode, UnitId, IsActive);

-- 9. Update statistics for query optimizer
ANALYZE RawDataRecords_SCD;
ANALYZE Employees;
ANALYZE Units;

-- 10. Query to check newly created indexes
SELECT 
    name as IndexName,
    tbl_name as TableName,
    sql as IndexDefinition
FROM sqlite_master 
WHERE type = 'index' 
    AND name LIKE '%_Optimized%'
ORDER BY tbl_name, name;
