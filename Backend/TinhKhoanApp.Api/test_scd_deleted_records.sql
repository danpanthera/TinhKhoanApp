-- Test script for SCD Type 2 with deleted records handling
-- This script demonstrates how the system handles records that are removed from newer imports

-- 1. Check current records in SCD table
SELECT 
    'Current Records Overview' as QueryType,
    DataSource,
    DataType,
    BranchCode,
    COUNT(*) as TotalRecords,
    SUM(CASE WHEN IsCurrent = 1 THEN 1 ELSE 0 END) as CurrentRecords,
    SUM(CASE WHEN IsDeleted = 1 THEN 1 ELSE 0 END) as DeletedRecords,
    MAX(StatementDate) as LatestStatementDate
FROM RawDataRecords_SCD
GROUP BY DataSource, DataType, BranchCode
ORDER BY DataSource, DataType, BranchCode;

-- 2. Check version history for a specific data type and branch
SELECT 
    'Version History Example' as QueryType,
    SourceId,
    VersionNumber,
    IsCurrent,
    IsDeleted,
    ValidFrom,
    ValidTo,
    ProcessingStatus,
    ProcessingNotes
FROM RawDataRecords_SCD
WHERE DataType = 'LN01' 
    AND BranchCode = '7801'
    AND SourceId IN (
        SELECT SourceId 
        FROM RawDataRecords_SCD 
        WHERE DataType = 'LN01' AND BranchCode = '7801'
        GROUP BY SourceId 
        HAVING COUNT(*) > 1
        LIMIT 5
    )
ORDER BY SourceId, VersionNumber;

-- 3. Check deleted records
SELECT 
    'Deleted Records' as QueryType,
    SourceId,
    DataType,
    BranchCode,
    VersionNumber,
    ValidFrom,
    ValidTo,
    ProcessingNotes
FROM RawDataRecords_SCD
WHERE IsDeleted = 1
    AND IsCurrent = 1
ORDER BY ValidFrom DESC
LIMIT 10;

-- 4. Performance check - compare record counts between imports
SELECT 
    'Import Comparison' as QueryType,
    DataType,
    BranchCode,
    DATE(StatementDate) as StatementDay,
    COUNT(*) as RecordsInImport,
    SUM(CASE WHEN IsDeleted = 0 THEN 1 ELSE 0 END) as ActiveRecords,
    SUM(CASE WHEN IsDeleted = 1 THEN 1 ELSE 0 END) as DeletedRecords
FROM RawDataRecords_SCD
WHERE IsCurrent = 1
GROUP BY DataType, BranchCode, DATE(StatementDate)
ORDER BY DataType, BranchCode, StatementDay DESC;

-- 5. Verify SCD Type 2 implementation
SELECT 
    'SCD Type 2 Verification' as QueryType,
    SourceId,
    COUNT(*) as VersionCount,
    MIN(ValidFrom) as FirstVersion,
    MAX(ValidFrom) as LatestVersion,
    SUM(CASE WHEN IsCurrent = 1 THEN 1 ELSE 0 END) as CurrentVersions,
    MAX(CASE WHEN IsCurrent = 1 THEN IsDeleted ELSE 0 END) as IsCurrentlyDeleted
FROM RawDataRecords_SCD
GROUP BY SourceId
HAVING COUNT(*) > 1
ORDER BY VersionCount DESC, SourceId
LIMIT 10;
