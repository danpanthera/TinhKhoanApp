-- Performance Optimization Indexes for Tinh-- 5. Indexes for KPI related tables
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_KpiIndicators_UnitPeriod')
CREATE INDEX IX_KpiIndicators_UnitPeriod 
ON KpiIndicators (UnitId, PeriodId, IsActive);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UnitKpiScorings_Composite')
CREATE INDEX IX_UnitKpiScorings_Composite 
ON UnitKpiScorings (UnitId, ScoringPeriod, IsActive);pp (SQL Server)
-- Created: 2025-06-17
-- Purpose: Optimize query performance for large datasets

-- 1. Composite indexes for RawDataRecords_SCD (most critical table)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RawDataRecords_SCD_Composite')
CREATE NONCLUSTERED INDEX IX_RawDataRecords_SCD_Composite 
ON RawDataRecords_SCD (DataSource, BranchCode, StatementDate, IsCurrent)
WHERE IsDeleted = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RawDataRecords_SCD_SourceId_Hash')
CREATE NONCLUSTERED INDEX IX_RawDataRecords_SCD_SourceId_Hash 
ON RawDataRecords_SCD (SourceId, RecordHash) 
INCLUDE (IsCurrent, VersionNumber);

-- 2. Indexes for JSON data queries (SQL Server JSON functions)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RawDataRecords_SCD_JsonData_AccountNo')
CREATE NONCLUSTERED INDEX IX_RawDataRecords_SCD_JsonData_AccountNo 
ON RawDataRecords_SCD (CAST(JSON_VALUE(JsonData, '$.AccountNumber') AS NVARCHAR(50)));

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RawDataRecords_SCD_JsonData_Balance')
CREATE NONCLUSTERED INDEX IX_RawDataRecords_SCD_JsonData_Balance 
ON RawDataRecords_SCD (CAST(JSON_VALUE(JsonData, '$.Balance') AS DECIMAL(18,2)));

-- 3. Covering indexes for Employees table
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Employees_Active_Covering')
CREATE INDEX IX_Employees_Active_Covering 
ON Employees (UnitId, IsActive) 
WHERE IsActive = 1;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Employees_Search_Covering')
CREATE INDEX IX_Employees_Search_Covering 
ON Employees (EmployeeCode, FullName, CBCode)
WHERE IsActive = 1;

-- 4. Indexes for Units table
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Units_ParentUnit')
CREATE INDEX IX_Units_ParentUnit 
ON Units (ParentUnitId, IsActive);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Units_Code_Name')
CREATE INDEX IX_Units_Code_Name 
ON Units (Code, Name);

-- 5. Indexes for KPI related tables
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_KpiIndicators_UnitPeriod')
CREATE INDEX IX_KpiIndicators_UnitPeriod 
ON KpiIndicators (UnitId, PeriodId, IsActive);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UnitKpiScorings_Composite')
CREATE INDEX IX_UnitKpiScorings_Composite 
ON UnitKpiScorings (UnitId, KpiIndicatorId, Period, IsActive);

-- 6. Indexes for import/export operations
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RawDataRecords_SCD_ImportBatch')
CREATE INDEX IX_RawDataRecords_SCD_ImportBatch 
ON RawDataRecords_SCD (CreatedAt, DataSource, BranchCode);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RawDataRecords_SCD_Export')
CREATE INDEX IX_RawDataRecords_SCD_Export 
ON RawDataRecords_SCD (StatementDate, DataSource, IsCurrent)
WHERE IsDeleted = 0;

-- 7. Performance indexes for reporting
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RawDataRecords_SCD_DateRange')
CREATE INDEX IX_RawDataRecords_SCD_DateRange 
ON RawDataRecords_SCD (StatementDate)
WHERE IsCurrent = 1 AND IsDeleted = 0;

-- 8. Foreign key indexes (if not automatically created)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Employees_UnitId')
CREATE INDEX IX_Employees_UnitId 
ON Employees (UnitId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Employees_PositionId')
CREATE INDEX IX_Employees_PositionId 
ON Employees (PositionId);

-- 9. Update statistics for query optimizer (SQL Server syntax)
UPDATE STATISTICS RawDataRecords_SCD;
UPDATE STATISTICS Employees;
UPDATE STATISTICS Units;
UPDATE STATISTICS KpiIndicators;
UPDATE STATISTICS UnitKpiScorings;

-- 10. Query to check index usage (SQL Server syntax)
SELECT 
    i.name as IndexName,
    t.name as TableName,
    'N/A' as IndexDefinition -- SQL Server doesn't store index definition in system tables
FROM sys.indexes i
JOIN sys.tables t ON i.object_id = t.object_id
WHERE i.name LIKE 'IX_%'
ORDER BY t.name, i.name;
