-- Performance Optimization Indexes for TinhKhoanApp
-- Created: 2025-06-17
-- Purpose: Optimize query performance for large datasets

-- 1. Composite indexes for RawDataRecords_SCD (most critical table)
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_Composite 
ON RawDataRecords_SCD (DataSource, BranchCode, StatementDate, IsCurrent)
WHERE IsDeleted = 0;

CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_SourceId_Hash 
ON RawDataRecords_SCD (SourceId, RecordHash) 
INCLUDE (IsCurrent, VersionNumber);

-- 2. Indexes for JSON data queries (if using JSON data)
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_JsonData_AccountNo 
ON RawDataRecords_SCD (json_extract(JsonData, '$.AccountNumber'));

CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_JsonData_Balance 
ON RawDataRecords_SCD (json_extract(JsonData, '$.Balance'));

-- 3. Covering indexes for Employees table
CREATE INDEX IF NOT EXISTS IX_Employees_Active_Covering 
ON Employees (UnitId, IsActive) 
WHERE IsActive = 1;

CREATE INDEX IF NOT EXISTS IX_Employees_Search_Covering 
ON Employees (EmployeeCode, FullName, CBCode)
WHERE IsActive = 1;

-- 4. Indexes for Units table
CREATE INDEX IF NOT EXISTS IX_Units_ParentUnit 
ON Units (ParentUnitId, IsActive);

CREATE INDEX IF NOT EXISTS IX_Units_Code_Name 
ON Units (Code, Name);

-- 5. Indexes for KPI related tables
CREATE INDEX IF NOT EXISTS IX_KpiIndicators_UnitPeriod 
ON KpiIndicators (UnitId, PeriodId, IsActive);

CREATE INDEX IF NOT EXISTS IX_UnitKpiScorings_Composite 
ON UnitKpiScorings (UnitId, KpiIndicatorId, Period, IsActive);

-- 6. Indexes for import/export operations
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_ImportBatch 
ON RawDataRecords_SCD (CreatedAt, DataSource, BranchCode);

CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_Export 
ON RawDataRecords_SCD (StatementDate, DataSource, IsCurrent)
WHERE IsDeleted = 0;

-- 7. Performance indexes for reporting
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_DateRange 
ON RawDataRecords_SCD (StatementDate)
WHERE IsCurrent = 1 AND IsDeleted = 0;

-- 8. Foreign key indexes (if not automatically created)
CREATE INDEX IF NOT EXISTS IX_Employees_UnitId 
ON Employees (UnitId);

CREATE INDEX IF NOT EXISTS IX_Employees_PositionId 
ON Employees (PositionId);

-- 9. Update statistics for query optimizer
ANALYZE RawDataRecords_SCD;
ANALYZE Employees;
ANALYZE Units;
ANALYZE KpiIndicators;
ANALYZE UnitKpiScorings;

-- 10. Query to check index usage
SELECT 
    name as IndexName,
    tbl_name as TableName,
    sql as IndexDefinition
FROM sqlite_master 
WHERE type = 'index' 
    AND name LIKE 'IX_%'
ORDER BY tbl_name, name;
